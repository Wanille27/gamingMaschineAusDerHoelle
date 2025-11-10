class_name Example
extends Node2D

# Assuming `TileMap` is your play area and your players are `CharacterBody2D` nodes.
@onready var level: TileMap = $TileMap
@onready var players: Array[CharacterBody2D] = [$Player, $Player2]
@onready var split_screen: SplitScreen2D

func _ready():
	var config := SplitScreen2DConfig.new()
	config.play_area = level
	config.min_players = 2
	config.max_players = 4
	config.transparent_background = true
	config.rebuild_when_player_added = false
	config.rebuild_when_player_removed = false
	config.rebuild_when_screen_resized = false
	config.rebuild_delay = 0.1
	
	split_screen = SplitScreen2D.from_config(config)
	
	for player in players:
		split_screen.add_player(player)
	
	add_child(split_screen)
