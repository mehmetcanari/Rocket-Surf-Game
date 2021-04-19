using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraFollow : MonoBehaviour
{
    public Vector3 offset;
    public ScoreCounter sc;
    public Transform target;
    public DragShoot ds;

    private void Start()
    {
        offset = new Vector3(0, 10, -100);
    }

    private void Update()
    {
        if (ds.currentState != DragShoot.State.Crash)
        {
            transform.position = target.position + offset;
            transform.LookAt(target);
        }

        if (ds.currentState == DragShoot.State.Fly)
        {
            offset = Vector3.Lerp(offset, new Vector3(0, 50, -150), 0.2f);
        }

        if (ds.currentState == DragShoot.State.Crash)
        {
            sc.CalculateScore();
        }
        if (ds.isEnded)
        {
            transform.DORotate(new Vector3(30, 0, 0), 1);
            transform.DOMoveY(transform.position.y + 70, 1);
            transform.DOMoveZ(transform.position.z - 70, 1);
            ds.isEnded = false;
        }
    }
}
