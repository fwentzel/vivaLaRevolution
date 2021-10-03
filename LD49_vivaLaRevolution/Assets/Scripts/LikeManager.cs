
using UnityEngine;

using UnityEngine.Events;

public class LikeManager : MonoBehaviour
{
    public float likeability { get; private set; } = 0;
    public GameObject protestorPrefab;
    public GameObject protestorParent;
    public UnityEvent<float> onLikeChange;
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
         onLikeChange?.Invoke(likeability);
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
            if(likeability*-1 > protestorParent.transform.childCount){
                
                print("YOU LOST! The Instable System took over");
            }
            print("likeability: Removing "+likeability+" random protestors");
            //deactivate protestor
            for (int i = 0; i < Mathf.FloorToInt(likeability*-1); i++)
            {
                int index = Random.Range(0, protestorParent.transform.childCount-1);
                Transform randomProtestor = protestorParent.transform.GetChild(index);
                Destroy(randomProtestor.gameObject);
            }
        }
    }
    public void IncreaseLikeability(float amount)
    {
        likeability = Mathf.Clamp(likeability + amount, -10, 10);
        onLikeChange?.Invoke(likeability);

    }
    public void DecreaseLikeability(float amount)
    {
        likeability = Mathf.Clamp(likeability - amount, -10, 10);
        onLikeChange?.Invoke(likeability);
    }
}
