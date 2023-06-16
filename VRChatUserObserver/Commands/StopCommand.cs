using System;
using System.Windows.Input;
using NLog;
using VRChatUserObserver.Windows;

namespace VRChatUserObserver.Commands; 

public class StopCommand : ICommand {
	private static readonly Logger LOGGER = LogManager.GetCurrentClassLogger();
	private readonly MainWindowViewModel _viewModel;

	public StopCommand(MainWindowViewModel viewModel) {
		this._viewModel = viewModel;
	}
	
	public bool CanExecute(object? parameter) {
		return this._viewModel is { IsLoggedIn: true, IsRunning: true };
	}

	public void Execute(object? parameter) {
		LOGGER.Info("Stop");
		this._viewModel.IsRunning = false;
	}

	public event EventHandler? CanExecuteChanged;
	
	public void RaiseCanExecuteChanged() {
		CanExecuteChanged?.Invoke(this, EventArgs.Empty);
	}
}