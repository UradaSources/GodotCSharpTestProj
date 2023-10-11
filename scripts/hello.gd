@tool
extends Node2D

var ext = load("res://scripts/cFunc.cs")

# Called when the node enters the scene tree for the first time.
func _ready():
	var newNode = ext.new()
	newNode.printHello("hello c#")

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass
