using UnityEngine;

namespace FaraBombRush.Controllers.Components;

public class FaraStandardShaderController : MonoBehaviour
{
    void Start()
    {
        SetMaterial();
    }

    void SetMaterial()
    {
        var material = this.GetComponent<Renderer>().material;

        /////////////////////////////////////////////////////
        // Main Maps
        /////////////////////////////////////////////////////

        // Albedo
        var texture = material.mainTexture;
        material.mainTexture = texture;
        material.color = Color.red;
        // 以下と同等
        // material.SetTexture("_MainTex", texture);
        // material.SetColor("_Color", Color.red);

        // Metallic
        material.SetTexture("_MetallicGlossMap", texture);
        material.SetFloat("_Metallic", 1.0f);

        // Smoothness
        //   -> Albedo Alpha
        material.SetInt("_SmoothnessTextureChannel", 1);
        material.SetFloat("_GlossMapScale", 0.93f);

        // Smoothness
        //   -> Metallic Alpha
        material.SetInt("_SmoothnessTextureChannel", 0);
        material.SetFloat("_Glossiness", 0.9f);

        // Normal Map
        material.SetTexture("_BumpMap", texture);
        material.SetFloat("_BumpScale", 1.4f);

        // Height Map
        material.SetTexture("_ParallaxMap", texture);
        material.SetFloat("_Parallax", 1.5f);

        // Occlusion
        material.SetTexture("_OcclusionMap", texture);
        material.SetFloat("_OcclusionStrength", 1.6f);

        // Ditail Mask
        material.SetTexture("_DetailMask", texture);

        // Emission
        // インスペクターでは無効に見えるが効いている
        material.EnableKeyword("_EMISSION");
        material.SetColor("_EmissionColor", Color.green);
        material.SetTexture("_EmissionMap", texture);

        // Tiling
        material.mainTextureScale = new Vector2(2, 2);
        // 以下と同等
        // material.SetTextureScale("_MainTex", new Vector2(2, 2));

        // Offset
        material.mainTextureOffset = new Vector2(3, 3);
        // 以下と同等
        // material.SetTextureOffset("_MainTex", new Vector2(3, 3));

        /////////////////////////////////////////////////////
        // Secondary Maps
        /////////////////////////////////////////////////////

        // Detail Albedo x
        material.SetTexture("_DetailAlbedoMap", texture);

        // Normal Map
        material.SetTexture("_DetailNormalMap", texture);
        material.SetFloat("_DetailNormalMapScale", 1.2f);

        // Tiling
        material.SetTextureScale("_DetailAlbedoMap", new Vector2(4, 4));

        // Offset
        material.SetTextureOffset("_DetailAlbedoMap", new Vector2(5, 5));

        // UVSet
        material.SetFloat("_UVSec", 0); // 0:UV0, 1:UV1

        /////////////////////////////////////////////////////
        // Forward Rendering Options
        /////////////////////////////////////////////////////

        // Specular Highlights
        material.SetFloat("_SpecularHighlights", 0); // 0:OFF, 1:ON

        // Reflections
        material.SetFloat("_GlossyReflections", 0); // 0:OFF, 1:ON

        /////////////////////////////////////////////////////
        // Advanced Options
        /////////////////////////////////////////////////////

        //   Enable GPUI Instancing
        material.enableInstancing = true;

        //   Double Sided Global
        material.doubleSidedGI = true;
    }
}