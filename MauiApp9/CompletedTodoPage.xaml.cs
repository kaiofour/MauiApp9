using MauiApp9.Models;
using MauiApp9.Services;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Threading.Tasks;

namespace MauiApp9
{
    public partial class CompletedTodoPage : ContentPage, IRefreshablePage
    {
        private readonly ApiService _apiService;
        private int _userId => SessionService.Instance.UserId;

        public ObservableCollection<TodoItem> CompletedTasks { get; } = new ObservableCollection<TodoItem>();


        public CompletedTodoPage()
        {
            InitializeComponent();
            _apiService = new ApiService();
            BindingContext = this;
            LoadTasks();
        }

        public async Task ReloadTasksAsync() => LoadTasks();

        private async Task LoadTasks()
        {
            try
            {
                string jsonResponse = await _apiService.GetTasksAsync("inactive", _userId);
                var result = JsonSerializer.Deserialize<JsonElement>(jsonResponse);

                if (result.GetProperty("status").GetInt32() == 200)
                {
                    var newTasks = new List<TodoItem>();
                    var data = result.GetProperty("data");

                    foreach (JsonProperty item in data.EnumerateObject())
                    {
                        var task = item.Value;
                        newTasks.Add(new TodoItem
                        {
                            item_id = task.GetProperty("item_id").GetInt32(),
                            item_name = task.GetProperty("item_name").GetString(),
                            item_description = task.GetProperty("item_description").GetString(),
                            status = "inactive",
                            user_id = _userId,
                            timemodified = DateTime.Now.ToString(),
                            IsCompleted = true
                        });
                    }

                    // Update on main thread
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        CompletedTasks.Clear();
                        foreach (var task in newTasks)
                        {
                            CompletedTasks.Add(task);
                        }

                        // Reset the ListView binding
                        CompletedTodoListView.ItemsSource = null;
                        CompletedTodoListView.ItemsSource = CompletedTasks;
                    });
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load tasks: {ex.Message}", "OK");
            }
        }




        private async void OnCompletedTodoItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is TodoItem task)
            {
                await Navigation.PushAsync(new EditTodoPage(this, task));
            }
        }

        private bool _isRefreshing;

        // Modify the delete method
        private async void OnDeleteCompletedTaskClicked(object sender, EventArgs e)
        {
            if (_isRefreshing) return;

            try
            {
                _isRefreshing = true;

                if (sender is Image image && image.BindingContext is TodoItem task)
                {
                    bool confirm = await DisplayAlert("Delete", $"Delete task '{task.item_name}'?", "Yes", "No");
                    if (confirm)
                    {
                        var response = await _apiService.DeleteTaskAsync(task.item_id);
                        var result = JsonSerializer.Deserialize<JsonElement>(response);

                        if (result.GetProperty("status").GetInt32() == 200)
                        {
                            // Use a fresh reload instead of removing single item
                            await LoadTasks();

                            // Small delay to ensure UI updates
                            await Task.Delay(100);
                        }
                    }
                }
            }
            finally
            {
                _isRefreshing = false;
            }
        }

        private async void OnListClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//TodoPage");
        }

        private async void OnProfileCLicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//ProfilePage");
        }
    }
}