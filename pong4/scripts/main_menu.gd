extends Control

@onready var single_player_button = $VBoxContainer/SinglePlayer
@onready var multiplayer_button = $VBoxContainer/MultiPlayer

var buttons = []
var current_index = 0

func _ready():
	# Sammle alle Buttons
	buttons = [single_player_button, multiplayer_button]
	
	# Setze Fokus auf ersten Button
	if buttons.size() > 0:
		update_button_focus()

func _input(event):
	# Eigene Navigation mit W/S oder Pfeiltasten
	if event.is_action_pressed("ui_down") or event.is_action_pressed("paddle_down"):
		current_index = (current_index + 1) % buttons.size()
		update_button_focus()
		accept_event()
	
	elif event.is_action_pressed("ui_up") or event.is_action_pressed("paddle_up"):
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

func _on_single_player_pressed():
	# Setze den Spielmodus auf Einzelspieler
	GameManager.is_multiplayer = false
	get_tree().change_scene_to_file("res://scenes/game.tscn")

func _on_multi_player_pressed():
	# Setze den Spielmodus auf Multiplayer
	GameManager.is_multiplayer = true
	get_tree().change_scene_to_file("res://scenes/game.tscn")
