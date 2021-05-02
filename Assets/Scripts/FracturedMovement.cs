using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FracturedMovement : MonoBehaviour
{
    public RocketController rc;
    public GameObject[] fracturedPieces;

    void Update()
    {
        FractureCheck();
    }

    private void FractureCheck()
    {
        if (rc.currentState == RocketController.State.Crash)
        {
            fracturedPieces = GameObject.FindGameObjectsWithTag("Fracture");

            for (var i = fracturedPieces.Length - 1; i > -1; i--)
            {
                fracturedPieces[i].GetComponent<Rigidbody>().AddForce(Vector3.down * 35, ForceMode.Impulse);
            }
        }
    }
}
