using Newtonsoft.Json;

namespace raitichan.com.vrchat_api.JsonObject;

[JsonObject]
public sealed class WorldInfo {
	[JsonProperty] public string? id;
	[JsonProperty] public string? name;
}