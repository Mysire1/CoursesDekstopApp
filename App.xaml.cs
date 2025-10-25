using System.IO; // <--- ДОДАЙТЕ ЦЕЙ USING
using System.Windows;
using CoursesDekstopApp.data;
using CoursesDekstopApp.viewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CoursesDekstopApp.services;
using CoursesDekstopApp.services.impl;
using CoursesDekstopApp.viewModels.Queries;

namespace CoursesDekstopApp
{
    public partial class App : Application
    {
        private static IHost? AppHost { get; set; }

        public App()
        {
            AppHost = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.SetBasePath(Directory.GetCurrentDirectory());
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                
                .ConfigureServices((context, services) =>
                {
                    services.AddDbContext<ApplicationDbContext>(options =>
                    {
                        var connectionString = context.Configuration.GetConnectionString("DefaultConnection");
                        options.UseNpgsql(connectionString);
                    });
                    
                    services.AddTransient<IStudentService, StudentServiceImpl>();
                    services.AddTransient<IGroupService, GroupServiceImpl>();
                    
                    services.AddSingleton<MainWindow>();
                    services.AddSingleton<MainViewModel>();
                    
                    services.AddTransient<SearchGroupsViewModel>();
                    services.AddTransient<CalculateCostViewModel>();
                    
                    services.AddTransient<FailedExamsViewModel>();
                    services.AddTransient<TeachersByLanguageCountViewModel>();
                    services.AddTransient<StudentPaymentStatusViewModel>();
                    services.AddTransient<LoyaltyDiscountViewModel>();
                    services.AddTransient<StudentsByLanguageViewModel>();
                    services.AddTransient<SmallGroupSurchargeViewModel>();
                    services.AddTransient<LargeGroupDiscountViewModel>();
                    services.AddTransient<ScheduleViewModel>();
                })
                .Build();
         }

        protected override async void OnStartup(StartupEventArgs e)
        {
            try
            {
                await AppHost!.StartAsync();
                
                var startupForm = AppHost.Services.GetRequiredService<MainWindow>();
                
                startupForm.Show();

                base.OnStartup(e);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Виникла критична помилка під час запуску:\n\n{ex.ToString()}",
                    "Помилка Запуску",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Shutdown();
            }
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            if (AppHost != null)
            {
                await AppHost.StopAsync();
            }
            base.OnExit(e);
        }
    }
}