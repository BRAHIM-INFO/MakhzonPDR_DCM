using MakhzonPDR_DCM.Models;
using MakhzonPDR_DCM.Services;
using System.Text;
using System.Text.Json;
using System.Collections.ObjectModel;
using System.Net.Http.Json;

namespace MakhzonPDR_DCM.Page;

public partial class StockListPage : ContentPage
{
    ApiService _apiService = new ApiService();
    // قائمة لتخزين كل البيانات القادمة من السيرفر
    List<Stock> _allStock = new List<Stock>();

    public   StockListPage()
    {
        InitializeComponent();
        LoadData();
    }

    private async void OnRefreshClicked(object sender, EventArgs e)
    {
        // 1. تشغيل مؤشر التحميل
        loadingIndicator.IsRunning = true;

        // 2. جلب البيانات من السيرفر
        await LoadData();

        // 3. إرسال إشعار لكل المستخدمين
        string message = "L'inventaire a été actualisé. Les dernières modifications sont disponibles.";
        _ = Task.Run(() => _apiService.SendNotificationToAll(message));

        loadingIndicator.IsRunning = false;

        // تنبيه بسيط للمستخدم الحالي
        await DisplayAlert("Succès", "Données actualisées et notification envoyée", "OK");
    } 

    async Task LoadData()
    {
        try
        {
            loadingIndicator.IsRunning = true;
            _allStock = await _apiService.GetStocksAsync();
            StockCollection.ItemsSource = _allStock;
            loadingIndicator.IsRunning = false;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", "Impossible de charger les données: " + ex.Message, "OK");
            loadingIndicator.IsRunning = false;
        }
    }

    // --- منطق البحث ---
    // عند الكتابة في مربع البحث
    async void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        string searchTerm = e.NewTextValue;

        // سنبدأ البحث إذا مسح المستخدم النص أو كتب 3 أحرف فأكثر (لتقليل الضغط على السيرفر)
        if (string.IsNullOrWhiteSpace(searchTerm) || searchTerm.Length >= 3)
        {
            try
            {
                loadingIndicator.IsRunning = true;

                // نطلب البيانات المفلترة من السيرفر مباشرة
                var results = await _apiService.GetStocksAsync(searchTerm);
                StockCollection.ItemsSource = results;

                loadingIndicator.IsRunning = false;
            }
            catch
            {
                loadingIndicator.IsRunning = false;
            }
        }
    }

    // --- منطق التعديل والتحديث الفوري --- async void  OnStockSelected(object sender, SelectionChangedEventArgs e)
    //private async void OnEditButtonClicked(object sender, EventArgs e)

    async void OnStockSelected(object sender, SelectionChangedEventArgs e)
    {
        var selectedItem = e.CurrentSelection.FirstOrDefault() as Stock;
        if (selectedItem == null) return;  
         
        string result = await DisplayPromptAsync("Modifier Casier",
            $"Nouveau emplacement pour {selectedItem.REF}:",
            initialValue: selectedItem.CASIER);
 
        string resultStock = await DisplayPromptAsync("Modifier Stock",
           $"Nouveau Qte pour {selectedItem.REF}:",
           initialValue: selectedItem.en_Stock.ToString());


        if (result != null)
        {
            bool success = await _apiService.UpdateCasierAsync(selectedItem.REF, result, resultStock);

            if (success)
            {
                // 1. تحديث الواجهة محلياً
                selectedItem.CASIER = result;
                var currentList = StockCollection.ItemsSource;
                StockCollection.ItemsSource = null;
                StockCollection.ItemsSource = currentList;

                // m2. صياغة رسالة الإشعار
                string notificationMsg = $"Le produit ({selectedItem.REF} / {selectedItem.INTITULE}) a été déplacé vers le casier: {result}";

                // 3. إرسال الإشعار (بدون انتظار Task.Run لكي لا يتوقف التطبيق)
                _ = Task.Run(() => _apiService.SendNotificationToAll(notificationMsg));

                await DisplayAlert("Succès", "Casier mis à jour et notification envoyée !", "OK");
            }
            else
            {
                await DisplayAlert("Erreur", "Échec de la mise à jour.", "OK");
            }
        }
    ((CollectionView)sender).SelectedItem = null;
    }

     

     

}