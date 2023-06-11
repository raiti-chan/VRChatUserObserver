using System.Net;
using raitichan.com.vrchat_api.JsonObject;

namespace raitichan.com.vrchat_api;

public class VRChatAPI {
	public const string API_BASE = "https://api.vrchat.cloud/api/1/";
	public const string WEB_SOCKET_API = "wss://vrchat.com/";

	private static readonly Uri API_BASE_URI = new(API_BASE);

	private readonly APIClient _apiClient;

	public SystemAPI SystemApi { get; }
	public AuthAPI AuthAPI { get; }
	public UserAPI UserApi { get; }
	public WorldAPI WorldApi { get; }


	public VRChatAPI() {
		this._apiClient = new APIClient(API_BASE_URI);
		this.SystemApi = new SystemAPI(this._apiClient);
		this.AuthAPI = new AuthAPI(this._apiClient);
		this.UserApi = new UserAPI(this._apiClient);
		this.WorldApi = new WorldAPI(this._apiClient);
	}

	public VRChatAPI(Configuration configuration) : this() {
		CookieContainer container = this._apiClient.CookieContainer;
		container.Add(API_BASE_URI, new Cookie(nameof(Configuration.auth), configuration.auth ?? "") { Path = "/" });
		container.Add(API_BASE_URI, new Cookie(nameof(Configuration.twoFactorAuth), configuration.twoFactorAuth ?? "") { Path = "/" });
	}

	public Configuration GetConfiguration() {
		CookieContainer container = this._apiClient.CookieContainer;
		CookieCollection collection = container.GetCookies(API_BASE_URI);
		Configuration configuration = new();

		foreach (Cookie cookie in collection) {
			switch (cookie.Name) {
				case nameof(configuration.auth):
					configuration.auth = cookie.Value;
					break;
				case nameof(configuration.twoFactorAuth):
					configuration.twoFactorAuth = cookie.Value;
					break;
			}
		}

		return configuration;
	}
	
	public WebSocketAPI CreateWebSocketApi() {
		return new WebSocketAPI();
	}
}