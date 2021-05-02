using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraFollow : MonoBehaviour
{
    private Vector3 offset;
    private Vector3 lerpedOffset = new Vector3(0, 50, -150);
    public ScoreCounter sc;
    public Transform target;
    public RocketController rc;

    private void Start()
    {
        offset = new Vector3(0, 10, -100);
    }

    private void Update()
    {
        switch (RocketController.Instance.currentState)
        {
            case RocketController.State.Fly:
                Fly();
                break;

            case RocketController.State.Crash:
                Crash();
                break;
        }

        if (rc.currentState != RocketController.State.Crash)
        {
            transform.position = target.position + offset;
            transform.LookAt(target);
        }

        if (rc.isEnded)
        {
            transform.DORotate(new Vector3(30, 0, 0), 1);
            transform.DOMoveY(transform.position.y + 110, 1);
            transform.DOMoveZ(transform.position.z - 80, 1);
            rc.isEnded = false;
        }
    }
    
    private void Crash()
    {
        sc.CalculateScore();
    }

    private void Fly()
    {
        offset = Vector3.Lerp(offset, lerpedOffset, 0.2f);
    }
}
