// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "NOT_Lonely/TranslucentAnim_NL"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("BaseColor", 2D) = "white" {}
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_Normal("Normal", 2D) = "bump" {}
		_Metallic("Metallic(R) Smoothness(A)", 2D) = "black" {}
		_Occlusion("Occlusion", 2D) = "white" {}
		_Smoothness("Smoothness", Range( 0 , 1)) = 1
		_Noise("Noise", 2D) = "white" {}
		_NoiseTiling("Noise Tiling", Float) = 0.25
		_Speed("Speed", Float) = 1
		_Amplitude("Amplitude", Float) = 1
		_NoiseSpeed("Noise Speed", Range( 0 , 1)) = 0.1
		_TurbulenceAmplitude("Turbulence Amplitude", Range( 0.001 , 10)) = 1
		[Header(Translucency)]
		_Translucency("Strength", Range( 0 , 50)) = 1
		_TransNormalDistortion("Normal Distortion", Range( 0 , 1)) = 0.1
		_TransScattering("Scaterring Falloff", Range( 1 , 50)) = 2
		_TransDirect("Direct", Range( 0 , 1)) = 1
		_TransAmbient("Ambient", Range( 0 , 1)) = 0.2
		_TransShadow("Shadow", Range( 0 , 1)) = 0.9
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" }
		Cull Off
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#pragma target 3.0
		#pragma multi_compile_instancing
		#pragma surface surf StandardCustom keepalpha addshadow fullforwardshadows exclude_path:deferred nodirlightmap dithercrossfade vertex:vertexDataFunc 
		struct Input
		{
			float3 worldPos;
			float2 uv_texcoord;
			half ASEVFace : VFACE;
			float4 vertexColor : COLOR;
		};

		struct SurfaceOutputStandardCustom
		{
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			half Alpha;
			half3 Translucency;
		};

		uniform float _Amplitude;
		uniform half _Speed;
		uniform sampler2D _Noise;
		uniform half _NoiseTiling;
		uniform float _NoiseSpeed;
		uniform float _TurbulenceAmplitude;
		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform float4 _Color;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform sampler2D _Metallic;
		uniform float4 _Metallic_ST;
		uniform half _Smoothness;
		uniform sampler2D _Occlusion;
		uniform float4 _Occlusion_ST;
		uniform half _Translucency;
		uniform half _TransNormalDistortion;
		uniform half _TransScattering;
		uniform half _TransDirect;
		uniform half _TransAmbient;
		uniform half _TransShadow;
		uniform float _Cutoff = 0.5;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			float4 appendResult14 = (float4(ase_worldPos.x , 0.0 , ase_worldPos.z , 0.0));
			float mulTime13 = _Time.y * _Speed;
			float2 appendResult50 = (float2(ase_worldPos.x , ase_worldPos.z));
			float mulTime33 = _Time.y * _NoiseSpeed;
			float2 temp_cast_0 = (mulTime33).xx;
			float2 uv_TexCoord51 = v.texcoord.xy * ( appendResult50 * ( _NoiseTiling * 0.01 ) ) + temp_cast_0;
			v.vertex.xyz += ( v.color.g * ( ( sin( appendResult14 ) * ( _Amplitude * cos( ( appendResult14 + mulTime13 ) ) ) ) * (0.0 + (tex2Dlod( _Noise, float4( uv_TexCoord51, 0, 0.0) ).r - 0.0) * (_TurbulenceAmplitude - 0.0) / (1.0 - 0.0)) ) ).xyz;
		}

		inline half4 LightingStandardCustom(SurfaceOutputStandardCustom s, half3 viewDir, UnityGI gi )
		{
			#if !DIRECTIONAL
			float3 lightAtten = gi.light.color;
			#else
			float3 lightAtten = lerp( _LightColor0.rgb, gi.light.color, _TransShadow );
			#endif
			half3 lightDir = gi.light.dir + s.Normal * _TransNormalDistortion;
			half transVdotL = pow( saturate( dot( viewDir, -lightDir ) ), _TransScattering );
			half3 translucency = lightAtten * (transVdotL * _TransDirect + gi.indirect.diffuse * _TransAmbient) * s.Translucency;
			half4 c = half4( s.Albedo * translucency * _Translucency, 0 );

			SurfaceOutputStandard r;
			r.Albedo = s.Albedo;
			r.Normal = s.Normal;
			r.Emission = s.Emission;
			r.Metallic = s.Metallic;
			r.Smoothness = s.Smoothness;
			r.Occlusion = s.Occlusion;
			r.Alpha = s.Alpha;
			return LightingStandard (r, viewDir, gi) + c;
		}

		inline void LightingStandardCustom_GI(SurfaceOutputStandardCustom s, UnityGIInput data, inout UnityGI gi )
		{
			#if defined(UNITY_PASS_DEFERRED) && UNITY_ENABLE_REFLECTION_BUFFERS
				gi = UnityGlobalIllumination(data, s.Occlusion, s.Normal);
			#else
				UNITY_GLOSSY_ENV_FROM_SURFACE( g, s, data );
				gi = UnityGlobalIllumination( data, s.Occlusion, s.Normal, g );
			#endif
		}

		void surf( Input i , inout SurfaceOutputStandardCustom o )
		{
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			float3 tex2DNode4 = UnpackNormal( tex2D( _Normal, uv_Normal ) );
			float3 appendResult10 = (float3(tex2DNode4.r , tex2DNode4.g , ( tex2DNode4.b * -1.0 )));
			float3 switchResult6 = (((i.ASEVFace>0)?(tex2DNode4):(appendResult10)));
			o.Normal = switchResult6;
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float4 tex2DNode2 = tex2D( _MainTex, uv_MainTex );
			float4 temp_output_58_0 = ( _Color * tex2DNode2 );
			o.Albedo = temp_output_58_0.rgb;
			float2 uv_Metallic = i.uv_texcoord * _Metallic_ST.xy + _Metallic_ST.zw;
			float4 tex2DNode3 = tex2D( _Metallic, uv_Metallic );
			o.Metallic = tex2DNode3.r;
			o.Smoothness = ( tex2DNode3.a * _Smoothness );
			float2 uv_Occlusion = i.uv_texcoord * _Occlusion_ST.xy + _Occlusion_ST.zw;
			float4 tex2DNode59 = tex2D( _Occlusion, uv_Occlusion );
			float switchResult66 = (((i.ASEVFace>0)?(tex2DNode59.r):(1.0)));
			o.Occlusion = switchResult66;
			o.Translucency = ( ( temp_output_58_0 * tex2DNode59.r ) * i.vertexColor.r ).rgb;
			o.Alpha = 1;
			clip( tex2DNode2.a - _Cutoff );
		}

		ENDCG
	}
	Fallback "Standard"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17000
1927;1;1906;1010;1027.208;418.4582;1;True;True
Node;AmplifyShaderEditor.RangedFloatNode;11;-2032,816;Half;False;Property;_Speed;Speed;9;0;Create;True;0;0;False;0;1;0.3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;12;-1840,592;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;55;-2124.587,1319.216;Float;False;Constant;_TilingMultiplier;TilingMultiplier;11;0;Create;True;0;0;False;0;0.01;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;45;-2126.083,984.0314;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;46;-2124.38,1192.736;Half;False;Property;_NoiseTiling;Noise Tiling;8;0;Create;True;0;0;False;0;0.25;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;29;-1814.027,1315.715;Float;False;Property;_NoiseSpeed;Noise Speed;11;0;Create;True;0;0;False;0;0.1;0.01;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;54;-1825.488,1162.516;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;13;-1824,784;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;50;-1846.199,1003.094;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;14;-1552,560;Float;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleTimeNode;33;-1472.942,1279.665;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;47;-1610.753,1100.123;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;15;-1376,688;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;17;-1040,704;Float;False;Property;_Amplitude;Amplitude;10;0;Create;True;0;0;False;0;1;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CosOpNode;16;-1168,816;Float;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;51;-1259.439,1109.106;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;56;-866.8317,1338.15;Float;False;Property;_TurbulenceAmplitude;Turbulence Amplitude;12;0;Create;True;0;0;False;0;1;0.01;0.001;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;19;-1168,544;Float;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;8;-1589.715,383.6859;Half;False;Constant;_Float0;Float 0;5;0;Create;True;0;0;False;0;-1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;18;-816,784;Float;False;2;2;0;FLOAT;0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SamplerNode;25;-975.0089,1073.845;Float;True;Property;_Noise;Noise;7;0;Create;True;0;0;False;0;None;bdbe94d7623ec3940947b62544306f1c;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;4;-1760.932,111.0896;Float;True;Property;_Normal;Normal;3;0;Create;True;0;0;False;0;None;61243fe42094beb4680f087931d6084b;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;57;-906.0193,-698.0734;Float;False;Property;_Color;Color;0;0;Create;True;0;0;False;0;1,1,1,1;1,1,1,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;2;-1034.699,-429.2002;Float;True;Property;_MainTex;BaseColor;1;0;Create;False;0;0;False;0;None;b0aa017d4419e0748a5604f2343d4180;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;59;-693.0987,-102.3081;Float;True;Property;_Occlusion;Occlusion;5;0;Create;True;0;0;False;0;None;f66719d2c050be04c839762a3e6678bc;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;-544,688;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TFHCRemapNode;34;-517.84,1107.325;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-1373.012,265.7027;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;58;-607.0189,-422.4738;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;24;-515.7805,335.491;Half;False;Property;_Smoothness;Smoothness;6;0;Create;True;0;0;False;0;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;10;-1183.579,149.4857;Float;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.VertexColorNode;20;-637.886,473.3108;Float;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;3;-929.4749,204.0063;Float;True;Property;_Metallic;Metallic(R) Smoothness(A);4;0;Create;False;0;0;False;0;None;27ca4f68f89f85c42b4a382e0a243e60;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;60;-100.0862,-63.70881;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;44;-322.3368,772.2783;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;67;-458.6836,123.5427;Float;False;Constant;_Float1;Float 1;14;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;23;-242.7805,238.491;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SwitchByFaceNode;66;-265.6836,73.54272;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;62;81.8589,68.17059;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SwitchByFaceNode;6;-1014.487,17.95301;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;22;-170.336,530.4127;Float;False;2;2;0;FLOAT;0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;281.1494,-9.284587;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;NOT_Lonely/TranslucentAnim_NL;False;False;False;False;False;False;False;False;True;False;False;False;True;False;False;False;True;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Masked;0.5;True;True;0;False;TransparentCutout;;AlphaTest;ForwardOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;Standard;2;13;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;54;0;46;0
WireConnection;54;1;55;0
WireConnection;13;0;11;0
WireConnection;50;0;45;1
WireConnection;50;1;45;3
WireConnection;14;0;12;1
WireConnection;14;2;12;3
WireConnection;33;0;29;0
WireConnection;47;0;50;0
WireConnection;47;1;54;0
WireConnection;15;0;14;0
WireConnection;15;1;13;0
WireConnection;16;0;15;0
WireConnection;51;0;47;0
WireConnection;51;1;33;0
WireConnection;19;0;14;0
WireConnection;18;0;17;0
WireConnection;18;1;16;0
WireConnection;25;1;51;0
WireConnection;21;0;19;0
WireConnection;21;1;18;0
WireConnection;34;0;25;1
WireConnection;34;4;56;0
WireConnection;7;0;4;3
WireConnection;7;1;8;0
WireConnection;58;0;57;0
WireConnection;58;1;2;0
WireConnection;10;0;4;1
WireConnection;10;1;4;2
WireConnection;10;2;7;0
WireConnection;60;0;58;0
WireConnection;60;1;59;1
WireConnection;44;0;21;0
WireConnection;44;1;34;0
WireConnection;23;0;3;4
WireConnection;23;1;24;0
WireConnection;66;0;59;1
WireConnection;66;1;67;0
WireConnection;62;0;60;0
WireConnection;62;1;20;1
WireConnection;6;0;4;0
WireConnection;6;1;10;0
WireConnection;22;0;20;2
WireConnection;22;1;44;0
WireConnection;0;0;58;0
WireConnection;0;1;6;0
WireConnection;0;3;3;1
WireConnection;0;4;23;0
WireConnection;0;5;66;0
WireConnection;0;7;62;0
WireConnection;0;10;2;4
WireConnection;0;11;22;0
ASEEND*/
//CHKSM=99AA9C4A2DF3053E80172A9715F5EF4346157AD0