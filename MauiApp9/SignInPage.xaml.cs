using MauiApp9.Services;
using System.Text.Json;

namespace MauiApp9
{
    public partial class SignInPage : ContentPage
    {
        private readonly ApiService _apiService;

        public SignInPage()
        {
            InitializeComponent();
            _apiService = new ApiService();
        }

        private async void SignInButton_Clicked(object sender, EventArgs e)
        {
            string email = EmailEntry.Text;
            string password = PasswordEntry.Text;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                await DisplayAlert("Error", "Please enter both email and password.", "OK");
                return;
            }

            string jsonResponse = await _apiService.SignInAsync(email, password);

            try
            {
                var result = JsonSerializer.Deserialize<JsonElement>(jsonResponse);
                int status = result.GetProperty("status").GetInt32();

                if (status == 200)
                {
                    var userData = result.GetProperty("data");
                    int userId = userData.GetProperty("id").GetInt32();
                    string fname = userData.GetProperty("fname").GetString();
                    string lname = userData.GetProperty("lname").GetString();

                    SessionService.Instance.UserId = userId;
                    SessionService.Instance.FName = fname;
                    SessionService.Instance.LName = lname;
                    SessionService.Instance.Email = userData.GetProperty("email").GetString();

                    Application.Current.MainPage = new AppShell();

                    await DisplayAlert("Success", "Credentials are valid.", "OK"); //new

                    await Shell.Current.GoToAsync("//TodoPage");
                }
                else
                {
                    string message = result.GetProperty("message").GetString();
                    await DisplayAlert("Login Failed", message, "OK");
                }
            }
            catch
            {
                await DisplayAlert("Error", "Invalid server response.", "OK");
            }
        }

        private async void SignUpButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SignUpPage());
        }
    }
}