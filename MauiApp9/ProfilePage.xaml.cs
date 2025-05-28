using MauiApp9.Services;

namespace MauiApp9
{
    public partial class ProfilePage : ContentPage
    {
        public ProfilePage()
        {
            InitializeComponent();
        }

        private async void OnSignOutClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Sign Out", "Are you sure you want to sign out?", "Yes", "No");
            if (!confirm) return;

            SessionService.Instance.Clear();
            await Shell.Current.GoToAsync("//SignInPage");
        }

        private async void OnListClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//TodoPage");
        }

        private async void OnCheckClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//CompletedTodoPage");
        }
    }
}