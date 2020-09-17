Shader "GameUI/DottedLine"
{
    Properties
    {
		 _Color("Colour", Color) = (1,1,1,1)
		 _MaxAlpha("Max alpha", Range(0,1)) = 1
		 _FadeDst("Fade Dst", Float) = 5
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Overlay" }
        ZTest LEqual
        ZWrite Off
		  Blend SrcAlpha OneMinusSrcAlpha
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

            float4 _Color;
            float2 _Size;
				float _FadeDst;
				float _MaxAlpha;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float x = i.uv.x * _Size.x;
                int n = (int)(x/_Size.y*0.5);
					 float alpha = saturate(x / _FadeDst) * _MaxAlpha;
                
                if (n%2==0) {
                    clip(-1);
                }

                return float4(_Color.rbg, alpha);
                return float4(i.uv.x,0,0,1);
            }
            ENDCG
        }
    }
}
