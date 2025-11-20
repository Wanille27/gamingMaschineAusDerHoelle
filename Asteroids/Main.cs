using Godot;
using System;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace Asteroids;

public partial class Main : Node2D {
	private Label _fpsLabel;

	public static ulong Score { get; set; }

	private MeshInstance2D _screen;
	private PlayerShip _PlayerShip;
	private byte _maxNumAsteroids = 5;
	
	[Signal]
	public delegate void GameOverEventHandler();
	
	public override void _Ready() {
		base._Ready();
		//DisplayServer.WindowSetSize(new (1920, 1080));
		RenderingServer.SetDefaultClearColor(Colors.Black);

		this._screen = GetNode<MeshInstance2D>("Screen");
		this._screen._Ready();
		this._PlayerShip = GetNode<PlayerShip>("PlayerShip/Ship");
		this._PlayerShip.ShipDestroyed += _on_PlayerShip_ShipDestroyed;
		
		Global.Instance.AsteroidLargeScene = GD.Load<PackedScene>("res://Asteroid_Large.tscn");
		Global.Instance.AsteroidMediumScene = GD.Load<PackedScene>("res://Asteroid_Medium.tscn");
		Global.Instance.AsteroidSmallScene = GD.Load<PackedScene>("res://Asteroid_Small.tscn");
		
		foreach (Node child in this.GetChildren()) child._Ready();

		var container = GetNode<VBoxContainer>("VBoxContainer");
		container.SetSize(DisplayServer.WindowGetSize());
		this._fpsLabel = container.GetNode<Label>("FPS Label");
		this.GameOver += () => {
			container.GetNode<CenterContainer>("CenterContainer").GetNode<Label>("Score Label").SetFinalScore("You Lost with score of " + Score);
			container.GetNode<CenterContainer>("CenterContainer").SetVSizeFlags(Control.SizeFlags.ExpandFill);
		};
		        
		        var global = Global.Instance;
		        global.ScreenRect = this.GetViewportRect().Size;
		
		        for (var i = 0; i < 5; i++) {
		            this.SpawnNewAsteroid();
		        }
		
		        Console.WriteLine($"Initializing PLayer {this._PlayerShip}");
		        this._PlayerShip.SetPosition(global.ScreenRect / 2);
		
		        var asteroidSpawnTimer = new Timer();
		        AddChild(asteroidSpawnTimer);
		        asteroidSpawnTimer.WaitTime = 1.0f;
		        asteroidSpawnTimer.Timeout += () => {
		            if (GetTree().GetNodesInGroup("asteroids").Count < _maxNumAsteroids)
		            {
		                SpawnNewAsteroid();
		            }
		        };
		        asteroidSpawnTimer.Start();
		    }
	private void SpawnNewAsteroid() {
		var size = (Asteroid.SizeType) Global.Instance.rng.RandiRange((int) Asteroid.SizeType.Large, (int) Asteroid.SizeType.Small);
		var asteroidScene = size switch {
			Asteroid.SizeType.Large => Global.Instance.AsteroidLargeScene,
			Asteroid.SizeType.Medium => Global.Instance.AsteroidMediumScene,
			Asteroid.SizeType.Small => Global.Instance.AsteroidSmallScene,
			_ => throw new ArgumentOutOfRangeException()
		};

        var asteroid = asteroidScene.Instantiate<Asteroid>();
        var spawnPosition = new Vector2(Global.Instance.rng.RandiRange(0, (int)Global.Instance.ScreenRect.X), Global.Instance.rng.RandiRange(0, (int)Global.Instance.ScreenRect.Y));
        asteroid.Position = spawnPosition;

		asteroid.Exploded += _on_Asteroid_Exploded;
						this.AddChild(asteroid);
					}
				
					private void _on_PlayerShip_ShipDestroyed()
					{
						EmitSignal(SignalName.GameOver);
					}
				
					private void _on_Asteroid_Exploded(PackedScene newAsteroidScene, int count)
					{
						for (int i = 0; i < count; i++)
						{					var asteroid = newAsteroidScene.Instantiate<Asteroid>();
					var spawnPosition = new Vector2(Global.Instance.rng.RandiRange(0, (int)Global.Instance.ScreenRect.X), Global.Instance.rng.RandiRange(0, (int)Global.Instance.ScreenRect.Y));
					asteroid.Position = spawnPosition;
		
					asteroid.Exploded += _on_Asteroid_Exploded;
					this.AddChild(asteroid);
				}
			}
			
			protected override void Dispose(bool disposing) {		base.Dispose(disposing);
		if (!disposing) return;
		//this._PlayerShip?.QueueFree();
		this._AsteroidLargeTemplate?.QueueFree();
		this._AsteroidMediumTemplate?.QueueFree();
		this._AsteroidSmallTemplate?.QueueFree();
	}

	public static void ScreenWrap(Node2D node) {
		Vector2I screenSize = DisplayServer.ScreenGetSize();
		Vector2 position = node.GlobalPosition;
		if (position.X > screenSize.X) {
			node.SetGlobalPosition(position with { X = 0 });
		} else if (position.X < 0) {
			node.SetGlobalPosition(position with { X = screenSize.X });
		}
		if (position.Y > screenSize.Y) {
			node.SetGlobalPosition(position with { Y = 0 });
		} else if (position.Y < 0) {
			node.SetGlobalPosition(position with { Y = screenSize.Y });
		}
	}
}
