﻿using Newtonsoft.Json;

namespace raitichan.com.vrchat_api; 

public sealed class SystemAPI {

	private readonly APIClient _apiClient;

	public SystemAPI(APIClient apiClient) {
		this._apiClient = apiClient;
	}

	public ConfigResult? GetConfig() {
		HttpResponseMessage responseMessage = this._apiClient.Get("config");
		Task<string> readAsStringAsync = responseMessage.Content.ReadAsStringAsync();
		readAsStringAsync.Wait();
		string jsonStr = readAsStringAsync.Result;
		ConfigResult? config = JsonConvert.DeserializeObject<ConfigResult>(jsonStr);
		return config;
	}
	
}

[JsonObject]
public sealed class ConfigResult {
	[JsonProperty]
	public string? clientApiKey;
}