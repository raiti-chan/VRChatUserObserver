using Newtonsoft.Json;

namespace raitichan.com.vrchat_api.JsonObject;

[JsonObject]
public sealed class VerifyResult {
	[JsonProperty] public bool verified;
}