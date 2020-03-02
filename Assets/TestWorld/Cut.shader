Shader "Cut" {
Properties {

}

SubShader {
	Tags { "Queue" = "Transparent-1" }
	LOD 100
	cull off
    Lighting Off
    ZTest LEqual
    ZWrite On
    ColorMask a
	Pass {
		Color (0,0,0,0)
	}
}

Fallback off
}
