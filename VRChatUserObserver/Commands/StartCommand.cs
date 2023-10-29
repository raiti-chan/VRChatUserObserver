using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json;
using NLog;
using raitichan.com.vrchat_api;
using raitichan.com.vrchat_api.JsonObject;
using VRChatUserObserver.Windows;
using XSNotifications;

namespace VRChatUserObserver.Commands;

public class StartCommand : ICommand {
	private static readonly Logger LOGGER = LogManager.GetCurrentClassLogger();
	private readonly MainWindowViewModel _viewModel;

	public StartCommand(MainWindowViewModel viewModel) {
		this._viewModel = viewModel;
	}

	public bool CanExecute(object? parameter) {
		return this._viewModel is { IsLoggedIn: true, IsRunning: false };
	}

	public void Execute(object? parameter) {
		LOGGER.Info("Start : {}", this._viewModel.ObservingTargetId);
		if (string.IsNullOrEmpty(this._viewModel.ObservingTargetId)) {
			LOGGER.Info("Target not set.");
			return;
		}

		LOGGER.Info("Check Auth.");
		AuthResult? authResult = App.INSTANCE.VRChatApi.AuthAPI.GetAuth();
		if (authResult is not { ok: true }) {
			LOGGER.Warn("Failure.");
			return;
		}

		LOGGER.Info("Success.");

		UserAPI userApi = App.INSTANCE.VRChatApi.UserApi;
		UserInfo? userInfo = userApi.GetUser(this._viewModel.ObservingTargetId);
		if (userInfo == null) {
			LOGGER.Warn("User information could not be retrieved.");
			return;
		}

		Properties.Settings.Default.target = this._viewModel.ObservingTargetId;
		Properties.Settings.Default.Save();

		this._viewModel.ObservingTargetName = userInfo.displayName ?? "NONE";
		this._viewModel.CurrentState = userInfo.state == State.active ? "offline" : userInfo.state.ToString();
		this._viewModel.CurrentLocation = userInfo.state == State.offline ? "offline" : App.INSTANCE.VRChatApi.WorldApi.GetWorld(userInfo.worldId ?? string.Empty)?.name ?? "private";
		this._viewModel.IsRunning = true;
		this._viewModel.RunningId++;
		new Task(this.Run, (authResult.token, userInfo.state, userInfo.location)).Start();
	}

	private void Run(object? param) {
		int runningId = this._viewModel.RunningId;
		LOGGER.Info($"Start Observing thread ID:{runningId}");
		if (param is not (string token, State currentState, string currentLocation)) {
			this._viewModel.IsRunning = false;
			return;
		}

		while (this._viewModel.IsRunning && runningId == this._viewModel.RunningId) {
			try {
				LOGGER.Info("Check Auth.");
				if (App.INSTANCE.VRChatApi.AuthAPI.GetAuth() is not { ok: true }) {
					LOGGER.Warn("Failure.");
					LOGGER.Warn("Stopping Ovserving thread.");
					this._viewModel.IsRunning = false;
					continue;
				}

				LOGGER.Info("Connect to Websocket.");
				using WebSocketAPI webSocketApi = App.INSTANCE.VRChatApi.CreateWebSocketApi();
				webSocketApi.Connection(token);

				LOGGER.Info("Create XSNotifier.");
				using XSNotifier xsNotifier = new();

				LOGGER.Info("Start observing loop.");
				while (this._viewModel.IsRunning && runningId == this._viewModel.RunningId) {
					WebSocketReceiveData? receiveData = webSocketApi.ReceiveMassage();
					if (receiveData?.content == null) continue;
					if (!this._viewModel.IsRunning) break;
					if (runningId != this._viewModel.RunningId) break;

					switch (receiveData.type) {
						case "friend-online": {
							if (currentState != State.online) {
								FriendOnline? friendOnline = JsonConvert.DeserializeObject<FriendOnline>(receiveData.content);
								if (friendOnline?.userId != this._viewModel.ObservingTargetId) break;
								
								LOGGER.Info("{} to Online", this._viewModel.ObservingTargetName);
								LOGGER.Info(JsonConvert.SerializeObject(friendOnline));
								
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
								
								LOGGER.Info("{} to Active", this._viewModel.ObservingTargetName);
								LOGGER.Info(JsonConvert.SerializeObject(friendActive));

								
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
								
								LOGGER.Info("{} to Offline", this._viewModel.ObservingTargetName);
								LOGGER.Info(JsonConvert.SerializeObject(friendOffline));
								
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
									LOGGER.Info("{} Location", this._viewModel.ObservingTargetName);
									LOGGER.Info(JsonConvert.SerializeObject(friendLocation));
									
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
									LOGGER.Info("{} Location", this._viewModel.ObservingTargetName);
									LOGGER.Info(JsonConvert.SerializeObject(friendLocation));
									
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

				LOGGER.Info("Stop observing loop.");
			} catch (Exception e) {
				LOGGER.Error(e);
				File.WriteAllText("./CrashReport.txt", e.ToString());
				LOGGER.Info("Reconnect.");
			}
		}

		LOGGER.Info($"Stop Observing thread ID:{runningId}");
	}

	private static void Notification(XSNotifier xsNotifier, string title, string? content = null) {
		if (content != null) {
			LOGGER.Info("{} : {}", title, content);
			App.INSTANCE.SendNotification(title, content);
			xsNotifier.SendNotification(new XSNotification { Title = title, Content = content });
		} else {
			LOGGER.Info(title);
			App.INSTANCE.SendNotification(title);
			xsNotifier.SendNotification(new XSNotification { Title = title });
		}
	}


	public event EventHandler? CanExecuteChanged;

	public void RaiseCanExecuteChanged() {
		CanExecuteChanged?.Invoke(this, EventArgs.Empty);
	}
}