using System.IO;
using System.Text;
using System.Windows;
using Newtonsoft.Json;
using NLog;
using raitichan.com.vrchat_api;
using raitichan.com.vrchat_api.JsonObject;
using VRChatUserObserver.Windows;

namespace VRChatUserObserver {
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App {
		private const string CONFIGURATION_PATH = "./configuration.json";
		private static readonly Logger LOGGER = LogManager.GetCurrentClassLogger();
		public static App INSTANCE { get; private set; } = null!;

		public VRChatAPI VRChatApi { get; private set; } = null!;
		private NotificationIcon NotificationIcon { get; set; } = null!;

		private MainWindowModel _mainWindowModel = null!;
		private MainWindowViewModel _mainWindowViewModel = null!;

		public void ShowMainWindow() {
			LOGGER.Info("Show Window");
			this.MainWindow ??= new MainWindow(this._mainWindowViewModel);
			this.MainWindow.Show();
		}

		public void SendNotification(string title, string? content = null) {
			LOGGER.Info("Notification : {} : {}", title, content);
			this.NotificationIcon.ShowNotification(title, content);
		}

		protected override void OnStartup(StartupEventArgs e) {
			base.OnStartup(e);
			LOGGER.Info("StartUP");

			INSTANCE = this;
			if (File.Exists(CONFIGURATION_PATH)) {
				LOGGER.Info("Load Configuration.");
				Configuration? configuration = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(CONFIGURATION_PATH));
				this.VRChatApi = configuration == null ? new VRChatAPI() : new VRChatAPI(configuration);
			} else {
				this.VRChatApi = new VRChatAPI();
			}

			this.NotificationIcon = new NotificationIcon();

			this._mainWindowModel = new MainWindowModel {
				ObservingTargetId = VRChatUserObserver.Properties.Settings.Default.target
			};
			this._mainWindowViewModel = new MainWindowViewModel(this._mainWindowModel);
			
			LOGGER.Info("Check Token.");
			if (this.VRChatApi.AuthAPI.GetAuth() is { ok: true }) {
				LOGGER.Info("Success!");
				this._mainWindowModel.IsLoggedIn = true;
				this._mainWindowModel.UserName = this.VRChatApi.AuthAPI.GetUser()?.displayName ?? "Offline";
			} else {
				LOGGER.Warn("Failure.");
			}
			
			this.ShowMainWindow();
			
			this.ShutdownMode = ShutdownMode.OnExplicitShutdown;
		}

		protected override void OnExit(ExitEventArgs e) {
			base.OnExit(e);
			LOGGER.Info("Shutdown.");
			this.SaveConfiguration();
			this.VRChatApi.Dispose();
			this.NotificationIcon.Dispose();
		}

		public void SaveConfiguration() {
			LOGGER.Info("SaveConfiguration.");
			Configuration configuration = this.VRChatApi.GetConfiguration();
			File.WriteAllText(CONFIGURATION_PATH, JsonConvert.SerializeObject(configuration), Encoding.UTF8);
		}
	}
}