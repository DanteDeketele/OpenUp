using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Waypoint
{
    public Vector3 Position;
    public UnityAction Action;
    public Vector3 LookRotation;
    public GameObject Visuals;

    public Waypoint(Vector3 position)
    {
        Position = position;
        Action = null;
    }

    public Waypoint(Vector3 position, Vector3 lookRotation, GameObject visuals)
    {
        Position = position;
        LookRotation = lookRotation;
        Action = null;
        Visuals = visuals;
    }


    public Waypoint(Vector3 position, UnityAction action)
    {
        this.Position = position;
        this.Action = action;
    }
}
