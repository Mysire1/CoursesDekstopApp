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
    public partial class StudentsByLanguageViewModel : QueryViewModelBase
    {
        private readonly ApplicationDbContext _context;
        
        [ObservableProperty]
        private ObservableCollection<Student> _studentsLearningGerman = new();
        [ObservableProperty]
        private ObservableCollection<Student> _studentsLearningMultipleLanguages = new();
        
        public StudentsByLanguageViewModel(ApplicationDbContext context)
        {
            _context = context;
        }
        
        [RelayCommand]
        private async Task GetStudentsByLanguageStatsAsync()
        {
            try
            {
                var germanStudents = await _context.Students
                    .Where(s => s.Enrollments.Any(e => e.Group.Level.Language.Name == "Німецька"))
                    .Distinct()
                    .ToListAsync();
                UpdateObservableCollection(StudentsLearningGerman, germanStudents);
                
                var multiLanguageStudents = await _context.Students
                    .Where(s => s.Enrollments
                                .Select(e => e.Group.Level.LanguageId)
                                .Distinct()
                                .Count() > 1)
                    .ToListAsync();
                UpdateObservableCollection(StudentsLearningMultipleLanguages, multiLanguageStudents);

                if (StudentsLearningGerman.Count == 0 && StudentsLearningMultipleLanguages.Count == 0)
                {
                    ShowInfo("Слухачів за цими критеріями не знайдено.");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Помилка виконання: {ex.Message}", "7");
            }
        }
    }
}