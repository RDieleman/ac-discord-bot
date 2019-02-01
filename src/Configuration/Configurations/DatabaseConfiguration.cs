using Core.Configuration;

namespace Configuration
{
    public class DatabaseConfiguration : IDatabaseConfig
    {
        private readonly IConfiguration _config;

        private const string ConnectionStringKey = "ConnectionString";

        public DatabaseConfiguration(IConfiguration config)
        {
            _config = config;
        }

        public string GetConnectionString()
            => _config.GetValueFor(ConnectionStringKey);
    }
}