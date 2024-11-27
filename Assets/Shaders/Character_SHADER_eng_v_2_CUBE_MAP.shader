Shader "Character_SHADER_eng_v_2_CUBE_MAP"
{
  Properties
  {
    _Base ("Base", 2D) = "white" {}
    _NormalMap ("Normal Map", 2D) = "bump" {}
    _SpecularMap ("Specular Map", 2D) = "white" {}
    _SpecularLevel ("Specular Level", float) = 1
    _GlossLevel ("Gloss Level", float) = 0.5
    _GlossRRimGRefB ("Gloss(R)Rim(G)Ref(B)", 2D) = "white" {}
    _RimPower ("Rim Power", float) = 1
    _RimColor ("Rim Color", Color) = (0.5,0.5,0.5,1)
    _Reflection ("Reflection", Cube) = "_Skybox" {}
    _RefLevel ("Ref Level", float) = 1
  }
  SubShader
  {
    Tags
    { 
      "RenderType" = "Opaque"
    }
    Pass // ind: 1, name: FORWARDBASE
    {
      Name "FORWARDBASE"
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
      #define conv_mxt3x3_0(mat4x4) float3(mat4x4[0].x,mat4x4[1].x,mat4x4[2].x)
      #define conv_mxt3x3_1(mat4x4) float3(mat4x4[0].y,mat4x4[1].y,mat4x4[2].y)
      #define conv_mxt3x3_2(mat4x4) float3(mat4x4[0].z,mat4x4[1].z,mat4x4[2].z)
      
      
      #define CODE_BLOCK_VERTEX
      //uniform float4x4 unity_ObjectToWorld;
      //uniform float4x4 unity_WorldToObject;
      //uniform float4x4 unity_MatrixVP;
      //uniform float3 _WorldSpaceCameraPos;
      //uniform float4 _WorldSpaceLightPos0;
      //uniform float4 glstate_lightmodel_ambient;
      uniform float4 _LightColor0;
      uniform sampler2D _Base;
      uniform float4 _Base_ST;
      uniform sampler2D _GlossRRimGRefB;
      uniform float4 _GlossRRimGRefB_ST;
      uniform float _RimPower;
      uniform samplerCUBE _Reflection;
      uniform float _RefLevel;
      uniform sampler2D _NormalMap;
      uniform float4 _NormalMap_ST;
      uniform float _SpecularLevel;
      uniform float _GlossLevel;
      uniform float4 _RimColor;
      uniform sampler2D _SpecularMap;
      uniform float4 _SpecularMap_ST;
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
          float3 xlv_TEXCOORD2 :TEXCOORD2;
          float3 xlv_TEXCOORD3 :TEXCOORD3;
          float3 xlv_TEXCOORD4 :TEXCOORD4;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float2 xlv_TEXCOORD0 :TEXCOORD0;
          float4 xlv_TEXCOORD1 :TEXCOORD1;
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
          float3 tmpvar_1;
          float4 tmpvar_2;
          tmpvar_2.w = 0;
          tmpvar_2.xyz = float3(in_v.normal);
          tmpvar_1 = mul(tmpvar_2, unity_WorldToObject).xyz.xyz;
          float4 tmpvar_3;
          tmpvar_3.w = 0;
          tmpvar_3.xyz = float3(in_v.tangent.xyz);
          float3 tmpvar_4;
          tmpvar_4 = normalize(mul(unity_ObjectToWorld, tmpvar_3).xyz);
          float4 tmpvar_5;
          tmpvar_5.w = 1;
          tmpvar_5.xyz = float3(in_v.vertex.xyz);
          out_v.vertex = mul(unity_MatrixVP, mul(unity_ObjectToWorld, tmpvar_5));
          out_v.xlv_TEXCOORD0 = in_v.texcoord.xy;
          out_v.xlv_TEXCOORD1 = mul(unity_ObjectToWorld, in_v.vertex);
          out_v.xlv_TEXCOORD2 = tmpvar_1;
          out_v.xlv_TEXCOORD3 = tmpvar_4;
          out_v.xlv_TEXCOORD4 = normalize((((tmpvar_1.yzx * tmpvar_4.zxy) - (tmpvar_1.zxy * tmpvar_4.yzx)) * in_v.tangent.w));
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          float4 tmpvar_1;
          float3 finalColor_2;
          float4 node_24_3;
          float3 lightDirection_4;
          float3 normalLocal_5;
          float3 tmpvar_6;
          tmpvar_6 = normalize(in_f.xlv_TEXCOORD2);
          float3x3 tmpvar_7;
          conv_mxt3x3_0(tmpvar_7).x = in_f.xlv_TEXCOORD3.x;
          conv_mxt3x3_0(tmpvar_7).y = in_f.xlv_TEXCOORD4.x;
          conv_mxt3x3_0(tmpvar_7).z = tmpvar_6.x;
          conv_mxt3x3_1(tmpvar_7).x = in_f.xlv_TEXCOORD3.y;
          conv_mxt3x3_1(tmpvar_7).y = in_f.xlv_TEXCOORD4.y;
          conv_mxt3x3_1(tmpvar_7).z = tmpvar_6.y;
          conv_mxt3x3_2(tmpvar_7).x = in_f.xlv_TEXCOORD3.z;
          conv_mxt3x3_2(tmpvar_7).y = in_f.xlv_TEXCOORD4.z;
          conv_mxt3x3_2(tmpvar_7).z = tmpvar_6.z;
          float3 tmpvar_8;
          tmpvar_8 = normalize((_WorldSpaceCameraPos - in_f.xlv_TEXCOORD1.xyz));
          float2 P_9;
          P_9 = TRANSFORM_TEX(in_f.xlv_TEXCOORD0, _NormalMap);
          float3 tmpvar_10;
          tmpvar_10 = ((tex2D(_NormalMap, P_9).xyz * 2) - 1).xyz;
          normalLocal_5 = tmpvar_10;
          float3 tmpvar_11;
          tmpvar_11 = normalize(mul(normalLocal_5, tmpvar_7));
          float3 tmpvar_12;
          float3 I_13;
          I_13 = (-tmpvar_8);
          tmpvar_12 = (I_13 - (2 * (dot(tmpvar_11, I_13) * tmpvar_11)));
          float3 tmpvar_14;
          tmpvar_14 = normalize(_WorldSpaceLightPos0.xyz);
          lightDirection_4 = tmpvar_14;
          float4 tmpvar_15;
          float2 P_16;
          P_16 = TRANSFORM_TEX(in_f.xlv_TEXCOORD0, _GlossRRimGRefB);
          tmpvar_15 = tex2D(_GlossRRimGRefB, P_16);
          node_24_3 = tmpvar_15;
          float4 tmpvar_17;
          tmpvar_17 = texCUBE(_Reflection, tmpvar_12);
          float4 tmpvar_18;
          float2 P_19;
          P_19 = TRANSFORM_TEX(in_f.xlv_TEXCOORD0, _SpecularMap);
          tmpvar_18 = tex2D(_SpecularMap, P_19);
          float4 tmpvar_20;
          float2 P_21;
          P_21 = TRANSFORM_TEX(in_f.xlv_TEXCOORD0, _Base);
          tmpvar_20 = tex2D(_Base, P_21);
          finalColor_2 = (((max(0, dot(tmpvar_11, lightDirection_4)) * _LightColor0.xyz) + (glstate_lightmodel_ambient * 2).xyz) * tmpvar_20.xyz);
          float _tmp_dvx_19 = (tmpvar_18.w * _SpecularLevel);
          finalColor_2 = (finalColor_2 + ((_LightColor0.xyz * pow(max(0, dot(normalize((tmpvar_8 + lightDirection_4)), tmpvar_11)), exp2((((node_24_3.x * _GlossLevel) * 10) + 1)))) * float3(_tmp_dvx_19, _tmp_dvx_19, _tmp_dvx_19)));
          float tmpvar_22;
          tmpvar_22 = (1 - max(0, dot(tmpvar_11, tmpvar_8)));
          finalColor_2 = (finalColor_2 + ((((node_24_3.y * tmpvar_22) * (tmpvar_22 * _RimPower)) * _RimColor.xyz) + ((node_24_3.z * tmpvar_17.xyz) * _RefLevel)));
          float4 tmpvar_23;
          tmpvar_23.w = 1;
          tmpvar_23.xyz = float3(finalColor_2);
          tmpvar_1 = tmpvar_23;
          out_f.color = tmpvar_1;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack "Diffuse"
}
