extends Node

var score = 0
var timer_started = false
var manual_time_string := ""
var use_manual_time := false

@onready var score_label = $ScoreLabel 

func _ready():
	if OS.has_feature("dedicated_server"):
		print("Starting dedicated server...")
		MultiplayerManager.become_host()

func add_point():
	score += 1
	score_label.text = "Coins: %d" % score

func become_host():
	print("Become host pressed")
	%MultiplayerHUD.hide()
	MultiplayerManager.become_host()
	
func join_as_player_2():
	print("Join as player 2")
	%MultiplayerHUD.hide()
	MultiplayerManager.join_as_player_2()
