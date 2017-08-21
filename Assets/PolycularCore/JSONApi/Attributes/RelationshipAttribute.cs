namespace JSONApi
{
    public enum RelationshipType
    {
        HAS_MANY,
        BELONGS_TO,
        BELONGS_TO_MANY
    }

    [System.AttributeUsage(
        System.AttributeTargets.Field |
        System.AttributeTargets.Property)
	]
    public class RelationshipAttribute : System.Attribute
    {
        public RelationshipType Type { get; private set; }

        public RelationshipAttribute(RelationshipType type)
        {
            Type = type;
        }
    }
}
