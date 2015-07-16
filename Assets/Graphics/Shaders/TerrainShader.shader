Shader "Terrain/Multipluer" {
	Properties {
		_Height1 ("Height1",float) = 1
		_Height2 ("Height2",float) = 1
		_Grass1 ("Height Map Bot", 2D) = "white" {}
		_Grass2 ("Height Map Mid", 2D) = "white" {}
		_Grass3 ("Height Map Top", 2D) = "white" {}
		_Grunt1 ("Angle Map", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _Grass1;
		sampler2D _Grass2;
		sampler2D _Grass3;
		sampler2D _Grunt1;
		
		float _Height1;
		float _Height2;

		struct Input {
			float2 uv_Grass1;
			float2 uv_Grass2;
			float2 uv_Grass3;
			float2 uv_Grunt1;
			float3 worldPos;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c1 = tex2D (_Grass1, IN.uv_Grass1);
			half4 c2 = tex2D (_Grass2, IN.uv_Grass2);
			half4 c3 = tex2D (_Grass3, IN.uv_Grass3);
			float coof2 = clamp(IN.worldPos.y/_Height1,0,1);
			float coof3 = clamp((IN.worldPos.y - _Height1) / (_Height2 - _Height1),0,1);
			half3 c = c1.rgb*(1.0-coof2)*(1.0-coof3) + c2.rgb*coof2*(1.0-coof3) + c3.rgb*coof3;
			half4 g = tex2D (_Grunt1, IN.uv_Grunt1);
			half angle = dot (float3(0,1,0), o.Normal);
			o.Albedo = c*angle + g*(1.0-angle);
			o.Alpha = 1;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
