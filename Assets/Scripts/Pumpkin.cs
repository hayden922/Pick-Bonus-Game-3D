using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Pumpkin : MonoBehaviour
{


     [SerializeField] Outline outlineScript;
     [SerializeField] EventScript eventScript;
    [SerializeField] GameController controller;
    [SerializeField] private Transform pump; 

    [SerializeField] private Transform pumpLid; 
    [SerializeField] private Vector3 spinTargetRotation;

    [SerializeField] private float spinAnimationDuration;

    [SerializeField] private Vector3 openTargetPosition;

    [SerializeField] private float openAnimationDuration;

     [SerializeField] private Vector3 lidOpenTargetRotation;

     [SerializeField] private Vector3 lidOpenTargetPosition;

    [SerializeField] private float lidOpenAnimationDuration;

    [SerializeField] private Vector3 openTargetScale;

    
    public float defaultPos{get; private set;}
    public float defaultRot{get; private set;}
    public float defaultScl{get; private set;}

    
    float lastClick = 0;

    [SerializeField] private float shakeAnimationDuration;
    [SerializeField] private float shakeAnimationStrength;

    [SerializeField] Button chestButton;

    [SerializeField] Vector3 pumpLidPos;

    [SerializeField] int pumpNum;


    static public bool hoveredOnce = false; 
     
   

    private bool hovering = false;
    private bool shakeDone = false;

    static public bool clicked = false;
    
    void Awake()
    {
        

    }
    
    void Start()
    {
        pumpLid.localRotation = Quaternion.identity;
        pumpLid.localPosition = pumpLidPos;
        SpinChest();
        outlineScript.OutlineWidth = 0;
        

        
    }

    
    void Update()
    {

        
     
    }

    public void Hover()
    {
            if(clicked == false)
            {
                var foundPumpkins = FindObjectsOfType<Pumpkin>();
               // for (int i = 0; i < foundPumpkins.Length; ++i)
              //  {
              //      foundPumpkins[i].HoverLeave();
//
              // }
                hoveredOnce = true;
                outlineScript.OutlineWidth = 10;
                
                hovering = true;
                Shake();
            }
            
        
        
        
    }

    public void Click()
    {
        
       
        if (lastClick > (Time.time - 1f))
        {
            return;
        } 
	    lastClick = Time.time;


        
        if(clicked == false)
        {
            pump.transform.DOKill();
            outlineScript.OutlineWidth = 0;
           
            controller.DisableChestButtons();
            clicked = true;
            Open();
        }
        
       
        
    }

    public void HoverLeave()
    {
        
            outlineScript.OutlineWidth = 0;
           
            hovering = false;
            ShakeLeave();
        
        
    }

    private void Open()
    {
        
        pump.transform.DOKill();
        
        pump.localRotation = Quaternion.identity;
        
        ResetLid();
        
        pump.DOMove(openTargetPosition, openAnimationDuration).SetEase(Ease.Linear);
        
        pump.DOScale(openTargetScale, openAnimationDuration).SetEase(Ease.Linear).OnComplete(()=>{

            pump.DORotate(spinTargetRotation, 0.75f, RotateMode.FastBeyond360).SetEase(Ease.Linear).OnComplete(()=>{

                if (controller.NextIsPooper())
                {
                    pumpLid.DOMove(lidOpenTargetPosition, lidOpenAnimationDuration).SetEase(Ease.Linear);
                    pumpLid.DOLocalRotate(lidOpenTargetRotation, lidOpenAnimationDuration).SetEase(Ease.Linear).OnComplete(()=>{  
                                     
                        controller.ChestClick(this);  

                    });


                   
                  

                    
                }
                else
                {
                    pumpLid.DOMove(lidOpenTargetPosition, lidOpenAnimationDuration).SetEase(Ease.Linear);
                    pumpLid.DOLocalRotate(lidOpenTargetRotation, lidOpenAnimationDuration).SetEase(Ease.Linear).OnComplete(()=>{  
                                    
                        controller.ChestClick(this);  

                    });


                  

                }
                
                

            });
            
        });


        
    }

    public void Restart()
    {
        
        HoverLeave();
        pump.localScale = Vector3.one;
        pump.localPosition = Vector3.zero;
        pump.localRotation = Quaternion.identity;
        ResetLid();
        SpinChest();
    }


    public void CollectionEnd()
    {
        clicked = false;
        HoverLeave();
        ButtonOff();
        pump.transform.DOKill();
       
        controller.RemoveFromSelectable(pumpNum);
        this.gameObject.SetActive(false);
        controller.EnableChestButtons();
        
        
        
    }


    private void ResetLid()
    {
        pumpLid.localRotation = Quaternion.identity;
        pumpLid.localPosition = pumpLidPos;
    }

    private void Shake()
    {   
        pump.transform.DOKill();
        shakeDone = false;
        pump.localScale = Vector3.one;
        pump.localPosition = Vector3.zero;
        pump.localRotation = Quaternion.identity;
        ResetLid();
        pump.transform.DOKill();
        pump.DOShakePosition(shakeAnimationDuration, shakeAnimationStrength);
        pump.DOShakeRotation(shakeAnimationDuration, shakeAnimationStrength);
        pump.DOShakeScale(shakeAnimationDuration, shakeAnimationStrength).OnComplete(()=> {
            
            pump.localScale = Vector3.one;
            pump.localPosition = Vector3.zero;
            pump.localRotation = Quaternion.identity;
            ResetLid();
            
            if(hovering == false)
            {

                pump.transform.DOKill();
                SpinChest();

               
                
            }
            shakeDone = true;
            
            });
        

    }
    private void ShakeLeave()
    {
       
        
        if(shakeDone == true)
        {
            
            SpinChest();
            shakeDone = false;
        }
        
        
    }

    private void SpinChest()
    {

        
         
           
         
            shakeDone = false;
            outlineScript.OutlineWidth = 0;
           pump.transform.DOKill();
            pump.DORotate(spinTargetRotation, spinAnimationDuration, RotateMode.FastBeyond360).SetLoops(-1,LoopType.Restart).SetEase(Ease.Linear);
        
        
        
    }


    public void ButtonOn()
    {
        chestButton.interactable = true;
    }

    public void ButtonOff()
    {
        HoverLeave();
        chestButton.interactable = false;
    }
    


}
