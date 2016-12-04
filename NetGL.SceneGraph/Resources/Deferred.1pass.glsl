#if(__VERSION__ > 420)
	layout(early_fragment_tests) in;
#endif

layout(std140) uniform uniform_GlobalLights {
	vec3 uniform_CameraPosition;		
	int _offset0;
};

layout(std140) uniform uniform_Standart {
	float uniform_Time;
	uint uniform_ID;
	mat4 uniform_ModelViewProjectionMatrix;
	mat4 uniform_ModelViewMatrix;
	mat4 uniform_ModelMatrix;
	mat4 uniform_InversedModelMatrix;
	mat3 uniform_NormalMatrix;
};

#if defined VERTEX_SHADER
	in vec3 in_Position;	
	in vec3 in_Normal;
	in vec4 in_Color;
	in vec3 in_Tangent;
	in vec2 in_TexCoord0;
	
	out VertexOut {		
		vec3 normal;
		vec3 view_pos;
		vec4 color;
		vec3 tangent;
		vec2 texcoord0;
		
		vec3 view_pos_ws;
		vec3 normal_ws;
	} vs_out;
	
	struct vertex_info {
		vec3 model_pos;
		vec3 normal;
		vec4 color;
		vec3 tangent;
		vec2 texcoord0;
	};
		
	void getVertex(inout vertex_info vertex);
	
	#ifndef CUSTOM_VERTEX
		void getVertex(inout vertex_info vertex) {
			vertex.model_pos = in_Position;
			vertex.normal = in_Normal;
			vertex.color = in_Color;
			vertex.tangent = in_Tangent;
			vertex.texcoord0 = in_TexCoord0;
		}
	#endif
	
	void main(void) {
		vertex_info vertex;
		vertex.model_pos = vec3(0.0);
		vertex.normal = vec3(0.0);
		vertex.color = vec4(0.0);
		vertex.tangent = vec3(0.0);
		vertex.texcoord0 = vec2(0.0);
		
		getVertex(vertex);
	
		vec4 position = vec4(vertex.model_pos, 1.0);	
	
		vs_out.normal = normalize(uniform_NormalMatrix * vertex.normal);
		vs_out.tangent = normalize(uniform_NormalMatrix * vertex.tangent);
		
		vs_out.view_pos = (uniform_ModelViewMatrix * position).xyz;		
		vs_out.view_pos_ws = (uniform_ModelMatrix * vec4(in_Position, 1.0)).xyz - uniform_CameraPosition;
		vs_out.normal_ws = normalize((uniform_InversedModelMatrix * vec4(in_Normal, 0.0)).xyz);		
		vs_out.color = vertex.color;
		vs_out.texcoord0 = vertex.texcoord0;
		
		gl_Position = uniform_ModelViewProjectionMatrix * position;
	}
#endif

#if defined FRAGMENT_SHADER
	layout (location = 0) out vec4 albedo;	
	layout (location = 1) out vec4 emission;
	layout (location = 2) out vec3 view_pos;
	layout (location = 3) out vec4 normal;
	layout (location = 4) out uint data;
	
	in VertexOut {
		vec3 normal;
		vec3 view_pos;
		vec4 color;
		vec3 tangent;
		vec2 texcoord0;
		vec3 view_pos_ws;
		vec3 normal_ws;
	} frag_in;
	
	vec3 UnpackNormal(sampler2D sampler);
	
	struct fragment_info {	
		vec4 albedo;	
		vec3 normal;
		vec3 emission;
		float specPower;
		float specIntensity;
	};
	
	void getFragment(inout fragment_info fragment);
	
	void main(void) {
		fragment_info fragment;
		fragment.albedo = vec4(0.0, 0.0, 0.0, 1.0);
		fragment.normal = vec3(0.0);
		fragment.emission = vec3(0.0);
		fragment.specPower = float(0.0);
		fragment.specIntensity = float(0.0);
		
		getFragment(fragment);
		
		albedo.xyz = fragment.albedo.xyz;		
		emission.xyz = fragment.emission.xyz;	
		normal.xyz = fragment.normal.xyz;
		view_pos.xyz = frag_in.view_pos.xyz;
		
		normal.w = fragment.specPower;		
		albedo.w = fragment.specIntensity;	
			
		data = uniform_ID;
	}
	
	vec3 UnpackNormal(sampler2D sampler, vec2 texCoords) {
		vec3 N = normalize(frag_in.normal);
		vec3 T = normalize(frag_in.tangent);
		vec3 B = normalize(cross(N, T));
		mat3 TBN = mat3(T, B, N);
		vec3 nm = texture(sampler, texCoords).xyz * 2.0 - vec3(1.0);
		return TBN * normalize(nm);
	}	
#endif