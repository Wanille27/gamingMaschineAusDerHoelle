extends Area2D

func _on_body_entered(body: Node2D) -> void:
	if body.is_in_group("Player"):
		# Multiplayer-Check
		if MultiplayerManager.multiplayer_mode_enabled:
			if multiplayer.get_unique_id() == body.get("player_id"):
				HighscoreManager.add_highscore(body.get("score"), body.get("time_elapsed"))
				print("Player %s WINS!" % multiplayer.get_unique_id())
