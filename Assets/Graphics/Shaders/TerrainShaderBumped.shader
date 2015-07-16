Shader "Terrain/MultipluerBumped" {
	Properties {
		_Height1 ("Height1",float) = 1
		_Height2 ("Height2",float) = 1
		_Grass1 ("Height Map Bot", 2D) = "white" {}
		_Grass2 ("Height Map Mid", 2D) = "white" {}
		_BumpGrass2 ("Height Bump Map Mid", 2D) = "white" {}
		_Grass3 ("Height Map Top", 2D) = "white" {}
		_BumpGrass3 ("Height Bump Map Top", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert addshadow 
		#pragma target 3.0 

		sampler2D _Grass1;
		sampler2D _Grass2;
		sampler2D _Grass3;
		sampler2D _BumpGrass2;
		sampler2D _BumpGrass3;
		
		float _Height1;
		float _Height2;

		struct Input {
			float2 uv_Grass1;
			float2 uv_Grass2;
			float2 uv_Grass3;
			float2 uv_BumpGrass2;
			float2 uv_BumpGrass3;
			float3 worldPos;
		};

		void surf (Input IN, inout SurfaceOutput o) 
		{
			// Normal
			half3 c2 = UnpackNormal (tex2D (_BumpGrass2, IN.uv_BumpGrass2));
			half3 c3 = UnpackNormal (tex2D (_BumpGrass3, IN.uv_BumpGrass3));
			float coof2 = clamp(IN.worldPos.y/_Height1,0,1);
			float coof3 = clamp((IN.worldPos.y - _Height1) / (_Height2 - _Height1),0,1);
			half3 norm = c2.rgb*(1.0-coof3) + c3.rgb*coof3;
			//o.Normal = c;
			
			// Color
			half3 c1 = tex2D (_Grass1, IN.uv_Grass1);
			c2 = tex2D (_Grass2, IN.uv_Grass2); 
			c3 = tex2D (_Grass3, IN.uv_Grass3);
			half3 c = c1.rgb*(1.0-coof2)*(1.0-coof3) + c2.rgb*coof2*(1.0-coof3) + c3.rgb*coof3;
			o.Albedo = c;
			o.Normal = norm;
			o.Alpha = 1;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
