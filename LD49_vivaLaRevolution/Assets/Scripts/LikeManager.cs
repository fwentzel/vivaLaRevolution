
using UnityEngine;

public class LikeManager : MonoBehaviour
{
    public float likeability { get; private set; } = 0;
    public GameObject protestorPrefab;
    public GameObject protestorParent;
    [SerializeField] private int repeatTime = 10;

    public static LikeManager instance { get; private set; }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }
    private void Start()
    {
        InvokeRepeating("EffectProtestors", repeatTime, repeatTime);
    }

    private void EffectProtestors()
    {
        if (likeability > 0)
        {
            print("likeability: Spawning "+likeability+" protestors with random Protestors");
            //Spawn new Protestors
            for (int i = 0; i < Mathf.FloorToInt(likeability); i++)
            {
                int index = Random.Range(0, protestorParent.transform.childCount);
                Transform randomProtestor = protestorParent.transform.GetChild(index);
                Instantiate(protestorParent, randomProtestor.position, Quaternion.identity);
            }
        }
        else if (likeability < 0)
        {
            print("likeability: Removing "+likeability+" random protestors");
            //deactivate protestor
            for (int i = 0; i < Mathf.CeilToInt(likeability); i++)
            {
                int index = Random.Range(0, protestorParent.transform.childCount);
                Transform randomProtestor = protestorParent.transform.GetChild(index);
                Destroy(randomProtestor);
            }
        }
    }
    public void IncreaseLikeability(float amount)
    {
        likeability = Mathf.Clamp(likeability + amount, -10, 10);

    }
    public void DecreaseLikeability(float amount)
    {
        likeability = Mathf.Clamp(likeability - amount, -10, 10);
    }
}
