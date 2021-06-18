#ifndef CUSTOM_LIGHTING_INCLUDED
#define CUSTOM_LIGHTING_INCLUDED

float3 IncomingLight (Surface surface, Light light) {
	return saturate(dot(surface.normal, light.direction)) * light.color;
}
float3 GetLighting (Surface surface ,BRDF brdf, Light light) {
	return IncomingLight(surface, light) * DirectBRDF(surface, brdf, light);
}
float3 GetLighting (Surface surface ,BRDF brdf) {

	float3 col = 0.0;
	int count = GetDirectionalLightCount ();
	for	(int i = 0;i < count;i++){
		col += GetLighting(surface,brdf, GetDirectionalLight(i));
	}
	return col;
}


#endif