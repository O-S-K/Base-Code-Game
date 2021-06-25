Shader "Custom/BasicLightTintedRampVC"
{
  Properties
  {
    _ShadowColor ("Shadow Color", Color) = (0,0,0,0)
    _VariationColor ("Variation Color", Color) = (1,1,1,1)
    _ShadeColor ("Shade Color", Color) = (0.5,0.5,0.5,0.5)
    _ShadeMin ("Shade Min", Range(-1, 1)) = 0
    _ShadeMax ("Shade Max", Range(-1, 1)) = 0
    _TintColor ("TintColor", Color) = (1,1,1,1)
    [Enum(Off,0,Front,1,Back,2)] _Culling ("Culling", float) = 2
  }
  SubShader
  {
    Tags
    { 
      "LIGHTMODE" = "FORWARDBASE"
      "RenderType" = "Opaque"
    }
    LOD 200
    Pass // ind: 1, name: BASE
    {
      Name "BASE"
      Tags
      { 
        "LIGHTMODE" = "FORWARDBASE"
        "RenderType" = "Opaque"
        "SHADOWSUPPORT" = "true"
      }
      LOD 200
      Cull Off
      Lighting On
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
      
      
      #define CODE_BLOCK_VERTEX
      //uniform float4x4 unity_ObjectToWorld;
      //uniform float4x4 unity_WorldToObject;
      //uniform float4x4 unity_MatrixVP;
      //uniform float4 _WorldSpaceLightPos0;
      uniform float _ShadeMin;
      uniform float _ShadeMax;
      uniform float4 _TintColor;
      uniform float4 _VariationColor;
      uniform float4 _ShadeColor;
      struct appdata_t
      {
          float4 vertex :POSITION0;
          float3 normal :NORMAL0;
          float4 color :COLOR0;
      };
      
      struct OUT_Data_Vert
      {
          float4 color :COLOR0;
          float3 vs_NORMAL0 :NORMAL0;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float4 color :COLOR0;
          float3 vs_NORMAL0 :NORMAL0;
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
          out_v.vertex = UnityObjectToClipPos(in_v.vertex);
          out_v.color = in_v.color;
          u_xlat0.x = dot(in_v.normal.xyz, conv_mxt4x4_0(unity_WorldToObject).xyz);
          u_xlat0.y = dot(in_v.normal.xyz, conv_mxt4x4_1(unity_WorldToObject).xyz);
          u_xlat0.z = dot(in_v.normal.xyz, conv_mxt4x4_2(unity_WorldToObject).xyz);
          out_v.vs_NORMAL0.xyz = float3(normalize(u_xlat0.xyz));
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      float u_xlat0_d;
      float3 u_xlat16_1;
      float3 u_xlat16_2;
      float3 u_xlat3;
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          u_xlat0_d = dot(in_f.vs_NORMAL0.xyz, _WorldSpaceLightPos0.xyz);
          u_xlat0_d = max(u_xlat0_d, 0);
          u_xlat0_d = (u_xlat0_d + (-_ShadeMin));
          u_xlat3.x = ((-_ShadeMin) + _ShadeMax);
          u_xlat3.x = (float(1) / u_xlat3.x);
          u_xlat0_d = (u_xlat3.x * u_xlat0_d);
          u_xlat0_d = clamp(u_xlat0_d, 0, 1);
          u_xlat3.x = ((u_xlat0_d * (-2)) + 3);
          u_xlat0_d = (u_xlat0_d * u_xlat0_d);
          u_xlat0_d = (u_xlat0_d * u_xlat3.x);
          u_xlat3.xyz = float3((in_f.color.xyz * _VariationColor.www));
          u_xlat3.xyz = float3(((in_f.color.xyz * _VariationColor.xyz) + u_xlat3.xyz));
          u_xlat16_1.xyz = float3((u_xlat3.xyz * _ShadeColor.xyz));
          u_xlat16_2.xyz = float3((((-u_xlat3.xyz) * _ShadeColor.xyz) + u_xlat3.xyz));
          u_xlat16_1.xyz = float3(((float3(u_xlat0_d, u_xlat0_d, u_xlat0_d) * u_xlat16_2.xyz) + u_xlat16_1.xyz));
          out_f.color.xyz = float3((u_xlat16_1.xyz * _TintColor.xyz));
          out_f.color.w = 1;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack "Diffuse"
}
