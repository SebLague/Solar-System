Shader "Custom/Fireball blend add"
{
	Properties
	{
		_TintColor ("Tint Color", Color) = (1,1,1,1)
		_MainTex ("Particle Texture", 2D) = "white" {}
		_ScrollTex ("Scroll", 2D) = "white" {}
		_ScrollFactor ("Scroll factor", Float) = 0.5
		_ScrollSpeed ("Scroll speed", Float) = 0.5
		_InvFade ("Soft Particles Distance", Float) = 0.1
	}

	Category
	{
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
		Blend One OneMinusSrcAlpha
		//Blend SrcAlpha OneMinusSrcAlpha
		ColorMask RGB
		Cull Off Lighting Off ZWrite Off

		SubShader
		{
			Pass
			{
		
				CGPROGRAM
					#pragma vertex vert
					#pragma fragment frag
					#pragma target 2.0
			
					#include "UnityCG.cginc"
					#include "../../../Shaders/Aura.cginc"

					sampler2D _MainTex;
					float4 _MainTex_ST;
					fixed4 _TintColor;
					sampler2D_float _CameraDepthTexture;
					float _InvFade;
					sampler2D _ScrollTex;
					float4 _ScrollTex_ST;
					float _ScrollFactor;
					float _ScrollSpeed;

					struct appdata_t
					{
						float4 vertex : POSITION;
						fixed4 color : COLOR;
						float2 texcoord : TEXCOORD0;
					};

					struct v2f
					{
						float4 vertex : SV_POSITION;
						fixed4 color : COLOR;
						float2 texcoord : TEXCOORD0;
						float2 texcoordScroll : TEXCOORD1;
						float4 projPos : TEXCOORD2;
						float3 frustumSpacePosition : TEXCOORD3;
					};

					v2f vert(appdata_t v)
					{
						v2f o;
						o.vertex = UnityObjectToClipPos(v.vertex);
						o.projPos = ComputeScreenPos(o.vertex);
						COMPUTE_EYEDEPTH(o.projPos.z);

						o.color = v.color;
						o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
						o.texcoordScroll = TRANSFORM_TEX(v.texcoord, _ScrollTex);
						o.texcoordScroll.y += _Time.y * _ScrollSpeed;

						o.frustumSpacePosition = Aura_GetFrustumSpaceCoordinates(v.vertex);

						return o;
					}

					fixed4 frag(v2f i) : SV_Target
					{
						float sceneZ = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
						float partZ = i.projPos.z;
						float fade = ClampedInverseLerp(0, _InvFade, (sceneZ - partZ));
						i.color.a *= fade;

						float4 baseTextureColor = tex2D(_MainTex, i.texcoord);
						float4 scrollTextureColor = tex2D(_ScrollTex, i.texcoordScroll) * _ScrollFactor;

						fixed4 col = (baseTextureColor * scrollTextureColor * 2) *i.color * _TintColor;

						Aura_ApplyFog(col, i.frustumSpacePosition);
						col.xyz *= col.w;

						return col;
					}
				ENDCG 
			}
		}	
	}
}
