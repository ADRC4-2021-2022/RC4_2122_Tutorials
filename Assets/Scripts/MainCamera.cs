using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public Vector3 Target = Vector3.zero;
    private float _rotateSpeed = 0.1f;
    private float _panSpeed = 0.1f;
    private float _zoomSpeed = 0.1f;
    private float _minXRotation = -10f, _maxXRotation = 90f;
    private Pose _startTransform;
    private float _height = 15f;

    private ProjectControls _projectControls;

    private void Awake()
    {
        _startTransform = new Pose(transform.position, transform.rotation);
        _projectControls = new ProjectControls();

    }

    private void OnEnable()
    {
        _projectControls.Enable();
    }
    private void OnDisable()
    {
        _projectControls.Disable();
    }


    void Update()
    {
        
        if (Application.platform == RuntimePlatform.Android) return;
        bool pan = _projectControls.Camera.Pan.IsPressed();
        bool rotate = _projectControls.Camera.Rotate.IsPressed();
        Vector2 mousePosition = _projectControls.Camera.MousePosition.ReadValue<Vector2>();

        if (pan)
        {
            float right = -mousePosition.x * _panSpeed;
            float up = -mousePosition.y * _panSpeed;

            Pan(right, up);
        }
        else if (rotate)
        {
            float yaw = mousePosition.x;
            float pitch = mousePosition.y;

            Rotate(yaw, pitch);
        }

        float zoomFactor = _projectControls.Camera.Zoom.ReadValue<float>();
        if (zoomFactor != 0)
        {
            Debug.Log(zoomFactor);
            Zoom(zoomFactor*_zoomSpeed);
        }
    }


    public void Pan(Vector2 motion) => Pan(motion.x, motion.y);
    public void Pan(float x, float y)
    {
        Vector3 vector = transform.rotation * new Vector3(x, y, 0);
        transform.position += vector;
        //if (transform.position.y < 0) transform.position = new Vector3(transform.position.x,0,transform.position.z);
        //_target += vector;//Enable is you don't want a fixed target
    }

    public void Rotate(Vector2 motion) => Rotate(motion.x, motion.y);
    public void Rotate(float yaw, float pitch)
    {
        yaw *= _rotateSpeed;
        pitch *= _rotateSpeed;

        float xAngle = FormatAngle(transform.rotation.eulerAngles.x);
        //Clamp rotation around x
        if (xAngle + pitch > _maxXRotation && pitch > 0)
            pitch = 0;
        if (xAngle + pitch < _minXRotation && pitch < 0)
            pitch = 0;
        transform.RotateAround(Target, Vector3.up, yaw);
        transform.RotateAround(Target, transform.rotation * Vector3.right, pitch);
        if (transform.position.y < 0) transform.position = new Vector3(transform.position.x, 0, transform.position.z);
    }


    public void Zoom(float zoomFactor)
    {
        float distance = (Target - transform.position).magnitude * zoomFactor;
        transform.Translate(Vector3.forward * distance, Space.Self);
    }
    //Put angles in a format from -180 to 180 (instead of unity's 0 - 360)
    public float FormatAngle(float angle) => angle < 180 ? angle : angle - 360;

    public void SetTarget(Vector3 position) => Target = position;

    public void ResetPosition() => transform.SetPositionAndRotation(_startTransform.position, _startTransform.rotation);

    public void SetPositionTop()
    {
        VoxelGridManager gridManager = GameObject.FindObjectsOfType<VoxelGridManager>()[0];

        transform.SetPositionAndRotation(new Vector3(gridManager.GridCentre.x, gridManager.GridCentre.y + _height, gridManager.GridCentre.z), Quaternion.Euler(90, 0, 0));
    }
}