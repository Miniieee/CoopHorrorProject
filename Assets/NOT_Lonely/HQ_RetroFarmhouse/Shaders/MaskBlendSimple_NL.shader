// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "NOT_Lonely/MaskBlendSimple_NL"
{
	Properties
	{
		[NoScaleOffset]_MainTex("Albedo", 2D) = "white" {}
		[NoScaleOffset][Normal]_BumpMap("Normal", 2D) = "bump" {}
		[NoScaleOffset]_Masks("Mask (R) AO (G) Heigh (B) Gloss (A)", 2D) = "white" {}
		[NoScaleOffset]_DirtMask("Dirt Mask", 2D) = "white" {}
		[Header (Base Surface)]_ColorA("Color A", Color) = (1,1,1,1)
		_ColorB("Color B", Color) = (1,1,1,1)
		_Glossiness("Gloss", Range( 0 , 1)) = 1
		_NormalPower("Normal Power", Float) = 1
		_AOPower("AO Power", Range( 0 , 2)) = 1
		_Tiling("Tiling", Range( 0 , 30)) = 1
		[Header (Dirt)]_DirtColor("Dirt Color", Color) = (1,1,1,1)
		_DirtTiling("Dirt Tiling", Range( 0 , 30)) = 1
		_DirtMaskContrast("Dirt Mask Contrast", Range( 0 , 5)) = 1
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGINCLUDE
		#include "UnityStandardUtils.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
		};

		uniform float _NormalPower;
		uniform sampler2D _BumpMap;
		uniform float _Tiling;
		uniform float4 _ColorB;
		uniform sampler2D _MainTex;
		uniform float4 _ColorA;
		uniform sampler2D _Masks;
		uniform float4 _DirtColor;
		uniform sampler2D _DirtMask;
		uniform float _DirtTiling;
		uniform float _DirtMaskContrast;
		uniform float _Glossiness;
		uniform float _AOPower;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float3 ase_worldPos = i.worldPos;
			float2 appendResult163 = (float2(ase_worldPos.x , ase_worldPos.y));
			float2 appendResult162 = (float2(ase_worldPos.x , ase_worldPos.z));
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 ase_normWorldNormal = normalize( ase_worldNormal );
			float2 lerpResult168 = lerp( appendResult163 , appendResult162 , abs( ase_normWorldNormal.y ));
			float2 appendResult164 = (float2(ase_worldPos.z , ase_worldPos.y));
			float2 lerpResult169 = lerp( lerpResult168 , appendResult164 , abs( ase_normWorldNormal.x ));
			float2 temp_output_170_0 = ( ( 0.4 * _Tiling ) * lerpResult169 );
			o.Normal = UnpackScaleNormal( tex2D( _BumpMap, temp_output_170_0 ), _NormalPower );
			float4 tex2DNode1 = tex2D( _MainTex, temp_output_170_0 );
			float4 tex2DNode182 = tex2D( _Masks, temp_output_170_0 );
			float4 lerpResult184 = lerp( ( _ColorB * tex2DNode1 ) , ( _ColorA * tex2DNode1 ) , tex2DNode182.r);
			float clampResult199 = clamp( ( tex2D( _DirtMask, ( ( _DirtTiling * 0.4 ) * lerpResult169 ) ).r + ( 1.0 - pow( tex2DNode182.g , 2.0 ) ) ) , 0.0 , 1.0 );
			float temp_output_194_0 = ( _DirtColor.a * pow( clampResult199 , _DirtMaskContrast ) );
			float4 lerpResult193 = lerp( lerpResult184 , _DirtColor , temp_output_194_0);
			o.Albedo = lerpResult193.rgb;
			o.Smoothness = ( ( tex2DNode182.a * _Glossiness ) - temp_output_194_0 );
			o.Occlusion = pow( tex2DNode182.g , _AOPower );
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float4 tSpace0 : TEXCOORD1;
				float4 tSpace1 : TEXCOORD2;
				float4 tSpace2 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Standard"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16200
21;602;1906;1124;2474.787;-142.1415;2.131653;True;True
Node;AmplifyShaderEditor.WorldNormalVector;161;-1536.556,504.9592;Float;False;True;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldPosInputsNode;160;-1720.415,943.4261;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DynamicAppendNode;163;-1410.398,1101.16;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.AbsOpNode;167;-1229.542,571.8855;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;162;-1400.153,934.3201;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;172;-724.8002,268.3427;Float;False;Constant;_Float0;Float 0;26;0;Create;True;0;0;False;0;0.4;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;164;-1413.104,1258.048;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LerpOp;168;-940.0516,799.0415;Float;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;190;-852.8267,451.9387;Float;False;Property;_Tiling;Tiling;9;0;Create;True;0;0;False;0;1;1;0;30;0;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;166;-1231.368,484.2669;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;189;-522.4008,394.3927;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;169;-622.7202,803.6156;Float;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;170;-184.6035,645.4871;Float;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;187;-1205.307,-237.8339;Float;True;Property;_Masks;Mask (R) AO (G) Heigh (B) Gloss (A);2;1;[NoScaleOffset];Create;False;0;0;False;0;None;74f1431d8ec87624081bb25c8e008c02;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.RangedFloatNode;41;-824.3782,175.2955;Float;False;Property;_DirtTiling;Dirt Tiling;11;0;Create;True;0;0;False;0;1;5;0;30;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;171;-507.6705,156.1612;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;182;-6.398018,985.7859;Float;True;Property;_MaskAOHeighGloss;Mask AO Heigh Gloss;7;0;Create;True;0;0;False;0;None;74f1431d8ec87624081bb25c8e008c02;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;201;183.3206,144.4533;Float;False;Constant;_Float1;Float 1;12;0;Create;True;0;0;False;0;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;47;-1.825317,-852.2652;Float;False;991.1719;427.0242;Dirt mask;5;59;199;58;197;196;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;188;-180.4661,-11.0833;Float;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PowerNode;200;417.8203,13.94904;Float;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;196;337.6552,-523.0803;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;37;48.59913,-368.6944;Float;True;Property;_DirtMask;Dirt Mask;3;1;[NoScaleOffset];Create;True;0;0;False;0;None;16a6cc839b633034ebadb6c03cfc10f6;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;197;507.1065,-705.9249;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;186;695.5864,-248.2441;Float;False;Property;_ColorB;Color B;5;0;Create;True;0;0;False;0;1,1,1,1;1,1,1,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;1;607.1343,348.2296;Float;True;Property;_MainTex;Albedo;0;1;[NoScaleOffset];Create;False;0;0;False;0;None;a63921ee1c1056543a2ce53cd9055e2e;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;58;22.51693,-512.7426;Float;False;Property;_DirtMaskContrast;Dirt Mask Contrast;12;0;Create;True;0;0;False;0;1;1.63;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;8;709.7004,75.95763;Float;False;Property;_ColorA;Color A;4;0;Create;True;0;0;False;1;Header (Base Surface);1,1,1,1;1,1,1,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;199;641.0674,-694.0423;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;185;1069.162,-152.1186;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;1078.783,77.39339;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;192;833.4305,-1100.134;Float;False;Property;_DirtColor;Dirt Color;10;0;Create;True;0;0;False;1;Header (Dirt);1,1,1,1;0.2205881,0.1659109,0.06812275,0.428;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;59;815.4544,-704.1683;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;63;952.2121,1240.041;Float;False;Property;_Glossiness;Gloss;6;0;Create;False;0;0;False;0;1;0.978;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;184;1472.068,-104.4428;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;202;1273.265,949.6199;Float;False;Property;_AOPower;AO Power;8;0;Create;True;0;0;False;0;1;0.5;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;83;517.2837,830.6694;Float;False;Property;_NormalPower;Normal Power;7;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;64;1302.79,1123.469;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;194;1130.978,-448.0105;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;195;1848.307,841.184;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;193;1842.675,-164.5383;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;3;809.4232,702.7311;Float;True;Property;_BumpMap;Normal;1;2;[NoScaleOffset];[Normal];Create;False;0;0;False;0;None;e59e1d67cf3cefd4f9e0ac138f8812e9;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;203;1591.312,766.8506;Float;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;2277.77,23.9218;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;NOT_Lonely/MaskBlendSimple_NL;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;Standard;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;163;0;160;1
WireConnection;163;1;160;2
WireConnection;167;0;161;2
WireConnection;162;0;160;1
WireConnection;162;1;160;3
WireConnection;164;0;160;3
WireConnection;164;1;160;2
WireConnection;168;0;163;0
WireConnection;168;1;162;0
WireConnection;168;2;167;0
WireConnection;166;0;161;1
WireConnection;189;0;172;0
WireConnection;189;1;190;0
WireConnection;169;0;168;0
WireConnection;169;1;164;0
WireConnection;169;2;166;0
WireConnection;170;0;189;0
WireConnection;170;1;169;0
WireConnection;171;0;41;0
WireConnection;171;1;172;0
WireConnection;182;0;187;0
WireConnection;182;1;170;0
WireConnection;188;0;171;0
WireConnection;188;1;169;0
WireConnection;200;0;182;2
WireConnection;200;1;201;0
WireConnection;196;0;200;0
WireConnection;37;1;188;0
WireConnection;197;0;37;1
WireConnection;197;1;196;0
WireConnection;1;1;170;0
WireConnection;199;0;197;0
WireConnection;185;0;186;0
WireConnection;185;1;1;0
WireConnection;7;0;8;0
WireConnection;7;1;1;0
WireConnection;59;0;199;0
WireConnection;59;1;58;0
WireConnection;184;0;185;0
WireConnection;184;1;7;0
WireConnection;184;2;182;1
WireConnection;64;0;182;4
WireConnection;64;1;63;0
WireConnection;194;0;192;4
WireConnection;194;1;59;0
WireConnection;195;0;64;0
WireConnection;195;1;194;0
WireConnection;193;0;184;0
WireConnection;193;1;192;0
WireConnection;193;2;194;0
WireConnection;3;1;170;0
WireConnection;3;5;83;0
WireConnection;203;0;182;2
WireConnection;203;1;202;0
WireConnection;0;0;193;0
WireConnection;0;1;3;0
WireConnection;0;4;195;0
WireConnection;0;5;203;0
ASEEND*/
//CHKSM=956D74F22AE2858E03876A563F374037965C8F93