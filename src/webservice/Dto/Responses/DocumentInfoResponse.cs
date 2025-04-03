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
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CodeListHub.Dto
{
    /// <summary>
    /// Representation of a document info entry
    /// </summary>
    [SwaggerSchema(ReadOnly = true)]
    public class DocumentInfoResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentInfoResponse"/> class.
        /// </summary>
        /// <param name="documentInfo">The data from the database for assigning</param>
        public DocumentInfoResponse(DocumentInfo documentInfo)
        {
            Language = documentInfo.Language;
            ShortName = documentInfo.ShortName;
            LongName = documentInfo.LongName;
            Type = (DocumentType)documentInfo.DocumentType;
            Version = documentInfo.Version;
            CanonicalUri = documentInfo.CanonicalUri;
            CanonicalVersionUri = documentInfo.CanonicalVersionUri;
            Tags = documentInfo.Tags.Select(x => x.Value).ToArray();
            Publisher = string.IsNullOrEmpty(documentInfo.Publisher?.ShortName) ? null : new PublisherResponse(documentInfo.Publisher);
            PublishedAt = documentInfo.PublishedAt;
            ValidFrom = documentInfo.ValidFrom;
            ValidTo = documentInfo.ValidTo;
        }

        /// <summary>
        /// Canonical URI which uniquely identifies all versions this document (collectively)
        /// </summary>
        [JsonPropertyOrder(6)]
        public string CanonicalUri { get; set; }

        /// <summary>
        /// Canonical URI which uniquely identifies this document
        /// </summary>
        [Required]
        [JsonPropertyOrder(7)]
        public string CanonicalVersionUri { get; set; }

        /// <summary>
        /// The document type
        /// </summary>
        [Required]
        [JsonPropertyOrder(4)]
        public DocumentType Type { get; set; }

        /// <summary>
        /// A list of tags for this document
        /// </summary>
        [JsonPropertyOrder(8)]
        public string[] Tags { get; set; }

        /// <summary>
        /// The language of the document
        /// </summary>
        [Required]
        [JsonPropertyOrder(1)]
        public string Language { get; set; }

        /// <summary>
        /// Human-readable name of this document
        /// </summary>
        [JsonPropertyOrder(3)]
        public string LongName { get; set; }

        /// <summary>
        /// The timepoint of the publication of the document.
        /// </summary>
        [JsonPropertyOrder(9)]
        public DateTimeOffset? PublishedAt { get; set; }

        /// <summary>
        /// The publisher that is responsible for publication and/or maintenance of the codes
        /// </summary>
        [JsonPropertyOrder(12)]
        public PublisherResponse Publisher { get; set; }

        /// <summary>
        /// A short identifier of this document
        /// </summary>
        [Required]
        [JsonPropertyOrder(2)]
        public string ShortName { get; set; }

        /// <summary>
        /// The timepoint from which this document is valid.
        /// </summary>
        [JsonPropertyOrder(10)]
        public DateTimeOffset? ValidFrom { get; set; }

        /// <summary>
        /// The timepoint until which this document is valid.
        /// </summary>
        [JsonPropertyOrder(11)]
        public DateTimeOffset? ValidTo { get; set; }

        /// <summary>
        /// The version of the document
        /// </summary>
        [JsonPropertyOrder(5)]
        public string Version { get; set; }
    }
}
