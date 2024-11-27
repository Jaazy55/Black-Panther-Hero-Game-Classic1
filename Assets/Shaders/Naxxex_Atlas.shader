Shader "Naxxex/Atlas"
{
  Properties
  {
    _Tex_Main ("Tex_Main", 2D) = "white" {}
    _Tex_Lightmap ("Tex_Lightmap", 2D) = "white" {}
    _Tex_Cube ("Tex_Cube", Cube) = "_Skybox" {}
    _Cube_intensity ("Cube_intensity", Range(0, 1)) = 1
    _Tex_ReflectiveMask ("Tex_ReflectiveMask", 2D) = "white" {}
  }
  SubShader
  {
    Tags
    { 
      "RenderType" = "Opaque"
    }
    LOD 100
    Pass // ind: 1, name: FORWARD
    {
      Name "FORWARD"
      Tags
      { 
        "LIGHTMODE" = "FORWARDBASE"
        "RenderType" = "Opaque"
        "SHADOWSUPPORT" = "true"
      }
      LOD 100
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
      //uniform float4x4 unity_ObjectToWorld;
      //uniform float4x4 unity_WorldToObject;
      //uniform float4x4 unity_MatrixVP;
      //uniform float3 _WorldSpaceCameraPos;
      uniform sampler2D _Tex_Main;
      uniform float4 _Tex_Main_ST;
      uniform sampler2D _Tex_Lightmap;
      uniform float4 _Tex_Lightmap_ST;
      uniform samplerCUBE _Tex_Cube;
      uniform float _Cube_intensity;
      uniform sampler2D _Tex_ReflectiveMask;
      uniform float4 _Tex_ReflectiveMask_ST;
      struct appdata_t
      {
          float4 vertex :POSITION;
          float3 normal :NORMAL;
          float4 texcoord :TEXCOORD0;
          float4 texcoord1 :TEXCOORD1;
      };
      
      struct OUT_Data_Vert
      {
          float2 xlv_TEXCOORD0 :TEXCOORD0;
          float2 xlv_TEXCOORD1 :TEXCOORD1;
          float4 xlv_TEXCOORD2 :TEXCOORD2;
          float3 xlv_TEXCOORD3 :TEXCOORD3;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float2 xlv_TEXCOORD0 :TEXCOORD0;
          float2 xlv_TEXCOORD1 :TEXCOORD1;
          float4 xlv_TEXCOORD2 :TEXCOORD2;
          float3 xlv_TEXCOORD3 :TEXCOORD3;
      };
      
      struct OUT_Data_Frag
      {
          float4 color :SV_Target0;
      };
      
      OUT_Data_Vert vert(appdata_t in_v)
      {
          OUT_Data_Vert out_v;
          float3x3 tmpvar_1;
          tmpvar_1[0] = conv_mxt4x4_0(unity_WorldToObject).xyz;
          tmpvar_1[1] = conv_mxt4x4_1(unity_WorldToObject).xyz;
          tmpvar_1[2] = conv_mxt4x4_2(unity_WorldToObject).xyz;
          float4 tmpvar_2;
          tmpvar_2.w = 1;
          tmpvar_2.xyz = float3(in_v.vertex.xyz);
          out_v.vertex = mul(unity_MatrixVP, mul(unity_ObjectToWorld, tmpvar_2));
          out_v.xlv_TEXCOORD0 = in_v.texcoord.xy;
          out_v.xlv_TEXCOORD1 = in_v.texcoord1.xy;
          out_v.xlv_TEXCOORD2 = mul(unity_ObjectToWorld, in_v.vertex);
          out_v.xlv_TEXCOORD3 = normalize(mul(in_v.normal, tmpvar_1));
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          float4 tmpvar_1;
          float4 finalRGBA_2;
          float4 _Tex_ReflectiveMask_var_3;
          float4 _Tex_Lightmap_var_4;
          float4 _Tex_Main_var_5;
          float3 tmpvar_6;
          tmpvar_6 = normalize(in_f.xlv_TEXCOORD3);
          float3 tmpvar_7;
          float3 I_8;
          I_8 = (-normalize((_WorldSpaceCameraPos - in_f.xlv_TEXCOORD2.xyz)));
          tmpvar_7 = (I_8 - (2 * (dot(tmpvar_6, I_8) * tmpvar_6)));
          float4 tmpvar_9;
          float2 P_10;
          P_10 = TRANSFORM_TEX(in_f.xlv_TEXCOORD0, _Tex_Main);
          tmpvar_9 = tex2D(_Tex_Main, P_10);
          _Tex_Main_var_5 = tmpvar_9;
          float4 tmpvar_11;
          float2 P_12;
          P_12 = TRANSFORM_TEX(in_f.xlv_TEXCOORD1, _Tex_Lightmap);
          tmpvar_11 = tex2D(_Tex_Lightmap, P_12);
          _Tex_Lightmap_var_4 = tmpvar_11;
          float4 tmpvar_13;
          float2 P_14;
          P_14 = TRANSFORM_TEX(in_f.xlv_TEXCOORD0, _Tex_ReflectiveMask);
          tmpvar_13 = tex2D(_Tex_ReflectiveMask, P_14);
          _Tex_ReflectiveMask_var_3 = tmpvar_13;
          float3 tmpvar_15;
          tmpvar_15 = bool3(_Tex_Lightmap_var_4.xyz > float3(0.5, 0.5, 0.5));
          float3 b_16;
          float _tmp_dvx_27 = (1 - ((1 - (2 * (_Tex_Lightmap_var_4.xyz - 0.5))) * (1 - _Tex_Main_var_5.xyz)));
          b_16 = float3(_tmp_dvx_27, _tmp_dvx_27, _tmp_dvx_27);
          float3 c_17;
          float _tmp_dvx_28 = ((2 * _Tex_Lightmap_var_4.xyz) * _Tex_Main_var_5.xyz);
          c_17 = float3(_tmp_dvx_28, _tmp_dvx_28, _tmp_dvx_28);
          float tmpvar_18;
          if(tmpvar_15.x)
          {
              tmpvar_18 = b_16.x;
          }
          else
          {
              tmpvar_18 = c_17.x;
          }
          float tmpvar_19;
          if(tmpvar_15.y)
          {
              tmpvar_19 = b_16.y;
          }
          else
          {
              tmpvar_19 = c_17.y;
          }
          float tmpvar_20;
          if(tmpvar_15.z)
          {
              tmpvar_20 = b_16.z;
          }
          else
          {
              tmpvar_20 = c_17.z;
          }
          float3 tmpvar_21;
          tmpvar_21.x = tmpvar_18;
          tmpvar_21.y = tmpvar_19;
          tmpvar_21.z = tmpvar_20;
          float4 tmpvar_22;
          tmpvar_22 = texCUBE(_Tex_Cube, tmpvar_7);
          float4 tmpvar_23;
          tmpvar_23.w = 1;
          tmpvar_23.xyz = float3(lerp(clamp(tmpvar_21, 0, 1), (tmpvar_22.xyz * _Cube_intensity), _Tex_ReflectiveMask_var_3.xxx));
          finalRGBA_2 = tmpvar_23;
          tmpvar_1 = finalRGBA_2;
          out_f.color = tmpvar_1;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack "Diffuse"
}
