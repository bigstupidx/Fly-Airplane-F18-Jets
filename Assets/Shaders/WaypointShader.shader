// Unlit alpha-blended shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "Custom/WaypointShader" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
}

SubShader {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	LOD 100
	
	ZWrite Off
	Blend SrcAlpha OneMinusSrcAlpha 
	
	Pass {  
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				fixed4 texcoord : TEXCOORD0;
				float3 viewT:TEXCOORD1;
				float3 normal : TEXCOORD2;
			};

			fixed4 _Color;
			
			v2f vert (appdata_t v)
			{
				v2f o;

				half c = abs(cos(_Time*100)*0.15f);
				v.normal = v.normal*sqrt(c);
				v.vertex.x = v.vertex.x + v.normal.x;
				v.vertex.y = v.vertex.y + v.normal.y;
				v.vertex.z = v.vertex.z + v.normal.z;

				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);		
				o.texcoord = _Color;
				o.texcoord.a =  0.3 + abs((cos(_Time*100))*0.25);

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = i.texcoord;
				return col;
			}
		ENDCG
	}
}

}
