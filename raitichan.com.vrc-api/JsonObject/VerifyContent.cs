using Newtonsoft.Json;

namespace raitichan.com.vrchat_api.JsonObject;

[JsonObject]
public sealed class VerifyContent {
	[JsonProperty] public string? code;
}