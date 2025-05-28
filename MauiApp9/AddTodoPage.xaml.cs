using MauiApp9.Services;
using System.Text.Json;
using System.Text.Json.Nodes;
using MauiApp9.Models;
using MauiApp9.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MauiApp9
{
    public partial class AddTodoPage : ContentPage
    {
        private readonly ApiService _apiService;
        private readonly IRefreshablePage _todoPage;

        public AddTodoPage(IRefreshablePage todoPage)
        {
            InitializeComponent();
            _apiService = new ApiService();
            _todoPage = todoPage;
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            string title = TitleEntry.Text;
            string description = DescriptionEntry.Text ?? string.Empty; // Make description optional
            string status = "active";
            string timemodified = DateTime.Now.ToString();

            if (string.IsNullOrWhiteSpace(title))
            {
                await DisplayAlert("Validation", "Please enter at least a task title.", "OK");
                return;
            }

            string response = await _apiService.AddTaskAsync(
                title,
                description,
                SessionService.Instance.UserId,
                status
            );


            var result = JsonSerializer.Deserialize<JsonElement>(response);

            if (result.GetProperty("status").GetInt32() == 200)
            {
                await DisplayAlert("Success", "Task added successfully.", "OK");
                await _todoPage.ReloadTasksAsync();
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Error", "Failed to add task. PLEASE Add Description", "OK");
            }
        }
    }
}