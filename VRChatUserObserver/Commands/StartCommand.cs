using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json;
using raitichan.com.vrchat_api;
using raitichan.com.vrchat_api.JsonObject;
using XSNotifications;

namespace VRChatUserObserver.Commands;

public class StartCommand : ICommand {
	private readonly MainWindowViewModel _viewModel;

	public StartCommand(MainWindowViewModel viewModel) {
		this._viewModel = viewModel;
	}

	public bool CanExecute(object? parameter) {
		return this._viewModel is { IsLoggedIn: true, IsRunning: false };
	}

	public void Execute(object? parameter) {
		if (string.IsNullOrEmpty(this._viewModel.ObservingTargetId)) {
			return;
		}

		AuthResult? authResult = App.INSTANCE.VRChatApi.AuthAPI.GetAuth();
		if (authResult is not { ok: true }) {
			return;
		}

		UserAPI userApi = App.INSTANCE.VRChatApi.UserApi;
		UserInfo? userInfo = userApi.GetUser(this._viewModel.ObservingTargetId);
		if (userInfo == null) {
			return;
		}

		this._viewModel.ObservingTargetName = userInfo.displayName ?? "NONE";
		this._viewModel.CurrentState = userInfo.state == State.active ? "offline" : userInfo.state.ToString();
		this._viewModel.CurrentLocation = userInfo.state == State.offline ? "offline" : App.INSTANCE.VRChatApi.WorldApi.GetWorld(userInfo.worldId ?? string.Empty)?.name ?? "private";
		this._viewModel.IsRunning = true;
		new Task(this.Run, (authResult.token, userInfo.state, userInfo.location)).Start();
	}

	private void Run(object? param) {
		if (param is not (string token, State currentState, string currentLocation)) {
			this._viewModel.IsRunning = false;
			return;
		}

		while (this._viewModel.IsRunning) {
			try {
				if (App.INSTANCE.VRChatApi.AuthAPI.GetAuth() is not { ok: true }) {
					this._viewModel.IsRunning = false;
					return;
				}

				using WebSocketAPI webSocketApi = App.INSTANCE.VRChatApi.CreateWebSocketApi();
				using XSNotifier xsNotifier = new();
				webSocketApi.Connection(token);
				while (this._viewModel.IsRunning) {
					WebSocketReceiveData? receiveData = webSocketApi.ReceiveMassage();
					if (receiveData?.content == null) continue;
					switch (receiveData.type) {
						case "friend-online": {
							if (currentState != State.online) {
								FriendOnline? friendOnline = JsonConvert.DeserializeObject<FriendOnline>(receiveData.content);
								if (friendOnline?.userId != this._viewModel.ObservingTargetId) break;
								currentState = State.online;
								currentLocation = friendOnline.location ?? "private";
								this._viewModel.CurrentState = "online";
								this._viewModel.CurrentLocation = friendOnline.world?.name ?? "private";

								string title = $"{this._viewModel.ObservingTargetName} is Online!";
								string content = this._viewModel.CurrentLocation;

								Notification(xsNotifier, title, content);
							}

							break;
						}
						case "friend-active": {
							if (currentState == State.online) {
								FriendActive? friendActive = JsonConvert.DeserializeObject<FriendActive>(receiveData.content);
								if (friendActive?.userId != this._viewModel.ObservingTargetId) break;
								currentState = State.active;
								currentLocation = "offline";
								this._viewModel.CurrentState = "offline";
								this._viewModel.CurrentLocation = "offline";

								string title = $"{this._viewModel.ObservingTargetName} is Offline!";

								Notification(xsNotifier, title);
							}

							break;
						}
						case "friend-offline": {
							if (currentState == State.online) {
								FriendOffline? friendOffline = JsonConvert.DeserializeObject<FriendOffline>(receiveData.content);
								if (friendOffline?.userId != this._viewModel.ObservingTargetId) break;
								currentState = State.offline;
								currentLocation = "offline";
								this._viewModel.CurrentState = "offline";
								this._viewModel.CurrentLocation = "offline";

								string title = $"{this._viewModel.ObservingTargetName} is Offline!";

								Notification(xsNotifier, title);
							}

							break;
						}
						case "friend-location": {
							FriendLocation? friendLocation = JsonConvert.DeserializeObject<FriendLocation>(receiveData.content);
							if (friendLocation?.userId != this._viewModel.ObservingTargetId) break;
							if (friendLocation.location == "traveling") {
								string location = friendLocation.travelingToLocation ?? "private";
								if (currentLocation != location) {
									currentLocation = location;
									string worldId = new Instance(location).WorldId;
									this._viewModel.CurrentLocation = App.INSTANCE.VRChatApi.WorldApi.GetWorld(worldId)?.name ?? "private";

									string title = $"{this._viewModel.ObservingTargetName} Traveled!";
									string content = this._viewModel.CurrentLocation;

									Notification(xsNotifier, title, content);
								}
							} else {
								string location = friendLocation.location ?? "private";
								if (currentLocation != location) {
									currentLocation = location;
									this._viewModel.CurrentLocation = friendLocation.world?.name ?? "private";

									string title = $"{this._viewModel.ObservingTargetName} Traveled!";
									string content = this._viewModel.CurrentLocation;

									Notification(xsNotifier, title, content);
								}
							}

							break;
						}
					}
				}
			} catch (Exception e) {
				File.WriteAllText("./CrashReport.txt", e.ToString());
			}
		}
	}

	private static void Notification(XSNotifier xsNotifier, string title, string? content = null) {
		if (content != null) {
			Console.WriteLine($"{title} : {content}");
			App.INSTANCE.SendNotification(title, content);
			xsNotifier.SendNotification(new XSNotification { Title = title, Content = content });
		} else {
			Console.WriteLine(title);
			App.INSTANCE.SendNotification(title);
			xsNotifier.SendNotification(new XSNotification { Title = title });
		}
	}


	public event EventHandler? CanExecuteChanged;

	public void RaiseCanExecuteChanged() {
		CanExecuteChanged?.Invoke(this, EventArgs.Empty);
	}
}