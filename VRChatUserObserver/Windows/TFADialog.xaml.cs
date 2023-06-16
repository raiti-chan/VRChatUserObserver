using System.Windows;
using raitichan.com.vrchat_api;
using raitichan.com.vrchat_api.JsonObject;

namespace VRChatUserObserver.Windows;

public partial class TFADialog : Window {
	public TFADialog() {
		InitializeComponent();
	}

	private void OnVerify(object sender, RoutedEventArgs e) {
		AuthAPI authApi = App.INSTANCE.VRChatApi.AuthAPI;
		VerifyResult? verifyResult = authApi.PostTwoFactorAuthEmailOTPVerify(this.code.Text);
		if (verifyResult is not { verified: true }) {
			MessageBox.Show("Two-factor authentication failed.");
			return;
		}
		this.DialogResult = true;
		this.Close();
	}
}