using System;

namespace Calluna
{
    /// <summary>
    /// A string key identifying a definition of type <typeparamref name="TDefinition"/> - e.g.
    /// <c>Id&lt;AttributeId&gt;</c> for attributes. Two ids are equal when their strings are equal
    /// (ordinal), regardless of where they came from, so logic, dictionaries and tests can work with ids
    /// without loading the definition assets: <c>new Id&lt;AttributeId&gt;("Knowledge")</c>. The type
    /// parameter only keeps ids of different definition types apart at compile time.
    /// Definitions deriving from <see cref="ScriptableObjectId{TSelf}"/> expose theirs as
    /// <see cref="ScriptableObjectId{TSelf}.Key"/>.
    /// </summary>
    public readonly struct Id<TDefinition> : IEquatable<Id<TDefinition>>
    {
        /// <summary>The id string; null for a default-constructed id.</summary>
        public string Value { get; }

        /// <summary>False for a default-constructed id or one with an empty string.</summary>
        public bool IsValid => !string.IsNullOrEmpty(Value);

        public Id(string value)
        {
            Value = value;
        }

        public bool Equals(Id<TDefinition> other) => string.Equals(Value, other.Value, StringComparison.Ordinal);

        public override bool Equals(object obj) => obj is Id<TDefinition> other && Equals(other);

        public override int GetHashCode() => Value == null ? 0 : StringComparer.Ordinal.GetHashCode(Value);

        public override string ToString() => Value ?? string.Empty;

        public static bool operator ==(Id<TDefinition> left, Id<TDefinition> right) => left.Equals(right);

        public static bool operator !=(Id<TDefinition> left, Id<TDefinition> right) => !left.Equals(right);
    }
}
