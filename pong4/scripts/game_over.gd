extends Control

@onready var winner_label = $VBoxContainer/WinnerLabel
@onready var score_label = $VBoxContainer/ScoreLabel

func _ready():
	# Zeige Gewinner und Score an
	var winner = GameManager.winner
	var final_score = GameManager.final_score
	
	if winner == 1:
		winner_label.text = "Spieler 1 gewinnt!"
	elif winner == 2:
		winner_label.text = "Spieler 2 gewinnt!"
	else:
		winner_label.text = "Unentschieden!"
	
	score_label.text = str(final_score.x) + " : " + str(final_score.y)

func _on_restart_button_pressed():
	# Starte das Spiel neu mit denselben Einstellungen
	get_tree().change_scene_to_file("res://scenes/game.tscn")

func _on_main_menu_button_pressed():
	# Zurück zum Hauptmenü
	get_tree().change_scene_to_file("res://scenes/main_menu.tscn")
