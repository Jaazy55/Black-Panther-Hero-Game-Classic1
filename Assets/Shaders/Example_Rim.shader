// Upgrade NOTE: commented out 'float4 unity_DynamicLightmapST', a built-in variable
// Upgrade NOTE: commented out 'float4 unity_LightmapST', a built-in variable

Shader "Example/Rim"
{
  Properties
  {
    _MainTex ("Texture", 2D) = "white" {}
    _BumpMap ("Bumpmap", 2D) = "bump" {}
    _RimColor ("Rim Color", Color) = (0.26,0.19,0.16,0)
    _RimPower ("Rim Power", Range(0.5, 8)) = 3
  }
  SubShader
  {
    Tags
    { 
      "RenderType" = "Opaque"
    }
    Pass // ind: 1, name: FORWARD
    {
      Name "FORWARD"
      Tags
      { 
        "LIGHTMODE" = "FORWARDBASE"
        "RenderType" = "Opaque"
        "SHADOWSUPPORT" = "true"
      }
      ZClip Off
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
      //uniform float4 unity_SHAr;
      //uniform float4 unity_SHAg;
      //uniform float4 unity_SHAb;
      //uniform float4 unity_SHBr;
      //uniform float4 unity_SHBg;
      //uniform float4 unity_SHBb;
      //uniform float4 unity_SHC;
      //uniform float4x4 unity_ObjectToWorld;
      //uniform float4x4 unity_WorldToObject;
      //uniform float4 unity_WorldTransformParams;
      //uniform float4x4 unity_MatrixVP;
      uniform float4 _MainTex_ST;
      uniform float4 _BumpMap_ST;
      //uniform float3 _WorldSpaceCameraPos;
      //uniform float4 _WorldSpaceLightPos0;
      uniform float4 _LightColor0;
      uniform sampler2D _MainTex;
      uniform sampler2D _BumpMap;
      uniform float4 _RimColor;
      uniform float _RimPower;
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
          float2 xlv_TEXCOORD5 :TEXCOORD5;
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
          float2 tmpvar_5;
          float4 tmpvar_6;
          tmpvar_6.w = 1;
          tmpvar_6.xyz = float3(in_v.vertex.xyz);
          tmpvar_4.xy = float2(TRANSFORM_TEX(in_v.texcoord.xy, _MainTex));
          tmpvar_4.zw = TRANSFORM_TEX(in_v.texcoord.xy, _BumpMap);
          float3 tmpvar_7;
          tmpvar_7 = mul(unity_ObjectToWorld, in_v.vertex).xyz.xyz;
          float3x3 tmpvar_8;
          tmpvar_8[0] = conv_mxt4x4_0(unity_WorldToObject).xyz;
          tmpvar_8[1] = conv_mxt4x4_1(unity_WorldToObject).xyz;
          tmpvar_8[2] = conv_mxt4x4_2(unity_WorldToObject).xyz;
          float3 tmpvar_9;
          tmpvar_9 = normalize(mul(in_v.normal, tmpvar_8));
          float3x3 tmpvar_10;
          tmpvar_10[0] = conv_mxt4x4_0(unity_ObjectToWorld).xyz;
          tmpvar_10[1] = conv_mxt4x4_1(unity_ObjectToWorld).xyz;
          tmpvar_10[2] = conv_mxt4x4_2(unity_ObjectToWorld).xyz;
          float3 tmpvar_11;
          tmpvar_11 = normalize(mul(tmpvar_10, in_v.tangent.xyz));
          worldTangent_3 = tmpvar_11;
          float tmpvar_12;
          tmpvar_12 = (in_v.tangent.w * unity_WorldTransformParams.w);
          tangentSign_2 = tmpvar_12;
          float3 tmpvar_13;
          tmpvar_13 = (((tmpvar_9.yzx * worldTangent_3.zxy) - (tmpvar_9.zxy * worldTangent_3.yzx)) * tangentSign_2);
          worldBinormal_1 = tmpvar_13;
          float4 tmpvar_14;
          tmpvar_14.x = worldTangent_3.x;
          tmpvar_14.y = worldBinormal_1.x;
          tmpvar_14.z = tmpvar_9.x;
          tmpvar_14.w = tmpvar_7.x;
          float4 tmpvar_15;
          tmpvar_15.x = worldTangent_3.y;
          tmpvar_15.y = worldBinormal_1.y;
          tmpvar_15.z = tmpvar_9.y;
          tmpvar_15.w = tmpvar_7.y;
          float4 tmpvar_16;
          tmpvar_16.x = worldTangent_3.z;
          tmpvar_16.y = worldBinormal_1.z;
          tmpvar_16.z = tmpvar_9.z;
          tmpvar_16.w = tmpvar_7.z;
          float3 normal_17;
          normal_17 = tmpvar_9;
          float4 tmpvar_18;
          tmpvar_18.w = 1;
          tmpvar_18.xyz = float3(normal_17);
          float3 res_19;
          float3 x_20;
          x_20.x = dot(unity_SHAr, tmpvar_18);
          x_20.y = dot(unity_SHAg, tmpvar_18);
          x_20.z = dot(unity_SHAb, tmpvar_18);
          float3 x1_21;
          float4 tmpvar_22;
          tmpvar_22 = (normal_17.xyzz * normal_17.yzzx);
          x1_21.x = dot(unity_SHBr, tmpvar_22);
          x1_21.y = dot(unity_SHBg, tmpvar_22);
          x1_21.z = dot(unity_SHBb, tmpvar_22);
          res_19 = (x_20 + (x1_21 + (unity_SHC.xyz * ((normal_17.x * normal_17.x) - (normal_17.y * normal_17.y)))));
          float3 tmpvar_23;
          tmpvar_23 = max(((1.055 * pow(max(res_19, float3(0, 0, 0)), float3(0.4166667, 0.4166667, 0.4166667))) - 0.055), float3(0, 0, 0));
          res_19 = tmpvar_23;
          out_v.vertex = mul(unity_MatrixVP, mul(unity_ObjectToWorld, tmpvar_6));
          out_v.xlv_TEXCOORD0 = tmpvar_4;
          out_v.xlv_TEXCOORD1 = tmpvar_14;
          out_v.xlv_TEXCOORD2 = tmpvar_15;
          out_v.xlv_TEXCOORD3 = tmpvar_16;
          out_v.xlv_TEXCOORD4 = max(float3(0, 0, 0), tmpvar_23);
          out_v.xlv_TEXCOORD5 = tmpvar_5;
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
          float3 tmpvar_6;
          float3 lightDir_7;
          float3 tmpvar_8;
          tmpvar_8.x = in_f.xlv_TEXCOORD1.w;
          tmpvar_8.y = in_f.xlv_TEXCOORD2.w;
          tmpvar_8.z = in_f.xlv_TEXCOORD3.w;
          float3 tmpvar_9;
          tmpvar_9 = _WorldSpaceLightPos0.xyz;
          lightDir_7 = tmpvar_9;
          float3 tmpvar_10;
          tmpvar_10 = normalize((_WorldSpaceCameraPos - tmpvar_8));
          float3 tmpvar_11;
          float3 tmpvar_12;
          tmpvar_12 = ((tex2D(_BumpMap, in_f.xlv_TEXCOORD0.zw).xyz * 2) - 1).xyz;
          float tmpvar_13;
          tmpvar_13 = clamp(dot(normalize((((in_f.xlv_TEXCOORD1.xyz * tmpvar_10.x) + (in_f.xlv_TEXCOORD2.xyz * tmpvar_10.y)) + (in_f.xlv_TEXCOORD3.xyz * tmpvar_10.z))), tmpvar_12), 0, 1);
          float tmpvar_14;
          tmpvar_14 = (1 - tmpvar_13);
          float tmpvar_15;
          tmpvar_15 = pow(tmpvar_14, _RimPower);
          tmpvar_11 = (_RimColor.xyz * tmpvar_15);
          tmpvar_5 = tex2D(_MainTex, in_f.xlv_TEXCOORD0.xy).xyz.xyz;
          worldN_3.x = dot(in_f.xlv_TEXCOORD1.xyz, tmpvar_12);
          worldN_3.y = dot(in_f.xlv_TEXCOORD2.xyz, tmpvar_12);
          worldN_3.z = dot(in_f.xlv_TEXCOORD3.xyz, tmpvar_12);
          tmpvar_6 = worldN_3;
          tmpvar_1 = _LightColor0.xyz;
          tmpvar_2 = lightDir_7;
          float4 c_16;
          float4 c_17;
          float diff_18;
          float tmpvar_19;
          tmpvar_19 = max(0, dot(tmpvar_6, tmpvar_2));
          diff_18 = tmpvar_19;
          c_17.xyz = float3(((tmpvar_5 * tmpvar_1) * diff_18));
          c_17.w = 0;
          c_16.w = c_17.w;
          c_16.xyz = float3((c_17.xyz + (tmpvar_5 * in_f.xlv_TEXCOORD4)));
          c_4.xyz = float3((c_16.xyz + tmpvar_11));
          c_4.w = 1;
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
        "LIGHTMODE" = "FORWARDADD"
        "RenderType" = "Opaque"
      }
      ZClip Off
      ZWrite Off
      Blend One One
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
      //uniform float4x4 unity_ObjectToWorld;
      //uniform float4x4 unity_WorldToObject;
      //uniform float4 unity_WorldTransformParams;
      //uniform float4x4 unity_MatrixVP;
      uniform float4 _MainTex_ST;
      uniform float4 _BumpMap_ST;
      //uniform float4 _WorldSpaceLightPos0;
      uniform float4 _LightColor0;
      uniform sampler2D _LightTexture0;
      uniform float4x4 unity_WorldToLight;
      uniform sampler2D _MainTex;
      uniform sampler2D _BumpMap;
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
          float2 xlv_TEXCOORD5 :TEXCOORD5;
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
          float2 tmpvar_5;
          float4 tmpvar_6;
          tmpvar_6.w = 1;
          tmpvar_6.xyz = float3(in_v.vertex.xyz);
          tmpvar_4.xy = float2(TRANSFORM_TEX(in_v.texcoord.xy, _MainTex));
          tmpvar_4.zw = TRANSFORM_TEX(in_v.texcoord.xy, _BumpMap);
          float3x3 tmpvar_7;
          tmpvar_7[0] = conv_mxt4x4_0(unity_WorldToObject).xyz;
          tmpvar_7[1] = conv_mxt4x4_1(unity_WorldToObject).xyz;
          tmpvar_7[2] = conv_mxt4x4_2(unity_WorldToObject).xyz;
          float3 tmpvar_8;
          tmpvar_8 = normalize(mul(in_v.normal, tmpvar_7));
          float3x3 tmpvar_9;
          tmpvar_9[0] = conv_mxt4x4_0(unity_ObjectToWorld).xyz;
          tmpvar_9[1] = conv_mxt4x4_1(unity_ObjectToWorld).xyz;
          tmpvar_9[2] = conv_mxt4x4_2(unity_ObjectToWorld).xyz;
          float3 tmpvar_10;
          tmpvar_10 = normalize(mul(tmpvar_9, in_v.tangent.xyz));
          worldTangent_3 = tmpvar_10;
          float tmpvar_11;
          tmpvar_11 = (in_v.tangent.w * unity_WorldTransformParams.w);
          tangentSign_2 = tmpvar_11;
          float3 tmpvar_12;
          tmpvar_12 = (((tmpvar_8.yzx * worldTangent_3.zxy) - (tmpvar_8.zxy * worldTangent_3.yzx)) * tangentSign_2);
          worldBinormal_1 = tmpvar_12;
          float3 tmpvar_13;
          tmpvar_13.x = worldTangent_3.x;
          tmpvar_13.y = worldBinormal_1.x;
          tmpvar_13.z = tmpvar_8.x;
          float3 tmpvar_14;
          tmpvar_14.x = worldTangent_3.y;
          tmpvar_14.y = worldBinormal_1.y;
          tmpvar_14.z = tmpvar_8.y;
          float3 tmpvar_15;
          tmpvar_15.x = worldTangent_3.z;
          tmpvar_15.y = worldBinormal_1.z;
          tmpvar_15.z = tmpvar_8.z;
          out_v.vertex = mul(unity_MatrixVP, mul(unity_ObjectToWorld, tmpvar_6));
          out_v.xlv_TEXCOORD0 = tmpvar_4;
          out_v.xlv_TEXCOORD1 = tmpvar_13;
          out_v.xlv_TEXCOORD2 = tmpvar_14;
          out_v.xlv_TEXCOORD3 = tmpvar_15;
          out_v.xlv_TEXCOORD4 = mul(unity_ObjectToWorld, in_v.vertex).xyz;
          out_v.xlv_TEXCOORD5 = tmpvar_5;
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
          float3 lightCoord_5;
          float3 tmpvar_6;
          float3 lightDir_7;
          float3 tmpvar_8;
          tmpvar_8 = normalize((_WorldSpaceLightPos0.xyz - in_f.xlv_TEXCOORD4));
          lightDir_7 = tmpvar_8;
          float3 tmpvar_9;
          tmpvar_9 = ((tex2D(_BumpMap, in_f.xlv_TEXCOORD0.zw).xyz * 2) - 1).xyz;
          float4 tmpvar_10;
          tmpvar_10.w = 1;
          tmpvar_10.xyz = float3(in_f.xlv_TEXCOORD4);
          lightCoord_5 = mul(unity_WorldToLight, tmpvar_10).xyz.xyz;
          float tmpvar_11;
          tmpvar_11 = dot(lightCoord_5, lightCoord_5);
          float tmpvar_12;
          tmpvar_12 = tex2D(_LightTexture0, float2(tmpvar_11, tmpvar_11)).w.x;
          worldN_3.x = dot(in_f.xlv_TEXCOORD1, tmpvar_9);
          worldN_3.y = dot(in_f.xlv_TEXCOORD2, tmpvar_9);
          worldN_3.z = dot(in_f.xlv_TEXCOORD3, tmpvar_9);
          tmpvar_6 = worldN_3;
          tmpvar_1 = _LightColor0.xyz;
          tmpvar_2 = lightDir_7;
          tmpvar_1 = (tmpvar_1 * tmpvar_12);
          float4 c_13;
          float4 c_14;
          float diff_15;
          float tmpvar_16;
          tmpvar_16 = max(0, dot(tmpvar_6, tmpvar_2));
          diff_15 = tmpvar_16;
          c_14.xyz = float3(((tex2D(_MainTex, in_f.xlv_TEXCOORD0.xy).xyz * tmpvar_1) * diff_15));
          c_14.w = 0;
          c_13.w = c_14.w;
          c_13.xyz = float3(c_14.xyz);
          c_4.xyz = float3(c_13.xyz);
          c_4.w = 1;
          out_f.color = c_4;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
    Pass // ind: 3, name: PREPASS
    {
      Name "PREPASS"
      Tags
      { 
        "LIGHTMODE" = "PREPASSBASE"
        "RenderType" = "Opaque"
      }
      ZClip Off
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
      //uniform float4x4 unity_ObjectToWorld;
      //uniform float4x4 unity_WorldToObject;
      //uniform float4 unity_WorldTransformParams;
      //uniform float4x4 unity_MatrixVP;
      uniform float4 _BumpMap_ST;
      uniform sampler2D _BumpMap;
      struct appdata_t
      {
          float4 tangent :TANGENT;
          float4 vertex :POSITION;
          float3 normal :NORMAL;
          float4 texcoord :TEXCOORD0;
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
          tmpvar_4.w = 1;
          tmpvar_4.xyz = float3(in_v.vertex.xyz);
          float3 tmpvar_5;
          tmpvar_5 = mul(unity_ObjectToWorld, in_v.vertex).xyz.xyz;
          float3x3 tmpvar_6;
          tmpvar_6[0] = conv_mxt4x4_0(unity_WorldToObject).xyz;
          tmpvar_6[1] = conv_mxt4x4_1(unity_WorldToObject).xyz;
          tmpvar_6[2] = conv_mxt4x4_2(unity_WorldToObject).xyz;
          float3 tmpvar_7;
          tmpvar_7 = normalize(mul(in_v.normal, tmpvar_6));
          float3x3 tmpvar_8;
          tmpvar_8[0] = conv_mxt4x4_0(unity_ObjectToWorld).xyz;
          tmpvar_8[1] = conv_mxt4x4_1(unity_ObjectToWorld).xyz;
          tmpvar_8[2] = conv_mxt4x4_2(unity_ObjectToWorld).xyz;
          float3 tmpvar_9;
          tmpvar_9 = normalize(mul(tmpvar_8, in_v.tangent.xyz));
          worldTangent_3 = tmpvar_9;
          float tmpvar_10;
          tmpvar_10 = (in_v.tangent.w * unity_WorldTransformParams.w);
          tangentSign_2 = tmpvar_10;
          float3 tmpvar_11;
          tmpvar_11 = (((tmpvar_7.yzx * worldTangent_3.zxy) - (tmpvar_7.zxy * worldTangent_3.yzx)) * tangentSign_2);
          worldBinormal_1 = tmpvar_11;
          float4 tmpvar_12;
          tmpvar_12.x = worldTangent_3.x;
          tmpvar_12.y = worldBinormal_1.x;
          tmpvar_12.z = tmpvar_7.x;
          tmpvar_12.w = tmpvar_5.x;
          float4 tmpvar_13;
          tmpvar_13.x = worldTangent_3.y;
          tmpvar_13.y = worldBinormal_1.y;
          tmpvar_13.z = tmpvar_7.y;
          tmpvar_13.w = tmpvar_5.y;
          float4 tmpvar_14;
          tmpvar_14.x = worldTangent_3.z;
          tmpvar_14.y = worldBinormal_1.z;
          tmpvar_14.z = tmpvar_7.z;
          tmpvar_14.w = tmpvar_5.z;
          out_v.vertex = mul(unity_MatrixVP, mul(unity_ObjectToWorld, tmpvar_4));
          out_v.xlv_TEXCOORD0 = TRANSFORM_TEX(in_v.texcoord.xy, _BumpMap);
          out_v.xlv_TEXCOORD1 = tmpvar_12;
          out_v.xlv_TEXCOORD2 = tmpvar_13;
          out_v.xlv_TEXCOORD3 = tmpvar_14;
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          float4 res_1;
          float3 worldN_2;
          float3 tmpvar_3;
          float3 tmpvar_4;
          tmpvar_4 = ((tex2D(_BumpMap, in_f.xlv_TEXCOORD0).xyz * 2) - 1).xyz;
          worldN_2.x = dot(in_f.xlv_TEXCOORD1.xyz, tmpvar_4);
          worldN_2.y = dot(in_f.xlv_TEXCOORD2.xyz, tmpvar_4);
          worldN_2.z = dot(in_f.xlv_TEXCOORD3.xyz, tmpvar_4);
          tmpvar_3 = worldN_2;
          res_1.xyz = float3(((tmpvar_3 * 0.5) + 0.5));
          res_1.w = 0;
          out_f.color = res_1;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
    Pass // ind: 4, name: PREPASS
    {
      Name "PREPASS"
      Tags
      { 
        "LIGHTMODE" = "PREPASSFINAL"
        "RenderType" = "Opaque"
      }
      ZClip Off
      ZWrite Off
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
      //uniform float3 _WorldSpaceCameraPos;
      //uniform float4 _ProjectionParams;
      //uniform float4 unity_SHAr;
      //uniform float4 unity_SHAg;
      //uniform float4 unity_SHAb;
      //uniform float4 unity_SHBr;
      //uniform float4 unity_SHBg;
      //uniform float4 unity_SHBb;
      //uniform float4 unity_SHC;
      //uniform float4x4 unity_ObjectToWorld;
      //uniform float4x4 unity_WorldToObject;
      //uniform float4 unity_WorldTransformParams;
      //uniform float4x4 unity_MatrixVP;
      uniform float4 _MainTex_ST;
      uniform float4 _BumpMap_ST;
      uniform sampler2D _MainTex;
      uniform sampler2D _BumpMap;
      uniform float4 _RimColor;
      uniform float _RimPower;
      uniform sampler2D _LightBuffer;
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
          float4 xlv_TEXCOORD3 :TEXCOORD3;
          float4 xlv_TEXCOORD4 :TEXCOORD4;
          float3 xlv_TEXCOORD5 :TEXCOORD5;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float4 xlv_TEXCOORD0 :TEXCOORD0;
          float3 xlv_TEXCOORD2 :TEXCOORD2;
          float4 xlv_TEXCOORD3 :TEXCOORD3;
          float3 xlv_TEXCOORD5 :TEXCOORD5;
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
          float3 tmpvar_5;
          float4 tmpvar_6;
          float3 tmpvar_7;
          float4 tmpvar_8;
          float4 tmpvar_9;
          tmpvar_9.w = 1;
          tmpvar_9.xyz = float3(in_v.vertex.xyz);
          tmpvar_8 = mul(unity_MatrixVP, mul(unity_ObjectToWorld, tmpvar_9));
          tmpvar_4.xy = float2(TRANSFORM_TEX(in_v.texcoord.xy, _MainTex));
          tmpvar_4.zw = TRANSFORM_TEX(in_v.texcoord.xy, _BumpMap);
          float3 tmpvar_10;
          tmpvar_10 = mul(unity_ObjectToWorld, in_v.vertex).xyz.xyz;
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
          float3 tmpvar_17;
          tmpvar_17 = (_WorldSpaceCameraPos - tmpvar_10);
          float tmpvar_18;
          tmpvar_18 = dot(tmpvar_17, worldTangent_3);
          tmpvar_5.x = tmpvar_18;
          float tmpvar_19;
          tmpvar_19 = dot(tmpvar_17, worldBinormal_1);
          tmpvar_5.y = tmpvar_19;
          float tmpvar_20;
          tmpvar_20 = dot(tmpvar_17, tmpvar_12);
          tmpvar_5.z = tmpvar_20;
          float4 o_21;
          float4 tmpvar_22;
          tmpvar_22 = (tmpvar_8 * 0.5);
          float2 tmpvar_23;
          tmpvar_23.x = tmpvar_22.x;
          tmpvar_23.y = (tmpvar_22.y * _ProjectionParams.x);
          o_21.xy = float2((tmpvar_23 + tmpvar_22.w));
          o_21.zw = tmpvar_8.zw;
          tmpvar_6.zw = float2(0, 0);
          tmpvar_6.xy = float2(0, 0);
          float3x3 tmpvar_24;
          tmpvar_24[0] = conv_mxt4x4_0(unity_WorldToObject).xyz;
          tmpvar_24[1] = conv_mxt4x4_1(unity_WorldToObject).xyz;
          tmpvar_24[2] = conv_mxt4x4_2(unity_WorldToObject).xyz;
          float4 tmpvar_25;
          tmpvar_25.w = 1;
          tmpvar_25.xyz = float3(normalize(mul(in_v.normal, tmpvar_24)));
          float4 normal_26;
          normal_26 = tmpvar_25;
          float3 res_27;
          float3 x_28;
          x_28.x = dot(unity_SHAr, normal_26);
          x_28.y = dot(unity_SHAg, normal_26);
          x_28.z = dot(unity_SHAb, normal_26);
          float3 x1_29;
          float4 tmpvar_30;
          tmpvar_30 = (normal_26.xyzz * normal_26.yzzx);
          x1_29.x = dot(unity_SHBr, tmpvar_30);
          x1_29.y = dot(unity_SHBg, tmpvar_30);
          x1_29.z = dot(unity_SHBb, tmpvar_30);
          res_27 = (x_28 + (x1_29 + (unity_SHC.xyz * ((normal_26.x * normal_26.x) - (normal_26.y * normal_26.y)))));
          float3 tmpvar_31;
          tmpvar_31 = max(((1.055 * pow(max(res_27, float3(0, 0, 0)), float3(0.4166667, 0.4166667, 0.4166667))) - 0.055), float3(0, 0, 0));
          res_27 = tmpvar_31;
          tmpvar_7 = tmpvar_31;
          out_v.vertex = tmpvar_8;
          out_v.xlv_TEXCOORD0 = tmpvar_4;
          out_v.xlv_TEXCOORD1 = tmpvar_10;
          out_v.xlv_TEXCOORD2 = tmpvar_5;
          out_v.xlv_TEXCOORD3 = o_21;
          out_v.xlv_TEXCOORD4 = tmpvar_6;
          out_v.xlv_TEXCOORD5 = tmpvar_7;
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          float4 tmpvar_1;
          float4 c_2;
          float4 light_3;
          float3 viewDir_4;
          float3 tmpvar_5;
          float3 tmpvar_6;
          tmpvar_6 = normalize(in_f.xlv_TEXCOORD2);
          viewDir_4 = tmpvar_6;
          tmpvar_5 = viewDir_4;
          float3 tmpvar_7;
          float3 tmpvar_8;
          tmpvar_8 = ((tex2D(_BumpMap, in_f.xlv_TEXCOORD0.zw).xyz * 2) - 1).xyz;
          float tmpvar_9;
          tmpvar_9 = clamp(dot(normalize(tmpvar_5), tmpvar_8), 0, 1);
          float tmpvar_10;
          tmpvar_10 = (1 - tmpvar_9);
          float tmpvar_11;
          tmpvar_11 = pow(tmpvar_10, _RimPower);
          tmpvar_7 = (_RimColor.xyz * tmpvar_11);
          float4 tmpvar_12;
          tmpvar_12 = tex2D(_LightBuffer, in_f.xlv_TEXCOORD3);
          light_3 = tmpvar_12;
          light_3 = (-log2(max(light_3, float4(0.001, 0.001, 0.001, 0.001))));
          light_3.xyz = float3((light_3.xyz + in_f.xlv_TEXCOORD5));
          float4 c_13;
          c_13.xyz = float3((tex2D(_MainTex, in_f.xlv_TEXCOORD0.xy).xyz * light_3.xyz));
          c_13.w = 0;
          c_2 = c_13;
          c_2.xyz = float3((c_2.xyz + tmpvar_7));
          c_2.w = 1;
          tmpvar_1 = c_2;
          out_f.color = tmpvar_1;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
    Pass // ind: 5, name: DEFERRED
    {
      Name "DEFERRED"
      Tags
      { 
        "LIGHTMODE" = "DEFERRED"
        "RenderType" = "Opaque"
      }
      ZClip Off
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
      //uniform float3 _WorldSpaceCameraPos;
      //uniform float4 unity_SHAr;
      //uniform float4 unity_SHAg;
      //uniform float4 unity_SHAb;
      //uniform float4 unity_SHBr;
      //uniform float4 unity_SHBg;
      //uniform float4 unity_SHBb;
      //uniform float4 unity_SHC;
      //uniform float4x4 unity_ObjectToWorld;
      //uniform float4x4 unity_WorldToObject;
      //uniform float4 unity_WorldTransformParams;
      //uniform float4x4 unity_MatrixVP;
      uniform float4 _MainTex_ST;
      uniform float4 _BumpMap_ST;
      uniform sampler2D _MainTex;
      uniform sampler2D _BumpMap;
      uniform float4 _RimColor;
      uniform float _RimPower;
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
          float4 xlv_TEXCOORD5 :TEXCOORD5;
          float3 xlv_TEXCOORD6 :TEXCOORD6;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float4 xlv_TEXCOORD0 :TEXCOORD0;
          float4 xlv_TEXCOORD1 :TEXCOORD1;
          float4 xlv_TEXCOORD2 :TEXCOORD2;
          float4 xlv_TEXCOORD3 :TEXCOORD3;
          float3 xlv_TEXCOORD4 :TEXCOORD4;
          float3 xlv_TEXCOORD6 :TEXCOORD6;
      };
      
      struct OUT_Data_Frag
      {
          float4 color :SV_Target0;
          float4 color1 :SV_Target1;
          float4 color2 :SV_Target2;
          float4 color3 :SV_Target3;
      };
      
      OUT_Data_Vert vert(appdata_t in_v)
      {
          OUT_Data_Vert out_v;
          float3 worldBinormal_1;
          float tangentSign_2;
          float3 worldTangent_3;
          float4 tmpvar_4;
          float3 tmpvar_5;
          float4 tmpvar_6;
          float4 tmpvar_7;
          tmpvar_7.w = 1;
          tmpvar_7.xyz = float3(in_v.vertex.xyz);
          tmpvar_4.xy = float2(TRANSFORM_TEX(in_v.texcoord.xy, _MainTex));
          tmpvar_4.zw = TRANSFORM_TEX(in_v.texcoord.xy, _BumpMap);
          float3 tmpvar_8;
          tmpvar_8 = mul(unity_ObjectToWorld, in_v.vertex).xyz.xyz;
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
          float4 tmpvar_15;
          tmpvar_15.x = worldTangent_3.x;
          tmpvar_15.y = worldBinormal_1.x;
          tmpvar_15.z = tmpvar_10.x;
          tmpvar_15.w = tmpvar_8.x;
          float4 tmpvar_16;
          tmpvar_16.x = worldTangent_3.y;
          tmpvar_16.y = worldBinormal_1.y;
          tmpvar_16.z = tmpvar_10.y;
          tmpvar_16.w = tmpvar_8.y;
          float4 tmpvar_17;
          tmpvar_17.x = worldTangent_3.z;
          tmpvar_17.y = worldBinormal_1.z;
          tmpvar_17.z = tmpvar_10.z;
          tmpvar_17.w = tmpvar_8.z;
          float3 tmpvar_18;
          tmpvar_18 = (_WorldSpaceCameraPos - tmpvar_8);
          float tmpvar_19;
          tmpvar_19 = dot(tmpvar_18, worldTangent_3);
          tmpvar_5.x = tmpvar_19;
          float tmpvar_20;
          tmpvar_20 = dot(tmpvar_18, worldBinormal_1);
          tmpvar_5.y = tmpvar_20;
          float tmpvar_21;
          tmpvar_21 = dot(tmpvar_18, tmpvar_10);
          tmpvar_5.z = tmpvar_21;
          tmpvar_6.zw = float2(0, 0);
          tmpvar_6.xy = float2(0, 0);
          float3 normal_22;
          normal_22 = tmpvar_10;
          float4 tmpvar_23;
          tmpvar_23.w = 1;
          tmpvar_23.xyz = float3(normal_22);
          float3 res_24;
          float3 x_25;
          x_25.x = dot(unity_SHAr, tmpvar_23);
          x_25.y = dot(unity_SHAg, tmpvar_23);
          x_25.z = dot(unity_SHAb, tmpvar_23);
          float3 x1_26;
          float4 tmpvar_27;
          tmpvar_27 = (normal_22.xyzz * normal_22.yzzx);
          x1_26.x = dot(unity_SHBr, tmpvar_27);
          x1_26.y = dot(unity_SHBg, tmpvar_27);
          x1_26.z = dot(unity_SHBb, tmpvar_27);
          res_24 = (x_25 + (x1_26 + (unity_SHC.xyz * ((normal_22.x * normal_22.x) - (normal_22.y * normal_22.y)))));
          float3 tmpvar_28;
          tmpvar_28 = max(((1.055 * pow(max(res_24, float3(0, 0, 0)), float3(0.4166667, 0.4166667, 0.4166667))) - 0.055), float3(0, 0, 0));
          res_24 = tmpvar_28;
          out_v.vertex = mul(unity_MatrixVP, mul(unity_ObjectToWorld, tmpvar_7));
          out_v.xlv_TEXCOORD0 = tmpvar_4;
          out_v.xlv_TEXCOORD1 = tmpvar_15;
          out_v.xlv_TEXCOORD2 = tmpvar_16;
          out_v.xlv_TEXCOORD3 = tmpvar_17;
          out_v.xlv_TEXCOORD4 = tmpvar_5;
          out_v.xlv_TEXCOORD5 = tmpvar_6;
          out_v.xlv_TEXCOORD6 = max(float3(0, 0, 0), tmpvar_28);
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          float4 outEmission_1;
          float3 worldN_2;
          float3 tmpvar_3;
          float3 viewDir_4;
          float3 tmpvar_5;
          float3 tmpvar_6;
          tmpvar_6 = normalize(in_f.xlv_TEXCOORD4);
          viewDir_4 = tmpvar_6;
          tmpvar_5 = viewDir_4;
          float3 tmpvar_7;
          float3 tmpvar_8;
          float4 tmpvar_9;
          tmpvar_9 = tex2D(_MainTex, in_f.xlv_TEXCOORD0.xy);
          tmpvar_7 = tmpvar_9.xyz;
          float3 tmpvar_10;
          tmpvar_10 = ((tex2D(_BumpMap, in_f.xlv_TEXCOORD0.zw).xyz * 2) - 1).xyz;
          float tmpvar_11;
          tmpvar_11 = clamp(dot(normalize(tmpvar_5), tmpvar_10), 0, 1);
          float tmpvar_12;
          tmpvar_12 = (1 - tmpvar_11);
          float tmpvar_13;
          tmpvar_13 = pow(tmpvar_12, _RimPower);
          tmpvar_8 = (_RimColor.xyz * tmpvar_13);
          worldN_2.x = dot(in_f.xlv_TEXCOORD1.xyz, tmpvar_10);
          worldN_2.y = dot(in_f.xlv_TEXCOORD2.xyz, tmpvar_10);
          worldN_2.z = dot(in_f.xlv_TEXCOORD3.xyz, tmpvar_10);
          tmpvar_3 = worldN_2;
          float4 emission_14;
          float3 tmpvar_15;
          float3 tmpvar_16;
          tmpvar_15 = tmpvar_7;
          tmpvar_16 = tmpvar_3;
          float4 outGBuffer2_17;
          float4 tmpvar_18;
          tmpvar_18.xyz = float3(tmpvar_15);
          tmpvar_18.w = 1;
          float4 tmpvar_19;
          tmpvar_19.xyz = float3(0, 0, 0);
          tmpvar_19.w = 0;
          float4 tmpvar_20;
          tmpvar_20.w = 1;
          tmpvar_20.xyz = float3(((tmpvar_16 * 0.5) + 0.5));
          outGBuffer2_17 = tmpvar_20;
          float4 tmpvar_21;
          tmpvar_21.w = 1;
          tmpvar_21.xyz = float3(tmpvar_8);
          emission_14 = tmpvar_21;
          emission_14.xyz = float3((emission_14.xyz + (tmpvar_9.xyz * in_f.xlv_TEXCOORD6)));
          outEmission_1.w = emission_14.w;
          outEmission_1.xyz = float3(exp2((-emission_14.xyz)));
          out_f.color = tmpvar_18;
          out_f.color1 = tmpvar_19;
          out_f.color2 = outGBuffer2_17;
          out_f.color3 = outEmission_1;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
    Pass // ind: 6, name: META
    {
      Name "META"
      Tags
      { 
        "LIGHTMODE" = "META"
        "RenderType" = "Opaque"
      }
      ZClip Off
      Cull Off
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
      //uniform float4x4 unity_ObjectToWorld;
      //uniform float4x4 unity_WorldToObject;
      //uniform float4 unity_WorldTransformParams;
      //uniform float4x4 unity_MatrixVP;
      // uniform float4 unity_LightmapST;
      // uniform float4 unity_DynamicLightmapST;
      uniform float4 unity_MetaVertexControl;
      uniform float4 _MainTex_ST;
      uniform float4 _BumpMap_ST;
      //uniform float3 _WorldSpaceCameraPos;
      uniform sampler2D _MainTex;
      uniform sampler2D _BumpMap;
      uniform float4 _RimColor;
      uniform float _RimPower;
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
          float4 xlv_TEXCOORD0 :TEXCOORD0;
          float4 xlv_TEXCOORD1 :TEXCOORD1;
          float4 xlv_TEXCOORD2 :TEXCOORD2;
          float4 xlv_TEXCOORD3 :TEXCOORD3;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float4 xlv_TEXCOORD0 :TEXCOORD0;
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
          float4 vertex_5;
          vertex_5 = in_v.vertex;
          if(unity_MetaVertexControl.x)
          {
              vertex_5.xy = float2(((in_v.texcoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw));
              float tmpvar_6;
              if((in_v.vertex.z>0))
              {
                  tmpvar_6 = 0.0001;
              }
              else
              {
                  tmpvar_6 = 0;
              }
              vertex_5.z = tmpvar_6;
          }
          if(unity_MetaVertexControl.y)
          {
              vertex_5.xy = float2(((in_v.texcoord2.xy * unity_DynamicLightmapST.xy) + unity_DynamicLightmapST.zw));
              float tmpvar_7;
              if((vertex_5.z>0))
              {
                  tmpvar_7 = 0.0001;
              }
              else
              {
                  tmpvar_7 = 0;
              }
              vertex_5.z = tmpvar_7;
          }
          float4 tmpvar_8;
          tmpvar_8.w = 1;
          tmpvar_8.xyz = float3(vertex_5.xyz);
          tmpvar_4.xy = float2(TRANSFORM_TEX(in_v.texcoord.xy, _MainTex));
          tmpvar_4.zw = TRANSFORM_TEX(in_v.texcoord.xy, _BumpMap);
          float3 tmpvar_9;
          tmpvar_9 = mul(unity_ObjectToWorld, in_v.vertex).xyz.xyz;
          float3x3 tmpvar_10;
          tmpvar_10[0] = conv_mxt4x4_0(unity_WorldToObject).xyz;
          tmpvar_10[1] = conv_mxt4x4_1(unity_WorldToObject).xyz;
          tmpvar_10[2] = conv_mxt4x4_2(unity_WorldToObject).xyz;
          float3 tmpvar_11;
          tmpvar_11 = normalize(mul(in_v.normal, tmpvar_10));
          float3x3 tmpvar_12;
          tmpvar_12[0] = conv_mxt4x4_0(unity_ObjectToWorld).xyz;
          tmpvar_12[1] = conv_mxt4x4_1(unity_ObjectToWorld).xyz;
          tmpvar_12[2] = conv_mxt4x4_2(unity_ObjectToWorld).xyz;
          float3 tmpvar_13;
          tmpvar_13 = normalize(mul(tmpvar_12, in_v.tangent.xyz));
          worldTangent_3 = tmpvar_13;
          float tmpvar_14;
          tmpvar_14 = (in_v.tangent.w * unity_WorldTransformParams.w);
          tangentSign_2 = tmpvar_14;
          float3 tmpvar_15;
          tmpvar_15 = (((tmpvar_11.yzx * worldTangent_3.zxy) - (tmpvar_11.zxy * worldTangent_3.yzx)) * tangentSign_2);
          worldBinormal_1 = tmpvar_15;
          float4 tmpvar_16;
          tmpvar_16.x = worldTangent_3.x;
          tmpvar_16.y = worldBinormal_1.x;
          tmpvar_16.z = tmpvar_11.x;
          tmpvar_16.w = tmpvar_9.x;
          float4 tmpvar_17;
          tmpvar_17.x = worldTangent_3.y;
          tmpvar_17.y = worldBinormal_1.y;
          tmpvar_17.z = tmpvar_11.y;
          tmpvar_17.w = tmpvar_9.y;
          float4 tmpvar_18;
          tmpvar_18.x = worldTangent_3.z;
          tmpvar_18.y = worldBinormal_1.z;
          tmpvar_18.z = tmpvar_11.z;
          tmpvar_18.w = tmpvar_9.z;
          out_v.vertex = mul(unity_MatrixVP, mul(unity_ObjectToWorld, tmpvar_8));
          out_v.xlv_TEXCOORD0 = tmpvar_4;
          out_v.xlv_TEXCOORD1 = tmpvar_16;
          out_v.xlv_TEXCOORD2 = tmpvar_17;
          out_v.xlv_TEXCOORD3 = tmpvar_18;
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
          tmpvar_4.x = in_f.xlv_TEXCOORD1.w;
          tmpvar_4.y = in_f.xlv_TEXCOORD2.w;
          tmpvar_4.z = in_f.xlv_TEXCOORD3.w;
          float3 tmpvar_5;
          tmpvar_5 = normalize((_WorldSpaceCameraPos - tmpvar_4));
          float3 tmpvar_6;
          float3 tmpvar_7;
          tmpvar_6 = tex2D(_MainTex, in_f.xlv_TEXCOORD0.xy).xyz.xyz;
          float3 tmpvar_8;
          tmpvar_8 = ((tex2D(_BumpMap, in_f.xlv_TEXCOORD0.zw).xyz * 2) - 1).xyz;
          float tmpvar_9;
          tmpvar_9 = clamp(dot(normalize((((in_f.xlv_TEXCOORD1.xyz * tmpvar_5.x) + (in_f.xlv_TEXCOORD2.xyz * tmpvar_5.y)) + (in_f.xlv_TEXCOORD3.xyz * tmpvar_5.z))), tmpvar_8), 0, 1);
          float tmpvar_10;
          tmpvar_10 = (1 - tmpvar_9);
          float tmpvar_11;
          tmpvar_11 = pow(tmpvar_10, _RimPower);
          tmpvar_7 = (_RimColor.xyz * tmpvar_11);
          tmpvar_2 = tmpvar_6;
          tmpvar_3 = tmpvar_7;
          float4 res_12;
          res_12 = float4(0, 0, 0, 0);
          if(unity_MetaFragmentControl.x)
          {
              float4 tmpvar_13;
              tmpvar_13.w = 1;
              tmpvar_13.xyz = float3(tmpvar_2);
              res_12.w = tmpvar_13.w;
              float3 tmpvar_14;
              tmpvar_14 = clamp(pow(tmpvar_2, clamp(unity_OneOverOutputBoost, 0, 1)), float3(0, 0, 0), float3(unity_MaxOutputValue, unity_MaxOutputValue, unity_MaxOutputValue));
              res_12.xyz = float3(tmpvar_14);
          }
          if(unity_MetaFragmentControl.y)
          {
              float3 emission_15;
              if(int(unity_UseLinearSpace))
              {
                  emission_15 = tmpvar_3;
              }
              else
              {
                  emission_15 = (tmpvar_3 * ((tmpvar_3 * ((tmpvar_3 * 0.305306) + 0.6821711)) + 0.01252288));
              }
              float4 tmpvar_16;
              float alpha_17;
              float3 tmpvar_18;
              tmpvar_18 = (emission_15 * 0.01030928);
              alpha_17 = (ceil((max(max(tmpvar_18.x, tmpvar_18.y), max(tmpvar_18.z, 0.02)) * 255)) / 255);
              float tmpvar_19;
              tmpvar_19 = max(alpha_17, 0.02);
              alpha_17 = tmpvar_19;
              float4 tmpvar_20;
              tmpvar_20.xyz = float3((tmpvar_18 / tmpvar_19));
              tmpvar_20.w = tmpvar_19;
              tmpvar_16 = tmpvar_20;
              res_12 = tmpvar_16;
          }
          tmpvar_1 = res_12;
          out_f.color = tmpvar_1;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack "Diffuse"
}
