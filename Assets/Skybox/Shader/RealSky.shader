Shader "RealSky/RealSky" {

    Properties {
        _Texture01 ("Base (RGB)", 2D) = "white" {}
    }

    Category {

        ZWrite On

    SubShader {

        Pass {    
            Tags { "RenderType"="Opaque" }

            Lighting Off

            SetTexture [_Texture01] { combine texture }
        }

    } 


}

}