using MakhzonPDR_DCM.Models;
using MakhzonPDR_DCM.Services;

namespace MakhzonPDR_DCM.Page;

public partial class UsersPage : ContentPage
{
    ApiService _apiService = new ApiService();

    public UsersPage()
    {
        InitializeComponent();
        LoadUsers();
    }

    async void LoadUsers()
    {
        UsersCollection.ItemsSource = await _apiService.GetUsersAsync();
    }

    async void OnAddUserClicked(object sender, EventArgs e)
    {
        string name = await DisplayPromptAsync("Nouveau", "Nom du utilisateur:");
        string email = await DisplayPromptAsync("Nouveau", "Email:");
        string pass = await DisplayPromptAsync("Nouveau", "Mot de passe:");

        if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(email))
        {
            var newUser = new User { Name = name, Email = email, Password = pass, Role = "Worker" };
            await _apiService.AddUserAsync(newUser);
            LoadUsers();
        }
    }

    private async void OnDeleteUserClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var user = button?.CommandParameter as User; // افترضنا أن الكلاس اسمه User

        if (user == null) return;

        // 1. تأكيد الحذف من الأدمن
        bool confirm = await DisplayAlert("Confirmation", $"Voulez-vous حذف {user.Name} ?", "Oui", "Non");

        if (confirm)
        {
            try
            {
                // 2. استدعاء الـ API الخاص بك لحذف المستخدم من قاعدة البيانات
                bool isDeleted = await _apiService.DeleteUserAsync(user.Id);

                if (isDeleted)
                {
                    // 3. إرسال الإشعار لجميع المستخدمين بعد الحذف الناجح
                    await _apiService.SendNotificationToAll(user.Name);

                    // 4. تحديث القائمة في الواجهة
                    LoadUsers();

                    await DisplayAlert("Succès", "Utilisateur supprimé et notification envoyée", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erreur", "خطأ أثناء الحذف: " + ex.Message, "OK");
            }
        }
    }

    // دالة إرسال الإشعار الخاصة بالـ Users
    //private async Task SendDeletionNotification(string userName)
    //{
    //    using (var client = new HttpClient())
    //    {
    //        var OneSignalAppId = "c8a48984-39ff-4048-b012-275acf55a313";
    //        var OneSignalRestKey = "os_v2_app_zcsitbbz75aermase5nm6vndcor5afb5dybuzff3x5jg7jphwyigl3vsipv7sy7fiyq665b2cbd3ku4fme33atwnddl5jfxuv7umpyi";

    //        client.DefaultRequestHeaders.Authorization =
    //            new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", OneSignalRestKey);

    //        var notification = new
    //        {
    //            app_id = OneSignalAppId,
    //            included_segments = new[] { "Total Subscriptions\r\n" },
    //            contents = new
    //            {
    //                en = $"User '{userName}' has been removed from the system.",
    //                fr = $"L'utilisateur '{userName}' a été supprimé du système.",
    //                ar = $"تم حذف المستخدم '{userName}' من النظام."
    //            },
    //            headings = new
    //            {
    //                en = "System Update",
    //                fr = "Mise à jour Système",
    //                ar = "تحديث النظام"
    //            }
    //        };

    //        var json = System.Text.Json.JsonSerializer.Serialize(notification);
    //        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

    //        await client.PostAsync("https://onesignal.com/api/v1/notifications", content);
    //    }
    //}
     
}