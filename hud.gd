extends CanvasLayer

var coinscollected = 0
var score = 0
var time_elapsed := 0.0
var timer_started = false
var manual_time_string := ""
var use_manual_time := false


func _ready():
	$coincount.text = "Coins: " + str(coinscollected)


func _on_coin_2_body_entered(body: Node2D) -> void:
	coinscollected = coinscollected + 1
	$coincount.text = "Coins: " + str(coinscollected)
	
func _process(delta):
	if Input.is_action_pressed("move_left") or Input.is_action_pressed("move_right") or Input.is_action_pressed("move_up"):
		timer_started = true
		
	if timer_started :
		if use_manual_time and manual_time_string != "":
			set_time_from_string(manual_time_string)
		else:
			time_elapsed += delta
			var minutes = int(time_elapsed) / 60
			var seconds = int(time_elapsed) % 60
			var milliseconds = int((time_elapsed - int(time_elapsed)) * 1000)
			$timecount.text = "Time: %02d:%02d.%03d" % [minutes, seconds, milliseconds]
				
			#if multiplayer.is_server():
				#var game_manager = get_tree().get_current_scene().get_node("GameManager")
				#if game_manager:
					#game_manager.time_elapsed += 5.0
					
func set_time_from_string(time_string: String):
	var parts = time_string.split(":")
	if parts.size() == 2:
		var minutes = int(parts[0])
		var sec_parts = parts[1].split(".")
		if sec_parts.size() == 2:
			var seconds = int(sec_parts[0])
			var milliseconds = int(sec_parts[1])
			%timecount.text = "Zeit: %02d:%02d.%03d" % [minutes, seconds, milliseconds]
			
func game_over():
	var endscreen_scene = load("res://endscreen.tscn").instantiate()
	endscreen_scene.coinscollected = coinscollected
	endscreen_scene.time_elapsed = time_elapsed	
	# Alte Szene entfernen und neue hinzufügen
	get_tree().root.add_child(endscreen_scene)
	get_tree().current_scene.queue_free()
	get_tree().change_scene_to_file("res://endscreen.tscn")
