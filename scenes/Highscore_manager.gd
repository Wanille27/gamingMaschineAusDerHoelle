extends Node

var highscores: Array = []
const SAVE_PATH = "user://savedata.tres"

func add_highscore(score: int, time: float, name: String = ""):
	if name == "":
		name = get_random_name()
	highscores.append({ "name": name, "score": score, "time": time })
	highscores.sort_custom(_sort_by_score)
	if highscores.size() > 10:
		highscores = highscores.slice(0, 10)
	save_highscores()

func _sort_by_score(a, b):
	return b["score"] - a["score"]

func get_random_name() -> String:
	var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
	var name = ""
	for i in range(5):
		name += chars[randi() % chars.length()]
	return "Player_" + name

func save_highscores():
	var file = FileAccess.open(SAVE_PATH, FileAccess.WRITE)
	file.store_string(JSON.stringify(highscores))
	file.close()

func load_highscores():
	if FileAccess.file_exists(SAVE_PATH):
		var file = FileAccess.open(SAVE_PATH, FileAccess.READ)
		highscores = JSON.parse_string(file.get_as_text())
		file.close()
