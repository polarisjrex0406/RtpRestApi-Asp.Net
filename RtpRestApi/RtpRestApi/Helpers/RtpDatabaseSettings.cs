namespace RtpRestApi.Helpers
{
    public class RtpDatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;

        public string DatabaseName { get; set; } = null!;

        public string AdminsCollectionName { get; set; } = null!;

        public string AdminPasswordsCollectionName { get; set; } = null!;

        public string SettingsCollectionName { get; set; } = null!;

        public string TopicsCollectionName { get; set; } = null!;

        public string ArtifactsCollectionName { get; set; } = null!;

        public string BaseUrl { get; set; } = null!;

        public string ApiKey { get; set; } = null!;

        public string DataSource { get; set; } = null!;
    }
}
