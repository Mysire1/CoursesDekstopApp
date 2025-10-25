using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CoursesDekstopApp.data;
using DefaultNamespace;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CoursesDekstopApp.viewModels
{
    public class CostResult
    {
        public string Name { get; set; } = string.Empty;
        public decimal Cost { get; set; }
        public string? Details { get; set; }
    }
    
    public partial class MainViewModel : ObservableObject
    {
        private readonly ApplicationDbContext _context;
        
        [ObservableProperty]
        private ObservableCollection<Student> _students = new();
        
        [ObservableProperty]
        private ObservableCollection<Language> _languages = new();
        [ObservableProperty]
        private ObservableCollection<Teacher> _teachers = new();
        
        [ObservableProperty]
        private Language? _selectedLanguage;
        [ObservableProperty]
        private Teacher? _selectedTeacher;
        
        [ObservableProperty]
        private ObservableCollection<Group> _filteredGroups = new();
        
        [ObservableProperty]
        private string _costCalculationResult = string.Empty;
        [ObservableProperty]
        private ObservableCollection<CostResult> _costBreakdown = new();


        public MainViewModel(ApplicationDbContext context)
        {
            _context = context;
            
            LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                var studentsFromDb = await _context.Students.ToListAsync();
                Students.Clear();
                foreach (var student in studentsFromDb)
                {
                    Students.Add(student);
                }
                
                var languagesFromDb = await _context.Languages.ToListAsync();
                Languages.Clear();
                foreach (var lang in languagesFromDb)
                {
                    Languages.Add(lang);
                }

                var teachersFromDb = await _context.Teachers.ToListAsync();
                Teachers.Clear();
                foreach (var teacher in teachersFromDb)
                {
                    Teachers.Add(teacher);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження даних: {ex.Message}");
            }
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
                
                FilteredGroups.Clear();
                foreach (var group in result)
                {
                    FilteredGroups.Add(group);
                }

                if (FilteredGroups.Count == 0)
                {
                    MessageBox.Show("Груп за вашими критеріями не знайдено.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка пошуку: {ex.Message}");
            }
        }
        
        [RelayCommand]
        private async Task CalcCostAllLanguagesAsync()
        {
            CostCalculationResult = string.Empty;
            
            var costResults = await _context.Levels
                .Include(l => l.Language)
                .GroupBy(l => l.Language.Name)
                .Select(g => new CostResult
                {
                    Name = g.Key, // Назва мови
                    Cost = g.Sum(l => l.Cost)
                })
                .ToListAsync();

            CostBreakdown.Clear();
            foreach (var res in costResults) CostBreakdown.Add(res);
        }
        
        [RelayCommand]
        private async Task CalcCostSelectedLanguageAsync()
        {
            if (SelectedLanguage == null)
            {
                MessageBox.Show("Будь ласка, оберіть мову зі списку.");
                return;
            }

            CostBreakdown.Clear(); // Очищуємо таблицю

            var totalCost = await _context.Levels
                .Where(l => l.LanguageId == SelectedLanguage.LanguageId)
                .SumAsync(l => l.Cost);

            CostCalculationResult = $"Повна вартість вивчення мови '{SelectedLanguage.Name}': {totalCost:C}";
        }
        
        [RelayCommand]
        private async Task CalcCostByLevelAsync()
        {
            if (SelectedLanguage == null)
            {
                MessageBox.Show("Будь ласка, оберіть мову зі списку.");
                return;
            }

            CostCalculationResult = string.Empty;

            var costResults = await _context.Levels
                .Where(l => l.LanguageId == SelectedLanguage.LanguageId)
                .OrderBy(l => l.LevelOrder)
                .Select(l => new CostResult
                {
                    Name = l.Name,
                    Cost = l.Cost,
                    Details = $"Рівень {l.LevelOrder}"
                })
                .ToListAsync();

            CostBreakdown.Clear();
            foreach (var res in costResults) CostBreakdown.Add(res);
        }
        
        [RelayCommand]
        private async Task CalcCostPerMonthAsync()
        {
            if (SelectedLanguage == null)
            {
                MessageBox.Show("Будь ласка, оберіть мову зі списку.");
                return;
            }

            CostCalculationResult = string.Empty;

            var costResults = await _context.Levels
                .Where(l => l.LanguageId == SelectedLanguage.LanguageId)
                .OrderBy(l => l.LevelOrder)
                .Select(l => new CostResult
                {
                    Name = l.Name,
                    Cost = l.Cost / 3, 
                    Details = $"{l.Cost:C} / 3 місяці"
                })
                .ToListAsync();

            CostBreakdown.Clear();
            foreach (var res in costResults) CostBreakdown.Add(res);
        }
    }
}