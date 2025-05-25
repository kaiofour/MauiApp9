using System;
using System.Net.Http;
using Microsoft.Maui.Controls;
using System.Text.Json;
using System.Threading.Tasks;

namespace MauiApp9;

public partial class SignInPage : ContentPage
{
    private readonly HttpClient _httpClient = new HttpClient();
    private const string BaseUrl = "https://todo-list.dcism.org/";

    public SignInPage()
    {
        InitializeComponent();
        _httpClient.BaseAddress = new Uri(BaseUrl);
    }

    private async void SignInButton_Clicked(object sender, EventArgs e)
    {
        var email = EmailEntry.Text?.Trim();
        var password = PasswordEntry.Text;

        if (string.IsNullOrEmpty(email))
        {
            await DisplayAlert("Error", "Please enter email", "OK");
            return;
        }

        if (string.IsNullOrEmpty(password))
        {
            await DisplayAlert("Error", "Please enter password", "OK");
            return;
        }

        try
        {
            // Test connection using getUser_action.php endpoint
            var (connectionSuccess, connectionMessage) = await TestApiConnection();
            if (!connectionSuccess)
            {
                await DisplayAlert("Connection Error", connectionMessage, "OK");
                return;
            }

            // Proceed with sign in
            string url = $"signin_action.php?email={Uri.EscapeDataString(email)}&password={Uri.EscapeDataString(password)}";
            var response = await _httpClient.GetAsync(url);
            var responseBody = await response.Content.ReadAsStringAsync();

            System.Diagnostics.Debug.WriteLine("API RESPONSE: " + responseBody);

            var signInResponse = JsonSerializer.Deserialize<SignInResponse>(responseBody);

            if (signInResponse?.status == 200 && signInResponse.data?.id > 0)
            {
                Preferences.Set("user_id", signInResponse.data.id.ToString());
                await Shell.Current.GoToAsync("//TodoPage");
            }
            else
            {
                await DisplayAlert("Error", signInResponse?.message ?? "Login failed", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }
    }

    // Tests connection using getUser_action.php endpoint
    private async Task<(bool success, string message)> TestApiConnection()
    {
        try
        {
            // Using a lightweight GET request to check connection
            var response = await _httpClient.GetAsync("getUser_action.php?email=test@test.com");

            // Even if the user doesn't exist, we just want to check if the endpoint is reachable
            return response.IsSuccessStatusCode
                ? (true, "API is reachable")
                : (false, $"API returned status: {response.StatusCode}");
        }
        catch (HttpRequestException httpEx)
        {
            return (false, $"Network error: {httpEx.Message}");
        }
        catch (Exception ex)
        {
            return (false, $"Connection test failed: {ex.Message}");
        }
    }

    private async void SignUpButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//SignUpPage");
    }
}

public class SignInResponse
{
    public int status { get; set; }
    public string? message { get; set; }
    public UserData? data { get; set; }
}

public class UserData
{
    public int id { get; set; }
    public string? fname { get; set; }
    public string? lname { get; set; }
    public string? email { get; set; }
    public string? timemodified { get; set; }
}