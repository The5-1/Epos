using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActiveCameraType {Free, RTS, TP};


public class Camera_Manager : MonoBehaviour {


    static public Camera_Manager singleton;

    public Camera mainCamera;
    public Camera activeCamera;

    private Vector3 camTypePos;
    private Vector3 camTypeRot;
    public float camTypeSmoothing = 2.0f;

    private bool camSwitchAnimRunning = false;


    public CameraType_RTS cam_RTS;
    public CameraType_Freecam cam_Free;
    public CameraType_TP cam_TP;


    public ActiveCameraType activeCameraType = ActiveCameraType.TP;


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
        mainCamera = this.gameObject.AddComponent<Camera>();
        mainCamera.tag = "MainCamera";
        this.gameObject.AddComponent<GUILayer>();
        this.gameObject.AddComponent<AudioListener>();
        this.gameObject.AddComponent<FlareLayer>();
    }

    private void initCameraController()
    {
        cam_Free = new CameraType_Freecam();
        cam_RTS = new CameraType_RTS();
        cam_TP = new CameraType_TP();
        cam_TP.setTarget(Player_Manager.singleton.mainPlayer.GO);
        activeCamera = mainCamera;
    }

    private void disableAllButMainCamera()
    {
        Camera[] cams = FindObjectsOfType<Camera>();
        foreach (Camera cam in cams)
        {
            if (cam == mainCamera) continue;
            cam.gameObject.SetActive(false);
        }
    }


    //Late update updates after everything else was moved, perfect for cameras
    void LateUpdate () {

        if (Input.GetKeyDown("c"))
        {
            changeCamera((ActiveCameraType)(((int)activeCameraType + 1) % 3));
        }

        switch (activeCameraType)
        {
            case ActiveCameraType.Free:
                //cam_Free.UpdateCamera(cameraComponent, smoothing);
                cam_Free.updateControllerTransform(out camTypePos, out camTypeRot);
                camTypeSmoothing = cam_Free.smoothing;
                break;
            case ActiveCameraType.RTS:
                //cam_RTS.UpdateCamera(cameraComponent, smoothing);
                cam_RTS.updateControllerTransform(out camTypePos, out camTypeRot);
                camTypeSmoothing = cam_RTS.smoothing;
                break;
            case ActiveCameraType.TP:
                //cam_RTS.UpdateCamera(cameraComponent, smoothing);
                cam_TP.updateControllerTransform(out camTypePos, out camTypeRot);
                camTypeSmoothing = cam_TP.smoothing;
                break;

            default: return;
        }
        applyTransformToCamera(camTypePos, camTypeRot, camTypeSmoothing);

    }

    protected void applyTransformToCamera(Vector3 pos, Vector3 rot, float smooth)
    {
        if (smooth == 0.0f)
        {
            mainCamera.transform.position = pos;
            mainCamera.transform.rotation = Quaternion.Euler(rot);
        }
        else
        {
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, pos, 25.0f / (smooth + 1.0f) * Time.deltaTime);
            mainCamera.transform.rotation = Quaternion.Slerp(mainCamera.transform.rotation, Quaternion.Euler(rot), 25.0f / (smooth + 1.0f) * Time.deltaTime);
        }

    }

    public void changeCamera(ActiveCameraType index)
    {
        activeCameraType = index;
    }

}
