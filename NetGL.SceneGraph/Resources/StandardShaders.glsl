#ifdef FlatColor
uniform vec4 uniform_Color = vec4(1.0);

void getFragment(inout fragment_info fragment) {
	fragment.emission = uniform_Color.xyz;
	fragment.albedo.w = uniform_Color.w;
}
#endif

#ifdef FlatVertexColor
uniform vec4 uniform_Color = vec4(1.0);

void getFragment(inout fragment_info fragment) {
	fragment.emission = uniform_Color.xyz * frag_in.color.xyz;
	fragment.albedo.w = uniform_Color.w * frag_in.color.w;
}
#endif

#ifdef FlatTextureColor
uniform vec4 uniform_Color = vec4(1.0);
uniform sampler2D uniform_MainTexture;

void getFragment(inout fragment_info fragment) {
	vec4 texture = texture(uniform_MainTexture, frag_in.texcoord0);
	
	fragment.emission = uniform_Color.xyz * texture.xyz;
	fragment.albedo.w = uniform_Color.w * texture.w;
}
#endif

#ifdef DiffuseColor
uniform vec4 uniform_Color = vec4(1.0);
uniform float uniform_SpecPower = 0.0;
uniform float uniform_SpecIntensity = 0.0;

void getFragment(inout fragment_info fragment) {
	fragment.normal = frag_in.normal;			
	fragment.albedo = uniform_Color;
	fragment.specPower = uniform_SpecPower;
	fragment.specIntensity = uniform_SpecIntensity;
}
#endif

#ifdef DiffuseColorTexture
uniform vec4 uniform_Color = vec4(1.0);
uniform sampler2D uniform_MainTexture;

void getFragment(inout fragment_info fragment) {
	vec4 texture = texture(uniform_MainTexture, frag_in.texcoord0);
	
	fragment.albedo = uniform_Color * texture;
}
#endif

#ifdef DiffuseNormalTextureColor
uniform vec4 uniform_Color = vec4(1.0);
uniform sampler2D uniform_MainTexture;
uniform sampler2D uniform_NormalTexture;

void getFragment(inout fragment_info fragment) {
	vec4 texture = texture(uniform_MainTexture, frag_in.texcoord0);
	
	fragment.normal = UnpackNormal(uniform_NormalTexture, frag_in.texcoord0);	
	fragment.albedo = uniform_Color * texture;
}
#endif

#ifdef ReflectionDiffuseColor
uniform vec4 uniform_Color = vec4(1.0);
uniform float uniform_Reflectivity = 0.6;
uniform samplerCube uniform_ReflectionTexture;

void getFragment(inout fragment_info fragment) {
	vec3 normal = frag_in.normal;
	vec3 reflectedDirection = reflect(normalize(frag_in.view_pos_ws), normalize(frag_in.normal_ws));

	fragment.emission = uniform_Reflectivity * texture(uniform_ReflectionTexture, reflectedDirection).xyz;
	fragment.normal = normal;
	fragment.albedo = uniform_Color;
}
#endif

#ifdef RimDiffuseColorTexture
uniform vec4 uniform_Color;
uniform sampler2D uniform_MainTexture;
uniform vec3 uniform_Emission;
uniform vec3 uniform_Rim  = vec3(0.6, 0.6, 0.8);
uniform float uniform_RimPower = 4.0;

void getFragment(inout fragment_info fragment) {
	vec3 normal = normalize(frag_in.normal);	
	float rim = 1.0 - clamp(dot(-normalize(frag_in.view_pos), normal), 0.0, 1.0);

	fragment.emission = uniform_Rim * pow (rim, uniform_RimPower) + uniform_Emission;
	fragment.normal = normal;
	fragment.albedo = uniform_Color * texture(uniform_MainTexture, frag_in.texcoord0);
}
#endif

#ifdef RimReflectionDiffuseTextureColor
uniform vec4 uniform_Color;
uniform sampler2D uniform_MainTexture;
uniform vec3 uniform_Emission;
uniform vec3 uniform_Rim  = vec3(0.6, 0.6, 0.8);
uniform float uniform_RimPower = 4.0;
uniform float uniform_Reflectivity = 0.6;
uniform samplerCube uniform_ReflectionTexture;

void getFragment(inout fragment_info fragment) {
	vec3 normal = frag_in.normal;	
	float rim = 1.0 - clamp(dot(-normalize(frag_in.view_pos), normal), 0.0, 1.0);	
	vec3 reflectedDirection = reflect(normalize(frag_in.view_pos_ws), normalize(frag_in.normal_ws));

	fragment.emission = uniform_Rim * pow (rim, uniform_RimPower) + uniform_Emission + uniform_Reflectivity * texture(uniform_ReflectionTexture, reflectedDirection).xyz;
	fragment.normal = normal;
	fragment.albedo = uniform_Color * texture(uniform_MainTexture, frag_in.texcoord0);
}
#endif