using JSONApi.JSON;

namespace JSONApi.JSON
{
    public class Error
    {
        public string Id { get; set; }

        public Links Links { get; set; }

        public string Status { get; set; }

        public string Code { get; set; }

        public string Title { get; set; }

        public string Detail { get; set; }

        public Source Source { get; set; }

        public Meta Meta { get; set; }
    }
}
