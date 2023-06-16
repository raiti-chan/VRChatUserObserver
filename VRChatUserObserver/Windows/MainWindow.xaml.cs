using System.Windows;

namespace VRChatUserObserver.Windows;

public partial class MainWindow : Window {
	public MainWindow() {
		InitializeComponent();
	}

	public MainWindow(MainWindowViewModel viewModel) : this() {
		this.DataContext = viewModel;
	}
}