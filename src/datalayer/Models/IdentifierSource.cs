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

namespace CodeListHub.DataLayer
{
    /// <summary>
    /// Source information for a general identifier.
    /// </summary>
    [Owned]
    public class IdentifierSource
    {
        /// <summary>
        /// Human-readable name of the source.
        /// </summary>
        [Comment("Human-readable name for the source.")]
        public string LongName { get; set; }

        /// <summary>
        /// Short name of the source.
        /// </summary>
        [Comment("Short name of the source.")]
        public string ShortName { get; set; }

        /// <summary>
        /// More information about the source.
        /// </summary>
        [Comment("More information about the source.")]
        public string Url { get; set; }
    }
}

