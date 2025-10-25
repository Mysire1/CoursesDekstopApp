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
    public partial class SmallGroupSurchargeViewModel : QueryViewModelBase
    {
        private readonly IGroupService _groupService;
        
        [ObservableProperty]
        private ObservableCollection<Group> _smallGroups = new();
        
        public SmallGroupSurchargeViewModel(IGroupService groupService)
        {
            _groupService = groupService;
        }
        
        [RelayCommand]
        private async Task ApplySmallGroupSurchargeAsync()
        {
            try
            {
                var (affectedGroups, studentsCount) = await _groupService.ApplySurchargeForSmallGroupsAsync(5, 20);
                
                UpdateObservableCollection(SmallGroups, affectedGroups);
                
                if (studentsCount > 0)
                {
                    ShowInfo($"Успішно збільшено вартість для {studentsCount} студентів у {affectedGroups.Count} малих групах.");
                }
                else if (affectedGroups.Count > 0)
                {
                    ShowInfo($"Знайдено {affectedGroups.Count} малих груп, але вартість для студентів вже була змінена раніше.");
                }
                else
                {
                    ShowInfo("Малих груп (менше 5 студентів) не знайдено.");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Помилка застосування надбавки: {ex.Message}", "8");
            }
        }
    }
}