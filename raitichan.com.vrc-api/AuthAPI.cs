﻿using System.Net;
using System.Text;
using Newtonsoft.Json;
using raitichan.com.vrchat_api.JsonObject;

namespace raitichan.com.vrchat_api;

public sealed class AuthAPI {
	private readonly APIClient _apiClient;

	public AuthAPI(APIClient apiClient) {
		this._apiClient = apiClient;
	}

	public UserInfo? GetUser(string userName, string password) {
		string authorization = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{userName}:{password}"));

		using HttpRequestMessage requestMessage = new(HttpMethod.Get, "auth/user");
		requestMessage.Headers.Add("Authorization", $"Basic {authorization}");

		using HttpResponseMessage responseMessage = this._apiClient.Send(requestMessage);
		if (responseMessage.StatusCode != HttpStatusCode.OK) {
			return null;
		}

		Task<string> readAsStringAsync = responseMessage.Content.ReadAsStringAsync();
		readAsStringAsync.Wait();

		string jsonStr = readAsStringAsync.Result;
		UserInfo? userResult = JsonConvert.DeserializeObject<UserInfo>(jsonStr);
		return userResult;
	}

	public UserInfo? GetUser() {
		using HttpResponseMessage responseMessage = this._apiClient.Get("auth/user");
		if (responseMessage.StatusCode != HttpStatusCode.OK) {
			return null;
		}

		Task<string> readAsStringAsync = responseMessage.Content.ReadAsStringAsync();
		readAsStringAsync.Wait();

		string jsonStr = readAsStringAsync.Result;
		UserInfo? userResult = JsonConvert.DeserializeObject<UserInfo>(jsonStr);
		return userResult;
	}

	public VerifyResult? PostTwoFactorAuthEmailOTPVerify(string code) {
		VerifyContent verifyContent = new() { code = code };
		using StringContent content = new(JsonConvert.SerializeObject(verifyContent), Encoding.UTF8, "application/json");

		using HttpResponseMessage responseMessage = this._apiClient.Post("auth/twofactorauth/emailotp/verify", content);
		if (responseMessage.StatusCode != HttpStatusCode.OK) {
			return null;
		}
		
		Task<string> readAsStringAsync = responseMessage.Content.ReadAsStringAsync();
		readAsStringAsync.Wait();

		string jsonStr = readAsStringAsync.Result;
		VerifyResult? verifiedResult = JsonConvert.DeserializeObject<VerifyResult>(jsonStr);
		return verifiedResult;
	}

	public VerifyResult? PostTwoFactorAuthTOTPVerify(string code) {
		VerifyContent verifyContent = new() { code = code };
		using StringContent content = new(JsonConvert.SerializeObject(verifyContent), Encoding.UTF8, "application/json");

		using HttpResponseMessage responseMessage = this._apiClient.Post("auth/twofactorauth/totp/verify", content);
		if (responseMessage.StatusCode != HttpStatusCode.OK) {
			return null;
		}

		Task<string> readAsStringAsync = responseMessage.Content.ReadAsStringAsync();
		readAsStringAsync.Wait();

		string jsonStr = readAsStringAsync.Result;
		VerifyResult? verifyResult = JsonConvert.DeserializeObject<VerifyResult>(jsonStr);
		return verifyResult;
	}

	public VerifyResult? PostTwoFactorAuthTOTPVerify(string userName, string password,string code) {
		string authorization = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{userName}:{password}"));
		VerifyContent verifyContent = new() { code = code };
		using StringContent content = new(JsonConvert.SerializeObject(verifyContent), Encoding.UTF8, "application/json");

		using HttpRequestMessage requestMessage = new(HttpMethod.Get, "auth/twofactorauth/totp/verify") {
			Content = content
		};
		requestMessage.Headers.Add("Authorization", $"Basic {authorization}");
		
		using HttpResponseMessage responseMessage = this._apiClient.Send(requestMessage);
		if (responseMessage.StatusCode != HttpStatusCode.OK) {
			return null;
		}

		Task<string> readAsStringAsync = responseMessage.Content.ReadAsStringAsync();
		readAsStringAsync.Wait();

		string jsonStr = readAsStringAsync.Result;
		VerifyResult? verifyResult = JsonConvert.DeserializeObject<VerifyResult>(jsonStr);
		return verifyResult;
	}

	public AuthResult? GetAuth() {
		using HttpResponseMessage responseMessage = this._apiClient.Get("auth");
		if (responseMessage.StatusCode != HttpStatusCode.OK) {
			return null;
		}
		
		Task<string> readAsStringAsync = responseMessage.Content.ReadAsStringAsync();
		readAsStringAsync.Wait();
		
		string jsonStr = readAsStringAsync.Result;
		AuthResult? authResult = JsonConvert.DeserializeObject<AuthResult>(jsonStr);
		return authResult;
	}

	public void PutLogout() {
		this._apiClient.Put("logout");
	}
}
