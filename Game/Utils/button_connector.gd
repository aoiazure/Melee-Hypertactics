class_name ButtonConnector extends Node

@export var signal_name: StringName = ""
@export_multiline var expression_to_run: String = ""

var _expression: Expression

func _ready() -> void:
	if not expression_to_run.is_empty():
		var e: Expression = Expression.new()
		if e.parse(expression_to_run) != OK:
			push_error(e.get_error_text())
			return
		_expression = e
		get_parent().connect(signal_name, _expression.execute)
