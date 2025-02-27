#region CodeListHub - Copyright (C) STÜBER SYSTEMS GmbH
/*    
 *    CodeListHub 
 *    
 *    Copyright (C) STÜBER SYSTEMS GmbH
 *
 *    This program is free software: you can redistribute it and/or modify
 *    it under the terms of the GNU Affero General Public License, version 3,
 *    as published by the Free Software Foundation.
 *
 *    This program is distributed in the hope that it will be useful,
 *    but WITHOUT ANY WARRANTY; without even the implied warranty of
 *    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 *    GNU Affero General Public License for more details.
 *
 *    You should have received a copy of the GNU Affero General Public License
 *    along with this program. If not, see <http://www.gnu.org/licenses/>.
 *
 */
#endregion

using CodeListHub.DataLayer;
using Enbrea.Csv;
using Enbrea.Konsoli;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using OpenCodeList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeListHub.CLI
{
    /// <summary>
    /// Manager for importing raw data to database
    /// </summary>
    public class ImportManager
    {
        private readonly AppConfiguration _appConfiguration;
        private readonly ConsoleWriter _consoleWriter;
        private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportManager"/> class.
        /// </summary>
        /// <param name="appConfiguration">Configuration data</param>
        public ImportManager(AppConfiguration appConfiguration)
        {
            _appConfiguration = appConfiguration;
            _dbContextFactory = new PooledDbContextFactory<AppDbContext>(AppDbContextOptionsFactory.CreateDbContextOptions(_appConfiguration.Database));
            _consoleWriter = ConsoleWriterFactory.CreateConsoleWriter(ProgressUnit.Count);
        }

        /// <summary>
        /// Executes the data import
        /// </summary>
        /// <param name="cancellationToken">A cancellation token</param>
        /// <returns>A task that represents the asynchronous import operation.</returns>
        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                using var dbContext = _dbContextFactory.CreateDbContext();

                // Start...
                _consoleWriter.Caption($"Import data");

                // Get base import folder
                var importFolder = new DirectoryInfo(_appConfiguration.FileDepot.RootFolderName);

                // Import data
                await ImportFolder(dbContext, importFolder, cancellationToken);

            }
            catch (Exception ex)
            {
                _consoleWriter.NewLine();
                _consoleWriter.Error($"Import failed. {ex.Message}");
                throw;
            }
        }

        private async Task ImportDocument(AppDbContext dbContext, CodeListDocument oclDocument, string oclMetaFilePath, CancellationToken cancellationToken)
        {
            if (oclDocument.MetaOnly)
            {
                var csvFilePath = PathUtils.ChangeExtensionWithDoublePeriods(oclMetaFilePath, ".csv");
                var oclFilePath = PathUtils.ChangeExtensionWithDoublePeriods(oclMetaFilePath, ".ocl");

                // Open CSV file stream
                using var strReader = new StreamReader(csvFilePath);

                // Create CSV reader
                var csvTableReader = new CsvTableReader(strReader, new CsvConfiguration { Separator = ',' });

                // Read header line from CSV file
                await csvTableReader.ReadHeadersAsync();

                // Read line by line from CSV file
                while (await csvTableReader.ReadAsync() > 0)
                {
                    var row = oclDocument.Rows.Add();

                    foreach (var column in oclDocument.Columns)
                    {
                        row[column.Id] = csvTableReader[column.Id];
                    }
                }

                // Save code list document 
                await oclDocument.SaveAsync(oclFilePath, cancellationToken);

                // Import meta info to databse
                await ImportToDatabase(dbContext, oclDocument, oclMetaFilePath, oclFilePath, csvFilePath, cancellationToken);
            }
        }

        private async Task ImportDocument(AppDbContext dbContext, CodeListSetDocument oclDocument, string oclFilePath, CancellationToken cancellationToken)
        {
            if (!oclDocument.MetaOnly)
            {
                var csvFilePath = Path.ChangeExtension(oclFilePath, ".csv");
                var oclMetaFilePath = Path.ChangeExtension(oclFilePath, ".meta.ocl");

                // Save code list set document as meta document
                await oclDocument.SaveAsMetaOnlyAsync(oclMetaFilePath, cancellationToken);

                // Create CSV file stream
                using var strWriter = new StreamWriter(csvFilePath);

                // Create CSV writer
                var csvTableWriter = new CsvTableWriter(strWriter, new CsvConfiguration { Separator = ',' });

                // Write header line to CSV file
                await csvTableWriter.WriteHeadersAsync(
                    PropertyNames.CanonicalUri,
                    PropertyNames.CanonicalVersionUri,
                    PropertyNames.LocationUrls);

                // Write data rows to CSV file
                foreach (var documentRef in oclDocument.DocumentRefs)
                {
                    csvTableWriter.SetValue(PropertyNames.CanonicalUri, documentRef.CanonicalUri);
                    csvTableWriter.SetValue(PropertyNames.CanonicalVersionUri, documentRef.CanonicalVersionUri);
                    csvTableWriter.SetValue(PropertyNames.LocationUrls, documentRef.LocationUrls[0]);
                    await csvTableWriter.WriteAsync();
                }

                // Import meta info to databse
                await ImportToDatabase(dbContext, oclDocument, oclMetaFilePath, oclFilePath, csvFilePath, cancellationToken);
            }
        }

        private async Task ImportFolder(AppDbContext dbContext, DirectoryInfo oclfolder, CancellationToken cancellationToken)
        {
            // Recursively iterate over files
            _consoleWriter.StartProgress($"Process folder \"{oclfolder.Name}\"...");

            var fileCount = 0;

            foreach (var oclFile in oclfolder.GetFiles("*.ocl"))
            {
                var oclDocument = await DocumentLoader.LoadAsync(oclFile, cancellationToken);

                if (oclDocument is CodeListDocument codeListDocument)
                {
                    await ImportDocument(dbContext, codeListDocument, oclFile.FullName, cancellationToken);
                }
                else if (oclDocument is CodeListSetDocument codeListSetDocument)
                {
                    await ImportDocument(dbContext, codeListSetDocument, oclFile.FullName, cancellationToken);
                }

                _consoleWriter.ContinueProgress(++fileCount);
            }

            _consoleWriter.FinishProgress(fileCount);

            // Recursively iterate over sub folders
            foreach (var subFolder in oclfolder.GetDirectories())
            {
                await ImportFolder(dbContext, subFolder, cancellationToken);
            }
        }

        private async Task ImportToDatabase(
            AppDbContext dbContext, 
            Document oclDocument,
            string oclMetaFilePath,
            string oclFilePath,
            string csvFilePath, 
            CancellationToken cancellationToken)
        {
            await dbContext.Set<DocumentInfo>().AddAsync(new DocumentInfo()
            {
                DocumentType = (oclDocument is CodeListDocument) ? DocumentType.CodeList : DocumentType.CodeListSet,
                Language = oclDocument.Identification.Language,
                ShortName = oclDocument.Identification.ShortName,
                LongName = oclDocument.Identification.LongName,
                CanonicalUri = oclDocument.Identification.CanonicalUri,
                CanonicalVersionUri = oclDocument.Identification.CanonicalVersionUri,
                Version = oclDocument.Identification.Version,
                Tags = await GetTags(dbContext, oclDocument, cancellationToken),
                PublishedAt = oclDocument.Identification.PublishedAt?.ToUniversalTime(),
                Publisher = await GetPublisher(dbContext, oclDocument, cancellationToken),
                ValidFrom = oclDocument.Identification.ValidFrom?.ToUniversalTime(),
                ValidTo = oclDocument.Identification.ValidTo?.ToUniversalTime(),
                Files =
                [
                    new DocumentFile()
                    {
                        MediaType = "text/json",
                        MetaOnly = false,
                        FilePath = oclFilePath
                    },
                    new DocumentFile()
                    {
                        MediaType = "text/json",
                        MetaOnly = true,
                        FilePath = oclMetaFilePath
                    },
                    new DocumentFile()
                    {
                        MediaType = "text/csv",
                        MetaOnly = false,
                        FilePath = csvFilePath
                    }
                ]
            }, cancellationToken);

            await dbContext.SaveChangesAsync(cancellationToken);
        }

        private async Task<IList<Tag>> GetTags(AppDbContext dbContext, Document oclDocument, CancellationToken cancellationToken)
        {
            var tagList = new List<Tag>();

            foreach (var tagValue in oclDocument.Identification.Tags)
            {
                var tag = await dbContext.Set<Tag>()
                    .Where(x => x.Value == tagValue)
                    .SingleOrDefaultAsync(cancellationToken);

                if (tag == null)
                {
                    tag = new Tag()
                    {
                        Value = tagValue
                    };

                    await dbContext.Set<Tag>().AddAsync(tag, cancellationToken);
                }

                tagList.Add(tag);
            }

            return tagList;
        }

        private async Task<DataLayer.Publisher> GetPublisher(AppDbContext dbContext, Document oclDocument, CancellationToken cancellationToken)
        {
            var publisherName = oclDocument.Identification.Publisher?.ShortName;

            if (publisherName != null)
            {
                var publisher = await dbContext.Set<DataLayer.Publisher>()
                    .Where(x => x.ShortName == publisherName)
                    .SingleOrDefaultAsync(cancellationToken);

                if (publisher == null)
                {
                    publisher = new DataLayer.Publisher()
                    {
                        Identifier = new DataLayer.Identifier()
                        {
                            Value = oclDocument.Identification.Publisher?.Identifier?.Value,
                            Source = new DataLayer.IdentifierSource()
                            {
                                ShortName = oclDocument.Identification.Publisher?.Identifier?.Source?.ShortName,
                                LongName = oclDocument.Identification.Publisher?.Identifier?.Source?.LongName,
                                Url = oclDocument.Identification.Publisher?.Identifier?.Source?.Url.ToString()
                            },
                        },
                        ShortName = oclDocument.Identification.Publisher?.ShortName,
                        LongName = oclDocument.Identification.Publisher?.LongName,
                        Url = oclDocument.Identification.Publisher?.Url.ToString()
                    };

                    await dbContext.Set<DataLayer.Publisher>().AddAsync(publisher, cancellationToken);
                }

                return publisher;
            }
            else
            {
                throw new Exception("No publisher definied");
            }
        }
    }
}
