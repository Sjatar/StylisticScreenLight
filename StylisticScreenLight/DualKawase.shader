Shader "Hidden/StylisticLight"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    
    CGINCLUDE

    #include "UnityCG.cginc"

    sampler2D _MainTex;
    float4 _MainTex_ST;
    float4 _MainTex_TexelSize;

    float2 offset;
    float4 pixel_col;
    
    struct v2f
    {
        float4 pos : POSITION;
        half2 uv_MainTex : TEXCOORD0;
    };

    v2f vert_nothing(v2f v)
    {
        v2f o;
        o.pos = UnityObjectToClipPos(v.pos);
        o.uv_MainTex = TRANSFORM_TEX(v.uv_MainTex, _MainTex);
        
        return o;
    }

    half4 KawaseDown (v2f i) : SV_Target
    {
        pixel_col = tex2D(_MainTex,i.uv_MainTex) * 4;
        pixel_col += tex2D(_MainTex,i.uv_MainTex+1.5*float2(_MainTex_TexelSize.x,_MainTex_TexelSize.y));
        pixel_col += tex2D(_MainTex,i.uv_MainTex+1.5*float2(_MainTex_TexelSize.x,-_MainTex_TexelSize.y));
        pixel_col += tex2D(_MainTex,i.uv_MainTex+1.5*float2(-_MainTex_TexelSize.x,_MainTex_TexelSize.y));
        pixel_col += tex2D(_MainTex,i.uv_MainTex+1.5*float2(-_MainTex_TexelSize.x,-_MainTex_TexelSize.y));
        pixel_col /= 8;
        
        return half4(pixel_col);
    }

    half4 KawaseUp (v2f i) : SV_Target
    {
        pixel_col = tex2D(_MainTex,i.uv_MainTex+3.0*float2(_MainTex_TexelSize.x, 0));
        pixel_col += tex2D(_MainTex,i.uv_MainTex+3.0*float2(-_MainTex_TexelSize.x, 0));
        
        pixel_col += tex2D(_MainTex,i.uv_MainTex+3.0*float2(0, _MainTex_TexelSize.y));
        pixel_col += tex2D(_MainTex,i.uv_MainTex+3.0*float2(0, -_MainTex_TexelSize.y));

        pixel_col += tex2D(_MainTex,i.uv_MainTex+1.5*float2(_MainTex_TexelSize.x,_MainTex_TexelSize.y)) *2.0;
        pixel_col += tex2D(_MainTex,i.uv_MainTex+1.5*float2(_MainTex_TexelSize.x,-_MainTex_TexelSize.y)) *2.0;
        pixel_col += tex2D(_MainTex,i.uv_MainTex+1.5*float2(-_MainTex_TexelSize.x,_MainTex_TexelSize.y)) *2.0;
        pixel_col += tex2D(_MainTex,i.uv_MainTex+1.5*float2(-_MainTex_TexelSize.x,-_MainTex_TexelSize.y)) *2.0;
        pixel_col /= 12;
            
        return half4(pixel_col);
    }
    
    ENDCG

    SubShader
    {
        Pass
        {
            Name "KawaseDown"
            
            CGPROGRAM
            #pragma vertex vert_nothing
            #pragma fragment KawaseDown
            ENDCG
        }
        
        Pass
        {
            Name "KawaseUp"
            
            CGPROGRAM
            #pragma vertex vert_nothing
            #pragma fragment KawaseUp
            ENDCG
        }
    }
}