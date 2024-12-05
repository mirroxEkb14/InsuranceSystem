using System.Windows;
using System.Windows.Controls;

namespace InsuranceSystemDemo.Controls;

public partial class BindablePasswordBox : UserControl
{
    #region Password Property for Custom PasswordBox
    private bool _isPasswordChanging;

    public string Password
    {
        get { return (string)GetValue(PasswordProperty); }
        set { SetValue(PasswordProperty, value); }
    }

    public static readonly DependencyProperty PasswordProperty =
        DependencyProperty.Register(
            "Password",
            typeof(string),
            typeof(BindablePasswordBox),
            new PropertyMetadata(string.Empty, PasswordPropertyChanged));

    //
    // Summary:
    //     When the «BindablePasswordBox» password changes, the value of the «BindablePasswordBox» is sent
    //         back to the binding.
    //     When the binding changes, the value is sent onto the «BindablePasswordBox», which keeps the 
    //         «BindablePasswordBox» in sync with the binding with the value on the ViewModel.
    private static void PasswordPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is BindablePasswordBox passwordBox)
            passwordBox.UpdatePassword();
    }

    private void UpdatePassword()
    {
        if (!_isPasswordChanging)
            passwordBox.Password = Password;
    }
    #endregion

    public BindablePasswordBox() =>
        InitializeComponent();

    //
    // Summary:
    //     Notifies whenever the password is changed.
    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        _isPasswordChanging = true;
        Password = passwordBox.Password;
        _isPasswordChanging = false;
    }
}
