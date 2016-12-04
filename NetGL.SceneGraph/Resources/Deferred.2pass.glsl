#ifdef VERTEX_SHADER
	in vec3 in_Position;
	in vec2 in_TexCoord0;
	
	out vec2 texCoord;
	
	void main(void) {
		gl_Position = vec4(in_Position, 1);
		texCoord = in_TexCoord0;
	}
#endif

#ifdef FRAGMENT_SHADER
	struct Light {
		vec3 Position;
		vec3 Direction;
		vec3 Diffuse;
		vec3 Attenuation;
		int Type;   //0 - directional, 1 - point, 2 - spot
	};
	
	layout(std140) uniform uniform_GlobalLights {
		vec3 uniform_CameraPosition;
		int _offset0;
		vec3 uniform_Ambient;
		int _offset1;	
		
		int uniform_LightCount;
		int uniform_LightCastingShadowNumber;	
		Light uniform_Light[MAX_LIGHTS];
	};

	layout (location = 0) out vec4 fragColor;
	
	uniform sampler2D uniform_AlbedoTexture;
	uniform sampler2D uniform_NormalTexture;
	uniform sampler2D uniform_PositionTexture;
	uniform usampler2D uniform_DataTexture;
	uniform sampler2D uniform_EmissionTexture;
	//uniform sampler2D uniform_DepthTexture;
	
	uniform vec4 uniform_Color;
	
	in vec2 texCoord;
	
	struct fragment_info {
		vec3 albedo;
		vec3 normal;
		vec3 view_pos;
		vec3 emission;
		uint id;
		float specPower;
		float specIntensity;
		//float depth;
	};
	
	fragment_info UnpackGBuffer() {
		fragment_info result;
	
		vec4 albedo = texture(uniform_AlbedoTexture, texCoord);
		vec4 normal = texture(uniform_NormalTexture, texCoord);
		uvec4 data = texture(uniform_DataTexture, texCoord);
		vec4 emission = texture(uniform_EmissionTexture, texCoord);
		vec4 view_pos = texture(uniform_PositionTexture, texCoord);
		//vec4 depth = texture(uniform_DepthTexture, texCoord);
	
		result.albedo = albedo.xyz;
		result.emission = emission.xyz;
		result.normal = normalize(normal.xyz);
		result.view_pos = view_pos.xyz;
		result.id = data.x;
		result.specPower = normal.w;
		result.specIntensity = albedo.w;
		//result.depth = depth.x;
	
		return result;
	}
	
	void main(void) {
		fragment_info fragment = UnpackGBuffer();
		
		if(fragment.id == 0u) {
			fragColor = uniform_Color;
			return;
		}
		
		vec3 result = fragment.emission;
				
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
					lightVec = light.Position - fragment.view_pos;
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
							vec3 v = normalize(-fragment.view_pos);
							vec3 r = -reflect(lightVec, fragment.normal);
							float specFactor = clamp(pow(max(0.0, dot(r, v)), fragment.specPower), 0.0, 1.0);
							result += light.Diffuse * specFactor * fragment.specIntensity * attenuation;
						}
					}
				}
			}
		}
		
		fragColor = vec4(result, 1.0);
	}
#endif