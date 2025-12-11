Shader "Custom/VerticalFogGradient"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        
        Pass
        {
            Name "VerticalFog"
            ZWrite Off
            ZTest Always
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            
            struct Attributes
            {
                uint vertexID : SV_VertexID;
            };
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 texcoord : TEXCOORD0;
            };
            
            TEXTURE2D(_BlitTexture);
            SAMPLER(sampler_BlitTexture);
            float4 _BlitTexture_TexelSize;
            
            float4 _TopColor;
            float4 _BottomColor;
            float _GradientHeight;
            float _GradientPower;
            float _FogStrength;
            
            Varyings Vert(Attributes input)
            {
                Varyings output;
                float4 pos = GetFullScreenTriangleVertexPosition(input.vertexID);
                float2 uv = GetFullScreenTriangleTexCoord(input.vertexID);
                
                output.positionCS = pos;
                output.texcoord = uv;
                return output;
            }
            
            float4 frag(Varyings input) : SV_Target
            {
                float4 color = SAMPLE_TEXTURE2D(_BlitTexture, sampler_BlitTexture, input.texcoord);
                
                // Sample depth
                float depth = SampleSceneDepth(input.texcoord);
                
                // Reconstruct world position from depth
                float3 worldPos = ComputeWorldSpacePosition(input.texcoord, depth, UNITY_MATRIX_I_VP);
                
                // Calculate vertical gradient based on world Y position
                float heightFactor = saturate((worldPos.y + _GradientHeight * 0.5) / _GradientHeight);
                heightFactor = pow(heightFactor, _GradientPower);
                
                // Blend between bottom and top colors
                float4 fogColor = lerp(_BottomColor, _TopColor, heightFactor);
                
                // Apply fog overlay
                return lerp(color, fogColor, _FogStrength);
            }
            ENDHLSL
        }
    }
}