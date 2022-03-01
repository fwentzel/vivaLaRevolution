using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtestorManager : MonoBehaviour
{
    public static ProtestorManager instance { get; private set; }
    public Transform protestorParent;
    [SerializeField] private GameObject protestorPrefab;

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

    internal void SpawnProtestor()
    {
        int v = UnityEngine.Random.Range(0, protestorParent.childCount - 1);
        Instantiate(protestorPrefab, protestorParent.GetChild(v).transform.position, Quaternion.identity, transform);
    }
}
