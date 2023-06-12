using System;
using System.Windows.Input;
using raitichan.com.vrchat_api;

namespace VRChatUserObserver.Commands;

public class ChangeAccountCommand : ICommand {
	private readonly MainWindowViewModel _viewModel;

	public ChangeAccountCommand(MainWindowViewModel viewModel) {
		this._viewModel = viewModel;
	}
	
	public bool CanExecute(object? parameter) {
		return true;
	}

	public void Execute(object? parameter) {
		LoginDialog loginDialog = new();
		bool loginResult = loginDialog.ShowDialog() ?? false;
		this._viewModel.IsLoggedIn = loginResult;
		AuthAPI authApi = App.INSTANCE.VRChatApi.AuthAPI;
		this._viewModel.UserName = authApi.GetUser()?.displayName ?? "Offline";
	}

	public event EventHandler? CanExecuteChanged {
		add { }
		remove { }
	}
}