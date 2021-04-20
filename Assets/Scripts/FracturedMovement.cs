using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FracturedMovement : MonoBehaviour
{
    public GameObject[] fracturedPieces;
    

    void Update()
    {
        #region FractureMovement
        fracturedPieces = GameObject.FindGameObjectsWithTag("Fracture");

        for (var i = fracturedPieces.Length - 1; i > -1; i--)
        {
            fracturedPieces[i].GetComponent<Rigidbody>().AddForce(Vector3.down * 35 , ForceMode.Impulse);
        }
        #endregion
    }
}
