
using System.Collections.Generic;
using UnityEngine;

public class PoliceManager : MonoBehaviour
{
    public GameObject policePrefab;
    public Transform mainBuilding;
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
                GameObject obj = Instantiate(policePrefab, mainBuilding.transform.position, Quaternion.identity);
                group.members.Add(obj.GetComponent<Police>());
            }
            group.SetHoldPositionToNext();
        }
    }


   

}
