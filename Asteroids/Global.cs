using Godot;

namespace Asteroids;

public partial class Global : Node {
	public static Global Instance { get; private set; }
	static Global() => Instance = new ();

	public override void _Ready() {
		this.rng.Randomize();
	}
	
	public readonly RandomNumberGenerator rng = new ();

	[Export] public float PlayerShipSpeedMultiplier		= 1.0F;
	[Export] public float PlayerShipScale				= 0.25F;
	
	[Export] public float AsteroidGlobalScale			= 1F;
	[Export] public float AsteroidGlobalLinearSpeedMin	= 20F;
	
	[Export] public float AsteroidLargeSpeedMultiplier	= 0.75F;
	[Export] public float AsteroidMediumSpeedMultiplier	= 1.75F;
	[Export] public float AsteroidSmallSpeedMultiplier	= 2.5F;
	
	[Export] public float AsteroidLargeScaleMultiplier	=	0.5F;
	[Export] public float AsteroidMediumScaleMultiplier =  0.25F;
	[Export] public float AsteroidSmallScaleMultiplier  = 0.125F;
	
	[Export] public float AsteroidAngularSpeed			= 0;
	[Export] public float AsteroidAngularRotation		= 0;
	
	[Export] public Color LineColor						= Colors.Red;
	[Export] public float LineWeight					= 2;
	[Export] public Vector2 ScreenRect					= Vector2.Zero;

	public const string Joystick0Left = "Joystick0Left";
	public const string Joystick0Right = "Joystick0Right";
	public const string Joystick0Down = "Joystick0Down";
	public const string Joystick0Up = "Joystick0Up";

}
