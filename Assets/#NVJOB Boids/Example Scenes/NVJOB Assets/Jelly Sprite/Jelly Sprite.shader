// Copyright (c) 2016 Unity Technologies. MIT license - license_unity.txt
// #NVJOB 2D Set Jelly Sprite. MIT license - license_nvjob.txt
// Copyright (c) 2016 #NVJOB Nicholas Veselov - https://nvjob.github.io
// #NVJOB 2D Set Jelly Sprite V1.1 - https://nvjob.github.io/unity/nvjob-2dset-jelly-sprite.html
// You can become one of the patrons, or make a sponsorship donation - https://nvjob.github.io/patrons


Shader "#NVJOB/2D Set/Jelly Sprite" {


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


Properties{
//----------------------------------------------

[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
_Color("Tint", Color) = (1,1,1,1)
[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
[Header(Jelly Settings)][Space(5)]
_JellyX("Jelly X Amplitude", Range(-50, 50)) = 0.1
_JellyXs("Jelly X Amp Speed", Range(-50, 50)) = 1
[Space(10)]
_JellyXB("Jelly X Bias", Range(-50, 50)) = 0.1
_JellyXBs("Jelly X Bias Speed", Range(-50, 50)) = 1
[Space(10)]
_JellyY("Jelly Y Amplitude", Range(-50, 50)) = 0.1
_JellyYs("Jelly Y Speed", Range(-50, 50)) = 1
[Space(10)]
_JellyYB("Jelly Y Bias", Range(-50, 50)) = 0.1
_JellyYBs("Jelly Y Bias Speed", Range(-50, 50)) = 1
[Header(Other Settings)][Space(5)]
_Saturation("Saturation", Range(0, 5)) = 1
_Brightness("Brightness", Range(0, 5)) = 1
_Contrast("Contrast", Range(0, 5)) = 1

//----------------------------------------------
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


SubShader{
///////////////////////////////////////////////////////////////////////////////////////////////////////////////

Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" "PreviewType" = "Plane" "CanUseSpriteAtlas" = "True" }

Cull Off
Lighting Off
ZWrite Off
Blend One OneMinusSrcAlpha

///////////////////////////////////////////////////////////////////////////////////////////////////////////////

Pass{
//----------------------------------------------

CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile _ PIXELSNAP_ON
#include "UnityCG.cginc"

//----------------------------------------------

struct appdata_t {
float4 vertex : POSITION;
float4 color : COLOR;
float2 texcoord : TEXCOORD0;
};

//----------------------------------------------

struct v2f {
float4 vertex : SV_POSITION;
fixed4 color : COLOR;
float2 texcoord : TEXCOORD0;
};

//----------------------------------------------

sampler2D _MainTex;
sampler2D _AlphaTex;
float _AlphaSplitEnabled;
fixed4 _Color;
fixed _JellyX, _JellyXs, _JellyXB, _JellyXBs;
fixed _JellyY, _JellyYs, _JellyYB, _JellyYBs;
fixed _Saturation, _Contrast, _Brightness;

//----------------------------------------------

v2f vert(appdata_t IN) {
v2f OUT;
OUT.vertex = UnityObjectToClipPos(IN.vertex);
OUT.texcoord = IN.texcoord;
OUT.color = IN.color * _Color;
#ifdef PIXELSNAP_ON
OUT.vertex = UnityPixelSnap(OUT.vertex);
#endif
IN.vertex.x += (sign(IN.vertex.x) * sin(_Time.w * _JellyXs)) * _JellyX;
IN.vertex.x += sin(_Time.w * _JellyXBs) * _JellyXB;
IN.vertex.y += (sign(IN.vertex.y) * cos(_Time.w * _JellyYs)) * _JellyY;
IN.vertex.y += sin(_Time.w * _JellyYBs) * _JellyYB;
OUT.vertex = UnityObjectToClipPos(IN.vertex);
OUT.texcoord = IN.texcoord;
return OUT;
}

//----------------------------------------------

fixed4 SampleSpriteTexture(float2 uv) {
fixed4 color = tex2D(_MainTex, uv);
#if UNITY_TEXTURE_ALPHASPLIT_ALLOWED
if (_AlphaSplitEnabled)
color.a = tex2D(_AlphaTex, uv).r;
#endif
return color;
}

//----------------------------------------------

fixed4 frag(v2f IN) : SV_Target{
fixed4 tex = SampleSpriteTexture(IN.texcoord) * IN.color;
fixed Lum = dot(tex, float3(0.2126, 0.7152, 0.0722));
fixed3 color = lerp(Lum.xxx, tex, _Saturation);
color = color * _Brightness;
color = (color - 0.5) * _Contrast + 0.5;
color.rgb *= tex.a;
return float4(color, tex.a);
}

//----------------------------------------------

ENDCG
}

///////////////////////////////////////////////////////////////////////////////////////////////////////////////
}

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}