namespace Interview.Backend.Auth
{
    public class UserClaim
    {
        public Guid Identity { get; set; }

        public string Nickname { get; set; }

        public string Avatar { get; set; }

        public string TwitchIdentity { get; set; }

        public List<string> Roles { get; set; }
    }
}
