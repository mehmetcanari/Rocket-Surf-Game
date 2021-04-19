using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RingRoll : MonoBehaviour
{
    void Update()
    {
        transform.Rotate(0, 0, 50 * Time.deltaTime);
    }
}
