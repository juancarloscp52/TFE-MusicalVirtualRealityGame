﻿Shader "Flat/Absolute Circle" {
	Properties{
		_BackgroundColor("Background Color", Color) = (0,0,0,1)
		_ForegroundMask("Foreground Mask", 2D) = "white" {}
		_Radius("Radius", Range(0,1)) = 0.5
		_BorderSize("Border Size", Range(0,1)) = 0.5
	}
		SubShader{
		Tags{ "RenderType" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		LOD 200


		CGPROGRAM
//		// Physically based Standard lighting model, and enable shadows on all light types
#pragma surface surf Standard fullforwardshadows

	sampler2D _ForegroundMask;

	struct Input {
		float2 uv_ForegroundMask;
	};

	fixed4 _BackgroundColor;
	half _Radius;
	half _BorderSize;

	void surf(Input IN, inout SurfaceOutputStandard o) {
		
		fixed x = (-0.5 + IN.uv_ForegroundMask.x) * 2;
		fixed y = (-0.5 + IN.uv_ForegroundMask.y) * 2;

		// Albedo comes from a texture tinted by color
		fixed size = sqrt(x*x + y*y);
		clip(size - (_Radius-_BorderSize));
		clip(_Radius - size);
		o.Albedo = _BackgroundColor;
		o.Alpha = 1;
	}
	ENDCG
	}
	FallBack "Diffuse"
}