using System.Windows;
using Do_An.BLL; // Cần import BLL để dùng enum

namespace Do_An
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // Ghi đè phương thức OnStartup
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // ----------------------------------------------------
            // 2. KHI CHẠY THẬT (NỘP BÀI):
            // (Bạn comment khối này khi test Admin)
            // ----------------------------------------------------

            //GiaoDienDangNhap loginWindow = new GiaoDienDangNhap();
            // loginWindow.Show();
        }
    }
}