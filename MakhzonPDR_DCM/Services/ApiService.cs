using MakhzonPDR_DCM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MakhzonPDR_DCM.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "http://magasinpdr-001-site1.jtempurl.com/api";

        public ApiService()
        {
            _httpClient = new HttpClient();
        }

        // دالة إرسال الإشعار عبر OneSignal
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

        // STOCK -----------------------------------------

        public async Task<List<Stock>> GetStocksAsync(string search = "")
        {
            try
            {
                // نرسل كلمة البحث كبرامتر ?search=...
                // إذا كان فارغاً سيعيد السيرفر أول 100 عنصر كما برمجته أنت
                string url = $"{BaseUrl}/Stock?search={Uri.EscapeDataString(search)}";
                return await _httpClient.GetFromJsonAsync<List<Stock>>(url);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(@"Error: {0}", ex.Message);
                return new List<Stock>();
            }
        }

         

        public async Task<bool> UpdateCasierAsync(string refId, string newCasier , string newEnStock)
        {
            try
            {
                // تشفير القيم لضمان عملها حتى لو احتوت على مسافات أو رموز مثل / أو #
                string cleanRef = Uri.EscapeDataString(refId);
                string cleanCasier = Uri.EscapeDataString(newCasier ?? "");
                string cleanEnStock = (newEnStock).ToString();

                string url = $"{BaseUrl}/Stock/UpdateCasier?refId={cleanRef}&newCasier={cleanCasier}&newEnStock={cleanEnStock}";

                var response = await _httpClient.PutAsync(url, null);

                // إذا فشل التحديث، يمكنك رؤية السبب في الـ Debug
                if (!response.IsSuccessStatusCode)
                {
                    string error = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine("Update Failed: " + error);
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception: " + ex.Message);
                return false;
            }
        }

        // Users ------------------------------------------ 
        public async Task<User> LoginAsync(string email, string password)
        {
            try
            {
                var loginData = new { Email = email, Password = password };
                var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/Users/login", loginData);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<User>();
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
         
        public async Task<List<User>> GetUsersAsync() =>
            await _httpClient.GetFromJsonAsync<List<User>>($"{BaseUrl}/Users");

        public async Task<bool> AddUserAsync(User user)
        {
            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/Users", user);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{BaseUrl}/Users/{id}");
            return response.IsSuccessStatusCode;
        }

        // Doashbord ------------------------------------------
        public class DashboardStats
        {
            public int Total { get; set; }
            public int Updated { get; set; }
            public int New { get; set; }
        }

        public async Task<DashboardStats> GetStatsAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<DashboardStats>($"{BaseUrl}/Stock/stats");
            }
            catch { return new DashboardStats(); }
        }
         
        public async Task<List<SpecialProduct>> GetSpecialProductsAsync() =>
        await _httpClient.GetFromJsonAsync<List<SpecialProduct>>($"{BaseUrl}/SpecialProducts");

        public async Task<bool> AddSpecialProductAsync(SpecialProduct p)
        {
            var res = await _httpClient.PostAsJsonAsync($"{BaseUrl}/SpecialProducts", p);
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteSpecialProductAsync(int id)
        {
            var res = await _httpClient.DeleteAsync($"{BaseUrl}/SpecialProducts/{id}");
            return res.IsSuccessStatusCode;
        }
         
        // دالة تحديث منتج خاص
        public async Task<bool> UpdateSpecialProductAsync(int id, SpecialProduct p)
        {
            try
            {
                // نرسل طلب PUT مع رقم المنتج (id) والجسم (JSON) الذي يحتوي على البيانات الجديدة
                var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/SpecialProducts/{id}", p);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(@"Error UpdateSpecialProduct: {0}", ex.Message);
                return false;
            }
        }

    }
}
