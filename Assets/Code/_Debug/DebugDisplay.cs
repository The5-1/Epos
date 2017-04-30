using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class DebugDisplay : MonoBehaviour {

    public static List<string> DebugStrings;
    private static Dictionary<string,string> DebugDictionary;
    private StringBuilder SB;

    float deltaTime = 0.0f;
    public int textsize = 4;

    private void Awake()
    {
        DebugStrings = new List<string>();
        DebugDictionary = new Dictionary<string, string>();
        SB = new StringBuilder();
        DebugStrings.Add("hello");
        DebugStrings.Add("world");
    }

    void FixedUpdate()
    {
        SB.Remove(0, SB.Length);
        foreach(string s in DebugStrings)
        {
            SB.AppendLine(s);
        }

        foreach (string s in DebugDictionary.Values)
        {
            SB.AppendLine(s);
        }
    }

    public static void setString(string key, string s)
    {
        DebugDictionary.Add(key, s);
    }

    void OnGUI()
    {
        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(0, 20, w, h * textsize / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * textsize / 100;
        style.normal.textColor = new Color(0.5f, 0.5f, 0.5f, 1.0f);

        string text = SB.ToString();
        GUI.Label(rect, text, style);
    }
}
