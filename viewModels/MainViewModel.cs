using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CoursesDekstopApp.data;
using DefaultNamespace;
using CoursesDekstopApp.viewModels.Queries;
using Microsoft.EntityFrameworkCore;

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
        #region Залежності та Конструктор
        
        private readonly ApplicationDbContext _context;
        
        public MainViewModel(ApplicationDbContext context,
                             SearchGroupsViewModel searchGroups,
                             CalculateCostViewModel calculateCost,
                             FailedExamsViewModel failedExams,
                             TeachersByLanguageCountViewModel teachersByLanguageCount,
                             StudentPaymentStatusViewModel studentPaymentStatus,
                             LoyaltyDiscountViewModel loyaltyDiscount,
                             StudentsByLanguageViewModel studentsByLanguage,
                             SmallGroupSurchargeViewModel smallGroupSurcharge,
                             LargeGroupDiscountViewModel largeGroupDiscount,
                             ScheduleViewModel schedule)
        {
            _context = context;
            
            SearchGroups = searchGroups;
            CalculateCost = calculateCost;
            FailedExams = failedExams;
            TeachersByLanguageCount = teachersByLanguageCount;
            StudentPaymentStatus = studentPaymentStatus;
            LoyaltyDiscount = loyaltyDiscount;
            StudentsByLanguage = studentsByLanguage;
            SmallGroupSurcharge = smallGroupSurcharge;
            LargeGroupDiscount = largeGroupDiscount;
            Schedule = schedule;
            
            _ = LoadBaseDataAsync();
        }
        #endregion

        #region Базові дані (для ComboBox'ів та вкладки Студенти)
        
        [ObservableProperty]
        private ObservableCollection<Student> _students = new();
        [ObservableProperty]
        private ObservableCollection<Language> _languages = new();
        [ObservableProperty]
        private ObservableCollection<Teacher> _teachers = new();
        [ObservableProperty]
        private ObservableCollection<Group> _allGroups = new();
        
        private async Task LoadBaseDataAsync()
        {
            try
            {
                var students = await _context.Students.ToListAsync();
                var languages = await _context.Languages.ToListAsync();
                var teachers = await _context.Teachers.ToListAsync();
                var groups = await _context.Groups.Include(g => g.Teacher).ToListAsync();
                
                UpdateObservableCollectionInternal(Students, students);
                UpdateObservableCollectionInternal(Languages, languages);
                UpdateObservableCollectionInternal(Teachers, teachers);
                UpdateObservableCollectionInternal(AllGroups, groups);
                
                UpdateObservableCollectionInternal(SearchGroups.AvailableLanguages, languages);
                UpdateObservableCollectionInternal(SearchGroups.AvailableTeachers, teachers);

                UpdateObservableCollectionInternal(CalculateCost.AvailableLanguages, languages);

                UpdateObservableCollectionInternal(Schedule.AvailableGroups, groups);
                UpdateObservableCollectionInternal(Schedule.AvailableTeachers, teachers);
            }
            catch (Exception ex)
            {
                ShowErrorInternal($"Помилка завантаження базових даних: {ex.Message}");
            }
        }
        
        private void UpdateObservableCollectionInternal<T>(ObservableCollection<T> collection, List<T> data)
        {
            if (collection == null) return;
            collection.Clear();
            if (data != null) foreach (var item in data) collection.Add(item);
        }
        private void ShowErrorInternal(string message, string? title = "Помилка")
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }
         private void ShowInfoInternal(string message, string title = "Інформація")
        {
             MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        #endregion

        #region ViewModel Запитів (Властивості)
        public SearchGroupsViewModel SearchGroups { get; }
        public CalculateCostViewModel CalculateCost { get; }
        public FailedExamsViewModel FailedExams { get; }
        public TeachersByLanguageCountViewModel TeachersByLanguageCount { get; }
        public StudentPaymentStatusViewModel StudentPaymentStatus { get; }
        public LoyaltyDiscountViewModel LoyaltyDiscount { get; }
        public StudentsByLanguageViewModel StudentsByLanguage { get; }
        public SmallGroupSurchargeViewModel SmallGroupSurcharge { get; }
        public LargeGroupDiscountViewModel LargeGroupDiscount { get; }
        public ScheduleViewModel Schedule { get; }
        #endregion

        #region Адміністрування - Додати студента
        
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanAddStudent))]
        private string _newStudentFirstName = string.Empty;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanAddStudent))]
        private string _newStudentLastName = string.Empty;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanAddStudent))]
        private DateTime? _newStudentDateOfBirth = null;
        [ObservableProperty]
        private string? _newStudentPhone;
        [ObservableProperty]
        private string? _newStudentEmail;
        
        public bool CanAddStudent =>
            !string.IsNullOrWhiteSpace(NewStudentFirstName) &&
            !string.IsNullOrWhiteSpace(NewStudentLastName) &&
            NewStudentDateOfBirth.HasValue;

        [RelayCommand(CanExecute = nameof(CanAddStudent))]
        private async Task AddStudentAsync()
        {
            if (NewStudentDateOfBirth.HasValue && DateTime.Today.Year - NewStudentDateOfBirth.Value.Year < 11)
            {
                 ShowInfoInternal("Студент має бути старшим за 10 років.");
                return;
            }
            
            var newStudent = new Student
            {
                FirstName = NewStudentFirstName,
                LastName = NewStudentLastName,
                DateOfBirth = NewStudentDateOfBirth!.Value,
                Phone = NewStudentPhone,
                Email = NewStudentEmail,
                RegistrationDate = DateTime.Now,
                CreatedAt = DateTime.Now
            };

            try
            {
                _context.Students.Add(newStudent);
                await _context.SaveChangesAsync();
                ShowInfoInternal($"Студента {newStudent.FullName} успішно додано!", "Успіх");
                
                NewStudentFirstName = string.Empty;
                NewStudentLastName = string.Empty;
                NewStudentDateOfBirth = null;
                NewStudentPhone = null;
                NewStudentEmail = null;
                
                await LoadBaseDataAsync();
            }
            catch (Exception ex)
            {
                 ShowErrorInternal($"Помилка додавання студента: {ex.Message}");
            }
        }
        #endregion
    }
}