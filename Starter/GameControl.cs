using System.Collections.Generic;
using Godot;
using GodotPlugins.Game;

namespace Starter;

public partial class GameControl : PanelContainer
{
	public enum GameTypeE {
		Asteroids,
		BitKnight,
		Pong,
	}

	[Export] public GameTypeE GameType { get; set; }

	public Button SelectionButton { get; private set; }
	public SelectionLabel SelectionLabel { get; private set; }

	public override void _Ready() {
		base._Ready();
		Node container = this.GetChild(0);
		this.SelectionButton = (Button) container.FindChild("Button", recursive: true);
		this.SelectionLabel = (SelectionLabel) container.FindChild("SelectionLabel", recursive: true);
		this.SelectionLabel.SetVisible(false);
		this.SetFocusMode(FocusModeEnum.All);
	}

	public void Select() {
		this.SelectionLabel.SetVisible(true);
		this.SelectionButton.Set("theme_override_colors/font_color", MenuPanel.SelectedTextColor);
		this.GrabFocus();
	}
	public void DeSelect() {
		this.SelectionButton.Set("theme_override_colors/font_color", MenuPanel.DefaultTextColor);
		this.SelectionLabel.SetVisible(false);
	}
}
