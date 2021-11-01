using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class ItemManager : MonoBehaviour
{
    public static ItemManager instance;
    private RTSSelection _rtsSelection;
    public RectTransform content;
    public GameObject iconPrefab;
    private List<ItemIcon> _itemIcons = new List<ItemIcon>();
    public CanvasGroup canvasGroup;

    public Transform aimingRecticle;
    public Transform maxDistanceRecticle;
    public ItemIcon selectedItem = null;

    InputActions.ItemActions itemInput;

    //Debugging
    public int giveItemsToProtestorsOnStart = 3;
    public List<Item> items;


    private void Awake()
    {
        instance = this;

    }

    private void Start()
    {
        _rtsSelection = FindObjectOfType<RTSSelection>();
        if (_rtsSelection != null)
            _rtsSelection.OnUnitSelection.AddListener(OnSelectedUnits);

        itemInput = InputActionsManager.instance.inputActions.Item;
        itemInput.Enable();
        itemInput.Cancel.performed += ctx => DeselectItem();


        //Debugging 
        Protestor[] protestors = FindObjectsOfType<Protestor>();
        int itemsGiven = 0;
        for (int i = 0; i < protestors.Length; i++)
        {
            if (itemsGiven < giveItemsToProtestorsOnStart && items.Count > 0)
            {
                Item item = Instantiate(items[UnityEngine.Random.Range(0, items.Count)]);
                protestors[i].GiveItem(item);
                itemsGiven++;
            }
        }

    }

    public void OnSelectedUnits(List<Protestor> protestors)
    {
        PopulateList(protestors);
    }

    public void PopulateList(List<Protestor> protestors)
    {
        _itemIcons.ForEach((itemIcon) => itemIcon.gameObject.SetActive(false));
        foreach (var protestor in protestors)
        {
            if (!protestor.item)
                continue;
            _itemIcons.Find(itemIcon => itemIcon.item == protestor.item)?.gameObject.SetActive(true);
        }

    }

    public void AddToList(Protestor protestor)
    {
        if (!protestor.item)
            return;

        GameObject itemIconObj = Instantiate(iconPrefab, content);
        ItemIcon itemIcon = itemIconObj.GetComponent<ItemIcon>();
        itemIcon.Setup(protestor.item);
        itemIconObj.SetActive(RTSSelection.instance.selectedUnits.Contains(protestor));
        _itemIcons.Add(itemIcon);


    }
    public void RemoveItemFromList(ItemIcon itemIcon)
    {
        _itemIcons.Remove(itemIcon);
        if (selectedItem == itemIcon)
            DeselectItem();


    }
    public void RemoveItemFromList(Item item)
    {
        foreach (ItemIcon itemIcon in _itemIcons)
        {
            if (itemIcon.item == item)
            {
                RemoveItemFromList(itemIcon);
                return;
            }
        }
    }
    private void DeselectItem()
    {
        if (selectedItem != null)
        {

            selectedItem.Deselect();
            selectedItem = null;
        }
    }


    public void Update()
    {
        Vector3 position = RTSSelection.CastToGround(itemInput.Point.ReadValue<Vector2>());


        aimingRecticle.gameObject.SetActive(selectedItem != null);
        maxDistanceRecticle.gameObject.SetActive(selectedItem != null);
        if (selectedItem && selectedItem.item)
        {
            RTSSelection.instance.DisableSelectionInput();
            position = selectedItem.item.GetImprovedPosition(position);

            aimingRecticle.position = position + Vector3.up * 0.5f;
            aimingRecticle.transform.localScale = Vector3.one * 2 * selectedItem.item.influenceRadius;

            maxDistanceRecticle.position = selectedItem.item.transform.position;
            maxDistanceRecticle.transform.localScale = Vector3.one * 2 * selectedItem.item.maxDistance;

            if (itemInput.Fire.triggered)
            {
                RTSSelection.instance.EnableSelectionInput();
                selectedItem.Release();
            }
        }
        else
        {
            RTSSelection.instance.EnableSelectionInput();
        }



    }
}
