using System.IO;
using System.Text;
using System.Windows;
using Newtonsoft.Json;
using raitichan.com.vrchat_api;
using raitichan.com.vrchat_api.JsonObject;
using XSNotifications;

namespace VRChatUserObserver {
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App {
		private const string CONFIGURATION_PATH = "./configuration.json";
		public static App INSTANCE { get; private set; } = null!;

		public VRChatAPI VRChatApi { get; private set; } = null!;
		private XSNotifier XsNotifier { get; set; } = null!;
		private NotificationIcon NotificationIcon { get; set; } = null!;

		private MainWindowModel _mainWindowModel = null!;
		private MainWindowViewModel _mainWindowViewModel = null!;

		public void ShowMainWindow() {
			this.MainWindow ??= new MainWindow(this._mainWindowViewModel);
			this.MainWindow.Show();
		}

		public void SendNotification(string title, string? content = null) {
			this.NotificationIcon.ShowNotification(title, content);
		}

		protected override void OnStartup(StartupEventArgs e) {
			base.OnStartup(e);

			INSTANCE = this;
			if (File.Exists(CONFIGURATION_PATH)) {
				Configuration? configuration = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(CONFIGURATION_PATH));
				this.VRChatApi = configuration == null ? new VRChatAPI() : new VRChatAPI(configuration);
			} else {
				this.VRChatApi = new VRChatAPI();
			}

			this.NotificationIcon = new NotificationIcon();
			this.XsNotifier = new XSNotifier();

			this._mainWindowModel = new MainWindowModel();
			this._mainWindowViewModel = new MainWindowViewModel(this._mainWindowModel);
			if (this.VRChatApi.AuthAPI.GetAuth() is { ok: true }) {
				this._mainWindowModel.IsLoggedIn = true;
				this._mainWindowModel.UserName = this.VRChatApi.AuthAPI.GetUser()?.displayName ?? "Offline";
			}

			this.MainWindow = new MainWindow(this._mainWindowViewModel);
			this.MainWindow.Show();


			this.ShutdownMode = ShutdownMode.OnExplicitShutdown;
		}

		protected override void OnExit(ExitEventArgs e) {
			base.OnExit(e);
			Configuration configuration = this.VRChatApi.GetConfiguration();
			File.WriteAllText(CONFIGURATION_PATH, JsonConvert.SerializeObject(configuration), Encoding.UTF8);
			this.VRChatApi.Dispose();
			this.NotificationIcon.Dispose();
			this.XsNotifier.Dispose();
		}
	}
}