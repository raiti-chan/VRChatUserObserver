using System.Windows;
using NLog;
using raitichan.com.vrchat_api;
using raitichan.com.vrchat_api.JsonObject;

namespace VRChatUserObserver.Windows;

public partial class TFADialog {
	private static readonly Logger LOGGER = LogManager.GetCurrentClassLogger();
	
	public TFADialog() {
		InitializeComponent();
	}

	private void OnVerify(object sender, RoutedEventArgs e) {
		LOGGER.Info("Verify");
		AuthAPI authApi = App.INSTANCE.VRChatApi.AuthAPI;
		VerifyResult? verifyResult = authApi.PostTwoFactorAuthEmailOTPVerify(this.code.Text);
		if (verifyResult is not { verified: true }) {
			MessageBox.Show("Two-factor authentication failed.");
			return;
		}
		LOGGER.Info("Success");
		this.DialogResult = true;
		this.Close();
	}
}