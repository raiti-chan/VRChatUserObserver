using System.Net;
using Newtonsoft.Json;

namespace raitichan.com.vrchat_api;

public sealed class UserAPI {
	private readonly APIClient _apiClient;

	public UserAPI(APIClient apiClient) {
		this._apiClient = apiClient;
	}

	public UserResult? GetUser(string userId) {
		HttpResponseMessage responseMessage = this._apiClient.Get($"users/{userId}");
		Task<string> readAsStringAsync = responseMessage.Content.ReadAsStringAsync();
		readAsStringAsync.Wait();

		if (responseMessage.StatusCode != HttpStatusCode.OK) {
			return null;
		}

		string jsonStr = readAsStringAsync.Result;
		UserResult? userResult = JsonConvert.DeserializeObject<UserResult>(jsonStr);
		return userResult;
	}
}