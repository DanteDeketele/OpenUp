using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    /*[Header("Parameters")]
    [SerializeField] private float _zoomSpeed = 20.0f;
    [SerializeField] private AnimationCurve _zoomCurve;

    [Header("Movement Volume")]
    [SerializeField] private Vector2 _volumeSize = Vector2.one*100;
    [SerializeField] private float _minimumDistance = 5.0f;
    [SerializeField] private Color _gizmosVolumeColor = Color.yellow;

    private Camera _camera;
    private float _zoom = 1.0f;

    private Vector2 mouseClickPos;
    private Vector2 mouseCurrentPos;
    private bool panning = false;

    private float MaxHeight => MinimumDistToEdge / Mathf.Tan(Mathf.Deg2Rad * _camera.fieldOfView);
    private float Height => Mathf.Lerp(_minimumDistance, MaxHeight, _zoomCurve.Evaluate(_zoom));
    private float MinimumDistToEdge => Mathf.Abs(_volumeSize.x - transform.position.x);

    // Start is called before the first frame update
    void Start()
    {
        _camera = GetComponent<Camera>();
    }

    
    void Update()
    {
        /*if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            _zoom -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * _zoomSpeed;
            _zoom = Mathf.Clamp01(_zoom);
            transform.position = new Vector3(transform.position.x, Height, transform.position.z);
        }*//*

        // When LMB clicked get mouse click position and set panning to true
        if (Input.GetKeyDown(KeyCode.Mouse0) && !panning)
        {
            mouseClickPos = _camera.ScreenToWorldPoint(Input.mousePosition);
            panning = true;
        }
        // If LMB is already clicked, move the camera following the mouse position update
        if (panning)
        {
            mouseCurrentPos = _camera.ScreenToWorldPoint(Input.mousePosition);
            var distance = mouseCurrentPos - mouseClickPos;
            transform.position += new Vector3(-distance.x, -distance.y, 0);
        }

        // If LMB is released, stop moving the camera
        if (Input.GetKeyUp(KeyCode.Mouse0))
            panning = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = _gizmosVolumeColor;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(_volumeSize.x, 0, _volumeSize.y));
    }*/
}
