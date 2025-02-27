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
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;

namespace CodeListHub.DataLayer
{
    /// <summary>
    /// A file representation of a code list document or a code list set document
    /// </summary>
    [Table(DbTables.DocumentFile)]
    [Comment("File representations of code list documents or code list set documents")]
    public class DocumentFile : BaseEntity
    {
        /// <summary>
        /// The file path
        /// </summary>
        [Required]
        [Comment("The file path")]
        public string FilePath { get; set; }

        /// <summary>
        /// Just metadata without data?
        /// </summary>
        [Comment("Just metadata without data?")]
        public bool MetaOnly { get; set; }

        /// <summary>
        /// Media type of the file
        /// </summary>
        [Comment("Media type of the file")]
        public string MediaType { get; set; }

        #region Foreign keys
        [Comment("Reference to DocumentInfo")]
        public Guid DocumentInfoId { get; set; }
        #endregion Foreign keys
    }
}

