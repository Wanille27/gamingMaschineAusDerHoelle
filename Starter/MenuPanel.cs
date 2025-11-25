using System;
using System.IO;
using System.Net;
using Asteroids;
using Godot;
using MainAsteroids = Starter.gamingMaschineAusDerHoelle.Asteroids.MainAsteroids;

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

	private Node2D _gamePlaceholder;
	
	[Export]
	private PackedScene AsteroidsScene { get; set; }
	
	[Signal]
	private delegate void CloseGameEventHandler();
	
	public MenuPanel() {
		Console.WriteLine("Created");
	}
	
	public override void _Ready() {
		RenderingServer.SetDefaultClearColor(Colors.Black);
		var root = this.FindParent("RootControl");
		this._gamePlaceholder = (Node2D) root.FindChild("GamePlaceholder");
		if (this._gamePlaceholder == null) throw new ("Can't find GamePlaceholder");
		
		this._buttonContainer = (HBoxContainer) this.FindChild("ButtonContainer", recursive: true);
		this._controls = [
			(GameControl) this.FindChild("AsteroidsGameControl"),
			(GameControl) this.FindChild("PongGameControl"),
			(GameControl) this.FindChild("BitKnightGameControl"),
		];
		
		this._missionLabel = (Label) this.FindChild("MissionLabel");
		this._explanationLabel = (Label) this.FindChild("ExplanationLabel");
		
		//this.AsteroidsScene = GD.Load<PackedScene>("res://gamingMaschineAusDerHoelle/Asteroids/Main.tscn");
		//var AsteroidsMainI = this.AsteroidsScene.Instantiate<Asteroids.Main>();
		//AsteroidsMainI.Ready += () => { //this.AddChild(AsteroidsMainI); };
		
		this.CloseGame += () => {
			this.UpdateSelection();
			this.Show();
			var curNode = this._gamePlaceholder;
			this._gamePlaceholder.Hide();
			Console.Out.WriteLine("....");
			//this._gamePlaceholder.CallDeferred(nameof(this._gamePlaceholder.ReplaceBy), new Node2D());
			//this._gamePlaceholder.ReplaceBy(new Node2D());
			this.RemoveChild(this._gamePlaceholder);
			Console.Out.WriteLine("....");
			this._gamePlaceholder.Free();
		};
		this.UpdateSelection();
	}

	public override void _Input(InputEvent @event) {
		this._controls[this.Selected].DeSelect();

		if (@event is InputEventJoypadMotion or InputEventKey) {
			if (Input.IsActionJustPressed("Joystick0Left") ||  Input.IsActionJustPressed("LeftKeyboard")) {
				Console.Out.WriteLine("\t:: Left");
				//File.Open("res://asteroids/", FileMode.Open);
				//if (this.Selected >= this._controls.Length - 1) this.Selected = 0;
				//else this.Selected++;
				ActionOnLeft();
			} else if (Input.IsActionJustPressed("Joystick0Right") ||  Input.IsActionJustPressed("RightKeyboard")) {
				Console.Out.WriteLine("\t:: Right");
				//if (this.Selected <= 0) this.Selected = (byte) (this._controls.Length - 1);
				//else this.Selected--;
				ActionOnRight();
			} else if (@event is InputEventKey { Keycode: Key.Enter }) {
				this.StartGame();
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

	private void StartGame() {
		switch (this._controls[this.Selected].GameType) {
			case GameControl.GameTypeE.Asteroids:
				this.GetTree().ChangeSceneToPacked(this.AsteroidsScene);
				//Console.Out.WriteLine($"{this._gamePlaceholder}, {this.AsteroidsScene}");
			//	var game = (MainAsteroids) this.AsteroidsScene.Instantiate();
			//	game.MenuPanelI = this;
			//	this._gamePlaceholder.ReplaceBy(game); 
				break;
			default: break;
		}
		this.Hide();
		this._gamePlaceholder._Ready();
		this._gamePlaceholder.Show();
	}

	private void UpdateSelection() {
		var selectedControl = this._controls[this.Selected];
		selectedControl.Select();
		this._missionLabel.SetText(GameDescription.GameDescriptions[selectedControl.GameType].Mission);
		this._explanationLabel.SetText(GameDescription.GameDescriptions[selectedControl.GameType].Explanation);
	}

	protected override void Dispose(bool disposing) {
		base.Dispose(disposing);
		this._gamePlaceholder.QueueFree();
	}
}
