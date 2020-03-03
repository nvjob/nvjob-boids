// Copyright (c) 2016 Unity Technologies. MIT license - license_unity.txt
// #NVJOB Simple Boids. MIT license - license_nvjob.txt
// #NVJOB Nicholas Veselov - https://nvjob.github.io
// #NVJOB Simple Boids v1.1.1 - https://nvjob.github.io/unity/nvjob-boids


using System.Collections;
using UnityEngine;

[HelpURL("https://nvjob.github.io/unity/nvjob-boids")]
[AddComponentMenu("#NVJOB/Boids/Shark")]


///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


public class Shark : MonoBehaviour
{
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    [Header("General Settings")]
    public float speed = 5;
    public float walkZone = 100;
    public GameObject camRig0, camRig1;
    public bool debug;

    [Header("Hunting Settings")]
    public float huntingZone = 50;
    public LayerMask layerFlock;
    public Material sharkMaterial;

    //--------------

    Transform thisTransform, camRigTr0, camRigTr1;
    Vector3 vel, velCam, target, targetCurent, targetRandom, targetFlock;
    float startYpos, huntTime, huntSpeed, speedSh, acselSh;
    bool hunting;
    static WaitForSeconds delay0 = new WaitForSeconds(8.0f);


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    void Awake()
    {
        //--------------

        thisTransform = transform;
        camRigTr0 = camRig0.transform;
        camRigTr1 = camRig1.transform;        

        startYpos = transform.position.y;
        huntSpeed = 1.0f; 

        camRig0.SetActive(true);
        camRig1.SetActive(false);        

        StartCoroutine(RandomVector());

        //--------------
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    void LateUpdate()
    {
        //--------------

        Hunting();
        Move();
        CamerasRig();
        DebugPath();

        //--------------
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    void Hunting()
    {
        //-------------- 

        if (hunting == false)
        {
            targetCurent = targetRandom;
            if (huntTime > 0) huntTime -= Time.deltaTime;
            else if (Physics.CheckSphere(thisTransform.position, huntingZone, layerFlock)) hunting = true;
            if (huntSpeed > 1.0f) huntSpeed -= Time.deltaTime * 0.2f;
            if (acselSh > 0) acselSh -= Time.deltaTime * 0.1f;
        }
        else
        {
            if (huntTime < 4.0f) huntTime += Time.deltaTime * 0.5f;
            else hunting = false;
            Collider[] flockColliders = Physics.OverlapSphere(thisTransform.position, huntingZone, layerFlock);
            if (flockColliders.Length > 0) targetFlock = flockColliders[0].transform.position;
            targetCurent = targetFlock;
            if (huntSpeed < 2.0f) huntSpeed += Time.deltaTime * 0.2f;
            if (acselSh < 0.5f) acselSh += Time.deltaTime * 0.1f;
        }

        if (acselSh >= 0)
        {
            speedSh += Time.deltaTime * acselSh;
            sharkMaterial.SetFloat("_ScriptControl", speedSh);
        }

        //--------------
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    void Move()
    {
        //--------------

        target = Vector3.SmoothDamp(target, targetCurent, ref vel, 3.0f);
        target = new Vector3(target.x, startYpos, target.y);
        Vector3 newDir = Vector3.RotateTowards(thisTransform.forward, target - thisTransform.position, Time.deltaTime * 0.35f, 0);
        thisTransform.rotation = Quaternion.LookRotation(newDir);
        thisTransform.Translate(Vector3.forward * Time.deltaTime * huntSpeed * speed);

        //--------------
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    void CamerasRig()
    {
        //--------------

        if (Input.GetMouseButtonDown(1))
        {
            camRig0.SetActive(!camRig0.activeSelf);
            camRig1.SetActive(!camRig1.activeSelf);
        }

        if (camRig0.activeSelf) camRigTr0.position = thisTransform.position;
        else
        {
            camRigTr1.position = thisTransform.position;
            camRigTr1.rotation = Quaternion.Lerp(camRigTr1.rotation, thisTransform.rotation, Time.deltaTime * 0.4f);
        }

        //--------------
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    IEnumerator RandomVector()
    {
        //--------------

        while (true)
        {
            targetRandom = Random.insideUnitSphere * walkZone;
            yield return delay0;
        }

        //--------------
    }
    

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    void DebugPath()
    {
        //--------------

        if (debug == true) Debug.DrawLine(thisTransform.position, targetCurent);

        //--------------
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}