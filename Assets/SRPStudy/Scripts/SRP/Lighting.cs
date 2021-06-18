using UnityEngine;
using UnityEngine.Rendering;
using Unity.Collections;

public class Lighting
{
    const int MaxDirLightCount = 4;
    const string bufferName = "Lighting";
    static int
        dirLightCountId = Shader.PropertyToID("_DirectionalLightCount"),
        //dirLightColorId = Shader.PropertyToID("_DirectionalLightColor"),
        //dirLightDirectionId = Shader.PropertyToID("_DirectionalLightDirection"),        
		dirLightColorsId = Shader.PropertyToID("_DirectionalLightColors"),
		dirLightDirectionsId = Shader.PropertyToID("_DirectionalLightDirections");

    static Vector4[]
    dirLightColors = new Vector4[MaxDirLightCount],
    dirLightDirections = new Vector4[MaxDirLightCount];

    CommandBuffer buffer = new CommandBuffer { name = bufferName};
    CullingResults _cullingResults;

    public void Setup(ScriptableRenderContext context, CullingResults cullingResults) {
        _cullingResults = cullingResults;
        buffer.BeginSample(bufferName);
        SetupLights();
        buffer.EndSample(bufferName);
        context.ExecuteCommandBuffer(buffer);
        buffer.Clear();
    }
    void SetupLights() {

        NativeArray<VisibleLight> visibleLights = _cullingResults.visibleLights;

        int dirLightCount = 0;
        for (int i = 0; i < visibleLights.Length; i++)
        {
            var light = visibleLights[i];
            if (light.lightType == LightType.Directional) {
                SetupDirectionalLight(dirLightCount++,ref light);
                if (dirLightCount >= MaxDirLightCount){
                    break;
                }
            }            
        }

        buffer.SetGlobalInt(dirLightCountId, visibleLights.Length);
        buffer.SetGlobalVectorArray(dirLightColorsId, dirLightColors);
        buffer.SetGlobalVectorArray(dirLightDirectionsId, dirLightDirections);

    }

    void SetupDirectionalLight(int index,ref VisibleLight visibleLight) {
        dirLightColors[index] = visibleLight.finalColor;
        dirLightDirections[index] = -visibleLight.localToWorldMatrix.GetColumn(2);        
    }
}