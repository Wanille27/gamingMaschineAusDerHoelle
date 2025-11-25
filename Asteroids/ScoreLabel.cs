using System;
using System.Threading.Tasks;
using Godot;

namespace Asteroids;

public partial class ScoreLabel : Label
{
	private bool _running = true;
	private Task _updateTextTask;

	private Color _color;
	
	public ScoreLabel() {
		this._updateTextTask = Task.Run(() => {
			while (this._running) {
				Task.Delay(100).Wait();
				this.CallDeferred(Label.MethodName.SetText, AsteroidsMain.Score.ToString());
			}

			return Task.CompletedTask;
		});
		this._color = this.LabelSettings.GetFontColor();
	}

	~ScoreLabel() {
		this._running = false;
		this._updateTextTask.Wait();
		this._updateTextTask.Dispose();
		this.QueueFree();
	}

	public void SetToEndState(string text) {
		this._running = false;
		this._updateTextTask.Wait();
		this.CallDeferred(Label.MethodName.SetText, text);
		this._running = true;

		var lightUp = false;
		float alpha = 99;
		this._updateTextTask = Task.Run(async () => {
			while (this._running) {
				//var color = (Color) this.LabelSettings.CallDeferred(LabelSettings.MethodName.GetFontColor);
				await Task.Delay(12);
				if (lightUp) {
					alpha++;
					if (alpha >= 100) lightUp = false;
				} else {
					alpha--;
					if (alpha <= 0) lightUp = true;
				}

				this._color.A = alpha / 100;

				//await Console.Out.WriteLineAsync($"Modulate: {lightUp}::{this._color.A}");
				//this.SetModulate(modulate);
				_ = this.LabelSettings.CallDeferred(LabelSettings.MethodName.SetFontColor, this._color);
			}
		});
	}
}
