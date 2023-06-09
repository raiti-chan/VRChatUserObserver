using System.Net.WebSockets;
using System.Text;
using Newtonsoft.Json;
using raitichan.com.vrchat_api.JsonObject;

namespace raitichan.com.vrchat_api;

public class WebSocketAPI {
	private const string USER_AGENT = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/114.0.0.0 Safari/537.36";

	// 8kb
	private readonly byte[] _buffer = new byte[1028 * 8];
	
	private readonly ClientWebSocket _client;

	public WebSocketAPI() {
		this._client = new ClientWebSocket();
		this._client.Options.SetRequestHeader("User-Agent", USER_AGENT);
	}

	public void Connection(string auth) {
		this._client.ConnectAsync(new Uri($"{VRChatAPI.WEB_SOCKET_API}?authToken={auth}"), CancellationToken.None).Wait();
	}

	public WebSocketReceiveData? ReceiveMassage() {
		StringBuilder stringBuilder = new();
		Memory<byte> memory = new(this._buffer);
		ValueWebSocketReceiveResult result;
		do {
			ValueTask<ValueWebSocketReceiveResult> receiveAsync = this._client.ReceiveAsync(memory, CancellationToken.None);
			result = receiveAsync.IsCompleted ? receiveAsync.Result : receiveAsync.AsTask().Result;
			stringBuilder.Append(Encoding.UTF8.GetString(_buffer, 0, result.Count));
		} while (!result.EndOfMessage);
		WebSocketReceiveData? webSocketReceiveData = JsonConvert.DeserializeObject<WebSocketReceiveData>(stringBuilder.ToString());
		return webSocketReceiveData;
	}
}