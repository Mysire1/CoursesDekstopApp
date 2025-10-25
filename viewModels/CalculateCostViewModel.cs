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
    public partial class CalculateCostViewModel : QueryViewModelBase
    {
        private readonly ApplicationDbContext _context;
        
        [ObservableProperty]
        private string _costCalculationResult = string.Empty;
        [ObservableProperty]
        private ObservableCollection<CostResult> _costBreakdown = new();
        
        public ObservableCollection<Language> AvailableLanguages { get; set; }
        
        [ObservableProperty]
        private Language? _selectedLanguage;
        
        public CalculateCostViewModel(ApplicationDbContext context)
        {
            _context = context;
            AvailableLanguages = new ObservableCollection<Language>();
        }
        
        [RelayCommand]
        private async Task CalcCostAllLanguagesAsync()
        {
             CostCalculationResult = string.Empty;
             try
             {
                 var costResults = await _context.Levels
                    .Include(l => l.Language)
                    .GroupBy(l => l.Language.Name)
                    .Select(g => new CostResult { Name = g.Key, Cost = g.Sum(l => l.Cost) })
                    .ToListAsync();
                 UpdateObservableCollection(CostBreakdown, costResults);
             }
             catch (Exception ex)
             {
                 ShowError($"Помилка розрахунку: {ex.Message}", "2 (всі мови)");
             }
        }
        
        [RelayCommand]
        private async Task CalcCostSelectedLanguageAsync()
        {
            if (SelectedLanguage == null) { ShowInfo("Будь ласка, оберіть мову зі списку."); return; }
            CostBreakdown.Clear();
            try
            {
                var totalCost = await _context.Levels
                    .Where(l => l.LanguageId == SelectedLanguage.LanguageId)
                    .SumAsync(l => l.Cost);
                CostCalculationResult = $"Повна вартість вивчення мови '{SelectedLanguage.Name}': {totalCost:C}";
            }
             catch (Exception ex)
             {
                 ShowError($"Помилка розрахунку: {ex.Message}", "2 (обрана мова)");
             }
        }
        
        [RelayCommand]
        private async Task CalcCostByLevelAsync()
        {
            if (SelectedLanguage == null) { ShowInfo("Будь ласка, оберіть мову зі списку."); return; }
            CostCalculationResult = string.Empty;
            try
            {
                var costResults = await _context.Levels
                    .Where(l => l.LanguageId == SelectedLanguage.LanguageId)
                    .OrderBy(l => l.LevelOrder)
                    .Select(l => new CostResult { Name = l.Name, Cost = l.Cost, Details = $"Рівень {l.LevelOrder}" })
                    .ToListAsync();
                UpdateObservableCollection(CostBreakdown, costResults);
            }
             catch (Exception ex)
             {
                 ShowError($"Помилка розрахунку: {ex.Message}", "2 (за рівнями)");
             }
        }
        
        [RelayCommand]
        private async Task CalcCostPerMonthAsync()
        {
            if (SelectedLanguage == null) { ShowInfo("Будь ласка, оберіть мову зі списку."); return; }
            CostCalculationResult = string.Empty;
            try
            {
                var costResults = await _context.Levels
                    .Where(l => l.LanguageId == SelectedLanguage.LanguageId)
                    .OrderBy(l => l.LevelOrder)
                    .Select(l => new CostResult { Name = l.Name, Cost = l.Cost / l.DurationInMonths, Details = $"{l.Cost:C} / {l.DurationInMonths} місяці" })
                    .ToListAsync();
                UpdateObservableCollection(CostBreakdown, costResults);
            }
             catch (Exception ex)
             {
                 ShowError($"Помилка розрахунку: {ex.Message}", "2 (за місяць)");
             }
        }
    }
}