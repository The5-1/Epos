using UnityEngine;
using System.Collections;

public class MoveStateFloater : MonoBehaviour {

    public MovementController movCon;
    public Camera cam;
    public GameObject go;
    public TextMesh text;
    string nickname;
    public int textsize = 80;
    public Color textcolor = Color.black;
    public Rigidbody rb;


    void Awake () {
        movCon = this.GetComponent<MovementController>();
        go = new GameObject("textMesh");
        text = go.AddComponent<TextMesh>();
        //text.gameObject.AddComponent<MeshRenderer>();
        rb = this.GetComponent<Rigidbody>();
    }

    void Start()
    {
        cam = Camera.main;
        go.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        text.alignment = TextAlignment.Left;
        text.anchor = TextAnchor.LowerCenter;
        text.fontSize = textsize;
        text.color = textcolor;
    }
	
	void FixedUpdate () {
        text.transform.position = this.transform.position + Vector3.up * 1.0f;
        text.transform.rotation = cam.transform.rotation;
        text.text = "Speed: " + movCon._State_MovementSpeed + "\n" +
            "Terrain: " + movCon._State_Terrain + "\n" +
            "Headspace: " + movCon._State_Headspace + "\n" +
            "Climb: " + movCon._State_LatchedSurface + "\n" +
            "Ragdoll: " + movCon._State_Ragdoll + "\n" +
            "\n" +
            "RB velocit: " + rb.velocity + ":" + rb.velocity.magnitude + "\n" +
            "RB angleVelocit: " + rb.angularVelocity + ":" + rb.angularVelocity.magnitude + "\n" +
            "MoveSpeed: " + movCon._FinalMoveVector.magnitude;
    }
}
