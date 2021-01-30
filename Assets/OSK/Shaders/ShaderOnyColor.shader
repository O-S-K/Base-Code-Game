Shader "Example/Test" {
    Properties {
      _Color ("Color", Color) = (1,1,1,1)
      _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader {
      Tags { "RenderType" = "Opaque" }
      CGPROGRAM
      #pragma surface surf Lambert
      struct Input 
      {
          float2 uv_MainTex;
      };
      fixed4 _Color;
      sampler2D _MainTex;
      void surf (Input IN, inout SurfaceOutput o) {
          o.Albedo = _Color;
      }
      ENDCG
    }
    Fallback "Diffuse"
  }