using Microsoft.Extensions.Configuration;

namespace Sybon.Auth
{
    public class AuthSecurityConfiguration
    {
        public class SybonArchiveConfiguration
        {
            public SybonArchiveConfiguration(IConfiguration configuration)
            {
                Configuration = configuration;
            }

            private IConfiguration Configuration { get; }
            
            public string Url => Configuration.GetValue<string>("Url");
        }

        public class InfluxDbConfiguration
        {
            public InfluxDbConfiguration(IConfiguration configuration)
            {
                Configuration = configuration;
            }
        
            private IConfiguration Configuration { get; }
            
            public string Url => Configuration.GetValue<string>("Url");
            public string Password => Configuration.GetValue<string>("Password");
            public string UserName => Configuration.GetValue<string>("UserName");
            public string Database => Configuration.GetValue<string>("Database");
        }
        
        public AuthSecurityConfiguration(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        public SybonArchiveConfiguration SybonArchive => new SybonArchiveConfiguration(Configuration.GetSection("SybonArchive"));
        public InfluxDbConfiguration InfluxDb => new InfluxDbConfiguration(Configuration.GetSection("InfluxDb"));
        public string ApiKey => Configuration.GetValue<string>("ApiKey");
    }
}