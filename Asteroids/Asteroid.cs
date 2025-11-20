#nullable enable
using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Godot;
using Godot.Collections;
using Array = System.Array;

namespace Asteroids;

public partial class Asteroid : RigidBody2D {
	private bool Active { get; set; } = true;
	[Export] public float RotationSpeed = 1;
	private Vector2 _bodyRect = Vector2.Zero;
	public static uint ActiveAsteroids = 0;

	[Export] private SizeType _sizeType;

	[Export] private float _lineWeight = 2;
	[Export] private uint _scorePerKill;
	[Export] private byte _maxHealth = 1;
	private byte _health;

	private Line2D _selfLineShape;
	private CollisionPolygon2D _selfCollisionBody;
	private Node2D[] _cracks;

	[Signal]
	public delegate void ExplodedEventHandler(PackedScene newAsteroidScene, int count);

	public enum SizeType {
		Small = 0,
		Medium = 1,
		Large = 2
	}

	public Asteroid() {
		this.BodyEntered += this.ActionOnCollision;
	}

	public override void _Ready() {
		base._Ready();
		AddToGroup("asteroids");
		ActiveAsteroids++;

		_selfLineShape = GetNode<Line2D>("Line2D");
		_selfCollisionBody = GetNode<CollisionPolygon2D>("CollisionPolygon2D");

		_selfLineShape.DefaultColor = Global.Instance.LineColor;
		_selfLineShape.Width = _lineWeight;

		foreach (Vector2 point in _selfLineShape.Points) {
			_bodyRect.X = Math.Max(_bodyRect.X, point.X);
			_bodyRect.Y = Math.Max(_bodyRect.Y, point.Y);
		}

		_cracks = new Node2D[_selfLineShape.GetChildCount()]; 
		for (var i = 0; i < _cracks.Length; i++) {
			_cracks[i] = (Node2D) _selfLineShape.GetChild(i);
			for (var j = 0; j < _cracks[i].GetChildCount(); j++) {
				var lineshape = (Line2D) _cracks[i].GetChild(j);
				lineshape.DefaultColor = Global.Instance.LineColor;
				lineshape.Width = _lineWeight;
			}
		}
		
		_health = _maxHealth;

        var global = Global.Instance;
        LinearVelocity = new Vector2(global.rng.RandfRange(-1, 1), global.rng.RandfRange(-1, 1)).Normalized() 
                          * global.AsteroidGlobalLinearSpeedMin 
                          * _sizeType switch
                          {
                              SizeType.Large => global.AsteroidLargeSpeedMultiplier,
                              SizeType.Medium => global.AsteroidMediumSpeedMultiplier,
                              SizeType.Small => global.AsteroidSmallSpeedMultiplier,
                              _ => 1f
                          };

        Scale = Vector2.One * _sizeType switch
        {
            SizeType.Large => global.AsteroidLargeScaleMultiplier,
            SizeType.Medium => global.AsteroidMediumScaleMultiplier,
            SizeType.Small => global.AsteroidSmallScaleMultiplier,
            _ => 1f
        };
        
        Mass = _sizeType switch
        {
            SizeType.Large => 100_000_000,
            SizeType.Medium => 500_000,
            SizeType.Small => 5_000,
            _ => 5000
        };
	}

	private void OnScreenExit() {
		this.Active = false;
		this.Hide();
	}

	public override void _PhysicsProcess(double delta) {
		base._PhysicsProcess(delta);
		Main.ScreenWrap(this);
	}

	//public override void _PhysicsProcess(double delta) {
	//	base._PhysicsProcess(delta);
	//	var global = Global.Instance;
	//	//var spawnVector = new SpawnVector(
	//	//	this.Position.AngleToPoint(global.ScreenRect / 2),
	//	//	15.0f
	//	//);
	//	//spawnVector.angle += 0.2f;
	//	//if (spawnVector.angle >= 2) spawnVector.angle = 0;
	//	//Console.WriteLine($"Angle to center: {spawnVector.angle}");
	//	
	//	//this.Position.AngleToPoint(global.ScreenRect / 2), 15F
	//	Console.Out.WriteLine($"Position {this.Position.AngleToPoint(global.ScreenRect / 2)}");
	//	// creates rotation, cool.
	//	//this.Position = new SpawnVector(
	//	//	(float) (this.Position.AngleToPoint(global.ScreenRect / 2) + delta * 1),
	//	//	global.ScreenRect.Length() / 2 + this._bodyRect.Length() / 2
	//	//).MapToScreenCoordinates();
	//	//this.Position = this.Position.Normalized() * (global.ScreenRect.Length() + this._bodyRect.Length());
	//	Console.Out.WriteLine($"Position {this.Position}");
	//	//this.Position = spawnVector.MapToScreenCoordinates();
	//}

	private void ActionOnCollision(Node target) {
		if (target is RigidBody2D body) {
			int multiplier = body is Asteroid asteroid? (int) asteroid._sizeType : 0;
			this.ApplyCentralImpulse((this.Position - body.Position).Normalized() * (75_000F * (multiplier + 1)));
		}
		if (target is not PlayerProjectile) return;
		this._health--;
		if (this._health == 0) {
			Main.Score += this._scorePerKill;
			this.DestructAndSpawnChildren();
			return;
		}

		this._cracks[this._maxHealth - this._health - 1].SetVisible(true);
	}

	private void DestructAndSpawnChildren()
	{
		PackedScene newAsteroidScene = _sizeType switch
		{
			SizeType.Large => Global.Instance.AsteroidMediumScene,
			SizeType.Medium => Global.Instance.AsteroidSmallScene,
			_ => null
		};

		if (newAsteroidScene != null)
		{
			EmitSignal(SignalName.Exploded, newAsteroidScene, 2);
		}

		Destroy();
	}
	
	public override void _ExitTree() => this.Destroy();
	public void Destroy() {
		ActiveAsteroids--;
		QueueFree();
	}
}
