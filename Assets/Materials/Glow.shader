Shader "Custom/Glow" 
{
	Properties {
		_Color("Color", Color) = (1,1,1,1)
		_Intensity("Intensity", Range(1.0, 5.0)) = 2.0
	}
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		
		CGPROGRAM
		#pragma surface surf Lambert

		float4 _Color;
		float _Intensity;

		struct Input 
		{	
			float3 viewDir;
		};
		
		void surf (Input IN, inout SurfaceOutput o) 
		{
			half rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
			o.Emission = _Color.rgb * pow(rim, _Intensity);
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
