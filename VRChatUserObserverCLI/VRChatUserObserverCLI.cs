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

			Configuration newConfiguration = vrchatApi.GetConfiguration();
			string newConfigurationStr = JsonConvert.SerializeObject(newConfiguration);
			File.WriteAllText(CONFIGURATION_PATH, newConfigurationStr, Encoding.UTF8);
		}

		UserInfo? loginUserInfo = vrchatApi.AuthAPI.GetUser();
		if (loginUserInfo == null) {
			Console.Error.WriteLine("Login failed.");
			return;
		}
		Console.WriteLine($"You are logged in : {loginUserInfo.displayName}");
	}

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
	Console.WriteLine($"Start Observer.");

	if (target.id == null) return;

	
	XSNotifier xsNotifier = new XSNotifier();
	string? currentWorldId = target.location;

	while (true) {
		Thread.Sleep(60000);
		Console.WriteLine("Update State");
		UserInfo? updateResult = vrchatApi.UserApi.GetUser(target.id);
		if (updateResult == null) {
			Console.Error.WriteLine("Unknown Error");
			continue;
		}

		if (currentWorldId != updateResult.worldId) {
			switch (updateResult.worldId) {
				case "offline": {
					string title = $"{target.displayName} is Offline!";

					Console.WriteLine(title);

					new ToastContentBuilder()
						.AddText(title)
						.Show();

					xsNotifier.SendNotification(new XSNotification() {
						Title = title
					});
					break;
				}
				case "private": {
					string title = $"{target.displayName} in Private World!";

					Console.WriteLine(title);

					new ToastContentBuilder()
						.AddText(title)
						.Show();

					xsNotifier.SendNotification(new XSNotification() {
						Title = title
					});
					break;
				}
				case "traveling": {
					string title = $"{target.displayName} is Traveling World!";
					string worldName = "Unknown";
					if (updateResult.travelingToWorld != null) {
						var worldResult = vrchatApi.WorldApi.GetWorld(updateResult.travelingToWorld);
						worldName = worldResult?.name ?? updateResult.travelingToWorld;
					}

					string content = $"To: {worldName}";

					Console.WriteLine(title);
					Console.WriteLine(content);

					new ToastContentBuilder()
						.AddText(title)
						.AddText(content)
						.Show();

					xsNotifier.SendNotification(new XSNotification() {
						Title = title,
						Content = content
					});
					break;
				}
				default: {
					string worldName = "Unknown";
					if (updateResult.worldId != null) {
						var worldResult = vrchatApi.WorldApi.GetWorld(updateResult.worldId);
						worldName = worldResult?.name ?? updateResult.worldId;
					}

					string title = $"{target.displayName} in {worldName}";

					Console.WriteLine(title);

					new ToastContentBuilder()
						.AddText(title)
						.Show();

					xsNotifier.SendNotification(new XSNotification() {
						Title = title
					});
					break;
				}
			}

			currentWorldId = updateResult.worldId;
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