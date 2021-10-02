
using System.Collections.Generic;
using UnityEngine;

public class PoliceManager : MonoBehaviour
{
    public GameObject policePrefab;
    public HoldPoint spawnPoint;
    public List<PoliceGroup> groups;
    public static PoliceManager instance { get; private set; }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
        FillGroups();
    }

    private void FillGroups()
    {

        groups = new List<PoliceGroup>(FindObjectsOfType<PoliceGroup>());
        foreach (PoliceGroup group in groups)
        {
            for (int i = 0; i < group.startSize; i++)
            {
                GameObject obj = Instantiate(policePrefab, spawnPoint.transform.position, Quaternion.identity);
                Police police = obj.GetComponent<Police>();
                group.members.Add(police);
                police.group = group;
            }
            group.SetHoldPositionToNext();
        }
    }




}
