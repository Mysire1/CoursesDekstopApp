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
    public partial class SearchGroupsViewModel : QueryViewModelBase
    {
        private readonly ApplicationDbContext _context;
        
        [ObservableProperty]
        private Language? _selectedLanguage;
        [ObservableProperty]
        private Teacher? _selectedTeacher;
        [ObservableProperty]
        private ObservableCollection<Group> _filteredGroups = new();
        
        public ObservableCollection<Language> AvailableLanguages { get; }
        public ObservableCollection<Teacher> AvailableTeachers { get; }
        
        public SearchGroupsViewModel(ApplicationDbContext context,
                                     ObservableCollection<Language> languages,
                                     ObservableCollection<Teacher> teachers)
        {
            _context = context;
            AvailableLanguages = languages;
            AvailableTeachers = teachers;
        }
        
        [RelayCommand]
        private async Task SearchGroupsAsync()
        {
            try
            {
                var query = _context.Groups
                    .Include(g => g.Level)
                    .Include(g => g.Level.Language)
                    .Include(g => g.Teacher)
                    .AsQueryable();
                
                if (SelectedLanguage != null)
                {
                    query = query.Where(g => g.Level.LanguageId == SelectedLanguage.LanguageId);
                }
                if (SelectedTeacher != null)
                {
                    query = query.Where(g => g.TeacherId == SelectedTeacher.TeacherId);
                }
                
                var result = await query.ToListAsync();
                
                UpdateObservableCollection(FilteredGroups, result);
                
                if (FilteredGroups.Count == 0)
                {
                    ShowInfo("Груп за вашими критеріями не знайдено.");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Помилка пошуку: {ex.Message}", "1");
            }
        }
    }
}