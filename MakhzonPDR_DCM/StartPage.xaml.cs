using MakhzonPDR_DCM.Page;
using System.Text;
using System.Text.Json;

namespace MakhzonPDR_DCM;

public partial class StartPage : ContentPage
{
	public StartPage()
	{
		InitializeComponent();
	}

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        // الانتقال لصفحة تسجيل الدخول
        await Navigation.PushAsync(new MainPage());
        // إرسال إشعار شامل
        string notificationMsg = $"✅ MAJ 📦 Stock ";
        _ = Task.Run(() => SendNotificationToAll(notificationMsg));

    }

    public async Task SendNotificationToAll(string messageText)
    {
        try
        {
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "https://onesignal.com/api/v1/notifications");

                // تأكد من أن المفتاح يبدأ بكلمة Basic إذا كان مفتاح REST API عادي
                request.Headers.Add("Authorization", "Basic os_v2_app_zcsitbbz75aermase5nm6vndcor5afb5dybuzff3x5jg7jphwyigl3vsipv7sy7fiyq665b2cbd3ku4fme33atwnddl5jfxuv7umpyi");
                var payload = new
                {
                    app_id = "c8a48984-39ff-4048-b012-275acf55a313",
                    included_segments = new[] { "Total Subscriptions" },
                    headings = new { en = "تحديث في المخزن 📦", fr = "Mise à jour Stock 📦 " },
                    contents = new { messageText }
                };

                var json = JsonSerializer.Serialize(payload);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");

                await client.SendAsync(request);
            }
        }
        catch (Exception ex)
        {
            // لا يفضل إظهار DisplayAlert هنا لكي لا ينزعج المستخدم إذا فشل الإنترنت في إرسال الإشعار فقط
            System.Diagnostics.Debug.WriteLine("OneSignal Error: " + ex.Message);
        }
    }
}