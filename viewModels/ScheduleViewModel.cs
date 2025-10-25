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
    public partial class ScheduleViewModel : QueryViewModelBase
    {
        private readonly ApplicationDbContext _context;
        
        [ObservableProperty]
        private Group? _selectedGroupForSchedule;
        [ObservableProperty]
        private Teacher? _selectedTeacherForSchedule;
        
        [ObservableProperty]
        private ObservableCollection<Schedule> _scheduleResults = new();
        public ObservableCollection<Group> AvailableGroups { get; }
        public ObservableCollection<Teacher> AvailableTeachers { get; }
        
        public ScheduleViewModel(ApplicationDbContext context,
                                 ObservableCollection<Group> groups,
                                 ObservableCollection<Teacher> teachers)
        {
            _context = context;
            AvailableGroups = groups;
            AvailableTeachers = teachers;
        }
        
        [RelayCommand]
        private async Task GetScheduleAsync()
        {
            try
            {
                ScheduleResults.Clear();
                List<Schedule> results;

                if (SelectedGroupForSchedule != null)
                {
                    results = await _context.Schedules
                        .Include(s => s.Group)
                        .Include(s => s.Classroom)
                        .Where(s => s.GroupId == SelectedGroupForSchedule.GroupId)
                        .OrderBy(s => s.DayOfWeek).ThenBy(s => s.StartTime)
                        .ToListAsync();
                    
                    SelectedTeacherForSchedule = null;
                }
                else if (SelectedTeacherForSchedule != null)
                {
                    results = await _context.Schedules
                        .Include(s => s.Group)
                        .Include(s => s.Classroom)
                        .Where(s => s.Group.TeacherId == SelectedTeacherForSchedule.TeacherId)
                        .OrderBy(s => s.DayOfWeek).ThenBy(s => s.StartTime)
                        .ToListAsync();
                    
                    SelectedGroupForSchedule = null;
                }
                else
                {
                    ShowInfo("Будь ласка, оберіть групу АБО викладача.");
                    return;
                }
                
                UpdateObservableCollection(ScheduleResults, results);
                
                if (ScheduleResults.Count == 0)
                {
                    ShowInfo("Розклад не знайдено.");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Помилка отримання розкладу: {ex.Message}", "10");
            }
            finally
            {
                SetProperty(ref _selectedGroupForSchedule, null);
                SetProperty(ref _selectedTeacherForSchedule, null);
            }
        }
    }
}