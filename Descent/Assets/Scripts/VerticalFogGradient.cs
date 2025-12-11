using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;

public class VerticalFogGradient : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        public Material fogMaterial;

        [Header("Colors")]
        public Color topFogColor = new Color(0.5f, 0.85f, 1f, 1f);
        public Color bottomFogColor = new Color(0.1f, 0.25f, 0.45f, 1f);

        [Header("Gradient Settings")]
        public float gradientHeight = 40f;
        public float gradientPower = 1.5f;

        [Header("Strength")]
        [Range(0f, 1f)]
        public float fogStrength = 0.4f;
    }

    public Settings settings = new Settings();
    private VerticalFogPass pass;

    public override void Create()
    {
        pass = new VerticalFogPass(settings);
        pass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (settings.fogMaterial == null)
        {
            Debug.LogWarning("Vertical Fog Material not assigned!");
            return;
        }

        pass.ConfigureInput(ScriptableRenderPassInput.Color | ScriptableRenderPassInput.Depth);
        pass.Setup(settings);
        renderer.EnqueuePass(pass);
    }

    protected override void Dispose(bool disposing)
    {
        pass?.Dispose();
    }

    class VerticalFogPass : ScriptableRenderPass
    {
        private Settings settings;
        private ProfilingSampler profilingSampler;

        public VerticalFogPass(Settings settings)
        {
            this.settings = settings;
            profilingSampler = new ProfilingSampler("VerticalFogGradient");
        }

        public void Setup(Settings s)
        {
            settings = s;
        }

        private class PassData
        {
            internal Material material;
            internal Settings settings;
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            if (settings.fogMaterial == null) return;

            // Only render during Play mode
            #if UNITY_EDITOR
            if (!Application.isPlaying) return;
            #endif

            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
            UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();

            TextureHandle source = resourceData.activeColorTexture;
            if (!source.IsValid()) return;

            // Read from global shader properties set by UnderwaterDepthController
            Color topColor = Shader.GetGlobalColor("_HeightFogTopColor");
            Color bottomColor = Shader.GetGlobalColor("_HeightFogBottomColor");
            float gradientHeight = Shader.GetGlobalFloat("_HeightFogGradientHeight");
            float gradientPower = Shader.GetGlobalFloat("_HeightFogGradientPower");
            float fogStrength = Shader.GetGlobalFloat("_HeightFogStrength");

            // Fallback to settings if globals not set
            if (topColor == Color.clear) topColor = settings.topFogColor;
            if (bottomColor == Color.clear) bottomColor = settings.bottomFogColor;
            if (gradientHeight == 0) gradientHeight = settings.gradientHeight;
            if (gradientPower == 0) gradientPower = settings.gradientPower;
            if (fogStrength == 0) fogStrength = settings.fogStrength;

            settings.fogMaterial.SetColor("_TopColor", topColor);
            settings.fogMaterial.SetColor("_BottomColor", bottomColor);
            settings.fogMaterial.SetFloat("_GradientHeight", gradientHeight);
            settings.fogMaterial.SetFloat("_GradientPower", gradientPower);
            settings.fogMaterial.SetFloat("_FogStrength", fogStrength);

            // Update material properties
            //settings.fogMaterial.SetColor("_TopColor", settings.topFogColor);
            //settings.fogMaterial.SetColor("_BottomColor", settings.bottomFogColor);

            var desc = cameraData.cameraTargetDescriptor;
            desc.depthBufferBits = 0;
            desc.msaaSamples = 1;

            TextureHandle tempTexture = UniversalRenderer.CreateRenderGraphTexture(renderGraph, desc, "_TempVerticalFog", false);

            // Apply fog effect
            using (var builder = renderGraph.AddRasterRenderPass<PassData>("Vertical Fog Gradient", out var passData, profilingSampler))
            {
                passData.material = settings.fogMaterial;
                passData.settings = settings;

                builder.UseTexture(source, AccessFlags.Read);
                builder.SetRenderAttachment(tempTexture, 0, AccessFlags.Write);

                builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
                {
                    Blitter.BlitTexture(context.cmd, source, new Vector4(1, 1, 0, 0), data.material, 0);
                });
            }

            // Copy back
            using (var builder = renderGraph.AddRasterRenderPass<PassData>("Vertical Fog Copy", out var passData, profilingSampler))
            {
                passData.material = null;

                builder.UseTexture(tempTexture, AccessFlags.Read);
                builder.SetRenderAttachment(source, 0, AccessFlags.Write);

                Material blitMat = Blitter.GetBlitMaterial(UnityEngine.Rendering.TextureDimension.Tex2D);

                builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
                {
                    Blitter.BlitTexture(context.cmd, tempTexture, new Vector4(1, 1, 0, 0), blitMat, 0);
                });
            }
        }

        public void Dispose()
        {
            // clean?
        }
    }
}