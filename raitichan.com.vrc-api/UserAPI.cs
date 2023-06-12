using System.Net;
using Newtonsoft.Json;
using raitichan.com.vrchat_api.JsonObject;

namespace raitichan.com.vrchat_api;

public sealed class UserAPI {
	private readonly APIClient _apiClient;

	public UserAPI(APIClient apiClient) {
		this._apiClient = apiClient;
	}

	public UserInfo? GetUser(string userId) {
		using HttpResponseMessage responseMessage = this._apiClient.Get($"users/{userId}");
		if (responseMessage.StatusCode != HttpStatusCode.OK) {
			return null;
		}

		Task<string> readAsStringAsync = responseMessage.Content.ReadAsStringAsync();
		readAsStringAsync.Wait();

		string jsonStr = readAsStringAsync.Result;
		UserInfo? userResult = JsonConvert.DeserializeObject<UserInfo>(jsonStr);
		return userResult;
	}
}