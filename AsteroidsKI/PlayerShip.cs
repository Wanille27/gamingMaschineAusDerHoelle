using System;
using System.Threading.Tasks;
using Godot;

namespace Asteroids;

public partial class PlayerShip : RigidBody2D
{
	
	private SteerDirection _steerDirection;
	
	private Node2D _selfFront;
	private Vector2 _selfFrontBasePosition;
	private Node2D _selfCenter;
	public Line2D SelfLineShape;
	private Line2D _selfFireLineShape;
	private Line2D[] _selfBackwardFireLineShapes = new Line2D[2];
	private Line2D[] _selfSidewardsFireLineShapes = new Line2D[2];
	private CollisionPolygon2D _selfCollisionBody;
	private PackedScene _projectileScene;

	private float _currentBurstDelay = 0;
	private bool _shouldShoot;
	private bool _alive = true;
	private byte _health;

	[Export] public byte MaxHealth = 3;
	[Export] public float SpeedMultiplier;
	[Export] public float FireBurstDelay;
	[Export] public float FireBurstCount;
	[Export] private Color _shipColor = new(0.349F, 0.513F, 0.94F);
	[Export] private Color _shipEngineFireColor = new(0.349F, 0.513F, 0.94F, 0.75F);

	[Signal]
	public delegate void ShipDestroyedEventHandler();

	private struct SteerDirection(
		SteerDirection.LinearDirection linearDirection,
		SteerDirection.AngularDirection angularDirection
	) {
		internal enum LinearDirection { NONE, FORWARDS, BACKWARDS }

		internal enum AngularDirection { CLOCKWISE, ANTI_CLOCKWISE, NONE }

		public LinearDirection linearDirection = linearDirection;
		public AngularDirection angularDirection = angularDirection;
	}

	public PlayerShip() {
		this._health = this.MaxHealth;
	}
	
	public override void _Ready() {
		base._Ready();
		_projectileScene = GD.Load<PackedScene>("res://Player_Projectile_1.tscn");
		Console.Out.WriteLine("Initializing PlayerShip");
		var global = Global.Instance;
		
		this._selfCollisionBody = GetNode<CollisionPolygon2D>("CollisionPolygon2D");
		this.SelfLineShape = GetNode<Line2D>("LineShape");
		this._selfFront = GetNode<Marker2D>("Front");
		this._selfFrontBasePosition = this._selfFront.Position;
		this._selfCenter = GetNode<Marker2D>("Center");
		this._selfFireLineShape = GetNode<Line2D>("LineFireShape");
		this._selfBackwardFireLineShapes[0] = GetNode<Line2D>("LineBackwardsFireShapeLeft");
		this._selfBackwardFireLineShapes[1] = GetNode<Line2D>("LineBackwardsFireShapeRight");
		this._selfSidewardsFireLineShapes[0] = GetNode<Line2D>("LineSideWardsRight");
		this._selfSidewardsFireLineShapes[1] = GetNode<Line2D>("LineSideWardsLeft");

		foreach (Line2D backwardFireLine in this._selfBackwardFireLineShapes) {
			backwardFireLine.DefaultColor = this._shipEngineFireColor;
			backwardFireLine.Width = global.LineWeight;
			backwardFireLine.Hide();
		}

		foreach (Line2D sidewardsFireLine in this._selfSidewardsFireLineShapes) {
			sidewardsFireLine.DefaultColor = this._shipEngineFireColor;
			sidewardsFireLine.Width = global.LineWeight;
			sidewardsFireLine.Hide();
		}
		
		this._selfFireLineShape.DefaultColor = this._shipEngineFireColor;
		this._selfFireLineShape.Width = global.LineWeight;
		this._selfFireLineShape.Hide();
		
		this.SelfLineShape.DefaultColor = this._shipColor;
		this.SelfLineShape.Width = global.LineWeight;
		
		//this.SetScale(Vector2.One * global.PlayerShipScale * new Vector2(0.01f, 0.01f));
		this.BodyEntered += this.OnAreaEntered;
		//this.Connect(RigidBody2D.SignalName.BodyEntered, new(this, MethodName.OnAreaEntered));
		this._steerDirection.angularDirection = SteerDirection.AngularDirection.NONE;
		this._steerDirection.linearDirection = SteerDirection.LinearDirection.NONE;
	}
	
	private new void SetScale(Vector2 scale) {
		//base.SetScale(scale);
		//this.SelfCollisionBody.SetScale(scale);
		for (var i = 0; i < this._selfCollisionBody.Polygon.Length; i++ ) {
			this._selfCollisionBody.Polygon[i] *= 0;
		}
		
		for (var i = 0; i < this.SelfLineShape.Points.Length; i++ ) {
			this.SelfLineShape.Points[i] *= 0;
		}
		//this.SelfLineShape.SetScale(scale);
	}

	public override void _PhysicsProcess(double delta) {
		base._PhysicsProcess(delta);
		this._selfFront.SetPosition(this._selfFrontBasePosition.Rotated(this.Rotation));
		
		//this.SelfCenter.SetPosition(this.SelfLineShape.Points[0]);
		switch (this._steerDirection.linearDirection) {
			case SteerDirection.LinearDirection.FORWARDS:
				this.ApplyForce(
					(this._selfFront.Position - this._selfCenter.Position).Normalized() * (float) (
						8_000 * Global.Instance.PlayerShipSpeedMultiplier * delta
					)
				);
				break;
			case SteerDirection.LinearDirection.BACKWARDS:
				this.ApplyForce(
					(this._selfCenter.Position - this._selfFront.Position).Normalized() * (float) (
						2_000 * Global.Instance.PlayerShipSpeedMultiplier * delta
					)
				);
				break;
		}

		switch (this._steerDirection.angularDirection) {
			case SteerDirection.AngularDirection.CLOCKWISE: {
				this.ApplyTorque((float)(double.Pi * 12_000F * delta));
				break;
			}
			case SteerDirection.AngularDirection.ANTI_CLOCKWISE: {
				this.ApplyTorque((float)(-double.Pi * 12_000F * delta));
				break;
			}
		}

		if (this._shouldShoot) {
			this.FireGuns();
			this._shouldShoot = false;
		}
		this._currentBurstDelay -= (float) delta;
		
		Main.ScreenWrap(this);
	}

	public override void _Input(InputEvent @event) {
		if (this._alive == false) return;
		// check whether it is arrow down or up
		if (@event is not InputEventKey eventKey) return;
		if (eventKey.IsReleased()) {
			switch (eventKey.Keycode) {
				case Key.W:
				case Key.S:
				case Key.Down:
				case Key.Up: {
					this._steerDirection.linearDirection = SteerDirection.LinearDirection.NONE;
					break;
				}
				case Key.A:
				case Key.Left:
				case Key.D:
				case Key.Right: {
					this._steerDirection.angularDirection = SteerDirection.AngularDirection.NONE;
					break;
				}
			}
			this.PlayEngineVfx();
		} else if (eventKey.IsPressed()) {
			switch (eventKey.Keycode) 
			{
				case Key.S:
				case Key.Down:
					this._steerDirection.linearDirection = SteerDirection.LinearDirection.BACKWARDS;
					this.PlayEngineVfx();
					break;
				case Key.W:
				case Key.Up:
					this._steerDirection.linearDirection = SteerDirection.LinearDirection.FORWARDS;
					this.PlayEngineVfx();
					break;
				case Key.A:
				case Key.Left:
					this._steerDirection.angularDirection = SteerDirection.AngularDirection.ANTI_CLOCKWISE;
					this.PlayEngineVfx();
					break;
				case Key.D:
				case Key.Right:
					this._steerDirection.angularDirection = SteerDirection.AngularDirection.CLOCKWISE;
					this.PlayEngineVfx();
					break;
				case Key.Space:
					this._shouldShoot = true;
					break;
			}
		}
	}

	private void FireGuns() {
		if (!(_currentBurstDelay <= 0)) return;

		var projectile = _projectileScene.Instantiate<PlayerProjectile>();
		var direction = (this._selfFront.Position - this._selfCenter.Position).Normalized();
		var position = 1.25F * (this._selfFront.Position - this._selfCenter.Position) + this.Position;
		
		projectile.Init(position, direction);
		projectile.LinearVelocity += this.LinearVelocity;
		
		GetParent().AddChild(projectile);
		this._currentBurstDelay = this.FireBurstDelay;
	}

	public void ResetVfx() {
		this._selfFireLineShape.Hide();
		foreach (var fireLineShape in this._selfBackwardFireLineShapes) fireLineShape.Hide();
	}

	private void OnAreaEntered(Node node) {
		if (!IsInstanceValid(this)) return;
		if (node is not RigidBody2D body) return;
		
		this.ApplyCentralImpulse((this.Position - body.Position).Normalized() * 50F);

		this._health--;
		if (this._health != 0) return;
		this.ResetVfx();
		this._alive = false;
		this._steerDirection.angularDirection = SteerDirection.AngularDirection.NONE;
		this._steerDirection.linearDirection = SteerDirection.LinearDirection.NONE;
		EmitSignal(SignalName.ShipDestroyed);
	}

	public void PlayEngineVfx() {
		foreach (Line2D fireLineShape in this._selfBackwardFireLineShapes) fireLineShape.Hide();
		foreach (Line2D fireLineShape in this._selfSidewardsFireLineShapes) fireLineShape.Hide();
		this._selfFireLineShape.Hide();
		
		switch (this._steerDirection.linearDirection) {
			case SteerDirection.LinearDirection.FORWARDS:
				this._selfFireLineShape.SetVisible(true);
				break;
			case SteerDirection.LinearDirection.BACKWARDS:
				foreach (var fireLineShape in this._selfBackwardFireLineShapes) fireLineShape.SetVisible(true);
				break;
		}
		switch (this._steerDirection.angularDirection) {
			case SteerDirection.AngularDirection.CLOCKWISE:
				this._selfSidewardsFireLineShapes[0].SetVisible(true);
				this._selfBackwardFireLineShapes[0].SetVisible(true);
				break;
			case SteerDirection.AngularDirection.ANTI_CLOCKWISE:
				this._selfSidewardsFireLineShapes[1].SetVisible(true);
				this._selfBackwardFireLineShapes[1].SetVisible(true);
				break;
		}
	}
}
