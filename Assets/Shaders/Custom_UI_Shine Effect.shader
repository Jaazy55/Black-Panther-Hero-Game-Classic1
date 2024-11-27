Shader "Custom/UI/Shine Effect"
{
  Properties
  {
    [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
    _Color ("Tint", Color) = (1,1,1,1)
    _StencilComp ("Stencil Comparison", float) = 8
    _Stencil ("Stencil ID", float) = 0
    _StencilOp ("Stencil Operation", float) = 0
    _StencilWriteMask ("Stencil Write Mask", float) = 255
    _StencilReadMask ("Stencil Read Mask", float) = 255
    _ColorMask ("Color Mask", float) = 15
    _ShineWidth ("ShineWidth", Range(0, 1)) = 0.01
    _ShineLocation ("ShineLocation", Range(0, 1)) = 0
    _EmissionGain ("Emission Gain", Range(0, 1)) = 0.3
    [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", float) = 0
  }
  SubShader
  {
    Tags
    { 
      "CanUseSpriteAtlas" = "true"
      "IGNOREPROJECTOR" = "true"
      "PreviewType" = "Plane"
      "QUEUE" = "Transparent"
      "RenderType" = "Transparent"
    }
    Pass // ind: 1, name: SHINEEFFECT
    {
      Name "SHINEEFFECT"
      Tags
      { 
        "CanUseSpriteAtlas" = "true"
        "IGNOREPROJECTOR" = "true"
        "PreviewType" = "Plane"
        "QUEUE" = "Transparent"
        "RenderType" = "Transparent"
      }
      ZClip Off
      ZWrite Off
      Cull Off
      Stencil
      { 
        Ref 0
        ReadMask 0
        WriteMask 0
        Pass Keep
        Fail Keep
        ZFail Keep
        PassFront Keep
        FailFront Keep
        ZFailFront Keep
        PassBack Keep
        FailBack Keep
        ZFailBack Keep
      } 
      Blend SrcAlpha OneMinusSrcAlpha
      ColorMask 0
      // m_ProgramMask = 6
      CGPROGRAM
      //#pragma target 4.0
      
      #pragma vertex vert
      #pragma fragment frag
      
      #include "UnityCG.cginc"
      
      
      #define CODE_BLOCK_VERTEX
      //uniform float4x4 unity_ObjectToWorld;
      //uniform float4x4 unity_MatrixVP;
      uniform float4 _Color;
      uniform float4 _MainTex_ST;
      uniform float4 _TextureSampleAdd;
      uniform float4 _ClipRect;
      uniform float _ShineWidth;
      uniform float _ShineLocation;
      uniform float _EmissionGain;
      uniform sampler2D _MainTex;
      struct appdata_t
      {
          float4 vertex :POSITION;
          float4 color :COLOR;
          float4 texcoord :TEXCOORD0;
      };
      
      struct OUT_Data_Vert
      {
          float4 xlv_COLOR :COLOR;
          float2 xlv_TEXCOORD0 :TEXCOORD0;
          float4 xlv_TEXCOORD1 :TEXCOORD1;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float4 xlv_COLOR :COLOR;
          float2 xlv_TEXCOORD0 :TEXCOORD0;
          float4 xlv_TEXCOORD1 :TEXCOORD1;
      };
      
      struct OUT_Data_Frag
      {
          float4 color :SV_Target0;
      };
      
      OUT_Data_Vert vert(appdata_t in_v)
      {
          OUT_Data_Vert out_v;
          float4 tmpvar_1;
          tmpvar_1 = in_v.vertex;
          float4 tmpvar_2;
          float4 tmpvar_3;
          tmpvar_3.w = 1;
          tmpvar_3.xyz = float3(tmpvar_1.xyz);
          tmpvar_2 = (in_v.color * _Color);
          out_v.vertex = mul(unity_MatrixVP, mul(unity_ObjectToWorld, tmpvar_3));
          out_v.xlv_COLOR = tmpvar_2;
          out_v.xlv_TEXCOORD0 = TRANSFORM_TEX(in_v.texcoord.xy, _MainTex);
          out_v.xlv_TEXCOORD1 = tmpvar_1;
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          float4 tmpvar_1;
          float currentDistanceProjection_2;
          float4 color_3;
          float4 tmpvar_4;
          tmpvar_4 = ((tex2D(_MainTex, in_f.xlv_TEXCOORD0) + _TextureSampleAdd) * in_f.xlv_COLOR);
          color_3 = tmpvar_4;
          float tmpvar_5;
          float2 tmpvar_6;
          if(float((_ClipRect.z>=in_f.xlv_TEXCOORD1.x)))
          {
              tmpvar_6.x = 1;
          }
          else
          {
              tmpvar_6.x = 0;
          }
          if(float((_ClipRect.w>=in_f.xlv_TEXCOORD1.y)))
          {
              tmpvar_6.y = 1;
          }
          else
          {
              tmpvar_6.y = 0;
          }
          float2 tmpvar_7;
          tmpvar_7 = (float2(bool2(in_f.xlv_TEXCOORD1.xy >= _ClipRect.xy)) * tmpvar_6);
          tmpvar_5 = (tmpvar_7.x * tmpvar_7.y);
          color_3.w = (color_3.w * tmpvar_5);
          float tmpvar_8;
          tmpvar_8 = (((in_f.xlv_TEXCOORD0.x * 5.9) + in_f.xlv_TEXCOORD0.y) / 6);
          currentDistanceProjection_2 = tmpvar_8;
          float tmpvar_9;
          tmpvar_9 = clamp(((abs((clamp(currentDistanceProjection_2, (_ShineLocation - _ShineWidth), (_ShineLocation + _ShineWidth)) - _ShineLocation)) - _ShineWidth) / (-_ShineWidth)), 0, 1);
          color_3.xyz = float3((color_3.xyz + ((color_3.xyz * ((tmpvar_9 * (tmpvar_9 * (3 - (2 * tmpvar_9)))) / 2)) * exp((_EmissionGain * 4)))));
          tmpvar_1 = color_3;
          out_f.color = tmpvar_1;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack Off
}
