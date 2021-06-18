using System.Collections;

using UnityEngine;
using UnityEngine.Rendering;
using Conditional = System.Diagnostics.ConditionalAttribute;

#if UNITY_EDITOR
using UnityEditor;
#endif

public partial class CameraRenderer
{
    static ShaderTagId 
        unlitShaderTid = new ShaderTagId("SRPDefaultUnlit"),
        litShaderTid = new ShaderTagId("CustomLit");
    const string bufferName = "Render Camera";

    ScriptableRenderContext _context;
    Camera _camera;
    CustomRenderPipeline _pipeline;
    

    CommandBuffer buffer = new CommandBuffer { name = bufferName};
    CullingResults _cullingResults;

    Lighting lighting = new Lighting();

    /// <summary>
    /// 负责绘制相机看到的所有几何图形。
    /// 可以自定义相机使用什么方式来渲染，前向渲染还是延迟渲染
    /// </summary>
    /// <param name="context"></param>
    /// <param name="camera"></param>
    public void Render(CustomRenderPipeline pipeline, ScriptableRenderContext context,Camera camera) {
        _pipeline = pipeline;
        _context = context;
        _camera = camera;

        PrepareBuffer();//多相机支持
        PrepareForSceneWindow();//Scene 场景绘制UI
        if (!Cull()) {
            return;
        }

        //buffer = new CommandBuffer { name = bufferName };
        Setup();
        lighting.Setup(context, _cullingResults);//设置光照信息

        DrawVisibleGeometry(_pipeline.DynamicBatching,_pipeline.Instancing);
        DrawUnsupportedShaders();
        DrawGizmos();

        Submit();//前面的所有绘制命令都是缓冲的，最后通过调用Submit来提交这些命令，才会安排到执行队列        
    }

    void Setup() 
    {
        _context.SetupCameraProperties(_camera);//设置相机的位置和方向（视图矩阵），以及是透视还是正交投影（投影矩阵），即 MVP 变换
        
        CameraClearFlags clearFlags = _camera.clearFlags;
  
        buffer.ClearRenderTarget(
            clearFlags <= CameraClearFlags.Depth,
            clearFlags == CameraClearFlags.Color,
            clearFlags == CameraClearFlags.Color ? _camera.backgroundColor.linear : Color.clear);
        buffer.BeginSample(SampleName);//注入到 Profiler 和 FrameDebugger 中以便查看
        ExecuteBuffer();
    }

    

    protected virtual void DrawVisibleGeometry(bool useDynamicBatching,bool useGPUInstancing) {
        //绘制几何体
        var sortingSettings = new SortingSettings(_camera);
        var drawingSetting = new DrawingSettings(unlitShaderTid, sortingSettings)
        {
            enableDynamicBatching = useDynamicBatching,
            enableInstancing = useGPUInstancing
        };
        drawingSetting.SetShaderPassName(1, litShaderTid);
        var filteringSetting = new FilteringSettings(RenderQueueRange.opaque);//指出哪些队列是允许的
                

        //先绘制不透明物体
        //filteringSetting.renderQueueRange = RenderQueueRange.opaque;
        sortingSettings.criteria = SortingCriteria.CommonOpaque;
        drawingSetting.sortingSettings = sortingSettings;
        _context.DrawRenderers(_cullingResults,ref drawingSetting,ref filteringSetting);


        //绘制天空盒
        _context.DrawSkybox(_camera);

        //绘制透明物体
        sortingSettings.criteria = SortingCriteria.CommonTransparent;
        drawingSetting.sortingSettings = sortingSettings;
        filteringSetting.renderQueueRange = RenderQueueRange.transparent;        
        _context.DrawRenderers(_cullingResults, ref drawingSetting, ref filteringSetting);
        
    }

    void Submit() {
        buffer.EndSample(SampleName);
        ExecuteBuffer();//在FrameDebugger 记录结束后再执行缓冲区，否则，如果先执行缓冲区再记录，会导致缓冲区执行完后被清理了，FrameDebugger记录不到任何渲染步骤
        _context.Submit();        
    }


    void ExecuteBuffer() {
        _context.ExecuteCommandBuffer(buffer);//执行缓冲区，会从缓冲区复制命令，但是不会清除缓冲区，所以需要手动清理
        buffer.Clear();//清理缓冲区
    }

    bool Cull() {        
        if (_camera.TryGetCullingParameters(out var cullingParameters)){
            
            _cullingResults = _context.Cull(ref cullingParameters);
            return true;
        }
        return false;
    }
}