using System;
using System.Collections.Generic; // Потрібно для List<T>
using System.Collections.ObjectModel; // Потрібно для ObservableCollection<T>
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel; // Потрібно для [ObservableObject] та [ObservableProperty]
using CommunityToolkit.Mvvm.Input;       // Потрібно для [RelayCommand]
using CoursesDekstopApp.data;
using DefaultNamespace; // Namespace для моделей
using Microsoft.EntityFrameworkCore;

namespace CoursesDekstopApp.viewModels.Queries
{
    public partial class StudentPaymentStatusViewModel : QueryViewModelBase
    {
        private readonly ApplicationDbContext _context;
        
        [ObservableProperty]
        private ObservableCollection<Student> _paymentStatusStudents = new();
        
        public StudentPaymentStatusViewModel(ApplicationDbContext context)
        {
            _context = context;
        }
        
        private void UpdatePaymentStatusList(List<Student> students, string queryDescription)
        {
            UpdateObservableCollection(PaymentStatusStudents, students);
            if (PaymentStatusStudents.Count == 0)
            {
                ShowInfo($"Слухачів за критерієм '{queryDescription}' не знайдено.");
            }
        }
        
        [RelayCommand]
        private async Task GetFullyPaidStudentsAsync()
        {
            try
            {
                var students = await _context.Students
                    .Where(s => s.Enrollments.Any(e => e.Payments.Sum(p => p.Amount) >= e.FinalCost))
                    .Distinct() 
                    .ToListAsync();
                UpdatePaymentStatusList(students, "Повністю сплатили");
            }
            catch (Exception ex) { ShowError(ex.Message, "5.1"); }
        }
        
        [RelayCommand]
        private async Task GetNotFullyPaidStudentsAsync()
        {
            try
            {
                var students = await _context.Students
                    .Where(s => s.Enrollments.Any(e => e.FinalCost > e.Payments.Sum(p => p.Amount)))
                    .Distinct()
                    .ToListAsync();
                UpdatePaymentStatusList(students, "Є заборгованість");
            }
            catch (Exception ex) { ShowError(ex.Message, "5.2"); }
        }
        
        [RelayCommand]
        private async Task GetStudentsWithDebtLessThan50Async()
        {
            try
            {
                var students = await _context.Students
                    .Where(s => s.Enrollments.Any(e =>
                        e.FinalCost > e.Payments.Sum(p => p.Amount) &&
                        e.Payments.Sum(p => p.Amount) > (e.FinalCost * 0.5m)
                    ))
                    .Distinct()
                    .ToListAsync();
                UpdatePaymentStatusList(students, "Борг < 50%");
            }
            catch (Exception ex) { ShowError(ex.Message, "5.3"); }
        }

        [RelayCommand]
        private async Task GetStudentsWithDeferralsAsync()
        {
            try
            {
                var students = await _context.Students
                    .Where(s => s.Enrollments.Any(e => e.PaymentDeferrals.Any()))
                    .Distinct()
                    .ToListAsync();
                UpdatePaymentStatusList(students, "Мають відтермінування");
            }
            catch (Exception ex) { ShowError(ex.Message, "5.4"); }
        }
        
        [RelayCommand]
        private async Task GetStudentsWithDiscountsAsync()
        {
            try
            {
                var students = await _context.Students
                    .Where(s => s.HasDiscount == true || s.DiscountPercentage > 0)
                    .ToListAsync();
                UpdatePaymentStatusList(students, "Мають пільги");
            }
            catch (Exception ex) { ShowError(ex.Message, "5.5"); }
        }
    }
}