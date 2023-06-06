using System.Net;
using Newtonsoft.Json;

namespace raitichan.com.vrchat_api;

public class WorldAPI {
	private readonly APIClient _apiClient;

	public WorldAPI(APIClient apiClient) {
		this._apiClient = apiClient;
	}

	public WorldResult? GetWorld(string worldId) {
		HttpResponseMessage responseMessage = this._apiClient.Get($"worlds/{worldId}");
		Task<string> readAsStringAsync = responseMessage.Content.ReadAsStringAsync();
		readAsStringAsync.Wait();

		if (responseMessage.StatusCode != HttpStatusCode.OK) {
			return null;
		}

		string jsonStr = readAsStringAsync.Result;
		WorldResult? worldResult = JsonConvert.DeserializeObject<WorldResult>(jsonStr);
		return worldResult;
	}
}

[JsonObject]
public sealed class WorldResult {
	[JsonProperty] public string? name;
}