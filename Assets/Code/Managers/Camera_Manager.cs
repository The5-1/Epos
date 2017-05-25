using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Manager : MonoBehaviour {


    static public Camera_Manager singleton;

    public Camera cameraComponent;
    
    public Camera_Type_RTS cam_RTS;
    public Camera_Type_Freecam cam_Free;

    public ushort activeCameraType = 0;

    public float smoothing = 2.0f;

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
        cam_Free = new Camera_Type_Freecam();
        cam_RTS = new Camera_Type_RTS();
    }


    //Late update updates after everything else was moved, perfect for cameras
	void LateUpdate () {

        if (Input.GetKeyDown("c"))
        { changeCamera((ushort)((activeCameraType + 1) % 2));}

        switch(activeCameraType)
        {
            case 0:
                cam_Free.UpdateCamera(cameraComponent, smoothing);
                break;
            case 1:
                cam_RTS.UpdateCamera(cameraComponent, smoothing);
                break;

            default: return;
        }
    }

    public void changeCamera(ushort index)
    {
        activeCameraType = index;

        /*
        switch (activeController)
        {
            case 0:

                cam_RTS.leaveCamera();
                cam_Free.enterCamera();
                break;
            case 1:
                cam_Free.leaveCamera();
                cam_RTS.enterCamera();
                break;

            default: return;
        }
        */
    }

}
