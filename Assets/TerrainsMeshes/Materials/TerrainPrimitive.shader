Shader "Custom/TerrainPrimitive" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_SubTex ("Base (RGB)", 2D) = "white" {}
		_BlendTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _MainTex;
		sampler2D _SubTex;
		sampler2D _BlendTex;

		struct Input {
			float2 uv_MainTex;
			float2 uv_BlendTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			half4 c2 = tex2D (_SubTex, IN.uv_MainTex);
			half4 b = tex2D (_BlendTex, IN.uv_BlendTex)*2.5;

			half blendValue = (1- b.g);

			o.Albedo = ((c * blendValue) + c2*(1 - blendValue));
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
