using System;
using System.Windows;
using System.Windows.Input;
using NLog;
using raitichan.com.vrchat_api;
using raitichan.com.vrchat_api.JsonObject;
using VRChatUserObserver.Windows;

namespace VRChatUserObserver.Commands;

public class LoginCommand : ICommand {
	private static readonly Logger LOGGER = LogManager.GetCurrentClassLogger();
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
		LOGGER.Info("Login : {}", this._viewModel.UserName);
		
		AuthAPI authApi = App.INSTANCE.VRChatApi.AuthAPI;
		authApi.PutLogout();
		
		UserInfo? userInfo = authApi.GetUser(this._viewModel.UserName, this._viewModel.Password);
		if (userInfo == null) {
			LOGGER.Warn("Login failed.");
			MessageBox.Show("Wrong username or password.");
			return false;
		}

		AuthResult? authResult = authApi.GetAuth();
		if (authResult == null) {
			LOGGER.Warn("Login failed.");
			MessageBox.Show("Wrong username or password.");
			return false;
		}

		if (authResult.ok) {
			LOGGER.Info("Success.");
			return true;
		}
		
		LOGGER.Info("Need two factor auth.");
		TFADialog tfaDialog = new();
		tfaDialog.ShowDialog();
		return tfaDialog.DialogResult ?? false;
	}

	public event EventHandler? CanExecuteChanged {
		add { }
		remove { }
	}
}