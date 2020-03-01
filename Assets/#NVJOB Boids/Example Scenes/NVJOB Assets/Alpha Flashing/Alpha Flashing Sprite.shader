// Copyright (c) 2016 Unity Technologies. MIT license - license_unity.txt
// #NVJOB Alpha Flashing Text. MIT license - license_nvjob.txt
// Copyright (c) 2016 #NVJOB Nicholas Veselov - https://nvjob.github.io
// #NVJOB Alpha Flashing Text 2.1 - https://nvjob.github.io/unity/alpha-flashing-text
// You can become one of the patrons, or make a sponsorship donation - https://nvjob.github.io/patrons


Shader "#NVJOB/Alpha Flashing Sprite" {


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


Properties{
//----------------------------------------------

[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
[HDR]_Color("Tint 0", Color) = (1,1,1,1)
[Toggle(TINT_ONE_ON)]
_TintOneOn("Tint 1 On", int) = 1
[HDR]_Color1("Tint 1", Color) = (1,1,1,1)
[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
[Header(Blink)][Space(5)]
_SpeedCC("Blink Color rate", float) = 1.0
_Speed("Blink rate", float) = 1.0
_VectorValue("Brightness Vector Value", Vector) = (0,0,0,0.2)
[Space(5)]
[Toggle(UNSCALED_TIME_ON)]
_UnscaledTime("Unscaled Time On", int) = 0
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
#pragma shader_feature UNSCALED_TIME_ON
#pragma shader_feature TINT_ONE_ON
#include "UnityCG.cginc"

//----------------------------------------------

sampler2D _MainTex, _AlphaTex;
float _AlphaSplitEnabled;
fixed4 _Color, _Color1;
fixed _Saturation, _Contrast, _Brightness;
half _Speed, _SpeedCC;
half4 _VectorValue;
float timeCh, _NV_AF_Time;

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

v2f vert(appdata_t IN) {
v2f OUT;
OUT.vertex = UnityObjectToClipPos(IN.vertex);
OUT.texcoord = IN.texcoord;
#ifdef UNSCALED_TIME_ON
timeCh = _NV_AF_Time;
#else
timeCh = _Time.y;
#endif
#ifdef TINT_ONE_ON
half4 ccs = _Color + (_Color1 * sin(timeCh * _SpeedCC) * 0.5);
OUT.color = IN.color * ccs;
#else
OUT.color = IN.color * _Color;
#endif
#ifdef PIXELSNAP_ON
OUT.vertex = UnityPixelSnap(OUT.vertex);
#endif
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
half2 wpNorm = IN.texcoord;
half vectorSum = (sin(wpNorm.x) * _VectorValue.x) + (sin(wpNorm.y) * _VectorValue.y);
#ifdef UNSCALED_TIME_ON
timeCh = _NV_AF_Time;
#else
timeCh = _Time.y;
#endif
half sinTime = sin(vectorSum + (timeCh * _Speed)) * _VectorValue.w;
fixed cola = tex.a * (abs(sinTime) + _VectorValue.z);
color.rgb *= cola;
return float4(color, cola);
}

//----------------------------------------------

ENDCG
}

///////////////////////////////////////////////////////////////////////////////////////////////////////////////
}

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}