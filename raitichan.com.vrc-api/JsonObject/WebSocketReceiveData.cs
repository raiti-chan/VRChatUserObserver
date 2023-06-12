using Newtonsoft.Json;

namespace raitichan.com.vrchat_api.JsonObject; 

[JsonObject]
public class WebSocketReceiveData {
	[JsonProperty] public string? type;
	[JsonProperty] public string? content;
}

[JsonObject]
public class FriendOnline {
	[JsonProperty] public string? userId;
	[JsonProperty] public UserInfo? user;
	[JsonProperty] public string? location;
	[JsonProperty] public string? instance;
	[JsonProperty] public WorldInfo? world;
	[JsonProperty] public bool canRequestInvite;
}

[JsonObject]
public class FriendActive {
	[JsonProperty] public string? userId;
	[JsonProperty] public UserInfo? user;
}

[JsonObject]
public class FriendOffline {
	[JsonProperty] public string? userId;
}

[JsonObject]
public class FriendLocation {
	[JsonProperty] public string? userId;
	[JsonProperty] public UserInfo? user;
	[JsonProperty] public string? location;
	[JsonProperty] public string? instance;
	[JsonProperty] public WorldInfo? world;
	[JsonProperty] public string? travelingToLocation;
	[JsonProperty] public bool canRequestInvite;
}