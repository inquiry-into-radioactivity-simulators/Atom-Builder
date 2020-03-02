Shader "SubtractMix" {
	Properties {
		_RedColor ("Red Color", Color) = (0.5,0.5,0.5,0.5)
		_BlueColor ("Blue Color", Color) = (0.5,0.5,0.5,0.5)
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
			Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
			Colormask RGBA
			Lighting Off ZWrite Off Fog {Mode off}

		Pass{
			Blend  One One
			SetTexture [_MainTex] {
				combine texture alpha - texture double
			}
			SetTexture[_MainTex] {
				constantColor [_BlueColor]
				combine constant * previous 
			}
		}
		Pass{
			Blend One One
			SetTexture [_MainTex] {
				combine texture - texture alpha 
			}
			SetTexture[_MainTex] {
				constantColor [_RedColor]
				combine constant * previous double
			}
		}

	} 
	FallBack off
}