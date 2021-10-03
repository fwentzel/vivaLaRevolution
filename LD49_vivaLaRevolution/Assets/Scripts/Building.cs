﻿
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


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


    private void Start()
    {
        renderer = GetComponent<Renderer>();
        if (renderer)
            initialColor = renderer.material.color;
    }


    public void EnterBuilding(Protestor protestor)
    {
        if (isCaptured)
        {
            LeaveProtestors();
        }


        if (!protestors.Contains(protestor))
            protestors.Add(protestor);
    }

    public void Update()
    {
        if (isCaptured || protestors.Count == 0)
            return;


        captureTime += Time.deltaTime * protestors.Count;
        captureTime = Mathf.Clamp(captureTime, 0, captureDurration);

        float percent = captureTime / captureDurration;

        if (renderer)
        {
            renderer.material.color = Color.Lerp(initialColor, Color.red, percent);
        }



        if (percent > 0.98f)
        {
            LikeManager.instance.IncreaseLikeability(-1);
            isCaptured = true;
            LeaveProtestors();

            OnCaptured?.Invoke();
        }
    }

    public void EquipProtesters()
    {
        print("GIVING ITEM");
        foreach (var protestor in protestors)
        {
            if (!protestor.CanGiveItem())
            {
                print("Couldn't give item");
                continue;
            }

            if (itemPrefabs.Count == 0)
                return;
            GameObject itemObj = Instantiate(itemPrefabs[0], transform.position, Quaternion.identity);
            Item item = itemObj.GetComponent<Item>();

            print("GIVING ITEM");
            if (protestor.GiveItem(item)) ;
            itemPrefabs.RemoveAt(0);

        }
    }

    public void LeaveProtestors()
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
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
    }

    void OnMouseExit()
    {
        Cursor.SetCursor(null, Vector2.zero, cursorMode);
    }
}
