using System;
using System.Threading.Tasks;
using Godot;

namespace Asteroids;

public partial class PlayerShip : RigidBody2D {

	private bool _breakAngular = false;
	private SteerDirection _steerDirection;
	private Vector2 _steerVector = Vector2.Zero;
	
	private Node2D _selfFront;
	private Vector2 _selfFrontBasePosition;
	private Node2D _selfCenter;
	public Line2D SelfLineShape;
	private Line2D _selfFireLineShape;
	private Line2D[] _selfBackwardFireLineShapes = new Line2D[2];
	private Line2D[] _selfSidewardsFireLineShapes = new Line2D[2];
	private CollisionPolygon2D _selfCollisionBody;
	private readonly PlayerProjectile _baseProjectile;
	private AudioStreamPlayer2D _hitSoundEffectPlayer;
	private RocketEngineStreamPlayer2d _engineSoundEffectPlayer;

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
		this.SetPosition(Vector2.Zero);
		
		var projectileScene = GD.Load<PackedScene>("res://Player_Projectile_1.tscn");
		var projectileSceneI  = projectileScene.Instantiate<Node2D>();
		this._baseProjectile = (PlayerProjectile) projectileSceneI.GetChild(0).Duplicate();
		
		projectileSceneI.QueueFree();
		this._baseProjectile._Ready();
		//this.baseProjectile.SetVisible(false);
		this._health = this.MaxHealth;
	}
	
	public override void _Ready() {
		base._Ready();

		Console.Out.WriteLine($":: {Input.GetConnectedJoypads()}");
		Console.Out.WriteLine("Initializing PlayerShip");
		var global = Global.Instance;
		
		this._selfCollisionBody = this.GetChild(0) as CollisionPolygon2D;
		if (this._selfCollisionBody == null) throw new ("Can't find LineShape");

		this.SelfLineShape = this.GetChild(1) as Line2D;
		if (this.SelfLineShape == null) throw new ("Can't find LineShape");
		
		this._selfFront = this.GetChild(2) as Marker2D;
		if (this._selfFront == null) throw new ("Can't find Node2D");
		this._selfFrontBasePosition = this._selfFront.Position;
		
		this._selfCenter = this.GetChild(3) as Marker2D;
		if (this._selfCenter == null) throw new ("Can't find Node2D");
		
		this._selfFireLineShape = this.GetChild(4) as Line2D;
		if (this._selfFireLineShape == null) throw new ("Can't find Node2D");
		
		
		this._selfBackwardFireLineShapes[0] = this.GetChild(5) as Line2D;
		if (this._selfBackwardFireLineShapes[0] == null) throw new ("Can't find Node2D");
		
		this._selfBackwardFireLineShapes[1] = this.GetChild(6) as Line2D;
		if (this._selfBackwardFireLineShapes[1] == null) throw new ("Can't find Node2D");

		foreach (Line2D backwardFireLine in this._selfBackwardFireLineShapes) {
			backwardFireLine.SetDefaultColor(this._shipEngineFireColor);
			backwardFireLine.SetWidth(global.LineWeight);
			backwardFireLine.Hide();
		}
		this._selfSidewardsFireLineShapes[0] = this.GetChild(7) as Line2D;
		if (this._selfBackwardFireLineShapes[0] == null) throw new ("Can't find Node2D");
		this._selfSidewardsFireLineShapes[1] = this.GetChild(8) as Line2D;
		if (this._selfBackwardFireLineShapes[1] == null) throw new ("Can't find Node2D");
		foreach (Line2D sidewardsFireLine in this._selfSidewardsFireLineShapes) {
			sidewardsFireLine.SetDefaultColor(this._shipEngineFireColor);
			sidewardsFireLine.SetWidth(global.LineWeight);
			sidewardsFireLine.Hide();
		}

		this._hitSoundEffectPlayer = (AudioStreamPlayer2D) this.FindChild("AsteroidCollisionAudioStreamPlayer2D");
		this._engineSoundEffectPlayer = (RocketEngineStreamPlayer2d) this.FindChild("RocketEngineStreamPlayer2D");
		
		this._selfFireLineShape.SetDefaultColor(this._shipEngineFireColor);
		this._selfFireLineShape.SetWidth(global.LineWeight);
		this._selfFireLineShape.Hide();
		
		this.SelfLineShape.SetDefaultColor(this._shipColor);;
		this.SelfLineShape.SetWidth(global.LineWeight);
		
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

		if (this._steerVector != Vector2.Zero) {
			var forwardDir = (this._selfFront.Position - this._selfCenter.Position).Normalized();
			var angle = forwardDir.AngleTo(this._steerVector);
			Console.Out.WriteLine($"angle {angle}");
			//var targetDir = this._steerVector - forwardDir;


			// Apply force to move in the target direction
			//this.ApplyForce(
			//	targetDir * (float)(
			//		8_000 * Global.Instance.PlayerShipSpeedMultiplier * delta
			//	)
			//);

			// Calculate angle to target for turning
			//var angleToTarget = forwardDir.AngleTo(targetDir);
			//Console.Out.WriteLine($"Angle: {angleToTarget} radians");

			// Apply torque to turn towards the target direction
			// A small tolerance to prevent jittering when aligned
			var torqueDirection = -Mathf.Sign(angle);
			this.ApplyTorque((float)(torqueDirection * double.Pi * 12_000F * delta));
			//if (Mathf.Abs(angle) > 0.05f) 
			//{
			//	var torqueDirection = -Mathf.Sign(angle);
			//	this.ApplyTorque((float)(torqueDirection * double.Pi * 12_000F * delta));
			//}
			
		} else {

			//this.SelfCenter.SetPosition(this.SelfLineShape.Points[0]);
			switch (this._steerDirection.linearDirection) {
				case SteerDirection.LinearDirection.FORWARDS:
					this.ApplyForce(
						(this._selfFront.Position - this._selfCenter.Position).Normalized() * (float)(
							8_000 * Global.Instance.PlayerShipSpeedMultiplier * delta
						)
					);
					break;
				case SteerDirection.LinearDirection.BACKWARDS:
					this.ApplyForce(
						(this._selfCenter.Position - this._selfFront.Position).Normalized() * (float)(
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
		}

		if (this._shouldShoot) {
			this.FireGuns();
			this._shouldShoot = false;
		}
		this._currentBurstDelay -= (float) delta;
		
		AsteroidsMain.ScreenWrap(this);
	}

	public override void _Input(InputEvent @event) {
		if (this._alive == false || !this.IsNodeReady()) return;
		this._steerVector = Input.GetVector(
			Global.Joystick0Left,
			Global.Joystick0Right,
			Global.Joystick0Down,
			Global.Joystick0Up
		);

		if (this._steerVector != Vector2.Zero) Console.Out.WriteLine($"Steer vector: {this._steerVector}");
		// check whether it is arrow down or up
		if (@event is not InputEventKey eventKey) return;
		Console.Out.WriteLine($"key: {eventKey}");

		this._engineSoundEffectPlayer.Start();
		
		// (-0.0078122616, -0.9999695) U, L, ...
		// (0.70710677, -0.70710677) L & U, R & U, ...
		
		//for (var i = 0; i < 1; i++) {
		//	if (this._steerVector[i] < 0.01) this._steerVector[i] = 0;
		//	else if (this._steerVector[i] > 0.99) this._steerVector[i] = 1;
		//}
		//this._steerVector = this._steerVector.Normalized();
		
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
		if (!(this._currentBurstDelay <= 0)) return;
		PlayerProjectile projectile = this._baseProjectile.SpawnAt(
			1.25F * (this._selfFront.Position - this._selfCenter.Position) + this.Position,
			(this._selfFront.Position - this._selfCenter.Position).Normalized()
		);
		if (projectile == null) return;
		Console.Out.WriteLine($"{(this._selfFront.Position - this._selfCenter.Position) + this.Position}");
		projectile.LinearVelocity += this.LinearVelocity;
		this.AddSibling(projectile);
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
		
		this._hitSoundEffectPlayer.Play();
		
		this._health--;
		if (this._health != 0) return;
		this.ResetVfx();
		this._alive = false;
		var main = (AsteroidsMain) this.GetParent().GetParent();
		this._steerDirection.angularDirection = SteerDirection.AngularDirection.NONE;
		this._steerDirection.linearDirection = SteerDirection.LinearDirection.NONE;
		main.EmitSignal(AsteroidsMain.SignalName.GameOver);
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

	public override void _ExitTree() {
		base._ExitTree();
		this._baseProjectile.QueueFree();
	}
}
