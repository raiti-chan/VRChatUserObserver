using System.Windows;
using raitichan.com.vrchat_api;

namespace VRChatUserObserver {
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App {

		public static App INSTANCE { get; private set; } = null!;
		public APIClient ApiClient { get; private set; } = null!;
		
		protected override void OnStartup(StartupEventArgs e) {
			base.OnStartup(e);
			INSTANCE = this;
			this.ApiClient = new APIClient();
		}
	}
}
