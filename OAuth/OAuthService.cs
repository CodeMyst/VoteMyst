namespace VoteMyst.OAuth
{
    public class OAuthService
    {
        public string Name { get; set; }
        public string AuthEndpoint { get; set; }
        public string TokenEndpoint { get; set; }
        public string UserEndpoint { get; set; }
        public OAuthClaim[] Claims { get; set; }
        public string[] Scopes { get; set; }
    }
}