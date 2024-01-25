Shader "Sprites/Fading"
{
	Properties
	{
		[PerRendererData]  _MainTex ("Sprite Texture", 2D) = "white" { }
		_Color ("Color", Color) = (1,1,1,1)
		_Multiplier("Multiplier", float) = 1
		_Clip("Clip", float) = 0.01
		_MinAlpha("MinAlpha", float) = 0.3
		_MaxAlpha("MaxAlpha", float) = 0.8
		[HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
		[HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
	}

	SubShader
	{
		Tags
		{
			"Queue"="Transparent"
			"IgnoreProjector"="True"
			"RenderType"="Transparent"
			"PreviewType"="Square"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
		CGPROGRAM
			#pragma vertex SpriteVert
			#pragma fragment Frag
			#pragma target 2.0
			#pragma multi_compile_instancing
			#include "UnitySprites.cginc"

			float _Multiplier;
			float _Clip;
			float _MinAlpha;
			float _MaxAlpha;

			fixed4 Frag(v2f IN) : SV_Target
			{
				float rad = sqrt(pow((0.5 - IN.texcoord.x), 2) + pow((0.5 - IN.texcoord.y), 2));

				fixed4 OUT;

				OUT.rgb = IN.color;
				OUT.a = clamp((0.5 - rad) * _Multiplier, _MinAlpha, _MaxAlpha);
				clip(OUT.a - _Clip);

				return OUT;
			}
		ENDCG
		}
	}
}