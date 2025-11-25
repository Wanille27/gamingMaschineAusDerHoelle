using System.Globalization;
using System.Threading.Tasks;
using Godot;

namespace Asteroids;

public partial class FpsLabel : Label {
	private bool _running = true;
	private readonly Task _updateTextTask;
	
	public FpsLabel() {
		this._updateTextTask = Task.Run(() => {
			while (this._running) {
				Task.Delay(100).Wait();
				this.CallDeferred(Label.MethodName.SetText, Engine.GetFramesPerSecond().ToString(CultureInfo.CurrentCulture));
			}
			return Task.CompletedTask;
		});
	}

	~FpsLabel() {
		this._running = false;
		this._updateTextTask.Wait();
		this._updateTextTask.Dispose();
		this.QueueFree();
	}
}
