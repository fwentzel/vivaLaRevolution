﻿
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class Building : MonoBehaviour
{
    
    public List<Protestor> protestors = new List<Protestor>();
    public int maxProtestors = 3;

    public float captureDurration = 5f;
    private float captureTime = 0;
    public bool isCaptured = false;

    private Renderer renderer;
    private Color initialColor;

    public UnityEvent OnCaptured;
    
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
        
        
        if(!protestors.Contains(protestor))
            protestors.Add(protestor);
    }

    public void Update()
    {
        if (isCaptured)
            return;
        
        captureTime += Time.deltaTime * protestors.Count;
        captureTime = Mathf.Clamp(captureTime, 0, captureDurration);

        float percent = captureTime / captureDurration;
        
        if (renderer)
        {
            renderer.material.color = Color.Lerp(initialColor, Color.red,percent);
        }
        
        
        
        if (percent > 0.98f)
        {
            isCaptured = true;
            LeaveProtestors();
            
            OnCaptured?.Invoke();
        }
    }

    public void LeaveProtestors()
    {
        foreach (var protestor in protestors)
        {
            protestor.LeaveBuilding();
        }
        protestors.Clear();
    }

    public bool CanEnter()
    {
        return protestors.Count < maxProtestors || isCaptured;
    }
}
