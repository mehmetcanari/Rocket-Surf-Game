using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityColliders : MonoBehaviour
{
    public MeshRenderer[] cities;

    private void Start()
    {
        cities = GetComponentsInChildren<MeshRenderer>();
    }
}
