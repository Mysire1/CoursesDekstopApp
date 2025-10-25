using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CoursesDekstopApp.services;
using DefaultNamespace;

namespace CoursesDekstopApp.viewModels.Queries
{
    public partial class LoyaltyDiscountViewModel : QueryViewModelBase
    {
        private readonly IStudentService _studentService;
        
        [ObservableProperty]
        private ObservableCollection<Student> _discountedStudents = new();
        
        public LoyaltyDiscountViewModel(IStudentService studentService)
        {
            _studentService = studentService;
        }
        
        [RelayCommand]
        private async Task ApplyStudentDiscountAsync()
        {
            try
            {
                var (updatedStudents, count) = await _studentService.ApplyDiscountForFrequentStudentsAsync(10);
                
                UpdateObservableCollection(DiscountedStudents, updatedStudents);
                
                if (count > 0)
                {
                    ShowInfo($"Успішно надано знижку 10% для {count} студентів.\nСписок студентів на головній вкладці буде оновлено.");
                }
                else
                {
                    ShowInfo("Нових студентів для надання знижки не знайдено (можливо, вона вже у всіх є, хто пройшов > 3 рівнів).");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Помилка надання знижки: {ex.Message}", "6");
            }
        }
    }
}