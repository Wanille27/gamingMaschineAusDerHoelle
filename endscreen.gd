extends Node2D
@onready var score_label: Label = $VBoxContainer/Zeit_Score
func _on_button_button_down() -> void:
	get_tree().change_scene_to_file("res://startscreen.tscn")

func _ready():
	HighscoreManager.load_highscores()
	var text = "Highscores:\n"
	for entry in HighscoreManager.highscores:
		text += "%s | Score: %d | Time: %.2f\n" % [entry["name"], entry["score"], entry["time"]]
	$Zeit_Score.text = text
	print("Geladene Highscores:", HighscoreManager.highscores)
