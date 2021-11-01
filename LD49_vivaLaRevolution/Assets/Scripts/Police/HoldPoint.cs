
using System;
using UnityEngine;
using UnityEngine.UI;

public class HoldPoint : MonoBehaviour
{


    [SerializeField] float changePerSecond = 1;

    [SerializeField] float effectRange = 15;

    [SerializeField] LayerMask policeLayer;
    [SerializeField] LayerMask protestorLayer;
    [SerializeField] Image protestorProgessImage;
    [SerializeField] Image policeProgessImage;

    Collider[] output = new Collider[4];
    float capturedAmount = 0;

    private void Start()
    {
        protestorProgessImage.rectTransform.sizeDelta = new Vector2(effectRange * 2, effectRange * 2);
        policeProgessImage.rectTransform.sizeDelta = new Vector2(effectRange * 2, effectRange * 2);
        UpdateFillAmounts();
    }

    private void UpdateFillAmounts()
    {
        protestorProgessImage.fillAmount = capturedAmount / 100f;
        policeProgessImage.fillAmount = 1 - capturedAmount / 100f;
    }

    private void Update()
    {
        int policeAmount = Physics.OverlapSphereNonAlloc(transform.position, effectRange, output, policeLayer);
        int protestorAmount = Physics.OverlapSphereNonAlloc(transform.position, effectRange, output, protestorLayer);
        if (policeAmount > 0 && protestorAmount == 0 && capturedAmount > 0)
        {
            UpdateCaptureAmount(-changePerSecond * 2 * policeAmount);

        }
        else if (protestorAmount > 0 && policeAmount == 0 && capturedAmount < 100)
        {
            UpdateCaptureAmount(changePerSecond * protestorAmount);
        }

    }
    private void UpdateCaptureAmount(float amount)
    {
        capturedAmount += amount * Time.deltaTime;
        capturedAmount = Mathf.Clamp(capturedAmount,0,100);
        UpdateFillAmounts();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, 1.5f);

        Gizmos.DrawWireSphere(transform.position, effectRange);
    }
}
