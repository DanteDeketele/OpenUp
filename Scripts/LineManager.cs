using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class LineManager : MonoBehaviour
{
    [SerializeField] float _LineResolution = 1f;
    [SerializeField] int _lineLimit = 100;

    [SerializeField] GameObject _linePrefab;
    private AudioSource _audioSource;

    private bool _dragging = false;
    private Vector3 _previousPoint = Vector3.zero;
    private Vector2 _previousMousePos = Vector2.zero;

    private List<Vector3> _points = new List<Vector3>();

    private Ray MouseRay => Camera.main.ScreenPointToRay(Input.mousePosition);
    private Plane _plane;

    private GameObject _line;

    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _plane = new Plane(Vector3.up, -.5f);
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(1))
        {
            _dragging = true;
            _audioSource.Play();
            _points.Clear();
            if (_plane.Raycast(MouseRay, out float enter))
            {
                Vector3 hitPoint = MouseRay.GetPoint(enter);
                _previousPoint = hitPoint;
                _points.Add(hitPoint);
            }

            _line = Instantiate(_linePrefab);
        }
        
        if (Input.GetMouseButtonUp(1))
        {
            _dragging = false;
            Destroy(_line, 10);
        }

        if (_dragging && Vector2.Distance(_previousMousePos, Input.mousePosition) > 10)
        {
            _previousMousePos = Input.mousePosition;

            if (_points.Count > _lineLimit)
                return;

            if(_plane.Raycast(MouseRay, out float enter))
            {
                Vector3 hitPoint = MouseRay.GetPoint(enter);
                if (Vector3.Distance(hitPoint, _previousPoint) > _LineResolution)
                {
                    _previousPoint = hitPoint;
                    _points.Add(hitPoint);

                    LineRenderer renderer = _line.GetComponent<LineRenderer>();

                    renderer.positionCount = _points.Count;
                    renderer.SetPositions(_points.ToArray());
                }
            }
        }
        
    }
}
