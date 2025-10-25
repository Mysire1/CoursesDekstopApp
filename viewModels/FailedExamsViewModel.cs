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
    public partial class FailedExamsViewModel : QueryViewModelBase
    {
        private readonly ApplicationDbContext _context;
        
        [ObservableProperty]
        private int _failedExamsCount = 0;
        [ObservableProperty]
        private ObservableCollection<ExamResult> _failedExamResults = new();
        [ObservableProperty]
        private ObservableCollection<Level> _levelsWithFailures = new();
        
        public FailedExamsViewModel(ApplicationDbContext context)
        {
            _context = context;
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
                
                UpdateObservableCollection(FailedExamResults, failedResults);
                
                var levels = failedResults
                    .Select(er => er.Exam.Level)
                    .Distinct()
                    .ToList();
                UpdateObservableCollection(LevelsWithFailures, levels);
                
                if (FailedExamsCount == 0)
                {
                    ShowInfo("Слухачів, які не склали іспит, не знайдено.");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Помилка виконання: {ex.Message}", "3");
            }
        }
    }
}