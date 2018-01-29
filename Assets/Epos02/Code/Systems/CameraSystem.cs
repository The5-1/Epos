using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace The5_Epos02
{

    public enum ActiveCameraType { FreeCam, RTSCam, ThirdPersonCam };


    [RequireComponent(typeof(Camera))]
    public class CameraSystem : MonoBehaviour
    {
        
        public bool inputEnabled = true;
        public static Camera activeCamera;

        public ActiveCameraType activeCameraType;
        public CameraType_RTS camtype_RTS;
        public CameraType_Freecam camtype_Free;
        public CameraType_ThirdPerson camtype_TP;

        private Vector3 destinationPos;
        private Vector3 destinationRot;
        private float activeTypeSmoothing;

        private bool camSwitchAnimRunning = false;

        private void Awake()
        {
            init();
        }

        private void init()
        {
            Debug.Log(string.Format("{0}.init()", this));
            initCameraComponents();
            initCameraTypes();
            disableOtherCameras();
            changeCameraType(ActiveCameraType.ThirdPersonCam);
        }

        private void initCameraComponents()
        {
            CameraSystem.activeCamera = this.GetComponent<Camera>();
            CameraSystem.activeCamera.tag = "MainCamera";
            //this.gameObject.AddComponent<GUILayer>();
            //this.gameObject.AddComponent<AudioListener>();
            //this.gameObject.AddComponent<FlareLayer>();
        }

        private void initCameraTypes()
        {
            camtype_Free = new CameraType_Freecam();
            camtype_RTS = new CameraType_RTS();
            camtype_TP = new CameraType_ThirdPerson();
        }

        private void disableOtherCameras()
        {
            Camera[] cams = FindObjectsOfType<Camera>();
            foreach (Camera cam in cams)
            {
                if (cam == CameraSystem.activeCamera) continue;
                cam.gameObject.SetActive(false);
            }
        }

        public void changeCameraType(ActiveCameraType type)
        {
            activeCameraType = type;
            switch (activeCameraType)
            {
                case ActiveCameraType.FreeCam:
                    camtype_Free.setActive(CameraSystem.activeCamera);
                    break;
                case ActiveCameraType.RTSCam:
                    camtype_RTS.setActive(CameraSystem.activeCamera);
                    break;
                case ActiveCameraType.ThirdPersonCam:
                    camtype_TP.setActive(CameraSystem.activeCamera);
                    break;

                default: return;
            }
        }

        protected void updateCameraTypeDestination()
        {
            switch (activeCameraType)
            {
                case ActiveCameraType.FreeCam:
                    activeTypeSmoothing = camtype_Free.smoothing;
                    camtype_Free.getUpdatedDestination(out destinationPos, out destinationRot);
                    break;
                case ActiveCameraType.RTSCam:
                    activeTypeSmoothing = camtype_RTS.smoothing;
                    camtype_RTS.getUpdatedDestination(out destinationPos, out destinationRot);
                    break;
                case ActiveCameraType.ThirdPersonCam:
                    activeTypeSmoothing = camtype_TP.smoothing;
                    camtype_TP.getUpdatedDestination(out destinationPos, out destinationRot);
                    break;

                default: return;
            }
        }

        protected void applySmoothedCameraTransition()
        {
            if (activeTypeSmoothing <= 0.0f)
            {
                activeCamera.transform.position = destinationPos;
                activeCamera.transform.rotation = Quaternion.Euler(destinationRot);
            }
            else
            {
                activeCamera.transform.position = Vector3.Lerp(activeCamera.transform.position, destinationPos, 25.0f / (activeTypeSmoothing + 1.0f) * Time.deltaTime);
                activeCamera.transform.rotation = Quaternion.Slerp(activeCamera.transform.rotation, Quaternion.Euler(destinationRot), 25.0f / (activeTypeSmoothing + 1.0f) * Time.deltaTime);
            }

        }



        //Late update updates after everything else was moved, perfect for cameras
        void LateUpdate()
        {

            if (Input.GetKeyDown("c"))
            {
                changeCameraType((ActiveCameraType)(((int)activeCameraType + 1) % 3));
            }

            if (inputEnabled)
            {
                updateCameraTypeDestination();
                applySmoothedCameraTransition();
            }

        }



    }

}