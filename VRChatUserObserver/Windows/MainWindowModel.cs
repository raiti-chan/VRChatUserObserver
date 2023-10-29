namespace VRChatUserObserver.Windows;

public class MainWindowModel {
	public string UserName { get; set; } = "Offline";
	public string ObservingTargetId { get; set; } = string.Empty;
	public string ObservingTargetName { get; set; } = "NONE";
	public string CurrentState { get; set; } = "NONE";
	public string CurrentLocation { get; set; } = "NONE";

	public bool IsLoggedIn { get; set; }
	public bool IsRunning { get; set; }
	public int RunningId { get; set; } = 0;
}