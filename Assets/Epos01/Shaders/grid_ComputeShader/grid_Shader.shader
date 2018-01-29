// https://docs.unity3d.com/Manual/ShaderTut2.html

Shader "The5/grid_drawPoints"
{
	Properties
	{
        _ColorAdd ("Color Add", Color) = (0,1,0)
	}
	
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma target 5.0

			#pragma vertex vertex_shader //the vertex shaders function will be called "vertex_shader"
			#pragma fragment fragment_shader //the fragment shaders function will be called "fragment_shader"


			//all includes: D:\Program Files\Unity\Editor\Data\CGIncludes
			//see https://docs.unity3d.com/Manual/SL-BuiltinIncludes.html
			#include "UnityCG.cginc" //unity builtin library with some helper functions

			struct data
			{
				float3 pos;
			};

			//a normal mesh could only hod 65k points
			StructuredBuffer<data> buf_Points; //buffer that holds the points
			float3 _worldPos; //The center pos for the grid

			//properties:
			fixed3 _ColorAdd;

			//this is the data passed from vertex to fragment shader (just named "v2f" here)
			struct v2f
			{
				float4 pos: SV_Position;
				float3 color: COLOR0;
			};

			v2f vertex_shader(uint id: SV_VertexID)
			{
				v2f o; //o = output // i = input
				float3 worldPos = buf_Points[id].pos + _worldPos;
				o.pos = mul(UNITY_MATRIX_VP, float4(worldPos,1.0f));
				//o.pos = UnityObjectToClipPos(float4(worldPos,1.0f));
				o.color = worldPos + _ColorAdd;
				return o;
			}

			float4 fragment_shader(v2f i): COLOR
			{
				float4 col = float4(i.color,1);

				return col;
			}

			ENDCG
		}
	}

}
