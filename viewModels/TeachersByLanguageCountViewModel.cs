using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CoursesDekstopApp.data;
using DefaultNamespace;
using Microsoft.EntityFrameworkCore;

namespace CoursesDekstopApp.viewModels.Queries
{
    public partial class TeachersByLanguageCountViewModel : QueryViewModelBase
    {
        private readonly ApplicationDbContext _context;
        
        [ObservableProperty]
        private ObservableCollection<Teacher> _teachersWithOneLanguage = new();
        [ObservableProperty]
        private ObservableCollection<Teacher> _teachersWithTwoLanguages = new();
        [ObservableProperty]
        private ObservableCollection<Teacher> _teachersWithThreeLanguages = new();
        
        public TeachersByLanguageCountViewModel(ApplicationDbContext context)
        {
            _context = context;
        }
        
        [RelayCommand]
        private async Task GetTeachersByLanguageCountAsync()
        {
            try
            {
                var allTeachers = await _context.Teachers
                    .Include(t => t.TeacherLanguages)
                    .ToListAsync();
                
                UpdateObservableCollection(TeachersWithOneLanguage,
                    allTeachers.Where(t => t.TeacherLanguages.Count == 1).ToList());

                UpdateObservableCollection(TeachersWithTwoLanguages,
                    allTeachers.Where(t => t.TeacherLanguages.Count == 2).ToList());

                UpdateObservableCollection(TeachersWithThreeLanguages,
                    allTeachers.Where(t => t.TeacherLanguages.Count == 3).ToList());
                
                if (TeachersWithOneLanguage.Count == 0 && TeachersWithTwoLanguages.Count == 0 && TeachersWithThreeLanguages.Count == 0)
                {
                    ShowInfo("Викладачів, що відповідають критеріям (1, 2 або 3 мови), не знайдено.");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Помилка виконання: {ex.Message}", "4");
            }
        }
    }
}