using MakhzonPDR_DCM.Services;
using System.Text;
using System.Text.Json;

namespace MakhzonPDR_DCM.Page;

public partial class LoginPage : ContentPage
{
    ApiService _apiService = new ApiService();
    public LoginPage()
    {
        InitializeComponent();
    }
    private async void OnPerformLoginClicked(object sender, EventArgs e)
    {
        string email = EmailEntry.Text;
        string password = PasswordEntry.Text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            await DisplayAlert("Erreur", "Veuillez remplir tous les champs", "OK");
            return;
        }

        // إظهار مؤشر تحميل (اختياري)
        IsBusy = true;

        var user = await _apiService.LoginAsync(email, password);

        IsBusy = false;

        if (user != null)
        {
            // حفظ اسم المستخدم لعرضه في الصفحة الرئيسية (اختياري)
            Preferences.Set("UserName", user.Name);
            Preferences.Set("UserRole", user.Role);
            Preferences.Set("UserId", user.Id);
            // الانتقال إلى الصفحة الرئيسية
            Application.Current.MainPage = new NavigationPage(new MainPage());
        }
        else
        {
            await DisplayAlert("Échec", "Email أو Mot de passe غير صحيح", "OK");
        }
    } 
}