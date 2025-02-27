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
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeListHub.DataLayer
{
    /// <summary>
    /// The publisher responsible for publishing and/or maintaining the codes
    /// </summary>
    [Table(DbTables.Publisher)]
    [Index(nameof(ShortName), IsUnique = true)]
    [Comment("Publishers responsible for publishing and/or maintaining the codes")]
    public class Publisher : BaseEntity
    {
        /// <summary>
        /// Identifier for the publisher.
        /// </summary>
        [Required]
        public Identifier Identifier { get; set; }

        /// <summary>
        /// Human-readable name for the publisher.
        /// </summary>
        [Comment("Human-readable name for the publisher")]
        public string LongName { get; set; }

        /// <summary>
        /// Short name for the publisher.
        /// </summary>
        [Comment("Short name for the publisher")]
        public string ShortName { get; set; }

        /// <summary>
        /// Url with further information.
        /// </summary>
        [Comment("Url with further information")]
        public string Url { get; set; }
    }
}

