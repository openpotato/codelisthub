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

using Asp.Versioning;
using CodeListHub.DataLayer;
using CodeListHub.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace CodeListHub
{
    /// <summary>
    /// API controller for exploring and accessing code lists and code list sets
    /// </summary>
    [ApiVersion(1)]
    [Route("v{v:apiVersion}/documents")]
    [SwaggerTag("Explore and access code lists and code list sets")]
    public class DocumentController : BaseController
    {
        private readonly string[] _jsonMediaTypeNames = [MediaTypeNames.Application.Json, MediaTypeNames.Text.Json, MediaTypeNames.Text.Plain];
        private readonly string[] _defaultLanguages = ["de", "en"];

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentController"/> class.
        /// </summary>
        /// <param name="dbContext">Injected database context</param>
        public DocumentController(AppDbContext dbContext)
            : base(dbContext)
        {
        }

        /// <summary>
        /// Returns the content of a code list document or a code list set document in a format other than OpenCodeList.
        /// </summary>
        /// <param name="canonicalUri" example="urn:codelisthub:iso:countries">A canonical URI which uniquely identifies the version of the document.</param>
        /// <param name="language" example="de">The language of the document.</param>
        /// <param name="cancellationToken">A cancellation token</param>
        /// <returns>The document file in the requested alternative format</returns>
        [HttpGet("{canonicalUri}/alternative-format")]
        [ProducesResponseType(typeof(FileResult), statusCode: StatusCodes.Status200OK, MediaTypeNames.Text.Csv)]
        [ProducesResponseType(typeof(ProblemDetails), statusCode: 400, MediaTypeNames.Application.ProblemDetails)]
        [ProducesResponseType(typeof(ProblemDetails), statusCode: 500, MediaTypeNames.Application.ProblemDetails)]
        public async Task<IActionResult> GetAlternativeFormatAsync(
            [FromRoute, Required] Uri canonicalUri,
            [FromQuery] string language,
            CancellationToken cancellationToken = default)
        {
            // Get list of documents with given canonicalVersionUri
            var documentInfoList = await _dbContext.Set<DocumentInfo>()
                .Include(x => x.Files)
                .Where(x => x.CanonicalUri == canonicalUri.ToString() || x.CanonicalVersionUri == canonicalUri.ToString())
                .OrderByDescending(x => x.Version)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            // Found something?
            if (documentInfoList.Count > 0)
            {
                // Get language quality values from request query and headers
                StringValues languageValues;

                if (Request.Headers.TryGetValue(HeaderNames.AcceptLanguage, out var languageStrings))
                {
                    languageValues = StringValues.Concat(language, languageStrings);
                }
                else
                {
                    languageValues = StringValues.Concat(language, new StringValues(_defaultLanguages));
                }

                // Try to find document instance by those language quality values
                if (TryFindDocumentInfoByLanguage(documentInfoList, languageValues, out var documentInfo))
                {
                    // Get content type quality values from request headers
                    if (Request.Headers.TryGetValue(HeaderNames.Accept, out var contentTypeStrings))
                    {
                        // Try to find document file by content type quality values
                        if (TryFindDocumentFileByContentType(documentInfo.Files, contentTypeStrings, out var documentFile))
                        {
                            return new FileStreamResult(System.IO.File.OpenRead(documentFile.FilePath), documentFile.MediaType);
                        }
                        else
                        {
                            return Problem(statusCode: StatusCodes.Status404NotFound, detail: $"Requested representation for {canonicalUri} not available.");
                        }
                    }
                    else
                    {
                        return Problem(statusCode: StatusCodes.Status404NotFound, detail: $"HTTP header {HeaderNames.Accept} not present.");
                    }
                }
                else
                {
                    return Problem(statusCode: StatusCodes.Status406NotAcceptable, detail: $"Document for {canonicalUri} not available in the requested language(s).");
                }
            }
            else
            {
                return Problem(statusCode: StatusCodes.Status404NotFound, detail: $"Document for {canonicalUri} not found.");
            }
        }

        /// <summary>
        /// Returns a code list document or a code list set document as OpenCodeList format
        /// </summary>
        /// <param name="canonicalUri" example="urn:codelisthub:iso:countries">A canonical URI which uniquely identifies the version of the document.</param>
        /// <param name="language" example="de">The language of the document.</param>
        /// <param name="metaOnly">TRUE, only a meta document is returned</param>
        /// <param name="cancellationToken">A cancellation token</param>
        /// <returns>The document file in OpenCodeList format</returns>
        [HttpGet("{canonicalUri}")]
        [ProducesResponseType(typeof(FileResult), statusCode: StatusCodes.Status200OK, MediaTypeNames.Application.Json, MediaTypeNames.Text.Json, MediaTypeNames.Text.Plain)]
        [ProducesResponseType(typeof(ProblemDetails), statusCode: 400, MediaTypeNames.Application.ProblemDetails)]
        [ProducesResponseType(typeof(ProblemDetails), statusCode: 500, MediaTypeNames.Application.ProblemDetails)]
        public async Task<IActionResult> GetDocumentAsync(
            [FromRoute, Required] Uri canonicalUri,
            [FromQuery] string language,
            [FromQuery] bool metaOnly = false,
            CancellationToken cancellationToken = default)
        {
            // Get list of documents with given canonicalVersionUri
            var documentInfoList = await _dbContext.Set<DocumentInfo>()
                .Include(x => x.Files)
                .Where(x => x.CanonicalUri == canonicalUri.ToString() || x.CanonicalVersionUri == canonicalUri.ToString())
                .OrderByDescending(x => x.Version)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            // Found something?
            if (documentInfoList.Count > 0)
            {
                // Get language quality values from request query and headers
                StringValues languageValues;

                if (Request.Headers.TryGetValue(HeaderNames.AcceptLanguage, out var languageStrings))
                {
                    languageValues = StringValues.Concat(language, languageStrings);
                }
                else
                {
                    languageValues = StringValues.Concat(language, new StringValues(_defaultLanguages));  
                }

                // Try to find document instance by those language quality values
                if (TryFindDocumentInfoByLanguage(documentInfoList, languageValues, out var documentInfo))
                {
                    // Get document file with a JSON content type
                    var documentFile = documentInfo.Files.SingleOrDefault(x => _jsonMediaTypeNames.Any(x.MediaType.Contains) && x.MetaOnly == metaOnly);

                    // Found something?
                    if (documentFile != null)
                    {
                        // Get content type quality values from request headers
                        StringValues contentTypeValues;

                        if (Request.Headers.TryGetValue(HeaderNames.Accept, out var contentTypeStrings))
                        {
                            contentTypeValues = StringValues.Concat(contentTypeStrings, MediaTypeNames.Application.Json);
                        }
                        else
                        {
                            contentTypeValues = new StringValues(MediaTypeNames.Application.Json);
                        }

                        // Check if at least one content type quality value matches
                        if (TryFindContentType(_jsonMediaTypeNames, contentTypeValues, out var contentType))
                        {
                            return new FileStreamResult(System.IO.File.OpenRead(documentFile.FilePath), contentType);
                        }
                        else
                        {
                            return Problem(statusCode: StatusCodes.Status415UnsupportedMediaType, detail: $"None of the requested content types are supported.");
                        }
                    }
                    else
                    {
                        return Problem(statusCode: StatusCodes.Status404NotFound, detail: $"Requested representation for {canonicalUri} not available.");
                    }
                }
                else
                {
                    return Problem(statusCode: StatusCodes.Status406NotAcceptable, detail: $"Document for {canonicalUri} not available in the requested language(s).");
                }
            }
            else
            {
                return Problem(statusCode: StatusCodes.Status404NotFound, detail: $"Document for {canonicalUri} not found.");
            }
        }

        /// <summary>
        /// Returns the index of available code list documents and code list set documents
        /// </summary>
        /// <param name="type">Document type filter</param>
        /// <param name="language">Language filter</param>
        /// <param name="searchTerm">Search term as regular expression</param>
        /// <param name="tags">Tags filter</param>
        /// <param name="publishedFrom">Filter for timepoint of publication (from value)</param>
        /// <param name="publishedUntil">Filter for timepoint of publication (to value)</param>
        /// <param name="page">Page number (starting with 1)</param>
        /// <param name="pageSize">Page size (maximum 50)</param>
        /// <param name="cancellationToken">A cancellation token</param>
        /// <returns>A list of document entries</returns>
        [HttpGet("index")]
        [ProducesResponseType(typeof(IEnumerable<DocumentInfoResponse>), statusCode: StatusCodes.Status200OK, MediaTypeNames.Application.Json, MediaTypeNames.Text.Json, MediaTypeNames.Text.Plain)]
        [ProducesResponseType(typeof(ProblemDetails), statusCode: 400, MediaTypeNames.Application.ProblemDetails)]
        [ProducesResponseType(typeof(ProblemDetails), statusCode: 500, MediaTypeNames.Application.ProblemDetails)]
        [PaginationFilter]
        public async Task<IActionResult> GetDocumentIndexAsync(
            [FromQuery] string language,
            [FromQuery] string searchTerm,
            [FromQuery] string[] tags,
            [FromQuery] Dto.DocumentType? type,
            [FromQuery] DateTimeOffset? publishedFrom,
            [FromQuery] DateTimeOffset? publishedUntil,
            [FromQuery, Range(1, int.MaxValue)] int page = 1,
            [FromQuery, Range(1, 50)] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            return Ok(
                await _dbContext.Set<DocumentInfo>()
                    .Include(x => x.Publisher).ThenInclude(x => x.Identifier)
                    .Include(x => x.Tags)
                    .Where(
                        x => string.IsNullOrEmpty(searchTerm) ||
                        Regex.IsMatch(x.ShortName, searchTerm, RegexOptions.IgnoreCase) ||
                        Regex.IsMatch(x.LongName, searchTerm, RegexOptions.IgnoreCase) ||
                        Regex.IsMatch(x.Publisher.ShortName, searchTerm, RegexOptions.IgnoreCase) ||
                        Regex.IsMatch(x.Publisher.LongName, searchTerm, RegexOptions.IgnoreCase) ||
                        Regex.IsMatch(x.CanonicalUri, searchTerm, RegexOptions.IgnoreCase) ||
                        Regex.IsMatch(x.CanonicalVersionUri, searchTerm, RegexOptions.IgnoreCase)
                    )
                    .Where(x => string.IsNullOrEmpty(language) || x.Language == language)
                    .Where(x => type == null || x.DocumentType == (DataLayer.DocumentType)type)
                    .Where(x => tags.Length == 0 || x.Tags.Any(t => tags.Contains(t.Value)))
                    .Where(x => publishedFrom == null || x.PublishedAt >= publishedFrom)
                    .Where(x => publishedUntil == null || x.PublishedAt <= publishedUntil)
                    .OrderBy(x => x.ShortName).ThenBy(x => x.Version).ThenBy(x => x.Language)
                    .Select(x => new DocumentInfoResponse(x))
                    .AsNoTracking()
                    .ToPageAsync(page, pageSize, cancellationToken)
            );
        }

        /// <summary>
        /// Returns the index of available code list documents and code list set documents for a given canonical Uri
        /// </summary>
        /// <param name="language">Language filter</param>
        /// <param name="page">Page number (starting with 1)</param>
        /// <param name="pageSize">Page size (maximum 50)</param>
        /// <param name="cancellationToken">A cancellation token</param>
        /// <returns>A list of document entries</returns>
        [HttpGet("index/{canonicalUri}")]
        [ProducesResponseType(typeof(IEnumerable<DocumentInfoResponse>), statusCode: StatusCodes.Status200OK, MediaTypeNames.Application.Json, MediaTypeNames.Text.Json, MediaTypeNames.Text.Plain)]
        [ProducesResponseType(typeof(ProblemDetails), statusCode: 400, MediaTypeNames.Application.ProblemDetails)]
        [ProducesResponseType(typeof(ProblemDetails), statusCode: 500, MediaTypeNames.Application.ProblemDetails)]
        [PaginationFilter]
        public async Task<IActionResult> GetDocumentIndexByCanonicalUriAsync(
            [FromQuery] string language,
            [FromQuery, Range(1, int.MaxValue)] int page = 1,
            [FromQuery, Range(1, 50)] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            return Ok(
                await _dbContext.Set<DocumentInfo>()
                    .Include(x => x.Publisher).ThenInclude(x => x.Identifier)
                    .Include(x => x.Tags)
                    .Where(x => string.IsNullOrEmpty(language) || x.Language == language)
                    .OrderBy(x => x.ShortName).ThenBy(x => x.Version).ThenBy(x => x.Language)
                    .Select(x => new DocumentInfoResponse(x))
                    .AsNoTracking()
                    .ToPageAsync(page, pageSize, cancellationToken)
            );
        }

        /// <summary>
        /// Returns the list of available tags
        /// </summary>
        /// <param name="page">Page number (starting with 1)</param>
        /// <param name="pageSize">Page size (maximum 50)</param>
        /// <param name="cancellationToken">A cancellation token</param>
        /// <returns>List of tags</returns>
        [HttpGet("tags")]
        [ProducesResponseType(typeof(IEnumerable<string>), statusCode: StatusCodes.Status200OK, MediaTypeNames.Application.Json, MediaTypeNames.Text.Json, MediaTypeNames.Text.Plain)]
        [ProducesResponseType(typeof(ProblemDetails), statusCode: 400, MediaTypeNames.Application.ProblemDetails)]
        [ProducesResponseType(typeof(ProblemDetails), statusCode: 500, MediaTypeNames.Application.ProblemDetails)]
        [PaginationFilter]
        public async Task<IActionResult> GetTagsAsync(
            [FromQuery, Range(1, int.MaxValue)] int page = 1,
            [FromQuery, Range(1, 50)] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            return Ok(
                await _dbContext.Set<Tag>()
                    .OrderBy(x => x.Value)
                    .Select(x => x.Value)
                    .AsNoTracking()
                    .ToPageAsync(page, pageSize, cancellationToken));
        }

        /// <summary>
        /// Tries to find a content type from a given list of possible content types by comparing it with qualified content type values.
        /// </summary>
        /// <param name="contentTypeCandidates">The list of possible content types</param>
        /// <param name="contentTypeValues">The qualified content type values</param>
        /// <param name="contentType">The found content type</param>
        /// <returns>TRUE, if found</returns>
        private static bool TryFindContentType(string[] contentTypeCandidates, StringValues contentTypeValues, out string contentType)
        {
            var languageQualityValues = new QualityValueList(contentTypeValues);

            foreach (var languageQualityValue in languageQualityValues)
            {
                contentType = contentTypeCandidates.FirstOrDefault(x => x == languageQualityValue.Name);

                if (contentType != null)
                {
                    return true;
                }
            }
            contentType = null;
            return false;
        }

        /// <summary>
        /// Tries to find a document file from a given list of document files by comparing qualified content type values with the media type property 
        /// of each document file in the list
        /// </summary>
        /// <param name="documentFileList">The document file list</param>
        /// <param name="contentTypeValues">The qualified content type values</param>
        /// <param name="documentFile">The found document file</param>
        /// <returns>TRUE, if found</returns>
        private static bool TryFindDocumentFileByContentType(IList<DocumentFile> documentFileList, StringValues contentTypeValues, out DocumentFile documentFile)
        {
            var contentTypeQualityValues = new QualityValueList(contentTypeValues);

            foreach (var contentTypeQualityValue in contentTypeQualityValues)
            {
                documentFile = documentFileList.FirstOrDefault(x => x.MediaType == contentTypeQualityValue.Name);

                if (documentFile != null)
                {
                    return true;
                }
            }
            documentFile = null;
            return false;
        }

        /// <summary>
        /// Tries to find a document from a given list of documents by comparing qualified language values with the language property of each 
        /// document in the list
        /// </summary>
        /// <param name="documentInfolist">The document list</param>
        /// <param name="languageValues">The qualified language values</param>
        /// <param name="documentInfo">The found document</param>
        /// <returns>TRUE, if found</returns>
        private static bool TryFindDocumentInfoByLanguage(IList<DocumentInfo> documentInfolist, StringValues languageValues, out DocumentInfo documentInfo)
        {
            var languageQualityValues = new QualityValueList(languageValues);

            foreach (var languageQualityValue in languageQualityValues)
            {
                documentInfo = documentInfolist.FirstOrDefault(x => languageQualityValue.Name.StartsWith(x.Language) || x.Language.StartsWith(languageQualityValue.Name));

                if (documentInfo != null)
                {
                    return true;
                }
            }
            documentInfo = null;
            return false;
        }
    }
}
