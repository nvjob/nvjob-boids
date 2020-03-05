// Copyright (c) 2016 Unity Technologies. MIT license - license_unity.txt
// #NVJOB Simple Boids. MIT license - license_nvjob.txt
// #NVJOB Nicholas Veselov - https://nvjob.github.io
// #NVJOB Simple Boids v1.1.1 - https://nvjob.github.io/unity/nvjob-boids


using System.Collections;
using UnityEngine;

[HelpURL("https://nvjob.github.io/unity/nvjob-boids")]
[AddComponentMenu("#NVJOB/Boids/Simple Boids")]


///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


public class NVBoids : MonoBehaviour
{
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    [Header("General Settings")]
    public Vector2 behavioralCh = new Vector2(2.0f, 6.0f);
    public bool debug;

    [Header("Flock Settings")]
    [Range(1, 150)] public int flockNum = 2;
    [Range(0, 5000)] public int fragmentedFlock = 30;
    [Range(0, 1)] public float fragmentedFlockYLimit = 0.5f;
    [Range(0, 1.0f)] public float migrationFrequency = 0.1f;
    [Range(0, 1.0f)] public float posChangeFrequency = 0.5f;
    [Range(0, 100)] public float smoothChFrequency = 0.5f;

    [Header("Bird Settings")]
    public GameObject birdPref;
    [Range(1, 9999)] public int birdsNum = 10;
    [Range(0, 150)] public float birdSpeed = 1;
    [Range(0, 100)] public int fragmentedBirds = 10;
    [Range(0, 1)] public float fragmentedBirdsYLimit = 1;
    [Range(0, 10)] public float soaring = 0.5f;
    [Range(0.01f, 500)] public float verticalWawe = 20;
    public bool rotationClamp = false;
    [Range(0, 360)] public float rotationClampValue = 50;
    public Vector2 scaleRandom = new Vector2(1.0f, 1.5f);

    [Header("Danger Settings (one flock)")]
    public bool danger;
    public float dangerRadius = 15;
    public float dangerSpeed = 1.5f;
    public float dangerSoaring = 0.5f;
    public LayerMask dangerLayer;

    [Header("Information")] // These variables are only information.
    public string HelpURL = "nvjob.github.io/unity/nvjob-boids";
    public string ReportAProblem = "nvjob.github.io/support";
    public string Patrons = "nvjob.github.io/patrons";

    //-------------- 

    Transform thisTransform, dangerTransform;
    int dangerBird;
    Transform[] birdsTransform, flocksTransform;
    Vector3[] rdTargetPos, flockPos, velFlocks;
    float[] birdsSpeed, birdsSpeedCur, spVelocity;
    int[] curentFlock;
    float dangerSpeedCh, dangerSoaringCh;
    float timeTime;
    static WaitForSeconds delay0;


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    void Awake()
    {
        //--------------

        thisTransform = transform;
        CreateFlock();
        CreateBird();
        StartCoroutine(BehavioralChange());
        StartCoroutine(Danger());

        //--------------
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    void LateUpdate()
    {
        //--------------  

        FlocksMove();
        BirdsMove();

        //--------------
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    void FlocksMove()
    {
        //--------------  

        for (int f = 0; f < flockNum; f++)
        {
            flocksTransform[f].localPosition = Vector3.SmoothDamp(flocksTransform[f].localPosition, flockPos[f], ref velFlocks[f], smoothChFrequency);
        }

        //--------------
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    void BirdsMove()
    {
        //--------------

        float deltaTime = Time.deltaTime;
        timeTime += deltaTime;
        Vector3 translateCur = Vector3.forward * birdSpeed * dangerSpeedCh * deltaTime;
        Vector3 verticalWaweCur = Vector3.up * ((verticalWawe * 0.5f) - Mathf.PingPong(timeTime * 0.5f, verticalWawe));
        float soaringCur = soaring * dangerSoaring * deltaTime;

        //--------------

        for (int b = 0; b < birdsNum; b++)
        {
            if (birdsSpeedCur[b] != birdsSpeed[b]) birdsSpeedCur[b] = Mathf.SmoothDamp(birdsSpeedCur[b], birdsSpeed[b], ref spVelocity[b], 0.5f);
            birdsTransform[b].Translate(translateCur * birdsSpeed[b]);
            Vector3 tpCh = flocksTransform[curentFlock[b]].position + rdTargetPos[b] + verticalWaweCur - birdsTransform[b].position;
            Quaternion rotationCur = Quaternion.LookRotation(Vector3.RotateTowards(birdsTransform[b].forward, tpCh, soaringCur, 0));
            if (rotationClamp == false) birdsTransform[b].rotation = rotationCur;
            else birdsTransform[b].localRotation = BirdsRotationClamp(rotationCur, rotationClampValue);
        }

        //--------------
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    IEnumerator Danger()
    {
        //--------------

        if (danger == true)
        {
            delay0 = new WaitForSeconds(1.0f);

            while (true)
            {
                if (Random.value > 0.9f) dangerBird = Random.Range(0, birdsNum);
                dangerTransform.localPosition = birdsTransform[dangerBird].localPosition;

                if (Physics.CheckSphere(dangerTransform.position, dangerRadius, dangerLayer))
                {
                    dangerSpeedCh = dangerSpeed;
                    dangerSoaringCh = dangerSoaring;
                    yield return delay0;
                }
                else dangerSpeedCh = dangerSoaringCh = 1;

                yield return delay0;
            }
        }
        else dangerSpeedCh = dangerSoaringCh = 1;

        //--------------
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    IEnumerator BehavioralChange()
    {
        //--------------

        while (true)
        {
            yield return new WaitForSeconds(Random.Range(behavioralCh.x, behavioralCh.y));

            //---- Flocks

            for (int f = 0; f < flockNum; f++)
            {
                if (Random.value < posChangeFrequency)
                {
                    Vector3 rdvf = Random.insideUnitSphere * fragmentedFlock;
                    flockPos[f] = new Vector3(rdvf.x, Mathf.Abs(rdvf.y * fragmentedFlockYLimit), rdvf.z);
                }
            }

            //---- Birds

            for (int b = 0; b < birdsNum; b++)
            {
                birdsSpeed[b] = Random.Range(3.0f, 7.0f);
                Vector3 lpv = Random.insideUnitSphere * fragmentedBirds;
                rdTargetPos[b] = new Vector3(lpv.x, lpv.y * fragmentedBirdsYLimit, lpv.z);
                if (Random.value < migrationFrequency) curentFlock[b] = Random.Range(0, flockNum);
            } 
        }

        //--------------
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    void CreateFlock()
    {
        //--------------

        flocksTransform = new Transform[flockNum];
        flockPos = new Vector3[flockNum];
        velFlocks = new Vector3[flockNum];
        curentFlock = new int[birdsNum];

        for (int f = 0; f < flockNum; f++)
        {
            GameObject nobj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            nobj.SetActive(debug);
            flocksTransform[f] = nobj.transform;
            Vector3 rdvf = Random.onUnitSphere * fragmentedFlock;
            flocksTransform[f].position = thisTransform.position;
            flockPos[f] = new Vector3(rdvf.x, Mathf.Abs(rdvf.y * fragmentedFlockYLimit), rdvf.z);
            flocksTransform[f].parent = thisTransform;
        }

        //-------------- // For Danger and for flock hunter

        if (danger == true)
        {
            GameObject dobj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            dobj.GetComponent<MeshRenderer>().enabled = debug;
            dobj.layer = gameObject.layer;
            dangerTransform = dobj.transform;
            dangerTransform.parent = thisTransform;
        }

        //--------------
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    void CreateBird()
    {
        //--------------

        birdsTransform = new Transform[birdsNum];
        birdsSpeed = new float[birdsNum];
        birdsSpeedCur = new float[birdsNum];
        rdTargetPos = new Vector3[birdsNum];
        spVelocity = new float[birdsNum];

        for (int b = 0; b < birdsNum; b++)
        {
            birdsTransform[b] = Instantiate(birdPref, thisTransform).transform;
            Vector3 lpv = Random.insideUnitSphere * fragmentedBirds;
            birdsTransform[b].localPosition = rdTargetPos[b] = new Vector3(lpv.x, lpv.y * fragmentedBirdsYLimit, lpv.z);
            birdsTransform[b].localScale = Vector3.one * Random.Range(scaleRandom.x, scaleRandom.y);
            birdsTransform[b].localRotation = Quaternion.Euler(0, Random.value * 360, 0);
            curentFlock[b] = Random.Range(0, flockNum);
            birdsSpeed[b] = Random.Range(3.0f, 7.0f);
        }

        //--------------
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    static Quaternion BirdsRotationClamp(Quaternion rotationCur, float rotationClampValue)
    {
        //--------------

        Vector3 angleClamp = rotationCur.eulerAngles;
        rotationCur.eulerAngles = new Vector3(Mathf.Clamp((angleClamp.x > 180) ? angleClamp.x - 360 : angleClamp.x, -rotationClampValue, rotationClampValue), angleClamp.y, 0);
        return rotationCur;

        //--------------
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}