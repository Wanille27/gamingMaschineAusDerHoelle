#nullable enable
using System;
using System.Threading.Tasks;
using Godot;

namespace Asteroids;

public partial class PlayerProjectile : RigidBody2D
{
	private Line2D? _selfLineShape;
	[Export] private float _baseVelocity = 300;

	public PlayerProjectile() {
		this.BodyEntered += this.Destroy;
	}

	public PlayerProjectile? SpawnAt(Vector2 position, Vector2 direction, int lifetime = 4_000) {
		try {
			var projectile = (PlayerProjectile) this.Duplicate();
			projectile.SetPosition(position);
			projectile.SetLinearVelocity(direction.Normalized() * this._baseVelocity);

			projectile._Ready();
			Task.Run(async () => {
				await Task.Delay(lifetime);
				projectile.CallDeferred(MethodName.Destroy, new Variant().AsGodotObject());
			});
			return projectile;
		} catch (Exception) { return null; }
	}
	
	public override void _Ready() {
		base._Ready();

		this._selfLineShape = (Line2D) this.GetChild(1);
		this._selfLineShape.SetDefaultColor(Global.Instance.LineColor);
		this._selfLineShape.SetWidth(Global.Instance.LineWeight);
		this.SetVisible(true);
	}

	public override void _PhysicsProcess(double delta) {
		base._PhysicsProcess(delta);
		AsteroidsMain.ScreenWrap(this);
	}

	private void Destroy(Node? hitTarget) {
		//Console.Out.WriteLine($"Delete {this.GetClass().GetBaseName()}#{this.GetInstanceId()}");
		//this.Hide();
		//foreach (Node? child in this.GetChildren()) child?.QueueFree();
		this.QueueFree();
		//this.Dispose();
	}

	public override void _ExitTree() {
		base._ExitTree();
		this.Destroy(null);
	}

	//~PlayerProjectile() => this.Destroy(null);
}
