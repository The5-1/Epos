using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Manager : MonoBehaviour {


    static public Camera_Manager singleton;

    public Camera cameraComponent;
    public Camera_Controller_RTS cameraController;

    protected void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
            DontDestroyOnLoad(singleton);
            init();
        }
        else { Destroy(this); }
    }

    protected void OnDestroy()
    {
        if (singleton == this) { singleton = null; }
    }

    private void init()
    {
        initCameraComponents();
        initCameraController();
    }

    private void initCameraComponents()
    {
        cameraComponent = this.gameObject.AddComponent<Camera>();
        this.gameObject.AddComponent<GUILayer>();
        this.gameObject.AddComponent<AudioListener>();
        this.gameObject.AddComponent<FlareLayer>();
    }

    private void initCameraController()
    {
        cameraController = new Camera_Controller_RTS(cameraComponent);
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void LateUpdate () {
        if(cameraController != null)
        {
            cameraController.UpdateCamera();
        }
    }
}
