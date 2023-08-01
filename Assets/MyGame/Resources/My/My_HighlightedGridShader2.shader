Shader "My/HighlightedGridShader2"
{
  Properties
  {
    _MainTex ("MainTex", 2D) = "white" {}
    _ImageTex ("ImageTex", 2D) = "white" {}
    _Alpha ("Alpha", Range(0, 1)) = 1
    _CurrentColor ("HighlightedColor", Color) = (1,1,1,1)
    _CurrentRedMin ("MinRed", float) = 0
    _CurrentRedMax ("MaxRed", float) = 0
    _CurrentGreenMin ("MinGreen", float) = 0
    _CurrentGreenMax ("MaxGreen", float) = 0
    _CurrentBlueMin ("MinBlue", float) = 0
    _CurrentBlueMax ("MaxBlue", float) = 0
    _GrayColor ("GrayColor", Color) = (1,1,1,1)
    _GrayColor2 ("GrayColor2", Color) = (1,1,1,1)
    _GrayColor3 ("GrayColor3", Color) = (1,1,1,1)
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
      uniform float _CurrentRedMin;
      uniform float _CurrentRedMax;
      uniform float _CurrentGreenMin;
      uniform float _CurrentGreenMax;
      uniform float _CurrentBlueMin;
      uniform float _CurrentBlueMax;
      uniform float4 _GrayColor;
      uniform float4 _GrayColor2;
      uniform float4 _GrayColor3;
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
          float4 c_2;
          float4 imColor_3;
          float2 texcoord_4;
          texcoord_4 = (in_f.xlv_TEXCOORD0 * _MainTex_ST.xy);
          float4 tmpvar_5;
          tmpvar_5 = tex2D(_ImageTex, in_f.xlv_TEXCOORD0);
          imColor_3 = tmpvar_5;
          if((((imColor_3.x>=_CurrentRedMin) && (imColor_3.x<=_CurrentRedMax)) && (((imColor_3.y>=_CurrentGreenMin) && (imColor_3.y<=_CurrentGreenMax)) && ((imColor_3.z>=_CurrentBlueMin) && (imColor_3.z<=_CurrentBlueMax)))))
          {
              float4 col1_6;
              float4 col_7;
              float4 tmpvar_8;
              tmpvar_8 = tex2D(_MainTex, texcoord_4);
              col1_6 = imColor_3;
              float _tmp_dvx_64 = ((((col1_6.x + col1_6.y) + col1_6.z) / 6) + 0.5);
              col1_6.xyz = float3(_tmp_dvx_64, _tmp_dvx_64, _tmp_dvx_64);
              if((tmpvar_8.w<0.05))
              {
                  col_7.xyz = float3(_GrayColor3.xyz);
                  float tmpvar_9;
                  float x_10;
                  x_10 = (col1_6.x - 0.5);
                  tmpvar_9 = lerp(x_10, 0.17, _Alpha);
                  col_7.w = tmpvar_9;
              }
              else
              {
                  float4 tmpvar_11;
                  tmpvar_11 = lerp(_GrayColor3, _GrayColor2, float4(_Alpha, _Alpha, _Alpha, _Alpha));
                  col_7.xyz = float3(tmpvar_11.xyz);
                  float tmpvar_12;
                  float x_13;
                  x_13 = (col1_6.x - 0.5);
                  tmpvar_12 = lerp(x_13, _Alpha, _Alpha);
                  col_7.w = tmpvar_12;
              }
              tmpvar_1 = col_7;
          }
          else
          {
              c_2.w = tex2D(_MainTex, texcoord_4).w.x;
              if((c_2.w<0.1))
              {
                  c_2.w = 0;
                  c_2.xyz = float3(1, 1, 1);
              }
              else
              {
                  c_2.xyz = float3(_GrayColor.xyz);
                  c_2.w = (c_2.w * _Alpha);
              }
              tmpvar_1 = c_2;
          }
          out_f.color = tmpvar_1;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack Off
}
