using System;
using UnityEngine;

namespace Calluna
{
    public class StringId : ScriptableObject, IEquatable<StringId>
    {
        [field: SerializeField] public string Id { get; private set; }

        public bool Equals(StringId other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) && Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((StringId)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), Id);
        }

        public override string ToString()
        {
            return Id;
        }
    }
}
