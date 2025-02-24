// Upgrade NOTE: upgraded instancing buffer 'NOT_LonelyNOT_Lonely_RotateUVs' to new syntax.

// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "NOT_Lonely/NOT_Lonely_RotateUVs"
{
	Properties
	{
		[NoScaleOffset]_MainTex("Albedo (Smothness A)", 2D) = "white" {}
		[NoScaleOffset][Normal]_BumpMap("Normal Map", 2D) = "bump" {}
		_BumpScale("Normal Intensity", Float) = 1
		[NoScaleOffset]_OcclusionMap("Ambient Occlusion", 2D) = "white" {}
		_TilingX("Tiling X", Float) = 1
		_TilingY("Tiling Y", Float) = 1
		_Angle("Angle", Range( 0 , 360)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#pragma target 3.0
		#pragma multi_compile_instancing
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float _BumpScale;
		uniform sampler2D _BumpMap;
		uniform float _TilingX;
		uniform float _TilingY;
		uniform sampler2D _MainTex;
		uniform sampler2D _OcclusionMap;

		UNITY_INSTANCING_BUFFER_START(NOT_LonelyNOT_Lonely_RotateUVs)
			UNITY_DEFINE_INSTANCED_PROP(float, _Angle)
#define _Angle_arr NOT_LonelyNOT_Lonely_RotateUVs
		UNITY_INSTANCING_BUFFER_END(NOT_LonelyNOT_Lonely_RotateUVs)

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 appendResult33 = (float2(_TilingX , _TilingY));
			float2 uv_TexCoord26 = i.uv_texcoord * appendResult33;
			float2 temp_cast_0 = (0.0).xx;
			float _Angle_Instance = UNITY_ACCESS_INSTANCED_PROP(_Angle_arr, _Angle);
			float cos20 = cos( ( ( _Angle_Instance / 180 ) * UNITY_PI ) );
			float sin20 = sin( ( ( _Angle_Instance / 180 ) * UNITY_PI ) );
			float2 rotator20 = mul( uv_TexCoord26 - temp_cast_0 , float2x2( cos20 , -sin20 , sin20 , cos20 )) + temp_cast_0;
			o.Normal = UnpackScaleNormal( tex2D( _BumpMap, rotator20 ), _BumpScale );
			float4 tex2DNode21 = tex2D( _MainTex, rotator20 );
			o.Albedo = tex2DNode21.rgb;
			o.Smoothness = tex2DNode21.a;
			o.Occlusion = tex2D( _OcclusionMap, rotator20 ).r;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Standard"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15600
2205;179;1380;974;2116.282;626.6803;1;True;False
Node;AmplifyShaderEditor.RangedFloatNode;6;-1596.729,167.8227;Float;False;InstancedProperty;_Angle;Angle;6;0;Create;True;0;0;False;0;0;0;0;360;0;1;FLOAT;0
Node;AmplifyShaderEditor.IntNode;4;-1489.866,271.9131;Float;False;Constant;_Int0;Int 0;5;0;Create;True;0;0;False;0;180;0;0;1;INT;0
Node;AmplifyShaderEditor.RangedFloatNode;36;-1627.057,-143.2831;Float;False;Property;_TilingY;Tiling Y;5;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;35;-1627.057,-266.2831;Float;False;Property;_TilingX;Tiling X;4;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;10;-1280.553,166.0996;Float;False;2;0;FLOAT;0;False;1;INT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;33;-1425.745,-200.7659;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PiNode;15;-1115.328,167.2227;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;30;-1085.088,6.993164;Float;False;Constant;_Float1;Float 1;9;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;18;-909.5443,151.7768;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;26;-1168.149,-137.7932;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;23;-797.639,227.3793;Float;False;Property;_BumpScale;Normal Intensity;2;0;Create;False;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RotatorNode;20;-858.2312,-11.40753;Float;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;22;-458.5396,180.4787;Float;True;Property;_BumpMap;Normal Map;1;2;[NoScaleOffset];[Normal];Create;False;0;0;False;0;None;c0ef825bdc654554cbc2af46ffe2b7a5;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;25;-456.2405,398.1795;Float;True;Property;_OcclusionMap;Ambient Occlusion;3;1;[NoScaleOffset];Create;False;0;0;False;0;None;db8955747a03fd04791ca093b5a0aec0;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;21;-458.2274,-36.24937;Float;True;Property;_MainTex;Albedo (Smothness A);0;1;[NoScaleOffset];Create;False;0;0;False;0;None;48ff34383e60440458b86d7c7c5306f2;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;230.0884,1.300435;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;NOT_Lonely/NOT_Lonely_RotateUVs;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;Standard;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;10;0;6;0
WireConnection;10;1;4;0
WireConnection;33;0;35;0
WireConnection;33;1;36;0
WireConnection;15;0;10;0
WireConnection;18;0;15;0
WireConnection;26;0;33;0
WireConnection;20;0;26;0
WireConnection;20;1;30;0
WireConnection;20;2;18;0
WireConnection;22;1;20;0
WireConnection;22;5;23;0
WireConnection;25;1;20;0
WireConnection;21;1;20;0
WireConnection;0;0;21;0
WireConnection;0;1;22;0
WireConnection;0;4;21;4
WireConnection;0;5;25;1
ASEEND*/
//CHKSM=CD76053438848340BCB7D054C68DD1E5F6C9633B