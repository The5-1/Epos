//https://github.com/googlesamples/tango-examples-unity/blob/master/UnityExamples/Assets/TangoPrefabs/Shaders/PointCloud_Occlusion.shader

Shader "The5/PointCloud/PointCloudOcc"
{
   SubShader {
        Tags { "Queue" = "Background-1" }
        Pass {
            ColorMask 0
        
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
               float4 vertex : POSITION;
            };
       
            struct v2f
            {
               float4 vertex : SV_POSITION;
               float size : PSIZE;
            };
           
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.size = 30;
                return o;
            }
           
            fixed4 frag (v2f i) : SV_Target
            {
                return fixed4(1, 1, 1, 1);
            }
            ENDCG
        }
    }
}