using System;
using System.Windows.Input;

namespace VRChatUserObserver.Commands; 

public class StopCommand : ICommand {
	private readonly MainWindowViewModel _viewModel;

	public StopCommand(MainWindowViewModel viewModel) {
		this._viewModel = viewModel;
	}
	
	public bool CanExecute(object? parameter) {
		return this._viewModel is { IsLoggedIn: true, IsRunning: true };
	}

	public void Execute(object? parameter) {
		this._viewModel.IsRunning = false;
	}

	public event EventHandler? CanExecuteChanged;
	
	public void RaiseCanExecuteChanged() {
		CanExecuteChanged?.Invoke(this, EventArgs.Empty);
	}
}