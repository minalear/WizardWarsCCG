#version 400 core
in vec2 pos;

out vec2 TexCoords;

uniform mat4 proj;
uniform mat4 model;

void main()
{
	TexCoords = pos;
	gl_Position = proj * model * vec4(pos, 0, 1);
}