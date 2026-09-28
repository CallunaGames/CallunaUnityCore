namespace Calluna
{
    /// <summary>
    /// A <see cref="ScriptableObjectId"/> exposing its id as a typed <see cref="Id{TDefinition}"/> key:
    /// <code>
    /// public class AttributeId : ScriptableObjectId&lt;AttributeId&gt; { }
    /// Id&lt;AttributeId&gt; key = attributeAsset.Key;
    /// </code>
    /// The asset stays the place to author an id (inspector picking, metadata); logic compares and
    /// stores the key. Switching a class from <see cref="ScriptableObjectId"/> to this base keeps its
    /// serialized id, since the id field is still declared in <see cref="ScriptableObjectId"/>.
    /// </summary>
    public abstract class ScriptableObjectId<TSelf> : ScriptableObjectId
        where TSelf : ScriptableObjectId<TSelf>
    {
        public Id<TSelf> Key => new Id<TSelf>(Id);
    }
}
