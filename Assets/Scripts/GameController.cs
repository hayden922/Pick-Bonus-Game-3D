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
using System.ComponentModel;

public class GameController : MonoBehaviour
{


    [SerializeField] EventScript eventScript;

    [Category("ParticleSystems")]
    [SerializeField] ParticleSystem smoke;
    [SerializeField] ParticleSystem bats;

    [Category("Sounds")]

    [SerializeField] AudioClip batSound;
    [SerializeField] AudioClip smokeSound;


    
    [Category("Play Menu and Denominations")]
    [SerializeField] Button playButton;
    [SerializeField] Button increaseDenomButton;
    [SerializeField] Button decreaseDenomButton;
    [SerializeField] TextMeshProUGUI currentDenomText;
    [SerializeField] GameObject[] outlines; //used to show currently selected denom. (Not shown in current version)
    [SerializeField] RectTransform playMenu;
    [SerializeField] private Vector3 playMenuTargetPosition; 
    [SerializeField] private float playMenuAnimationDuration;
    [SerializeField] CanvasGroup playMenuCanvasGroup;
    static private float[] possibleDenoms = {0.25f,0.50f,1.00f,5.00f};
    static private float currentDenom {get; set;}
    private int denomValue;
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


    [Category("In-Game")]
    [SerializeField] Transform buttonGroup; //transform of parent of all pumpkin buttons.
    [SerializeField] MainPanelScript mainPanel; //panel containing pumpkins for in-game.
    [SerializeField] CandyScript candyScript;
    [SerializeField] WinningSolver solver; 
    [SerializeField] BalanceScript balance;
    [SerializeField] Pumpkin[] pumpkins; //array of all pumpkins with Pumpkin script.
    static public GameObject[] selectablePumpkins =  new GameObject[9]; //array used to check whether pumpkin button is selectable when using keyboard navigation.
     static public bool inGame = false;
    float winAmount;
    float[] chestAmounts;
    float tempAmount;
    int clickAmounts;
    int currentAmount;

    void Start()
    {
        playButton.interactable = true; //making sure play menu buttons are interactable.
        increaseDenomButton.interactable = true;
        decreaseDenomButton.interactable = true;
        
        //starting current denom selection at 0.25
        currentDenom = possibleDenoms[0]; 
        currentDenomText.text = currentDenom.ToString("C",new CultureInfo("en-US"));
        outlines[denomValue].SetActive(true);
       //making sure in-Game chest buttons are all disabled and pumpkins are not shown.
        DisableChestButtons(); 
        for (int i = 0; i < pumpkins.Length; i++)
        {
                pumpkins[i].gameObject.SetActive(false);
                pumpkins[i].ButtonOn();
                
        }

        
        

     
        
    }
   

    public void Play()
    {
        
       
        //Method attached to play button. Initiates main game.
        
        if (currentDenom > BalanceScript.currentBalance)
        {
            //game does not start if user does not have enough balance.

           Debug.Log("Not Enough Balance");

        }
        else
        {
            //sets user to in game setting correct values.
            inGame = true;
             for (int i = 0; i < selectablePumpkins.Length; i++)
             {
                

                selectablePumpkins[i] = buttonGroup.transform.GetChild(i).gameObject;;
             }
            
            

            



            //stops user from selecting play menu buttons
            playButton.interactable = false;
            increaseDenomButton.interactable = false;
            decreaseDenomButton.interactable = false;
            //resets value of last win to 0 and makes sure all win amounts are reset from previous play.
            balance.CheckBalance();
            balance.ResetLastWin();
            currentAmount = 0;
            winAmount = 0;
            balance.SubtractFromBalance(currentDenom); //subtracts selected cost to play.
             Debug.Log("Able to Play");
            
            

            for (int i = 0; i < pumpkins.Length; i++)
            {
                //for loops activates every pumpkin and resets their positions and animations.
                pumpkins[i].gameObject.SetActive(true);
                pumpkins[i].Restart();
                

            }
            

            playMenuCanvasGroup.alpha = 1f;
            playMenu.transform.localPosition = new Vector3(0f, 0f, 0f);
            playMenu.DOAnchorPos(new Vector2(0f, -1000f), playMenuAnimationDuration, false).SetEase(Ease.InOutQuint); //play menu fades and move off screen.
            playMenuCanvasGroup.DOFade(0, playMenuAnimationDuration);

            mainPanel.PanelEnter(pumpkins[0]); //method call in main panel script to move pumpkins (in-game panel) onto screen.
            
           

             



            float multi = solver.FindMultiplier(); //calls method to find random multiplier for player.
             Debug.Log("multi");
            Debug.Log(multi);
            if(multi == 0)
            {    
                //if multi is 0 it means the first clicked chest will be a pooper (user loses)           
                Debug.Log("LOST");
                clickAmounts = -1; //setting to -1 allows for if statment later to tell that first one will be pooper.

            }
            else
            {
                 (winAmount, chestAmounts) = solver.DecideWinAmounts(currentDenom); //if multiplier is not 0 call method to decide the win amounts and what amounts will be found each chest click.
                 clickAmounts = chestAmounts.Length; //list of chest amounts is the total amount of times user will be able to open chests before they get click and get a pooper.
            }

            

           
        }
        EnableChestButtons();
        Pumpkin.clicked = false;
        Pumpkin.hoveredOnce = false; //bool to check if player has already hovered over a chest before it autoselects the first one. 
        
        
    }


    


    public void DisableChestButtons()
    {
        //disable every pumpkin chest button.
       buttonGroup.gameObject.SetActive(false);
        
    }

    public void EnableChestButtons()
    {
        //enable every pumpkin chest button.
       buttonGroup.gameObject.SetActive(true);
        
    }

    public bool NextIsPooper()
    {
        //method to test if the next chest the player will click is a pooper, allows for different animations in pumpkin script.
        return(currentAmount >= clickAmounts);
    }


    IEnumerator BatsSpawn(Pumpkin currentChest)
    {
        //coroutine method plays two particle systems used when player gets pooper and ends game. 
        SoundManager.Instance.PlaySound(smokeSound);
        smoke.Play();
        yield return new WaitForSeconds(0.5f);
        SoundManager.Instance.PlaySound(batSound);
        bats.Play();
        yield return new WaitForSeconds(1.2f);
        currentChest.CollectionEnd();
        GameEnd();
         
    }



    public void ChestClick(Pumpkin currentChest)
    {
        //method called after pumpkin click animation is complete.

        
        DisableChestButtons();
        
        if(currentAmount >= clickAmounts)
        {
            //if pooper start pooper animation
            
            StartCoroutine(BatsSpawn(currentChest));

            Debug.Log("Pooper");
            
        }
        else
        {
            //if not pooper call method to spawn and move candy to last win number.
            Debug.Log(chestAmounts[currentAmount]);

            //balance.AddToBalance(chestAmounts[currentAmount]);

            candyScript.CollectCandy(chestAmounts[currentAmount], currentChest);

            currentAmount+=1;
        }

        

                
            
    }




    public void IncreaseDenom()
    {
        //method attached to button to increase current denomination in play menu
        //because denom value cannot exceed 3, current denom does not change if at max possible denom value.
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
        //method attached to button to decrease current denomination in play menu
        //because denom value cannot go below 0, current denom does not change if at minimum possible denom value.
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
        //method to to remove pumpkin from selectable pumpkins to allow for event system to find the first possible selectable pumpkin when navigating with keyboard.
        Debug.Log("removeSelectable");
        selectablePumpkins[number-1] = null;
        
    }

   
    public void GameEnd()
    {
            //method called upon getting pooper.
            //adds the win amount to balance
            balance.AddToBalance(winAmount);
            Pumpkin.hoveredOnce = false; //resets inital hover checker for next play
            
            //disables chest buttons and all pumpkins.
            DisableChestButtons(); 
            for (int i = 0; i < pumpkins.Length; i++)
            {
                pumpkins[i].gameObject.SetActive(false);
                pumpkins[i].ButtonOn();// just to make sure each button is enabled upon next play.
                
                

            }
            playMenuCanvasGroup.alpha = 0f;
            playMenu.transform.localPosition = new Vector3(0f, -1000f, 0f);
            playMenu.DOAnchorPos(new Vector2(0f, 0f), playMenuAnimationDuration+0.5f, false).SetEase(Ease.OutBounce); //animation to bring play menu back on screen.
            playMenuCanvasGroup.DOFade(1, playMenuAnimationDuration+0.5f).OnComplete(()=> 
            {
                //upon completion of animation enabling all playMenu buttons.
                playButton.interactable = true;
                increaseDenomButton.interactable = true;
                decreaseDenomButton.interactable = true;
                inGame = false;
            });
            mainPanel.PanelLeave(); //calls method for main (in-Game) panel that starts its DO animations to leave screen.
            


    }


}
