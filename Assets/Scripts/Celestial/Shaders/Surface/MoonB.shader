// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Celestial/MoonB"
{
	Properties
	{
		[Header(Colours)]
		_PrimaryColA ("Primary A", Color) = (1,1,1,1)
		_SecondaryColA ("Secondary A", Color) = (1,1,1,1)
		_PrimaryColB ("Primary B", Color) = (1,1,1,1)
		_SecondaryColB ("Secondary B", Color) = (1,1,1,1)

		_BiomeBlendStrength ("Biome Blend", Float) = 2
		_BiomeWeight ("Biome Weight", Float) = 1.6
		_BiomeWarpStrength("Biome Warp", Float) = 0
		_EjectaBrightness("Ejecta Brightness", Float) = 0.1

		[Header(Normals)]
		[NoScaleOffset] _CraterRay ("Crater Ray", 2D) = "white" {}
		[NoScaleOffset] _MainTex ("Albedo (RGB)", 2D) = "white" {}
		[NoScaleOffset] _NormalMapFlat ("Normal Map Flat", 2D) = "bump" {}
		[NoScaleOffset] _NormalMapSteep ("Normal Map Steep", 2D) = "bump" {}
		_NormalMapFlatScale ("Normal Map Flat Scale", Float) = 10
		_NormalMapSteepScale ("Normal Map Steep Scale", Float) = 10
		_MainTexScale ("Main Tex Scale", Float) = 10

		_RandomBiomeValues ("Random Biome Values", Vector) = (0,0,0,0)
		_NormalMapStrength ("Normal Map Strength", Range(0,1)) = 0.3

		[Header(Other)]
		_FresnelCol("Fresnel Colour", Color) = (1,1,1,1)
		_FresnelStrength("Fresnel Strength", float) = 0.5
		_FresnelPow("Fresnel Power", float) = 2
		_SmoothnessA ("Smoothness A", Range(0,1)) = 0.5
		_SmoothnessB ("Smoothness B", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_TestParams ("Test Params", Vector) = (0,0,0,0)

	 }
	 SubShader
	 {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows vertex:vert
		#pragma target 3.5

		#include "../Includes/Triplanar.cginc"
		#include "../Includes/Math.cginc"

		sampler2D _MainTex;
		sampler2D _CraterRay;
		sampler2D _NormalMapFlat;
		sampler2D _NormalMapSteep;

		float _NormalMapStrength;
		float _NormalMapFlatScale;
		float _NormalMapSteepScale;
		float _MainTexScale;

		float _SmoothnessA;
		float _SmoothnessB;
		float _Metallic;

		float2 heightMinMax;

		float _EjectaBrightness;

		float _BiomeBlendStrength;
		float _BiomeWeight;
		float _BiomeWarpStrength;
		float4 _RandomBiomeValues;
		float4 _TestParams;
		float _AvgBiomeNoiseDst;

		// Colors:
		float4 _PrimaryColA;
		float4 _SecondaryColA;
		float4 _PrimaryColB;
		float4 _SecondaryColB;
		float4 _SteepCol;

		float4 _FresnelCol;
		float _FresnelStrength;
		float _FresnelPow;
		float bodyScale;

		struct Input
		{
			float3 vertPos;
			float3 normal;
			float4 terrainData;
			float3 worldNormal;
			float4 tangent;
			float4 craterUV;
			float3 viewDir;
			float fresnel;
			INTERNAL_DATA
		};

		void vert (inout appdata_full v, out Input o) {
			UNITY_INITIALIZE_OUTPUT (Input,o);
			o.vertPos = v.vertex;
			o.terrainData = v.texcoord;
			o.normal = v.normal;
			o.tangent = v.tangent;
			//
			float craterDst = abs(v.texcoord.y);
			float craterColWeight = (v.texcoord.y < 0)? 0 : 1;
			float2 craterUV = 0.5 + float2(cos(v.texcoord.x), sin(v.texcoord.x)) * craterDst;
			o.craterUV = float4(craterUV.xy, craterDst, craterColWeight);

			// Fresnel (fade out when close to body)
			float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
			float3 bodyWorldCentre = mul(unity_ObjectToWorld, float4(0, 0, 0, 1)).xyz;
			float camRadiiFromSurface = (length(bodyWorldCentre - _WorldSpaceCameraPos.xyz) - bodyScale) / bodyScale;
			float fresnelT = smoothstep(0,1,camRadiiFromSurface);
			float3 viewDir = normalize(worldPos - _WorldSpaceCameraPos.xyz);
			float3 normWorld = normalize(mul(unity_ObjectToWorld, float4(v.normal,0)));
			o.fresnel = saturate(_FresnelStrength * fresnelT * pow(1 + dot(viewDir, normWorld), _FresnelPow));
		}

		fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
		{
			fixed4 c;
			c.rgb = s.Albedo * 0.8; 
			c.a = s.Alpha;
			return c;
		}

		

		void surf (Input IN, inout SurfaceOutputStandard o)
		{
			float4 texNoise = triplanar(IN.vertPos, IN.normal, _MainTexScale, _MainTex);

			float steepness = 1 - dot(IN.normal, normalize(IN.vertPos));

			float height01 = remap01(length(IN.vertPos), heightMinMax.x, heightMinMax.y);
			float t = 1-Blend(_TestParams.y,_TestParams.z, height01);

			float highAndFlat = (1 - remap01(steepness, 0.08, 0.2)) * Blend(0.6, 0.4, height01);

			float biomeWeight = Blend(_AvgBiomeNoiseDst * _BiomeWeight, _BiomeBlendStrength, IN.terrainData.w + IN.terrainData.z * _BiomeWarpStrength);
			//o.Albedo = lerp(_PrimaryColA, _PrimaryColB, biomeWeight);
			//o.Albedo = steepness;
			float heightBanded01 = saturate(int((height01+(texNoise.b-.5)*_TestParams.w) * _TestParams.z) / (_TestParams.z));
			float3 colA = lerp(_PrimaryColA, _SecondaryColA, height01);
			float3 colB = lerp(_PrimaryColB, _SecondaryColB, height01);

			float3 compositeCol = lerp(colA, colB, biomeWeight);// * lerp(0.7, 1, texNoise.b);
			compositeCol = lerp(compositeCol, _SteepCol, Blend(0.5, 0.18, height01));
			//compositeCol = lerp(compositeCol, 1, highAndFlat);

			// Ejecta
			// Crater ejecta
			//float ejectaColorWeight = IN.craterUV.w;
			//float3 ejectaCol = _SecondaryColA;
			float ejecta = tex2D(_CraterRay, IN.craterUV.xy).r;
			float craterAlpha = IN.craterUV.z < 1;
			//o.Albedo = lerp(biomeCol, ejectaCol, saturate(ejecta * 1.5) * craterAlpha);
			//compositeCol = lerp(compositeCol, _SteepCol, steepness);
			//	compositeCol = lerp(compositeCol, float3(1,0,0), ejecta * craterAlpha);
			compositeCol = lerp(compositeCol, _FresnelCol, IN.fresnel);
		
			//o.Albedo = craterAlpha;
			o.Albedo =compositeCol;

			// Normals
			float3 normalMapFlat = triplanarNormalTangentSpace(IN.vertPos, IN.normal, _NormalMapFlatScale, IN.tangent, _NormalMapFlat);
			//float3 normalMapSteep = triplanarNormalTangentSpace(IN.vertPos, IN.normal, _NormalMapSteepScale, IN.tangent, _NormalMapSteep);
			//float normalBlend = lerp(texNoise.r, texNoise.g, Blend(0, 2, terrainData.z));
			//float3 flatAndSteepNormal = lerp(normalMapFlat, normalMapSteep, normalBlend);
			//float3 normal = lerp(flatAndSteepNormal, normalMapSteep, steepness);
			float3 normal = lerp(float3(0,0,1), normalMapFlat, _NormalMapStrength);
			o.Normal = normal;

			//o.Albedo = highAndFlat;
			//o.Albedo = );

			// Set metallic and smoothness
			o.Metallic = 0;
			o.Smoothness = 0;

		}
		ENDCG
	}
	FallBack "Diffuse"
}