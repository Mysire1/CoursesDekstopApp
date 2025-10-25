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
    public partial class LargeGroupDiscountViewModel : QueryViewModelBase
    {
        private readonly IGroupService _groupService;
        
        [ObservableProperty]
        private ObservableCollection<Group> _largeGroups = new();
        
        public LargeGroupDiscountViewModel(IGroupService groupService)
        {
            _groupService = groupService;
        }
        
        [RelayCommand]
        private async Task ApplyLargeGroupDiscountAsync()
        {
            try
            {
                var (affectedGroups, studentsCount) = await _groupService.ApplyDiscountForLargeGroupsAsync(20, 5);
                
                UpdateObservableCollection(LargeGroups, affectedGroups);
                
                if (studentsCount > 0)
                {
                    ShowInfo($"Успішно надано знижку 5% для {studentsCount} студентів у {affectedGroups.Count} великих групах.");
                }
                else if (affectedGroups.Count > 0)
                {
                    ShowInfo($"Знайдено {affectedGroups.Count} великих груп, але знижка для студентів вже була застосована раніше.");
                }
                else
                {
                    ShowInfo("Великих груп (рівно 20 студентів) не знайдено.");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Помилка застосування знижки: {ex.Message}", "9");
            }
        }
    }
}