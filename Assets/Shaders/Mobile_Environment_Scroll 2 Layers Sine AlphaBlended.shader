Shader "Mobile/Environment/Scroll 2 Layers Sine AlphaBlended"
{
  Properties
  {
    _MainTex ("Base layer (RGB)", 2D) = "white" {}
    _DetailTex ("2nd layer (RGB)", 2D) = "white" {}
    _ScrollX ("Base layer Scroll speed X", float) = -0.3
    _ScrollY ("Base layer Scroll speed Y", float) = -0.2
    _Scroll2X ("2nd layer Scroll speed X", float) = 0
    _Scroll2Y ("2nd layer Scroll speed Y", float) = -0.1
    _SineAmplX ("Base layer sine amplitude X", float) = 0
    _SineAmplY ("Base layer sine amplitude Y", float) = 0
    _SineFreqX ("Base layer sine freq X", float) = 0
    _SineFreqY ("Base layer sine freq Y", float) = 0
    _SineAmplX2 ("2nd layer sine amplitude X", float) = 0
    _SineAmplY2 ("2nd layer sine amplitude Y", float) = 0
    _SineFreqX2 ("2nd layer sine freq X", float) = 200
    _SineFreqY2 ("2nd layer sine freq Y", float) = 10
    _Color ("Color", Color) = (1,1,1,1)
    _MMultiplier ("Layer Multiplier", float) = 2
  }
  SubShader
  {
    Tags
    { 
      "IGNOREPROJECTOR" = "true"
      "QUEUE" = "Transparent"
      "RenderType" = "Transparent"
    }
    LOD 100
    Pass // ind: 1, name: 
    {
      Tags
      { 
        "IGNOREPROJECTOR" = "true"
        "QUEUE" = "Transparent"
        "RenderType" = "Transparent"
      }
      LOD 100
      ZClip Off
      ZWrite Off
      Cull Off
      Blend SrcAlpha OneMinusSrcAlpha
      // m_ProgramMask = 6
      CGPROGRAM
      #pragma multi_compile LIGHTMAP_OFF
      //#pragma target 4.0
      
      #pragma vertex vert
      #pragma fragment frag
      
      #include "UnityCG.cginc"
      
      
      #define CODE_BLOCK_VERTEX
      //uniform float4 _Time;
      //uniform float4x4 unity_ObjectToWorld;
      //uniform float4x4 unity_MatrixVP;
      uniform float4 _MainTex_ST;
      uniform float4 _DetailTex_ST;
      uniform float _ScrollX;
      uniform float _ScrollY;
      uniform float _Scroll2X;
      uniform float _Scroll2Y;
      uniform float _MMultiplier;
      uniform float _SineAmplX;
      uniform float _SineAmplY;
      uniform float _SineFreqX;
      uniform float _SineFreqY;
      uniform float _SineAmplX2;
      uniform float _SineAmplY2;
      uniform float _SineFreqX2;
      uniform float _SineFreqY2;
      uniform float4 _Color;
      uniform sampler2D _MainTex;
      uniform sampler2D _DetailTex;
      struct appdata_t
      {
          float4 vertex :POSITION;
          float4 color :COLOR;
          float4 texcoord :TEXCOORD0;
      };
      
      struct OUT_Data_Vert
      {
          float4 xlv_TEXCOORD0 :TEXCOORD0;
          float4 xlv_TEXCOORD1 :TEXCOORD1;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float4 xlv_TEXCOORD0 :TEXCOORD0;
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
          float4 tmpvar_2;
          float4 tmpvar_3;
          tmpvar_3.w = 1;
          tmpvar_3.xyz = float3(in_v.vertex.xyz);
          float2 tmpvar_4;
          tmpvar_4.x = _ScrollX;
          tmpvar_4.y = _ScrollY;
          tmpvar_1.xy = float2((TRANSFORM_TEX(in_v.texcoord.xy, _MainTex) + frac((tmpvar_4 * _Time.xy))));
          float2 tmpvar_5;
          tmpvar_5.x = _Scroll2X;
          tmpvar_5.y = _Scroll2Y;
          tmpvar_1.zw = (TRANSFORM_TEX(in_v.texcoord.xy, _DetailTex) + frac((tmpvar_5 * _Time.xy)));
          tmpvar_1.x = (tmpvar_1.x + (sin((_Time * _SineFreqX)) * _SineAmplX).x);
          tmpvar_1.y = (tmpvar_1.y + (sin((_Time * _SineFreqY)) * _SineAmplY).x);
          tmpvar_1.z = (tmpvar_1.z + (sin((_Time * _SineFreqX2)) * _SineAmplX2).x);
          tmpvar_1.w = (tmpvar_1.w + (sin((_Time * _SineFreqY2)) * _SineAmplY2).x);
          tmpvar_2 = ((_MMultiplier * _Color) * in_v.color);
          out_v.vertex = mul(unity_MatrixVP, mul(unity_ObjectToWorld, tmpvar_3));
          out_v.xlv_TEXCOORD0 = tmpvar_1;
          out_v.xlv_TEXCOORD1 = tmpvar_2;
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          float4 tmpvar_1;
          tmpvar_1 = ((tex2D(_MainTex, in_f.xlv_TEXCOORD0.xy) * tex2D(_DetailTex, in_f.xlv_TEXCOORD0.zw)) * in_f.xlv_TEXCOORD1);
          out_f.color = tmpvar_1;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack Off
}
