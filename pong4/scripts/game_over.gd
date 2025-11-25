extends Control

@onready var winner_label = $VBoxContainer/WinnerLabel
@onready var score_label = $VBoxContainer/ScoreLabel
@onready var restart_button = $VBoxContainer/ButtonContainer/RestartButton
@onready var main_menu_button = $VBoxContainer/ButtonContainer/MainMenuButton

var buttons = []
var current_index = 0

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
	
	# Sammle alle Buttons
	buttons = [restart_button, main_menu_button]
	
	# Setze Fokus auf ersten Button
	if buttons.size() > 0:
		update_button_focus()

func _input(event):
	# Navigation mit W/S, A/D oder Pfeiltasten
	if event.is_action_pressed("ui_right") or event.is_action_pressed("ui_down"):
		current_index = (current_index + 1) % buttons.size()
		update_button_focus()
		accept_event()
	
	elif event.is_action_pressed("ui_left") or event.is_action_pressed("ui_up"):
		current_index = (current_index - 1 + buttons.size()) % buttons.size()
		update_button_focus()
		accept_event()
	
	elif event.is_action_pressed("ui_accept"):
		if buttons[current_index]:
			buttons[current_index].emit_signal("pressed")
		accept_event()

func update_button_focus():
	# Entferne Fokus von allen Buttons
	for btn in buttons:
		if btn:
			btn.release_focus()
	
	# Setze Fokus auf aktuellen Button
	if buttons[current_index]:
		buttons[current_index].grab_focus()

func _on_restart_button_pressed():
	get_tree().change_scene_to_file("res://scenes/main_menu.tscn")

#TODO//implementiere Weg zurück ins Main Menu der Spiele
func _on_main_menu_button_pressed():
	#get_tree().change_scene_to_file()
	pass
