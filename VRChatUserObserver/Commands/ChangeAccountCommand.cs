using System;
using System.Windows.Input;
using NLog;
using raitichan.com.vrchat_api;
using VRChatUserObserver.Windows;

namespace VRChatUserObserver.Commands;

public class ChangeAccountCommand : ICommand {
	private static readonly Logger LOGGER = LogManager.GetCurrentClassLogger();
	private readonly MainWindowViewModel _viewModel;

	public ChangeAccountCommand(MainWindowViewModel viewModel) {
		this._viewModel = viewModel;
	}
	
	public bool CanExecute(object? parameter) {
		return true;
	}

	public void Execute(object? parameter) {
		LOGGER.Info("Change Account");
		
		LoginDialog loginDialog = new();
		bool loginResult = loginDialog.ShowDialog() ?? false;
		LOGGER.Info("Change Account Result : {}", loginResult);
		
		this._viewModel.IsLoggedIn = loginResult;
		AuthAPI authApi = App.INSTANCE.VRChatApi.AuthAPI;
		this._viewModel.UserName = authApi.GetUser()?.displayName ?? "Offline";
		LOGGER.Info("Change To : {}", this._viewModel.UserName);
	}

	public event EventHandler? CanExecuteChanged {
		add { }
		remove { }
	}
}