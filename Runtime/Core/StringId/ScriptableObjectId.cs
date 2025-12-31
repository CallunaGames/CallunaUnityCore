using System;
using UnityEngine;

namespace Calluna
{
    public abstract class ScriptableObjectId : ScriptableObject, IEquatable<ScriptableObjectId>
    {
        [field: SerializeField] public string Descriptor { get; private set; }

        public bool Equals(ScriptableObjectId other)
        {
            return ReferenceEquals(this, other);
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(ScriptableObjectId left, ScriptableObjectId right)
        {
            return left?.Equals(right) ?? ReferenceEquals(null, right);
        }

        public static bool operator !=(ScriptableObjectId left, ScriptableObjectId right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            return Descriptor;
        }
    }
}
