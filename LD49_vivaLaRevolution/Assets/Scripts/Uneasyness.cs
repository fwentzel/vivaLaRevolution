
using UnityEngine;

public class Uneasyness : MonoBehaviour
{
    public float updateInterval = 1;
    public float nextUpdate = 0;
    public float value { get; private set; } = .2f;
    MeshRenderer meshRenderer;
     bool isPolice;
    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
         isPolice= tag.Equals("Police");
    }
    public void UpdateValueForGroups(Collider[] colliders)
    {
        if (Time.time >= nextUpdate)
        {
           
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

    public void UpdateValue(float amount)
    {
        value += amount;
        value = Mathf.Clamp(value, 0, 1);
         Color color = meshRenderer.material.color;
         meshRenderer.material.color = isPolice ? new Color(color.b * value, color.b * value, color.b) : new Color(color.r, color.r * value, color.r * value);
    }
}
