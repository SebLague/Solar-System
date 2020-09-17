Shader "Unlit/Phase"
{
    Properties
    {
		_G ("G", Range(-1, 1)) = 0
		_Params ("Params", Vector) = (0,0,0,0)
		[Toggle]
		_UseHG ("UseHG", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

				float _G;
				float4 _Params;
				float _UseHG;

				float phaseRayleight(float cosTheta) {
					return 3/(16*3.1415) * (1+cosTheta * cosTheta);
				}

				float phaseHG(float cosTheta, float g) {
					float a = (3 * (1 - g * g)) / (2 * (2 + g * g));
					float b = (1 + cosTheta * cosTheta) / pow (1 + g*g - 2 * g * cosTheta, 3/2);
					return a * b;
				}

				float phaseHG2(float cosTheta, float g) {
					float g2 = g*g;
            	return (1-g2) / (4*3.1415*pow(1+g2-2*g*(cosTheta), 1.5));
				}

				float Schlick(float k, float cosTheta) {
					return (1 - k * k) / (4 * 3.1415 * pow(1.0 - k * cosTheta, 2.0));
				}

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float cosTheta = dot (normalize(i.uv-0.5), float2(1,0));
					 return phaseRayleight(cosTheta);
                return phaseHG(cosTheta, _G) * _UseHG + Schlick(_G, cosTheta) * (1-_UseHG);
            }
            ENDCG
        }
    }
}
