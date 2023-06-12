using System;
using System.Windows;
using System.Windows.Input;
using raitichan.com.vrchat_api;
using raitichan.com.vrchat_api.JsonObject;

namespace VRChatUserObserver.Commands;

public class LoginCommand : ICommand {
	private readonly LoginDialogViewModel _viewModel;

	public LoginCommand(LoginDialogViewModel viewModel) {
		this._viewModel = viewModel;
	}

	public bool CanExecute(object? parameter) {
		return true;
	}

	public void Execute(object? parameter) {
		if (this.Login()) {
			this._viewModel.CloseDialog(true);
		}
	}

	private bool Login() {
		AuthAPI authApi = App.INSTANCE.VRChatApi.AuthAPI;
		authApi.PutLogout();
		UserInfo? userInfo = authApi.GetUser(this._viewModel.UserName, this._viewModel.Password);
		if (userInfo == null) {
			MessageBox.Show("Wrong username or password.");
			return false;
		}

		AuthResult? authResult = authApi.GetAuth();
		if (authResult == null) {
			MessageBox.Show("Wrong username or password.");
			return false;
		}

		if (authResult.ok) {
			return true;
		}
		
		TFADialog tfaDialog = new();
		tfaDialog.ShowDialog();
		return tfaDialog.DialogResult ?? false;
	}

	public event EventHandler? CanExecuteChanged {
		add { }
		remove { }
	}
}