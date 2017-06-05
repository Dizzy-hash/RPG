// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ASESampleShaders/TextureArray"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_TextureArray("Texture Array", 2DArray ) = "" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#include "UnityCG.cginc"
		#pragma target 3.5
		#pragma surface surf StandardSpecular keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float4 _TextureArray_ST;
		uniform UNITY_DECLARE_TEX2DARRAY( _TextureArray );

		void surf( Input i , inout SurfaceOutputStandardSpecular o )
		{
			float2 uv_TextureArray = i.uv_texcoord * _TextureArray_ST.xy + _TextureArray_ST.zw;
			float3 temp_cast_1 = 1.0;
			o.Normal = ( ( LinearToGammaSpace( UNITY_SAMPLE_TEX2DARRAY(_TextureArray, float3(uv_TextureArray, 1.0)  ).rgb ) * 2.0 ) - temp_cast_1 );
			o.Albedo = UNITY_SAMPLE_TEX2DARRAY(_TextureArray, float3(uv_TextureArray, 0.0)  ).rgb;
			o.Smoothness = ( 1.0 - UNITY_SAMPLE_TEX2DARRAY(_TextureArray, float3(uv_TextureArray, 3.0)  ).r );
			o.Occlusion = UNITY_SAMPLE_TEX2DARRAY(_TextureArray, float3(uv_TextureArray, 2.0)  ).r;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=3204
1927;-389;1066;703;1745.099;833.3865;2.345981;True;False
Node;AmplifyShaderEditor.CommentaryNode;96;-596.401,-244.2001;Float;791.3712;259.6804;Convert to gamma and remap to the -1 to 1 range because the entire array is in linear;5;85;82;83;95;116
Node;AmplifyShaderEditor.RangedFloatNode;83;-506.9004,-95.8996;Float;Constant;_Mul2;Mul2;4;2;0;0
Node;AmplifyShaderEditor.TextureArrayNode;104;-313.8021,252.7002;Float;Property;_TextureArray3;Texture Array 3;1;None;0;Instance;108;Auto;FLOAT2;0,0;FLOAT;0.0;FLOAT;0.0
Node;AmplifyShaderEditor.RangedFloatNode;110;-505.5033,289.4994;Float;Constant;_OcclusionIndex;Occlusion Index;1;2;0;0
Node;AmplifyShaderEditor.RangedFloatNode;109;-511.3033,102.2995;Float;Constant;_RoughnessIndex;Roughness Index;1;3;0;0
Node;AmplifyShaderEditor.RangedFloatNode;111;-1158.604,-172.9006;Float;Constant;_NormalIndex;Normal Index;1;1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;113;-553.9034,-373.3005;Float;Constant;_AlbedoIndex;Albedo Index;1;0;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;82;-290.3004,-190.8995;Float;FLOAT3;0.0;FLOAT;0.0,0,0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;407.6068,-183.0747;Float;True;3;Float;ASEMaterialInspector;StandardSpecular;ASESampleShaders/TextureArray;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;FLOAT3;0,0,0;FLOAT3;0,0,0;FLOAT3;0,0,0;FLOAT3;0,0,0;FLOAT;0.0;FLOAT;0.0;FLOAT3;0,0,0;FLOAT3;0,0,0;FLOAT;0.0;FLOAT;0.0;OBJECT;0;FLOAT3;0.0,0,0;FLOAT3;0.0,0,0;OBJECT;0;FLOAT4;0,0,0,0;FLOAT3;0,0,0
Node;AmplifyShaderEditor.OneMinusNode;106;54.20655,40.72321;Float;FLOAT;0.0
Node;AmplifyShaderEditor.LinearToGammaNode;95;-549.6006,-192.6001;Float;FLOAT3;0,0,0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;116;-68.43024,-162.9681;Float;FLOAT3;0.0;FLOAT;0.0,0,0
Node;AmplifyShaderEditor.RangedFloatNode;85;-268.5997,-68.2997;Float;Constant;_Sub1;Sub-1;4;1;0;0
Node;AmplifyShaderEditor.TextureArrayNode;103;-948.903,-205.1002;Float;Property;_TextureArray1;Texture Array 1;1;None;0;Instance;108;Auto;FLOAT2;0,0;FLOAT;0.0;FLOAT;0.0
Node;AmplifyShaderEditor.TextureArrayNode;105;-310.502,63.60038;Float;Property;_TextureArray2;Texture Array 2;1;None;0;Instance;108;Auto;FLOAT2;0,0;FLOAT;0.0;FLOAT;0.0
Node;AmplifyShaderEditor.TextureArrayNode;108;-306.7961,-432.2489;Float;Property;_TextureArray;Texture Array;3;None;0;Object;-1;Auto;FLOAT2;0,0;FLOAT;0.0;FLOAT;0.0
WireConnection;104;1;110;0
WireConnection;82;0;95;0
WireConnection;82;1;83;0
WireConnection;0;0;108;0
WireConnection;0;1;116;0
WireConnection;0;4;106;0
WireConnection;0;5;104;1
WireConnection;106;0;105;1
WireConnection;95;0;103;0
WireConnection;116;0;82;0
WireConnection;116;1;85;0
WireConnection;103;1;111;0
WireConnection;105;1;109;0
WireConnection;108;1;113;0
ASEEND*/
//CHKSM=03E7AC4722C115C0A1D8762F4A9AF07A33748648
