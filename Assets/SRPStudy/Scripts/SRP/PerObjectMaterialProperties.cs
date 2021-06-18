using System.Collections;
using UnityEngine;


[RequireComponent(typeof(MeshRenderer))]
[DisallowMultipleComponent]
public class PerObjectMaterialProperties : MonoBehaviour
{
    [SerializeField]
    Color color = Color.white;

    [SerializeField, Range(0f, 1f)]
    float cutoff = 0.5f, metallic = 0f, smoothness = 0.5f;

    static MaterialPropertyBlock MPB;
    static int colorID = Shader.PropertyToID("_Color");
    static int cutoffId = Shader.PropertyToID("_Cutoff");
    static int metallicId = Shader.PropertyToID("_Metallic");
    static int smoothnessId = Shader.PropertyToID("_Smoothness");


    private void Awake()
    {
        OnValidate();
    }

    private void OnValidate()
    {
        if (MPB == null) {
            MPB = new MaterialPropertyBlock();
        }

        MPB.SetColor(colorID, color);
        MPB.SetFloat(cutoffId, cutoff);
        MPB.SetFloat(metallicId, metallic);
        MPB.SetFloat(smoothnessId, smoothness);

        GetComponent<MeshRenderer>().SetPropertyBlock(MPB);
    }
}