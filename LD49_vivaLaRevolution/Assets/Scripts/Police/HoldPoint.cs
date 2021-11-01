
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoldPoint : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public UnityEvent onCapturePoint;
    public UnityEvent onLoosePoint;
    [SerializeField] float changePerSecond = 1;

    [SerializeField] float captureRadius = 15;
    [SerializeField] float buildingInfluenceRadius = 15;

    [SerializeField] LayerMask policeLayer;
    [SerializeField] LayerMask protestorLayer;
    [SerializeField] LayerMask buildingLayer;
    [SerializeField] Image protestorProgessImage;
    [SerializeField] Image policeProgessImage;
    [SerializeField] Image buildingInfluenceRangeImage;

    Collider[] output = new Collider[4];
    float capturedAmount = 0;
    Building[] buildingsToInfluence;

    Boolean isCaptured = false;

    private void Awake()
    {
        GetComponent<SphereCollider>().radius = captureRadius;
        buildingInfluenceRangeImage.gameObject.SetActive(false);
    }
    private void Start()
    {
        protestorProgessImage.rectTransform.sizeDelta = new Vector2(captureRadius * 2, captureRadius * 2);
        policeProgessImage.rectTransform.sizeDelta = new Vector2(captureRadius * 2, captureRadius * 2);
        buildingInfluenceRangeImage.rectTransform.sizeDelta = new Vector2(buildingInfluenceRadius * 2, buildingInfluenceRadius * 2);
        UpdateFillAmounts();
        SetupBuildingsToInfluence();
    }

    private void SetupBuildingsToInfluence()
    {
        bool isMainBuildingInRange = false;
        Collider[] buildingCollider = Physics.OverlapSphere(transform.position, buildingInfluenceRadius, buildingLayer);

        foreach (var item in buildingCollider)
        {
            if (item.CompareTag("MainBuilding"))
            {
                isMainBuildingInRange = true;
                break;
            }
        }
        //Exclude Mainbuilding
        buildingsToInfluence = isMainBuildingInRange ? new Building[buildingCollider.Length - 1] : new Building[buildingCollider.Length];
        int i = 0;
        foreach (Collider col in buildingCollider)
        {
            if (isMainBuildingInRange && col.CompareTag("MainBuilding"))
            {
                continue;
            }
            buildingsToInfluence[i] = col.GetComponent<Building>();
            i++;
        }
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
            print("capture");
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
        }
        isCaptured = false;
        onLoosePoint?.Invoke();
    }


    void IPointerEnterHandler.OnPointerEnter(PointerEventData pointerEventData)
    {
        // buildingInfluenceRangeImage.gameObject.SetActive(true);
        foreach (Building building in buildingsToInfluence)
        {
            building.quickOutline.OutlineColor = building.lootable ? Color.green : Color.red;
            building.quickOutline.enabled = true;

        }
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData pointerEventData)
    {
        // buildingInfluenceRangeImage.gameObject.SetActive(false);
        foreach (Building building in buildingsToInfluence)
        {
            building.quickOutline.enabled = false;
            building.quickOutline.OutlineColor = Color.white;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, 1.5f);

        Gizmos.DrawWireSphere(transform.position, captureRadius);
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, buildingInfluenceRadius);
    }
}
