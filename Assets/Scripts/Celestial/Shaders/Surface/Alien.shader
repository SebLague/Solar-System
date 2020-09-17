Shader "Celestial/Alien"
{
	Properties
	{
		[Header(Flat Terrain)]
		_ShoreCol("Shore Colour", Color) = (0,0,0,1)
		_FlatColA("Flat Colour A", Color) = (0,0,0,1)
		_FlatColB("Flat Colour B", Color) = (0,0,0,1)
		_FlatColBlend("Colour Blend", Range(0,3)) = 1.5
		_FlatColBlendNoise("Blend Noise", Range(0,1)) = 0.3
		_ShoreHeight("Shore Height", Range(0,0.2)) = 0.05
		_ShoreBlend("Shore Blend", Range(0,0.2)) = 0.03
		_MaxFlatHeight("Max Flat Height", Range(0,1)) = 0.5

		[Header(Steep Terrain)]
		_SteepColA("Steep Colour A", Color) = (0,0,0,1)
		_SteepColB("Steep Colour B", Color) = (0,0,0,1)
		_SteepBands("Steep Bands", Range(1, 20)) = 8
		_SteepBandStrength("Band Strength", Range(-1,1)) = 0.5

		[Header(Flat to Steep Transition)]
		_SteepnessThreshold("Steep Threshold", Range(0,1)) = 0.5
		_FlatToSteepBlend("Flat to Steep Blend", Range(0,1)) = 0.1
		_FlatToSteepNoise("Flat to Steep Noise", Range(0,1)) = 0.1


		[Header(Noise)]
		[NoScaleOffset] _NoiseTex ("Noise Texture", 2D) = "white" {}
		_NoiseScale("Noise Scale", Float) = 1

		[Header(Other)]
		_FresnelCol("Fresnel Colour", Color) = (1,1,1,1)
		_FresnelStrengthNear("Fresnel Strength Min", float) = 2
		_FresnelStrengthFar("Fresnel Strength Max", float) = 5
		_FresnelPow("Fresnel Power", float) = 2
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_GlossinessFlat ("Smoothness Flat", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0

		_TestParams ("Test Params", Vector) = (0,0,0,0)
	
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM

		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows vertex:vert
		#pragma target 3.5

		#include "../Includes/Triplanar.cginc"
		#include "../Includes/Math.cginc"

		struct Input
		{
			float2 uv_MainTex;
			float4 terrainData;
			float3 vertPos;
			float3 normal;
			float4 tangent;
			float fresnel;
		};

		float4 _FresnelCol;
		float _FresnelStrengthNear;
		float _FresnelStrengthFar;
		float _FresnelPow;
		float bodyScale;

		void vert (inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.vertPos = v.vertex;
			o.normal = v.normal;
			o.terrainData = v.texcoord;
			o.tangent = v.tangent;
			
			// Fresnel (fade out when close to body)
			float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
			float3 bodyWorldCentre = mul(unity_ObjectToWorld, float4(0, 0, 0, 1)).xyz;
			float camRadiiFromSurface = (length(bodyWorldCentre - _WorldSpaceCameraPos.xyz) - bodyScale) / bodyScale;
			float fresnelT = smoothstep(0,1,camRadiiFromSurface);
			float3 viewDir = normalize(worldPos - _WorldSpaceCameraPos.xyz);
			float3 normWorld = normalize(mul(unity_ObjectToWorld, float4(v.normal,0)));
			float fresStrength = lerp(_FresnelStrengthNear, _FresnelStrengthFar, fresnelT);
			o.fresnel = saturate(fresStrength * pow(1 + dot(viewDir, normWorld), _FresnelPow));
		}

		// Flat terrain:
		float4 _ShoreCol;
		float4 _FlatColA;
		float4 _FlatColA2;
		float4 _FlatColB;
		float4 _FlatColB2;
		float _FlatColBlend;
		float _FlatColBlendNoise;
		float _ShoreHeight;
		float _ShoreBlend;
		float _MaxFlatHeight;

		// Steep terrain
		float4 _SteepColA;
		float4 _SteepColB;
		float _SteepBands;
		float _SteepBandStrength;

		// Flat to steep transition
		float _SteepnessThreshold;
		float _FlatToSteepBlend;
		float _FlatToSteepNoise;

		// Other:
		float _Glossiness;
		float _GlossinessFlat;
		float _Metallic;

		sampler2D _NoiseTex;
		float _NoiseScale;
		float4 _TestParams;

		// Height data:
		float2 heightMinMax;
		float oceanLevel;



		float Blend2(float startHeight, float blendDst, float height) {
			float t = remap01(height, startHeight, startHeight + blendDst / 2);
			return t;
		}

		void surf (Input IN, inout SurfaceOutputStandard o)
		{
		
			// Calculate steepness: 0 where totally flat, 1 at max steepness
			float3 sphereNormal = normalize(IN.vertPos);
			float steepness = 1 - dot (sphereNormal, IN.normal);
			steepness = remap01(steepness, 0, 0.65);
			

			// Calculate heights
			float terrainHeight = length(IN.vertPos);
			float shoreHeight = lerp(heightMinMax.x, 1, oceanLevel);
			float aboveShoreHeight01 = remap01(terrainHeight, shoreHeight, heightMinMax.y);
			float flatHeight01 = remap01(aboveShoreHeight01, 0, _MaxFlatHeight);

			// Sample noise texture at two different scales
			float4 texNoise = triplanar(IN.vertPos, IN.normal, _NoiseScale, _NoiseTex);

			// Flat terrain colour
			float biomeWeight = saturate(Blend(0, 0.5, IN.terrainData.x + IN.terrainData.y * -0.15));
			float channelBlend = Blend(0, 1, IN.terrainData.w);
			float flatBlendA = lerp(texNoise.r, texNoise.b, channelBlend);
			//flatBlendA = (int)(flatBlendA * (_TestParams.w + 1)) / _TestParams.w;
			float spotBlend = lerp(-0.7, 0.7, Blend(0, 2, IN.terrainData.w));

			float3 flatColA = lerp(_FlatColA, _FlatColA2, saturate(flatHeight01 + (flatBlendA-0.5) * spotBlend));
			float3 flatColB = lerp(_FlatColB, _FlatColB2, saturate(flatHeight01 + (flatBlendA-0.5) * spotBlend));
			float3 flatTerrainCol = lerp(flatColA, flatColB, biomeWeight);
			//flatTerrainCol = flatColB;
			float shoreBlendWeight = 1-Blend(_ShoreHeight, _ShoreBlend, flatHeight01);
			flatTerrainCol = lerp(flatTerrainCol, _ShoreCol, shoreBlendWeight);

			// Steep terrain colour
			float3 sphereTangent = normalize(float3(-sphereNormal.z, 0, sphereNormal.x));
			float3 normalTangent = normalize(IN.normal - sphereNormal * dot(IN.normal, sphereNormal));
			float banding = dot(sphereTangent, normalTangent) * .5 + .5;
			banding += (texNoise.a-0.5) * 0.1;
			banding = (int)(banding * (_SteepBands + 1)) / _SteepBands;
			banding = (abs(banding - 0.5) * 2 - 0.5) * _SteepBandStrength;
			float3 steepTerrainCol = lerp(_SteepColA, _SteepColB, aboveShoreHeight01 + banding);
			
			// Flat to steep colour transition
			float flatBlendNoise = (texNoise.b - 0.5) * -_FlatToSteepNoise;

			//float b = lerp(0, _TestParams.x, Blend(0, 3, 5)) * 0.1;
			float flatStrength = 1 - Blend2(_SteepnessThreshold + flatBlendNoise, saturate(_FlatToSteepBlend), steepness);
			
			// Set surface colour
			float3 compositeCol = lerp(steepTerrainCol, flatTerrainCol, flatStrength);
			compositeCol = lerp(compositeCol, _FresnelCol, IN.fresnel);
			o.Albedo = compositeCol;

			// Glossiness
			//float glossiness = dot(o.Albedo, 1) / 3 * _Glossiness;
			//glossiness = max(glossiness, snowWeight * _SnowSpecular);
			o.Smoothness = max(steepness * _Glossiness, _GlossinessFlat);
			o.Metallic = _Metallic;
			//o.Albedo = camRadiiFromSurface;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
