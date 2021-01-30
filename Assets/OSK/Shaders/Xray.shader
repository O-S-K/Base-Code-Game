Shader "Custom/X-ray"
{
    Properties
	{
		_Color("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_OccluColor("Occlu Color", Color) = (1,1,1,1)
		 [Range] _LightColorMain("_Lingting", Range(0.0, 2.0)) = 0.0
		 
        _Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5

        _Glossiness("Smoothness", Range(0.0, 1.0)) = 0.5
        _GlossMapScale("Smoothness Scale", Range(0.0, 1.0)) = 1.0
        [Enum(Metallic Alpha,0,Albedo Alpha,1)] _SmoothnessTextureChannel ("Smoothness texture channel", Float) = 0

        [Gamma] _Metallic("Metallic", Range(0.0, 1.0)) = 0.0
        _MetallicGlossMap("Metallic", 2D) = "white" {}

        _EmissionColor("Color", Color) = (0,0,0)
        _EmissionMap("Emission", 2D) = "white" {}

        [Enum(UV0,0,UV1,1)] _UVSec ("UV Set for secondary textures", Float) = 0


        // Blending state
        [HideInInspector] _Mode ("__mode", Float) = 0.0
        [HideInInspector] _SrcBlend ("__src", Float) = 1.0
        [HideInInspector] _DstBlend ("__dst", Float) = 0.0
        [HideInInspector] _ZWrite ("__zw", Float) = 1.0
	}

	CGINCLUDE
        #define UNITY_SETUP_BRDF_INPUT MetallicSetup
    ENDCG

	SubShader
	{
		Tags { "Queue" = "Geometry+100" "RenderType" = "Opaque" "PerformanceChecks"="False"}
		LOD 300
 
		Pass
		{
			Blend One OneMinusSrcAlpha
			ZWrite Off
			ZTest Greater
 
			CGPROGRAM
			#include "Lighting.cginc"  
			fixed4 _OccluColor;
			uniform float _OutlineWidth;
 
			struct v2f
			{
				float4 pos : SV_POSITION;
				float3 normal : normal;
				float3 viewDir : TEXCOORD0;
			};
 
			v2f vert(appdata_base v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.viewDir = ObjSpaceViewDir(v.vertex);
				o.normal = v.normal;
				return o;
			}
 
			fixed4 frag(v2f i) : SV_Target
			{
				float3 normal = normalize(i.normal);
				float3 viewDir = normalize(i.viewDir);
				float rim = 1 - dot(normal, viewDir);
				//return fixed4(_OccluColor.rgb * rim, rim * 0.5);
				return fixed4(_OccluColor.rgb, 1);
			}
			#pragma vertex vert  
			#pragma fragment frag  
			ENDCG
		} 
			
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
 
 
			sampler2D _MainTex;
			float4 _MainTex_ST;
			uniform float4 _Color;
			uniform float4 _LightColor0;
			uniform float _LightColorMain;

			struct v2f 
			{
				float4 pos:SV_POSITION;
				float3 lightDir:TEXCOORD0;
				float3 viewDir:TEXCOORD1;
				float3 normal:TEXCOORD2;
				float2 texcoord:TEXCOORD3;
			};
 
			v2f vert(appdata_full v) 
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.normal = v.normal;
				o.lightDir = ObjSpaceLightDir(v.vertex);
				o.viewDir = ObjSpaceViewDir(v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}
 
			float4 frag(v2f i) :COLOR
			{
				float4 c = 1;
				float3 N = normalize(i.normal);
				c = _Color * _LightColor0 * _LightColorMain;
				c *= tex2D(_MainTex, i.texcoord);
				return c;
			}  
			ENDCG
		}
 Pass
        {
            Name "FORWARD"
            Tags { "LightMode" = "ForwardBase" }

            Blend [_SrcBlend] [_DstBlend]
            ZWrite [_ZWrite]

            CGPROGRAM
            #pragma target 3.0

            // -------------------------------------

            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
            #pragma shader_feature _EMISSION
            #pragma shader_feature_local _METALLICGLOSSMAP
            #pragma shader_feature_local _DETAIL_MULX2
            #pragma shader_feature_local _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature_local _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature_local _GLOSSYREFLECTIONS_OFF
            #pragma shader_feature_local _PARALLAXMAP

            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            // Uncomment the following line to enable dithering LOD crossfade. Note: there are more in the file to uncomment for other passes.
            //#pragma multi_compile _ LOD_FADE_CROSSFADE

            #pragma vertex vertBase
            #pragma fragment fragBase
            #include "UnityStandardCoreForward.cginc"

            ENDCG
        }
		UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
	}
}