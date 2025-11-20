using Godot;
using System;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace Asteroids;

public partial class Main : Node2D {
	private bool _running = true;
	private Thread _runner;

	private ulong _startTimeStampMilliSeconds;
	
	private Label _fpsLabel;

	public static ulong Score { get; set; }

	private MeshInstance2D _screen;
	private PlayerShip _playerShip;
	private byte _maxNumAsteroids = 5;

	public Asteroid AsteroidMediumTemplate { get; private set; }
	public Asteroid AsteroidLargeTemplate { get; private set; }
	public Asteroid AsteroidSmallTemplate { get; private set; }
	
	[Signal]
	public delegate void GameOverEventHandler();
	
	
	public override void _UnhandledInput(InputEvent @event) {
		if (@event is not InputEventKey eventKey) return;
		Console.Out.WriteLine($"Event: {OS.GetKeycodeString(eventKey.Keycode)}");
	}
	
	public override void _Ready() {
		base._Ready();

		Console.Out.WriteLine($"Joypads : {Input.GetConnectedJoypads()}");
		//DisplayServer.WindowSetSize(new (1920, 1080));
		RenderingServer.SetDefaultClearColor(Colors.Black);

		this._screen = (MeshInstance2D) this.GetChild(2);
		this._screen._Ready();
		//var playerShipScene = GD.Load<PackedScene>("res://PlayerShipSleek1.tscn");
		//var playerShipSceneI = playerShipScene.Instantiate<Node2D>();
		this._playerShip = (PlayerShip) this.GetChild(0).GetChild(0);
		//playerShipSceneI.QueueFree();
		//this.AddChild(this._PlayerShip);
		
		var asteroidLargeScene = GD.Load<PackedScene>("res://Asteroid_Large.tscn");
		var asteroidLargeSceneI = asteroidLargeScene.Instantiate<Node2D>();
		this.AsteroidLargeTemplate = (Asteroid) asteroidLargeSceneI.GetChild(0).Duplicate();
		asteroidLargeSceneI.QueueFree();
		
		var asteroidMediumScene = GD.Load<PackedScene>("res://Asteroid_Medium.tscn");
		var asteroidMediumSceneI = asteroidMediumScene.Instantiate<Node2D>();
		this.AsteroidMediumTemplate = (Asteroid) asteroidMediumSceneI.GetChild(0).Duplicate();
		asteroidMediumSceneI.QueueFree();
		
		var asteroidSmallScene = GD.Load<PackedScene>("res://Asteroid_Small.tscn");
		var asteroidSmallSceneI = asteroidSmallScene.Instantiate<Node2D>();
		this.AsteroidSmallTemplate = (Asteroid) asteroidSmallSceneI.GetChild(0).Duplicate();
		asteroidSmallSceneI.QueueFree();
		
		foreach (Node child in this.GetChildren()) child._Ready();

		var container = (VBoxContainer) this.GetChild(1);
		container.SetSize(DisplayServer.WindowGetSize());
		this._fpsLabel = (Label) container.GetChild(0).GetChild(0);
		this.GameOver += () => {
			((ScoreLabel) container.GetChild(1).GetChild(0)).SetToEndState("You Lost with score of " + Score);
			((CenterContainer) container.GetChild(1)).SetVSizeFlags(Control.SizeFlags.ExpandFill);
			this._running = false;
		};
		
		this.Setup();
		// Start the Async Runner thread.
		(this._runner = new(this.Run)).Start();
	}

	~Main() {
		// End Runner thread on Game end
		// TODO
		//new CancellationToken()
		this._runner.Abort();
		foreach (Node child in this.GetChildren()) child.QueueFree();
	}
	

	public override void _Notification(int what) {
		//Console.Out.WriteLine($"what: {what}");
		switch ((long) what) {
			case NotificationWMCloseRequest: this._running = false; goto default;
			default: base._Notification(what); break;
		}
	}

	private async void Run() {
		try {
			var spawnRandomAsteroids = new Task(async void () => {
				try {
					var lastSpawnTime = (long) Time.GetTicksMsec();
					while (this._running) {
						var spawnRoll = Global.Instance.rng.Randf();
						var time = (long) Time.GetTicksMsec();
						// g(x)=log(2,((ta+tb)/(10000))-((amax)/(acur)))
						double likelyHood = Math.Log2(
							((double)(time - 10_000 + lastSpawnTime) / lastSpawnTime) * (5 - 0) / 5
						);
						
						//									   ((Asteroid.ActiveAsteroids) / (double)this._maxNumAsteroids));
						//Console.Out.WriteLine(
						//	$"g(x)=log2({(double)(Time.GetTicksMsec() + lastSpawnTime) / 10_000} - {(double)Asteroid.ActiveAsteroids / this._maxNumAsteroids})");
						if (spawnRoll <= likelyHood) {
							//await Console.Out.WriteLineAsync($"At {time - lastSpawnTime} Likely hood : {likelyHood} roll {spawnRoll}");
							this.SpawnNewAsteroidAsync();
							lastSpawnTime = (long) Time.GetTicksMsec();
							await Task.Delay(3_000);
						}
					}
				} catch (Exception e) {
					Console.Error.WriteLine(e);
				}
			});
			spawnRandomAsteroids.Start();
			while (this._running) {
				await Task.Delay(100);
			}
		} catch (Exception e) {
			Console.Error.WriteLine(e);
		}
	}

	private void Setup() {
		var global = Global.Instance;
		global.ScreenRect = this.GetViewportRect().Size;

		for (var i = 0; i < 5; i++) this.SpawnNewAsteroid();

		Console.WriteLine($"Initializing PLayer {this._playerShip}");
		this._playerShip.SetPosition(global.ScreenRect / 2);
		
	}

	private void SpawnNewAsteroid() {
		var size = (Asteroid.SizeType) Global.Instance.rng.RandiRange((int) Asteroid.SizeType.Large, (int) Asteroid.SizeType.Small);
		Asteroid asteroid = size switch {
			Asteroid.SizeType.Large => this.AsteroidLargeTemplate.SpawnRandom(),
			Asteroid.SizeType.Medium => this.AsteroidMediumTemplate.SpawnRandom(),
			Asteroid.SizeType.Small => this.AsteroidSmallTemplate.SpawnRandom(),
			_ => throw new ArgumentOutOfRangeException()
		};

		Console.WriteLine($"new Asteroid: {asteroid.Position}");
		this.AddChild(asteroid);
	}
	
	private async void SpawnNewAsteroidAsync() {
		try {
			var size = (Asteroid.SizeType) Global.Instance.rng.RandiRange((int) Asteroid.SizeType.Large, (int) Asteroid.SizeType.Small);
			Asteroid asteroid = size switch {
				Asteroid.SizeType.Large => this.AsteroidLargeTemplate.SpawnRandom(),
				Asteroid.SizeType.Medium => this.AsteroidMediumTemplate.SpawnRandom(),
				Asteroid.SizeType.Small => this.AsteroidSmallTemplate.SpawnRandom(),
				_ => throw new ArgumentOutOfRangeException()
			};

			await Console.Out.WriteLineAsync($"new Asteroid: {asteroid.Position}");
			this.CallDeferred(Node.MethodName.AddChild, asteroid);
		} catch (Exception) {
			// ignored
		}
	}

	protected override void Dispose(bool disposing) {
		base.Dispose(disposing);
		if (!disposing) return;
		//this._PlayerShip?.QueueFree();
		this.AsteroidLargeTemplate?.Destroy();
		this.AsteroidMediumTemplate?.Destroy();
		this.AsteroidSmallTemplate?.Destroy();
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
