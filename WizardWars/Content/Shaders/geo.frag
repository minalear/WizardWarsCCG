#version 400 core
out vec4 outColor;

uniform vec4 DrawColor;

void main()
{
	outColor = DrawColor;
}