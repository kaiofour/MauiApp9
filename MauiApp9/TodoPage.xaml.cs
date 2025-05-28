using MauiApp9.Models;
using MauiApp9.Services;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace MauiApp9
{
    public partial class TodoPage : ContentPage, IRefreshablePage
    {
        private readonly ApiService _apiService;
        private int _userId => SessionService.Instance.UserId;
        public ObservableCollection<TodoItem> Tasks { get; } = new ObservableCollection<TodoItem>();

        public TodoPage()
        {
            InitializeComponent();
            _apiService = new ApiService();
            BindingContext = this;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadTasks();
            TodoListView.IsVisible = Tasks?.Count > 0; // Show only if there are items
            NoTasksLabel.IsVisible = !TodoListView.IsVisible;
        }

        public async Task ReloadTasksAsync()
        {
            await LoadTasks();
        }

        private async Task LoadTasks()
        {
            try
            {
                string jsonResponse = await _apiService.GetTasksAsync("active", _userId);
                var result = JsonSerializer.Deserialize<JsonElement>(jsonResponse);

                if (result.GetProperty("status").GetInt32() == 200)
                {
                    // Create a temporary list to verify uniqueness
                    var uniqueTasks = new Dictionary<int, TodoItem>();
                    var data = result.GetProperty("data");

                    foreach (JsonProperty item in data.EnumerateObject())
                    {
                        var task = item.Value;
                        var taskId = task.GetProperty("item_id").GetInt32();

                        // Only add if not already in dictionary
                        if (!uniqueTasks.ContainsKey(taskId))
                        {
                            uniqueTasks[taskId] = new TodoItem
                            {
                                item_id = taskId,
                                item_name = task.GetProperty("item_name").GetString(),
                                item_description = task.GetProperty("item_description").GetString(),
                                status = "active",
                                timemodified = DateTime.Now.ToString(),
                                IsCompleted = false
                            };
                        }
                    }

                    // Clear and update on main thread
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Tasks.Clear();
                        foreach (var task in uniqueTasks.Values)
                        {
                            Tasks.Add(task);
                        }

                        TodoListView.IsVisible = Tasks.Count > 0;
                        NoTasksLabel.IsVisible = !TodoListView.IsVisible;
                    });
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load tasks: {ex.Message}", "OK");
            }
        }

        private async void OnTodoItemTapped(object sender, EventArgs e)
        {
            // Get the task from the ViewCell's BindingContext
            if (sender is ViewCell viewCell && viewCell.BindingContext is TodoItem task)
            {
                // Clone the task to avoid reference issues
                var taskCopy = new TodoItem
                {
                    item_id = task.item_id,
                    item_name = task.item_name,
                    item_description = task.item_description,
                    status = task.status,
                    user_id = task.user_id,
                    timemodified = DateTime.Now.ToString(),
                    IsCompleted = task.IsCompleted
                };

                await Navigation.PushAsync(new EditTodoPage(this, taskCopy));
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
            try
            {
                if (sender is VisualElement image && image.BindingContext is TodoItem task)
                {
                    bool confirm = await DisplayAlert("Delete", $"Delete task '{task.item_name}'?", "Yes", "No");
                    if (confirm)
                    {
                        var response = await _apiService.DeleteTaskAsync(task.item_id);
                        var result = JsonSerializer.Deserialize<JsonElement>(response);

                        if (result.GetProperty("status").GetInt32() == 200)
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                var taskToRemove = Tasks.FirstOrDefault(t => t.item_id == task.item_id);
                                if (taskToRemove != null)
                                {
                                    Tasks.Remove(taskToRemove);
                                    // Update UI states
                                    TodoListView.IsVisible = Tasks.Count > 0;
                                    NoTasksLabel.IsVisible = !TodoListView.IsVisible;
                                }
                            });
                        }
                        else
                        {
                            await DisplayAlert("Error", "Failed to delete task.", "OK");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Delete failed: {ex.Message}", "OK");
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


    }
}


