Shader "Custom/WaterShader" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Cubemap("CubeMap", CUBE) = ""{}
		_ReflAmount ("Reflection Amount", Range(0.01, 1)) = 0.5
		_Color ("Color", Color) = (1,1,1,1)
		_BumpMap ("Normal Map", 2D) = "bump" {}
		_NormalPower ("NormalPower", Range(0.01, 300)) = 0.5
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert
//#pragma target 3.0
		sampler2D _MainTex;
		samplerCUBE _Cubemap;
		sampler2D _BumpMap;

		float _ReflAmount;	
		float4 _Color;
		float _NormalPower;
		float _RefractionPower;
		struct Input {
			float2 uv_MainTex;
			float3 worldRefl;
			float2 uv_BumpMap;
		};

		void vert (inout appdata_full v)
		{
			float4 vPos = mul (UNITY_MATRIX_MV, v.vertex);
			v.vertex = mul (vPos, UNITY_MATRIX_IT_MV);
			v.texcoord = mul( UNITY_MATRIX_TEXTURE0, v.texcoord );  
		}

		void surf (Input IN, inout SurfaceOutput o) {
			float2 uvOffset;
			uvOffset.x = _Time/5.0f;
			float3 norm = UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap*20.1f + uvOffset));


			half4 c = tex2D (_MainTex, IN.uv_MainTex*10 - uvOffset*2);
			
			float dirtiness = 1;

			o.Emission = texCUBE(_Cubemap, IN.worldRefl + norm*_NormalPower).rgb * _ReflAmount;
			o.Albedo = c.rgb* _Color;
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
