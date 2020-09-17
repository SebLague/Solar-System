Shader "Custom/Lines2"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            float thickness;
            StructuredBuffer<float2> points;
            int numPoints;
            float3 params;
            float4 colour;

            float sqrDstToSeg(float2 lineStart, float2 lineEnd, float2 p) {
                float2 offset = lineEnd - lineStart;
                float l2 = dot(offset, offset);
                float t = max(0, min(1, dot(p-lineStart, offset) / l2));
                float2 projection = lineStart + t * offset;
                float2 offsetToSegment = p - projection;
                return dot(offsetToSegment, offsetToSegment);
                //return length(p-projection); // distance
            }

            fixed2 WorldToScreenPos(fixed3 pos){
                pos = normalize(pos - _WorldSpaceCameraPos)*(_ProjectionParams.y + (_ProjectionParams.z - _ProjectionParams.y))+_WorldSpaceCameraPos;
                fixed2 uv =0;
                fixed3 toCam = mul(unity_WorldToCamera, pos);
                fixed camPosZ = toCam.z;
                fixed height = 2 * camPosZ / unity_CameraProjection._m11;
                fixed width = _ScreenParams.x / _ScreenParams.y * height;
                uv.x = (toCam.x + width / 2)/width;
                uv.y = (toCam.y + height / 2)/height;
                return uv;
            }

            

            fixed4 frag (v2f i) : SV_Target
            {
                
                float nearestSqrDst = 2;
                float2 p1 = (points[0]);
                for (int pointIndex = 1; pointIndex < numPoints; pointIndex ++) {
                    float2 p2 = (points[pointIndex]);
                    nearestSqrDst = min(nearestSqrDst, sqrDstToSeg(p1,p2, i.uv));
                    p1 = p2;
                }
                float dstToLine = sqrt(nearestSqrDst);
                const float thicknessScale = 0.001;
                float weight = 1-saturate((dstToLine-params.y * thicknessScale) * params.x);

                float4 background = tex2D(_MainTex, i.uv);
                return background * (1-weight) + weight * colour;
                //return 1-saturate((dstToLine-params.y/1000) * params.x);
                //return pow(1-saturate((dstToLine-params.y/1000) * params.x), params.z);
            }
            ENDCG
        }
    }
}
