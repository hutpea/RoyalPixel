Shader "My/HighlightedGridShader"
{
  Properties
  {
    _MainTex ("MainTex", 2D) = "white" {}
    _ImageTex ("ImageTex", 2D) = "white" {}
    _Alpha ("Alpha", Range(0, 1)) = 1
    _CurrentColor ("HighlightedColor", Color) = (1,1,1,1)
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
      Blend SrcAlpha OneMinusSrcAlpha
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
      uniform sampler2D _ImageTex;
      uniform float4 _MainTex_ST;
      uniform float _Alpha;
      uniform float4 _CurrentColor;
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
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          float4 tmpvar_1;
          float2 texcoord_2;
          texcoord_2 = (in_f.xlv_TEXCOORD0 * _MainTex_ST.xy);
          float4 tmpvar_3;
          tmpvar_3 = tex2D(_ImageTex, in_f.xlv_TEXCOORD0);
          if((_CurrentColor.w>0.95))
          {
              if((((tmpvar_3.x==_CurrentColor.x) && (tmpvar_3.y==_CurrentColor.y)) && (tmpvar_3.z==_CurrentColor.z)))
              {
                  float4 col_4;
                  col_4.xyz = float3(_CurrentColor.xyz);
                  col_4.w = tex2D(_MainTex, texcoord_2).w.x;
                  col_4.w = (col_4.w * _Alpha);
                  tmpvar_1 = col_4;
              }
              else
              {
                  float4 c_5;
                  c_5.xyz = float3(0.95, 0.95, 0.95);
                  c_5.w = 0.5;
                  tmpvar_1 = c_5;
              }
          }
          else
          {
              float4 c_1_6;
              c_1_6.xyz = float3(0.95, 0.95, 0.95);
              c_1_6.w = 0.5;
              tmpvar_1 = c_1_6;
          }
          out_f.color = tmpvar_1;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack Off
}
