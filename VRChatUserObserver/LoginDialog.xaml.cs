namespace VRChatUserObserver; 

public partial class LoginDialog {
	public LoginDialog() {
		InitializeComponent();
		
		if (this.DataContext is LoginDialogViewModel viewModel) {
			viewModel.OnCloseDialog += OnCloseDialog;
		}
	}

	private void OnCloseDialog(bool result) {
		this.DialogResult = result;
		this.Close();
	}
}