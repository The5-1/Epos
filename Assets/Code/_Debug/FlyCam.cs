using UnityEngine;
using System.Collections;

public class FlyCam : MonoBehaviour
{

    /*
	EXTENDED FLYCAM
		Desi Quintans (CowfaceGames.com), 17 August 2012.
		Based on FlyThrough.js by Slin (http://wiki.unity3d.com/index.php/FlyThrough), 17 May 2011.
	*/

    public float baseMoveSpeed = 10;
    public float baseClimbSpeed = 4;

    public float cameraSensitivity = 90;

    public float slowMoveFactor = 0.25f;
    public float fastMoveFactor = 3;

    public float rotationX = 0.0f;
    public float rotationY = 0.0f;

    private float aspect;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        aspect = Screen.width / Screen.height;

        rotationX = transform.localRotation.eulerAngles.y;
        rotationY = -transform.localRotation.eulerAngles.x;
    }

    void Update()
    {
        fub01();
    }

    protected void fub01()
    {
        if (Input.GetKey("mouse 0") || Input.GetKey("mouse 1"))
        {
            Cursor.lockState = CursorLockMode.Locked;

            float movespeedfactor;

            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) { movespeedfactor = fastMoveFactor; }
            else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) { movespeedfactor = slowMoveFactor; }
            else { movespeedfactor = 1.0f; }

            rotationX += Input.GetAxis("Mouse X") * cameraSensitivity * Time.deltaTime;
            rotationY += Input.GetAxis("Mouse Y") * cameraSensitivity * Time.deltaTime;
            rotationY = Mathf.Clamp(rotationY, -90, 90);

            transform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
            transform.localRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);

            transform.position += transform.forward * baseMoveSpeed * movespeedfactor * Input.GetAxis("Vertical") * Time.deltaTime;
            transform.position += transform.right * baseMoveSpeed * movespeedfactor * Input.GetAxis("Horizontal") * Time.deltaTime;


            if (Input.GetKey(KeyCode.E)) { transform.position += transform.up * baseClimbSpeed * movespeedfactor * Time.deltaTime; }
            if (Input.GetKey(KeyCode.Q)) { transform.position -= transform.up * baseClimbSpeed * movespeedfactor * Time.deltaTime; }
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    protected void vanilla()
    {
        float movespeedfactor;

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) { movespeedfactor = fastMoveFactor; }
        else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) { movespeedfactor = slowMoveFactor; }
        else { movespeedfactor = 1.0f; }

        rotationX += Input.GetAxis("Mouse X") * cameraSensitivity * Time.deltaTime;
        rotationY += Input.GetAxis("Mouse Y") * cameraSensitivity * Time.deltaTime;
        rotationY = Mathf.Clamp(rotationY, -90, 90);

        transform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
        transform.localRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);

        transform.position += transform.forward * baseMoveSpeed * movespeedfactor * Input.GetAxis("Vertical") * Time.deltaTime;
        transform.position += transform.right * baseMoveSpeed * movespeedfactor * Input.GetAxis("Horizontal") * Time.deltaTime;


        if (Input.GetKey(KeyCode.E)) { transform.position += transform.up * baseClimbSpeed * movespeedfactor * Time.deltaTime; }
        if (Input.GetKey(KeyCode.Q)) { transform.position -= transform.up * baseClimbSpeed * movespeedfactor * Time.deltaTime; }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Cursor.lockState = (Cursor.lockState == CursorLockMode.Locked) ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }

}