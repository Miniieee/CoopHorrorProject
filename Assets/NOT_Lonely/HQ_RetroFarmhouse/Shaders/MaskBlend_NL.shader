// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "MaskBlend_NL"
{
	Properties
	{
		[Toggle]_ShowVertexColorsDebug("Show Vertex Colors (Debug)", Float) = 0
		[Toggle]_ShowVertexAlpha("Show Vertex Alpha", Float) = 0
		[Header (First Layer (bottom))]_Color("Color", Color) = (1,1,1,1)
		[NoScaleOffset]_MainTex("Albedo", 2D) = "white" {}
		[NoScaleOffset][Normal]_BumpMap("Normal", 2D) = "bump" {}
		_NormalPower("Normal Power", Float) = 1
		_Tiling("Tiling (X, Y)", Vector) = (1,1,0,0)
		_Offset("Offset (X, Y)", Vector) = (0,0,0,0)
		_Glossiness("Gloss", Range( 0 , 1)) = 1
		[NoScaleOffset][Header (Second Layer (use ALPHA vertex color channel to erase 2nd layer))]_LayerMasks("Mask_01 (R), Mask_02 (G), Dirt Mask (B)", 2D) = "black" {}
		[NoScaleOffset]_AOUV2("AO (UV2)", 2D) = "white" {}
		_Mask_01Tiling("Mask_01 Tiling", Range( 0.01 , 2)) = 1
		[NoScaleOffset]_SecondBaseColor("Albedo", 2D) = "white" {}
		[NoScaleOffset][Normal]_SecondNormal("Normal", 2D) = "bump" {}
		_NormalPowerSecondLayer("Normal Power", Float) = 1
		_NormalBlend("Normal Blend", Range( 0 , 1)) = 0.3
		_TilingTopLayerAlbedo("Tiling (X, Y)", Vector) = (1,1,0,0)
		_OffsetTopLayerAlbedo("Offset (X, Y)", Vector) = (0,0,0,0)
		_DetailColor("Color", Color) = (1,1,1,1)
		_Gloss("Gloss", Range( 0 , 1)) = 1
		_BlendContrast("Blend Contrast", Range( 0 , 1)) = 0
		_TopLayerAmount("Mask_01 wear (Triplanar)", Range( 0 , 1)) = 0
		_Mask_02wearUV0("Mask_02 wear (UV0)", Range( 0 , 1)) = 0
		_Mask_AO("Mask_AO", Range( 0 , 1)) = 0
		[Header(Dirt (use BLUE vertex color channel for painting))]_DirtColor("Dirt Color", Color) = (1,1,1,0.7843137)
		_DirtTiling("Dirt Tiling", Range( 0 , 30)) = 1
		_DirtMaskContrast("Dirt Mask Contrast", Range( 0 , 5)) = 1
		_AODirt("AO Dirt", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] _texcoord4( "", 2D ) = "white" {}
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
			float2 uv_texcoord;
			float2 uv4_texcoord4;
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
			float4 vertexColor : COLOR;
		};

		uniform float _NormalPower;
		uniform sampler2D _BumpMap;
		uniform float2 _Tiling;
		uniform float2 _Offset;
		uniform float _NormalPowerSecondLayer;
		uniform sampler2D _SecondNormal;
		uniform float2 _TilingTopLayerAlbedo;
		uniform float2 _OffsetTopLayerAlbedo;
		uniform float _Mask_AO;
		uniform sampler2D _AOUV2;
		uniform sampler2D _LayerMasks;
		uniform float _Mask_01Tiling;
		uniform float _TopLayerAmount;
		uniform float _Mask_02wearUV0;
		uniform float _BlendContrast;
		uniform float _NormalBlend;
		uniform float _ShowVertexAlpha;
		uniform float _ShowVertexColorsDebug;
		uniform float4 _Color;
		uniform sampler2D _MainTex;
		uniform sampler2D _SecondBaseColor;
		uniform float4 _DetailColor;
		uniform float4 _DirtColor;
		uniform float _AODirt;
		uniform float _DirtTiling;
		uniform float _DirtMaskContrast;
		uniform float _Glossiness;
		uniform float _Gloss;


		inline float4 TriplanarSamplingSF( sampler2D topTexMap, float3 worldPos, float3 worldNormal, float falloff, float tilling, float3 normalScale, float3 index )
		{
			float3 projNormal = ( pow( abs( worldNormal ), falloff ) );
			projNormal /= ( projNormal.x + projNormal.y + projNormal.z ) + 0.00001;
			float3 nsign = sign( worldNormal );
			half4 xNorm; half4 yNorm; half4 zNorm;
			xNorm = ( tex2D( topTexMap, tilling * worldPos.zy * float2( nsign.x, 1.0 ) ) );
			yNorm = ( tex2D( topTexMap, tilling * worldPos.xz * float2( nsign.y, 1.0 ) ) );
			zNorm = ( tex2D( topTexMap, tilling * worldPos.xy * float2( -nsign.z, 1.0 ) ) );
			return xNorm * projNormal.x + yNorm * projNormal.y + zNorm * projNormal.z;
		}


		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_TexCoord4 = i.uv_texcoord * _Tiling + _Offset;
			float2 uv_TexCoord55 = i.uv_texcoord * _TilingTopLayerAlbedo + _OffsetTopLayerAlbedo;
			float2 uv4_AOUV2180 = i.uv4_texcoord4;
			float4 tex2DNode180 = tex2D( _AOUV2, uv4_AOUV2180 );
			float temp_output_147_0 = ( 1.0 - ( _Mask_AO * tex2DNode180.r ) );
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float4 triplanar156 = TriplanarSamplingSF( _LayerMasks, ase_worldPos, ase_worldNormal, 1.0, _Mask_01Tiling, 1.0, 0 );
			float temp_output_106_0 = ( ( 1.0 - tex2D( _LayerMasks, uv_TexCoord4 ).g ) * ( 1.0 - ( triplanar156.x * (-0.5 + (_TopLayerAmount - 0.0) * (1.0 - -0.5) / (1.0 - 0.0)) ) ) );
			float HeightMask90 = saturate(pow(((( temp_output_147_0 + temp_output_106_0 )*( ( temp_output_147_0 + ( ( ( 1.0 - (-0.5 + (_Mask_02wearUV0 - 0.0) * (1.0 - -0.5) / (1.0 - 0.0)) ) * temp_output_106_0 ) * i.vertexColor.a ) ) * i.vertexColor.a ))*4)+(( ( temp_output_147_0 + ( ( ( 1.0 - (-0.5 + (_Mask_02wearUV0 - 0.0) * (1.0 - -0.5) / (1.0 - 0.0)) ) * temp_output_106_0 ) * i.vertexColor.a ) ) * i.vertexColor.a )*2),(0.001 + (_BlendContrast - 0.0) * (150.0 - 0.001) / (1.0 - 0.0))));
			float3 lerpResult75 = lerp( UnpackScaleNormal( tex2D( _BumpMap, uv_TexCoord4 ), _NormalPower ) , UnpackScaleNormal( tex2D( _SecondNormal, uv_TexCoord55 ), _NormalPowerSecondLayer ) , ( HeightMask90 * _NormalBlend ));
			o.Normal = lerpResult75;
			float4 tex2DNode1 = tex2D( _MainTex, uv_TexCoord4 );
			float4 lerpResult6 = lerp( ( ( _Color * tex2DNode1 ) * i.vertexColor.g ) , ( tex2D( _SecondBaseColor, uv_TexCoord55 ) * _DetailColor ) , HeightMask90);
			float2 appendResult163 = (float2(ase_worldPos.x , ase_worldPos.y));
			float2 appendResult162 = (float2(ase_worldPos.x , ase_worldPos.z));
			float3 ase_normWorldNormal = normalize( ase_worldNormal );
			float2 lerpResult168 = lerp( appendResult163 , appendResult162 , abs( ase_normWorldNormal.y ));
			float2 appendResult164 = (float2(ase_worldPos.z , ase_worldPos.y));
			float2 lerpResult169 = lerp( lerpResult168 , appendResult164 , abs( ase_normWorldNormal.x ));
			float clampResult155 = clamp( ( ( ( 1.0 - tex2DNode180.r ) * _AODirt ) + pow( tex2D( _LayerMasks, ( ( _DirtTiling * 0.1 ) * lerpResult169 ) ).b , _DirtMaskContrast ) ) , 0.0 , 1.0 );
			float temp_output_48_0 = ( i.vertexColor.b * clampResult155 * _DirtColor.a );
			float4 lerpResult38 = lerp( lerpResult6 , _DirtColor , temp_output_48_0);
			float4 temp_cast_0 = (i.vertexColor.a).xxxx;
			o.Albedo = lerp(lerp(lerpResult38,i.vertexColor,_ShowVertexColorsDebug),temp_cast_0,_ShowVertexAlpha).rgb;
			float clampResult46 = clamp( ( ( ( tex2DNode1.a * _Glossiness ) + ( HeightMask90 * _Gloss ) ) * ( 1.0 - temp_output_48_0 ) ) , 0.0 , 1.0 );
			o.Smoothness = clampResult46;
			o.Occlusion = tex2DNode180.r;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows dithercrossfade 

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
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float4 customPack1 : TEXCOORD1;
				float4 tSpace0 : TEXCOORD2;
				float4 tSpace1 : TEXCOORD3;
				float4 tSpace2 : TEXCOORD4;
				half4 color : COLOR0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.customPack1.zw = customInputData.uv4_texcoord4;
				o.customPack1.zw = v.texcoord3;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				o.color = v.color;
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
				surfIN.uv_texcoord = IN.customPack1.xy;
				surfIN.uv4_texcoord4 = IN.customPack1.zw;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				surfIN.vertexColor = IN.color;
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
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16200
7;7;1906;1124;2518.588;472.3981;1;True;True
Node;AmplifyShaderEditor.RangedFloatNode;11;-1259.756,955.234;Float;False;Property;_TopLayerAmount;Mask_01 wear (Triplanar);21;0;Create;False;0;0;False;0;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;21;-2237.672,675.4022;Float;True;Property;_LayerMasks;Mask_01 (R), Mask_02 (G), Dirt Mask (B);9;1;[NoScaleOffset];Create;False;0;0;False;1;Header (Second Layer (use ALPHA vertex color channel to erase 2nd layer));None;a713005c3c921c041b68ee5cbb3d09f7;False;black;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.RangedFloatNode;157;-1656.347,399.7981;Float;False;Property;_Mask_01Tiling;Mask_01 Tiling;11;0;Create;True;0;0;False;0;1;0.172;0.01;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;52;-2703.457,411.998;Float;False;Property;_Offset;Offset (X, Y);7;0;Create;False;0;0;False;0;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;51;-2704.594,247.0763;Float;False;Property;_Tiling;Tiling (X, Y);6;0;Create;False;0;0;False;0;1,1;1,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.WorldNormalVector;161;-2799.307,1803.183;Float;False;True;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldPosInputsNode;160;-2893.465,2051.851;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TriplanarNode;156;-1270.982,382.9906;Float;True;Spherical;World;False;Top Texture 0;_TopTexture0;white;-1;None;Mid Texture 0;_MidTexture0;white;-1;None;Bot Texture 0;_BotTexture0;white;-1;None;Triplanar Sampler;False;9;0;SAMPLER2D;;False;5;FLOAT;1;False;1;SAMPLER2D;;False;6;FLOAT;0;False;2;SAMPLER2D;;False;7;FLOAT;0;False;8;FLOAT;1;False;3;FLOAT;1;False;4;FLOAT;1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;47;-2201.653,1349.586;Float;False;1406.536;402.0559;Dirt mask;6;59;37;58;41;171;172;;1,1,1,1;0;0
Node;AmplifyShaderEditor.DynamicAppendNode;163;-2498.718,2179.77;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TFHCRemapNode;139;-904.1377,889.7039;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-0.5;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;4;-2390.409,348.3147;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;162;-2499.104,2051.845;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.AbsOpNode;167;-2492.294,1870.109;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;172;-2063.425,1648.604;Float;False;Constant;_Float0;Float 0;26;0;Create;True;0;0;False;0;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;168;-2213.205,1949.066;Float;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;164;-2497.492,2313.36;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.AbsOpNode;166;-2494.12,1782.491;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;22;-1560.403,639.1815;Float;True;Property;_TextureSample0;Texture Sample 0;3;0;Create;True;0;0;False;0;None;None;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;41;-2151.651,1472.109;Float;False;Property;_DirtTiling;Dirt Tiling;25;0;Create;True;0;0;False;0;1;3.1;0;30;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;25;-1261.222,1107.356;Float;False;Property;_Mask_02wearUV0;Mask_02 wear (UV0);22;0;Create;True;0;0;False;0;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;108;-673.126,910.8967;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;138;-900.1377,1097.704;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-0.5;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;100;-541.7921,668.2032;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;103;-542.5363,774.1003;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;169;-1895.873,1953.64;Float;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;171;-1845.425,1619.604;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;106;-339.4469,695.3091;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;145;-1929.217,-447.1147;Float;False;Property;_Mask_AO;Mask_AO;23;0;Create;True;0;0;False;0;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;180;-2010.221,-81.39465;Float;True;Property;_AOUV2;AO (UV2);10;1;[NoScaleOffset];Create;True;0;0;False;0;None;170498bde693471408c549b01c438001;True;3;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;170;-1677.465,1889.32;Float;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.OneMinusNode;33;-684.0139,1096.577;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;97;-328.9533,429.5049;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;17;-736.7394,459.3931;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;37;-1432.152,1399.586;Float;True;Property;_TextureSample1;Texture Sample 1;7;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;146;-1456.026,-270.9236;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;151;-732.8836,1309.136;Float;False;Property;_AODirt;AO Dirt;27;0;Create;True;0;0;False;0;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;69;-1047.591,-1042.353;Float;False;1342.229;495.8069;Second Layer Maps;7;49;55;56;57;73;81;84;;1,1,1,1;0;0
Node;AmplifyShaderEditor.OneMinusNode;152;-526.7864,1206.584;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;58;-1258.877,1654.309;Float;False;Property;_DirtMaskContrast;Dirt Mask Contrast;26;0;Create;True;0;0;False;0;1;3.66;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;56;-997.5908,-837.5969;Float;False;Property;_OffsetTopLayerAlbedo;Offset (X, Y);17;0;Create;False;0;0;False;0;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;153;-291.0508,1296.143;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;57;-997.0298,-992.353;Float;False;Property;_TilingTopLayerAlbedo;Tiling (X, Y);16;0;Create;False;0;0;False;0;1,1;2,2;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.PowerNode;59;-953.317,1467.869;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;111;-142.31,436.0836;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;39;503.321,-307.126;Float;False;Property;_DirtColor;Dirt Color;24;0;Create;True;0;0;False;1;Header(Dirt (use BLUE vertex color channel for painting));1,1,1,0.7843137;0.4558824,0.4070576,0.3754325,0.747;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;147;-1169.082,-305.7586;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;141;747.7748,-43.31528;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;177;43.72594,385.6368;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;8;-502.6349,-347.8461;Float;False;Property;_Color;Color;2;0;Create;True;0;0;False;1;Header (First Layer (bottom));1,1,1,1;1,1,1,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;91;-188.8006,930.0209;Float;False;Property;_BlendContrast;Blend Contrast;20;0;Create;True;0;0;False;0;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;154;-121.6866,1431.668;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-579.7241,-105.7671;Float;True;Property;_MainTex;Albedo;3;1;[NoScaleOffset];Create;False;0;0;False;0;None;98a4d09e0cc80c8499ecb9b78d7f90b6;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;55;-676.0099,-954.6851;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;155;93.62534,1368.679;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;140;357.4755,1044.122;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-166.1285,-148.149;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;176;79.54724,583.8287;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;49;-331.3899,-986.5439;Float;True;Property;_SecondBaseColor;Albedo;12;1;[NoScaleOffset];Create;False;0;0;False;0;None;c68296334e691ed45b62266cbc716628;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;179;178.8288,724.4209;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0.001;False;4;FLOAT;150;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;178;204.8403,420.7704;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;9;-190.4033,-377.6351;Float;False;Property;_DetailColor;Color;18;0;Create;False;0;0;False;0;1,1,1,1;1,1,1,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;50;148.6974,-243.9866;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;36;146.7522,-71.06419;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;48;646.7662,1031.476;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;63;-239.3424,79.36974;Float;False;Property;_Glossiness;Gloss;8;0;Create;False;0;0;False;0;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.HeightMapBlendNode;90;403.1136,396.1227;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;45;560.4303,552.7068;Float;False;Property;_Gloss;Gloss;19;0;Create;True;0;0;False;0;1;0.075;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;64;151.6995,40.73311;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;43;876.475,329.884;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;6;514.821,-8.165043;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;142;1032.521,582.6362;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;42;1117.672,165.7482;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;38;1032.225,-42.25751;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;81;-664.9685,-640.2746;Float;False;Property;_NormalBlend;Normal Blend;15;0;Create;True;0;0;False;0;0.3;0.343;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;84;-642.9283,-734.1786;Float;False;Property;_NormalPowerSecondLayer;Normal Power;14;0;Create;False;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;83;-892.0397,204.108;Float;False;Property;_NormalPower;Normal Power;5;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;67;988.965,898.2256;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;3;-570,95.44302;Float;True;Property;_BumpMap;Normal;4;2;[NoScaleOffset];[Normal];Create;False;0;0;False;0;None;6e7790c21aec6ba4788f19fcc3ce6d31;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;80;878.4573,-449.3071;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;73;-313.0837,-769.9641;Float;True;Property;_SecondNormal;Normal;13;2;[NoScaleOffset];[Normal];Create;False;0;0;False;0;None;b3d940e75e1f5d24684cd93a2758e1bf;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ToggleSwitchNode;159;1413.045,1.837402;Float;False;Property;_ShowVertexColorsDebug;Show Vertex Colors (Debug);0;0;Create;True;0;0;False;0;0;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;68;1292.102,198.7023;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;46;1478.151,156.5864;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;75;1113.966,-576.0596;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ToggleSwitchNode;181;1810.119,9.437716;Float;False;Property;_ShowVertexAlpha;Show Vertex Alpha;1;0;Create;True;0;0;False;0;0;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;2096.795,19.05547;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;MaskBlend_NL;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;156;0;21;0
WireConnection;156;3;157;0
WireConnection;163;0;160;1
WireConnection;163;1;160;2
WireConnection;139;0;11;0
WireConnection;4;0;51;0
WireConnection;4;1;52;0
WireConnection;162;0;160;1
WireConnection;162;1;160;3
WireConnection;167;0;161;2
WireConnection;168;0;163;0
WireConnection;168;1;162;0
WireConnection;168;2;167;0
WireConnection;164;0;160;3
WireConnection;164;1;160;2
WireConnection;166;0;161;1
WireConnection;22;0;21;0
WireConnection;22;1;4;0
WireConnection;108;0;156;1
WireConnection;108;1;139;0
WireConnection;138;0;25;0
WireConnection;100;0;22;2
WireConnection;103;0;108;0
WireConnection;169;0;168;0
WireConnection;169;1;164;0
WireConnection;169;2;166;0
WireConnection;171;0;41;0
WireConnection;171;1;172;0
WireConnection;106;0;100;0
WireConnection;106;1;103;0
WireConnection;170;0;171;0
WireConnection;170;1;169;0
WireConnection;33;0;138;0
WireConnection;97;0;33;0
WireConnection;97;1;106;0
WireConnection;37;0;21;0
WireConnection;37;1;170;0
WireConnection;146;0;145;0
WireConnection;146;1;180;1
WireConnection;152;0;180;1
WireConnection;153;0;152;0
WireConnection;153;1;151;0
WireConnection;59;0;37;3
WireConnection;59;1;58;0
WireConnection;111;0;97;0
WireConnection;111;1;17;4
WireConnection;147;0;146;0
WireConnection;141;0;39;4
WireConnection;177;0;147;0
WireConnection;177;1;111;0
WireConnection;154;0;153;0
WireConnection;154;1;59;0
WireConnection;1;1;4;0
WireConnection;55;0;57;0
WireConnection;55;1;56;0
WireConnection;155;0;154;0
WireConnection;140;0;141;0
WireConnection;7;0;8;0
WireConnection;7;1;1;0
WireConnection;176;0;147;0
WireConnection;176;1;106;0
WireConnection;49;1;55;0
WireConnection;179;0;91;0
WireConnection;178;0;177;0
WireConnection;178;1;17;4
WireConnection;50;0;49;0
WireConnection;50;1;9;0
WireConnection;36;0;7;0
WireConnection;36;1;17;2
WireConnection;48;0;17;3
WireConnection;48;1;155;0
WireConnection;48;2;140;0
WireConnection;90;0;176;0
WireConnection;90;1;178;0
WireConnection;90;2;179;0
WireConnection;64;0;1;4
WireConnection;64;1;63;0
WireConnection;43;0;90;0
WireConnection;43;1;45;0
WireConnection;6;0;36;0
WireConnection;6;1;50;0
WireConnection;6;2;90;0
WireConnection;142;0;48;0
WireConnection;42;0;64;0
WireConnection;42;1;43;0
WireConnection;38;0;6;0
WireConnection;38;1;39;0
WireConnection;38;2;142;0
WireConnection;67;0;48;0
WireConnection;3;1;4;0
WireConnection;3;5;83;0
WireConnection;80;0;90;0
WireConnection;80;1;81;0
WireConnection;73;1;55;0
WireConnection;73;5;84;0
WireConnection;159;0;38;0
WireConnection;159;1;17;0
WireConnection;68;0;42;0
WireConnection;68;1;67;0
WireConnection;46;0;68;0
WireConnection;75;0;3;0
WireConnection;75;1;73;0
WireConnection;75;2;80;0
WireConnection;181;0;159;0
WireConnection;181;1;17;4
WireConnection;0;0;181;0
WireConnection;0;1;75;0
WireConnection;0;4;46;0
WireConnection;0;5;180;1
ASEEND*/
//CHKSM=F01A2598A106D430397F830875ED76FEF71769D3