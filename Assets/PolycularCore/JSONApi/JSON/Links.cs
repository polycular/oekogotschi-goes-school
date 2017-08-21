namespace JSONApi.JSON
{
    public class Links
    {
        public Link Self { get; set; }

        public Link First { get; set; }

        public Link Last { get; set; }

        public Link Prev { get; set; }

        public Link Next { get; set; }

        public Meta Meta { get; set; }
    }
}