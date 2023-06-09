using System.Text;
using Microsoft.Toolkit.Uwp.Notifications;
using Newtonsoft.Json;
using raitichan.com.vrchat_api;
using raitichan.com.vrchat_api.JsonObject;
using XSNotifications;

const string CONFIGURATION_PATH = "./configuration.json";
{
	/* main */
	Configuration? configuration = LoadConfig();
	VRChatAPI vrchatApi = configuration is null ? new VRChatAPI() : new VRChatAPI(configuration);

	{
		// 認証チェック
		AuthResult? authResult = vrchatApi.AuthAPI.GetAuth();
		if (authResult is not { ok: true }) {
			Console.Write("Id: ");
			string id = Console.ReadLine() ?? "";

			Console.Write("Password: ");
			string password = Console.ReadLine() ?? "";
			UserInfo? userInfo = vrchatApi.AuthAPI.GetUser(id, password);
			if (userInfo is null) {
				Console.Error.WriteLine("The id or password is different.");
				return;
			}

			authResult = vrchatApi.AuthAPI.GetAuth();
			if (authResult is not { ok: true }) {
				Console.Write("Code: ");
				string code = Console.ReadLine() ?? "";
				VerifyResult? verifiedResult = vrchatApi.AuthAPI.PostTowFactorAuthEmailOTPVerify(code);
				if (verifiedResult is not { verified: true }) {
					Console.Error.WriteLine("Login failed.");
					return;
				}

				authResult = vrchatApi.AuthAPI.GetAuth();
				if (authResult is not { ok: true }) {
					Console.Error.WriteLine("Login failed.");
					return;
				}
			}

			configuration = vrchatApi.GetConfiguration();
			string configurationStr = JsonConvert.SerializeObject(configuration);
			File.WriteAllText(CONFIGURATION_PATH, configurationStr, Encoding.UTF8);
		}

		UserInfo? loginUserInfo = vrchatApi.AuthAPI.GetUser();
		if (loginUserInfo == null) {
			Console.Error.WriteLine("Login failed.");
			return;
		}

		Console.WriteLine($"You are logged in : {loginUserInfo.displayName}");
	}
	if (configuration?.auth == null) return;

	UserInfo target;
	while (true) {
		Console.Write("Observing target id: ");
		string? observingTargetId = Console.ReadLine();
		if (string.IsNullOrEmpty(observingTargetId)) {
			Console.Error.WriteLine("Please input.");
			continue;
		}

		UserInfo? targetUser = vrchatApi.UserApi.GetUser(observingTargetId);
		if (targetUser == null) {
			Console.Error.WriteLine("Not found User.");
			continue;
		}

		target = targetUser;
		break;
	}

	Console.WriteLine($"Set target: {target.displayName}");
	Console.WriteLine("Current Status");
	Console.WriteLine($"State: {target.state}");
	Console.WriteLine($"Location: {target.location}");
	Console.WriteLine($"TravelingTo: {target.travelingToWorld}");
	Console.WriteLine("Start Observer.");

	State currentState = target.state;
	string currentLocation = target.location ?? "";

	if (target.id == null) return;

	XSNotifier xsNotifier = new();

	WebSocketAPI webSocketApi = vrchatApi.WebSocketApi;
	webSocketApi.Connection(configuration.auth);

	while (true) {
		WebSocketReceiveData? receiveData = webSocketApi.ReceiveMassage();
		if (receiveData?.content == null) continue;
		switch (receiveData.type) {
			case "friend-online": {
				if (currentState != State.online) {
					FriendOnline? friendOnline = JsonConvert.DeserializeObject<FriendOnline>(receiveData.content);
					if (friendOnline?.userId != target.id) break;
					currentState = State.online;
					currentLocation = friendOnline.location ?? "private";

					string title = $"{target.displayName} is Online!";
					string content = friendOnline.world?.name ?? "Private";

					Notification(xsNotifier, title, content);
				}

				break;
			}
			case "friend-active": {
				if (currentState == State.online) {
					FriendActive? friendActive = JsonConvert.DeserializeObject<FriendActive>(receiveData.content);
					if (friendActive?.userId != target.id) break;
					currentState = State.active;

					string title = $"{target.displayName} is Offline!";

					Notification(xsNotifier, title);
				}

				break;
			}
			case "friend-offline": {
				if (currentState == State.online) {
					FriendOffline? friendOffline = JsonConvert.DeserializeObject<FriendOffline>(receiveData.content);
					if (friendOffline?.userId != target.id) break;
					currentState = State.offline;

					string title = $"{target.displayName} is Offline!";

					Notification(xsNotifier, title);
				}

				break;
			}
			case "friend-location": {
				FriendLocation? friendLocation = JsonConvert.DeserializeObject<FriendLocation>(receiveData.content);
				if (friendLocation?.userId != target.id) break;
				if (currentLocation != friendLocation.location) {
					currentLocation = friendLocation.location ?? "private";

					string title = $"{target.displayName} Traveled!";
					string content = friendLocation.world?.name ?? "Private";

					Notification(xsNotifier, title, content);
				}

				break;
			}
		}
	}
}


Configuration? LoadConfig() {
	if (!File.Exists(CONFIGURATION_PATH)) {
		return null;
	}

	// トークンを読み込む
	string configurationJson = File.ReadAllText(CONFIGURATION_PATH, Encoding.UTF8);
	Configuration? configuration = JsonConvert.DeserializeObject<Configuration>(configurationJson);
	return configuration;
}

void Notification(XSNotifier xsNotifier, string title, string? content = null) {
	if (content != null) {
		Console.WriteLine($"{title} : {content}");
		new ToastContentBuilder().AddText(title).AddText(content).Show();
		xsNotifier.SendNotification(new XSNotification { Title = title, Content = content });
	} else {
		Console.WriteLine(title);
		new ToastContentBuilder().AddText(title).Show();
		xsNotifier.SendNotification(new XSNotification { Title = title });
	}
}