// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Sprites/Layered"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        [PerRendererData] _MainTex2("Sprite Texture", 2D) = "white" {}
        [PerRendererData] _MainTex3("Sprite Texture", 2D) = "white" {}
        [PerRendererData] _MainTex4("Sprite Texture", 2D) = "white" {}
        [PerRendererData] _MainTex5("Sprite Texture", 2D) = "white" {}
        [PerRendererData] _MainTex6("Sprite Texture", 2D) = "white" {}
        [PerRendererData] _MainTex7("Sprite Texture", 2D) = "white" {}
        [PerRendererData] _MainTex8("Sprite Texture", 2D) = "white" {}
        [PerRendererData] _MainTex9("Sprite Texture", 2D) = "white" {}
        [PerRendererData] _MainTex10("Sprite Texture", 2D) = "white" {}
        _OffsetUV("UVOffset", Vector) = (0,0,0,0)
        _Color ("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
        CGPROGRAM
// Upgrade NOTE: excluded shader from DX11, OpenGL ES 2.0 because it uses unsized arrays
#pragma exclude_renderers d3d11 gles
            #pragma vertex SpriteVert
            #pragma fragment SpriteFrag
            #pragma target 2.0
            #pragma multi_compile_instancing
            #pragma multi_compile_local _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA

        // Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

    #ifndef UNITY_SPRITES_INCLUDED
    #define UNITY_SPRITES_INCLUDED

    #include "UnityCG.cginc"

    #ifdef UNITY_INSTANCING_ENABLED

        UNITY_INSTANCING_BUFFER_START(PerDrawSprite)
                // SpriteRenderer.Color while Non-Batched/Instanced.
                UNITY_DEFINE_INSTANCED_PROP(fixed4, unity_SpriteRendererColorArray)
                // this could be smaller but that's how bit each entry is regardless of type
                UNITY_DEFINE_INSTANCED_PROP(fixed2, unity_SpriteFlipArray)
            UNITY_INSTANCING_BUFFER_END(PerDrawSprite)

            #define _RendererColor  UNITY_ACCESS_INSTANCED_PROP(PerDrawSprite, unity_SpriteRendererColorArray)
            #define _Flip           UNITY_ACCESS_INSTANCED_PROP(PerDrawSprite, unity_SpriteFlipArray)

        #endif // instancing

        CBUFFER_START(UnityPerDrawSprite)
        #ifndef UNITY_INSTANCING_ENABLED
            fixed4 _RendererColor;
            fixed2 _Flip;
            //The offset of the main sprite placed in the sprite renderer
            float2 _MainOffset;
            float2 _UVOffsets[9];
        #endif
            float _EnableExternalAlpha;
            float2 _OffsetUV;
        CBUFFER_END

            // Material Color.
            fixed4 _Color;

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            inline float4 UnityFlipSprite(in float3 pos, in fixed2 flip)
            {
                return float4(pos.xy * flip, pos.z, 1.0);
            }

            v2f SpriteVert(appdata_t IN)
            {
                v2f OUT;

                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                OUT.vertex = UnityFlipSprite(IN.vertex, _Flip);
                OUT.vertex = UnityObjectToClipPos(OUT.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color * _RendererColor;

                #ifdef PIXELSNAP_ON
                OUT.vertex = UnityPixelSnap(OUT.vertex);
                #endif

                return OUT;
            }

            sampler2D _MainTex;
            sampler2D _AlphaTex;
            sampler2D _MainTex2;
            sampler2D _MainTex3;
            sampler2D _MainTex4;
            sampler2D _MainTex5;
            sampler2D _MainTex6;
            sampler2D _MainTex7;
            sampler2D _MainTex8;
            sampler2D _MainTex9;
            sampler2D _MainTex10;

            fixed4 MergeColors(fixed4 color1, fixed4 color2)
            {
                color1.rgb = (color1.rgb * (1.0f - color2.a)) + (color2.rgb * color2.a);
                color1.a = min(color1.a + color2.a, 1.0f);
                return color1;
            }

            fixed4 SampleSpriteTexture(float2 uv)
            {
                uv = uv + _MainOffset;
                fixed4 color = MergeColors(tex2D(_MainTex, uv), tex2D(_MainTex2, uv));
                color = MergeColors(color, tex2D(_MainTex2, uv));
                color = MergeColors(color, tex2D(_MainTex3, uv));
                color = MergeColors(color, tex2D(_MainTex4, uv));
                color = MergeColors(color, tex2D(_MainTex5, uv));
                color = MergeColors(color, tex2D(_MainTex6, uv));
                color = MergeColors(color, tex2D(_MainTex7, uv));
                color = MergeColors(color, tex2D(_MainTex8, uv));
                color = MergeColors(color, tex2D(_MainTex9, uv));
                color = MergeColors(color, tex2D(_MainTex10, uv));

            /*
            #if ETC1_EXTERNAL_ALPHA
                fixed4 alpha = tex2D(_AlphaTex, uv);
                color.a = lerp(color.a, alpha.r, _EnableExternalAlpha);
            #endif
            */

                return color;
            }

            fixed4 SpriteFrag(v2f IN) : SV_Target
            {
                fixed4 c = SampleSpriteTexture(IN.texcoord) * IN.color;
                c.rgb *= c.a;
                return c;
            }

            #endif // UNITY_SPRITES_INCLUDED

        ENDCG
        }
    }
}
