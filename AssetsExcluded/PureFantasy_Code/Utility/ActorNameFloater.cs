using UnityEngine;
using System.Collections;

public class ActorNameFloater : MonoBehaviour {

    public Actor actor;
    public Camera cam;
    public GameObject go;
    public TextMesh text;
    string nickname;
    public int textsize = 60;
    public Color textcolor = Color.black;


    void Awake () {
        actor = this.GetComponent<Actor>();
        go = new GameObject("textMesh: " + actor._ActorData._Name);
        text = go.AddComponent<TextMesh>();
        //text.gameObject.AddComponent<MeshRenderer>();
    }

    void Start()
    {
        cam = Camera.main;
        go.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        text.alignment = TextAlignment.Center;
        text.anchor = TextAnchor.LowerCenter;
        text.fontSize = textsize;
        text.color = textcolor;
    }
	
	void FixedUpdate () {
        text.transform.position = this.transform.position + Vector3.up * 1.0f;
        text.transform.rotation = cam.transform.rotation;
        if (actor._ActorData._Nickname != "") nickname = "\"" + actor._ActorData._Nickname + "\"";
        text.text = actor._ActorData._Name + " " + nickname + " " + actor._ActorData._Surname + "\n" + actor._ActorData._Title;
    }
}
