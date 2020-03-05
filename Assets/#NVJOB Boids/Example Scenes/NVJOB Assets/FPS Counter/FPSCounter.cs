// Copyright (c) 2016 Unity Technologies. MIT license - license_unity.txt
// #NVJOB FPS counter and graph - simple and fast. MIT license - license_nvjob.txt
// #NVJOB FPS counter and graph - simple and fast V1.2.5 - https://nvjob.github.io/unity/nvjob-fps-counter-and-graph
// #NVJOB Nicholas Veselov (independent developer) - https://nvjob.github.io


using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

[HelpURL("https://nvjob.github.io/unity/nvjob-fps-counter-and-graph")]
[AddComponentMenu("#NVJOB/Tools/FPS Counter and Graph")]


///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


public class FPSCounter : MonoBehaviour
{
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    [Header("Counter Settings")]
    public int frameUpdate = 60;
    public int highestPossibleFPS = 300;

    [Header("Graph Settings")]
    public float graphUpdate = 1.0f;
    public Color graphColor = new Color(1, 1, 1, 0.5f);

    [Header("Information")] // These variables are only information.
    public string HelpURL = "nvjob.github.io/unity/nvjob-fps-counter-and-graph";
    public string ReportAProblem = "nvjob.github.io/support";
    public string Patrons = "nvjob.github.io/patrons";

    //--------------

    GameObject counter, graph;
    Transform graphTr;
    Text counterText;
    float ofsetX;
    int curCount, lineCount;

    //--------------

    static WaitForSeconds stGraphUpdate;
    static GameObject[] stLines;
    static int stNumLines;


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    void Awake()
    {
        //--------------

        CreateCounter();

        //--------------
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    void OnEnable()
    {
        //--------------

        StartCoroutine(DrawGraph());

        //--------------
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    void Update()
    {
        //--------------

        // StFPS.Counter().x - min fps
        // StFPS.Counter().y - avg fps
        // StFPS.Counter().z - max fps

        Vector3Int allFps = StFPS.Counter(frameUpdate, Time.deltaTime);
        curCount = allFps.y;
        counterText.text = "MIN " + allFps.x.ToString() + " | AVG " + allFps.y.ToString() + " | MAX " + allFps.z.ToString();

        //-------------- 

        if (Input.GetKeyDown(KeyCode.F1)) // Hide Counter
        {
            counter.SetActive(!counter.activeSelf);
            graph.SetActive(!graph.activeSelf);
        }

        //--------------
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    IEnumerator DrawGraph()
    {
        //--------------

        while (true)
        {
            yield return stGraphUpdate;

            GameObject obj = GiveLine();
            Image img = obj.GetComponent<Image>();
            img.rectTransform.anchorMin = new Vector2(ofsetX, 0);
            float anchorMaxY = 1.0f / highestPossibleFPS * curCount;
            if (anchorMaxY > 1) anchorMaxY = 1;
            img.rectTransform.anchorMax = new Vector2(ofsetX + 0.01f, anchorMaxY);
            img.rectTransform.offsetMax = img.rectTransform.offsetMin = new Vector2(0, 0);
            obj.SetActive(true);

            if (lineCount++ > 49)
            {
                foreach (Transform child in graphTr) child.gameObject.SetActive(false);
                ofsetX = lineCount = 0;
            }
            else ofsetX += 0.02f;
        }

        //--------------
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    void CreateCounter()
    {
        //--------------

        stGraphUpdate = new WaitForSeconds(graphUpdate);

        counter = transform.Find("Counter").gameObject;
        counterText = transform.Find("Counter/CounterText").gameObject.GetComponent<Text>();
        transform.Find("Counter/MaxFPSText").gameObject.GetComponent<Text>().text = highestPossibleFPS.ToString();
        transform.Find("Counter/HalfFPSText").gameObject.GetComponent<Text>().text = Mathf.Round(highestPossibleFPS * 0.5f).ToString();

        graphTr = transform.Find("Graph");
        graph = graphTr.gameObject;        

        stNumLines = 100;
        stLines = new GameObject[stNumLines];

        for (int i = 0; i < stNumLines; i++)
        {
            stLines[i] = new GameObject();
            stLines[i].SetActive(false);
            stLines[i].name = "Line_" + i;
            stLines[i].transform.parent = graphTr;
            Image img = stLines[i].AddComponent<Image>();
            img.rectTransform.localScale = Vector3.one;
            img.color = graphColor;
        }

        //--------------
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    static GameObject GiveLine()
    {
        //--------------

        for (int i = 0; i < stNumLines; i++) if (!stLines[i].activeSelf) return stLines[i];
        return null;

        //--------------
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}


///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


public static class StFPS
{
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    static List<float> fpsBuffer = new List<float>();
    static float fpsB;
    static Vector3Int fps;


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    public static Vector3Int Counter(int frameUpdate, float deltaTime)
    {
        //--------------

        int fpsBCount = fpsBuffer.Count;

        if (fpsBCount <= frameUpdate) fpsBuffer.Add(1.0f / Time.deltaTime);
        else
        {
            fps.x = Mathf.RoundToInt(fpsBuffer.Min());
            fps.z = Mathf.RoundToInt(fpsBuffer.Max());
            for (int f = 0; f < fpsBCount; f++) fpsB += fpsBuffer[f];
            fpsBuffer = new List<float> { 1.0f / Time.deltaTime };
            fpsB = fpsB / fpsBCount;
            fps.y = Mathf.RoundToInt(fpsB);
            fpsB = 0;
        }

        if (Time.timeScale == 1 && fps.y > 0 ) return fps;
        else return Vector3Int.zero;

        //--------------
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}