using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName ="Rendering/自定义渲染管线")]
public class CustomRenderPipelineAsset : RenderPipelineAsset
{
    [SerializeField]
    bool useSRPBatcher,dynamicBatching,instancing;


    protected override RenderPipeline CreatePipeline()
    {
        return new CustomRenderPipeline(useSRPBatcher,dynamicBatching, instancing);
    }
    
}
