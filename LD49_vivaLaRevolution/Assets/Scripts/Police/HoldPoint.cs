﻿
using UnityEngine;

public class HoldPoint : MonoBehaviour
{
    private void OnDrawGizmos() {
        Gizmos.color=Color.blue;
         Gizmos.DrawSphere(transform.position, 1.5f);
    }
}