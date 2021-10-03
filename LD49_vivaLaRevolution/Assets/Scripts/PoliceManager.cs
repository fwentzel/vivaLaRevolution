
using System.Collections.Generic;
using UnityEngine;

public class PoliceManager : MonoBehaviour
{
    public GameObject policePrefab;
    public HoldPoint spawnPoint;
    public List<PoliceGroup> groups;
    public float respawnInterval = 5;
    private float nextRespawn = 5;

    public Vector2Int minMaxSpawnAmount = new Vector2Int(1,3);
    public static PoliceManager instance { get; private set; }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
        nextRespawn = Time.time + respawnInterval;

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
        }
    }

    private void FillGroups(int amountPerGroup = -1)
    {

        groups = new List<PoliceGroup>(FindObjectsOfType<PoliceGroup>());
        foreach (PoliceGroup group in groups)
        {
            int v = amountPerGroup < 0 ? group.startSize : amountPerGroup;
            for (int i = 0; i < v; i++)
            {
                GameObject obj = Instantiate(policePrefab, spawnPoint.transform.position, Quaternion.identity);
                Police police = obj.GetComponent<Police>();
                group.members.Add(police);
                police.group = group;
            }
            group.SetHoldPosition();
        }
    }




}
