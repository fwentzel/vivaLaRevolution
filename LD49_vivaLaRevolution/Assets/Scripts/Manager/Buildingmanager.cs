
using UnityEngine;

public class Buildingmanager : MonoBehaviour
{
    public float randomnessMultiplier = 10;
    public Transform buildingParent;
    private void Awake()
    {

        foreach (Transform child in buildingParent)
        {
            
            if (child.tag.Equals("MainBuilding"))
                continue;
            float v = Mathf.PerlinNoise(child.position.x, child.position.z) - 0.5f;
            v *= randomnessMultiplier;
            print(v);
            child.localScale += new Vector3(0, v, 0);
            child.position = new Vector3(child.position.x, child.localScale.y / 2f, child.position.z);

            Renderer renderer = child.GetComponent<Renderer>();
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
