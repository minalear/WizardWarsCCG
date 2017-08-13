#version 400 core
layout(location = 0) in vec2 pos;
layout(location = 1) in vec2 uv;

out vec2 UV;

uniform mat4 proj;
uniform mat4 model;
uniform mat4 texMatrix;

void main()
{
	UV = (texMatrix * vec4(uv, 0, 1)).xy;
	gl_Position = proj * model * vec4(pos, 0, 1);
}