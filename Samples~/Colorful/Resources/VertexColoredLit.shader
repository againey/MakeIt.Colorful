// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/VertexColoredLit" {
	Properties {
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 80
		
	Pass {
		Name "FORWARD"
		Tags { "LightMode" = "ForwardBase" }
CGPROGRAM
#pragma vertex vert_surf
#pragma fragment frag_surf
#pragma multi_compile_fwdbase
#pragma multi_compile_fog
#include "HLSLSupport.cginc"
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"

		inline float3 LightingLambertVS (float3 normal, float3 lightDir)
		{
			fixed diff = max (0, dot (normal, lightDir));			
			return _LightColor0.rgb * diff;
		}

		struct Input {
			float4 color : COLOR;
		};

		struct v2f_surf {
  float4 pos : SV_POSITION;
  float4 color : COLOR;
  #ifdef LIGHTMAP_OFF
  fixed3 normal : TEXCOORD1;
  #endif
  #ifndef LIGHTMAP_OFF
  float2 lmap : TEXCOORD2;
  #endif
  #ifdef LIGHTMAP_OFF
  fixed3 vlight : TEXCOORD2;
  #endif
  LIGHTING_COORDS(3,4)
  UNITY_FOG_COORDS(5)
};
v2f_surf vert_surf (appdata_full v)
{
	v2f_surf o;
	o.pos = UnityObjectToClipPos (v.vertex);
	o.color = v.color;
	#ifndef LIGHTMAP_OFF
	o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
	#endif
	float3 worldN = UnityObjectToWorldNormal(v.normal);
	#ifdef LIGHTMAP_OFF
	o.normal = worldN;
	#endif
	#ifdef LIGHTMAP_OFF
	
	o.vlight = float3(0.9, 0.9, 0.9) + LightingLambertVS (worldN, _WorldSpaceLightPos0.xyz) * 0.1;
	
	#endif // LIGHTMAP_OFF
	TRANSFER_VERTEX_TO_FRAGMENT(o);
	UNITY_TRANSFER_FOG(o,o.pos);
	return o;
}
fixed4 frag_surf (v2f_surf IN) : SV_Target
{
	SurfaceOutput o;
	o.Albedo = IN.color.rgb;
	o.Emission = 0.0;
	o.Specular = 0.0;
	o.Alpha = IN.color.a;
	o.Gloss = 0.0;
	#ifdef LIGHTMAP_OFF
	o.Normal = IN.normal;
	#else
	o.Normal = 0;
	#endif
	fixed atten = LIGHT_ATTENUATION(IN);
	fixed4 c = 0;
	#ifdef LIGHTMAP_OFF
	c.rgb = o.Albedo * IN.vlight * atten;
	#endif // LIGHTMAP_OFF
	#ifndef LIGHTMAP_OFF
	fixed3 lm = DecodeLightmap (UNITY_SAMPLE_TEX2D(unity_Lightmap, IN.lmap.xy));
	#ifdef SHADOWS_SCREEN
	c.rgb += o.Albedo * min(lm, atten*2);
	#else
	c.rgb += o.Albedo * lm;
	#endif
	c.a = o.Alpha;
	#endif // !LIGHTMAP_OFF
	UNITY_APPLY_FOG(IN.fogCoord, c);
	UNITY_OPAQUE_ALPHA(c.a);
	return c;
}

ENDCG
	}
}

FallBack "Mobile/VertexLit"
}
