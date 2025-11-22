extends Area2D

@export var is_player_one = false
@export var is_ai_controlled = false
@export var ball: Node2D  # Ball-Referenz für die KI

@onready var cshape = $CollisionShape2D

var active = true

var up_input = "paddle_up"
var down_input = "paddle_down"

const max_velocity = 10.0
var velocity = 0.0
var acceleration = 50.0

func _ready() -> void:
	# Warte einen Frame, damit game.gd is_ai_controlled setzen kann
	await get_tree().process_frame
	_setup_controls()

func _setup_controls() -> void:
	if not is_player_one and not is_ai_controlled:
		up_input = "paddle_up_two"
		down_input = "paddle_down_two"

func _physics_process(delta: float) -> void:
	if !active:
		return
	
	var move_dir = 0.0

	if is_ai_controlled:
		if ball:
			var tolerance = 5.0
			if ball.global_position.y > global_position.y + tolerance:
				move_dir = 1.0
			elif ball.global_position.y < global_position.y - tolerance:
				move_dir = -1.0
	else:
		move_dir = Input.get_axis(up_input, down_input)
	
	velocity += move_dir * acceleration * delta
	
	if move_dir == 0.0:
		velocity = move_toward(velocity, 0.0, 2.0)
	
	velocity = clamp(velocity, -max_velocity, max_velocity)
	
	global_position.y += velocity
	global_position.y = clampf(global_position.y, 0, get_window().size.y +300)

func _on_body_entered(body: Node2D) -> void:
	if body is Ball:
		body.bounce_from_paddle(global_position.y, cshape.shape.get_rect().size.y)
