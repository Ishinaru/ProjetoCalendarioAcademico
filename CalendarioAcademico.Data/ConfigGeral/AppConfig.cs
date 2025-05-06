using Microsoft.Extensions.Configuration;

namespace CalendarioAcademico.Data.ConfigGeral
{
    public class AppConfig
    {
        public string ConnectionString { get; } = string.Empty;
        public AppConfig(IConfiguration configuration) 
        { 
            ConnectionString = configuration.GetConnectionString("CalendarioAcademicoDB") ?? string.Empty; 
        }
    }
}
