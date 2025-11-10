extends Area2D


func _on_body_entered(body: Node2D) -> void:
	get_tree().change_scene_to_file("res://endscreen.tscn")
	if MultiplayerManager.multiplayer_mode_enabled && multiplayer.get_unique_id() == body.player_id:
		print("Player %s WINS!" % multiplayer.get_unique_id())
