
using UnityEngine;

public class Uneasyness : MonoBehaviour
{
    public float updateInterval = 1;
    public float nextUpdate = 0;
    public float value { get; private set; } = .2f;
    MeshRenderer meshRenderer;
    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }
    public void UpdateValue(Collider[] colliders)
    {
        if (Time.time >= nextUpdate)
        {
            bool isPolice = tag.Equals("Police");
            int numPolice = 0;
            int numProtestors = 0;
            foreach (Collider collider in colliders)
            {
                if (collider.tag.Equals("Police"))
                {
                    numPolice++;
                }
                else if (collider.tag.Equals("Protestor"))
                {
                    numProtestors++;
                }
            }
            if (numProtestors >= numPolice && isPolice)
            {
                value += .03f;
            }
            else if (numPolice >= numProtestors && !isPolice)
            {
                value += .03f;
            }
            else
            {
                value -= .01f;
            }
            value = Mathf.Clamp(value, 0, 1);
            nextUpdate = Time.time + updateInterval;
            Color color = meshRenderer.material.color;
            meshRenderer.material.color = isPolice ? new Color(color.b * value, color.b * value, color.b) : new Color(color.r, color.r * value, color.r * value);
        }
    }
}
