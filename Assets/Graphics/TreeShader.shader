Shader "Custom/Tree" 
{
    Properties 
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
        _Scale ("Scale", Vector) = (1,1,1,1)
        _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5              
    }

    SubShader 
    {
        Tags 
        {
            "Queue"="AlphaTest" 
            "IgnoreProjector"="True" 
            "RenderType"="TransparentCutout"
        }
        
        Cull Back
        LOD 400

        CGPROGRAM
        #pragma surface surf BlinnPhong alphatest:_Cutoff vertex:treevertex 
        //addshadow nolightmaps
        #include "TerrainEngine.cginc"
        
        void treevertex (inout appdata_full v) 
        {
            TerrainAnimateTree(v.vertex, v.color.w);
            float3 T1 = float3(1, 0, 1);
            float3 Bi = cross(T1, v.normal);
            float3 newTangent = cross(v.normal, Bi);
            normalize(newTangent);
            v.tangent.xyz = newTangent.xyz;
            if (dot(cross(v.normal,newTangent),Bi) < 0)
                v.tangent.w = -1.0f;
            else
                v.tangent.w = 1.0f;
        }

        sampler2D _MainTex;

        float4 _Color;
        float _Shininess;

        struct Input {
            float2 uv_MainTex;
            float4 color : COLOR;
        };

        void surf (Input IN, inout SurfaceOutput o) 
        {
            half4 c = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = c.rgb * _Color.rgb * IN.color.a;
            o.Alpha = c.a;
            o.Specular = _Shininess;
        }
        ENDCG
    }

    Fallback Off
}