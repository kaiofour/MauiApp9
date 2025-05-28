
using System.Text.Json;
using MauiApp9.Models;
using MauiApp9.Services;

namespace MauiApp9
{
    public partial class EditTodoPage : ContentPage
    {
        private readonly ApiService _apiService;
        private readonly TodoItem _task;
        private readonly IRefreshablePage _sourcePage;

        public EditTodoPage(IRefreshablePage sourcePage, TodoItem task)
        {
            InitializeComponent();
            _apiService = new ApiService();
            _task = task;
            _sourcePage = sourcePage;

            TitleEntry.Text = _task.item_name;
            DescriptionEntry.Text = _task.item_description;
        }

        private async void OnUpdateClicked(object sender, EventArgs e)
        {
            _task.item_name = TitleEntry.Text;
            _task.item_description = DescriptionEntry.Text;
            _task.timemodified = DateTime.Now.ToString();

            if (string.IsNullOrWhiteSpace(_task.item_name))
            {
                await DisplayAlert("Warning!", "Title must not be empty.", "OK");
                return;
            }

            var response = await _apiService.EditTaskAsync(
                _task.item_id,
                _task.item_name,
                _task.item_description
            );


            var result = JsonSerializer.Deserialize<JsonElement>(response);

            if (result.GetProperty("status").GetInt32() == 200)
            {
                await _sourcePage.ReloadTasksAsync();
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Error", "Failed to update task.", "OK");
            }
        }

        private async void OnCompleteClicked(object sender, EventArgs e)
        {
            string newStatus = _task.status == "active" ? "inactive" : "active";
            var response = await _apiService.ToggleTaskStatusAsync(_task.item_id, newStatus);
            var result = JsonSerializer.Deserialize<JsonElement>(response);

            if (result.GetProperty("status").GetInt32() == 200)
            {
                await _sourcePage.ReloadTasksAsync();
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Error", "Failed to update task status.", "OK");
            }
        }

        private async void OnDeleteClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Delete", $"Delete task '{_task.item_name}'?", "Yes", "No");
            if (!confirm) return;

            var response = await _apiService.DeleteTaskAsync(_task.item_id);
            var result = JsonSerializer.Deserialize<JsonElement>(response);

            if (result.GetProperty("status").GetInt32() == 200)
            {
                await _sourcePage.ReloadTasksAsync();
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Error", "Failed to delete task.", "OK");
            }
        }

        private async void OnListClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//TodoPage");
        }

        private async void OnProfileClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//ProfilePage");
        }
    }
}