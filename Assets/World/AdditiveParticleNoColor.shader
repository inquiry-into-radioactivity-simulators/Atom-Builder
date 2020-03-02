Shader "Particles/AdditiveNoColor" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Pass{
			Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
			Colormask RGB
			Lighting Off ZWrite Off Fog {Mode off}
			Blend One One
			SetTexture [_MainTex] {

				combine texture 
			}
		}
		
	} 
	FallBack off
}
