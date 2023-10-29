using System.ComponentModel;
using System.Runtime.CompilerServices;
using VRChatUserObserver.Commands;

namespace VRChatUserObserver.Windows; 

public sealed class MainWindowViewModel : INotifyPropertyChanged {
	private readonly MainWindowModel _mainWindowModel;
	
	public event PropertyChangedEventHandler? PropertyChanged;

	public string UserName {
		get => this._mainWindowModel.UserName;
		set {
			if (this._mainWindowModel.UserName == value) return;
			this._mainWindowModel.UserName = value;
			this.OnPropertyChanged();
		}
	}

	public string ObservingTargetId {
		get => this._mainWindowModel.ObservingTargetId;
		set {
			if (this._mainWindowModel.ObservingTargetId == value) return;
			this._mainWindowModel.ObservingTargetId = value;
			this.OnPropertyChanged();
		}
	}

	public string ObservingTargetName {
		get => this._mainWindowModel.ObservingTargetName;
		set {
			if (this._mainWindowModel.ObservingTargetName == value) return;
			this._mainWindowModel.ObservingTargetName = value;
			this.OnPropertyChanged();
		}
	}

	public string CurrentState {
		get => this._mainWindowModel.CurrentState;
		set {
			if (this._mainWindowModel.CurrentState == value) return;
			this._mainWindowModel.CurrentState = value;
			this.OnPropertyChanged();
		}
	}

	public string CurrentLocation {
		get => this._mainWindowModel.CurrentLocation;
		set {
			if (this._mainWindowModel.CurrentLocation == value) return;
			this._mainWindowModel.CurrentLocation = value;
			this.OnPropertyChanged();
		}
	}

	public bool IsLoggedIn {
		get => this._mainWindowModel.IsLoggedIn;
		set {
			if (this._mainWindowModel.IsLoggedIn == value) return;
			this._mainWindowModel.IsLoggedIn = value;
			this.OnPropertyChanged();
			this.StartCommand.RaiseCanExecuteChanged();
			this.StopCommand.RaiseCanExecuteChanged();
		}
	}

	public bool IsRunning {
		get => this._mainWindowModel.IsRunning;
		set {
			if (this._mainWindowModel.IsRunning == value) return;
			this._mainWindowModel.IsRunning = value;
			this.OnPropertyChanged();
			this.OnPropertyChanged(nameof(CanChangeTarget));
			this.StartCommand.RaiseCanExecuteChanged();
			this.StopCommand.RaiseCanExecuteChanged();
		}
	}

	public int RunningId {
		get => this._mainWindowModel.RunningId;
		set {
			if (this._mainWindowModel.RunningId == value) return;
			this._mainWindowModel.RunningId = value;
			this.OnPropertyChanged();
		}
	}

	public bool CanChangeTarget => !this._mainWindowModel.IsRunning;

	public ExitCommand ExitCommand { get; }
	public ChangeAccountCommand ChangeAccountCommand { get; }
	public StartCommand StartCommand { get; }
	public StopCommand StopCommand { get; }

	public MainWindowViewModel() : this(new MainWindowModel()) {
	}

	public MainWindowViewModel(MainWindowModel model) {
		this._mainWindowModel = model;
		this.ExitCommand = new ExitCommand();
		this.ChangeAccountCommand = new ChangeAccountCommand(this);
		this.StartCommand = new StartCommand(this);
		this.StopCommand = new StopCommand(this);
	}

	private void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}