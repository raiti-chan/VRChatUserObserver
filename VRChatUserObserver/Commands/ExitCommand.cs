using System;
using System.Windows.Input;

namespace VRChatUserObserver.Commands;

public class ExitCommand : ICommand {
	
	public bool CanExecute(object? parameter) {
		return true;
	}

	public void Execute(object? parameter) {
		App.INSTANCE.Shutdown();
	}

	public event EventHandler? CanExecuteChanged {
		add { }
		remove { }
	}
}