Shader "My/ColoringNumbersShader"
{
  Properties
  {
    _MainTex ("MainTex", 2D) = "white" {}
    _ImageTex ("ImageTex", 2D) = "white" {}
    _NumberTex ("NumberTex", 2D) = "white" {}
    _CurrentRedMin ("MinRed", float) = 0
    _CurrentRedMax ("MaxRed", float) = 0
    _CurrentGreenMin ("MinGreen", float) = 0
    _CurrentGreenMax ("MaxGreen", float) = 0
    _CurrentBlueMin ("MinBlue", float) = 0
    _CurrentBlueMax ("MaxBlue", float) = 0
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
              float4 col_6;
              col_6.xyz = float3(_GrayColor.xyz);
              col_6.w = tex2D(_MainTex, texcoord_4).w.x;
              col_6.w = (col_6.w * _Alpha);
              col_6.w = (col_6.w + 0.4);
              tmpvar_1 = col_6;
          }
          else
          {
              c_2.xyz = float3(1, 1, 1);
              c_2.w = 0.6;
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
