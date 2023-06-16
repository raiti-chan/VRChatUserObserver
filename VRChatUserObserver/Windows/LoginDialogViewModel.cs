using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using VRChatUserObserver.Commands;

namespace VRChatUserObserver.Windows; 

public sealed class LoginDialogViewModel : INotifyPropertyChanged {
	private readonly LoginDialogModel _loginDialogModel;

	public string UserName {
		get => this._loginDialogModel.UserName;
		set {
			if (this._loginDialogModel.UserName == value) return;
			this._loginDialogModel.UserName = value;
			this.OnPropertyChanged();
		}
	}

	public string Password {
		get => this._loginDialogModel.Password;
		set {
			if (this._loginDialogModel.Password == value) return;
			this._loginDialogModel.Password = value;
			this.OnPropertyChanged();
		}
	}
	
	public LoginCommand LoginCommand { get; }

	public LoginDialogViewModel() {
		this._loginDialogModel = new LoginDialogModel();
		this.LoginCommand = new LoginCommand(this);
	}

	public event Action<bool>? OnCloseDialog;

	public void CloseDialog(bool result) {
		this.OnCloseDialog?.Invoke(result);
	} 
	
	public event PropertyChangedEventHandler? PropertyChanged;

	private void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}