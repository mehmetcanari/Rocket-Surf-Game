using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraFollow : MonoBehaviour
{
    public ScoreCounter sc;

    public Transform target;
    public Vector3 offset;
    public DragShoot ds;

    Vector3 lastPos;

    bool shake;

    private void Start()
    {
        offset = new Vector3(0, -10, -100);
    }

    private void Update()
    {
        transform.position = target.position + offset;
        if (ds.currentState == DragShoot.State.Fly)
        {
            
            offset = Vector3.Lerp(offset, new Vector3(0, 50, -150), 0.2f);
        }

        if (ds.currentState == DragShoot.State.Crash)
        {
            shake = true;
            lastPos = transform.position;
            

            Debug.Log("True");
            
            if (ds.isEnded)
            {
                transform.DOShakePosition(3, 2, 5, 70, true);
                transform.DORotate(new Vector3(30, 0, 0), 1);
                transform.DOMoveY(transform.position.y + 70, 1);
                transform.DOMoveZ(transform.position.z - 70, 1);
                ds.isEnded = false;
            }

        }
        if (shake)
        {
            StartCoroutine(Shake(.15f, .4f));
            sc.CalculateScore();
        }

        transform.LookAt(target);
    }

    public IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalPos = lastPos;

        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(0, 0f) * magnitude;
            float y = Random.Range(0f, 0f) * magnitude;

            lastPos = new Vector3(x, y, originalPos.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        lastPos = originalPos;
    }
}
