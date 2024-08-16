Shader "Hidden/StylisticLight"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ScreenTexture ("Texture", 2D) = "White" {}
        _MixRatio ("float", float) = 0.5
    }
    
    CGINCLUDE

    #include "UnityCG.cginc"

    sampler2D _MainTex;
    float4 _MainTex_ST;
    float4 _MainTex_TexelSize;

    sampler2D _ScreenTex;
    float4 _ScreenTex_ST;
    float4 _ScreenTex_TexelSize;

    float _MixRatio;
    
    half4 pixel_col;
    
    struct v2f
    {
        float4 pos : POSITION;
        half2 uv_MainTex : TEXCOORD0;
        half2 uv_ScreenTex : TEXCOORD1;
    };

    v2f vert_nothing(v2f v)
    {
        v2f o;
        o.pos = UnityObjectToClipPos(v.pos);
        o.uv_MainTex = TRANSFORM_TEX(v.uv_MainTex, _MainTex);
        o.uv_ScreenTex = TRANSFORM_TEX(v.uv_ScreenTex, _ScreenTex);
        
        return o;
    }

    //##############################mix Pass 0##############################

    half4 frag_mix (v2f i) : SV_Target
    {
        half4 camera = tex2D(_MainTex,i.uv_MainTex).rgba;
        half4 screen = tex2D(_ScreenTex,i.uv_ScreenTex).rgba;
        pixel_col.a = camera.a;

        pixel_col.rgb = half3((camera.rgb * ( 1.0f - _MixRatio) + screen.rgb * _MixRatio) * camera.a);
        
        return pixel_col; 
    }
    
    ENDCG

    SubShader
    {
        Pass
        {
            name "mix"
            
            CGPROGRAM
            #pragma vertex vert_nothing
            #pragma fragment frag_mix
            ENDCG
        }
    }
}