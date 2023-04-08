using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    [Header("General")]
    public bool Selected = false;
    public bool LookForward = true;
    private int waitForWaypoints;
    [SerializeField] private GameObject StaticLookDirectionArrow;
    [SerializeField] private float _walkSpeed = 3.0f;
    [SerializeField] private float _rotateSpeed = 10.0f; 
    [SerializeField] private CharacterController _controller;
    private TimeManager _timeManager;

    [Header("Path")]
    [SerializeField]private GameObject _arrowPrefab;
    private List<Waypoint> _waypoints = new List<Waypoint>();
    private Waypoint _selectedWaypoint;
    private bool _dragingLookDirection = false;

    [Header("PatheDrawer")]
    [SerializeField] LayerMask _colliderMask;
    [SerializeField] float _LineResolution = 1f;
    [SerializeField] int _lineLimit = 100;
    [SerializeField] LineRenderer _line;
    private bool _dragging = false;
    private Vector3 _previousPoint = Vector3.zero;
    private Vector2 _previousMousePos = Vector2.zero;
    private List<Vector3> _points = new List<Vector3>();
    private Ray MouseRay => Camera.main.ScreenPointToRay(Input.mousePosition);
    private Plane _plane;

    [Header("Field of View")]
    private Vector3 _lookdirection;
    private Vector3 _currentLookDirection;

    [Header("Gizmos")]
    [SerializeField] private bool _enableGizmos = true;

    // Start is called before the first frame update
    void Start()
    {
        _currentLookDirection = transform.forward;
        _lookdirection = transform.forward;
        _plane = new Plane(Vector3.up, -.5f);
        _timeManager = GameObject.FindGameObjectWithTag("TimeManager").GetComponent<TimeManager>();
        if (_timeManager == null)
        {
            Debug.LogError("There is no TimeManager in the scene with the TimeManager tag!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        DrawLine();

        if (!_timeManager.Paused)
        {
            MoveToPoint();

            _lookdirection = _lookdirection.normalized * 5;
            _currentLookDirection = Vector3.MoveTowards(_currentLookDirection, _lookdirection, Time.deltaTime * _rotateSpeed);
            Vector3 lookpos = _currentLookDirection + transform.position;
            lookpos.y = transform.position.y;
            transform.LookAt(lookpos, Vector3.up);
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (_plane.Raycast(MouseRay, out float enter))
            {
                Vector3 hitPoint = MouseRay.GetPoint(enter);
                if (GetWaypoint(hitPoint, out int index))
                {
                    _dragingLookDirection = true;
                    _selectedWaypoint = _waypoints[index];
                }
            }
        }

        if (_dragingLookDirection)
        {
            if (_plane.Raycast(MouseRay, out float enter))
            {
                Vector3 hitPoint = MouseRay.GetPoint(enter);
                Waypoint wp = _selectedWaypoint;
                wp.LookRotation = hitPoint - wp.Position;
                if (wp.Visuals == null)
                    wp.Visuals = Instantiate(_arrowPrefab);
                wp.Visuals.transform.position = wp.Position;
                wp.Visuals.transform.LookAt(hitPoint, Vector3.up);
            }
        }

        if (_dragingLookDirection && Input.GetMouseButtonUp(1))
        {
            _dragingLookDirection = false;
        }

        
    }

    private void MoveToPoint()
    {
        if (_points.Count == 0)
        {
            return;
        }

        _controller.Move(((_waypoints[0].Position - new Vector3(transform.position.x, _waypoints[0].Position.y, transform.position.z))).normalized * Time.deltaTime * _walkSpeed);

        if (Vector3.Distance(_waypoints[0].Position, new Vector3(transform.position.x, _waypoints[0].Position.y, transform.position.z)) < 0.05f)
        {
            if (LookForward && _waypoints.Count >=4 )
            {
                _lookdirection = _waypoints[3].Position - transform.position;
            }
            if (_waypoints[0].LookRotation != default)
            {
                LookForward = false;
                StaticLookDirectionArrow.SetActive(true);
                Vector3 lookpos = _waypoints[0].LookRotation + transform.position;
                lookpos.y = transform.position.y;
                StaticLookDirectionArrow.transform.LookAt(lookpos - transform.position, StaticLookDirectionArrow.transform.up);
                waitForWaypoints = 6;
                _lookdirection = _waypoints[0].LookRotation;
            }
            if (waitForWaypoints == 0)
            {
                LookForward = true;
                StaticLookDirectionArrow.SetActive(false);
            }
            else
            {
                waitForWaypoints--;
            }

            if (_waypoints[0].Visuals != null)
                Destroy(_waypoints[0].Visuals);

            _points.RemoveAt(0);
            _waypoints.RemoveAt(0);
            UpdateLine();
        }
    }

    #region DrawLine

    public void DrawLine()
    {
        if (!Selected)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            
            if (_plane.Raycast(MouseRay, out float enter))
            {
                Vector3 hitPoint = MouseRay.GetPoint(enter);
                if (Vector3.Distance(hitPoint, transform.position) < .7f)
                {
                    _dragging = true;
                    _previousPoint = hitPoint;

                    foreach (var wp in _waypoints)
                    {
                        Destroy(wp.Visuals);
                    }
                    
                    _points.Clear();
                    _waypoints.Clear();
                    UpdateLine();
                }
                else if (_points.Count > 0)
                {
                    if (GetWaypoint(hitPoint, out int index))
                    {
                        _dragging = true;
                        _previousPoint = _points[index];
                        for (int i = _waypoints.Count-1; i > index; i--)
                        {
                            Destroy(_waypoints[i].Visuals);
                            _points.RemoveAt(i);
                            _waypoints.RemoveAt(i);
                            UpdateLine();
                        }
                    }
                }

            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            _dragging = false;
        }

        if (_dragging && Vector2.Distance(_previousMousePos, Input.mousePosition) > 5)
        {
            _previousMousePos = Input.mousePosition;
            
            if (_points.Count > _lineLimit)
                return;

            if (_plane.Raycast(MouseRay, out float enter))
            {
                Vector3 hitPoint = MouseRay.GetPoint(enter);
                float dist = Vector3.Distance(hitPoint, _previousPoint);
                if (dist > _LineResolution)
                {
                    RaycastHit hit;
                    if (!Physics.SphereCast(_previousPoint, _controller.radius, hitPoint - _previousPoint, out  hit, (hitPoint - _previousPoint).magnitude, _colliderMask))
                    {
                        _previousPoint = hitPoint;
                        _points.Add(hitPoint);
                        UpdateLine();
                        MakeWaypoint(hitPoint);
                    }
                }
            }
        }
    }

    private void UpdateLine()
    {
        Vector3[] points = _points.ToArray();
        _line.positionCount = points.Length;
        _line.SetPositions(points);
    }

    #endregion DrawLine

    private void MakeWaypoint(Vector3 position)
    {
        Waypoint waypoint = new Waypoint(position);
        _waypoints.Add(waypoint);
    }

    // UNOPTIMISED!!!
    public bool GetWaypoint(Vector3 position, out int index)
    {
        index = -1;
        for (int i = 0; i < _waypoints.Count; i++)
        {
            if (Vector3.Distance(_waypoints[i].Position, position) < _LineResolution)
            {
                index = i;
                return true;
            }
        }
        return false;
    }

    #region Gizmos

    private void OnDrawGizmos()
    {
        if (!_enableGizmos)
            return;

        DrawGizmnosPath();
    }

    private void DrawGizmnosPath()
    {
        for (int i = 1; i < _waypoints.Count; i++)
        {
            Vector3 beginPos = _waypoints[i - 1].Position;
            Vector3 endPos = _waypoints[i].Position;
            Gizmos.DrawLine(beginPos, endPos);

            if (_waypoints[i].Action != null)
            {
                Gizmos.DrawSphere(endPos, 1);
            }

            if (_waypoints[i].LookRotation != default)
            {
                Gizmos.DrawRay(_waypoints[i].Position, _waypoints[i].LookRotation);
            }
        }
    }

    #endregion Gizmos
}
