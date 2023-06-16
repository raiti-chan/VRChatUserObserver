using System;
using System.Windows.Input;
using NLog;

namespace VRChatUserObserver.Commands;

public class ExitCommand : ICommand {
	private static readonly Logger LOGGER = LogManager.GetCurrentClassLogger();
	
	public bool CanExecute(object? parameter) {
		return true;
	}

	public void Execute(object? parameter) {
		LOGGER.Info("Exit");
		App.INSTANCE.Shutdown();
	}

	public event EventHandler? CanExecuteChanged {
		add { }
		remove { }
	}
}