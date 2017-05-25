using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Manager : MonoBehaviour {


    static public Camera_Manager singleton;

    public Camera cameraComponent;
    
    public Camera_Controller_RTS cam_RTS;
    public Camera_Controller_Freecam cam_Free;

    public ushort activeController = 0;

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
        cam_Free = new Camera_Controller_Freecam(cameraComponent);
        cam_RTS = new Camera_Controller_RTS(cameraComponent);
    }


    //Late update updates after everything else was moved, perfect for cameras
	void LateUpdate () {

        if (Input.GetKeyDown("c"))
        { changeCamera((ushort)((activeController + 1) % 2));}

        switch(activeController)
        {
            case 0:
                cam_Free.UpdateCamera();
                break;
            case 1:
                cam_RTS.UpdateCamera();
                break;

            default: return;
        }
    }

    public void changeCamera(ushort index)
    {
        activeController = index;

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
    }

}
