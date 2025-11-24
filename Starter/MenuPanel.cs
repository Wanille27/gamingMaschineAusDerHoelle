using System;
using Godot;

namespace Starter;

public partial class MenuPanel : PanelContainer
{
	public static Color DefaultTextColor { get; } = Color.FromString("ffd000", Colors.Yellow);
	public static Color SelectedTextColor { get; } = Color.FromString("ff0000", Colors.Red);
	
	public int Selected { get; private set; } = 0;

	private HBoxContainer _buttonContainer;
	private GameControl[] _controls;
	private Label _missionLabel;
	private Label _explanationLabel;
	
	private Node _asteroidsScene;
	
	public MenuPanel() {
		Console.WriteLine("Created");
	}
	
	public override void _Ready() {
		RenderingServer.SetDefaultClearColor(Colors.Black);
		this._buttonContainer = (HBoxContainer) this.FindChild("ButtonContainer", recursive: true);
		this._controls = [
			(GameControl) this.FindChild("AsteroidsGameControl"),
			(GameControl) this.FindChild("PongGameControl"),
			(GameControl) this.FindChild("BitKnightGameControl"),
		];
		
		this._missionLabel = (Label) this.FindChild("MissionLabel");
		this._explanationLabel = (Label) this.FindChild("ExplanationLabel");
	}

	public override void _Input(InputEvent @event) {
		this._controls[this.Selected].DeSelect();

		if (@event is InputEventJoypadMotion or InputEventKey) {
			if (Input.IsActionJustPressed("Joystick0Left") ||  Input.IsActionJustPressed("LeftKeyboard")) {
				Console.Out.WriteLine("\t:: Left");
				//if (this.Selected >= this._controls.Length - 1) this.Selected = 0;
				//else this.Selected++;
				ActionOnLeft();
			} else if (Input.IsActionJustPressed("Joystick0Right") ||  Input.IsActionJustPressed("RightKeyboard")) {
				Console.Out.WriteLine("\t:: Right");
				//if (this.Selected <= 0) this.Selected = (byte) (this._controls.Length - 1);
				//else this.Selected--;
				ActionOnRight();
			}
		}

		Console.Out.WriteLine($"Select: {this.Selected}");
		this.UpdateSelection();
		return;

		void ActionOnRight() {
			this.Selected =
				this.Selected <= 0
					? this._controls.Length - 1
					: this.Selected - 1
			;
		}

		// check whether it is arrow down or up
		void ActionOnLeft() {
			this.Selected =
				this.Selected >= this._controls.Length - 1
					? this.Selected = 0
					: this.Selected + 1
			;
		}
	}

	private void UpdateSelection() {
		var selectedControl = this._controls[this.Selected];
		selectedControl.Select();
		this._missionLabel.SetText(GameDescription.GameDescriptions[selectedControl.GameType].Mission);
		this._explanationLabel.SetText(GameDescription.GameDescriptions[selectedControl.GameType].Explanation);
	}
}
