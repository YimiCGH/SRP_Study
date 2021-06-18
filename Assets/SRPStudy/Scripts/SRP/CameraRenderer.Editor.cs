using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Profiling;
using Conditional = System.Diagnostics.ConditionalAttribute;

#if UNITY_EDITOR
using UnityEditor;

#endif

public partial class CameraRenderer
{
   
    const bool OverrideErrorMatrial = true;

    static ShaderTagId[] legacyShaderTagIds = { 
        new ShaderTagId("Always"),
        new ShaderTagId("ForwardBase"),
        new ShaderTagId("PrepassBase"),
        new ShaderTagId("Vertex"),
        new ShaderTagId("VertexLMRGBM"),
        new ShaderTagId("VertexLM"),
    };

#if UNITY_EDITOR
    string SampleName { get; set; }
#else
    const string SampleName => bufferName;
#endif

    Material errorMaterial;

    [Conditional("UNITY_EDITOR"),Conditional("DEVELOPMENT_BUILD")]
    void DrawUnsupportedShaders() {

        if (errorMaterial == null) {
            Shader errorShader = Shader.Find("Hidden/InternalErrorShader");
            errorMaterial = new Material(errorShader) { hideFlags = HideFlags.HideAndDontSave};
        }

        var drawSetting = new DrawingSettings(legacyShaderTagIds[0],new SortingSettings(_camera));
        for (int i = 1; i < legacyShaderTagIds.Length; i++){
            drawSetting.SetShaderPassName(i, legacyShaderTagIds[i]);
        }

        if (OverrideErrorMatrial)
            drawSetting.overrideMaterial = errorMaterial;

        var filteringSetting = FilteringSettings.defaultValue;

        _context.DrawRenderers(_cullingResults,ref drawSetting,ref filteringSetting);
    }
    [Conditional("UNITY_EDITOR")]
    void DrawGizmos() {
        if (Handles.ShouldRenderGizmos()) {
            _context.DrawGizmos(_camera,GizmoSubset.PreImageEffects);
            _context.DrawGizmos(_camera, GizmoSubset.PostImageEffects);
        }
    }

    [Conditional("UNITY_EDITOR")]
    void PrepareForSceneWindow() {
        if (_camera.cameraType == CameraType.SceneView)
        {
            ScriptableRenderContext.EmitWorldGeometryForSceneView(_camera);//在 Scene 窗口绘制 UI
        }
    }

    [Conditional("UNITY_EDITOR")]
    void PrepareBuffer()
    {
        Profiler.BeginSample("Editor Only");
        buffer.name = SampleName = _camera.name;
        Profiler.EndSample();
    }
}