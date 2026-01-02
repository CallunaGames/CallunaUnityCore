using System;
using UnityEngine;

namespace Calluna
{
    public abstract class ScriptableObjectId : ScriptableObject,
        IEquatable<ScriptableObjectId>, IEquatable<string>
    {
        [field: SerializeField] public string Id { get; private set; }

        public bool Equals(ScriptableObjectId other)
        {
            return ReferenceEquals(this, other);
        }

        public bool Equals(string other)
        {
            return Id == other;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            if (ReferenceEquals(null, obj)) return false;
            if (obj is string stringObj && Id == stringObj) return true;
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(ScriptableObjectId left, ScriptableObjectId right)
        {
            return left?.Equals(right) ?? ReferenceEquals(null, right);
        }

        public static bool operator ==(ScriptableObjectId left, string right)
        {
            return left?.Equals(right) ?? ReferenceEquals(null, right);
        }

        public static bool operator !=(ScriptableObjectId left, ScriptableObjectId right)
        {
            return !(left == right);
        }

        public static bool operator !=(ScriptableObjectId left, string right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            return Id;
        }
    }
}