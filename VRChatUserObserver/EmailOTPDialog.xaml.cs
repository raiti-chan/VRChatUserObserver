using System;
using System.Windows;
using raitichan.com.vrchat_api;

namespace VRChatUserObserver; 

public partial class EmailOTPDialog : Window {
	public EmailOTPDialog() {
		InitializeComponent();
	}

	private void OnEnterButton(object sender, RoutedEventArgs e) {
		AuthAPI authApi = new(App.INSTANCE.ApiClient);
		VerifiedResult? result = authApi.PostTowFactorAuthEmailOTPVerify(this.code.Text);
		if (result != null) {
			this.DialogResult = result.verified;
		}
		
		this.Close();
	}
}