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

	public enum SizeType {
		Small = 0,
		Large = 1,
		Medium = 2,
	}

	public Asteroid() {
		this.BodyEntered += this.ActionOnCollision;
	}

	public override void _Ready() {
		base._Ready();

		this._selfLineShape = (Line2D) this.GetChild(0);
		this._selfCollisionBody = (CollisionPolygon2D) this.GetChild(1);
		if (this._selfLineShape == null) throw new ("Can't find LineShape");
		if (this._selfCollisionBody == null) throw new ("Can't find CollisionBody");
		this._selfLineShape.SetDefaultColor(Global.Instance.LineColor);
		this._selfLineShape.SetWidth(this._lineWeight);

		foreach (Vector2 point in this._selfLineShape.Points) {
			this._bodyRect.X = Math.Max(this._bodyRect.X, point.X);
			this._bodyRect.Y = Math.Max(this._bodyRect.Y, point.Y);
		}

		this._cracks = new Node2D[this._selfLineShape.GetChildCount()]; 
		for (var i = 0; i < this._cracks.Length; i++) {
			this._cracks[i] = (Node2D) this._selfLineShape.GetChild(i);
			for (var j = 0; j < this._cracks[i].GetChildCount(); j++) {
				var lineshape = (Line2D) this._cracks[i].GetChild(j);
				lineshape.SetDefaultColor(Global.Instance.LineColor);
				lineshape.SetWidth(this._lineWeight);
			}
		}
		
		this._health = this._maxHealth;
	}

	private void OnScreenExit() {
		this.Active = false;
		this.Hide();
	}

	public override void _PhysicsProcess(double delta) {
		base._PhysicsProcess(delta);
		Starter.gamingMaschineAusDerHoelle.Asteroids.MainAsteroids.ScreenWrap(this);
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

	private new void SetScale(Vector2 scale) {
		this._selfCollisionBody.Scale = (scale);
		this._selfLineShape.Scale = (scale);
	}
	
	[Pure]
	public Asteroid SpawnRandom() {
		var global = Global.Instance;
		var asteroid = (Asteroid) this.Duplicate();
		
		asteroid._Ready();
		ActiveAsteroids++;
		
		
		// get random Position within Global.ScreenRect
		//asteroid.Position = new SpawnVector(
		//	angle: global.rng.RandfRange(0, 1),
		//	distance: global.rng.RandfRange(0, 1)
		//).MapToScreenCoordinates();
		asteroid.Position = new SpawnVector(
			global.rng.RandfRange(0, float.Tau),
			global.ScreenRect.Length() / 2 + asteroid._bodyRect.Length()
		).MapToScreenCoordinates();

		Vector2[] screenEdges = [new (0, global.ScreenRect.Y), new (global.ScreenRect.X, 0), new (global.ScreenRect.X, global.ScreenRect.Y), new (0, 0)];
		var farthest1 = 0;
		for (var i = 1; i < screenEdges.Length; i++) {
			if (asteroid.Position.DistanceTo(screenEdges[farthest1]) <= asteroid.Position.DistanceTo(screenEdges[i])) {
				farthest1 = i;
			}
		}
		Vector2 randomEdgePoint = screenEdges[farthest1] * global.rng.RandfRange(0, 1);
		asteroid.LinearVelocity = (randomEdgePoint - asteroid.Position).Normalized()
								  * global.AsteroidGlobalLinearSpeedMin
								  * asteroid._sizeType switch {
									  SizeType.Large => global.AsteroidLargeSpeedMultiplier,
									  SizeType.Medium => global.AsteroidMediumSpeedMultiplier,
									  SizeType.Small => global.AsteroidSmallSpeedMultiplier,
								  };
		
		asteroid.SetScale(Vector2.One * asteroid._sizeType switch {
			SizeType.Large => global.AsteroidLargeScaleMultiplier,
			SizeType.Medium => global.AsteroidMediumScaleMultiplier,
			SizeType.Small => global.AsteroidSmallScaleMultiplier,
		});
		
		asteroid.Mass = asteroid._sizeType switch {
			SizeType.Large  => 100_000_000,
			SizeType.Medium => 500_000,
			SizeType.Small  => 5_000,
		};


		Console.WriteLine($"Screen center: {global.ScreenRect / 2}");
		Console.WriteLine($"Asteroid pos: {asteroid.Position}");
		Console.WriteLine($"Asteroid vel: {asteroid.LinearVelocity}");
		
		//asteroid.RotationSpeed = global.rng.RandfRange(-Global.AsteroidAngularSpeed, Global.AsteroidAngularSpeed);
		//asteroid.LinearVelocity = new (
		//	x: global.rng.RandfRange(-Global.AsteroidLinearSpeed, Global.AsteroidLinearSpeed),
		//	y: global.rng.RandfRange(-Global.AsteroidLinearSpeed, Global.AsteroidLinearSpeed)
		//);
		//asteroid.AngularVelocity =
		//	global.rng.RandfRange(-Global.AsteroidAngularSpeed, Global.AsteroidAngularSpeed);

		return asteroid;
	}
	
	public struct SpawnVector {
		public float AngleRadians { get; set; } = 0;
		public float Lenght { get; set; } = 0; // 0 - 1
		
		[Pure]
		public Vector2 MapToScreenCoordinates() {
			//var angleV = (float) (this.angle * Math.Tau);
			return new (
				(Global.Instance.ScreenRect.X / 2) + (float) Math.Cos(this.AngleRadians) * this.Lenght,
				(Global.Instance.ScreenRect.Y / 2) + (float) Math.Sin(this.AngleRadians) * this.Lenght
			);
		}
		
		public SpawnVector() {}

		public SpawnVector(float angleRadians, float distance) {
			this.AngleRadians = angleRadians;
			this.Lenght = distance;
		}
		
		public SpawnVector(Vector2 origin, Vector2 direction) {
			this.AngleRadians = direction.AngleToPoint(origin);
			this.Lenght = direction.Length();
		}
	}

	private void ActionOnCollision(Node target) {
		if (target is RigidBody2D body) {
			int multiplier = body is Asteroid asteroid? (int) asteroid._sizeType : 0;
			this.ApplyCentralImpulse((this.Position - body.Position).Normalized() * (75_000F * (multiplier + 1)));
		}
		if (target is not PlayerProjectile) return;
		this._health--;
		if (this._health == 0) {
			Starter.gamingMaschineAusDerHoelle.Asteroids.MainAsteroids.Score += this._scorePerKill;
			// TODO
			this.DestructAndSpawnChildren();
			return;
		}

		Console.Out.WriteLine($":: max: {this._maxHealth} ; cur: {this._health} ::");
		this._cracks[this._maxHealth - this._health - 1].SetVisible(true);
	}

	private void DestructAndSpawnChildren()
	{
		var main = this.GetParent<Starter.gamingMaschineAusDerHoelle.Asteroids.MainAsteroids>();
		if (main is null)
		{
			this.Destroy();
			return;
		}

		Asteroid[] newAsteroids;
		switch (this._sizeType)
		{
			case SizeType.Large:
				newAsteroids = [
					(Asteroid) main.AsteroidMediumTemplate.Duplicate()
				];
				newAsteroids[0].SetScale(new(0.1f, 0.1f));
				break;
			case SizeType.Medium:
				newAsteroids = [
					(Asteroid) main.AsteroidSmallTemplate.Duplicate()
				];
				break;
			case SizeType.Small:
			default:
				this.Destroy();
				main.RemoveChild(this);
				//this.Destroy();
				return;
		}

		foreach (Asteroid asteroid in newAsteroids) {
			main.AddChild(this.CreateSplitAsteroid(asteroid));
		}

		this.Destroy();
		main.RemoveChild(this);
	}
	
	private Asteroid CreateSplitAsteroid(in Asteroid asteroid) {
		asteroid.Position = this.Position;
		
		Vector2 newDirection = this.LinearVelocity.Rotated((float)GD.RandRange(-Math.PI / 4, Math.PI / 4));
		asteroid.LinearVelocity = newDirection;

		return asteroid;
	}
	
	//~Asteroid() => this.Destroy();
	public override void _ExitTree() => this.Destroy();
	
	public void Destroy() {
		Console.Out.WriteLine($"Delete {this.GetClass().GetBaseName()}#{this.GetInstanceId()}");
		//foreach (Node child in this.GetChildren()) child.QueueFree();
		this.QueueFree();
	}
}
