#if(__VERSION__ > 420)
	layout(early_fragment_tests) in;
#endif

#if defined USE_LIGHTING
	struct Light {
		vec3 Position;
		vec3 Direction;
		vec3 Diffuse;
		vec3 Attenuation;
		int Type;   //0 - directional, 1 - point, 2 - spot
	};
#endif

layout(std140) uniform uniform_GlobalLights {
	vec3 uniform_CameraPosition;		
	int _offset0;
	vec3 uniform_Ambient;
	int _offset1;		
				
	#if defined USE_LIGHTING
		int uniform_LightCount;
		int uniform_LightCastingShadowNumber;	
		Light uniform_Light[MAX_LIGHTS];
	#endif
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
	
	uniform vec3 asd;
		
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
	layout (location = 0) out vec4 fragColor;

	struct fragment_info {	
		vec4 albedo;
		vec3 normal;
		vec3 emission;
		float specPower;
		float specIntensity;
	};
	
	in VertexOut {
		vec3 normal;
		vec3 view_pos;
		vec4 color;
		vec3 tangent;
		vec2 texcoord0;
		vec3 view_pos_ws;
		vec3 normal_ws;
	} frag_in;

	void getFragment(inout fragment_info fragment);
	
	void main(void) {
		fragment_info fragment;
		fragment.albedo = vec4(0.0, 0.0, 0.0, 1.0);
		fragment.normal = vec3(0.0);
		fragment.emission = vec3(0.0);
		fragment.specPower = float(0.0);
		fragment.specIntensity = float(0.0);
		
		getFragment(fragment);
		
		vec3 result = fragment.emission;
				
		#if defined USE_LIGHTING
			result += uniform_Ambient * fragment.albedo.xyz;			
            
			if(length(fragment.albedo) > 0.0)
			{
				for(int i = 0; i < uniform_LightCount; ++i) {
					vec3 lightVec = vec3(0.0);
					float attenuation = 1.0;
					Light light = uniform_Light[i];
			
					if(light.Type == 0) {
						attenuation = 1.0;
						lightVec = light.Direction;
					}
					if(light.Type == 1) {			
						lightVec = light.Position - frag_in.view_pos;
						float distance = length(lightVec);
						lightVec = normalize(lightVec);
						attenuation = 1.0 / (light.Attenuation.x + light.Attenuation.y * distance + light.Attenuation.z * distance * distance);
					}
			
					if(attenuation > 0.0) {
						float lambertFactor = dot(fragment.normal, lightVec);
						float diffuseFactor = lambertFactor * attenuation;
											
						if(diffuseFactor > 0.0) {
							result += fragment.albedo.xyz * light.Diffuse * diffuseFactor;
						
							if(fragment.specPower > 0.0 && fragment.specIntensity > 0.0) {
								vec3 v = normalize(-frag_in.view_pos);
								vec3 r = -reflect(lightVec, fragment.normal);
								float specFactor = clamp(pow(max(0.0, dot(r, v)), fragment.specPower), 0.0, 1.0);
								result += light.Diffuse * specFactor * fragment.specIntensity * attenuation;
							}
						}
					}
				}
			}
		#endif
		
		fragColor = vec4(result, fragment.albedo.w);
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