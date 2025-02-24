// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "NOT_Lonely/MaskBlend3Colors_NL"
{
	Properties
	{
		[Toggle]_ShowVertexColorsDebug("Show Vertex Colors (Debug)", Float) = 0
		[Toggle]_ShowVertexAlphaDebug("Show Vertex Alpha (Debug)", Float) = 0
		[Header (1st Layer (bottom))]_Color("Color", Color) = (1,1,1,1)
		[NoScaleOffset]_MainTex("Albedo", 2D) = "white" {}
		[NoScaleOffset][Normal]_BumpMap("Normal", 2D) = "bump" {}
		_NormalPower("Normal Power", Float) = 1
		_Tiling("Tiling (X, Y)", Vector) = (1,1,0,0)
		_Offset("Offset (X, Y)", Vector) = (0,0,0,0)
		_Glossiness("Gloss", Range( 0 , 1)) = 0
		[NoScaleOffset][Header (2nd Layer (use ALPHA vertex color channel to erase 2nd layer))]_LayerMasks("Mask_01 (R), Mask_02 (G), Dirt Mask (B)", 2D) = "black" {}
		[NoScaleOffset]_AOUV2("AO (UV2)", 2D) = "white" {}
		_Mask_01_tiling("Mask_01 Tiling", Range( 0.01 , 2)) = 1
		[NoScaleOffset]_SecondBaseColor("Albedo", 2D) = "white" {}
		[NoScaleOffset][Normal]_SecondNormal("Normal", 2D) = "bump" {}
		_NormalPowerSecondLayer("Normal Power", Float) = 1
		_NormalBlend("Normal Blend", Range( 0 , 1)) = 0.3
		_TilingTopLayerAlbedo("Tiling (X, Y)", Vector) = (1,1,0,0)
		_OffsetTopLayerAlbedo("Offset (X, Y)", Vector) = (0,0,0,0)
		_Gloss("Gloss", Range( 0 , 1)) = 1
		_BlendContrast("Blend Contrast", Range( 0 , 1)) = 0.5
		_TopLayerAmount("Mask_01 wear (Triplanar)", Range( 0 , 1)) = 0
		_Mask_02wearUV0("Mask_02 wear (UV0)", Range( 0 , 1)) = 0
		_Mask_AO("Mask_AO", Range( 0 , 1)) = 0
		[Header(2nd layer colors (use R and G vertex colors to override masks))]_Color_01("Color_01", Color) = (1,1,1,1)
		_Color_02VertexRchannel("Color_02 (Vertex R-channel)", Color) = (1,1,1,1)
		_Color_03VertexGchannel("Color_03 (Vertex G-channel)", Color) = (1,1,1,1)
		[Header(Dirt (use BLUE vertex color channel for painting))]_DirtColor("Dirt Color", Color) = (1,1,1,1)
		_DirtTiling("Dirt Tiling", Range( 0 , 30)) = 1
		_AODirt("AO Dirt", Range( 0 , 1)) = 0
		_DirtMaskContrast("Dirt Mask Contrast", Range( 0 , 5)) = 1
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
		uniform float _Mask_01_tiling;
		uniform float _TopLayerAmount;
		uniform float _Mask_02wearUV0;
		uniform float _BlendContrast;
		uniform float _NormalBlend;
		uniform float _ShowVertexAlphaDebug;
		uniform float _ShowVertexColorsDebug;
		uniform float4 _Color;
		uniform sampler2D _MainTex;
		uniform sampler2D _SecondBaseColor;
		uniform float4 _Color_01;
		uniform float4 _Color_02VertexRchannel;
		uniform float4 _Color_03VertexGchannel;
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
			float2 uv4_AOUV2210 = i.uv4_texcoord4;
			float4 tex2DNode210 = tex2D( _AOUV2, uv4_AOUV2210 );
			float clampResult212 = clamp( ( (0.0 + (_Mask_AO - 0.0) * (3.0 - 0.0) / (1.0 - 0.0)) * tex2DNode210.r ) , 0.0 , 1.0 );
			float temp_output_181_0 = ( 1.0 - clampResult212 );
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float4 triplanar192 = TriplanarSamplingSF( _LayerMasks, ase_worldPos, ase_worldNormal, 1.0, _Mask_01_tiling, 1.0, 0 );
			float temp_output_106_0 = ( ( 1.0 - tex2D( _LayerMasks, uv_TexCoord4 ).g ) * ( 1.0 - ( triplanar192.x * (-0.5 + (_TopLayerAmount - 0.0) * (1.0 - -0.5) / (1.0 - 0.0)) ) ) );
			float HeightMask90 = saturate(pow(((( temp_output_181_0 + temp_output_106_0 )*( ( temp_output_181_0 + ( ( 1.0 - (-0.5 + (_Mask_02wearUV0 - 0.0) * (1.0 - -0.5) / (1.0 - 0.0)) ) * temp_output_106_0 ) ) * i.vertexColor.a ))*4)+(( ( temp_output_181_0 + ( ( 1.0 - (-0.5 + (_Mask_02wearUV0 - 0.0) * (1.0 - -0.5) / (1.0 - 0.0)) ) * temp_output_106_0 ) ) * i.vertexColor.a )*2),(0.001 + (_BlendContrast - 0.0) * (150.0 - 0.001) / (1.0 - 0.0))));
			float3 lerpResult75 = lerp( UnpackScaleNormal( tex2D( _BumpMap, uv_TexCoord4 ), _NormalPower ) , UnpackScaleNormal( tex2D( _SecondNormal, uv_TexCoord55 ), _NormalPowerSecondLayer ) , ( HeightMask90 * _NormalBlend ));
			o.Normal = lerpResult75;
			float4 tex2DNode1 = tex2D( _MainTex, uv_TexCoord4 );
			float temp_output_156_0 = ( i.vertexColor.r + i.vertexColor.g );
			float4 tex2DNode49 = tex2D( _SecondBaseColor, uv_TexCoord55 );
			float temp_output_157_0 = max( temp_output_156_0 , 1.0 );
			float4 lerpResult6 = lerp( ( _Color * tex2DNode1 ) , ( ( max( ( 1.0 - min( temp_output_156_0 , 1.0 ) ) , (float)0 ) * ( tex2DNode49 * _Color_01 ) ) + ( ( tex2DNode49 * _Color_02VertexRchannel ) * ( i.vertexColor.r / temp_output_157_0 ) ) + ( ( tex2DNode49 * _Color_03VertexGchannel ) * ( i.vertexColor.g / temp_output_157_0 ) ) ) , HeightMask90);
			float2 appendResult198 = (float2(ase_worldPos.x , ase_worldPos.y));
			float2 appendResult199 = (float2(ase_worldPos.x , ase_worldPos.z));
			float3 ase_normWorldNormal = normalize( ase_worldNormal );
			float2 lerpResult202 = lerp( appendResult198 , appendResult199 , abs( ase_normWorldNormal.y ));
			float2 appendResult201 = (float2(ase_worldPos.z , ase_worldPos.y));
			float2 lerpResult203 = lerp( lerpResult202 , appendResult201 , abs( ase_normWorldNormal.x ));
			float clampResult187 = clamp( ( ( ( 1.0 - tex2DNode210.r ) * _AODirt ) + pow( tex2D( _LayerMasks, ( ( _DirtTiling * 0.1 ) * lerpResult203 ) ).b , _DirtMaskContrast ) ) , 0.0 , 1.0 );
			float temp_output_48_0 = ( i.vertexColor.b * clampResult187 * _DirtColor.a );
			float4 lerpResult38 = lerp( lerpResult6 , _DirtColor , temp_output_48_0);
			float4 temp_cast_1 = (i.vertexColor.a).xxxx;
			o.Albedo = lerp(lerp(lerpResult38,i.vertexColor,_ShowVertexColorsDebug),temp_cast_1,_ShowVertexAlphaDebug).rgb;
			float clampResult46 = clamp( ( ( ( tex2DNode1.a * _Glossiness ) + ( HeightMask90 * _Gloss ) ) * ( 1.0 - temp_output_48_0 ) ) , 0.0 , 1.0 );
			o.Smoothness = clampResult46;
			o.Occlusion = tex2DNode210.r;
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
7;7;1906;1124;2765.255;785.8116;1;True;True
Node;AmplifyShaderEditor.RangedFloatNode;11;-1236.893,974.4603;Float;False;Property;_TopLayerAmount;Mask_01 wear (Triplanar);20;0;Create;False;0;0;False;0;0;0.493;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;197;-2607.758,1825.335;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldNormalVector;196;-2513.6,1576.667;Float;False;True;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.Vector2Node;52;-2732.822,396.9138;Float;False;Property;_Offset;Offset (X, Y);7;0;Create;False;0;0;False;0;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;51;-2733.959,231.9919;Float;False;Property;_Tiling;Tiling (X, Y);6;0;Create;False;0;0;False;0;1,1;1,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TexturePropertyNode;21;-2246.734,663.1748;Float;True;Property;_LayerMasks;Mask_01 (R), Mask_02 (G), Dirt Mask (B);9;1;[NoScaleOffset];Create;False;0;0;False;1;Header (2nd Layer (use ALPHA vertex color channel to erase 2nd layer));None;a7851ad47f305fc4589d75fb0169a983;False;black;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.RangedFloatNode;194;-1867.32,510.1571;Float;False;Property;_Mask_01_tiling;Mask_01 Tiling;11;0;Create;False;0;0;False;0;1;0.172;0.01;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;183;-1723.094,-179.8235;Float;False;Property;_Mask_AO;Mask_AO;22;0;Create;True;0;0;False;0;0;0.264;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;4;-2390.286,314.2429;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;176;-915.7617,916.3575;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-0.5;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TriplanarNode;192;-1490.1,488.8795;Float;True;Spherical;World;False;Top Texture 0;_TopTexture0;white;-1;None;Mid Texture 0;_MidTexture0;white;-1;None;Bot Texture 0;_BotTexture0;white;-1;None;Triplanar Sampler;False;9;0;SAMPLER2D;;False;5;FLOAT;1;False;1;SAMPLER2D;;False;6;FLOAT;0;False;2;SAMPLER2D;;False;7;FLOAT;0;False;8;FLOAT;1;False;3;FLOAT;1;False;4;FLOAT;1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;198;-2213.011,1953.254;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;199;-2213.397,1825.329;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;47;-1711.149,1277.701;Float;False;1090.498;280;Dirt mask;5;41;37;205;206;204;;1,1,1,1;0;0
Node;AmplifyShaderEditor.AbsOpNode;200;-2206.587,1643.593;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;22;-1506.703,750.4447;Float;True;Property;_TextureSample0;Texture Sample 0;3;0;Create;True;0;0;False;0;None;None;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;25;-1255.182,1100.13;Float;False;Property;_Mask_02wearUV0;Mask_02 wear (UV0);21;0;Create;True;0;0;False;0;0;0.95;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;202;-1927.5,1722.55;Float;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;41;-1700.165,1361.861;Float;False;Property;_DirtTiling;Dirt Tiling;27;0;Create;True;0;0;False;0;1;2.4;0;30;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;211;-1451.672,-148.1658;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;3;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;210;-2085.588,-372.3574;Float;True;Property;_AOUV2;AO (UV2);10;1;[NoScaleOffset];Create;True;0;0;False;0;None;170498bde693471408c549b01c438001;True;3;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;17;-1010.445,-659.9144;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;69;-2268.162,-2087.837;Float;False;1342.229;495.8069;Second Layer Maps;6;49;55;56;57;73;84;;1,1,1,1;0;0
Node;AmplifyShaderEditor.DynamicAppendNode;201;-2211.785,2086.843;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;205;-1605.985,1473.591;Float;False;Constant;_Float2;Float 2;26;0;Create;True;0;0;False;0;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;195;-2208.413,1555.975;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;108;-709.2159,906.6047;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;203;-1610.167,1727.124;Float;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TFHCRemapNode;175;-879.8189,1084.037;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-0.5;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;182;-1241.294,40.41527;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;103;-608.8397,766.2478;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;206;-1413.985,1439.591;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;100;-608.0955,660.3507;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;156;-699.3712,-638.7413;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;158;-706.1887,-531.9258;Float;False;Constant;_Float0;Float 0;25;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;57;-2217.6,-2037.837;Float;False;Property;_TilingTopLayerAlbedo;Tiling (X, Y);16;0;Create;False;0;0;False;0;1,1;1,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;56;-2218.161,-1883.08;Float;False;Property;_OffsetTopLayerAlbedo;Offset (X, Y);17;0;Create;False;0;0;False;0;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.OneMinusNode;33;-649.3429,1093.803;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;106;-321.81,683.5209;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;204;-1246.76,1449.804;Float;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;165;-592.5511,-1414.245;Float;False;Constant;_Float1;Float 1;25;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMinOpNode;173;-518.0732,-766.7743;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;55;-1896.579,-2000.169;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;212;-1063.013,104.9301;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;97;-262.8178,432.6768;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;164;-389.4828,-1410.325;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;188;-522.0482,1182.473;Float;False;Property;_AODirt;AO Dirt;28;0;Create;True;0;0;False;0;0;0.498;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;142;-1091.44,-1311.957;Float;False;Property;_Color_01;Color_01;23;0;Create;True;0;0;False;1;Header(2nd layer colors (use R and G vertex colors to override masks));1,1,1,1;0.8014706,0.7824498,0.748432,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;185;-292.2799,1100.899;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;181;-904.7814,151.2769;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;58;-706.0253,1600.22;Float;False;Property;_DirtMaskContrast;Dirt Mask Contrast;29;0;Create;True;0;0;False;0;1;2.56;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.IntNode;167;-367.5201,-1292.691;Float;False;Constant;_Int0;Int 0;25;0;Create;True;0;0;False;0;0;0;0;1;INT;0
Node;AmplifyShaderEditor.ColorNode;9;-1096.516,-1098.443;Float;False;Property;_Color_02VertexRchannel;Color_02 (Vertex R-channel);24;0;Create;True;0;0;False;0;1,1,1,1;0.4723184,0.6284003,0.6691177,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;37;-1079.168,1329.338;Float;True;Property;_TextureSample1;Texture Sample 1;7;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;141;-1098.712,-891.2687;Float;False;Property;_Color_03VertexGchannel;Color_03 (Vertex G-channel);25;0;Create;True;0;0;False;0;1,1,1,1;0.3970588,0.3970588,0.3970588,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;169;-721.949,-680.1298;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;157;-516.0294,-608.0135;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;49;-1540.52,-2030.598;Float;True;Property;_SecondBaseColor;Albedo;12;1;[NoScaleOffset];Create;False;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;170;-748.3159,-710.012;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;189;-112.8717,1235.138;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;209;-29.53451,410.8634;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;50;-571.8251,-972.6263;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;39;928.2552,-319.6768;Float;False;Property;_DirtColor;Dirt Color;26;0;Create;True;0;0;False;1;Header(Dirt (use BLUE vertex color channel for painting));1,1,1,1;0.4264705,0.4041525,0.351211,0.841;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;59;-55.50432,1442.435;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;91;-187.1628,837.8228;Float;False;Property;_BlendContrast;Blend Contrast;19;0;Create;True;0;0;False;0;0.5;0.513;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;147;-577.4581,-1106.317;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;146;-580.0927,-1248.16;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;166;-213.3345,-1410.099;Float;False;2;0;FLOAT;0;False;1;INT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;160;-272.253,-701.0028;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;159;-273.4359,-854.8203;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;208;-72.10709,648.1735;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;191;909.8845,683.2953;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;139;138.474,680.9771;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0.001;False;4;FLOAT;150;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;186;132.9884,1352.408;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;111;142.2552,377.1011;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;161;-76.0531,-1247.569;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;1;-651.9648,-50.36061;Float;True;Property;_MainTex;Albedo;3;1;[NoScaleOffset];Create;False;0;0;False;0;None;38e5f41cb2020f7478ed9544289e6561;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;8;-572.2756,-231.3399;Float;False;Property;_Color;Color;2;0;Create;True;0;0;False;1;Header (1st Layer (bottom));1,1,1,1;1,1,1,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;163;-71.75731,-986.4457;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;162;-73.04314,-1114.984;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;187;274.6884,1304.307;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.HeightMapBlendNode;90;558.1252,391.1783;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;45;560.4303,552.7068;Float;False;Property;_Gloss;Gloss;18;0;Create;True;0;0;False;0;1;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-166.1285,-148.149;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;168;154.9858,-1137.253;Float;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;190;465.3436,1197.566;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;63;-227.2129,231.8551;Float;False;Property;_Glossiness;Gloss;8;0;Create;False;0;0;False;0;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;48;647.6173,1180.476;Float;True;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;43;876.475,329.884;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;6;939.7551,-20.71585;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;64;151.6995,40.73311;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;84;-1863.498,-1779.661;Float;False;Property;_NormalPowerSecondLayer;Normal Power;14;0;Create;False;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;73;-1546.527,-1811.156;Float;True;Property;_SecondNormal;Normal;13;2;[NoScaleOffset];[Normal];Create;False;0;0;False;0;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;81;402.5843,-433.7419;Float;False;Property;_NormalBlend;Normal Blend;15;0;Create;True;0;0;False;0;0.3;0.3;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;83;-866.0479,254.3589;Float;False;Property;_NormalPower;Normal Power;5;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;67;990.7157,1143.85;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;38;1307.422,-52.35361;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;42;1110.5,228.5022;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;207;1608.998,-10.28249;Float;False;Property;_ShowVertexColorsDebug;Show Vertex Colors (Debug);0;0;Create;True;0;0;False;0;0;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;80;878.4573,-449.3071;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;171;472.5059,-1514.81;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;68;1292.102,198.7023;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;3;-632.5685,145.4468;Float;True;Property;_BumpMap;Normal;4;2;[NoScaleOffset];[Normal];Create;False;0;0;False;0;None;18f39a92d5462eb4ebd3369c1f26c90e;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;46;1478.151,156.5864;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;213;1949.563,19.12582;Float;False;Property;_ShowVertexAlphaDebug;Show Vertex Alpha (Debug);1;0;Create;True;0;0;False;0;0;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;75;1113.966,-576.0596;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;2257.175,34.96101;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;NOT_Lonely/MaskBlend3Colors_NL;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;4;0;51;0
WireConnection;4;1;52;0
WireConnection;176;0;11;0
WireConnection;192;0;21;0
WireConnection;192;3;194;0
WireConnection;198;0;197;1
WireConnection;198;1;197;2
WireConnection;199;0;197;1
WireConnection;199;1;197;3
WireConnection;200;0;196;2
WireConnection;22;0;21;0
WireConnection;22;1;4;0
WireConnection;202;0;198;0
WireConnection;202;1;199;0
WireConnection;202;2;200;0
WireConnection;211;0;183;0
WireConnection;201;0;197;3
WireConnection;201;1;197;2
WireConnection;195;0;196;1
WireConnection;108;0;192;1
WireConnection;108;1;176;0
WireConnection;203;0;202;0
WireConnection;203;1;201;0
WireConnection;203;2;195;0
WireConnection;175;0;25;0
WireConnection;182;0;211;0
WireConnection;182;1;210;1
WireConnection;103;0;108;0
WireConnection;206;0;41;0
WireConnection;206;1;205;0
WireConnection;100;0;22;2
WireConnection;156;0;17;1
WireConnection;156;1;17;2
WireConnection;33;0;175;0
WireConnection;106;0;100;0
WireConnection;106;1;103;0
WireConnection;204;0;206;0
WireConnection;204;1;203;0
WireConnection;173;0;156;0
WireConnection;173;1;158;0
WireConnection;55;0;57;0
WireConnection;55;1;56;0
WireConnection;212;0;182;0
WireConnection;97;0;33;0
WireConnection;97;1;106;0
WireConnection;164;0;165;0
WireConnection;164;1;173;0
WireConnection;185;0;210;1
WireConnection;181;0;212;0
WireConnection;37;0;21;0
WireConnection;37;1;204;0
WireConnection;169;0;17;2
WireConnection;157;0;156;0
WireConnection;157;1;158;0
WireConnection;49;1;55;0
WireConnection;170;0;17;1
WireConnection;189;0;185;0
WireConnection;189;1;188;0
WireConnection;209;0;181;0
WireConnection;209;1;97;0
WireConnection;50;0;49;0
WireConnection;50;1;141;0
WireConnection;59;0;37;3
WireConnection;59;1;58;0
WireConnection;147;0;49;0
WireConnection;147;1;9;0
WireConnection;146;0;49;0
WireConnection;146;1;142;0
WireConnection;166;0;164;0
WireConnection;166;1;167;0
WireConnection;160;0;169;0
WireConnection;160;1;157;0
WireConnection;159;0;170;0
WireConnection;159;1;157;0
WireConnection;208;0;181;0
WireConnection;208;1;106;0
WireConnection;191;0;39;4
WireConnection;139;0;91;0
WireConnection;186;0;189;0
WireConnection;186;1;59;0
WireConnection;111;0;209;0
WireConnection;111;1;17;4
WireConnection;161;0;166;0
WireConnection;161;1;146;0
WireConnection;1;1;4;0
WireConnection;163;0;50;0
WireConnection;163;1;160;0
WireConnection;162;0;147;0
WireConnection;162;1;159;0
WireConnection;187;0;186;0
WireConnection;90;0;208;0
WireConnection;90;1;111;0
WireConnection;90;2;139;0
WireConnection;7;0;8;0
WireConnection;7;1;1;0
WireConnection;168;0;161;0
WireConnection;168;1;162;0
WireConnection;168;2;163;0
WireConnection;190;0;191;0
WireConnection;48;0;17;3
WireConnection;48;1;187;0
WireConnection;48;2;190;0
WireConnection;43;0;90;0
WireConnection;43;1;45;0
WireConnection;6;0;7;0
WireConnection;6;1;168;0
WireConnection;6;2;90;0
WireConnection;64;0;1;4
WireConnection;64;1;63;0
WireConnection;73;1;55;0
WireConnection;73;5;84;0
WireConnection;67;0;48;0
WireConnection;38;0;6;0
WireConnection;38;1;39;0
WireConnection;38;2;48;0
WireConnection;42;0;64;0
WireConnection;42;1;43;0
WireConnection;207;0;38;0
WireConnection;207;1;17;0
WireConnection;80;0;90;0
WireConnection;80;1;81;0
WireConnection;171;0;73;0
WireConnection;68;0;42;0
WireConnection;68;1;67;0
WireConnection;3;1;4;0
WireConnection;3;5;83;0
WireConnection;46;0;68;0
WireConnection;213;0;207;0
WireConnection;213;1;17;4
WireConnection;75;0;3;0
WireConnection;75;1;171;0
WireConnection;75;2;80;0
WireConnection;0;0;213;0
WireConnection;0;1;75;0
WireConnection;0;4;46;0
WireConnection;0;5;210;1
ASEEND*/
//CHKSM=9160992697A9B49379FACB55531789A7F7920F58