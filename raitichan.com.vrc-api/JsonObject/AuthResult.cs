using Newtonsoft.Json;

namespace raitichan.com.vrchat_api.JsonObject;

[JsonObject]
public sealed class AuthResult {
	[JsonProperty] public bool ok;
	[JsonProperty] public string? token;
}