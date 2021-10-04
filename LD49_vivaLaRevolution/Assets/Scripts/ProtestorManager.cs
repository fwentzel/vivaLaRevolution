using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtestorManager : MonoBehaviour
{
    [SerializeField] Transform protestorParent;
    [SerializeField] GameObject protestorPrefab;
    public static ProtestorManager instance { get; private set; }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    public void OnProtestorDeath()
    {
        //1, because the last one alive will call this Method BEFORE dieing
        if (protestorParent.childCount <= 1)
        {
            GameManager.instance.EndGame(false);
        }
    }

    internal void SpawnProtestor(Vector3 position)
    {
        Instantiate(protestorPrefab , position, Quaternion.identity,transform);
    }
}
