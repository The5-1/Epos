using UnityEngine;
using System.Collections;

/// <summary>
/// has a camera object
/// has a actor to track
/// Toggle between Camera states: following, targeting
/// </summary>
[ExecuteInEditMode]
public class CameraController : MonoBehaviour {

    public Actor _actor;
    private Camera _camera;
    private Transform _cameraTransform;
    private Transform _transform; //cache reference to Transform!

    private Vector3 _focusPoint;
    private bool _shakeToggle = false;

    public float _heigtOffset = 1.0f;
    public float _OrthoZoom = 10.0f;
    public float _distance = 200.0f;
    public float _smoothSpeed = 5.0f;

    //note:
    // transform.up = local up
    // vectord3.up = global up


    void Awake()
    {
        if (_actor == null) _actor = this.GetComponent<Actor>();
        _transform = _actor.transform;
        _camera = _actor._camera;
        _cameraTransform = _camera.transform;

    }

    void Start()
    {
        MakeIsometric();
        PanToTarget(_transform.position + Vector3.up * _heigtOffset);
        ZoomOrtho(_OrthoZoom);
    }

    void FixedUpdate()
    {

        if (_actor._focusedObject != null && _actor._weaponDrawn)
        {
            //follow focus
            _focusPoint = _actor._focusedObject.transform.position - _transform.position;
            Follow(_focusPoint + Vector3.up * _heigtOffset);
        }
        else
        {
            //follow actor
            Follow(_transform.position + Vector3.up * _heigtOffset);
        }

        //if shake
        //trigger coroutine shake
    }

    void Follow(Vector3 targetPoint)
    {
        Vector3 targetCamPos = targetPoint + _camera.transform.forward * -1.0f * _distance;
        //LERP = linear interpolation
        _camera.transform.position = Vector3.Lerp(_camera.transform.position, targetCamPos, _smoothSpeed * Time.deltaTime);
    }


    public void PanToTarget(Vector3 targetPoint)
    {
        //set the cameras position (at it's current agle) exatly to the targets position 
        // and then moves away from it by the set distance
        _camera.transform.position = targetPoint + _camera.transform.forward * -1.0f * _distance;
    }

    public void ZoomOrtho(float zoom)
    {
        _camera.orthographicSize = zoom;
    }

    public void MakeIsometric()
    {
        _camera.transform.position = new Vector3(-10.0f, 15.0f, -10.0f);
        _camera.transform.localEulerAngles = new Vector3(30.0f, 45.0f, 0.0f);
        _camera.transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);
        _camera.orthographic = true;
    }

    //coroutine Camera Shake



}
