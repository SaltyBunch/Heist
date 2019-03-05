// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/FogOfWar"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_FogRadius("FogRadius", Float) = 1.0
		_FogMaxRadius("FogMaxRadius", Float) = 0.5
		_player1_Pos("_Player1_Pos", Vector) = ( 0,0,0,1 )
		_player2_Pos("_Player2_Pos", Vector) = ( 0,0,0,1 )
		_player3_Pos("_Player3_Pos", Vector) = ( 0,0,0,1 )
		_player4_Pos("_Player4_Pos", Vector) = ( 0,0,0,1 )
    }
    SubShader
    {
        Tags { "Queue" = "Transparent"  "IgnoreProjector" = "True" "RenderType" = "transparent"}
        LOD 200
		Blend SrcAlpha OneMinusSrcAlpha
		Cull off

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Lambert vertex:vert alpha:blend

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

		float	_FogRadius;
		float	_FogMaxRadius;
		float4	_player1_Pos;
		float4	_player2_Pos;
		float4	_player3_Pos;
		float4	_player4_Pos;

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

        struct Input
        {
            float2 uv_MainTex;
			float2 location;
        };
		float powerForPos(float4 pos, float2 nearVertex);
        

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

		void vert(inout appdata_full vertexData, out Input outData) 
		{
			float4 pos = UnityObjectToClipPos(vertexData.vertex);
			float4 posWorld = mul(unity_ObjectToWorld, vertexData.vertex);
			outData.uv_MainTex = vertexData.texcoord;
			outData.location = posWorld.xz;
		}



        void surf (Input IN, inout SurfaceOutput o)
        {
            fixed4 baseColor = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            
			float alpha = (1.0 - (baseColor.a + powerForPos(_player1_Pos, IN.location) + powerForPos(_player2_Pos, IN.location)
				+ powerForPos(_player3_Pos, IN.location) + powerForPos(_player4_Pos, IN.location)));


			o.Albedo = baseColor.rgb;
			o.Alpha = alpha;
        }

		//return 0 if (pos - nearVertex) > _FogRadius
		float powerForPos(float4 pos, float2 nearVertex) 
		{
			float atten = clamp(_FogRadius - length(pos.xz - nearVertex.xy),0.0,_FogRadius);

			return (1.0/_FogMaxRadius) *atten / _FogRadius;
		}

        ENDCG
    }
    FallBack "Transparent/VertexLit"
}
