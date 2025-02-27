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
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeListHub.DataLayer
{
    /// <summary>
    /// A tag
    /// </summary>
    [Table(DbTables.Tag)]
    [Index(nameof(Value), IsUnique = true)]
    [Comment("Tags of documents")]
    public class Tag : BaseEntity
    {
        /// <summary>
        /// The tag value.
        /// </summary>
        [Comment("The tag value.")]
        public string Value { get; set; }

        /// <summary>
        /// List of document indices which have this tag assigned
        /// </summary>
        [Comment("List of document indices which have this tag assigned.")]
        public virtual IList<DocumentInfo> DocumentInfos { get; set; }
    }
}

