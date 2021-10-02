using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceManager : MonoBehaviour
{
    GameObject policePrefab;
    public static PoliceManager instance{get;private set;}
    
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }
}
