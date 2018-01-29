using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace The5_Epos02
{
    /* These camera Types just calculate/update their destination transformations
     * The Camera system gets these and then interpolates between them smoothly
     */


    [System.Serializable]
    public abstract class CameraType
    {
        //stored transformation
        public Vector3 destinationPos;
        public Vector3 destinationRot;

        public CursorLockMode defaultCursorLockMode = CursorLockMode.None;

        public float smoothing = 2.0f;

        public float fov = 60;
        public float nearplane = 0.3f;
        public float farplane = 1000.0f;    

        public CameraType()
        {
            destinationPos = new Vector3();
            destinationRot = new Vector3();

            init();
        }

        protected abstract void init();

        protected abstract void calculateDestination();

        public void setActive(Camera cam)
        {
            updateCameraComponentParameters(cam);
        }

        public void updateCameraComponentParameters(Camera cam)
        {
            cam.fieldOfView = this.fov;
            cam.nearClipPlane = this.nearplane;
            cam.farClipPlane = this.farplane;
        }

        protected void overrideDestination(Vector3 pos, Vector3 rot)
        {
            destinationPos = pos;
            destinationRot = rot;
        }

        protected void getDestination(out Vector3 pos, out Vector3 rot)
        {
            pos = destinationPos;
            rot = destinationRot;
        }

        public void getUpdatedDestination(out Vector3 pos, out Vector3 rot)
        {
            calculateDestination();
            getDestination(out pos, out rot);
        }

    }

    [System.Serializable]
    public class CameraType_RTS : CameraType
    {
        #region Fields

        public float translateSpeed = 20;
        public float rotateSpeed = 150;
        public float climbSpeed = 10;
        public float slowMoveFactor = 0.25f;
        public float fastMoveFactor = 3.0f;


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
            RotationBounds = new Vector2(0.0f, 80.0f);

            getActiveRegionCameraBounds();

            defaultCursorLockMode = CursorLockMode.None;
            Cursor.lockState = defaultCursorLockMode;

            destinationRot.x = defaultAngleY;
        }

        protected void getActiveRegionCameraBounds()
        {
            //TODO: pass a region to provide its RTS bounds
            movementBounds = new Vector4(-1000.0f, 1000.0f, -1000.0f, 1000.0f);
            heightBounds = new Vector2(2.0f, 80.0f);

            myTerrain = Terrain.activeTerrain;
        }

        protected override void calculateDestination()
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

                destinationRot.y += Input.GetAxis("Mouse X") * speedRot;
                destinationRot.x -= Input.GetAxis("Mouse Y") * speedRot;
                destinationRot.x = Mathf.Clamp(destinationRot.x, RotationBounds.x, RotationBounds.y);
            }
            else
            {
                Cursor.lockState = defaultCursorLockMode;
            }

            float terrainHeight = myTerrain.SampleHeight(destinationPos);
            desiredHeight = Mathf.Clamp(desiredHeight + Input.GetAxis("Mouse ScrollWheel") * speedY * 50.0f, 0.0f, heightBounds.y);

            float axisH = Input.GetAxis("Horizontal");
            if (axisH == 0)
            {
                axisH = Mathf.Clamp(Mathf.Pow((Input.mousePosition.x / Screen.width) * 2.0f - 1.0f, scrollBorderPower), -1.0f, 1.0f);
                if (Input.mousePosition.x > scrollBorderWidth && Input.mousePosition.x < Screen.width - scrollBorderWidth) { axisH = 0; }
            }

            float axisV = Input.GetAxis("Vertical");
            if (axisV == 0)
            {
                axisV = Mathf.Clamp(Mathf.Pow((Input.mousePosition.y / Screen.height) * 2.0f - 1.0f, scrollBorderPower), -1.0f, 1.0f);
                if (Input.mousePosition.y > scrollBorderWidth && Input.mousePosition.y < Screen.height - scrollBorderWidth) { axisV = 0; }
            }

            destinationPos += Quaternion.AngleAxis(destinationRot.y, Vector3.up) * Vector3.forward * axisV * speedXZ;
            destinationPos += Quaternion.AngleAxis(destinationRot.y, Vector3.up) * Vector3.right * axisH * speedXZ;
            destinationPos.y = Mathf.Clamp(desiredHeight + terrainHeight, heightBounds.x, heightBounds.y);
        }
    }


    [System.Serializable]
    public class CameraType_Freecam : CameraType
    {
        public float rotationX;
        public float rotationY;

        public float translateSpeed = 20;
        public float rotateSpeed = 150;
        public float climbSpeed = 10;
        public float slowMoveFactor = 0.25f;
        public float fastMoveFactor = 3.0f;

        protected override void init()
        {
            defaultCursorLockMode = CursorLockMode.None;
            Cursor.lockState = defaultCursorLockMode;
        }

        protected override void calculateDestination()
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

                destinationRot.y += Input.GetAxis("Mouse X") * speedRot;
                destinationRot.x -= Input.GetAxis("Mouse Y") * speedRot;
                destinationRot.x = Mathf.Clamp(destinationRot.x, -90, 90);

                //quaternion*vector = rotated vector (not commutative!)
                destinationPos += Quaternion.Euler(destinationRot) * Vector3.forward * Input.GetAxis("Vertical") * speedXZ;
                destinationPos += Quaternion.Euler(destinationRot) * Vector3.right * Input.GetAxis("Horizontal") * speedXZ;

                if (Input.GetKey(KeyCode.E)) { destinationPos += Quaternion.Euler(destinationRot) * Vector3.up * speedY; }
                if (Input.GetKey(KeyCode.Q)) { destinationPos -= Quaternion.Euler(destinationRot) * Vector3.up * speedY; }
            }
            else
            {
                Cursor.lockState = defaultCursorLockMode;
            }
        }


    }


    [System.Serializable]
    public class CameraType_ThirdPerson : CameraType
    {
        public GameObject targetGO;

        public float distance = 4.0f;
        public float heightOffset = 2.0f;
        public float shoulderOffset = 0.0f;

        protected override void init()
        {
            this.setTarget(EposGame.playerActor.gameObject);

            smoothing = 0.5f;

            defaultCursorLockMode = CursorLockMode.None;
            Cursor.lockState = defaultCursorLockMode;
        }

        protected override void calculateDestination()
        {
            destinationPos = targetGO.transform.position - targetGO.transform.forward * distance + targetGO.transform.up * heightOffset;

            destinationRot = targetGO.transform.eulerAngles;

            
            /*
            float speedRot = rotateSpeed * Time.deltaTime;
            if (Input.GetKey("mouse 0") || Input.GetKey("mouse 1"))
            {
                storedRot.y += Input.GetAxis("Mouse X") * speedRot;
                storedRot.x -= Input.GetAxis("Mouse Y") * speedRot;
                storedRot.x = Mathf.Clamp(storedRot.x, -90, 90);
            }
            */
        }

        public void setTarget(GameObject target)
        {
            targetGO = target;
        }

    }
}