using Godot;

namespace Asteroids;

public partial class GameMusicPlayer2d : AudioStreamPlayer2D
{
	public GameMusicPlayer2d() {
		this.Finished += () => this.Play();
	}
}
