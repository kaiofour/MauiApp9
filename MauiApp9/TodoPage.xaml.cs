using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.ComponentModel;

namespace MauiApp9;

public partial class TodoPage : ContentPage
{
    private readonly TodoService _todoService;

    public ObservableCollection<TodoItem> Tasks { get; set; }

    public TodoPage(TodoService todoService)
    {
        InitializeComponent();
        _todoService = todoService;
        Tasks = new ObservableCollection<TodoItem>();
        BindingContext = this;

        LoadTasks();
    }

    private async void LoadTasks()
    {
        try
        {
            IsBusy = true;
            NoTasksLabel.Text = "Loading tasks...";
            NoTasksLabel.IsVisible = true;

            var userId = Preferences.Get("user_id", 0);
            var (success, message, items) = await _todoService.GetTodoItems("active", userId);

            Tasks.Clear();
            TodoService.PendingTasks.Clear();

            if (success)
            {
                if (items?.Count > 0)
                {
                    foreach (var item in items)
                    {
                        Tasks.Add(item);
                        TodoService.PendingTasks.Add(item);
                    }
                    NoTasksLabel.IsVisible = false;
                }
                else
                {
                    NoTasksLabel.Text = "No tasks found";
                    NoTasksLabel.IsVisible = true;
                }
            }
            else
            {
                await DisplayAlert("Error", message, "OK");
                NoTasksLabel.Text = "Error loading tasks";
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void UpdateNoTasksMessage() => NoTasksLabel.IsVisible = Tasks.Count == 0;

    private async void OnAddTaskClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(AddTodoPage));
    }

    private async void OnDeleteTaskClicked(object sender, EventArgs e)
    {
        if (sender is VisualElement element && element.BindingContext is TodoItem taskToDelete)
        {
            var confirm = await DisplayAlert("Confirm", $"Delete {taskToDelete.Title}?", "Yes", "No");
            if (confirm)
            {
                // Explicitly declare the tuple variables
                (bool success, string message) = await _todoService.DeleteTodoItem(taskToDelete.item_id);

                if (success)
                {
                    Tasks.Remove(taskToDelete);
                    TodoService.PendingTasks.Remove(taskToDelete);
                }
                else
                {
                    await DisplayAlert("Error", message, "OK");
                }
            }
        }
    }

    private async void OnCheckBoxChanged(object sender, CheckedChangedEventArgs e)
    {
        if (sender is CheckBox checkBox && checkBox.BindingContext is TodoItem checkedItem)
        {
            var newStatus = e.Value ? "inactive" : "active";
            // Explicitly declare the tuple variables
            (bool success, string message) = await _todoService.ChangeTodoStatus(checkedItem.item_id, newStatus);

            if (success)
            {
                if (e.Value)
                {
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        Tasks.Remove(checkedItem);
                        TodoService.PendingTasks.Remove(checkedItem);
                        TodoService.CompletedTasks.Add(checkedItem);
                        await Shell.Current.GoToAsync(nameof(CompletedTodoPage));
                    });
                }
            }
            else
            {
                checkBox.IsChecked = !e.Value;
                await DisplayAlert("Error", message, "OK");
            }
        }
    }

    private async void OnTodoItemTapped(object sender, EventArgs e)
    {
        if (sender is VisualElement visualElement && visualElement.BindingContext is TodoItem tappedItem)
        {
            TodoService.SelectedItem = tappedItem;
            await Shell.Current.GoToAsync(nameof(EditTodoPage));
        }
    }

    private async void OnCheckClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(CompletedTodoPage));
    }

    private async void OnProfileClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(ProfilePage));
    }


}


public partial class TodoItem : INotifyPropertyChanged
{
    private int _itemId;
    private string _title = string.Empty;
    private string _description = string.Empty;
    private bool _isCompleted;
    private int _userId;
    private string _timeModified;

    public int item_id
    {
        get => _itemId;
        set
        {
            if (_itemId != value)
            {
                _itemId = value;
                OnPropertyChanged(nameof(item_id));
            }
        }
    }

    public string Title
    {
        get => _title;
        set
        {
            if (_title != value)
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }
    }

    public string Description
    {
        get => _description;
        set
        {
            if (_description != value)
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }
    }

    public bool IsCompleted
    {
        get => _isCompleted;
        set
        {
            if (_isCompleted != value)
            {
                _isCompleted = value;
                OnPropertyChanged(nameof(IsCompleted));
                OnPropertyChanged(nameof(Status)); // Update status when completion changes
            }
        }
    }

    // This property maps to the API's "status" field
    public string Status
    {
        get => IsCompleted ? "inactive" : "active";
        set => IsCompleted = value == "inactive";
    }

    public int user_id
    {
        get => _userId;
        set
        {
            if (_userId != value)
            {
                _userId = value;
                OnPropertyChanged(nameof(user_id));
            }
        }
    }

    public string timemodified
    {
        get => _timeModified;
        set
        {
            if (_timeModified != value)
            {
                _timeModified = value;
                OnPropertyChanged(nameof(timemodified));
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}