Shader "Hidden/StylisticLight"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ScreenTexture ("Texture", 2D) = "white" {}
        _HighLightTex ("Texture", 2D) = "white" {}
        _BaseValue ("float", float) = 0.2
        _InputValue ("float", float) = 2.0
        _HighLightValue ("float", float) = 5
        _HighLightOffsetX ("float", float) = 0
        _HighLightOffsetY ("float", float) = 0
        _BrightnessThreshold ("float", float) = 0.5
        _MainXOffset ("float", float) = 0
        _MainYOffset ("float", float) = 0
        _ScreenXOffset ("float", float) = 0
        _ScreenYOffset ("float", float) = 0
    }
    
    CGINCLUDE

    #include "UnityCG.cginc"

    sampler2D _MainTex;
    float4 _MainTex_ST;
    float4 _MainTex_TexelSize;

    sampler2D _ScreenTex;
    float4 _ScreenTex_ST;
    float4 _ScreenTex_TexelSize;

    sampler2D _HighLightTex;
    float4 _HighLightTex_ST;
    float4 _HighLightTex_TexelSize;

    float _BaseValue;
    float _InputValue;
    float _HighLightValue;
    float _HighLightOffsetX;
    float _HighLightOffsetY;
    float _BrightnessThreshold;

    float _MainXOffset;
    float _MainYOffset;
    float _ScreenXOffset;
    float _ScreenYOffset;
    
    float4 pixel_col;
    
    struct v2f
    {
        float4 pos : POSITION;
        half2 uv_MainTex : TEXCOORD0;
        half2 uv_ScreenTex : TEXCOORD1;
        half2 uv_HighLightTex : TEXCOORD2;
    };

    v2f vert_aspectfix(v2f v)
    {
        v2f o;
        o.pos = UnityObjectToClipPos(v.pos);
        o.uv_MainTex = TRANSFORM_TEX(v.uv_MainTex, _MainTex);
        o.uv_ScreenTex = TRANSFORM_TEX(v.uv_ScreenTex, _ScreenTex);
        o.uv_HighLightTex = TRANSFORM_TEX(v.uv_HighLightTex, _HighLightTex);

        float mainWidth = _MainTex_TexelSize.z;
        float mainHeight = _MainTex_TexelSize.w;
        float screenWidth = _ScreenTex_TexelSize.z;
        float screenHeight = _ScreenTex_TexelSize.w;

        float mainXOffset = _MainXOffset/screenWidth;
        float mainYOffset = _MainYOffset/screenHeight;
        float screenXOffset = _ScreenXOffset/screenWidth;
        float screenYOffset = _ScreenYOffset/screenHeight;
        
        o.uv_ScreenTex.x *= mainWidth/screenWidth;
        o.uv_ScreenTex.y *= mainHeight/screenHeight;
        
        o.uv_ScreenTex.x += mainXOffset - screenXOffset;
        o.uv_ScreenTex.y += mainYOffset - screenYOffset;
        
        return o;
    }

    //##############################mix Pass 0##############################

    half4 frag_dynMask (v2f i) : SV_Target
    {
        float4 pixel_col = tex2D(_MainTex,i.uv_MainTex);
        float4 screen = tex2D(_ScreenTex,i.uv_ScreenTex);
        
        if (screen.r > _BrightnessThreshold)
        {
            screen.r -= _BrightnessThreshold;
            screen.r = screen.r/(screen.r + 1) + _BrightnessThreshold;
        }

        if (screen.g > _BrightnessThreshold)
        {
            screen.g -= _BrightnessThreshold;
            screen.g = screen.g/(screen.g + 1) + _BrightnessThreshold;
        }

        if (screen.b > _BrightnessThreshold)
        {
            screen.b -= _BrightnessThreshold;
            screen.b = screen.b/(screen.b + 1) + _BrightnessThreshold;
        }
        
        pixel_col.rgb *= _BaseValue + _InputValue * screen.rgb;

        float2 offset = float2(-_HighLightOffsetX * _HighLightTex_TexelSize.x,_HighLightOffsetY * _HighLightTex_TexelSize.y); 
        pixel_col.rgb *= 1+_HighLightValue * screen.rgb * (1 - tex2D(_HighLightTex,i.uv_HighLightTex+offset).a);
        
        return half4(pixel_col); 
    }
    
    ENDCG

    SubShader
    {
        Pass
        {
            name "mix"
            
            CGPROGRAM
            #pragma vertex vert_aspectfix
            #pragma fragment frag_dynMask
            ENDCG
        }
    }
}