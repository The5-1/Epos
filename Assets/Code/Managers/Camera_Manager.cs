using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Manager : MonoBehaviour {


    static public Camera_Manager singleton;

    public Camera mainCameraComponent;

    private Vector3 camTypePos;
    private Vector3 camTypeRot;
    public float camTypeSmoothing = 2.0f;
    private bool camSwitchAnimRunning = false;


    public Camera_Type_RTS cam_RTS;
    public Camera_Type_Freecam cam_Free;

    public ushort activeCameraType = 0;


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
        Debug.Log(string.Format("{0}.init()", this));
        initCameraComponents();
        disableAllButMainCamera();
        initCameraController();
    }

    private void initCameraComponents()
    {
        mainCameraComponent = this.gameObject.AddComponent<Camera>();
        mainCameraComponent.tag = "MainCamera";
        this.gameObject.AddComponent<GUILayer>();
        this.gameObject.AddComponent<AudioListener>();
        this.gameObject.AddComponent<FlareLayer>();
    }

    private void initCameraController()
    {
        cam_Free = new Camera_Type_Freecam();
        cam_RTS = new Camera_Type_RTS();
    }

    private void disableAllButMainCamera()
    {
        Camera[] cams = FindObjectsOfType<Camera>();
        foreach (Camera cam in cams)
        {
            if (cam == mainCameraComponent) continue;
            cam.gameObject.SetActive(false);
        }
    }


    //Late update updates after everything else was moved, perfect for cameras
    void LateUpdate () {

        if (Input.GetKeyDown("c"))
        {
            changeCamera((ushort)((activeCameraType + 1) % 2));
        }

        switch (activeCameraType)
        {
            case 0:
                //cam_Free.UpdateCamera(cameraComponent, smoothing);
                cam_Free.updateControllerTransform(out camTypePos, out camTypeRot);
                break;
            case 1:
                //cam_RTS.UpdateCamera(cameraComponent, smoothing);
                cam_RTS.updateControllerTransform(out camTypePos, out camTypeRot);
                break;

            default: return;
        }
        applyTransformToCamera(camTypePos, camTypeRot, camTypeSmoothing);

    }

    protected void applyTransformToCamera(Vector3 pos, Vector3 rot, float smooth)
    {
        if (smooth <= 1.0f)
        {
            mainCameraComponent.transform.position = pos;
            mainCameraComponent.transform.rotation = Quaternion.Euler(rot);
        }
        else
        {
            mainCameraComponent.transform.position = Vector3.Lerp(mainCameraComponent.transform.position, pos, 25.0f / (smooth + 1.0f) * Time.deltaTime);
            mainCameraComponent.transform.rotation = Quaternion.Slerp(mainCameraComponent.transform.rotation, Quaternion.Euler(rot), 25.0f / (smooth + 1.0f) * Time.deltaTime);
        }

    }

    public void changeCamera(ushort index)
    {
        activeCameraType = index;    
    }

}
