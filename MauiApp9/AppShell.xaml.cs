namespace MauiApp9;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(SignInPage), typeof(SignInPage));
        Routing.RegisterRoute(nameof(SignUpPage), typeof(SignUpPage));
        Routing.RegisterRoute(nameof(ProfilePage), typeof(ProfilePage));
        Routing.RegisterRoute(nameof(AddTodoPage), typeof(AddTodoPage));
        Routing.RegisterRoute(nameof(EditTodoPage), typeof(EditTodoPage));
    }


}
