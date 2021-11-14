
using UnityEngine;

public class Buildingmanager : MonoBehaviour
{
    public float magnitude=10;
    private void Awake()
    {

        foreach (Building building in FindObjectsOfType<Building>())
        {
            if (building.tag.Equals("MainBuilding"))
                continue;
            Transform buildingTransform = building.transform;
            float v = Mathf.PerlinNoise(buildingTransform.position.x, buildingTransform.position.z)-0.5f;
            v*=magnitude;

            buildingTransform.localScale += new Vector3(0, v, 0) ;
            buildingTransform.position= new Vector3(buildingTransform.position.x, buildingTransform.localScale.y/2f, buildingTransform.position.z);

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
