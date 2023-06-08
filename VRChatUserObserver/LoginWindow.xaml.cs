using System.Windows;
using raitichan.com.vrchat_api;
using raitichan.com.vrchat_api.JsonObject;

namespace VRChatUserObserver {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class LoginWindow : Window {
		public LoginWindow() {
			InitializeComponent();
		}

		private void OnLoginButton(object sender, RoutedEventArgs e) {
			AuthAPI authApi = new(App.INSTANCE.ApiClient);
			UserInfo? result = authApi.GetUser(this.id.Text, this.password.Text);
			if (result == null) return;
			
			EmailOTPDialog emailOtpDialog = new EmailOTPDialog();
			emailOtpDialog.ShowDialog();
		}
	}
}
