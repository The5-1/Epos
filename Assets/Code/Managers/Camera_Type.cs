using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Camera_Type
{
    #region Fields

    //stored transformation
    [SerializeField] protected Vector3 storedPos;
    [SerializeField] protected Vector3 storedRot;

    public CursorLockMode defaultCursorLockMode = CursorLockMode.None;

    public float translateSpeed = 10;
    public float rotateSpeed = 90;
    public float climbSpeed = 4;

    public float slowMoveFactor = 0.25f;
    public float fastMoveFactor = 3.0f;

    public float fov = 60;

    #endregion

    public Camera_Type()
    {
        storedPos = new Vector3();
        storedRot = new Vector3();

        init();
    }

    protected abstract void init();

    protected abstract void calculateCameraTransform();


    /*
    public void UpdateCamera(Camera cam, float smoothing = 0.0f)
    {
        calculateCameraTransform();
        applyTransformToCamera(cam, smoothing);
    }
    */

    public void setControllerTransform(Vector3 pos, Vector3 rot)
    {
        storedPos = pos;
        storedRot = rot;
    }

    public void getControllerTransform(out Vector3 pos, out Vector3 rot)
    {
        pos = storedPos;
        rot = storedRot;
    }

    public void updateControllerTransform(out Vector3 pos, out Vector3 rot)
    {
        calculateCameraTransform();
        getControllerTransform(out pos,out rot);
    }

}



[System.Serializable]
public class Camera_Type_RTS : Camera_Type
{
    #region Fields

    //region-specific
    public Terrain myTerrain;
    public Vector4 movementBounds; //probably all our maps will be rectangular
    public Vector2 heightBounds; //max height and min distance to floor

    public Vector2 RotationBounds;
    public float scrollBorderWidth = 50.0f;
    public float scrollBorderPower = 11.0f;

    public float desiredHeight = 25.0f;
    public float defaultAngleY = 50.0f;


    /*
    [SerializeField] private Vector3 camPos;
    [SerializeField] private Vector3 camForward;
    [SerializeField] private Vector3 camRight;
    [SerializeField] private Vector3 camRot;
    [SerializeField] private Transform targetTransform;

    [SerializeField] private Vector3 camPosOld;
    [SerializeField] private Vector3 camRotOld;
    */

    #endregion

    protected override void init()
    {
        translateSpeed = 20;
        rotateSpeed = 150;
        climbSpeed = 10;

        slowMoveFactor = 0.25f;
        fastMoveFactor = 3.0f;

        RotationBounds = new Vector2(0.0f, 80.0f);
        getActiveRegionCameraBounds();

        defaultCursorLockMode = CursorLockMode.None;
        Cursor.lockState = defaultCursorLockMode;

        storedRot.x = defaultAngleY;
    }

    protected void getActiveRegionCameraBounds()
    {
        //TODO: pass a region to provide its RTS bounds
        movementBounds = new Vector4(-1000.0f, 1000.0f, -1000.0f, 1000.0f);
        heightBounds = new Vector2(2.0f, 80.0f);

        myTerrain = Terrain.activeTerrain;
    }

    protected override void calculateCameraTransform()
    {
        float movespeedfactor;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) { movespeedfactor = fastMoveFactor; }
        else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) { movespeedfactor = slowMoveFactor; }
        else { movespeedfactor = 1.0f; }

        float speedRot = rotateSpeed * Time.deltaTime;
        float speedXZ = translateSpeed * movespeedfactor * Time.deltaTime;
        float speedY = climbSpeed * movespeedfactor * Time.deltaTime;
   
        if (Input.GetKey("mouse 2"))
        {
            Cursor.lockState = CursorLockMode.Locked;

            storedRot.y += Input.GetAxis("Mouse X") * speedRot;
            storedRot.x -= Input.GetAxis("Mouse Y") * speedRot;
            storedRot.x = Mathf.Clamp(storedRot.x, RotationBounds.x, RotationBounds.y);
        }
        else
        {
            Cursor.lockState = defaultCursorLockMode;
        }

        float terrainHeight = myTerrain.SampleHeight(storedPos);
        desiredHeight = Mathf.Clamp(desiredHeight + Input.GetAxis("Mouse ScrollWheel") * speedY * 50.0f, 0.0f, heightBounds.y);

        float axisH = Input.GetAxis("Horizontal");
        if (axisH == 0)
        {
            axisH = Mathf.Clamp(Mathf.Pow((Input.mousePosition.x / Screen.width) * 2.0f - 1.0f, scrollBorderPower),-1.0f,1.0f);
            if (Input.mousePosition.x > scrollBorderWidth && Input.mousePosition.x < Screen.width - scrollBorderWidth) { axisH = 0; }
        }

        float axisV = Input.GetAxis("Vertical");
        if (axisV == 0)
        {
            axisV = Mathf.Clamp(Mathf.Pow((Input.mousePosition.y / Screen.height) * 2.0f - 1.0f, scrollBorderPower),-1.0f, 1.0f);
            if (Input.mousePosition.y > scrollBorderWidth && Input.mousePosition.y < Screen.height - scrollBorderWidth) { axisV = 0; }
        }

        storedPos += Quaternion.AngleAxis(storedRot.y, Vector3.up) * Vector3.forward * axisV * speedXZ;
        storedPos += Quaternion.AngleAxis(storedRot.y, Vector3.up) * Vector3.right * axisH * speedXZ;
        storedPos.y = Mathf.Clamp(desiredHeight+terrainHeight, heightBounds.x, heightBounds.y);
    }
}


[System.Serializable]
public class Camera_Type_Freecam : Camera_Type
{
    private float rotationX;
    private float rotationY;

    protected override void init()
    {
        translateSpeed = 20;
        rotateSpeed = 150;
        climbSpeed = 10;

        slowMoveFactor = 0.25f;
        fastMoveFactor = 3.0f;

        defaultCursorLockMode = CursorLockMode.None;
        Cursor.lockState = defaultCursorLockMode;
    }

    protected override void calculateCameraTransform()
    {
        if (Input.GetKey("mouse 0") || Input.GetKey("mouse 1"))
        {
            Cursor.lockState = CursorLockMode.Locked;

            float movespeedfactor;
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) { movespeedfactor = fastMoveFactor; }
            else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) { movespeedfactor = slowMoveFactor; }
            else { movespeedfactor = 1.0f; }

            float speedRot = rotateSpeed * Time.deltaTime;
            float speedXZ = translateSpeed * movespeedfactor * Time.deltaTime;
            float speedY = climbSpeed * movespeedfactor * Time.deltaTime;

            storedRot.y += Input.GetAxis("Mouse X") * speedRot;
            storedRot.x -= Input.GetAxis("Mouse Y") * speedRot;
            storedRot.x = Mathf.Clamp(storedRot.x, -90, 90);

            //quaternion*vector = rotated vector (not commutative!)
            storedPos += Quaternion.Euler(storedRot) * Vector3.forward * Input.GetAxis("Vertical") * speedXZ;
            storedPos += Quaternion.Euler(storedRot) * Vector3.right * Input.GetAxis("Horizontal") * speedXZ;

            if (Input.GetKey(KeyCode.E)) { storedPos += Quaternion.Euler(storedRot)*Vector3.up * speedY; }
            if (Input.GetKey(KeyCode.Q)) { storedPos -= Quaternion.Euler(storedRot)*Vector3.up * speedY; }
        }
        else
        {
            Cursor.lockState = defaultCursorLockMode;
        }
    }


}
 