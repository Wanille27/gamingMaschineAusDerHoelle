using Godot;

namespace Asteroids;

public partial class PlayerProjectile : RigidBody2D
{
    [Export] private float _baseVelocity = 300;
    [Export] private float lifetime = 4.0f;

    public override void _Ready()
    {
        var timer = new Timer();
        timer.WaitTime = lifetime;
        timer.OneShot = true;
        timer.Timeout += QueueFree;
        AddChild(timer);
        timer.Start();

        var line = GetNode<Line2D>("Line2D");
        line.DefaultColor = Global.Instance.LineColor;
        line.Width = Global.Instance.LineWeight;
    }

    public void Init(Vector2 position, Vector2 direction)
    {
        Position = position;
        LinearVelocity = direction.Normalized() * _baseVelocity;
    }
    
    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        Main.ScreenWrap(this);
    }

    private void OnBodyEntered(Node body)
    {
        QueueFree();
    }
}
