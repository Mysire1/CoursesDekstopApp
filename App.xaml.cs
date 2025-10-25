using System.Windows;
using CoursesDekstopApp.data;
using CoursesDekstopApp.viewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CoursesDekstopApp.services;
using CoursesDekstopApp.services.impl;

namespace CoursesDekstopApp
{
    public partial class App : Application
    {
        private static IHost? AppHost { get; set; }

        public App()
        {
            AppHost = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddDbContext<ApplicationDbContext>(options =>
                    {
                        var connectionString = context.Configuration.GetConnectionString("DefaultConnection");
                        options.UseNpgsql(connectionString);
                    });
                    
                    services.AddSingleton<MainWindow>();
                    services.AddSingleton<MainViewModel>();
                    
                    services.AddTransient<IStudentService, StudentServiceImpl>();
    
                })
                .Build();
        }
        
        protected override async void OnStartup(StartupEventArgs e)
        {
            await AppHost!.StartAsync();
            
            var startupForm = AppHost.Services.GetRequiredService<MainWindow>();
            
            startupForm.Show(); 

            base.OnStartup(e);
        }
        
        protected override async void OnExit(ExitEventArgs e)
        {
            await AppHost!.StopAsync();
            base.OnExit(e);
        }
    }
}