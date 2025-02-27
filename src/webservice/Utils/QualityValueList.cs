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

using Microsoft.Extensions.Primitives;

namespace CodeListHub
{
    /// <summary>
    /// Provides a collection for working with http header quality values
    /// </summary>
    /// <remarks>
    /// See RFC 9110, Quality Values: https://www.rfc-editor.org/rfc/rfc9110.html#name-quality-values
    /// Adapted from https://github.com/fatso83/QValues
    /// </remarks>
    public sealed class QualityValueList : List<QualityValue>
    {
        private static readonly char[] delimiters = { ',' };
        private readonly bool _acceptWildcard;
        private bool _autoSort;

        /// <summary>
        /// Creates a new <see cref="QualityValueList"/> instance from a given string of 
        /// comma delimited quality values
        /// </summary>
        /// <param name="values">The raw string of quality values to load</param>
        public QualityValueList(string values)
            : this(values == null ? [] : values.Split(delimiters, StringSplitOptions.RemoveEmptyEntries))
        { }

        /// <summary>
        /// Creates a new <see cref="QualityValueList"/> instance from a given 
        /// <see cref="StringValues"/> struct of quality values
        /// </summary>
        /// <param name="values">The <see cref="StringValues"/> struct to load</param>
        public QualityValueList(StringValues values)
            : this(values.ToArray())
        { }

        /// <summary>
        /// Creates a new <see cref="QualityValueList"/> instance from a given string array 
        /// of quality values
        /// </summary>
        /// <param name="values">The array of quality value strings</param>
        public QualityValueList(string[] values)
        {
            int ordinal = -1;
            foreach (var value in values)
            {
                var qvalue = QualityValue.Parse(value.Trim(), ++ordinal);
                if (qvalue.Name.Equals("*") || qvalue.Name.Equals("*/*"))
                {
                    if (qvalue.CanAccept)
                    {
                        _acceptWildcard = true;
                    }
                }
                Add(qvalue);
            }
            DefaultSort();
            _autoSort = true;
        }

        /// <summary>
        /// Whether or not the wildcarded encoding is available and allowed
        /// </summary>
        public bool AcceptWildcard
        {
            get 
            { 
                return _acceptWildcard; 
            }
        }

        /// <summary>
        /// Whether, after an add operation, the list should be resorted
        /// </summary>
        public bool AutoSort
        {
            get 
            { 
                return _autoSort; 
            }
            set 
            { 
                _autoSort = value; 
            }
        }

        /// <summary>
        /// Adds a <see cref="QualityValue"/> item to the list, then applies sorting 
        /// if AutoSort is enabled.
        /// </summary>
        /// <param name="item">The item to add</param>
        public new void Add(QualityValue item)
        {
            base.Add(item);
            ApplyAutoSort();
        }

        /// <summary>
        /// Adds a range of <see cref="QualityValue"/> items to the list, then applies sorting 
        /// if AutoSort is enabled.
        /// </summary>
        /// <param name="collection">The items to add</param>
        public new void AddRange(IEnumerable<QualityValue> collection)
        {
            var state = _autoSort;
            _autoSort = false;
            base.AddRange(collection);
            _autoSort = state;
            ApplyAutoSort();
        }

        /// <summary>
        /// Sorts the list comparing by weight in descending order
        /// </summary>
        public void DefaultSort()
        {
            Sort(QualityValue.CompareByWeightDesc);
        }

        /// <summary>
        /// Finds the first <see cref="QualityValue"/> item with the given name (case-insensitive)
        /// </summary>
        /// <param name="name">The name of the <see cref="QualityValue"/> item to search for</param>
        /// <returns>The first <see cref="QualityValue"/> item to be found, otherwise null</returns>
        public QualityValue Find(string name)
        {
            return Find(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Finds the first <see cref="QualityValue"/> item that matches one of the specified candidates
        /// </summary>
        /// <remarks>
        /// Loops from the first item in the list to the last and finds the first candidate. The list must 
        /// be sorted for weight prior to calling this method.
        /// </remarks>
        /// <param name="candidates">The list of quality value names to find</param>
        /// <returns>The first <see cref="QualityValue"/> item to be found, otherwise null</returns>
        public QualityValue FindHighestWeight(params string[] candidates)
        {
            return Find(x => candidates.Any(s => x.Name.Equals(s, StringComparison.OrdinalIgnoreCase)));
        }

        /// <summary>
        /// Finds the first <see cref="QualityValue"/> item that matches one of the specified candidates.
        /// </summary>
        /// <remarks>
        /// Loops from the first item in the list to the last and finds the first candidate that can be 
        /// accepted. The list must be sorted for weight prior to calling this method.
        /// </remarks>
        /// <param name="candidates">The list of quality value names to find</param>
        /// <returns>The first <see cref="QualityValue"/> item to be found, otherwise null</returns>
        public QualityValue FindPreferred(params string[] candidates)
        {
            return Find(x => candidates.Any(s => x.Name.Equals(s, StringComparison.OrdinalIgnoreCase)) && x.CanAccept);
        }

        /// <summary>
        /// Applies the default sorting method if the AutoSort field is currently enabled.
        /// </summary>
        private void ApplyAutoSort()
        {
            if (_autoSort)
            {
                DefaultSort();
            }
        }
    }
}