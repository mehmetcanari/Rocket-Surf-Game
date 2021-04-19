using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelRotate : MonoBehaviour
{ 
    void Update()
    {
        transform.Rotate(0, 50 * Time.deltaTime, 0);
    }
}
