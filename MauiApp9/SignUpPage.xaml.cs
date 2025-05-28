using MauiApp9.Models;
using MauiApp9.Services;

namespace MauiApp9
{
    public partial class SignUpPage : ContentPage
    {
        private readonly ApiService _apiService;

        public SignUpPage()
        {
            InitializeComponent();
            _apiService = new ApiService();
        }

        private async void SignUpButton_Clicked(object sender, EventArgs e)
        {
            var model = new SignUpModel
            {
                first_name = FirstNameEntry.Text,
                last_name = LastNameEntry.Text,
                email = EmailEntry.Text,
                password = PasswordEntry.Text,
                confirm_password = ConfirmPasswordEntry.Text
            };

            if (string.IsNullOrWhiteSpace(model.first_name) ||
                string.IsNullOrWhiteSpace(model.last_name) ||
                string.IsNullOrWhiteSpace(model.email) ||
                string.IsNullOrWhiteSpace(model.password))
            {
                await DisplayAlert("Error", "Please fill in all fields.", "OK");
                return;
            }

            if (model.password != model.confirm_password)
            {
                await DisplayAlert("Error", "Passwords do not match.", "OK");
                return;
            }

            var response = await _apiService.SignUpAsync(model);
            await DisplayAlert("API Response", response, "OK");
        }

        private async void GoBackButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}