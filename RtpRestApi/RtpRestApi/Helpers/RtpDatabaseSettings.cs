namespace RtpRestApi.Helpers
{
    public class RtpDatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;

        public string DatabaseName { get; set; } = null!;

        public string AdminsCollectionName { get; set; } = null!;

        public string AdminPasswordsCollectionName { get; set; } = null!;

        public string SettingsCollectionName { get; set; } = null!;
    }
}
