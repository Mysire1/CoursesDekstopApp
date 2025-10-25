using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;
using System;
using System.Collections.ObjectModel;

namespace CoursesDekstopApp.viewModels.Queries
{
    public abstract partial class QueryViewModelBase : ObservableObject
    {
        protected void ShowError(string message, string? queryName = null)
        {
            string title = string.IsNullOrEmpty(queryName) ? "Помилка" : $"Помилка виконання запиту {queryName}";
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }
        
        protected void ShowInfo(string message, string? title = "Інформація")
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }
        
        protected void UpdateObservableCollection<T>(ObservableCollection<T> collection, List<T> data)
        {
            collection.Clear();
            if (data != null)
            {
                foreach (var item in data)
                {
                    collection.Add(item);
                }
            }
        }
    }
}