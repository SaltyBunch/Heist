// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/StandardOccluded-Other-Trans"
{
	Properties{
		[PerRendererData]_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
	    _Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_AlphaCutoff("Cutoff", Range(0,1)) = 0.5
	}
		SubShader{

		Pass
	{
		Tags{ "RenderType" = "Opaque" "Queue" = "Geometry+1" }
		ZTest Greater
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest

		half4 _OccludedColor;
		half _AlphaCutoff;
		sampler2D _MainTex;
	    fixed4 _Color;

    struct appdata
    {
        float4 vertex : POSITION; // vertex position
        float2 uv : TEXCOORD0; // texture coordinate
    };

    struct v2f
    {
        float2 uv : TEXCOORD0; // texture coordinate
        float4 vertex : SV_POSITION; // clip space position
    };

	v2f vert(appdata v)
	{
	    v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
        o.uv = v.uv;
		return o;
	}

		half4 frag(v2f i) : COLOR
	{
	    fixed4 c = tex2D(_MainTex, i.uv) * _Color;
		return _Color*c.a;
	}

		ENDCG
	}

		Tags{ "RenderType" = "Transparent" "Queue" = "Transparent+1" }
		LOD 200
		ZWrite On
		ZTest LEqual
        Lighting Off

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
#pragma target 3.0

		sampler2D _MainTex;

	struct Input {
		float2 uv_MainTex;
	};

	half _Glossiness;
	half _Metallic;
	half _AlphaCutoff;
	fixed4 _Color;

	void surf(Input IN, inout SurfaceOutputStandard o) {
		// Albedo comes from a texture tinted by color
		fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
		o.Albedo = c.rgb;
		// Metallic and smoothness come from slider variables
		o.Metallic = _Metallic;
		o.Smoothness = _Glossiness;
		o.Emission = c.rgb*c.a;
		clip(c.a-_AlphaCutoff);
	}
	ENDCG
	}
		FallBack "Diffuse"
}