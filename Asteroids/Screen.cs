using System;
using Godot;

namespace Asteroids;

public partial class Screen : MeshInstance2D
{
	public override void _Ready() {
		base._Ready();
		Vector2 screenSize = this.GetViewportRect().Size;
		this.SetPosition(screenSize / 2);
		this.SetScale(new (screenSize.X, screenSize.Y));
		//    Console.Out.WriteLine($"Screen Point (X: {this.Polygon[i].X}, Y: {this.Polygon[i].Y})");
		//for (var i = 0; i < this.Polygon.Length; i++) {
		//    Vector2 polygon = this.Polygon[i];
		//    Console.Out.WriteLine($"Screen Point (X: {this.Polygon[i].X}, Y: {this.Polygon[i].Y})");
		//}
	}
}
