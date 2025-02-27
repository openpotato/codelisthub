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

using Swashbuckle.AspNetCore.Annotations;

namespace CodeListHub.Dto
{
    /// <summary>
    /// Document types
    /// </summary>
    [SwaggerSchema(ReadOnly = true)]
    public enum DocumentType
    {
        /// <summary>
        /// A code list docuemnt
        /// </summary>
        CodeList,

        /// <summary>
        /// A code list set docuemnt
        /// </summary>
        CodeListSet
    }
}
