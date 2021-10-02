
using UnityEngine;

public class Buildingmanager : MonoBehaviour
{

    private void Awake()
    {

        foreach (Building building in FindObjectsOfType<Building>())
        {
            if (building.tag.Equals("MainBuilding"))
                continue;
            float v = Mathf.PerlinNoise(building.transform.position.x,building.transform.position.z)-0.5f;
            building.transform.localScale += new Vector3(0,v,0)*4 ;
        }
    }
}
