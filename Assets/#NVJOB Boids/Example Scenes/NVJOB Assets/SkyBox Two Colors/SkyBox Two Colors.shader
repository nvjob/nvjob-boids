// Copyright (c) 2016 Unity Technologies. MIT license - license_unity.txt
// #NVJOB SkyBox Two Colors. MIT license - license_nvjob.txt
// Copyright (c) 2016 #NVJOB Nicholas Veselov - https://nvjob.github.io
// SkyBox Two Colors V1.2 - https://nvjob.github.io/unity/skybox-two-colors
// You can become one of the patrons, or make a sponsorship donation - https://nvjob.github.io/patrons


Shader "#NVJOB/SkyBox/Two Colors" {


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


Properties{
//----------------------------------------------

[Header(Color A Settings)][Space(5)]
_ColorA("Color A", Color) = (0.0, 0.0, 0.0, 0)
_IntensityA("Intensity A", Float) = 1.1
_DirA("Direction A", Vector) = (0.18, -1.64, -0.19, 0)
[Header(Color B Settings)][Space(5)]
_ColorB("Color B", Color) = (0.0, 0.4, 0.4, 0)
_IntensityB("Intensity B", Float) = 1.1
_DirB("Direction B", Vector) = (1.42, -2.26, -0.50, 0)
[Header(Noise Mix)][Space(5)]
_NoiseScale("Noise Scale", Float) = 250
_NoiseIntensity("Noise Intensity", Float) = 1

//----------------------------------------------
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


SubShader {
Tags { "RenderType" = "Background" "Queue" = "Background" }

///////////////////////////////////////////////////////////////////////////////////////////////////////////////

Pass {
//----------------------------------------------

ZWrite Off
Cull Off
Fog { Mode Off }
CGPROGRAM
#pragma fragmentoption ARB_precision_hint_fastest
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"

//----------------------------------------------

struct appdata {
float4 position : POSITION;
float3 texcoord : TEXCOORD0;
};

//----------------------------------------------

struct v2f {
float4 position : SV_POSITION;
float3 texcoord : TEXCOORD0;
float4 worldpos : any;
};

//----------------------------------------------

half4 _ColorA, _ColorB, _DirA, _DirB;
half _NoiseScale, _NoiseIntensity, _IntensityA, _IntensityB;

//----------------------------------------------

v2f vert(appdata v) {
v2f o;
o.position = UnityObjectToClipPos(v.position);
o.texcoord = v.texcoord;
o.worldpos = o.position;
return o;
}

//----------------------------------------------

half4 frag(v2f i) : COLOR{
float2 wc = (i.worldpos.xy / i.worldpos.w) * _NoiseScale;
float4 dt = (dot(float2(171.0f, 231.0f), wc.xy));
dt.rgb = frac(dt / float3(103.0f, 71.0f, 97.0f)) - float3(0.5f, 0.5f, 0.5f);
half da = dot(normalize(i.texcoord), _DirA) * (0.11f + _DirA.w) + _IntensityA;
half db = dot(normalize(i.texcoord), _DirB) * (0.11f + _DirB.w) + _IntensityB;
return (lerp(_ColorA, _ColorB, pow(da * db, 2))) + (dt / 255.0f) * _NoiseIntensity;
}

//----------------------------------------------

ENDCG
}

///////////////////////////////////////////////////////////////////////////////////////////////////////////////
}

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}