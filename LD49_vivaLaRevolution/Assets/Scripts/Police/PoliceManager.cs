
using System.Collections.Generic;
using UnityEngine;

public class PoliceManager : MonoBehaviour
{
    public static PoliceManager instance { get; private set; }
    public GameObject policeMeelePrefab;
    public GameObject policeMusketeerPrefab;
    public Transform spawnParent;
    public HoldPoint defaultSpawnPoint;
    public float respawnInterval = 30;
    public int requiredAmountPerHoldPoint = 6;
    public Vector2Int minMaxSpawnAmount = new Vector2Int(1, 3);
    private float nextRespawn = 0;
    private List<PoliceGroup> groups;
    private float nextChangeHoldpositionCheck = 0;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
        groups = new List<PoliceGroup>(FindObjectsOfType<PoliceGroup>());

    }
    private void Start()
    {
        foreach (var group in groups)
        {
            group.SetupCurrentHoldpoint();
        }
        FillGroups();
        InvokeRepeating("CheckChangeHoldPositons", respawnInterval, 3f);

    }

    private void Update()
    {
        if (nextRespawn < Time.time)
        {
            FillGroups(Random.Range(minMaxSpawnAmount.x, minMaxSpawnAmount.y));
            nextRespawn = Time.time + respawnInterval;
        }

    }
    public void RegisterFatality(PoliceBase member)
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
            if (group.ignoreRespawnAndHoldpointCalc)
            {
                result++;
                continue;
            }

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
            if (group.ignoreRespawnAndHoldpointCalc && !isStart)
                continue;
            HoldPoint spawnHoldPoint = isStart ? group.holdPoints[group.currentHoldIndex] : defaultSpawnPoint;
            for (int i = 0; i <= v; i++)
            {
                if (group.members.Count >= group.startSize * 1.5f)
                {
                    continue;
                }
                GameObject obj;


                bool spawnMusketeer;
                if (isStart)
                {
                    //Do not use randomness on game start
                    spawnMusketeer = i / (group.startSize * 1f) < group.musketeerPercentage;
                }
                else
                {
                    spawnMusketeer = group.musketeerPercentage > Random.Range(0, 1f);
                }

                if (spawnMusketeer)
                {
                    obj = Instantiate(policeMusketeerPrefab, spawnHoldPoint.transform.position + new Vector3(Random.Range(0, .1f), 0, Random.Range(0, .1f)), Quaternion.identity, spawnParent);
                }
                else
                {
                    obj = Instantiate(policeMeelePrefab, spawnHoldPoint.transform.position + new Vector3(Random.Range(0, .1f), 0, Random.Range(0, .1f)), Quaternion.identity, spawnParent);
                }

                PoliceBase police = obj.GetComponent<PoliceBase>();
                group.members.Add(police);
                police.group = group;
                police.holdPosition = group.holdPoints[group.currentHoldIndex].transform;
            }

        }


    }




}
