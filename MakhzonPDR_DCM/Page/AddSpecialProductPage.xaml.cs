using MakhzonPDR_DCM.Models;
using MakhzonPDR_DCM.Services;
using System.Text.Json;

namespace MakhzonPDR_DCM.Page;

public partial class AddSpecialProductPage : ContentPage
{
    ApiService _apiService = new ApiService();
    SpecialProduct _existingItem;
    bool _isEditMode = false;

    // مُنشئ للإضافة
    public AddSpecialProductPage() { InitializeComponent(); }

    // مُنشئ للتعديل
    public AddSpecialProductPage(SpecialProduct item)
    {
        InitializeComponent();
        _existingItem = item;
        _isEditMode = true;
        Title = "Modifier Produit";

        // تعبئة الحقول بالبيانات الموجودة
        RefEntry.Text = item.REF;
        NameEntry.Text = item.INTITULE;
        MachineEntry.Text = item.MACHINE;
        QteEntry.Text = item.QTE.ToString();
        CasierEntry.Text = item.CASIER;
    }
     
    async void OnSaveClicked(object sender, EventArgs e)
    {
        var prod = new SpecialProduct
        {
            Id = _isEditMode ? _existingItem.Id : 0,
            REF = RefEntry.Text,
            INTITULE = NameEntry.Text,
            MACHINE = MachineEntry.Text,
            QTE = int.Parse(QteEntry.Text),
            CASIER = CasierEntry.Text
        };

        bool ok;
        if (_isEditMode)
            ok = await _apiService.UpdateSpecialProductAsync(prod.Id, prod);
        else
            ok = await _apiService.AddSpecialProductAsync(prod);

        if (ok)
        {
            // إرسال إشعار للمدير والزملاء
            string title = "Nouveau Produit  🆕";
            string body = $"Un nouvel article ({prod.REF}) Ajouter  : {prod.INTITULE}";

            _ = Task.Run(() => _apiService.SendNotificationToAll(body));

            await DisplayAlert("Succès", "Opération réussie", "OK");
            await Navigation.PopAsync();
        }

        // بعد نجاح إضافة المنتج الجديد
        //bool success = await _apiService.AddSpecialProductAsync(prod);

        //if (success)
        //{
        //    // إرسال إشعار للمدير والزملاء
        //    string title = "Nouveau Produit Machine 🆕";
        //    string body = $"Un nouvel article ({prod.REF}) Ajouter  : {prod.INTITULE}";

        //    _ = Task.Run(() => _apiService.SendNotificationToAll(body));

        //    await DisplayAlert("Succès", "Produit ajouté وتنبيه الجميع", "OK");
        //    await Navigation.PopAsync();
        //}
    }
}