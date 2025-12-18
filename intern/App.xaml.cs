using intern.Data;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;
using Microsoft.Extensions.Configuration;

namespace intern
{

    public partial class App : Application
    {

        public static ApplicationDbContext DbContext { get; private set; } = null!;

        public App()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfiguration config = builder.Build();

            var remote = config.GetConnectionString("DefaultConnection");
            var local = config.GetConnectionString("LocalConnection");

            if (!string.IsNullOrWhiteSpace(remote))
            {
                try
                {
                    var remoteOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                        .UseSqlServer(remote)
                        .Options;

                    var ctx = new ApplicationDbContext(remoteOptions);

                    if (ctx.Database.CanConnect())
                    {
                        DbContext = ctx;
                        return;
                    }
                }
                catch
                {
                }
            }

            if (!string.IsNullOrWhiteSpace(local))
            {
                try
                {
                    var localOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                        .UseSqlServer(local)
                        .Options;

                    var ctx = new ApplicationDbContext(localOptions);

                    if (ctx.Database.CanConnect())
                    {
                        DbContext = ctx;
                        return;
                    }
                }
                catch
                {
                    
                }
            }

            MessageBox.Show("Удалённая и локальная БД недоступны");
            Shutdown();
        }
    }
}
