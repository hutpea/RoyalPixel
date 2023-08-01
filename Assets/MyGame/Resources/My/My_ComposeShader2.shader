Shader "My/ComposeShader2"
{
  Properties
  {
    _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
    _TemplateTex ("Base (RGB) Trans (A)", 2D) = "black" {}
    _TemplateSet ("Is template set", float) = 0
    _ComposeType ("Compose Type", float) = 2
    _Coef ("Coef", float) = 1
    _Coef2 ("Coef2", float) = 1
  }
  SubShader
  {
    Tags
    { 
      "IGNOREPROJECTOR" = "true"
      "QUEUE" = "Transparent"
      "RenderType" = "Transparent"
    }
    LOD 110
    Pass // ind: 1, name: 
    {
      Tags
      { 
        "IGNOREPROJECTOR" = "true"
        "QUEUE" = "Transparent"
        "RenderType" = "Transparent"
      }
      LOD 110
      ZWrite Off
      Cull Off
      Fog
      { 
        Mode  Off
      } 
      Blend One OneMinusSrcAlpha
      // m_ProgramMask = 6
      CGPROGRAM
      //#pragma target 4.0
      
      #pragma vertex vert
      #pragma fragment frag
      
      #include "UnityCG.cginc"
      
      
      #define CODE_BLOCK_VERTEX
      //uniform float4x4 unity_ObjectToWorld;
      //uniform float4x4 unity_MatrixVP;
      uniform sampler2D _MainTex;
      uniform sampler2D _TemplateTex;
      uniform int _TemplateSet;
      uniform float _Coef;
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
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float2 xlv_TEXCOORD0 :TEXCOORD0;
      };
      
      struct OUT_Data_Frag
      {
          float4 color :SV_Target0;
      };
      
      OUT_Data_Vert vert(appdata_t in_v)
      {
          OUT_Data_Vert out_v;
          float4 tmpvar_1;
          tmpvar_1 = in_v.color;
          float4 tmpvar_2;
          float4 tmpvar_3;
          tmpvar_3.w = 1;
          tmpvar_3.xyz = float3(in_v.vertex.xyz);
          tmpvar_2 = tmpvar_1;
          out_v.vertex = mul(unity_MatrixVP, mul(unity_ObjectToWorld, tmpvar_3));
          out_v.xlv_COLOR = tmpvar_2;
          out_v.xlv_TEXCOORD0 = in_v.texcoord.xy;
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      float xlat_mutable_Coef;
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          xlat_mutable_Coef = _Coef;
          float4 tmpvar_1;
          float4 col_2;
          if(!int(_TemplateSet))
          {
              tmpvar_1 = tex2D(_MainTex, in_f.xlv_TEXCOORD0);
          }
          else
          {
              xlat_mutable_Coef = (_Coef / 2);
              float tmpvar_3;
              tmpvar_3 = max(0.7, (1 - xlat_mutable_Coef));
              col_2 = ((tex2D(_MainTex, in_f.xlv_TEXCOORD0) * tmpvar_3) + ((tex2D(_TemplateTex, in_f.xlv_TEXCOORD0) - 0.7) * xlat_mutable_Coef));
              tmpvar_1 = col_2;
          }
          out_f.color = tmpvar_1;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack Off
}
