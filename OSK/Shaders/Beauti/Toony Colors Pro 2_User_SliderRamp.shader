Shader "Toony Colors Pro 2/User/SliderRamp"
{
  Properties
  {
    [TCP2HeaderHelp(BASE, Base Properties)] _Color ("Color", Color) = (1,1,1,1)
    _HColor ("Highlight Color", Color) = (0.785,0.785,0.785,1)
    _SColor ("Shadow Color", Color) = (0.195,0.195,0.195,1)
    _MainTex ("Main Texture", 2D) = "white" {}
    [TCP2Separator] [TCP2Header(RAMP SETTINGS)] _RampThreshold ("Ramp Threshold", Range(0, 1)) = 0.5
    _RampSmooth ("Ramp Smoothing", Range(0.001, 1)) = 0.1
    [TCP2Separator] [HideInInspector] __dummy__ ("unused", float) = 0
  }
  SubShader
  {
    Tags
    { 
      "RenderType" = "Opaque"
    }
    Pass // ind: 1, name: FORWARD
    {
      Name "FORWARD"
      Tags
      { 
        "LIGHTMODE" = "FORWARDBASE"
        "RenderType" = "Opaque"
        "SHADOWSUPPORT" = "true"
      }
      // m_ProgramMask = 6
      CGPROGRAM
      #pragma multi_compile DIRECTIONAL
      //#pragma target 4.0
      
      #pragma vertex vert
      #pragma fragment frag
      
      #include "UnityCG.cginc"
      #define conv_mxt4x4_0(mat4x4) float4(mat4x4[0].x,mat4x4[1].x,mat4x4[2].x,mat4x4[3].x)
      #define conv_mxt4x4_1(mat4x4) float4(mat4x4[0].y,mat4x4[1].y,mat4x4[2].y,mat4x4[3].y)
      #define conv_mxt4x4_2(mat4x4) float4(mat4x4[0].z,mat4x4[1].z,mat4x4[2].z,mat4x4[3].z)
      #define conv_mxt4x4_3(mat4x4) float4(mat4x4[0].w,mat4x4[1].w,mat4x4[2].w,mat4x4[3].w)
      
      
      #define CODE_BLOCK_VERTEX
      //uniform float4x4 unity_ObjectToWorld;
      //uniform float4x4 unity_WorldToObject;
      //uniform float4x4 unity_MatrixVP;
      uniform float4 _MainTex_ST;
      //uniform float4 _WorldSpaceLightPos0;
      uniform float4 _LightColor0;
      uniform float4 _Color;
      uniform float4 _HColor;
      uniform float4 _SColor;
      uniform float _RampThreshold;
      uniform float _RampSmooth;
      uniform sampler2D _MainTex;
      struct appdata_t
      {
          float4 vertex :POSITION0;
          float3 normal :NORMAL0;
          float4 texcoord :TEXCOORD0;
      };
      
      struct OUT_Data_Vert
      {
          float2 texcoord :TEXCOORD0;
          float3 texcoord1 :TEXCOORD1;
          float4 texcoord2 :TEXCOORD2;
          float4 texcoord5 :TEXCOORD5;
          float4 texcoord6 :TEXCOORD6;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float2 texcoord :TEXCOORD0;
          float3 texcoord1 :TEXCOORD1;
      };
      
      struct OUT_Data_Frag
      {
          float4 color :SV_Target0;
      };
      
      float4 u_xlat0;
      float4 u_xlat1;
      float u_xlat6;
      OUT_Data_Vert vert(appdata_t in_v)
      {
          OUT_Data_Vert out_v;
          u_xlat0 = (in_v.vertex.yyyy * conv_mxt4x4_1(unity_ObjectToWorld));
          u_xlat0 = ((conv_mxt4x4_0(unity_ObjectToWorld) * in_v.vertex.xxxx) + u_xlat0);
          u_xlat0 = ((conv_mxt4x4_2(unity_ObjectToWorld) * in_v.vertex.zzzz) + u_xlat0);
          u_xlat1 = (u_xlat0 + conv_mxt4x4_3(unity_ObjectToWorld));
          out_v.texcoord2.xyz = ((conv_mxt4x4_3(unity_ObjectToWorld).xyz * in_v.vertex.www) + u_xlat0.xyz);
          out_v.vertex = mul(unity_MatrixVP, u_xlat1);
          u_xlat0.xy = TRANSFORM_TEX(in_v.texcoord.xy, _MainTex);
          out_v.texcoord.xy = u_xlat0.xy;
          u_xlat0.x = dot(in_v.normal.xyz, conv_mxt4x4_0(unity_WorldToObject).xyz);
          u_xlat0.y = dot(in_v.normal.xyz, conv_mxt4x4_1(unity_WorldToObject).xyz);
          u_xlat0.z = dot(in_v.normal.xyz, conv_mxt4x4_2(unity_WorldToObject).xyz);
          out_v.texcoord1.xyz = normalize(u_xlat0.xyz);
          out_v.texcoord2.w = 0;
          out_v.texcoord5 = float4(0, 0, 0, 0);
          out_v.texcoord6 = float4(0, 0, 0, 0);
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      float3 u_xlat16_0;
      float3 u_xlat16_1;
      float3 u_xlat10_2;
      float3 u_xlat16_3;
      float u_xlat16_6;
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          u_xlat16_0.x = dot(in_f.texcoord1.xyz, in_f.texcoord1.xyz);
          u_xlat16_0.x = rsqrt(u_xlat16_0.x);
          u_xlat16_0.xyz = (u_xlat16_0.xxx * in_f.texcoord1.xyz);
          u_xlat16_0.x = dot(u_xlat16_0.xyz, _WorldSpaceLightPos0.xyz);
          u_xlat16_0.x = max(u_xlat16_0.x, 0);
          u_xlat16_3.x = (((-_RampSmooth) * 0.5) + _RampThreshold);
          u_xlat16_0.x = ((-u_xlat16_3.x) + u_xlat16_0.x);
          u_xlat16_6 = ((_RampSmooth * 0.5) + _RampThreshold);
          u_xlat16_3.x = ((-u_xlat16_3.x) + u_xlat16_6);
          u_xlat16_3.x = (float(1) / u_xlat16_3.x);
          u_xlat16_0.x = (u_xlat16_3.x * u_xlat16_0.x);
          u_xlat16_0.x = clamp(u_xlat16_0.x, 0, 1);
          u_xlat16_3.x = ((u_xlat16_0.x * (-2)) + 3);
          u_xlat16_0.x = (u_xlat16_0.x * u_xlat16_0.x);
          u_xlat16_0.x = (u_xlat16_0.x * u_xlat16_3.x);
          u_xlat16_3.xyz = ((-_HColor.xyz) + _SColor.xyz);
          u_xlat16_3.xyz = ((_SColor.www * u_xlat16_3.xyz) + _HColor.xyz);
          u_xlat16_1.xyz = ((-u_xlat16_3.xyz) + _HColor.xyz);
          u_xlat16_0.xyz = ((u_xlat16_0.xxx * u_xlat16_1.xyz) + u_xlat16_3.xyz);
          u_xlat10_2.xyz = tex2D(_MainTex, in_f.texcoord.xy).xyz;
          u_xlat16_1.xyz = (u_xlat10_2.xyz * _Color.xyz);
          u_xlat16_1.xyz = (u_xlat16_1.xyz * _LightColor0.xyz);
          out_f.color.xyz = (u_xlat16_0.xyz * u_xlat16_1.xyz);
          out_f.color.w = 1;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
    Pass // ind: 2, name: FORWARD
    {
      Name "FORWARD"
      Tags
      { 
        "LIGHTMODE" = "FORWARDADD"
        "RenderType" = "Opaque"
      }
      ZWrite Off
      Blend One One
      // m_ProgramMask = 6
      CGPROGRAM
      #pragma multi_compile POINT
      //#pragma target 4.0
      
      #pragma vertex vert
      #pragma fragment frag
      
      #include "UnityCG.cginc"
      #define conv_mxt4x4_0(mat4x4) float4(mat4x4[0].x,mat4x4[1].x,mat4x4[2].x,mat4x4[3].x)
      #define conv_mxt4x4_1(mat4x4) float4(mat4x4[0].y,mat4x4[1].y,mat4x4[2].y,mat4x4[3].y)
      #define conv_mxt4x4_2(mat4x4) float4(mat4x4[0].z,mat4x4[1].z,mat4x4[2].z,mat4x4[3].z)
      #define conv_mxt4x4_3(mat4x4) float4(mat4x4[0].w,mat4x4[1].w,mat4x4[2].w,mat4x4[3].w)
      
      
      #define CODE_BLOCK_VERTEX
      //uniform float4x4 unity_ObjectToWorld;
      //uniform float4x4 unity_WorldToObject;
      //uniform float4x4 unity_MatrixVP;
      uniform float4x4 unity_WorldToLight;
      uniform float4 _MainTex_ST;
      //uniform float4 _WorldSpaceLightPos0;
      uniform float4 _LightColor0;
      uniform float4 _Color;
      uniform float4 _HColor;
      uniform float _RampThreshold;
      uniform float _RampSmooth;
      uniform sampler2D _MainTex;
      uniform sampler2D _LightTexture0;
      struct appdata_t
      {
          float4 vertex :POSITION0;
          float3 normal :NORMAL0;
          float4 texcoord :TEXCOORD0;
      };
      
      struct OUT_Data_Vert
      {
          float2 texcoord :TEXCOORD0;
          float3 texcoord1 :TEXCOORD1;
          float3 texcoord2 :TEXCOORD2;
          float3 texcoord3 :TEXCOORD3;
          float4 texcoord4 :TEXCOORD4;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float2 texcoord :TEXCOORD0;
          float3 texcoord1 :TEXCOORD1;
          float3 texcoord2 :TEXCOORD2;
      };
      
      struct OUT_Data_Frag
      {
          float4 color :SV_Target0;
      };
      
      float4 u_xlat0;
      float4 u_xlat1;
      float4 u_xlat2;
      float u_xlat10;
      OUT_Data_Vert vert(appdata_t in_v)
      {
          OUT_Data_Vert out_v;
          u_xlat1 = mul(unity_ObjectToWorld, float4(in_v.vertex.xyz,1.0));
          out_v.vertex = mul(unity_MatrixVP, u_xlat1);
          u_xlat1.xy = TRANSFORM_TEX(in_v.texcoord.xy, _MainTex);
          out_v.texcoord.xy = u_xlat1.xy;
          u_xlat1.x = dot(in_v.normal.xyz, conv_mxt4x4_0(unity_WorldToObject).xyz);
          u_xlat1.y = dot(in_v.normal.xyz, conv_mxt4x4_1(unity_WorldToObject).xyz);
          u_xlat1.z = dot(in_v.normal.xyz, conv_mxt4x4_2(unity_WorldToObject).xyz);
          out_v.texcoord1.xyz = normalize(u_xlat1.xyz);
          out_v.texcoord2.xyz = ((conv_mxt4x4_3(unity_ObjectToWorld).xyz * in_v.vertex.www) + u_xlat0.xyz);
          u_xlat0 = ((conv_mxt4x4_3(unity_ObjectToWorld) * in_v.vertex.wwww) + u_xlat0);
          u_xlat1.xyz = (u_xlat0.yyy * conv_mxt4x4_1(unity_WorldToLight).xyz);
          u_xlat1.xyz = ((conv_mxt4x4_0(unity_WorldToLight).xyz * u_xlat0.xxx) + u_xlat1.xyz);
          u_xlat0.xyz = ((conv_mxt4x4_2(unity_WorldToLight).xyz * u_xlat0.zzz) + u_xlat1.xyz);
          out_v.texcoord3.xyz = ((conv_mxt4x4_3(unity_WorldToLight).xyz * u_xlat0.www) + u_xlat0.xyz);
          out_v.texcoord4 = float4(0, 0, 0, 0);
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      float3 u_xlat0_d;
      float3 u_xlat10_0;
      float3 u_xlat16_1;
      float3 u_xlat16_2;
      float3 u_xlat16_3;
      float u_xlat16_5;
      float u_xlat16_9;
      float u_xlat12;
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          u_xlat0_d.xyz = ((-in_f.texcoord2.xyz) + _WorldSpaceLightPos0.xyz);
          u_xlat0_d.xyz = normalize(u_xlat0_d.xyz);
          u_xlat16_1.x = dot(in_f.texcoord1.xyz, in_f.texcoord1.xyz);
          u_xlat16_1.x = rsqrt(u_xlat16_1.x);
          u_xlat16_1.xyz = (u_xlat16_1.xxx * in_f.texcoord1.xyz);
          u_xlat16_1.x = dot(u_xlat16_1.xyz, u_xlat0_d.xyz);
          u_xlat16_1.x = max(u_xlat16_1.x, 0);
          u_xlat16_5 = (((-_RampSmooth) * 0.5) + _RampThreshold);
          u_xlat16_1.x = ((-u_xlat16_5) + u_xlat16_1.x);
          u_xlat16_9 = ((_RampSmooth * 0.5) + _RampThreshold);
          u_xlat16_5 = ((-u_xlat16_5) + u_xlat16_9);
          u_xlat16_5 = (float(1) / u_xlat16_5);
          u_xlat16_1.x = (u_xlat16_5 * u_xlat16_1.x);
          u_xlat16_1.x = clamp(u_xlat16_1.x, 0, 1);
          u_xlat16_5 = ((u_xlat16_1.x * (-2)) + 3);
          u_xlat16_1.x = (u_xlat16_1.x * u_xlat16_1.x);
          u_xlat16_1.x = (u_xlat16_1.x * u_xlat16_5);
          u_xlat16_1.xyz = (u_xlat16_1.xxx * _HColor.xyz);
          u_xlat0_d.xyz = (in_f.texcoord2.yyy * conv_mxt4x4_1(unity_WorldToLight).xyz);
          u_xlat0_d.xyz = ((conv_mxt4x4_0(unity_WorldToLight).xyz * in_f.texcoord2.xxx) + u_xlat0_d.xyz);
          u_xlat0_d.xyz = ((conv_mxt4x4_2(unity_WorldToLight).xyz * in_f.texcoord2.zzz) + u_xlat0_d.xyz);
          u_xlat0_d.xyz = (u_xlat0_d.xyz + conv_mxt4x4_3(unity_WorldToLight).xyz);
          u_xlat0_d.x = dot(u_xlat0_d.xyz, u_xlat0_d.xyz);
          u_xlat0_d.x = tex2D(_LightTexture0, u_xlat0_d.xx).x;
          u_xlat16_2.xyz = (u_xlat0_d.xxx * _LightColor0.xyz);
          u_xlat10_0.xyz = tex2D(_MainTex, in_f.texcoord.xy).xyz;
          u_xlat16_3.xyz = (u_xlat10_0.xyz * _Color.xyz);
          u_xlat16_2.xyz = (u_xlat16_2.xyz * u_xlat16_3.xyz);
          out_f.color.xyz = (u_xlat16_1.xyz * u_xlat16_2.xyz);
          out_f.color.w = 1;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack "Diffuse"
}
