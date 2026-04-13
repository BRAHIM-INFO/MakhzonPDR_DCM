using MakhzonPDR_DCM.Page;
using MakhzonPDR_DCM.Services;

namespace MakhzonPDR_DCM
{
    public partial class MainPage : ContentPage
    {
        ApiService _apiService = new ApiService();

        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing(); 
            await LoadDashboardStats();
        }

        private async void GoToStock(object sender, EventArgs e) =>
       await Navigation.PushAsync(new StockListPage());

        private async void GoToUsers(object sender, EventArgs e) =>
            await Navigation.PushAsync(new UsersPage());

        private async void OnAddProductClicked(object sender, EventArgs e) =>
           await Navigation.PushAsync(new SpecialProductsPage());

        //private async void GoToNotifications(object sender, EventArgs e) =>
        //    await Navigation.PushAsync(new NotificationsPage());

        private async void GoToHome(object sender, EventArgs e)
        {
            // عند ضغط الزر + في الرئيسية، نذهب لصفحة القائمة
            await Navigation.PushAsync(new StartPage());
        } 

        async Task LoadDashboardStats()
        {
            var stats = await _apiService.GetStatsAsync();

            // تحديث النصوص في الواجهة
            TotalStockLabel.Text = stats.Total.ToString("N0") + " Art."; // N0 تجعل الرقم مثل 21,000
            UpdatedStockLabel.Text = stats.Updated.ToString();
            NewProductsLabel.Text = stats.New.ToString();
        }
         
    }
}
