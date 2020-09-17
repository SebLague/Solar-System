// NOTE: only works properly with deferred rendering path.
// With forward rendering, shading will remain floating in air, and will block shadows on the ground (unless queue is
// set to transparent, but then won't receive shadows/write to depth texture)
// https://forum.unity.com/threads/custom-lighting-function-breaks-dithering.707096/

//https://ocias.com/blog/unity-stipple-transparency-shader/
//https://forum.unity.com/threads/clip-in-surface-shader-doesnt-clip-ligthing.470284/
//https://realtimevfx.com/t/transparency-issues-in-unity/4337/2
Shader "Custom/Stipple Transparency" {
Properties {
    _MainTex ("Base (RGB)", 2D) = "white" {}
    _Color ("Color", Color) = (1,1,1,1)
    _FadeDistance ("Fade Distance", Float) = 1.0
}
SubShader {

    Cull Off
    Tags { "Queue" = "Geometry" "IgnoreProjector" = "True"  "RenderType"="Geometry" }
    LOD 200
    CGPROGRAM
    #pragma surface surf Standard fullforwardshadows
    // Use shader model 3.0 target, to get nicer looking lighting
    #pragma target 3.0
    sampler2D _MainTex;
    

    struct Input {
        float2 uv_MainTex;
        float4 screenPos;
        float3 worldPos;
    };
    float _FadeDistance;
    fixed4 _Color;


    void surf (Input IN, inout SurfaceOutputStandard o) {
        float l = length(mul(unity_CameraInvProjection, float4(1,1,0,1)).xyz) * _ProjectionParams.y;
        float3 cameraPos = _WorldSpaceCameraPos;
        float dst = length(IN.worldPos - cameraPos);
        float fade = saturate((dst-l) / _FadeDistance);


        fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
        o.Albedo = _Color;
        o.Metallic = 0;
        o.Smoothness = 0;
        
        // Threshold values for each 4x4 block of pixels
        const float4x4 thresholdMatrix =
        {
            1, 9, 3, 11,
            13, 5, 15, 7,
            4, 12, 2, 10,
            16, 8, 14, 6
        };
        
        // Multiply screen pos by (width, height) of screen to get pixel coord
        float2 pixelPos = IN.screenPos.xy / IN.screenPos.w * _ScreenParams.xy;

        // Get threshold of current pixel and divide by 17 to get in range (0, 1)
        float threshold = thresholdMatrix[pixelPos.x % 4][pixelPos.y % 4] / 17;

        // Don't draw pixel if threshold is greater than the alpha
        // (the clip function discards the current pixel if the value is less than 0)
      	clip(fade - threshold);
        
    }

ENDCG
}
FallBack "VertexLit"
}