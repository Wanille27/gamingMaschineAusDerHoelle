using Godot;

namespace Asteroids;

public partial class ScoreLabel : Label
{
    public override void _Process(double delta)
    {
        Text = Main.Score.ToString();
    }

    public void SetFinalScore(string text)
    {
        Text = text;
    }
}

