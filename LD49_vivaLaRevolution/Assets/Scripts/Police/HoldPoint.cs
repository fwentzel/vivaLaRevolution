
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoldPoint : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float changePerSecond = 1;
    [SerializeField] private float captureRadius = 15;
    [SerializeField] private float buildingInfluenceRadius = 15;
    [SerializeField] private Image protestorProgessImage;
    [SerializeField] private Image policeProgessImage;
    public UnityEvent onCapturePoint;
    public UnityEvent onLoosePoint;

    private LayerMask policeLayer;
    private LayerMask protestorLayer;
    private LayerMask buildingLayer;
    private Collider[] output = new Collider[4];
    private float capturedAmount = 0;
    private Building[] buildingsToInfluence;

    private bool isCaptured = false;

    private void Awake()
    {
        GetComponent<SphereCollider>().radius = captureRadius;
        policeLayer = LayerMask.GetMask("Police");
        protestorLayer = LayerMask.GetMask("Protestor");
        buildingLayer = LayerMask.GetMask("Building");
    }
    private void Start()
    {
        protestorProgessImage.rectTransform.sizeDelta = new Vector2(captureRadius * 2, captureRadius * 2);
        policeProgessImage.rectTransform.sizeDelta = new Vector2(captureRadius * 2, captureRadius * 2);
        UpdateFillAmounts();
        SetupBuildingsToInfluence();

    }

    private void SetupBuildingsToInfluence()
    {
        Collider[] buildingCollider = Physics.OverlapSphere(transform.position, buildingInfluenceRadius, buildingLayer);
        List<Building> buildings = new List<Building>();
        foreach (Collider col in buildingCollider)
        {
            if (col.GetComponent<QuickOutline>())
            {
                buildings.Add(col.GetComponent<Building>());
            }
        }

        buildingsToInfluence = buildings.ToArray();

    }

    private void UpdateFillAmounts()
    {
        protestorProgessImage.fillAmount = capturedAmount / 100f;
        policeProgessImage.fillAmount = 1 - capturedAmount / 100f;

    }

    private void Update()
    {

        int policeAmount = Physics.OverlapSphereNonAlloc(transform.position, captureRadius, output, policeLayer);
        int protestorAmount = Physics.OverlapSphereNonAlloc(transform.position, captureRadius, output, protestorLayer);

        if (policeAmount > 0 && protestorAmount == 0 && capturedAmount > 0)
        {
            UpdateCaptureAmount(-changePerSecond * 2 * policeAmount);

        }
        else if (protestorAmount > 0 && policeAmount == 0 && !isCaptured)
        {
            UpdateCaptureAmount(changePerSecond * protestorAmount);
        }


    }
    private void UpdateCaptureAmount(float amount)
    {
        capturedAmount += amount * Time.deltaTime;
        capturedAmount = Mathf.Clamp(capturedAmount, 0, 100);
        if (capturedAmount < 100 && isCaptured)
        {
            LoosePoint();
        }
        else if (capturedAmount == 100 && !isCaptured)
        {
            CapturePoint();
        }
        UpdateFillAmounts();
    }

    private void CapturePoint()
    {
        foreach (Building building in buildingsToInfluence)
        {
            building.lootable = true;
        }
        isCaptured = true;
        onCapturePoint?.Invoke();
    }
    private void LoosePoint()
    {
        foreach (Building building in buildingsToInfluence)
        {
            building.lootable = false;
            building.LeaveProtestors();
        }
        isCaptured = false;
        onLoosePoint?.Invoke();
    }


    void IPointerEnterHandler.OnPointerEnter(PointerEventData pointerEventData)
    {
        ActivateBuildingOutlines();
    }

    private void ActivateBuildingOutlines()
    {
        foreach (Building building in buildingsToInfluence)
        {
            building.quickOutline.OutlineColor = building.lootable ? Color.white : Color.red;
            building.quickOutline.enabled = true;

        }
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData pointerEventData)
    {
        DeactivateBuildingOutlines();

    }


    private void DeactivateBuildingOutlines()
    {
        foreach (Building building in buildingsToInfluence)
        {
            building.quickOutline.enabled = false;
            building.quickOutline.OutlineColor = Color.white;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, captureRadius);
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, buildingInfluenceRadius);
    }
}
