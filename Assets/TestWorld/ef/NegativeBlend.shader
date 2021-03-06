Shader "NegativeBlend" {
	Properties {
		_Color ("Color", Color) = (0.25,0.25,0.25,0.25)
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Pass{
			Colormask A
			Lighting Off ZWrite Off Fog {Mode off}
			Blend One One
			SetTexture [_MainTex] {
				constantColor [_Color]
				combine texture  * constant quad
			}
		}
	} 
	FallBack off
}