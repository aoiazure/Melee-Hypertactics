shader_type canvas_item;

uniform float scroll : hint_range(0.0, 1.0, 0.01);
uniform sampler2D image;

void fragment() {
	COLOR = texture(image, vec2(UV.x + 1.0-scroll, UV.y));
}
