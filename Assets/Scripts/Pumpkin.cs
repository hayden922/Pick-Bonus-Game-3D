using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Pumpkin : MonoBehaviour
{

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
        

        
    }

    
    void Update()
    {
     
    }

    public void Hover()
    {
            if(clicked == false)
            {
                
                hovering = true;
                Shake();
            }
            
        
        
        
    }

    public void Click()
    {
        
        Debug.Log("hello");
        if (lastClick > (Time.time - 1f))
        {
            return;
        } 
	    lastClick = Time.time;


        
        if(clicked == false)
        {
            Debug.Log("was false");
            controller.DisableChestButtons();
            clicked = true;
            Open();
        }
        
       
        
    }

    public void HoverLeave()
    {
        if (clicked == false)
        {
            Debug.Log("left hover");
            hovering = false;
            ShakeLeave();
        }
        
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
                        Debug.Log("hello");             
                        controller.ChestClick(this);  

                    });


                   
                  

                    
                }
                else
                {
                    pumpLid.DOMove(lidOpenTargetPosition, lidOpenAnimationDuration).SetEase(Ease.Linear);
                    pumpLid.DOLocalRotate(lidOpenTargetRotation, lidOpenAnimationDuration).SetEase(Ease.Linear).OnComplete(()=>{  
                        Debug.Log("hello");             
                        controller.ChestClick(this);  

                    });


                  

                }
                
                

            });
            
        });


        
    }

    public void Restart()
    {
        pump.transform.DOKill();
        pump.localScale = Vector3.one;
        pump.localPosition = Vector3.zero;
        pump.localRotation = Quaternion.identity;
        ResetLid();
        SpinChest();
    }


    public void CollectionEnd()
    {
        
        HoverLeave();
        ButtonOff();
        clicked = false;
        pump.transform.DOKill();
        //pump.localScale = Vector3.one;
        //pump.localPosition = Vector3.zero;
        //pump.localRotation = Quaternion.identity;
        //ResetLid();
       // SpinChest();
        controller.RemoveFromSelectable(pumpNum);
        this.gameObject.SetActive(false);
        controller.EnableChestButtons();
        eventScript.SwitchToPumpkins();
        
        
    }


    private void ResetLid()
    {
        pumpLid.localRotation = Quaternion.identity;
        pumpLid.localPosition = pumpLidPos;
    }

    private void Shake()
    {   

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
            pump.transform.DOKill();
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
            pump.transform.DOKill();
            SpinChest();
            shakeDone = false;
        }
        
        
    }

    private void SpinChest()
    {

        if(clicked == false)
        {
            pump.transform.DOKill();
            pump.DORotate(spinTargetRotation, spinAnimationDuration, RotateMode.FastBeyond360).SetLoops(-1,LoopType.Restart).SetEase(Ease.Linear);
        }
        
        
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
