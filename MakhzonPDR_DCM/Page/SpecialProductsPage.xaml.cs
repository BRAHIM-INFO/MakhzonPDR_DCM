using MakhzonPDR_DCM.Models;
using MakhzonPDR_DCM.Services;

namespace MakhzonPDR_DCM.Page;

public partial class SpecialProductsPage : ContentPage
{
    ApiService _apiService = new ApiService();


    public SpecialProductsPage()
	{
		InitializeComponent();
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadData(); // تحميل البيانات كلما ظهرت الصفحة
    }

    async void LoadData()
    {
        SpecialItemsCollection.ItemsSource = await _apiService.GetSpecialProductsAsync();
    }
    async void OnAddNewClicked(object sender, EventArgs e)
    {
        // فتح صفحة الإدخال
        await Navigation.PushAsync(new AddSpecialProductPage());
    }

    async void OnDeleteClicked(object sender, EventArgs e)
    {
        var item = (sender as Element).BindingContext as SpecialProduct;
        bool confirm = await DisplayAlert("Confirmation", $"Supprimer {item.INTITULE} ?", "Oui", "Non");

        if (confirm)
        {
            bool success = await _apiService.DeleteSpecialProductAsync(item.Id);

            // إرسال إشعار للمدير والزملاء
            string title = "Produit Supprimer  ";
            string body = $"article ({item.REF})  /   : {item.INTITULE} a été Supprimé";

            _ = Task.Run(() => _apiService.SendNotificationToAll(body));


            if (success) LoadData(); // تحديث القائمة
        }
    }

    async void OnEditClicked(object sender, EventArgs e)
    {
        var item = (sender as Element).BindingContext as SpecialProduct;
        // نفتح نفس صفحة الإضافة ولكن نرسل لها البيانات لتعديلها
        await Navigation.PushAsync(new AddSpecialProductPage(item));
    }

}