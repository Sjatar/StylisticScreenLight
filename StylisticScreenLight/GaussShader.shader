Shader "Hidden/StylisticLight"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ArrayLength ("Int", int) = 1
    }
    
    CGINCLUDE

    #include "UnityCG.cginc"

    sampler2D _MainTex;
    float4 _MainTex_ST;
    float4 _MainTex_TexelSize;
    
    int _ArrayLength;
    
    UNITY_INSTANCING_BUFFER_START(Props)
    UNITY_DEFINE_INSTANCED_PROP(float, _GaussWeights[7])
    UNITY_DEFINE_INSTANCED_PROP(float, _GaussOffsets[7])
    UNITY_INSTANCING_BUFFER_END(Props)

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

    half4 frag_fastgaussx (v2f i) : SV_Target
    {
        for (int j = 0; j < _ArrayLength; j += 1)
        {
            offset = float2(UNITY_ACCESS_INSTANCED_PROP(Props, _GaussOffsets[j])*_MainTex_TexelSize.x, 0);
            pixel_col += tex2D(_MainTex,i.uv_MainTex+offset) * UNITY_ACCESS_INSTANCED_PROP(Props, _GaussWeights[j]);
            pixel_col += tex2D(_MainTex,i.uv_MainTex-offset) * UNITY_ACCESS_INSTANCED_PROP(Props, _GaussWeights[j]);
        }   
        
        return half4(pixel_col);
    }

    half4 frag_fastgaussy (v2f i) : SV_Target
    {
        for (int j = 0; j < _ArrayLength; j += 1)
        {
            offset = float2(0, UNITY_ACCESS_INSTANCED_PROP(Props, _GaussOffsets[j])*_MainTex_TexelSize.y);
            pixel_col += tex2D(_MainTex,i.uv_MainTex+offset) * UNITY_ACCESS_INSTANCED_PROP(Props, _GaussWeights[j]);
            pixel_col += tex2D(_MainTex,i.uv_MainTex-offset) * UNITY_ACCESS_INSTANCED_PROP(Props, _GaussWeights[j]);
        }
        
        return half4(pixel_col);
    }
    
    
    // float4 frag_box (v2f i) : SV_Target
    // {
    //     float4 pix1 = tex2D(_MainTex,i.uv + float2(_MainTex_TexelSize.x, _MainTex_TexelSize.y));
    //     float4 pix2 = tex2D(_MainTex,i.uv + float2(_MainTex_TexelSize.x, -_MainTex_TexelSize.y));
    //     float4 pix3 = tex2D(_MainTex,i.uv + float2(-_MainTex_TexelSize.x, _MainTex_TexelSize.y));
    //     float4 pix4 = tex2D(_MainTex,i.uv + float2(-_MainTex_TexelSize.x, -_MainTex_TexelSize.y));
    //     return float4(0.25f * (pix1 + pix2 + pix3 + pix4));
    // }
    
    // half4 frag_gauss (v2f i) : SV_Target
    // {
    //     half4 screen = tex2D(_MainTex,i.uv).rgba * (float)UNITY_ACCESS_INSTANCED_PROP(Props, _GaussWeights[0]);
    //     
    //     float Tau = 6.28318530718;
    //     
    //     fixed2 Radius = _Size / _MainTex_TexelSize.zw;
    //
    //     float2 v = (i.pos.xy/i.pos.w) * _ScreenParams.xy;
    //     float randNum = hash12(v);
    //     
    //     for (float j = 1.0 ; j <= _Quality; j += 1.0 )
    //     {
    //         for ( float d = 0.0; d < Tau; d += Tau / _StartPoints )
    //         {
    //             screen += tex2D(_MainTex, i.uv + fixed2(cos(d+randNum*Tau), sin(d+randNum*Tau)) * (Radius) * (j/_Quality)).rgba * (float)UNITY_ACCESS_INSTANCED_PROP(Props, _GaussWeights[j]);
    //         }
    //         
    //         _StartPoints *= _PointInc;
    //         Radius *= _SizeInc;
    //     }
    //     
    //     pixel_col.rgba = screen.rgba;
    //     
    //     return pixel_col;
    // }
    
    ENDCG

    SubShader
    {
        Pass
        {
            Name "gaussx"
            
            CGPROGRAM
            #pragma vertex vert_nothing
            #pragma fragment frag_fastgaussx
            ENDCG
        }
        
        Pass
        {
            Name "gaussy"
            
            CGPROGRAM
            #pragma vertex vert_nothing
            #pragma fragment frag_fastgaussy
            ENDCG
        }
    }
}