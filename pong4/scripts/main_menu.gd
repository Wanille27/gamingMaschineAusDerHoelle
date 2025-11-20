extends Control

func _on_single_player_pressed() -> void:
	# Setze den Spielmodus auf Singleplayer
	GameManager.is_multiplayer = false
	get_tree().change_scene_to_file("res://scenes/game.tscn")

func _on_multi_player_pressed() -> void:
	# Setze den Spielmodus auf Multiplayer
	GameManager.is_multiplayer = true
	get_tree().change_scene_to_file("res://scenes/game.tscn")

func _on_quit_button_pressed():
	get_tree().quit()
