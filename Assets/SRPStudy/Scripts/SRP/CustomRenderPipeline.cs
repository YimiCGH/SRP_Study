using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class CustomRenderPipeline : RenderPipeline
{
    CameraRenderer cameraRenderer = new CameraRenderer();

    public bool UseSRPBatcher;
    public bool DynamicBatching;
    public bool Instancing;

    public CustomRenderPipeline(bool useSRPBatcher,bool dynamicBatching,bool instancing) {
        UseSRPBatcher = useSRPBatcher;
        DynamicBatching = dynamicBatching;
        Instancing = instancing;

        GraphicsSettings.useScriptableRenderPipelineBatching = UseSRPBatcher;
        GraphicsSettings.lightsUseLinearIntensity = true;

    }

    protected override void Render(ScriptableRenderContext context, Camera[] cameras)
    {
        foreach (var camera in cameras)
        {
            cameraRenderer.Render(this,context,camera);
        }
    }
}