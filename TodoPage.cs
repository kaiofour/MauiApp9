using System.Text.Json;
using MauiApp9.Models;
using MauiApp9.Services;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace MauiApp9
{
    public partial class TodoPage : ContentPage, IRefreshablePage
    {
        private readonly ApiService _apiService;
        // Add a collection to hold your tasks (bind this to your ListView/CollectionView in XAML)
        public ObservableCollection<TodoItem> TaskCollection { get; set; } = new();

        public TodoPage()
        {
            InitializeComponent();
            _apiService = new ApiService();
            BindingContext = this;
        }

        private async void OnTodoItemTapped(object sender, EventArgs e)
        {
            if (sender is ViewCell viewCell && viewCell.BindingContext is TodoItem task)
            {
                await Navigation.PushAsync(new EditTodoPage(this, task));
            }
        }

        private async void OnCheckBoxChanged(object sender, CheckedChangedEventArgs e)
        {
            if (sender is CheckBox checkBox && checkBox.BindingContext is TodoItem task)
            {
                string newStatus = e.Value ? "inactive" : "active";
                var response = await _apiService.ToggleTaskStatusAsync(task.item_id, newStatus);
                var result = JsonSerializer.Deserialize<JsonElement>(response);

                if (result.GetProperty("status").GetInt32() != 200)
                {
                    await DisplayAlert("Error", "Failed to update task status.", "OK");
                    checkBox.IsChecked = !e.Value;
                }
                else
                {
                    await ReloadTasksAsync();
                }
            }
        }

        private async void OnDeleteTaskClicked(object sender, TappedEventArgs e)
        {
            if (sender is Image image && image.BindingContext is TodoItem task)
            {
                bool confirm = await DisplayAlert("Delete", $"Delete task '{task.item_name}'?", "Yes", "No");
                if (confirm)
                {
                    var response = await _apiService.DeleteTaskAsync(task.item_id);
                    var result = JsonSerializer.Deserialize<JsonElement>(response);

                    if (result.GetProperty("status").GetInt32() == 200)
                    {
                        await ReloadTasksAsync();
                    }
                    else
                    {
                        await DisplayAlert("Error", "Failed to delete task.", "OK");
                    }
                }
            }
        }

        private async void OnAddTaskClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddTodoPage(this));
        }

        private async void OnCheckClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//CompletedTodoPage");
        }

        private async void OnProfileClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//ProfilePage");
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await ReloadTasksAsync();
        }

        // Implementation for IRefreshablePage
        public async Task ReloadTasksAsync()
        {
            // Replace with your actual userId logic
            int userId = 1;
            string status = "active";
            var response = await _apiService.GetTasksAsync(status, userId);
            var result = JsonSerializer.Deserialize<JsonElement>(response);

            if (result.TryGetProperty("tasks", out var tasksElement))
            {
                var tasks = JsonSerializer.Deserialize<List<TodoItem>>(tasksElement.GetRawText());
                TaskCollection.Clear();
                foreach (var task in tasks)
                {
                    TaskCollection.Add(task);
                }
            }
        }
    }
}
