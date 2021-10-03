﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using TMPro.EditorUtilities;
using UnityEngine.Events;

public class Tutorial : MonoBehaviour
{
    public static Tutorial instance;

    [SerializeField]private List<TutorialStep> _tutorialSteps = new List<TutorialStep>();
    [SerializeField]private CanvasGroup canvasGroup;
    [SerializeField]private TMP_Text title;
    [SerializeField]private TMP_Text description;
    [SerializeField]private Image iconImage;
    [SerializeField] private Sprite defaultIcon;

    private RectTransform rect;

    public UnityEvent onClose;
    
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        rect = GetComponent<RectTransform>();
        
        onClose.AddListener(OpenNextTutorial);
        OpenNextTutorial();
    }


    public void OpenTutorial()
    {
        canvasGroup.interactable = true;
        canvasGroup.DOFade(1, 0.3f);
    }
    public void OpenTutorial(string title,string description, Sprite icon)
    {
        SetTitle(title);
        SetDescription(description);
        SetIcon(icon);
        OpenTutorial();
    }
    

    public void CloseTutorial()
    {
        canvasGroup.interactable = false;
        canvasGroup.DOFade(0, 0.3f).OnComplete(()=>onClose?.Invoke());
    }

    public void SetIcon(Sprite sprite)
    {
        if (sprite == null)
            sprite = defaultIcon;
        
        iconImage.sprite = sprite;
    }

    public void SetTitle(string titleText)
    {
        title.text = titleText;
    }

    public void SetDescription(string descriptionText)
    {
        description.text = descriptionText;
    }

    public void AddTutorialStep(TutorialStep tutorialStep)
    {
        _tutorialSteps.Add(tutorialStep);
    }

    public void OpenNextTutorial()
    {
        if(_tutorialSteps.Count==0)
            return;
        
        _tutorialSteps[0].Apply();
        _tutorialSteps.RemoveAt(0);
    }

    public void SkipTutorial()
    {
        _tutorialSteps.Clear();
    }
    
    [System.Serializable]
    public class TutorialStep
    {
        public string title;
        [TextArea]
        public string description;
        public Sprite icon;

        public void Apply()
        {
            Tutorial.instance.OpenTutorial(title,description,icon);
        }
    }
    

}