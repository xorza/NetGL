uniform vec2 size;
uniform vec2 position;
uniform vec2 viewport_size;
uniform vec4 color;
uniform uint id;

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
	vec2 position = (in_vertices[gl_VertexID] * size + position) / viewport_size;
	gl_Position = vec4(position * 2.0 - vec2(1.0), 0.0, 1.0);
	
	texcoords = in_texcoords[gl_VertexID];
}

#endif


#ifdef FRAGMENT_SHADER
in vec2 texcoords;

layout (location = 0) out vec4 frag_color;	
layout (location = 1) out uvec4 frag_id;

void main(void)
{
	frag_color = color;
	frag_id.x = id;
}

#endif