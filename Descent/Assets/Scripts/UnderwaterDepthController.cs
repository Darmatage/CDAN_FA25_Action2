using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class UnderwaterDepthController : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Volume postProcessVolume;
    public Light directionalLight;

    [Header("Water Placement Settings")]
    //public float waterSurfaceHeight = 0f;
    //public float maxDepth = 100f;
    public Transform surfaceMarker;
    public Transform bottomMarker;

    [Header("Surface Water Settings")]
    public Color surfaceFogColor = new Color(0.3f, 0.8f, 0.9f);
    public float surfaceFogDensity = 0.02f;
    public float surfaceLightIntensity = 1f;
    public float surfaceAmbientIntensity = 1f;
    //public float surfaceLightShaftIntensity = 1.5f;

    [Header("Deep Water Settings")]
    public Color deepFogColor = new Color(0.05f, 0.1f, 0.2f);
    public float deepFogDensity = 0.15f;
    public float deepLightIntensity = 0.2f;
    public float deepAmbientIntensity = 0.3f;
    //public float deepLightShaftIntensity = 0.1f;



    [Header("Color Grading")]
    public float surfaceSaturation = 0f;
    public float deepSaturation = -30f;
    public Color surfaceTint = Color.white;
    public Color deepTint = new Color(0.6f, 0.7f, 1f);

    [Header("Height Fog Gradient")]
    public bool enableHeightFog = true;
    public float heightFogRange = 40f;
    public Color topHeightFogColor = new Color(0.4f, 0.75f, 0.95f);
    public Color bottomHeightFogColor = new Color(0.08f, 0.15f, 0.3f);


    private float initialLightIntensity;
    private float initialAmbientIntensity;
    private ColorAdjustments colorAdjustments;

    [System.Obsolete]
    void Start()
    {

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }

        if (directionalLight == null)
        {
            directionalLight = RenderSettings.sun;
        }

        if (directionalLight != null)
        {
            initialLightIntensity = directionalLight.intensity;
        }


        initialAmbientIntensity = RenderSettings.ambientIntensity;

        if (postProcessVolume != null && postProcessVolume.profile.TryGet(out colorAdjustments))
        {
            colorAdjustments.active = true;
        }

        if (surfaceMarker == null)
        {
            Debug.LogError("UnderwaterDepthController: SurfaceMarker not assigned!");
        }
        if (bottomMarker == null)
        {
            Debug.LogError("UnderwaterDepthController: bottom Marker not assigned!");
        }

        UnderwaterLightParticleShafts particleShafts = FindObjectOfType<UnderwaterLightParticleShafts>();
        if (particleShafts != null)
        {
            particleShafts.surfaceMarker = surfaceMarker;
            particleShafts.bottomMarker = bottomMarker;
            Debug.Log("UnderwaterDepthController: Connected to particle shafts");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) return;

        //float depth = waterSurfaceHeight - player.position.y;
        //float normalizedDepth = Mathf.Clamp01(depth / maxDepth);
        float surfaceHeight = surfaceMarker.position.y;
        float bottomHeight = bottomMarker.position.y;
        float totalDepth = surfaceHeight - bottomHeight;

        if ( totalDepth <=0)
        {
            Debug.LogWarning("UnderwaterDepthController: surface marker mustr be above bottom marker!");
            return;
        }

        float currentDepth = surfaceHeight - player.position.y;
        float normalizedDepth = Mathf.Clamp01(currentDepth / totalDepth);


        UpdateFog(normalizedDepth);
        UpdateLighting(normalizedDepth);
        UpdateColorGrading(normalizedDepth);
        if (enableHeightFog)
        {
            UpdateHeightFog();
        }
        //UpdateLightShafts(normalizedDepth);

        Shader.SetGlobalFloat("_WaterDepth", normalizedDepth);
    }

    void UpdateFog(float depth)
    {
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Exponential;
        RenderSettings.fogColor = Color.Lerp(surfaceFogColor, deepFogColor, depth);
        RenderSettings.fogDensity = Mathf.Lerp(surfaceFogDensity, deepFogDensity, depth);
    }

    void UpdateLighting(float depth)
    {
        if (directionalLight != null)
        {
            directionalLight.intensity = Mathf.Lerp(initialLightIntensity * surfaceLightIntensity, initialLightIntensity * deepLightIntensity, depth);

        }

        RenderSettings.ambientIntensity = Mathf.Lerp(initialAmbientIntensity * surfaceAmbientIntensity, initialAmbientIntensity * deepAmbientIntensity, depth);

    }

    void UpdateColorGrading (float depth)
    {
        if (colorAdjustments != null)
        {
            colorAdjustments.saturation.value = Mathf.Lerp(surfaceSaturation, deepSaturation, depth);
            colorAdjustments.colorFilter.value = Color.Lerp(surfaceTint, deepTint, depth);
        }
    }

    void UpdateHeightFog()
    {
        if (player == null || bottomMarker == null) return;

        float bottomY = bottomMarker.position.y;
        float topY = bottomY + heightFogRange;

        Shader.SetGlobalFloat("_HeightFogBottom", bottomY);
        Shader.SetGlobalFloat("_HeightFogTop", topY);
        Shader.SetGlobalColor("_HeightFogColorTop", topHeightFogColor);
        Shader.SetGlobalColor("_HeightFogColorBottom", bottomHeightFogColor);
    }
    //void UpdateLightShafts(float depth)
    //{
        //float intensity = Mathf.Lerp(surfaceLightShaftIntensity, deepLightShaftIntensity, depth);
        //Shader.SetGlobalFloat("_LightShaftIntensity", intensity);
    //}
}
