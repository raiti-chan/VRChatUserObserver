using Microsoft.Toolkit.Uwp.Notifications;
using raitichan.com.vrchat_api;
using XSNotifications;

Console.Write("Id: ");
string? id = Console.ReadLine();
if (id == null) return;

Console.Write("Password: ");
string? password = Console.ReadLine();
if (password == null) return;

APIClient apiClient = new();
AuthAPI authApi = new(apiClient);
UserResult? userResult = authApi.GetUser(id, password);
if (userResult is null) {
	Console.Error.WriteLine("The id or password is different.");
	return;
}

Console.Write("Code: ");
string? code = Console.ReadLine();
if (code == null) return;
VerifiedResult? verifiedResult = authApi.PostTowFactorAuthEmailOTPVerify(code);

if (verifiedResult is not { verified: true }) {
	Console.Error.WriteLine("Login failed.");
	return;
}

userResult = authApi.GetUser();
if (userResult is null) {
	Console.Error.WriteLine("Login failed.");
	return;
}

Console.WriteLine($"You are logged in : {userResult.displayName}");


UserAPI userApi = new(apiClient);
UserResult target;
while (true) {
	Console.Write("Observing target id: ");
	string? observingTargetId = Console.ReadLine();
	if (string.IsNullOrEmpty(observingTargetId)) {
		Console.Error.WriteLine("Please input.");
		continue;
	}

	UserResult? targetUser = userApi.GetUser(observingTargetId);
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

WorldAPI worldApi = new WorldAPI(apiClient);
XSNotifier xsNotifier = new XSNotifier();


string? currentWorldId = target.location;

while (true) {
	Thread.Sleep(60000);
	Console.WriteLine("Update State");
	UserResult? updateResult = userApi.GetUser(target.id);
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
					var worldResult = worldApi.GetWorld(updateResult.travelingToWorld);
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
					var worldResult = worldApi.GetWorld(updateResult.worldId);
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