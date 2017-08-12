#version 400 core
in vec2 TexCoords;

out vec4 fragColor;

uniform sampler2D image;
uniform vec2 stepSize;
uniform bool drawOutline = true;
uniform vec4 drawColor = vec4(1, 1, 1, 1);

vec4 outline()
{
	float alpha = 4 * texture(image, TexCoords).a;
	alpha -= texture(image, TexCoords + vec2( stepSize.x * 2, 0.0f)).a;
	alpha -= texture(image, TexCoords + vec2(-stepSize.x * 2, 0.0f)).a;
	alpha -= texture(image, TexCoords + vec2(0.0f,  stepSize.y * 2)).a;
	alpha -= texture(image, TexCoords + vec2(0.0f, -stepSize.y * 2)).a;

	vec4 resColor = vec4(drawColor.rgb, alpha);

	return resColor;
}

void main()
{
	if (drawOutline)
		fragColor = outline();
	else
		fragColor = texture(image, TexCoords) * drawColor;
}