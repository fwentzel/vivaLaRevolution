
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
public class Building : MonoBehaviour
{

    public List<Protestor> protestors = new List<Protestor>();
    public List<GameObject> itemPrefabs = new List<GameObject>();
    public int maxProtestors = 3;

    public float captureDurration = 5f;
    private float captureTime = 0;
    public bool isCaptured = false;

    private Renderer renderer;
    private Color initialColor;

    public UnityEvent OnCaptured;

    public Texture2D cursorTexture;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;

    public int likeAbilityScore = -1;

    private QuickOutline _quickOutline;
    [SerializeField] int lootProbability =20;

    private void Start()
    {
        renderer = GetComponent<Renderer>();
        if (renderer)
            initialColor = renderer.material.color;

        _quickOutline = GetComponent<QuickOutline>();

        if (_quickOutline)
            _quickOutline.enabled = false;
    }


    public virtual void EnterBuilding(Protestor protestor)
    {
        if (isCaptured)
        {
            LeaveProtestors();
        }

        if (!protestors.Contains(protestor))
            protestors.Add(protestor);
    }

    public virtual void Update()
    {
        if (isCaptured || protestors.Count == 0)
            return;


        captureTime += Time.deltaTime * (protestors.Count / (float)maxProtestors);
        captureTime = Mathf.Clamp(captureTime, 0, captureDurration);

        float percent = captureTime / captureDurration;

        if (renderer)
        {
            renderer.material.color = Color.Lerp(initialColor, Color.red, percent);
        }



        if (percent > 0.98f)
        {
            Caputure();
        }
    }

    public void Caputure()
    {
        if (tag.Equals("MainBuilding"))
        {
            GameManager.instance.EndGame(true);
        }
        captureTime = captureDurration;
        // int rdm = UnityEngine.Random.Range(0, 100);
        // if (rdm < 50)
        
            // ProtestorManager.instance.SpawnProtestor();
        

        float initialScaleY = transform.localScale.y;
        transform.DOScaleY(transform.localScale.y * 1.4f, .20f).OnComplete(() => transform.DOScaleY(initialScaleY, .1f));
        isCaptured = true;
        LeaveProtestors();

        OnCaptured?.Invoke();
    }

    public virtual void EquipProtesters()
    {
        print("GIVING ITEM");
        foreach (var protestor in protestors)
        {
            if (!protestor.CanGiveItem())
            {
                print("Couldn't give item");
                continue;
            }
            int rdm = UnityEngine.Random.Range(0, 100);
            if (rdm < lootProbability)
            {
                if (itemPrefabs.Count == 0)
                    return;
                int itemIndex = UnityEngine.Random.Range(0, itemPrefabs.Count);
                GameObject itemObj = Instantiate(itemPrefabs[itemIndex], transform.position, Quaternion.identity);
                Item item = itemObj.GetComponent<Item>();

                print("GIVING ITEM");
                if (protestor.GiveItem(item)) ;
            }




        }
    }

    public virtual void LeaveProtestors()
    {
        EquipProtesters();
        foreach (var protestor in protestors)
        {
            if (protestor == null)
                continue;
            protestor.LeaveBuilding();
        }
        protestors.Clear();
    }

    public bool CanEnter()
    {
        return protestors.Count < maxProtestors || isCaptured;
    }
    void OnMouseEnter()
    {
        if (protestors.Count < maxProtestors)
        {
            Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
            if (_quickOutline)
                _quickOutline.enabled = true;
        }


    }

    void OnMouseExit()
    {
        Cursor.SetCursor(null, Vector2.zero, cursorMode);

        if (_quickOutline)
            _quickOutline.enabled = false;
    }
}
