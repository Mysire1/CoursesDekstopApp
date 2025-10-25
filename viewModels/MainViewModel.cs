using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CoursesDekstopApp.data;
using DefaultNamespace;
using CoursesDekstopApp.viewModels.Queries;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.Input;


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
            
            LoadBaseDataAsync(context);
        }
        #endregion

        #region Базові дані (для ComboBox'ів)
        [ObservableProperty]
        private ObservableCollection<Student> _students = new();
        [ObservableProperty]
        private ObservableCollection<Language> _languages = new();
        [ObservableProperty]
        private ObservableCollection<Teacher> _teachers = new();
        [ObservableProperty]
        private ObservableCollection<Group> _allGroups = new();
        
        private async Task LoadBaseDataAsync(ApplicationDbContext context)
        {
             try
            {
                UpdateObservableCollectionInternal(Students, await context.Students.ToListAsync());
                UpdateObservableCollectionInternal(Languages, await context.Languages.ToListAsync());
                UpdateObservableCollectionInternal(Teachers, await context.Teachers.ToListAsync());
                UpdateObservableCollectionInternal(AllGroups, await context.Groups.Include(g => g.Teacher).ToListAsync());
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
        private void ShowErrorInternal(string message)
        {
            MessageBox.Show(message, "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
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
        private async Task AddStudentAsync(ApplicationDbContext context)
        {
            if (NewStudentDateOfBirth.HasValue && DateTime.Today.Year - NewStudentDateOfBirth.Value.Year < 11)
            {
                 ShowInfoInternal("Студент має бути старшим за 10 років.");
                return;
            }

            var newStudent = new Student {
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
                context.Students.Add(newStudent);
                await context.SaveChangesAsync();
                ShowInfoInternal($"Студента {newStudent.FullName} успішно додано!", "Успіх");
                
                NewStudentFirstName = string.Empty;
                NewStudentLastName = string.Empty;
                NewStudentDateOfBirth = null;
                NewStudentPhone = null;
                NewStudentEmail = null;

                await LoadBaseDataAsync(context);
            }
            catch (Exception ex)
            {
                 ShowErrorInternal($"Помилка додавання студента: {ex.Message}");
            }
        }
        #endregion
    }
}