Shader "Custom/Texture Blend Alpha"
{
	Properties
	{
		_MainTex("Main (RGB)", 2D) = "black" {}
		_SecTex("Main (RGB)", 2D) = "white" {}
		_FilTex("Filter (Alpha)", 2D) = "black"{}
	}

	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _MainTex;
		sampler2D _SecTex;
		sampler2D _FilTex;

		struct Input
		{
			float2 uv_MainTex;
			float2 uv_SecTex;
			float2 uv_FilTex;
		};

		void surf(Input IN, inout SurfaceOutput o)
		{
			fixed4 main = tex2D(_MainTex, IN.uv_MainTex);
			fixed4 sec = tex2D(_SecTex, IN.uv_SecTex);
			fixed4 filter = tex2D(_FilTex, IN.uv_FilTex);

			o.Albedo = lerp(main.rgb, sec.rgb, filter.a);

			o.Alpha = main.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
