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
    public ItemIcon iconPreset;
    private List<ItemIcon> _itemIcons = new List<ItemIcon>();
    public CanvasGroup canvasGroup;

    public Transform aimingRecticle;
    public Transform maxDistanceRecticle;

    InputActions.ItemActions itemInput;

    private void Awake()
    {
        instance = this;
       itemInput = new InputActions().Item;
    }
    private void OnEnable()
    {
        itemInput.Enable();
    }

    private void OnDisable()
    {
        itemInput.Disable();
    }


    private void Start()
    {
        _rtsSelection = FindObjectOfType<RTSSelection>();
        if (_rtsSelection != null)
            _rtsSelection.OnUnitSelection.AddListener(OnSelectedUnits);
    }

    public void OnSelectedUnits(List<Protestor> protestors)
    {
        PopulateList(protestors);
    }

    public void PopulateList(List<Protestor> protestors)
    {
        ClearList();
        iconPreset.gameObject.SetActive(false);
        foreach (var protestor in protestors)
        {
            if (!protestor.item)
                continue;

            GameObject itemIconObj = Instantiate(iconPreset.gameObject, content);
            ItemIcon itemIcon = itemIconObj.GetComponent<ItemIcon>();
            itemIcon.Setup(protestor.item);
            itemIconObj.SetActive(true);
            _itemIcons.Add(itemIcon);
        }

    }

    public void AddToList(Protestor protestor)
    {
        iconPreset.gameObject.SetActive(false);
        if (!protestor.item)
            return;

        GameObject itemIconObj = Instantiate(iconPreset.gameObject, content);
        ItemIcon itemIcon = itemIconObj.GetComponent<ItemIcon>();
        itemIcon.Setup(protestor.item);
        itemIconObj.SetActive(true);
        _itemIcons.Add(itemIcon);


    }

    public void ClearList()
    {
        foreach (var itemIcon in _itemIcons)
        {
            if (!itemIcon)
                continue;
            Destroy(itemIcon.gameObject);
        }
        _itemIcons.Clear();
    }


    public void Update()
    {
        Vector3 position = RTSSelection.CastToGround(itemInput.Point.ReadValue<Vector2>());
        ItemIcon selectedItem = null;
        foreach (var itemIcon in _itemIcons)
        {
            //TODO refactor to event
            if (!itemIcon)
                continue;
            if (itemInput.Cancel.triggered)
                itemIcon.Deselect();

            if (itemIcon.isSelected)
                selectedItem = itemIcon;
        }

        aimingRecticle.gameObject.SetActive(selectedItem != null);
        maxDistanceRecticle.gameObject.SetActive(selectedItem != null);
        if (selectedItem && selectedItem.item)
        {
            
            position = selectedItem.item.GetImprovedPosition(position);

            aimingRecticle.position = position + Vector3.up * 0.5f;
            aimingRecticle.transform.localScale = Vector3.one * 2 * selectedItem.item.influenceRadius;

            maxDistanceRecticle.position = selectedItem.item.transform.position;
            maxDistanceRecticle.transform.localScale = Vector3.one * 2 * selectedItem.item.maxDistance;

            if (itemInput.Fire.triggered)
            {
                selectedItem.Release();
            }
        }



    }
}
