class_name SaveData 
extends Resource

const SAVE_PATH := "user://savedata.tres"
@export var high_score:= 0
@export var high_time := ""
@export var name := ""


func save() -> void:
	ResourceSaver.save(self, SAVE_PATH)

static func load_or_crate() -> SaveData:
	var res:SaveData
	if FileAccess.file_exists(SAVE_PATH):
		res = load(SAVE_PATH) as SaveData
	else:
		res = SaveData.new()
	return res
