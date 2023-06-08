using Newtonsoft.Json;

namespace raitichan.com.vrchat_api.JsonObject;

[JsonObject]
public class Configuration {
	[JsonProperty] public string? auth;
	[JsonProperty] public string? twoFactorAuth;
}