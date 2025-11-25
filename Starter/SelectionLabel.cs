using System;
using System.Diagnostics;
using Godot;

namespace Starter;

public partial class SelectionLabel : Label
{
	readonly byte Index;
	private static byte maxIndex = 0;
	
	public SelectionLabel() {
		this.Index = maxIndex++;
	}
	

	public override void _Input(InputEvent @event) {
		return;
		if (@event is not InputEventKey { Pressed: true } keyPress) return;

		MenuPanel menuPanel;
		try {
			menuPanel = (MenuPanel) this.FindParent("MenuPanel");
		} catch (InvalidCastException invalidCastException) {
			Console.WriteLine($"{invalidCastException.Message}");
			return;
		}
		
		// TODO Joystick button
		if (keyPress.Keycode == Key.Enter && this.Index == menuPanel.Selected) {
			// TODO 
			//this.GetTree().ChangeSceneToPacked("../../"); // oder ...file?
			
			Process ExternalProcess = new Process();
			ExternalProcess.StartInfo.FileName = "Notepad.exe";
			ExternalProcess.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
			ExternalProcess.Start();
			ExternalProcess.WaitForExit();
			return;
		}
		
		//this.SetVisible(this.Index == menuPanel.Selected);
		//this.SetFocusMode(this.Index == menuPanel.Selected ? FocusModeEnum.All : FocusModeEnum.None);
		//if (this.Index == menuPanel.Selected) this.GrabFocus();
	}
}
