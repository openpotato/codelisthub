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

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeListHub.DataLayer
{
    /// <summary>
    /// An index entry for a code list document or a code list set document
    /// </summary>
    [Table(DbTables.DocumentInfo)]
    [Index(nameof(Language), nameof(CanonicalVersionUri), IsUnique = true)]
    [Index(nameof(ShortName), IsUnique = false)]
    [Comment("Index entries for code list documents and code list set documents")]
    public class DocumentInfo : BaseEntity
    {
        /// <summary>
        /// Canonical URI which uniquely identifies all versions this document (collectively)
        /// </summary>
        [Comment("Canonical URI which uniquely identifies all versions this document (collectively)")]
        public Uri CanonicalUri { get; set; }

        /// <summary>
        /// Canonical URI which uniquely identifies this document
        /// </summary>
        [Required]
        [Comment("Canonical URI which uniquely identifies this document")]
        public Uri CanonicalVersionUri { get; set; }

        /// <summary>
        /// The document type
        /// </summary>
        [Required]
        [Comment("The document type")]
        public DocumentType DocumentType { get; set; }

        /// <summary>
        /// The path to a csv representation of the OpenCodeList document file
        /// </summary>
        [Comment("The list of files, each a different representation of the OpenCodeList document file")]
        public virtual IList<DocumentFile> Files { get; set; }

        /// <summary>
        /// A short identifier of this document
        /// </summary>
        [Required]
        [Comment("A short identifier of this document")]
        public string Language { get; set; }

        /// <summary>
        /// Human-readable name of this document
        /// </summary>
        [Comment("Human-readable name of this document")]
        public string LongName { get; set; }

        /// <summary>
        /// The timepoint of the publication of the document.
        /// </summary>
        [Comment("The timepoint of the publication of the document.")]
        public DateTimeOffset? PublishedAt { get; set; }

        /// <summary>
        /// The publisher that is responsible for publication and/or maintenance of this document
        /// </summary>
        [Comment("The publisher that is responsible for publication and/or maintenance of this document.")]
        public virtual Publisher Publisher { get; set; }

        /// <summary>
        /// A short identifier of this document
        /// </summary>
        [Required]
        [Comment("A short identifier of this document")]
        public string ShortName { get; set; }

        /// <summary>
        /// A list of tags for this document
        /// </summary>
        [Comment("A list of tags for this document")]
        public virtual IList<Tag> Tags { get; set; }

        /// <summary>
        /// The timepoint from which this document is valid.
        /// </summary>
        [Comment("The timepoint from which this document is valid.")]
        public DateTimeOffset? ValidFrom { get; set; }

        /// <summary>
        /// The timepoint until which this document is valid.
        /// </summary>
        [Comment("The timepoint until which this document is valid.")]
        public DateTimeOffset? ValidTo { get; set; }

        /// <summary>
        /// The version of the document
        /// </summary>
        [Comment("The version of the document")]
        public string Version { get; set; }

        #region Foreign keys
        [Comment("Reference to publisher")]
        public Guid PublisherId { get; set; }
        #endregion Foreign keys
    }
}
