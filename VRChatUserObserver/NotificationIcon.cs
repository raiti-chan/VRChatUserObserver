using System;
using System.Drawing;
using System.Windows.Forms;

namespace VRChatUserObserver; 

public sealed class NotificationIcon : IDisposable {
	private readonly NotifyIcon _notifyIcon;
	private readonly ContextMenuStrip _menu;

	public NotificationIcon() {
		this._menu = new ContextMenuStrip();
		this._notifyIcon = new NotifyIcon {
			Visible = true,
			Icon = SystemIcons.Application,
			Text = "VRChat User Observer",
			ContextMenuStrip = this._menu
		};
		this._notifyIcon.DoubleClick += OnShowWindow;
		this._menu.Items.Add("Show Window", null, OnShowWindow);
		this._menu.Items.Add("-");
		this._menu.Items.Add("Exit", null, OnExit);
	}

	public void ShowNotification(string title, string? content) {
		this._notifyIcon.ShowBalloonTip(30, title, content ?? "", ToolTipIcon.Info);
	}
	
	private static void OnShowWindow(object? sender, EventArgs e) {
		App.INSTANCE.ShowMainWindow();
	}
	
	private static void OnExit(object? sender, EventArgs e) {
		App.INSTANCE.Shutdown();
	}

	public void Dispose() {
		this._notifyIcon.Dispose();
		this._menu.Dispose();
	}
}