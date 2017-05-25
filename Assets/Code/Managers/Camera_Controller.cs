using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Camera_Controller
{
    #region Fields
    public Camera myCamera;

    public float translateSpeed = 10;
    public float rotateSpeed = 90;
    public float climbSpeed = 4;

    public float slowMoveFactor = 0.25f;
    public float fastMoveFactor = 3.0f;

    public float FoV = 60;

    #endregion

    public Camera_Controller(Camera cam)
    {
        myCamera = cam;
    }

    public abstract void init();
    public abstract void UpdateCamera();

}

[System.Serializable]
public class Camera_Controller_RTS : Camera_Controller
{
    #region Fields

    //region-specific
    public Terrain myTerrain;
    public Vector4 movementBounds; //probably all our maps will be rectangular
    public Vector2 heightBounds; //max height and min distance to floor

    public Vector2 RotationBounds;
    public float scrollBorderWidth = 50.0f;
    public float scrollBorderPower = 11.0f;

    [SerializeField] private Vector3 camPos;
    [SerializeField] private Vector3 camForward;
    [SerializeField] private Vector3 camRight;
    [SerializeField] private Vector3 camRot;
    [SerializeField] private Transform targetTransform;
    [SerializeField] private float camHeight;

    #endregion

    public Camera_Controller_RTS(Camera cam):base(cam) 
    {
        //TODO: needs a region as input to grab allowed camera bounds
        init();
    }

    public override void init()
    {
        translateSpeed = 20;
        rotateSpeed = 150;
        climbSpeed = 10;

        slowMoveFactor = 0.25f;
        fastMoveFactor = 3.0f;

        camPos = new Vector3();

        camRot = new Vector3();

        RotationBounds = new Vector2(0.0f, 80.0f);

        camHeight = myCamera.transform.position.y;
        getActiveRegionCameraBounds();
    }

    private void getActiveRegionCameraBounds()
    {
        //TODO: pass a region
        movementBounds = new Vector4(-1000.0f, 1000.0f, -1000.0f, 1000.0f);
        heightBounds = new Vector2(2.0f, 80.0f);

        myTerrain = Terrain.activeTerrain;
    }

    public override void UpdateCamera()
    {
        RTS();
    }

    private void RTS()
    {
        //rotation
        float rotationX = 0.0f;
        float rotationY = 0.0f;
        float speedRot = rotateSpeed * Time.deltaTime;

        if (Input.GetKey("mouse 2"))
        {
            Cursor.lockState = CursorLockMode.Locked;

            rotationX = Input.GetAxis("Mouse X") * speedRot;
            rotationY = Input.GetAxis("Mouse Y") * speedRot;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }

        //camera Speed
        float movespeedfactor;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) { movespeedfactor = fastMoveFactor; }
        else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) { movespeedfactor = slowMoveFactor; }
        else { movespeedfactor = 1.0f; }

        //height Y
        float speedY = climbSpeed * movespeedfactor * Time.deltaTime * 100.0f;
        float terrainHeight = myTerrain.SampleHeight(camPos);
        camHeight = Mathf.Clamp(camHeight + Input.GetAxis("Mouse ScrollWheel") * speedY, 0.0f, heightBounds.y);
        
        //paning XZ
        float speedXZ = translateSpeed * movespeedfactor * Time.deltaTime;

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


        //camPos.x = Mathf.Clamp(myCamera.transform.position.x + axisH * speedXZ, movementBounds.x, movementBounds.y); 
        //camPos.z = Mathf.Clamp(myCamera.transform.position.z + axisV * speedXZ, movementBounds.z, movementBounds.w);

        camForward = myCamera.transform.forward;
        camForward.y = 0.0f;
        camForward = camForward.normalized;

        camRight = myCamera.transform.right;
        camRight.y = 0.0f;
        camRight = camRight.normalized;

        camPos = camForward * axisV * speedXZ;
        camPos += camRight * axisH * speedXZ;
        camPos += myCamera.transform.position;
        camPos.y = Mathf.Clamp(camHeight, terrainHeight + heightBounds.x, heightBounds.y); //chage to camHeight +terrainHeight to keep constant distance to terrain


        camRot.y = camRot.y + rotationX; //horizontal
        camRot.x = Mathf.Clamp(camRot.x - rotationY, RotationBounds.x, RotationBounds.y); //vertical

        //Lerp to resulting position (this seems to work because delta time is in the calculations themselves... strange stuff
        myCamera.transform.position = Vector3.Lerp(myCamera.transform.position, camPos, 0.25f);
        myCamera.transform.rotation = Quaternion.Lerp(myCamera.transform.rotation, Quaternion.Euler(camRot), 0.25f);

    }


    /***************************************
    // Camera Boundraries
    public float xMax = 1000;
    public float xMin = -1000;
    public float zMax = 1000;
    public float zMin = -1000;
    //Height Boundraries
    public float yMax = 80; //max height
    public float yMin = 2; //min distance to terrain
    //Rotation
    public float VerticalRotationMin = 0;
    public float VerticalRotationMax = 80;
    //scrolling
    public float scrollZone = 35; //border arround the screen in pixels where scrolling starts
    public float scrollspeed = 30;
    public float rotationspeed=10;

    [SerializeField] private Vector3 desiredPosition;
    [SerializeField] private Vector3 curentRotation;
    [SerializeField] private Vector3 movement;

    [SerializeField] private Vector2 currentMousePos;
    [SerializeField] private Vector2 previousMousePos;
    [SerializeField] private Vector2 mousePosDelta;



    #endregion


    public Camera_Controller(Camera cam)
    {
        myCamera = cam;
        desiredPosition = myCamera.transform.position;
        movement = new Vector3();
        currentMousePos = new Vector2();
        currentMousePos = Input.mousePosition;
        previousMousePos = new Vector2();
        previousMousePos = Input.mousePosition;
        mousePosDelta = currentMousePos- previousMousePos;
    }

    public void getTerrain()
    {
        //TODO: add a method to find the current regions terrain, once we got our own terrain system
    }


    public void Update()
    {
        currentMousePos = Input.mousePosition;
        mousePosDelta = currentMousePos - previousMousePos;

        movement = Vector3.zero;
        float speed = scrollspeed * Time.deltaTime;
        curentRotation = myCamera.transform.eulerAngles;
        // Define Screen Area at wich velocity applies
        // 0.5f zeigt das es ein float ist , Unity kann mit double nichts anfangen
        if (!Input.GetMouseButton(1))
        {
            // Horizontal Movement
            if (currentMousePos.x < scrollZone || Input.GetKey("left") || Input.GetKey("a"))
            {
                movement.x -= speed * Mathf.Sin(curentRotation.y * Mathf.Deg2Rad + Mathf.PI * 0.5f);
                movement.z -= speed * Mathf.Cos(curentRotation.y * Mathf.Deg2Rad + Mathf.PI * 0.5f);
            }
            else if (currentMousePos.x > Screen.width - scrollZone || Input.GetKey("right") || Input.GetKey("d"))
            {
                movement.x += speed * Mathf.Sin(curentRotation.y * Mathf.Deg2Rad + Mathf.PI * 0.5f);
                movement.z += speed * Mathf.Cos(curentRotation.y * Mathf.Deg2Rad + Mathf.PI * 0.5f);
            }
            //Vertical Movement
            if (currentMousePos.y < scrollZone || Input.GetKey("down") || Input.GetKey("s"))
            {
                movement.x -= speed * Mathf.Sin(curentRotation.y * Mathf.Deg2Rad);
                movement.z -= speed * Mathf.Cos(curentRotation.y * Mathf.Deg2Rad);
            }
            else if (currentMousePos.y > Screen.height - scrollZone || Input.GetKey("up") || Input.GetKey("w"))
            {
                movement.x += speed * Mathf.Sin(curentRotation.y * Mathf.Deg2Rad);
                movement.z += speed * Mathf.Cos(curentRotation.y * Mathf.Deg2Rad);
            }
        }

        movement.y = MovementHeight(movement.y);

        //Bestimme yMin durch Ray nach unten
        float miny = minHeight();

        //Restrict Camera to Boundraries
        movement.x = Mathf.Clamp(movement.x + desiredPosition.x, xMin, xMax);
        movement.z = Mathf.Clamp(movement.z + desiredPosition.z, zMin, zMax);                    
        movement.y = Mathf.Clamp(movement.y + desiredPosition.y, miny, yMax);

        // Smooth Movement
        desiredPosition = movement;
        myCamera.transform.position = Vector3.Lerp(myCamera.transform.position, desiredPosition, 0.2f);
        //myCamera.transform.position = desiredPosition;

        RotateCameraHorizontal();
        RotateCameraVertical();

        previousMousePos = Input.mousePosition;
    }

    private float MovementHeight(float y)
    {
        y -= scrollspeed * Input.GetAxis("Mouse ScrollWheel");
        return y;
    }

    private float minHeight()
    {
        float ymin = Terrain.activeTerrain.SampleHeight(myCamera.transform.position) + yMin;
        return ymin;
    }

    void RotateCameraHorizontal()
    {
        if (Input.GetMouseButton(1) && mousePosDelta.x != 0)
        {
            myCamera.transform.Rotate(0, rotationspeed * Time.deltaTime * mousePosDelta.x, 0, Space.World);
        }
    }

    void RotateCameraVertical()
    {
        if (Input.GetMouseButton(1) && mousePosDelta.y != 0)
        {
            //myCamera.transform.Rotate(Mathf.Clamp(mousePosDelta.y * rotationspeed * Time.deltaTime, VerticalRotationMin, VerticalRotationMax), 0, 0);

            var cameraRotationX = mousePosDelta.y * rotationspeed * Time.deltaTime;
            var desiredRotationX = myCamera.transform.eulerAngles.x + cameraRotationX;
            if (desiredRotationX >= VerticalRotationMin && desiredRotationX <= VerticalRotationMax)
            {
                myCamera.transform.Rotate(cameraRotationX, 0, 0);
            }
        }
    }

    **************************/
}


public class Camera_Controller_Freecam : Camera_Controller
{


    private float rotationX;
    private float rotationY;

    public Camera_Controller_Freecam(Camera cam):base(cam) 
    {
        init();
    }


    public override void init()
    {
        translateSpeed = 20;
        rotateSpeed = 150;
        climbSpeed = 10;

        slowMoveFactor = 0.25f;
        fastMoveFactor = 3.0f;
    }


    public override void UpdateCamera()
    {
        freecam();
    }

    protected void freecam()
    {
        if (Input.GetKey("mouse 0") || Input.GetKey("mouse 1"))
        {
            Cursor.lockState = CursorLockMode.Locked;

            float movespeedfactor;

            //camera Speed
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) { movespeedfactor = fastMoveFactor; }
            else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) { movespeedfactor = slowMoveFactor; }
            else { movespeedfactor = 1.0f; }


            rotationX += Input.GetAxis("Mouse X") * rotateSpeed * Time.deltaTime;
            rotationY += Input.GetAxis("Mouse Y") * rotateSpeed * Time.deltaTime;
            rotationY = Mathf.Clamp(rotationY, -90, 90);

            myCamera.transform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
            myCamera.transform.localRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);

            myCamera.transform.position += myCamera.transform.forward * translateSpeed * movespeedfactor * Input.GetAxis("Vertical") * Time.deltaTime;
            myCamera.transform.position += myCamera.transform.right * translateSpeed * movespeedfactor * Input.GetAxis("Horizontal") * Time.deltaTime;


            if (Input.GetKey(KeyCode.E)) { myCamera.transform.position += myCamera.transform.up * climbSpeed * movespeedfactor * Time.deltaTime; }
            if (Input.GetKey(KeyCode.Q)) { myCamera.transform.position -= myCamera.transform.up * climbSpeed * movespeedfactor * Time.deltaTime; }
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }


}