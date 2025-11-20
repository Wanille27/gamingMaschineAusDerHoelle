using System;
using System.Threading;
using Godot;

namespace Asteroids;

public sealed partial class RocketEngineStreamPlayer2d : AudioStreamPlayer2D {
	public delegate void StartEvent();

	public event StartEvent StartPlayer;
	private bool _running;
		
	//public event EventHandler StartPlaying;

	public CancellationTokenSource CancellationTokenSource { get; private set;  }
	private readonly CancellationToken cancellationToken;

	public RocketEngineStreamPlayer2d() {
		this.CancellationTokenSource = new ();
		this.cancellationToken = this.CancellationTokenSource.Token;

		//this.StartPlaying += () => {
		//    new Thread(() => {
		//        while (!this.cancellationToken.IsCancellationRequested) {
		//            if (!this.IsPlaying()) this.Play();
		//        }
		//    });
		//};
		this.StartPlayer += () => {
			if (this._running) return;
			this._running = true;
			new Thread(() => {
				while (!this.cancellationToken.IsCancellationRequested) {
					if (!this.IsPlaying()) this.Play();
				}

				this._running = false;
			}).Start();
		};
	}

	public void Start() { this.StartPlayer?.Invoke(); }
	public override void _ExitTree() {
		base._ExitTree();
		this.CancellationTokenSource.Cancel();
	}
}
