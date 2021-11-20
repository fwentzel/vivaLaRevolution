using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputActionsManager : MonoBehaviour
{

    public static InputActionsManager instance { get; private set; }
    public InputActions inputActions;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
            
        if (inputActions == null)
            inputActions = new InputActions();
    }
}
