using System;
using System.Windows;
using raitichan.com.vrchat_api;
using raitichan.com.vrchat_api.JsonObject;

namespace VRChatUserObserver; 

public partial class EmailOTPDialog : Window {
	public EmailOTPDialog() {
		InitializeComponent();
	}

	private void OnEnterButton(object sender, RoutedEventArgs e) {
		AuthAPI authApi = new(App.INSTANCE.ApiClient);
		VerifyResult? result = authApi.PostTwoFactorAuthEmailOTPVerify(this.code.Text);
		if (result != null) {
			this.DialogResult = result.verified;
		}
		
		this.Close();
	}
}