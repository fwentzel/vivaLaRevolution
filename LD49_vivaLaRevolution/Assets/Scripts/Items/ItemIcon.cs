using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ItemIcon : MonoBehaviour
{
    public Item item;
    public Image iconImage;

    public UnityEvent onSelect;
    public UnityEvent onDeselect;

    public void Setup(Item item)
    {
        this.item = item;
        this.iconImage.sprite = item.icon;
    }



    public void Release()
    {
        if (!item)
        {
            Destroy(gameObject);
            return;
        }
        //TODO

        Vector3 position = RTSSelection.CastToGround(InputSystem.GetDevice<Pointer>().position.ReadValue());

        Debug.DrawRay(position, Vector3.up * 100, Color.magenta, 1f);

        item.Use(position);
        ItemManager.instance.RemoveItemFromList(this);

    }


    public void Select()
    {
        ItemManager.instance.SelectItem(this);
        print("Selected item");
        onSelect?.Invoke();
    }

    public void Deselect()
    {
        onDeselect?.Invoke();
    }
}
