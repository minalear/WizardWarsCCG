#version 400 core
in vec2 TexCoords;

out vec4 fragColor;

uniform sampler2D image;
uniform vec4 drawColor = vec4(1, 1, 1, 1);

void main()
{
	fragColor = texture(image, TexCoords) * drawColor;
}