Shader "Particles/Nucleon" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_MainTex2 ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Pass {
			Colormask a
			AlphaTest Greater 0.99
			ZWrite On
			SetTexture [_MainTex] {
				combine texture, texture
			}
		}
		Pass{
			Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
			Colormask RGB
			Lighting Off ZWrite Off Fog {Mode off}
			Blend SrcAlpha OneMinusSrcAlpha
				BindChannels {
					Bind "Color", color
					Bind "Vertex", vertex
					Bind "TexCoord", texcoord
				}
			SetTexture [_MainTex] {
				combine texture * primary, texture
			}
			SetTexture [_MainTex2] {
				combine previous+texture, previous
			}
		}
		
	} 
	FallBack off
}
