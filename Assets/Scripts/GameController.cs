using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using JetBrains.Annotations;
using TMPro;
using System;
using DG.Tweening;
using System.Threading;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] ParticleSystem smoke;

    [SerializeField] ParticleSystem bats;

    [SerializeField] EventScript eventScript;
    [SerializeField] Button playButton;

    [SerializeField] Button increaseDenomButton;
      [SerializeField] Button decreaseDenomButton;
    [SerializeField] Transform buttonGroup;
    [SerializeField] MainPanelScript mainPanel;
    [SerializeField] CandyScript candyScript;
    
    [SerializeField] WinningSolver solver;

    [SerializeField] BalanceScript balance;

    [SerializeField] Pumpkin[] pumpkins;

    

    static public GameObject[] selectablePumpkins =  new GameObject[9];
    
    [SerializeField] TextMeshProUGUI currentDenomText;

    
    [SerializeField] GameObject[] outlines;

     [SerializeField] RectTransform playMenu;

      [SerializeField] CanvasGroup canvasGroup;

     [SerializeField] private Vector3 playMenuTargetPosition; 
     [SerializeField] private float playMenuAnimationDuration;

     

     


    static private float[] possibleDenoms = {0.25f,0.50f,1.00f,5.00f};
    static private float currentDenom {get; set;}
    private int denomValue;

    float winAmount;
    float[] chestAmounts;
    

    float tempAmount;

    int clickAmounts;

    int currentAmount;
    private int DenomValue {
        
        get
        {
            return this.denomValue;
        } 
        set
        {
            if((value >=0) && (value <=3))
            {
                this.denomValue = value;
            }
            else
            {
                Debug.Log("No other denoms");
            }

        }
        
        
        
        }

    



    void Start()
    {
        playButton.interactable = true;
        increaseDenomButton.interactable = true;
        decreaseDenomButton.interactable = true;
        eventScript.SwitchToPlayMenu();
        currentDenom = possibleDenoms[0];
        currentDenomText.text = currentDenom.ToString("C",new CultureInfo("en-US"));
        outlines[denomValue].SetActive(true);
       
        DisableChestButtons();
        for (int i = 0; i < pumpkins.Length; i++)
        {
                pumpkins[i].gameObject.SetActive(false);
                pumpkins[i].ButtonOn();
                
        }

        
        

     
        
    }

    
    void FixedUpdate()
    {

        
    }


   

    public void Play()
    {
        
       
        
        
        if (currentDenom > BalanceScript.currentBalance)
        {

           Debug.Log("Not Enough Balance");

        }
        else
        {
             for (int i = 0; i < selectablePumpkins.Length; i++)
             {
                

                selectablePumpkins[i] = buttonGroup.transform.GetChild(i).gameObject;;
             }
            
            

            




            playButton.interactable = false;
            increaseDenomButton.interactable = false;
            decreaseDenomButton.interactable = false;
            
            balance.ResetLastWin();
            currentAmount = 0;
            winAmount = 0;
            balance.SubtractFromBalance(currentDenom);
             Debug.Log("Able to Play");
            
            

            for (int i = 0; i < pumpkins.Length; i++)
            {
                pumpkins[i].gameObject.SetActive(true);
                pumpkins[i].Restart();
                

            }
            

            canvasGroup.alpha = 1f;
            playMenu.transform.localPosition = new Vector3(0f, 0f, 0f);
            playMenu.DOAnchorPos(new Vector2(0f, -1000f), playMenuAnimationDuration, false).SetEase(Ease.InOutQuint);
            canvasGroup.DOFade(0, playMenuAnimationDuration);

            mainPanel.PanelEnter(pumpkins[0]);
            
           

             



            float multi = solver.FindMultiplier();
             Debug.Log("multi");
            Debug.Log(multi);
            if(multi == 0)
            {
                Debug.Log("LOST");
                clickAmounts = -1;

            }
            else
            {
                 (winAmount, chestAmounts) = solver.DecideWinAmounts(currentDenom);
                 clickAmounts = chestAmounts.Length;
            }

            

           
        }
        
        //eventScript.SwitchToPumpkins();
        
        
    }


    


    public void DisableChestButtons()
    {
       buttonGroup.gameObject.SetActive(false);
        
    }

    public void EnableChestButtons()
    {

       buttonGroup.gameObject.SetActive(true);
        
    }

    public bool NextIsPooper()
    {

        return(currentAmount >= clickAmounts);
    }


    IEnumerator BatsSpawn(Pumpkin currentChest)
    {
        smoke.Play();
        yield return new WaitForSeconds(0.5f);
        bats.Play();
        yield return new WaitForSeconds(1.2f);
        currentChest.CollectionEnd();
        GameEnd();
         
    }



    public void ChestClick(Pumpkin currentChest)
    {
        
        
        DisableChestButtons();
        
        if(currentAmount >= clickAmounts)
        {
            
            
            StartCoroutine(BatsSpawn(currentChest));

            Debug.Log("Pooper");
            
        }
        else
        {
            Debug.Log(chestAmounts[currentAmount]);

            //balance.AddToBalance(chestAmounts[currentAmount]);

            candyScript.CollectCandy(chestAmounts[currentAmount], currentChest);

            currentAmount+=1;
        }

        

                
            
    }




    public void IncreaseDenom()
    {
        if(DenomValue != DenomValue+1)
        {
        DenomValue+=1;
        currentDenom = possibleDenoms[denomValue];
        outlines[denomValue-1].SetActive(false);
        outlines[denomValue].SetActive(true);
        currentDenomText.text = currentDenom.ToString("C",new CultureInfo("en-US"));
        
        }
    }

    public void DecreaseDenom()
    {
        if(DenomValue != DenomValue-1)
        {
             DenomValue-=1;
        currentDenom = possibleDenoms[denomValue];
        outlines[denomValue+1].SetActive(false);
        outlines[denomValue].SetActive(true);
        currentDenomText.text = currentDenom.ToString("C",new CultureInfo("en-US"));
        
        }
       
    }


    public void RemoveFromSelectable(int number)
    {
        Debug.Log("removeSelectable");
        selectablePumpkins[number-1] = null;
        
    }

   
    public void GameEnd()
    {
            balance.AddToBalance(winAmount);
            
            DisableChestButtons();
            for (int i = 0; i < pumpkins.Length; i++)
            {
                pumpkins[i].gameObject.SetActive(false);
                pumpkins[i].ButtonOn();
                
                

            }
            canvasGroup.alpha = 0f;
            playMenu.transform.localPosition = new Vector3(0f, -1000f, 0f);
            playMenu.DOAnchorPos(new Vector2(0f, 0f), playMenuAnimationDuration, false).SetEase(Ease.OutElastic);
            canvasGroup.DOFade(1, playMenuAnimationDuration).OnComplete(()=> 
            {
                playButton.interactable = true;
                increaseDenomButton.interactable = true;
                decreaseDenomButton.interactable = true;
                eventScript.SwitchToPlayMenu();
            });
            mainPanel.PanelLeave();
            


    }


}
