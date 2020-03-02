Shader "PositiveBlendOnePass" {
	Properties {
		_Color ("Color", Color) = (0.25,0.25,0.25,0.25)
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Pass{
			Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
			Colormask RGB
			Lighting Off ZWrite Off Fog {Mode off}
			Blend SrcAlpha One
			SetTexture [_MainTex] {
				constantColor [_Color]
				combine texture  * constant, texture alpha * constant alpha QUAD
			}
		}
		
	} 
	FallBack off
}
