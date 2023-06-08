using Newtonsoft.Json;

namespace raitichan.com.vrchat_api.JsonObject;

[JsonObject]
public sealed class UserInfo {
	[JsonProperty] public string? id;
	[JsonProperty] public string? displayName;
	[JsonProperty] public State state;

	[JsonProperty] public string? worldId;
	[JsonProperty] public string? location;
	[JsonProperty] public string? instanceId;
	
	[JsonProperty] public string? travelingToInstance;
	[JsonProperty] public string? travelingToLocation;
	[JsonProperty] public string? travelingToWorld;

	[JsonProperty] public string[]? requiresTwoFactorAuth;
}


public enum State {
	active,
	online,
	offline
}