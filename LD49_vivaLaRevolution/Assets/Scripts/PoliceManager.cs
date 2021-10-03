﻿
using System.Collections.Generic;
using UnityEngine;

public class PoliceManager : MonoBehaviour
{
    public GameObject policePrefab;
    public HoldPoint defaultSpawnPoint;
    public List<PoliceGroup> groups;
    public float respawnInterval = 5;
    private float nextRespawn = 5;

    public int requiredAmountPerHoldPoint = 6;

    public Vector2Int minMaxSpawnAmount = new Vector2Int(1, 3);
    public static PoliceManager instance { get; private set; }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
        nextRespawn = Time.time + respawnInterval;
        groups = new List<PoliceGroup>(FindObjectsOfType<PoliceGroup>());

    }
    private void Start()
    {
        FillGroups();

    }

    private void Update()
    {
        if (nextRespawn < Time.time)
        {
            FillGroups(Random.Range(minMaxSpawnAmount.x, minMaxSpawnAmount.y));
            nextRespawn = Time.time + respawnInterval;
            CheckChangeHoldPositons();
        }
    }
    public void RegisterFatality(Police member)
    {
        member.group.members.Remove(member);
        CheckChangeHoldPositons();
    }

    private void CheckChangeHoldPositons()
    {
        int result = 0;
        bool fallBack = false;
        bool advance = false;
        foreach (PoliceGroup group in groups)
        {
            //Returns +1 if this group can advance, returns -1 if needs to fall back
            int tempResult = group.CheckChangeHoldPosition();
            result += tempResult;
            if (tempResult < 0)
            {
                fallBack = true;
            }

        }
        advance = result == groups.Count;
        foreach (PoliceGroup group in groups)
        {
            if (advance)
            {
                group.TryGoToNextHoldPoint();
            }
            else if (fallBack)
            {
                group.TryGoToPreviousHoldPoint();
            }

        }

    }

    private void FillGroups(int amountPerGroup = -1)
    {
        bool isStart = amountPerGroup < 0;
        foreach (PoliceGroup group in groups)
        {
            int v = isStart ? group.startSize : amountPerGroup;
            HoldPoint spawnHoldPoint = isStart ? group.GetHeadstartHoldpoint() : defaultSpawnPoint;
            for (int i = 0; i < v; i++)
            {

                GameObject obj = Instantiate(policePrefab, spawnHoldPoint.transform.position, Quaternion.identity);
                Police police = obj.GetComponent<Police>();
                group.members.Add(police);
                police.group = group;
                police.holdPosition = spawnHoldPoint.transform;
            }

        }


    }




}
