extends CanvasLayer
#var coinscollected = 0
#var score = 0
#var time_elapsed := 0.0
#var timer_started = false
#var manual_time_string := ""
#var use_manual_time := false
#
#func _ready():
#	# GameManager-Node finden (z. B. im Root oder in einer bestimmten Szene)
#	var game_manager = get_tree().get_root().get_node("res://hud.gd") # Pfad anpassen!
#
#func _on_coin_2_body_entered(body: Node2D) -> void:
#	coinscollected = coinscollected + 1
#	$Zeit_Score.text = "Coins: " + str(coinscollected)
#
