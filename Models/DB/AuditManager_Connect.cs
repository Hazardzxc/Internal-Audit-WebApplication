using Microsoft.EntityFrameworkCore;

namespace STD.Models.DB
{
    public class AuditManager_Connect : AuditManager_DB
    {
        public AuditManager_Connect() { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var ConnectionStrings = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

                string? _option = ConnectionStrings.GetValue<string>("ConnectionStrings:DefaultConnection");

                optionsBuilder.UseSqlServer(_option);
            }
        }
    }
}