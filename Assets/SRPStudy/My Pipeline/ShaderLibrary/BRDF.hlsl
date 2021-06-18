#ifndef CUSTOM_BRDF_INCLUDED
#define CUSTOM_BRDF_INCLUDED

struct BRDF {
	float3 diffuse;
	float3 specular;
	float roughness;
};

//绝缘体（非金属材质）也会有一点点的反射率
#define MIN_REFLECTIVITY 0.04
float OneMinusReflectivity (float metallic) {
	
	/*
	理论上，金属反射所有的光线，漫反射为0，即diffuse = 1 - metallic = 0;
	但是，实际中不是绝对的完美镜面，所以，把金属反射率从范围0-1 调整到 0-0.96
	*/

	float range = 1.0 - MIN_REFLECTIVITY;
	return range - metallic * range;
}
float Square (float v) {
	return v * v;
}

float SpecularStrength (Surface surface, BRDF brdf, Light light) {
	float3 h = SafeNormalize(light.direction + surface.viewDirection);
	float nh2 = Square(saturate(dot(surface.normal, h)));
	float lh2 = Square(saturate(dot(light.direction, h)));
	float r2 = Square(brdf.roughness);
	float d2 = Square(nh2 * (r2 - 1.0) + 1.00001);
	float normalization = brdf.roughness * 4.0 + 2.0;
	return r2 / (d2 * max(0.1, lh2) * normalization);
}

BRDF GetBRDF (Surface surface, bool applyAlphaToDiffuse = false) {
	BRDF brdf;

	float oneMinusReflectivity = OneMinusReflectivity( surface.metallic);

	brdf.diffuse = surface.color * oneMinusReflectivity ;
	if(applyAlphaToDiffuse){
		brdf.diffuse *= surface.alpha;
	}
	brdf.specular = lerp(MIN_REFLECTIVITY, surface.color, surface.metallic);
	
	float perceptualRoughness =
		PerceptualSmoothnessToPerceptualRoughness(surface.smoothness);
	brdf.roughness = PerceptualRoughnessToRoughness(perceptualRoughness);

	return brdf;
}

float3 DirectBRDF (Surface surface, BRDF brdf, Light light) {
	return SpecularStrength(surface, brdf, light) * brdf.specular + brdf.diffuse;
}
#endif