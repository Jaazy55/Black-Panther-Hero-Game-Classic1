// Upgrade NOTE: commented out 'float4 unity_DynamicLightmapST', a built-in variable
// Upgrade NOTE: commented out 'float4 unity_LightmapST', a built-in variable

Shader "Miami/Transparent/MobileWater_AlphaBumpSpec"
{
  Properties
  {
    _Color ("Color Tint", Color) = (1,1,1,1)
    _SpecColor ("Specular Color", Color) = (0.5,0.5,0.5,0)
    _Shininess ("Shininess", Range(0.01, 1)) = 0.078125
    _MainTex ("Base (RGB) TransGloss (A)", 2D) = "white" {}
    _BumpMap ("Normalmap", 2D) = "bump" {}
    _Shift ("Shift", Range(0, 10)) = 0
    _Delta ("Delta", Range(0, 10)) = 1
    _SpeedShift ("Speed Shift", Range(0, 10)) = 1
    _SpeedOffset ("Speed Offset", Range(0, 10)) = 1
    _CubemapScale ("CubemapScale", Range(0, 100)) = 1
    _ReflectColor ("Reflection Color", Color) = (1,1,1,0.5)
    _Cube ("Reflection Cubemap", Cube) = "_Skybox" {}
  }
  SubShader
  {
    Tags
    { 
      "IGNOREPROJECTOR" = "true"
      "QUEUE" = "Transparent"
      "RenderType" = "Transparent"
    }
    LOD 200
    Pass // ind: 1, name: FORWARD
    {
      Name "FORWARD"
      Tags
      { 
        "IGNOREPROJECTOR" = "true"
        "LIGHTMODE" = "FORWARDBASE"
        "QUEUE" = "Transparent"
        "RenderType" = "Transparent"
      }
      LOD 200
      ZClip Off
      ZWrite Off
      Blend SrcAlpha OneMinusSrcAlpha
      ColorMask RGB
      // m_ProgramMask = 6
      CGPROGRAM
      #pragma multi_compile DIRECTIONAL
      //#pragma target 4.0
      
      #pragma vertex vert
      #pragma fragment frag
      
      #include "UnityCG.cginc"
      #define conv_mxt4x4_0(mat4x4) float4(mat4x4[0].x,mat4x4[1].x,mat4x4[2].x,mat4x4[3].x)
      #define conv_mxt4x4_1(mat4x4) float4(mat4x4[0].y,mat4x4[1].y,mat4x4[2].y,mat4x4[3].y)
      #define conv_mxt4x4_2(mat4x4) float4(mat4x4[0].z,mat4x4[1].z,mat4x4[2].z,mat4x4[3].z)
      
      
      #define CODE_BLOCK_VERTEX
      //uniform float4 _SinTime;
      //uniform float4 unity_SHBr;
      //uniform float4 unity_SHBg;
      //uniform float4 unity_SHBb;
      //uniform float4 unity_SHC;
      //uniform float4x4 unity_ObjectToWorld;
      //uniform float4x4 unity_WorldToObject;
      //uniform float4 unity_WorldTransformParams;
      //uniform float4x4 unity_MatrixVP;
      uniform float _Shift;
      uniform float _Delta;
      uniform float _SpeedShift;
      uniform float _SpeedOffset;
      uniform float4 _MainTex_ST;
      uniform float4 _BumpMap_ST;
      //uniform float3 _WorldSpaceCameraPos;
      //uniform float4 _WorldSpaceLightPos0;
      //uniform float4 unity_SHAr;
      //uniform float4 unity_SHAg;
      //uniform float4 unity_SHAb;
      uniform float4 _LightColor0;
      uniform samplerCUBE _Cube;
      uniform sampler2D _MainTex;
      uniform sampler2D _BumpMap;
      uniform float4 _Color;
      uniform float4 _ReflectColor;
      uniform float _CubemapScale;
      struct appdata_t
      {
          float4 tangent :TANGENT;
          float4 vertex :POSITION;
          float3 normal :NORMAL;
          float4 texcoord :TEXCOORD0;
      };
      
      struct OUT_Data_Vert
      {
          float4 xlv_TEXCOORD0 :TEXCOORD0;
          float4 xlv_TEXCOORD1 :TEXCOORD1;
          float4 xlv_TEXCOORD2 :TEXCOORD2;
          float4 xlv_TEXCOORD3 :TEXCOORD3;
          float3 xlv_TEXCOORD4 :TEXCOORD4;
          float4 xlv_TEXCOORD6 :TEXCOORD6;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float4 xlv_TEXCOORD0 :TEXCOORD0;
          float4 xlv_TEXCOORD1 :TEXCOORD1;
          float4 xlv_TEXCOORD2 :TEXCOORD2;
          float4 xlv_TEXCOORD3 :TEXCOORD3;
          float3 xlv_TEXCOORD4 :TEXCOORD4;
      };
      
      struct OUT_Data_Frag
      {
          float4 color :SV_Target0;
      };
      
      OUT_Data_Vert vert(appdata_t in_v)
      {
          OUT_Data_Vert out_v;
          float3 worldBinormal_1;
          float tangentSign_2;
          float3 worldTangent_3;
          float4 tmpvar_4;
          float4 tmpvar_5;
          float4 tmpvar_6;
          float4 tmpvar_7;
          tmpvar_6.xzw = in_v.vertex.xzw;
          tmpvar_7.zw = in_v.texcoord.zw;
          tmpvar_6.y = (in_v.vertex.y + (_Shift * (_SinTime.y * _SpeedShift)));
          float tmpvar_8;
          tmpvar_8 = (_SinTime.y * _SpeedOffset);
          tmpvar_7.x = (in_v.texcoord.x + ((_Delta * sin(in_v.vertex.x)) * tmpvar_8));
          tmpvar_7.y = (in_v.texcoord.y + ((_Delta * sin(in_v.vertex.z)) * tmpvar_8));
          float4 tmpvar_9;
          tmpvar_9.w = 1;
          tmpvar_9.xyz = float3(tmpvar_6.xyz);
          tmpvar_4.xy = float2(TRANSFORM_TEX(tmpvar_7.xy, _MainTex));
          tmpvar_4.zw = TRANSFORM_TEX(tmpvar_7.xy, _BumpMap);
          float3 tmpvar_10;
          tmpvar_10 = mul(unity_ObjectToWorld, tmpvar_6).xyz.xyz;
          float3x3 tmpvar_11;
          tmpvar_11[0] = conv_mxt4x4_0(unity_WorldToObject).xyz;
          tmpvar_11[1] = conv_mxt4x4_1(unity_WorldToObject).xyz;
          tmpvar_11[2] = conv_mxt4x4_2(unity_WorldToObject).xyz;
          float3 tmpvar_12;
          tmpvar_12 = normalize(mul(in_v.normal, tmpvar_11));
          float3x3 tmpvar_13;
          tmpvar_13[0] = conv_mxt4x4_0(unity_ObjectToWorld).xyz;
          tmpvar_13[1] = conv_mxt4x4_1(unity_ObjectToWorld).xyz;
          tmpvar_13[2] = conv_mxt4x4_2(unity_ObjectToWorld).xyz;
          float3 tmpvar_14;
          tmpvar_14 = normalize(mul(tmpvar_13, in_v.tangent.xyz));
          worldTangent_3 = tmpvar_14;
          float tmpvar_15;
          tmpvar_15 = (in_v.tangent.w * unity_WorldTransformParams.w);
          tangentSign_2 = tmpvar_15;
          float3 tmpvar_16;
          tmpvar_16 = (((tmpvar_12.yzx * worldTangent_3.zxy) - (tmpvar_12.zxy * worldTangent_3.yzx)) * tangentSign_2);
          worldBinormal_1 = tmpvar_16;
          float4 tmpvar_17;
          tmpvar_17.x = worldTangent_3.x;
          tmpvar_17.y = worldBinormal_1.x;
          tmpvar_17.z = tmpvar_12.x;
          tmpvar_17.w = tmpvar_10.x;
          float4 tmpvar_18;
          tmpvar_18.x = worldTangent_3.y;
          tmpvar_18.y = worldBinormal_1.y;
          tmpvar_18.z = tmpvar_12.y;
          tmpvar_18.w = tmpvar_10.y;
          float4 tmpvar_19;
          tmpvar_19.x = worldTangent_3.z;
          tmpvar_19.y = worldBinormal_1.z;
          tmpvar_19.z = tmpvar_12.z;
          tmpvar_19.w = tmpvar_10.z;
          float3 normal_20;
          normal_20 = tmpvar_12;
          float3 x1_21;
          float4 tmpvar_22;
          tmpvar_22 = (normal_20.xyzz * normal_20.yzzx);
          x1_21.x = dot(unity_SHBr, tmpvar_22);
          x1_21.y = dot(unity_SHBg, tmpvar_22);
          x1_21.z = dot(unity_SHBb, tmpvar_22);
          out_v.vertex = mul(unity_MatrixVP, mul(unity_ObjectToWorld, tmpvar_9));
          out_v.xlv_TEXCOORD0 = tmpvar_4;
          out_v.xlv_TEXCOORD1 = tmpvar_17;
          out_v.xlv_TEXCOORD2 = tmpvar_18;
          out_v.xlv_TEXCOORD3 = tmpvar_19;
          out_v.xlv_TEXCOORD4 = (x1_21 + (unity_SHC.xyz * ((normal_20.x * normal_20.x) - (normal_20.y * normal_20.y))));
          out_v.xlv_TEXCOORD6 = tmpvar_5;
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          float3 tmpvar_1;
          float3 tmpvar_2;
          float3 worldN_3;
          float4 c_4;
          float3 tmpvar_5;
          float3 lightDir_6;
          float3 tmpvar_7;
          float3 tmpvar_8;
          tmpvar_8.x = in_f.xlv_TEXCOORD1.w;
          tmpvar_8.y = in_f.xlv_TEXCOORD2.w;
          tmpvar_8.z = in_f.xlv_TEXCOORD3.w;
          float3 tmpvar_9;
          tmpvar_9 = _WorldSpaceLightPos0.xyz;
          lightDir_6 = tmpvar_9;
          tmpvar_7 = (-normalize((_WorldSpaceCameraPos - tmpvar_8)));
          float3 tmpvar_10;
          float3 tmpvar_11;
          float4 c_12;
          float4 tmpvar_13;
          tmpvar_13 = tex2D(_MainTex, in_f.xlv_TEXCOORD0.xy);
          c_12 = tmpvar_13;
          tmpvar_10 = (c_12.xyz * _Color.xyz);
          float4 tmpvar_14;
          tmpvar_14 = texCUBE(_Cube, tmpvar_7);
          tmpvar_11 = ((tmpvar_14.xyz * _ReflectColor.xyz) * _CubemapScale);
          float3 tmpvar_15;
          tmpvar_15 = ((tex2D(_BumpMap, in_f.xlv_TEXCOORD0.zw).xyz * 2) - 1).xyz;
          worldN_3.x = dot(in_f.xlv_TEXCOORD1.xyz, tmpvar_15);
          worldN_3.y = dot(in_f.xlv_TEXCOORD2.xyz, tmpvar_15);
          worldN_3.z = dot(in_f.xlv_TEXCOORD3.xyz, tmpvar_15);
          tmpvar_5 = worldN_3;
          tmpvar_1 = _LightColor0.xyz;
          tmpvar_2 = lightDir_6;
          float3 normalWorld_16;
          normalWorld_16 = tmpvar_5;
          float4 tmpvar_17;
          tmpvar_17.w = 1;
          tmpvar_17.xyz = float3(normalWorld_16);
          float3 x_18;
          x_18.x = dot(unity_SHAr, tmpvar_17);
          x_18.y = dot(unity_SHAg, tmpvar_17);
          x_18.z = dot(unity_SHAb, tmpvar_17);
          float3 tmpvar_19;
          tmpvar_19 = max(((1.055 * pow(max(float3(0, 0, 0), (in_f.xlv_TEXCOORD4 + x_18)), float3(0.4166667, 0.4166667, 0.4166667))) - 0.055), float3(0, 0, 0));
          float4 c_20;
          float4 c_21;
          float diff_22;
          float tmpvar_23;
          tmpvar_23 = max(0, dot(tmpvar_5, tmpvar_2));
          diff_22 = tmpvar_23;
          c_21.xyz = float3(((tmpvar_10 * tmpvar_1) * diff_22));
          c_21.w = (tmpvar_14.w * _ReflectColor.w);
          c_20.w = c_21.w;
          c_20.xyz = float3((c_21.xyz + (tmpvar_10 * tmpvar_19)));
          c_4.w = c_20.w;
          c_4.xyz = float3((c_20.xyz + tmpvar_11));
          out_f.color = c_4;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
    Pass // ind: 2, name: FORWARD
    {
      Name "FORWARD"
      Tags
      { 
        "IGNOREPROJECTOR" = "true"
        "LIGHTMODE" = "FORWARDADD"
        "QUEUE" = "Transparent"
        "RenderType" = "Transparent"
      }
      LOD 200
      ZClip Off
      ZWrite Off
      Blend SrcAlpha One
      ColorMask RGB
      // m_ProgramMask = 6
      CGPROGRAM
      #pragma multi_compile POINT
      //#pragma target 4.0
      
      #pragma vertex vert
      #pragma fragment frag
      
      #include "UnityCG.cginc"
      #define conv_mxt4x4_0(mat4x4) float4(mat4x4[0].x,mat4x4[1].x,mat4x4[2].x,mat4x4[3].x)
      #define conv_mxt4x4_1(mat4x4) float4(mat4x4[0].y,mat4x4[1].y,mat4x4[2].y,mat4x4[3].y)
      #define conv_mxt4x4_2(mat4x4) float4(mat4x4[0].z,mat4x4[1].z,mat4x4[2].z,mat4x4[3].z)
      
      
      #define CODE_BLOCK_VERTEX
      //uniform float4 _SinTime;
      //uniform float4x4 unity_ObjectToWorld;
      //uniform float4x4 unity_WorldToObject;
      //uniform float4 unity_WorldTransformParams;
      //uniform float4x4 unity_MatrixVP;
      uniform float _Shift;
      uniform float _Delta;
      uniform float _SpeedShift;
      uniform float _SpeedOffset;
      uniform float4 _MainTex_ST;
      uniform float4 _BumpMap_ST;
      //uniform float3 _WorldSpaceCameraPos;
      //uniform float4 _WorldSpaceLightPos0;
      uniform float4 _LightColor0;
      uniform sampler2D _LightTexture0;
      uniform float4x4 unity_WorldToLight;
      uniform samplerCUBE _Cube;
      uniform sampler2D _MainTex;
      uniform sampler2D _BumpMap;
      uniform float4 _Color;
      uniform float4 _ReflectColor;
      struct appdata_t
      {
          float4 tangent :TANGENT;
          float4 vertex :POSITION;
          float3 normal :NORMAL;
          float4 texcoord :TEXCOORD0;
      };
      
      struct OUT_Data_Vert
      {
          float4 xlv_TEXCOORD0 :TEXCOORD0;
          float3 xlv_TEXCOORD1 :TEXCOORD1;
          float3 xlv_TEXCOORD2 :TEXCOORD2;
          float3 xlv_TEXCOORD3 :TEXCOORD3;
          float3 xlv_TEXCOORD4 :TEXCOORD4;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float4 xlv_TEXCOORD0 :TEXCOORD0;
          float3 xlv_TEXCOORD1 :TEXCOORD1;
          float3 xlv_TEXCOORD2 :TEXCOORD2;
          float3 xlv_TEXCOORD3 :TEXCOORD3;
          float3 xlv_TEXCOORD4 :TEXCOORD4;
      };
      
      struct OUT_Data_Frag
      {
          float4 color :SV_Target0;
      };
      
      OUT_Data_Vert vert(appdata_t in_v)
      {
          OUT_Data_Vert out_v;
          float3 worldBinormal_1;
          float tangentSign_2;
          float3 worldTangent_3;
          float4 tmpvar_4;
          float4 tmpvar_5;
          float4 tmpvar_6;
          tmpvar_5.xzw = in_v.vertex.xzw;
          tmpvar_6.zw = in_v.texcoord.zw;
          tmpvar_5.y = (in_v.vertex.y + (_Shift * (_SinTime.y * _SpeedShift)));
          float tmpvar_7;
          tmpvar_7 = (_SinTime.y * _SpeedOffset);
          tmpvar_6.x = (in_v.texcoord.x + ((_Delta * sin(in_v.vertex.x)) * tmpvar_7));
          tmpvar_6.y = (in_v.texcoord.y + ((_Delta * sin(in_v.vertex.z)) * tmpvar_7));
          float4 tmpvar_8;
          tmpvar_8.w = 1;
          tmpvar_8.xyz = float3(tmpvar_5.xyz);
          tmpvar_4.xy = float2(TRANSFORM_TEX(tmpvar_6.xy, _MainTex));
          tmpvar_4.zw = TRANSFORM_TEX(tmpvar_6.xy, _BumpMap);
          float3x3 tmpvar_9;
          tmpvar_9[0] = conv_mxt4x4_0(unity_WorldToObject).xyz;
          tmpvar_9[1] = conv_mxt4x4_1(unity_WorldToObject).xyz;
          tmpvar_9[2] = conv_mxt4x4_2(unity_WorldToObject).xyz;
          float3 tmpvar_10;
          tmpvar_10 = normalize(mul(in_v.normal, tmpvar_9));
          float3x3 tmpvar_11;
          tmpvar_11[0] = conv_mxt4x4_0(unity_ObjectToWorld).xyz;
          tmpvar_11[1] = conv_mxt4x4_1(unity_ObjectToWorld).xyz;
          tmpvar_11[2] = conv_mxt4x4_2(unity_ObjectToWorld).xyz;
          float3 tmpvar_12;
          tmpvar_12 = normalize(mul(tmpvar_11, in_v.tangent.xyz));
          worldTangent_3 = tmpvar_12;
          float tmpvar_13;
          tmpvar_13 = (in_v.tangent.w * unity_WorldTransformParams.w);
          tangentSign_2 = tmpvar_13;
          float3 tmpvar_14;
          tmpvar_14 = (((tmpvar_10.yzx * worldTangent_3.zxy) - (tmpvar_10.zxy * worldTangent_3.yzx)) * tangentSign_2);
          worldBinormal_1 = tmpvar_14;
          float3 tmpvar_15;
          tmpvar_15.x = worldTangent_3.x;
          tmpvar_15.y = worldBinormal_1.x;
          tmpvar_15.z = tmpvar_10.x;
          float3 tmpvar_16;
          tmpvar_16.x = worldTangent_3.y;
          tmpvar_16.y = worldBinormal_1.y;
          tmpvar_16.z = tmpvar_10.y;
          float3 tmpvar_17;
          tmpvar_17.x = worldTangent_3.z;
          tmpvar_17.y = worldBinormal_1.z;
          tmpvar_17.z = tmpvar_10.z;
          out_v.vertex = mul(unity_MatrixVP, mul(unity_ObjectToWorld, tmpvar_8));
          out_v.xlv_TEXCOORD0 = tmpvar_4;
          out_v.xlv_TEXCOORD1 = tmpvar_15;
          out_v.xlv_TEXCOORD2 = tmpvar_16;
          out_v.xlv_TEXCOORD3 = tmpvar_17;
          out_v.xlv_TEXCOORD4 = mul(unity_ObjectToWorld, tmpvar_5).xyz;
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          float3 tmpvar_1;
          float3 tmpvar_2;
          float3 worldN_3;
          float3 lightCoord_4;
          float3 tmpvar_5;
          float3 lightDir_6;
          float3 tmpvar_7;
          float3 tmpvar_8;
          tmpvar_8 = normalize((_WorldSpaceLightPos0.xyz - in_f.xlv_TEXCOORD4));
          lightDir_6 = tmpvar_8;
          tmpvar_7 = (-normalize((_WorldSpaceCameraPos - in_f.xlv_TEXCOORD4)));
          float3 tmpvar_9;
          float4 c_10;
          float4 tmpvar_11;
          tmpvar_11 = tex2D(_MainTex, in_f.xlv_TEXCOORD0.xy);
          c_10 = tmpvar_11;
          tmpvar_9 = (c_10.xyz * _Color.xyz);
          float3 tmpvar_12;
          tmpvar_12 = ((tex2D(_BumpMap, in_f.xlv_TEXCOORD0.zw).xyz * 2) - 1).xyz;
          float4 tmpvar_13;
          tmpvar_13.w = 1;
          tmpvar_13.xyz = float3(in_f.xlv_TEXCOORD4);
          lightCoord_4 = mul(unity_WorldToLight, tmpvar_13).xyz.xyz;
          float tmpvar_14;
          tmpvar_14 = dot(lightCoord_4, lightCoord_4);
          float tmpvar_15;
          tmpvar_15 = tex2D(_LightTexture0, float2(tmpvar_14, tmpvar_14)).w.x;
          worldN_3.x = dot(in_f.xlv_TEXCOORD1, tmpvar_12);
          worldN_3.y = dot(in_f.xlv_TEXCOORD2, tmpvar_12);
          worldN_3.z = dot(in_f.xlv_TEXCOORD3, tmpvar_12);
          tmpvar_5 = worldN_3;
          tmpvar_1 = _LightColor0.xyz;
          tmpvar_2 = lightDir_6;
          tmpvar_1 = (tmpvar_1 * tmpvar_15);
          float4 c_16;
          float4 c_17;
          float diff_18;
          float tmpvar_19;
          tmpvar_19 = max(0, dot(tmpvar_5, tmpvar_2));
          diff_18 = tmpvar_19;
          c_17.xyz = float3(((tmpvar_9 * tmpvar_1) * diff_18));
          c_17.w = (texCUBE(_Cube, tmpvar_7).w * _ReflectColor.w);
          c_16.w = c_17.w;
          c_16.xyz = float3(c_17.xyz);
          out_f.color = c_16;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
    Pass // ind: 3, name: META
    {
      Name "META"
      Tags
      { 
        "IGNOREPROJECTOR" = "true"
        "LIGHTMODE" = "META"
        "QUEUE" = "Transparent"
        "RenderType" = "Transparent"
      }
      LOD 200
      ZClip Off
      ZWrite Off
      Cull Off
      ColorMask RGB
      // m_ProgramMask = 6
      CGPROGRAM
      //#pragma target 4.0
      
      #pragma vertex vert
      #pragma fragment frag
      
      #include "UnityCG.cginc"
      #define conv_mxt4x4_0(mat4x4) float4(mat4x4[0].x,mat4x4[1].x,mat4x4[2].x,mat4x4[3].x)
      #define conv_mxt4x4_1(mat4x4) float4(mat4x4[0].y,mat4x4[1].y,mat4x4[2].y,mat4x4[3].y)
      #define conv_mxt4x4_2(mat4x4) float4(mat4x4[0].z,mat4x4[1].z,mat4x4[2].z,mat4x4[3].z)
      
      
      #define CODE_BLOCK_VERTEX
      //uniform float4 _SinTime;
      //uniform float4x4 unity_ObjectToWorld;
      //uniform float4x4 unity_WorldToObject;
      //uniform float4 unity_WorldTransformParams;
      //uniform float4x4 unity_MatrixVP;
      // uniform float4 unity_LightmapST;
      // uniform float4 unity_DynamicLightmapST;
      uniform float _Shift;
      uniform float _Delta;
      uniform float _SpeedShift;
      uniform float _SpeedOffset;
      uniform float4 unity_MetaVertexControl;
      uniform float4 _MainTex_ST;
      //uniform float3 _WorldSpaceCameraPos;
      uniform samplerCUBE _Cube;
      uniform sampler2D _MainTex;
      uniform float4 _Color;
      uniform float4 _ReflectColor;
      uniform float _CubemapScale;
      uniform float4 unity_MetaFragmentControl;
      uniform float unity_OneOverOutputBoost;
      uniform float unity_MaxOutputValue;
      uniform float unity_UseLinearSpace;
      struct appdata_t
      {
          float4 tangent :TANGENT;
          float4 vertex :POSITION;
          float3 normal :NORMAL;
          float4 texcoord :TEXCOORD0;
          float4 texcoord1 :TEXCOORD1;
          float4 texcoord2 :TEXCOORD2;
      };
      
      struct OUT_Data_Vert
      {
          float2 xlv_TEXCOORD0 :TEXCOORD0;
          float4 xlv_TEXCOORD1 :TEXCOORD1;
          float4 xlv_TEXCOORD2 :TEXCOORD2;
          float4 xlv_TEXCOORD3 :TEXCOORD3;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float2 xlv_TEXCOORD0 :TEXCOORD0;
          float4 xlv_TEXCOORD1 :TEXCOORD1;
          float4 xlv_TEXCOORD2 :TEXCOORD2;
          float4 xlv_TEXCOORD3 :TEXCOORD3;
      };
      
      struct OUT_Data_Frag
      {
          float4 color :SV_Target0;
      };
      
      OUT_Data_Vert vert(appdata_t in_v)
      {
          OUT_Data_Vert out_v;
          float3 worldBinormal_1;
          float tangentSign_2;
          float3 worldTangent_3;
          float4 tmpvar_4;
          float4 tmpvar_5;
          tmpvar_4.xzw = in_v.vertex.xzw;
          tmpvar_5.zw = in_v.texcoord.zw;
          tmpvar_4.y = (in_v.vertex.y + (_Shift * (_SinTime.y * _SpeedShift)));
          float tmpvar_6;
          tmpvar_6 = (_SinTime.y * _SpeedOffset);
          tmpvar_5.x = (in_v.texcoord.x + ((_Delta * sin(in_v.vertex.x)) * tmpvar_6));
          tmpvar_5.y = (in_v.texcoord.y + ((_Delta * sin(in_v.vertex.z)) * tmpvar_6));
          float4 vertex_7;
          vertex_7 = tmpvar_4;
          if(unity_MetaVertexControl.x)
          {
              vertex_7.xy = float2(((in_v.texcoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw));
              float tmpvar_8;
              if((in_v.vertex.z>0))
              {
                  tmpvar_8 = 0.0001;
              }
              else
              {
                  tmpvar_8 = 0;
              }
              vertex_7.z = tmpvar_8;
          }
          if(unity_MetaVertexControl.y)
          {
              vertex_7.xy = float2(((in_v.texcoord2.xy * unity_DynamicLightmapST.xy) + unity_DynamicLightmapST.zw));
              float tmpvar_9;
              if((vertex_7.z>0))
              {
                  tmpvar_9 = 0.0001;
              }
              else
              {
                  tmpvar_9 = 0;
              }
              vertex_7.z = tmpvar_9;
          }
          float4 tmpvar_10;
          tmpvar_10.w = 1;
          tmpvar_10.xyz = float3(vertex_7.xyz);
          float3 tmpvar_11;
          tmpvar_11 = mul(unity_ObjectToWorld, tmpvar_4).xyz.xyz;
          float3x3 tmpvar_12;
          tmpvar_12[0] = conv_mxt4x4_0(unity_WorldToObject).xyz;
          tmpvar_12[1] = conv_mxt4x4_1(unity_WorldToObject).xyz;
          tmpvar_12[2] = conv_mxt4x4_2(unity_WorldToObject).xyz;
          float3 tmpvar_13;
          tmpvar_13 = normalize(mul(in_v.normal, tmpvar_12));
          float3x3 tmpvar_14;
          tmpvar_14[0] = conv_mxt4x4_0(unity_ObjectToWorld).xyz;
          tmpvar_14[1] = conv_mxt4x4_1(unity_ObjectToWorld).xyz;
          tmpvar_14[2] = conv_mxt4x4_2(unity_ObjectToWorld).xyz;
          float3 tmpvar_15;
          tmpvar_15 = normalize(mul(tmpvar_14, in_v.tangent.xyz));
          worldTangent_3 = tmpvar_15;
          float tmpvar_16;
          tmpvar_16 = (in_v.tangent.w * unity_WorldTransformParams.w);
          tangentSign_2 = tmpvar_16;
          float3 tmpvar_17;
          tmpvar_17 = (((tmpvar_13.yzx * worldTangent_3.zxy) - (tmpvar_13.zxy * worldTangent_3.yzx)) * tangentSign_2);
          worldBinormal_1 = tmpvar_17;
          float4 tmpvar_18;
          tmpvar_18.x = worldTangent_3.x;
          tmpvar_18.y = worldBinormal_1.x;
          tmpvar_18.z = tmpvar_13.x;
          tmpvar_18.w = tmpvar_11.x;
          float4 tmpvar_19;
          tmpvar_19.x = worldTangent_3.y;
          tmpvar_19.y = worldBinormal_1.y;
          tmpvar_19.z = tmpvar_13.y;
          tmpvar_19.w = tmpvar_11.y;
          float4 tmpvar_20;
          tmpvar_20.x = worldTangent_3.z;
          tmpvar_20.y = worldBinormal_1.z;
          tmpvar_20.z = tmpvar_13.z;
          tmpvar_20.w = tmpvar_11.z;
          out_v.vertex = mul(unity_MatrixVP, mul(unity_ObjectToWorld, tmpvar_10));
          out_v.xlv_TEXCOORD0 = TRANSFORM_TEX(tmpvar_5.xy, _MainTex);
          out_v.xlv_TEXCOORD1 = tmpvar_18;
          out_v.xlv_TEXCOORD2 = tmpvar_19;
          out_v.xlv_TEXCOORD3 = tmpvar_20;
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          float4 tmpvar_1;
          float3 tmpvar_2;
          float3 tmpvar_3;
          float3 tmpvar_4;
          float3 tmpvar_5;
          tmpvar_5.x = in_f.xlv_TEXCOORD1.w;
          tmpvar_5.y = in_f.xlv_TEXCOORD2.w;
          tmpvar_5.z = in_f.xlv_TEXCOORD3.w;
          tmpvar_4 = (-normalize((_WorldSpaceCameraPos - tmpvar_5)));
          float3 tmpvar_6;
          float3 tmpvar_7;
          float4 c_8;
          float4 tmpvar_9;
          tmpvar_9 = tex2D(_MainTex, in_f.xlv_TEXCOORD0);
          c_8 = tmpvar_9;
          tmpvar_6 = (c_8.xyz * _Color.xyz);
          float _tmp_dvx_22 = ((texCUBE(_Cube, tmpvar_4).xyz * _ReflectColor.xyz) * _CubemapScale);
          tmpvar_7 = float3(_tmp_dvx_22, _tmp_dvx_22, _tmp_dvx_22);
          tmpvar_2 = tmpvar_6;
          tmpvar_3 = tmpvar_7;
          float4 res_10;
          res_10 = float4(0, 0, 0, 0);
          if(unity_MetaFragmentControl.x)
          {
              float4 tmpvar_11;
              tmpvar_11.w = 1;
              tmpvar_11.xyz = float3(tmpvar_2);
              res_10.w = tmpvar_11.w;
              float3 tmpvar_12;
              tmpvar_12 = clamp(pow(tmpvar_2, clamp(unity_OneOverOutputBoost, 0, 1)), float3(0, 0, 0), float3(unity_MaxOutputValue, unity_MaxOutputValue, unity_MaxOutputValue));
              res_10.xyz = float3(tmpvar_12);
          }
          if(unity_MetaFragmentControl.y)
          {
              float3 emission_13;
              if(int(unity_UseLinearSpace))
              {
                  emission_13 = tmpvar_3;
              }
              else
              {
                  emission_13 = (tmpvar_3 * ((tmpvar_3 * ((tmpvar_3 * 0.305306) + 0.6821711)) + 0.01252288));
              }
              float4 tmpvar_14;
              float alpha_15;
              float3 tmpvar_16;
              tmpvar_16 = (emission_13 * 0.01030928);
              alpha_15 = (ceil((max(max(tmpvar_16.x, tmpvar_16.y), max(tmpvar_16.z, 0.02)) * 255)) / 255);
              float tmpvar_17;
              tmpvar_17 = max(alpha_15, 0.02);
              alpha_15 = tmpvar_17;
              float4 tmpvar_18;
              tmpvar_18.xyz = float3((tmpvar_16 / tmpvar_17));
              tmpvar_18.w = tmpvar_17;
              tmpvar_14 = tmpvar_18;
              res_10 = tmpvar_14;
          }
          tmpvar_1 = res_10;
          out_f.color = tmpvar_1;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack "Legacy Shaders/Transparent/VertexLit"
}
