namespace JSONApi.JSON
{
	public class ResourceIdentifier : IIdentifierData
    {
        public string Id { get; set; }

        public string Type { get; set; }

        public Meta Meta { get; set; }
    }
}