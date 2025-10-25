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
                    
                    services.AddTransient<SearchGroupsViewModel>(provider =>
                        new SearchGroupsViewModel(
                            provider.GetRequiredService<ApplicationDbContext>(),
                            provider.GetRequiredService<MainViewModel>().Languages,
                            provider.GetRequiredService<MainViewModel>().Teachers
                        ));
                    
                    services.AddTransient<CalculateCostViewModel>(provider =>
                        new CalculateCostViewModel(
                            provider.GetRequiredService<ApplicationDbContext>(),
                            provider.GetRequiredService<MainViewModel>().Languages
                        ));
                    
                    services.AddTransient<FailedExamsViewModel>();
                    
                    services.AddTransient<TeachersByLanguageCountViewModel>();
                    
                    services.AddTransient<StudentPaymentStatusViewModel>();
                    
                    services.AddTransient<LoyaltyDiscountViewModel>();
                    
                    services.AddTransient<StudentsByLanguageViewModel>();
                    
                    services.AddTransient<SmallGroupSurchargeViewModel>();
                    
                    services.AddTransient<LargeGroupDiscountViewModel>();
                    
                    services.AddTransient<ScheduleViewModel>(provider =>
                        new ScheduleViewModel(
                            provider.GetRequiredService<ApplicationDbContext>(),
                            provider.GetRequiredService<MainViewModel>().AllGroups,
                            provider.GetRequiredService<MainViewModel>().Teachers
                        ));
                })
                .Build(); }
        
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