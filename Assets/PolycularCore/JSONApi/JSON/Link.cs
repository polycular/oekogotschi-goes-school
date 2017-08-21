namespace JSONApi.JSON
{
    public class Link
    {
        public static implicit operator Link(string str)
        {
            return new Link { Href = str };
        }

        public static implicit operator string(Link link)
        {
            return string.Copy(link.Href);
        }

        public string Href { get; set; }

        public Meta Meta { get; set; }

        public string About { get; set; }
    }
}