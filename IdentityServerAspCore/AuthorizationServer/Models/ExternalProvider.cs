namespace AuthorizationServer.Models
{
    /// <summary>
    /// Information on how to log into external providers.
    /// </summary>
    public class ExternalProvider
    {
        /// <summary>
        /// What to show to the user.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Unique name to log into provider.
        /// </summary>
        public string AuthenticationScheme { get; set; }
    }
}
