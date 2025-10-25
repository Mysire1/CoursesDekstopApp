using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CoursesDekstopApp.data;
using DefaultNamespace;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using CoursesDekstopApp.services;

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
        private readonly IStudentService _studentService;
        private readonly IGroupService _groupService;
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
        
        [ObservableProperty]
        private ObservableCollection<Student> _studentsLearningGerman = new();
        [ObservableProperty]
        private ObservableCollection<Student> _studentsLearningMultipleLanguages = new();
        
        [ObservableProperty]
        private ObservableCollection<Group> _smallGroups = new();
        
        [ObservableProperty]
        private ObservableCollection<Group> _largeGroups = new();
        
        [ObservableProperty]
        private ObservableCollection<Group> _allGroups = new();
        [ObservableProperty]
        private Group? _selectedGroupForSchedule;
        [ObservableProperty]
        private Teacher? _selectedTeacherForSchedule;
        [ObservableProperty]
        private ObservableCollection<Schedule> _scheduleResults = new();
        
        public MainViewModel(ApplicationDbContext context, IStudentService studentService, IGroupService groupService)
        {
            _context = context;
            _studentService = studentService;
            _groupService = groupService; 
            LoadDataAsync();
        }
        private async Task LoadDataAsync()
        {
            try
            {
                var studentsFromDb = await _context.Students.ToListAsync();
                Students.Clear();
                foreach (var student in studentsFromDb) Students.Add(student);
                var languagesFromDb = await _context.Languages.ToListAsync();
                Languages.Clear();
                foreach (var lang in languagesFromDb) Languages.Add(lang);
                var teachersFromDb = await _context.Teachers.ToListAsync();
                Teachers.Clear();
                foreach (var teacher in teachersFromDb) Teachers.Add(teacher);
                var allGroupsFromDb = await _context.Groups.Include(g => g.Teacher).ToListAsync();
                AllGroups.Clear();
                foreach (var group in allGroupsFromDb) AllGroups.Add(group);
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
        
        [RelayCommand]
        private async Task GetStudentsByLanguageStatsAsync()
        {
            try
            {
                var germanStudents = await _context.Students
                    .Where(s => s.Enrollments.Any(e => e.Group.Level.Language.Name == "Німецька"))
                    .Distinct()
                    .ToListAsync();
                
                StudentsLearningGerman.Clear();
                foreach (var s in germanStudents) StudentsLearningGerman.Add(s);
                
                var multiLanguageStudents = await _context.Students
                    .Where(s => s.Enrollments
                        .Select(e => e.Group.Level.LanguageId)
                        .Distinct()
                        .Count() > 1)
                    .ToListAsync();

                StudentsLearningMultipleLanguages.Clear();
                foreach (var s in multiLanguageStudents) StudentsLearningMultipleLanguages.Add(s);

                if (StudentsLearningGerman.Count == 0 && StudentsLearningMultipleLanguages.Count == 0)
                {
                    MessageBox.Show("Слухачів за цими критеріями не знайдено.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка виконання запиту 7: {ex.Message}");
            }
        }
        
        [RelayCommand]
        private async Task ApplySmallGroupSurchargeAsync()
        {
            try
            {
                var (affectedGroups, studentsCount) = await _groupService.ApplySurchargeForSmallGroupsAsync(5, 20); 

                SmallGroups.Clear();
                foreach (var g in affectedGroups)
                {
                    SmallGroups.Add(g);
                }

                if (studentsCount > 0)
                {
                    MessageBox.Show($"Успішно збільшено вартість для {studentsCount} студентів у {affectedGroups.Count} малих групах.");
                }
                else if (affectedGroups.Count > 0)
                {
                    MessageBox.Show($"Знайдено {affectedGroups.Count} малих груп, але вартість для студентів вже була змінена раніше.");
                }
                else
                {
                    MessageBox.Show("Малих груп (менше 5 студентів) не знайдено.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка застосування надбавки: {ex.Message}");
            }
        }
        
        [RelayCommand]
        private async Task ApplyLargeGroupDiscountAsync()
        {
            try
            {
                var (affectedGroups, studentsCount) = await _groupService.ApplyDiscountForLargeGroupsAsync(20, 5);
                
                LargeGroups.Clear();
                foreach (var g in affectedGroups)
                {
                    LargeGroups.Add(g);
                }

                if (studentsCount > 0)
                {
                    MessageBox.Show($"Успішно надано знижку 5% для {studentsCount} студентів у {affectedGroups.Count} великих групах.");
                }
                else if (affectedGroups.Count > 0)
                {
                    MessageBox.Show($"Знайдено {affectedGroups.Count} великих груп, але знижка для студентів вже була застосована раніше.");
                }
                else
                {
                    MessageBox.Show("Великих груп (рівно 20 студентів) не знайдено.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка застосування знижки: {ex.Message}");
            }
        }
        [RelayCommand]
        private async Task GetScheduleAsync()
        {
            try
            {
                ScheduleResults.Clear();
                List<Schedule> results = new List<Schedule>();

                if (SelectedGroupForSchedule != null)
                {
                    results = await _context.Schedules
                        .Include(s => s.Group)
                        .Include(s => s.Classroom)
                        .Where(s => s.GroupId == SelectedGroupForSchedule.GroupId)
                        .OrderBy(s => s.DayOfWeek).ThenBy(s => s.StartTime)
                        .ToListAsync();
                }
                else if (SelectedTeacherForSchedule != null)
                {
                    results = await _context.Schedules
                        .Include(s => s.Group)
                        .Include(s => s.Classroom)
                        .Where(s => s.Group.TeacherId == SelectedTeacherForSchedule.TeacherId)
                        .OrderBy(s => s.DayOfWeek).ThenBy(s => s.StartTime)
                        .ToListAsync();
                }
                else
                {
                    MessageBox.Show("Будь ласка, оберіть групу АБО викладача.");
                    return;
                }
                
                foreach (var s in results)
                {
                    ScheduleResults.Add(s);
                }

                if (ScheduleResults.Count == 0)
                {
                    MessageBox.Show("Розклад не знайдено.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка отримання розкладу: {ex.Message}");
            }
            finally
            {
                SelectedGroupForSchedule = null;
                SelectedTeacherForSchedule = null;
            }
        }
    }
}