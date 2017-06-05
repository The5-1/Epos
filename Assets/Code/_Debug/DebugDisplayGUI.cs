using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using UnityEngine.UI;

/// <summary>
/// Some global static class to call like DebugGUI.Log()
/// that then fires an event that the DebugGUIDisplay uses
/// </summary>
public static class DebugGUI
{
    public delegate void LogPosted(string text);
    public static event LogPosted LogPostedEvent;

    public static void Log(string text)
    {
        //fire event
        LogPostedEvent(text);
    }
}


public class DebugDisplayGUI : MonoBehaviour
{
    private StringBuilder SB;
    private int log_numberOfLinesMax = 60;
    private int log_numberOfLines = 0;
    private int log_lineClearIntervall = 60;

    private GameObject canvasGO;
    private Canvas canvas;
    private GraphicRaycaster canvasGraphicRaycaster;
    private RectTransform canvasRectTransform;
    public Camera camera;

    private IEnumerator clearLineCoroutine()
    {
        while (true)
        {
            removeFirstLine();
            yield return new WaitForSeconds(log_lineClearIntervall); //respects unity internal timescale
        }
    }

    void OnGUI()
    {
        //this is only for old/obsolete Immediate Mode GUI
        //current unity uses the Canvas GameObject
    }

    void Awake()
    {
        init();
    }

    private void init()
    {
        SB = new StringBuilder();

        createGUI();

        subscribeToEvents();
        startCoroutines();
    }

    private void createGUI()
    {
        camera = Camera.main;

        canvasGO = new GameObject("_DebugDisplayGUI");
        DontDestroyOnLoad(canvasGO);
        canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasRectTransform = canvasGO.GetComponent<RectTransform>();
        canvasGraphicRaycaster = canvasGO.AddComponent<GraphicRaycaster>();


        /*
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        Vector3 pos = camera.transform.position;
        pos += camera.transform.forward * 10.0f;
        canvas.worldCamera = camera;

        GameObject buttonGO = new GameObject("button");
        RectTransform buttonRT = buttonGO.AddComponent<RectTransform>();
        buttonRT.SetParent(canvasRectTransform);
        buttonRT.sizeDelta = new Vector2(200.0f, 100.0f);
        Button buttonBU = buttonGO.AddComponent<Button>();
        buttonBU.onClick.AddListener(() => { Debug.Log("button clicked"); });
        Image buttonIM = buttonGO.AddComponent<Image>();
        buttonIM.sprite = Resources.Load("UISprite", typeof(Sprite)) as Sprite;
        */
    }


    private void subscribeToEvents()
    {
        DebugGUI.LogPostedEvent += addLine;
    }

    private void startCoroutines()
    {
        StartCoroutine(clearLineCoroutine());
    }

    public void addLine(string text)
    {
        if (log_numberOfLines >= log_numberOfLinesMax) removeFirstLine();
        SB.Append(text);
        SB.Append("\n");
        log_numberOfLines++;
    }

    private void removeFirstLine()
    {
        string s = SB.ToString();
        SB.Length = 0;
        SB.Capacity = 0;
        SB.Append(s.Substring(s.IndexOf("\n") + 1));
        log_numberOfLines--;
    }
}
