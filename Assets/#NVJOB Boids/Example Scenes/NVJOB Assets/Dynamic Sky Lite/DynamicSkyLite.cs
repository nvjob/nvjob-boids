// Copyright (c) 2016 Unity Technologies. MIT license - license_unity.txt
// #NVJOB Dynamic Sky Lite (Standard render). MIT license - license_nvjob.txt
// #NVJOB Dynamic Sky Lite (Standard render) V2.3 - https://nvjob.github.io/unity/nvjob-dynamic-sky-lite
// #NVJOB Nicholas Veselov (independent developer) - https://nvjob.github.io


using UnityEngine;

[HelpURL("https://nvjob.github.io/unity/nvjob-dynamic-sky-lite")]
[AddComponentMenu("#NVJOB/Dynamic Sky/Dynamic Sky Lite")]


///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


public class DynamicSkyLite : MonoBehaviour
{
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    [Header("Settings")]
    public float ssgUvRotateSpeed = 1;
    public float ssgUvRotateDistance = 1;
    public Transform player;

    [Header("Information")] // These variables are only information.
    public string HelpURL = "nvjob.github.io/unity/nvjob-dynamic-sky-lite";
    public string ReportAProblem = "nvjob.github.io/support";
    public string Patrons = "nvjob.github.io/patrons";


    //--------------

    Vector2 ssgVector;
    Transform tr;


    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    


    private void Awake()
    {
        //--------------

        ssgVector = Vector2.zero;
        tr = transform;

        //--------------
    }



    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////



    void LateUpdate()
    {
        //--------------

        ssgVector = Quaternion.AngleAxis(Time.time * ssgUvRotateSpeed, Vector3.forward) * Vector2.one * ssgUvRotateDistance;
        Shader.SetGlobalFloat("_SkyShaderUvX", ssgVector.x);
        Shader.SetGlobalFloat("_SkyShaderUvZ", ssgVector.y);

        //--------------

        tr.position = new Vector3(player.position.x, tr.position.y, player.position.z);

        //--------------
    }



    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}
