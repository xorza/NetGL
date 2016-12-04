uniform vec2 size;
uniform vec2 position;
uniform sampler2D texture1;

#ifdef VERTEX_SHADER
const vec2[] in_vertices = vec2[4]
(
	vec2( 0,  0),
	vec2( 1,  0),
	vec2( 0,  1),                                  
	vec2( 1,  1)
);

const vec2[] in_texcoords = vec2[4]
(
	vec2(0, 0),
	vec2(1, 0),
	vec2(0, 1),
	vec2(1, 1)
);

out vec2 texcoords;

void main(void)
{
	vec2 position = in_vertices[gl_VertexID] * size + position;
	gl_Position = vec4(position * 2.0 - vec2(1.0), 0.0, 1.0);
	
	texcoords = in_texcoords[gl_VertexID];
}

#endif


#ifdef FRAGMENT_SHADER
in vec2 texcoords;

layout (location = 0) out vec4 frag_color;

void main(void)
{
	frag_color = texture(texture1, texcoords);
}

#endif