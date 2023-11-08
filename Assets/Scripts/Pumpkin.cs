using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.ComponentModel;

public class Pumpkin : MonoBehaviour
{
      [Category("Base Objects")]
    [SerializeField] private Transform pump; 

    [SerializeField] private Transform pumpLid; 

    [SerializeField] Button chestButton;

    [Category("Animation Values")]

    
    [SerializeField] private Vector3 spinTargetRotation;
    [SerializeField] private float spinAnimationDuration;
    [SerializeField] private Vector3 openTargetPosition;
    [SerializeField] private float openAnimationDuration;
    [SerializeField] Vector3 pumpLidPos;
     [SerializeField] private Vector3 lidOpenTargetRotation;
     [SerializeField] private Vector3 lidOpenTargetPosition;
    [SerializeField] private float lidOpenAnimationDuration;
    [SerializeField] private Vector3 openTargetScale;
    [SerializeField] private float shakeAnimationDuration;
    [SerializeField] private float shakeAnimationStrength;


    [Category("Other Scripts")]
     [SerializeField] Outline outlineScript;
     [SerializeField] EventScript eventScript;
    [SerializeField] GameController controller;
    
    

    [Category("Default Values for Pumpkin")]
    public float defaultPos{get; private set;}
    public float defaultRot{get; private set;}
    public float defaultScl{get; private set;}


    

    [Category("Sounds")]
    
    
    [SerializeField] AudioClip buttonPress;

    [SerializeField] AudioClip mysteryTone;


    [SerializeField] AudioClip hover;

    [SerializeField] AudioClip click;

    [Category("Values for Status Changes and Checks")]

    float lastClick = 0; //value used to stop spamming.
    [SerializeField] int pumpNum; //specfic number of pumpkin chest to allow for specfic changes made in selectable array.
    static public bool hoveredOnce = false; //if a pumpkin has been hovered on already.
    static public bool clicked = false; //if a pumpkin has currently been clicked.
    private bool hovering = false;  //bool for if the pumpkin is currently being hovered on.
    private bool shakeDone = false; //bool for if the pumpkin has finished its shake animation.

    
    void Start()
    {
        //making sure lid of chest is in its correct location.
        pumpLid.localRotation = Quaternion.identity;
        pumpLid.localPosition = pumpLidPos;
        //starts each chest spinning and with the outlines turned off.
        SpinChest();
        outlineScript.OutlineWidth = 0;
        

        
    }


    void Update()
    {
        if(clicked == true) //if statment to double check and make sure all chest buttons are disabled on click
        {
            controller.DisableChestButtons();
        }
    }


    public void Hover()
    {
            if(clicked == false)
            {
                if(chestButton.interactable == true)
                {
                    //only plays if button is interactable
                    SoundManager.Instance.PlaySound(hover);
                }
                
                //upon pointer enter on button method is called and if a pumpkin has not already been clicked currently will call shake animation and turn on (increase scale of) outline.
            
                hoveredOnce = true;
                outlineScript.OutlineWidth = 10;
                
                hovering = true;
                Shake();
            }
            
        
        
        
    }

    public void Click()
    {
        
        //Method on click of pumpkin button.
       
       //if statment for spamming prevention.



      
        if (lastClick > (Time.time - 1f))
        {
            Debug.Log("stopped");
            return;
            

        } 
	    lastClick = Time.time;

        

        
        if(clicked == false)
        {
            
            SoundManager.Instance.PlaySound(click);
            clicked = true;
            Debug.Log("notStopped");
            //if a pumpkin has not already been clicked stop all animation on pumpkin turn off outline and start open animation.
           // pump.transform.DOKill();
            outlineScript.OutlineWidth = 0;
           
            controller.DisableChestButtons();
            
            Open();
        }


        
        
        
       
        
    }

    public void HoverLeave()
    {


            if (clicked == false)
            {
                //method called on pointer leave of chest button.
                //turns off outline and calls shake leave method that makes that pumpkin spin again.
                outlineScript.OutlineWidth = 0;
           
                hovering = false;
                ShakeLeave();
            }
            
        
        
    }

    private void Open()
    {
        //method for opening pumpkin chest.
        
        pump.transform.DOKill(); //double checking to make sure all animations have stopped on clicked pumpkin chest.
        
        pump.localRotation = Quaternion.identity; //setting pumpkin rotation to default starting rot.
        
        ResetLid(); //method for reseting lid to original position.
        
        //moves chest to middle of screen, scaling it up and does premptive spin for rotating lid to open chest.
        pump.DOMove(openTargetPosition, openAnimationDuration).SetEase(Ease.Linear); 
        
        pump.DOScale(openTargetScale, openAnimationDuration).SetEase(Ease.Linear).OnComplete(()=>{

            SoundManager.Instance.PlaySound(mysteryTone);
            pump.DORotate(spinTargetRotation, 0.75f, RotateMode.FastBeyond360).SetEase(Ease.Linear).OnComplete(()=>{

                //if statment is here if I wanted to make animaiton changes between pooper or default candy collection animation, but not current differnece at the moment.
                if (controller.NextIsPooper())
                {
                    pumpLid.DOMove(lidOpenTargetPosition, lidOpenAnimationDuration).SetEase(Ease.Linear);
                    pumpLid.DOLocalRotate(lidOpenTargetRotation, lidOpenAnimationDuration).SetEase(Ease.Linear).OnComplete(()=>{  
                                     
                        controller.ChestClick(this);  //calls values that pulls next value in chest amounts.

                    });         
                }
                else
                {
                    pumpLid.DOMove(lidOpenTargetPosition, lidOpenAnimationDuration).SetEase(Ease.Linear);
                    pumpLid.DOLocalRotate(lidOpenTargetRotation, lidOpenAnimationDuration).SetEase(Ease.Linear).OnComplete(()=>{  
                                    
                        controller.ChestClick(this);  //calls values that pulls next value in chest amounts.

                    });


                  

                }
                
                

            });
            
        });


        
    }

    public void Restart()
    {
        //method for restarting values for pumpkin chest. Making sure the do not stay in hover upon the click of another chest.
        HoverLeave();
        pump.localScale = Vector3.one;
        pump.localPosition = Vector3.zero;
        pump.localRotation = Quaternion.identity;
        ResetLid();
        SpinChest();
    }


    public void CollectionEnd()
    {
        //method called upon end of candy collection.
        //removes pumpkin from panel and turns off this pumpkin's specfic button.
        HoverLeave();
        ButtonOff();
        pump.transform.DOKill();
        controller.RemoveFromSelectable(pumpNum);
        MoveChestDown();
        
       
        
        
        
        
    }

    
    

    


    void MoveChestDown()
    {
        //Animation for when candy collection is complete.
        pump.DOMove(new Vector3(0f, -12f, -2f), 0.4f, false).SetEase(Ease.Linear).OnComplete(()=>{

            
            this.gameObject.SetActive(false);
            clicked = false;
            controller.EnableChestButtons();
            

        });
    }




    private void ResetLid()
    {
        //resets position and rotation of chest's lid
        pumpLid.localRotation = Quaternion.identity;
        pumpLid.localPosition = pumpLidPos;
    }

    private void Shake()
    {   
        //method for shake animation upon hover or selection.
        pump.transform.DOKill(); // stopping all current animations
        shakeDone = false; 
        pump.localScale = Vector3.one;
        pump.localPosition = Vector3.zero;
        pump.localRotation = Quaternion.identity;
        ResetLid();
        pump.DOShakePosition(shakeAnimationDuration, shakeAnimationStrength);
        pump.DOShakeRotation(shakeAnimationDuration, shakeAnimationStrength);
        pump.DOShakeScale(shakeAnimationDuration, shakeAnimationStrength).OnComplete(()=> {
            
            pump.localScale = Vector3.one;
            pump.localPosition = Vector3.zero;
            pump.localRotation = Quaternion.identity;
            ResetLid();
            
            if(hovering == false)
            {
                //if hovering stops reset animaitons and start spin animation.
                pump.transform.DOKill();
                SpinChest();

               
                
            }
            
            shakeDone = true; //tells shake leave method whether the shake animation has finished.
            
            });
        

    }
    private void ShakeLeave()
    {
       
        
        if(shakeDone == true)
        {
            //if shake animation finished and pointer exited button start spinning again.
            
            SpinChest();
            shakeDone = false;
        }
        
        
    }

    private void SpinChest()
    {

        
         
           
         
            shakeDone = false; //double checking that shake done is turn off, as the start if spinning means it must be.
            outlineScript.OutlineWidth = 0; //Also making sure outline is turned off if chest starts spinning.
           pump.transform.DOKill(); //stops all current animations before it starts to spin.
            pump.DORotate(spinTargetRotation, spinAnimationDuration, RotateMode.FastBeyond360).SetLoops(-1,LoopType.Restart).SetEase(Ease.Linear); //infinitly loops until animation is killed.
        
        
        
    }


    public void ButtonOn()
    {
        chestButton.interactable = true;
    }

    public void ButtonOff()
    {
        HoverLeave();//calls hover leave to make sure pumpkin values are reset upon next play.
        chestButton.interactable = false;
    }
    


}
