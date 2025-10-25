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
        
        [ObservableProperty]
        private int _failedExamsCount = 0;
        [ObservableProperty]
        private ObservableCollection<ExamResult> _failedExamResults = new();
        [ObservableProperty]
        private ObservableCollection<Level> _levelsWithFailures = new();

        [ObservableProperty]
        private ObservableCollection<Teacher> _teachersWithOneLanguage = new();
        [ObservableProperty]
        private ObservableCollection<Teacher> _teachersWithTwoLanguages = new();
        [ObservableProperty]
        private ObservableCollection<Teacher> _teachersWithThreeLanguages = new();
        
        [ObservableProperty]
        private ObservableCollection<Student> _paymentStatusStudents = new();
        
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
        
        [RelayCommand]
        private async Task GetFailedExamsAsync()
        {
            try
            {
                var failedResults = await _context.ExamResults
                    .Include(er => er.Student)
                    .Include(er => er.Exam)
                    .ThenInclude(e => e.Level)
                    .ThenInclude(l => l.Language)
                    .Where(er => er.Grade < 50)
                    .ToListAsync();
                
                FailedExamsCount = failedResults.Count;
                
                FailedExamResults.Clear();
                foreach (var res in failedResults)
                {
                    FailedExamResults.Add(res);
                }
                
                var levels = failedResults
                    .Select(er => er.Exam.Level)
                    .Distinct()
                    .ToList();
                
                LevelsWithFailures.Clear();
                foreach (var level in levels)
                {
                    LevelsWithFailures.Add(level);
                }

                if (FailedExamsCount == 0)
                {
                    MessageBox.Show("Слухачів, які не склали іспит, не знайдено.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка виконання запиту 3: {ex.Message}");
            }
        }
        
        [RelayCommand]
        private async Task GetTeachersByLanguageCountAsync()
        {
            try
            {
                var allTeachers = await _context.Teachers
                    .Include(t => t.TeacherLanguages)
                    .ToListAsync();
                
                TeachersWithOneLanguage.Clear();
                allTeachers.Where(t => t.TeacherLanguages.Count == 1)
                    .ToList()
                    .ForEach(t => TeachersWithOneLanguage.Add(t));

                TeachersWithTwoLanguages.Clear();
                allTeachers.Where(t => t.TeacherLanguages.Count == 2)
                    .ToList()
                    .ForEach(t => TeachersWithTwoLanguages.Add(t));
                
                TeachersWithThreeLanguages.Clear();
                allTeachers.Where(t => t.TeacherLanguages.Count == 3)
                    .ToList()
                    .ForEach(t => TeachersWithThreeLanguages.Add(t));
                
                if (TeachersWithOneLanguage.Count == 0 && TeachersWithTwoLanguages.Count == 0 && TeachersWithThreeLanguages.Count == 0)
                {
                    MessageBox.Show("Викладачів, що відповідають критеріям (1, 2 або 3 мови), не знайдено.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка виконання запиту 4: {ex.Message}");
            }
        }
        
        private void UpdatePaymentStatusList(List<Student> students)
        {
            PaymentStatusStudents.Clear();
            foreach (var s in students)
            {
                PaymentStatusStudents.Add(s);
            }
            if (PaymentStatusStudents.Count == 0)
            {
                MessageBox.Show("Слухачів за цим критерієм не знайдено.");
            }
        }
        
        [RelayCommand]
        private async Task GetFullyPaidStudentsAsync()
        {
            var students = await _context.Students
                .Where(s => s.Enrollments.Any(e => e.Payments.Sum(p => p.Amount) >= e.FinalCost))
                .Distinct()
                .ToListAsync();
            UpdatePaymentStatusList(students);
        }
        
        [RelayCommand]
        private async Task GetNotFullyPaidStudentsAsync()
        {
            var students = await _context.Students
                .Where(s => s.Enrollments.Any(e => e.FinalCost > e.Payments.Sum(p => p.Amount)))
                .Distinct()
                .ToListAsync();
            UpdatePaymentStatusList(students);
        }
        
        [RelayCommand]
        private async Task GetStudentsWithDebtLessThan50Async()
        {
            var students = await _context.Students
                .Where(s => s.Enrollments.Any(e =>
                    e.FinalCost > e.Payments.Sum(p => p.Amount) &&
                    e.Payments.Sum(p => p.Amount) > (e.FinalCost * 0.5m)
                ))
                .Distinct()
                .ToListAsync();
            UpdatePaymentStatusList(students);
        }
        
        [RelayCommand]
        private async Task GetStudentsWithDeferralsAsync()
        {
            var students = await _context.Students
                .Where(s => s.Enrollments.Any(e => e.PaymentDeferrals.Any()))
                .Distinct()
                .ToListAsync();
            UpdatePaymentStatusList(students);
        }
        
        [RelayCommand]
        private async Task GetStudentsWithDiscountsAsync()
        {
            var students = await _context.Students
                .Where(s => s.HasDiscount == true || s.DiscountPercentage > 0)
                .ToListAsync();
            UpdatePaymentStatusList(students);
        }
    }
}