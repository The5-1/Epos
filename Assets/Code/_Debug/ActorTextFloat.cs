using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorTextFloat : MonoBehaviour {

    public int textsize = 60;
    public Color textcolor;
    public float height = 2.0f;

    public Actor _parentActor;
    public GameObject _thisGO;
    public GameObject _textGO;
    public GameObject _parentGO;
    public TextMesh _textmesh;
    public Camera _cam;
    public string _text;

#if true
    //this is a Property
    public Quaternion _camTargetRotation 
    {
        get { return _camTargetRotation; }
        set { _camTargetRotation = value; } 
    }

    //Test for coroutines and events to set properties
    IEnumerator coroutine_turn_to_camera(Vector3 target)
    {
        while (Quaternion.Angle(_textGO.transform.rotation, _cam.transform.rotation) > 0.005f)
        {
            _textGO.transform.rotation = Quaternion.Lerp(_cam.transform.rotation, _camTargetRotation, 7f * Time.deltaTime);

            yield return null;
        }
    }

#endif

    private void Awake()
    {
        _thisGO = this.gameObject;
        _textGO = new GameObject("textMesh");
        _textGO.transform.parent = this.transform;
        _textmesh = _textGO.AddComponent<TextMesh>();

        textcolor = new Color(0.0f, 0.0f, 0.0f, 0.5f);

    }

    private void Start()
    {
        _parentGO = this.transform.parent.gameObject;
        _cam = Camera.main;
        _textGO.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
        _textmesh.alignment = TextAlignment.Center;
        _textmesh.anchor = TextAnchor.LowerCenter;
        _textmesh.fontSize = textsize;
        _textmesh.color = textcolor;
        _textGO.transform.SetParent(_parentGO.gameObject.transform);
    }

    //FIXME: this should not be called every frame but just when the actor changes! Event system...
    void FixedUpdate()
    {
        _parentActor = _parentGO.GetComponent<Actor>();
        if (_parentActor)
        {
            _text = _parentActor._actorData.Name; //INFO: stringbuilder is faster since you can set the max size and dont have to reallocate, but not worth for debugging
            _text += "\n Age: " + _parentActor._actorData.AgeInSeconds + "/" + _parentActor._actorData.MaxAgeInSeconds;
            _text += "\n Gender: " + _parentActor._actorData.BreedData.gender.ToString();
            _textGO.transform.position = this.transform.position + Vector3.up * height;
            _textGO.transform.rotation = _cam.transform.rotation;
            _textmesh.text = _text;
        }
    }
}
