using System;
using System.Collections;
using System.Collections.Generic;

namespace BinaryFormatDataStructure
{
    public class BinaryObject: IReadOnlyDictionary<string, object>
    {
        private readonly Dictionary<string, object> _members = new Dictionary<string, object>();
        
        public string TypeName { get; internal set; }
        public string AssemblyName { get; internal set; }
        public object ReferenceObject { get; set; }

        public object this[string memberName] 
        {
            get
            {
                if (_members.TryGetValue(memberName, out object value))
                    return value;

                return null;
            }
        }

        public int Count => _members.Count;

        public IEnumerable<string> Keys => _members.Keys;

        public IEnumerable<object> Values => throw new NotSupportedException();

        public bool ContainsKey(string memberName)
        {
            return _members.ContainsKey(memberName);
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _members.GetEnumerator();
        }

        public bool TryGetValue(string memberName, out object value)
        {
            return _members.TryGetValue(memberName, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _members.GetEnumerator();
        }

        internal void AddMember(string memberName, object value)
        {
            _members[memberName] = value;
        }
    }
}
