using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

namespace raitichan.com.vrchat_api;

public sealed class AuthAPI {
	private readonly APIClient _apiClient;

	public AuthAPI(APIClient apiClient) {
		this._apiClient = apiClient;
	}

	public UserResult? GetUser(string userName, string password) {
		string authorization = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{userName}:{password}"));

		HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, "auth/user");
		HttpRequestHeaders headers = requestMessage.Headers;
		headers.Add("Authorization", $"Basic {authorization}");

		HttpResponseMessage responseMessage = this._apiClient.Send(requestMessage);
		Task<string> readAsStringAsync = responseMessage.Content.ReadAsStringAsync();
		readAsStringAsync.Wait();

		if (responseMessage.StatusCode != HttpStatusCode.OK) {
			return null;
		}

		string jsonStr = readAsStringAsync.Result;
		UserResult? userResult = JsonConvert.DeserializeObject<UserResult>(jsonStr);
		return userResult;
	}

	public UserResult? GetUser() {
		HttpResponseMessage responseMessage = this._apiClient.Get("auth/user");
		Task<string> readAsStringAsync = responseMessage.Content.ReadAsStringAsync();
		readAsStringAsync.Wait();

		if (responseMessage.StatusCode != HttpStatusCode.OK) {
			return null;
		}

		string jsonStr = readAsStringAsync.Result;
		UserResult? userResult = JsonConvert.DeserializeObject<UserResult>(jsonStr);
		return userResult;
	}

	public VerifiedResult? PostTowFactorAuthEmailOTPVerify(string code) {
		VerifyContent verifyContent = new VerifyContent { code = code };
		StringContent content = new(JsonConvert.SerializeObject(verifyContent), Encoding.UTF8, "application/json");

		HttpResponseMessage responseMessage = this._apiClient.Post("auth/twofactorauth/emailotp/verify", content);
		Task<string> readAsStringAsync = responseMessage.Content.ReadAsStringAsync();
		readAsStringAsync.Wait();

		if (responseMessage.StatusCode != HttpStatusCode.OK) {
			return null;
		}

		string jsonStr = readAsStringAsync.Result;
		VerifiedResult? verifiedResult = JsonConvert.DeserializeObject<VerifiedResult>(jsonStr);
		return verifiedResult;
	}
}

[JsonObject]
public sealed class VerifyContent {
	[JsonProperty] public string? code;
}

[JsonObject]
public sealed class UserResult {
	[JsonProperty] public string? id;
	[JsonProperty] public string? displayName;
	[JsonProperty] public State? state;

	[JsonProperty] public string? worldId;
	[JsonProperty] public string? location;
	[JsonProperty] public string? instanceId;
	
	[JsonProperty] public string? travelingToInstance;
	[JsonProperty] public string? travelingToLocation;
	[JsonProperty] public string? travelingToWorld;

	[JsonProperty] public string[]? requiresTwoFactorAuth;
}

[JsonObject]
public sealed class VerifiedResult {
	[JsonProperty] public bool verified;
}

public enum State {
	active,
	online,
	offline
}