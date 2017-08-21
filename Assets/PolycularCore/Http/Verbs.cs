namespace Http
{
    /// <summary>
    /// A list of HTTP methods as constants according to RFC 7231.
    /// It also adds the PATCH method according RFC 5789.
    /// <seealso cref="https://tools.ietf.org/html/rfc7231"/>
    /// <seealso cref="https://tools.ietf.org/html/rfc5789"/>
    /// </summary>
    public static class Verbs
    {
        public static readonly string OPTIONS = "OPTIONS";
        public static readonly string GET = "GET";
        public static readonly string HEAD = "HEAD";
        public static readonly string POST = "POST";
        public static readonly string PUT = "PUT";
        public static readonly string PATCH = "PATCH";
        public static readonly string DELETE = "DELETE";
        public static readonly string TRACE = "TRACE";
        public static readonly string CONNECT = "CONNECT";
    }
}
