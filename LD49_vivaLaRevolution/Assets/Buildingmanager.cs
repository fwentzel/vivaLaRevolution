
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
            building.transform.localScale += new Vector3(0,v,0)*10 ;


            Renderer renderer = building.GetComponent<Renderer>();
            if (renderer)
            {
                Color color = renderer.material.color;
                Color colorDark = color * 0.5f;
                Color colorLight = color * 1.1f;
                renderer.material.color = Color.Lerp(colorDark, colorLight, Random.Range(0, 1f));
            }
        }
    }
}
