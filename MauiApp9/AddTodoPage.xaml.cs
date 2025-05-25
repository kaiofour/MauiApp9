using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MauiApp9;

public partial class AddTodoPage : ContentPage
{
    private readonly TodoService _todoService;

    public ICommand NavigateBackCommand { get; private set; }

    public AddTodoPage(TodoService todoService)
    {
        InitializeComponent();
        _todoService = todoService;
        BindingContext = this;
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        string title = TitleEntry.Text;
        string description = DescriptionEntry.Text;

        if (!string.IsNullOrWhiteSpace(title))
        {
            var userId = Preferences.Get("user_id", 0);
            // Explicitly declare the tuple variables
            (bool success, string message, TodoItem newItem) = await _todoService.AddTodoItem(
                title,
                description,
                userId);

            if (success && newItem != null)
            {
                TodoService.PendingTasks.Add(newItem);
                TitleEntry.Text = string.Empty;
                DescriptionEntry.Text = string.Empty;
                await Shell.Current.GoToAsync("//TodoPage");
            }
            else
            {
                await DisplayAlert("Error", message ?? "Failed to add task", "OK");
            }
        }
        else
        {
            await DisplayAlert("Error", "Title cannot be empty", "OK");
        }
    }
}