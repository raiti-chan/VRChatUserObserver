using System.Net;
using Newtonsoft.Json;
using raitichan.com.vrchat_api.JsonObject;

namespace raitichan.com.vrchat_api;

public class WorldAPI {
	private readonly APIClient _apiClient;

	public WorldAPI(APIClient apiClient) {
		this._apiClient = apiClient;
	}

	public WorldInfo? GetWorld(string worldId) {
		HttpResponseMessage responseMessage = this._apiClient.Get($"worlds/{worldId}");
		if (responseMessage.StatusCode != HttpStatusCode.OK) {
			return null;
		}

		Task<string> readAsStringAsync = responseMessage.Content.ReadAsStringAsync();
		readAsStringAsync.Wait();


		string jsonStr = readAsStringAsync.Result;
		WorldInfo? worldResult = JsonConvert.DeserializeObject<WorldInfo>(jsonStr);
		return worldResult;
	}
}