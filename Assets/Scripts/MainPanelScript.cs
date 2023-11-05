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

public class MainPanelScript : MonoBehaviour
{
    [SerializeField] EventScript eventScript;
    [SerializeField] GameController controller;
    [SerializeField] RectTransform mainPanel;

    [SerializeField] int mainPanelAnimationDuration;
    [SerializeField] CanvasGroup canvasGroup;
    // Start is called before the first frame update
    void Start()
    {

        canvasGroup.alpha = 0f;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PanelEnter(Pumpkin starter)
    {   
        canvasGroup.alpha = 1f;
        mainPanel.transform.localPosition = new Vector3(0f, 1000f, 0f);
        
        mainPanel.DOAnchorPos(new Vector2(0f, 0f), mainPanelAnimationDuration, false).SetEase(Ease.OutBounce);

        StartCoroutine(PokeStarter(starter));
            
       
        

       // canvasGroup.DOFade(1, mainPanelAnimationDuration);

    }

    IEnumerator PokeStarter(Pumpkin starter)
    {
        yield return new WaitForSeconds(0.85f);
        
        
        controller.EnableChestButtons();
        /*
        yield return new WaitForSeconds(0.1f);
        if(Pumpkin.hoveredOnce == false)
        {
            eventScript.SwitchToNull();

        }
        eventScript.SwitchToPumpkins();
        */
        
        
        //starter.Hover();
    }

    public void PanelLeave()
    {
        
        mainPanel.transform.localPosition = new Vector3(0f, 0f, 0f);
        mainPanel.DOAnchorPos(new Vector2(0f, -1000f), mainPanelAnimationDuration, false).SetEase(Ease.InOutExpo).OnComplete(()=>{
            canvasGroup.alpha = 0f;
        });
       // canvasGroup.DOFade(0, mainPanelAnimationDuration);
    }
}
