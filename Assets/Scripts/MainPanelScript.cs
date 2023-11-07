using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using JetBrains.Annotations;
using TMPro;
using System;
using DG.Tweening;
using System.Threading;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class MainPanelScript : MonoBehaviour
{
    [SerializeField] EventScript eventScript;
    [SerializeField] GameController controller;
    [SerializeField] RectTransform mainPanel;
    [SerializeField] int mainPanelAnimationDuration;
    [SerializeField] CanvasGroup canvasGroup;

    [SerializeField] AudioClip panelEnter;
    
    void Start()
    {

        canvasGroup.alpha = 0f;
        
    }

    public void PanelEnter(Pumpkin starter)
    {   
        //Method called when game starts.
        //Starts animations to move panel onto screen.
        canvasGroup.alpha = 1f;
        mainPanel.transform.localPosition = new Vector3(0f, 1000f, 0f);
        
        mainPanel.DOAnchorPos(new Vector2(0f, 0f), mainPanelAnimationDuration, false).SetEase(Ease.OutBounce).OnComplete(()=> 
        {
            SoundManager.Instance.PlaySound(panelEnter);
        });

        StartCoroutine(PokeStarter(starter));

    }

    IEnumerator PokeStarter(Pumpkin starter)
    {
        //Coroutine to allow for a delay before chests are available to press. 
        //Wanted the timing to be slightly larger than the time it took for panel to come on screen so used coroutine. 
        yield return new WaitForSeconds(0.85f);
        
        
        controller.EnableChestButtons();
    }

    public void PanelLeave()
    {
        //Method for when game ends
        mainPanel.transform.localPosition = new Vector3(0f, 0f, 0f);
        mainPanel.DOAnchorPos(new Vector2(0f, -1000f), mainPanelAnimationDuration, false).SetEase(Ease.InOutExpo).OnComplete(()=>{
            canvasGroup.alpha = 0f;
        });
    }
}
