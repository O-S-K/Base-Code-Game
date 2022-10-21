Shader "Custom/Water4"
{
    Properties
    {
        _Color1 ("Color 1", Color) = (1,1,1,1)
        _Color2 ("Color 2", Color) = (1,1,1,1)
        _SpecColor ("Specular Color", Color) = (1.0,1.0,1.0,1.0)
        _Shininess ("Shininess", Float) = 10
        _WaveSpeed ("WaveSpeed", Range(0,50)) = 10
        _Displacement ("Displacement", Range(0,100)) = 3
        _FoamAmount ("Foam", Range(0,100))=3
        _Transparancy ("Transparancy", Range(0,1)) = 0.1
    }
    SubShader
    {

        Tags {  "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100
        GrabPass { "_GrabTexture"}
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.0
            // make fog work
            // #pragma multi_compile_fog
            
            #include "UnityCG.cginc"
            sampler2D _GrabTexture;
            float random (in float2 st) {
                return frac(sin(dot(st.xy,
                                    float2(12.9898,78.233)))
                                    * 43758.5453123);
            }
            float noise (float2 st) {
                    float2 i = floor(st);
                    float2 f = frac(st);

                    float a = random(i);
                    float b = random(i + float2(1.0, 0.0));
                    float c = random(i + float2(0.0, 1.0));
                    float d = random(i + float2(1.0, 1.0));

    
                    float2 u = f*f*(3.0-2.0*f);


                    return lerp(a, b, u.x) +
                            (c - a)* u.y * (1.0 - u.x) +
                            (d - b) * u.x * u.y;
            }
            float noise (float x,float y) {
                    float2 i = floor(float2(x,y));
                    float2 f = frac(float2(x,y));

                    float a = random(i);
                    float b = random(i + float2(1.0, 0.0));
                    float c = random(i + float2(0.0, 1.0));
                    float d = random(i + float2(1.0, 1.0));


                    float2 u = f*f*(3.0-2.0*f);


                    return lerp(a, b, u.x) +
                            (c - a)* u.y * (1.0 - u.x) +
                            (d - b) * u.x * u.y;
            }
            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 posWorld : TEXCOORD1;
                float4 vertex : SV_POSITION;
                float3 normalDir : TEXCOORD2;
                float4 screenPos : TEXCOORD3;
                float4 uvGrab : TEXCOORD4;
                float eyeDepth : TEXCOORD5;
            };
            sampler2D _CameraDepthTexture;
            fixed4 _Color1;
            fixed4 _Color2;
            float _WaveSpeed;
            float _Displacement;
            float3 CalculateNormal(float2 pos)
            {
                float3 newNormal;
                float x = pos.x;
                float y = pos.y;
                float diff = 0.05;
                float3 uplp = float3(noise(x,y)-noise(x-diff,y),1,noise(x,y)-noise(x,y-diff));
                float3 oppr = float3(noise(x+diff,y)-noise(x,y),1,noise(x,y)-noise(x,y-diff));
                float3 pdlp = float3(noise(x,y)-noise(x-diff,y),1,noise(x,y+diff)-noise(x,y));
                float3 pdpr = float3(noise(x+diff,y)-noise(x,y),1,noise(x,y+diff)-noise(x,y));
                newNormal = uplp + oppr + pdlp + pdpr;
                newNormal = normalize(newNormal);
                return newNormal;
            }
            float2 CalcWorldPos(float3 pos){
                return float2(pos.x/2 + _Time.x * _WaveSpeed,pos.z/2 + _Time.y * _WaveSpeed);
            }
            v2f vert (appdata_full v)
            {
                v2f o;
                o.normalDir = v.normal;
                o.posWorld = mul(unity_ObjectToWorld,v.vertex);
                float4 worldPos = mul(unity_ObjectToWorld,v.vertex);
                float displacement = noise(CalcWorldPos(worldPos.xyz));
                float4 Wpos = float4(worldPos.x,worldPos.y + _Displacement * displacement ,worldPos.z,worldPos.w);
                o.posWorld = Wpos;
                float4 WorldToObject = mul(unity_WorldToObject,Wpos);
                o.normalDir = CalculateNormal(CalcWorldPos(v.vertex.xyz));
                v.vertex = mul(unity_WorldToObject,Wpos);
                o.vertex = UnityObjectToClipPos(v.vertex);
                Wpos = mul(UNITY_MATRIX_VP,Wpos);
                o.uvGrab = ComputeGrabScreenPos(Wpos);
                o.screenPos = ComputeScreenPos(Wpos);
                COMPUTE_EYEDEPTH(o.eyeDepth);

                return o;
            }
            uniform float4 _SpecColor;
            uniform float _Shininess;    
            uniform float4 _LightColor0;
            float _Transparancy;
            float _FoamAmount;
            fixed4 frag (v2f i) : SV_Target
            {
                float3 normalDirection = CalculateNormal(CalcWorldPos(i.posWorld));
                normalDirection.xz *= -1;
                float3 viewDirection = normalize( _WorldSpaceCameraPos.xyz - i.posWorld.xyz );
                float3 lightDirection;
                float atten;
            
                if(_WorldSpaceLightPos0.w == 0.0){ //directional light
                    atten = 1.0;
                    lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                }
                else{
                    float3 fragmentToLightSource = _WorldSpaceLightPos0.xyz - i.posWorld.xyz;
                    float distance = length(fragmentToLightSource);
                    atten = 1.0/distance;
                    lightDirection = normalize(fragmentToLightSource);
                }
            
                //Lighting
                float3 diffuseReflection = atten * _LightColor0.xyz * saturate(dot(normalDirection, lightDirection));
                float3 specularReflection = diffuseReflection * _SpecColor.xyz * pow(saturate(dot(reflect(-lightDirection, normalDirection), viewDirection)) , _Shininess);
                
                float3 lightFinal = UNITY_LIGHTMODEL_AMBIENT.xyz + diffuseReflection + specularReflection;// + rimLighting;
      

                float4 projCoords = UNITY_PROJ_COORD(i.screenPos);
                float rawZ = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, projCoords);
                float sceneZ = LinearEyeDepth(rawZ);
                float surfaceZ = i.eyeDepth;

                float foam = 1-((sceneZ - surfaceZ) / _FoamAmount);
                fixed4 grab = tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(i.uvGrab));
                // fixed4 grab = tex2Dproj(_GrabTexture, projCoords);

                float3 cameraDir = i.posWorld - _WorldSpaceCameraPos;
                cameraDir = normalize(cameraDir);
                foam = saturate(foam);
                float3 fakeNormal = float3(0,1,0);
                float dt = -dot(cameraDir,fakeNormal);
                dt = clamp(dt,0,1); 
                fixed4 col = lerp(_Color1,_Color2,dt-0.2);
                col = lerp(col , grab, foam+_Transparancy);
                // sample the texture
                col.xyz = col.xyz * lightFinal.xyz;
                return col;
            }
            ENDCG
        }
    }
}