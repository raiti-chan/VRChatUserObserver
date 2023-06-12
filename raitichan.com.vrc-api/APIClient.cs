using System.Net;
using System.Text;

namespace raitichan.com.vrchat_api;

public sealed class APIClient : IDisposable {
	private const string USER_AGENT = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/114.0.0.0 Safari/537.36";

	private readonly HttpClientHandler _clientHandler;
	private readonly HttpClient _client;

	public CookieContainer CookieContainer => this._clientHandler.CookieContainer;

	public APIClient() {
		this._clientHandler = new HttpClientHandler();
		this._client = new HttpClient(this._clientHandler) {
			BaseAddress = new Uri(VRChatAPI.API_BASE)
		};

		this._client.DefaultRequestHeaders.Add("User-Agent", USER_AGENT);
	}

	public APIClient(Uri uri) {
		this._clientHandler = new HttpClientHandler();
		this._client = new HttpClient(this._clientHandler) {
			BaseAddress = uri
		};
		this._client.DefaultRequestHeaders.Add("User-Agent", USER_AGENT);
	}

	public HttpResponseMessage Get(string endPoint) {
		Task<HttpResponseMessage> getAsyncTask = this._client.GetAsync(endPoint);
		getAsyncTask.Wait();
		return getAsyncTask.Result;
	}

	public HttpResponseMessage Post(string endPoint, HttpContent content) {
		Task<HttpResponseMessage> postAsyncTask = this._client.PostAsync(endPoint, content);
		postAsyncTask.Wait();
		return postAsyncTask.Result;
	}

	public HttpResponseMessage Put(string endPoint) {
		StringContent stringContent = new StringContent("application/json", Encoding.UTF8);
		Task<HttpResponseMessage> putAsyncTask = this._client.PutAsync(endPoint, stringContent);
		putAsyncTask.Wait();
		return putAsyncTask.Result;
	}
	
	public HttpResponseMessage Send(HttpRequestMessage requestMessage) {
		return this._client.Send(requestMessage);
	}
	

	public void Dispose() {
		this._client.Dispose();
	}
}