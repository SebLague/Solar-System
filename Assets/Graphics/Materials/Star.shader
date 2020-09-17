Shader "Celestial/Star"
{
	SubShader
	{
		Tags { "Queue" = "Overlay" "RenderType" = "Transparent"}
		LOD 100
		ZWrite Off
		Lighting Off
      Blend SrcAlpha OneMinusSrcAlpha

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
				float4 vertex : SV_POSITION;
				float4 col : TEXCOORD0;
				float brightnessFalloff : TEXCOORD1;

			};

			float daytimeFade;
			sampler2D _MainTex;
			sampler2D _Spectrum;
			sampler2D _OceanMask;


			v2f vert (appdata v)
			{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
				
					float4 screenPos = ComputeScreenPos(o.vertex);
					float2 screenSpaceUV = screenPos.xy / screenPos.w;
					float4 backgroundCol = tex2Dlod(_MainTex, float4(screenSpaceUV.xy, 0, 0));
					float oceanMask = tex2Dlod(_OceanMask, float4(screenSpaceUV.xy, 0, 0));

					float backgroundBrightness = saturate(dot(backgroundCol.rgb, 1) / 3 * daytimeFade);
					float starBrightness = (1 - backgroundBrightness) * (1-oceanMask);
					float4 starCol = tex2Dlod(_Spectrum, float4(v.uv.y, 0.5, 0, 0));
					//o.col = lerp(backgroundCol, starCol, starBrightness);
					o.col = float4(starCol.rgb,starBrightness);
					o.brightnessFalloff = v.uv.x;
					/*
					if (oceanMask.x >0.5) {
						o.col = float4(1,0,0,0);
					}
					else {
						o.col = float4(0,1,0,0);
					}
					*/
		
					return o;
			}

			float4 frag (v2f i) : SV_Target
			{	
				float b = i.brightnessFalloff;
				b = saturate (b+0.1);
				b*=b;
				
				return float4(i.col.rgb, i.col.a * b);
			}

			ENDCG
		}
	}
}
