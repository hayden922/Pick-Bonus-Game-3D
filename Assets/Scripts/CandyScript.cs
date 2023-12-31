using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using JetBrains.Annotations;
using TMPro;
using System;
using DG.Tweening;
using System.Collections.ObjectModel;
using UnityEngine.UIElements;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine.SocialPlatforms;
using System.Runtime.InteropServices;
using UnityEngine.AI;


public class CandyScript : MonoBehaviour
{

    [field: Header("GameController")]
    [SerializeField] GameController controller;
     
    [field: Header("Chest Amount Text")]
    [SerializeField] float chestAmountAnimationDuration;
    [SerializeField] CanvasGroup chestAmountCanvasGroup;
    [SerializeField] TextMeshProUGUI chestAmountText;
    [SerializeField] RectTransform chestAmountRect;
    RectTransform chestAmountStartingPos;
    Color RedTextColor = new Color(239f / 255f, 76f / 255f, 49f / 255f);
    //"#EF4C31";
    Color GreenTextColor = new Color(173f / 255f, 254f / 255f, 88f / 255f);
    //"#ADFE58"
    
    [field: Header("Multiplier UI")]
    [SerializeField] Vector2 multiplierPosition;
    [SerializeField] TextMeshProUGUI multiUIText;
    [SerializeField]  TextMeshProUGUI multiWinAmountTempText;
    [SerializeField]  CanvasGroup multiWinAmountTempCanvasGroup;
    int multiplierNumber = 0;
    [SerializeField] GameObject[] multiplierUICandy;
    [SerializeField] GameObject  multiplierCandyParent;
    GameObject candyObUI = null;
    Vector3 candyObUIStartingPos;

    [field: Header("Candy Bag")]
    [SerializeField] GameObject candyBag;
    [SerializeField] float candyBagAnimationDuration;
    [SerializeField] float candyBagAnimationStrength;
    Vector3 candyBagDefaultScale;   
    
    [field: Header("Balance")]
    [SerializeField] BalanceScript balance;

    [field: Header("Candy Stats")]
    [SerializeField] private GameObject candyPrefab;
    [SerializeField] private GameObject[] candyPrefabExtras;
    GameObject candyPrefabExtra;
    [SerializeField] int candyAnimationDuration;
     [SerializeField] Transform candyAnimationPosition;
    [SerializeField] private Transform spawnLocation;
    List<GameObject> candy = new List<GameObject>();
    [SerializeField] private Transform parent;
    bool ended = false;

    [field: Header("Sounds")]
    [SerializeField] AudioClip collectionSound;
    [SerializeField] AudioClip woosh;
    [SerializeField] AudioClip winSound;
    [SerializeField] AudioClip candyStick;
    [SerializeField] AudioClip multiNotification;

    void Start()
    {
        candyBagDefaultScale = candyBag.transform.localScale;
        chestAmountCanvasGroup.alpha = 0f;
        chestAmountStartingPos = chestAmountRect;
        chestAmountText.color = RedTextColor;
        
        
    }

    private void EndingWait(Pumpkin currentChest)
    {
        ended = true; //bool checker to make sure this method is called before other code finishes.
        //remove amount gained from chest on screen
        chestAmountCanvasGroup.alpha = 1f;
        chestAmountRect.transform.localPosition = new Vector3(0f, 0f, 0f);
        chestAmountRect.DOAnchorPos(new Vector3(0f,-1000f), chestAmountAnimationDuration).SetEase(Ease.InOutQuint).OnComplete(()=>
        {
            Debug.Log("moving text");
            chestAmountCanvasGroup.alpha = 0f;
            currentChest.CollectionEnd(); //after all candies have move to end position call Collection End on the current chest passed in method parameter. (the currently clicked chest)
            SoundManager.Instance.PlaySound(woosh);
            chestAmountText.color = RedTextColor;  
        }); 
        chestAmountCanvasGroup.DOFade(0f, chestAmountAnimationDuration);
    }

    public void ResetMultiplierUI()
    {
        //resets the multiplier text to 0x and gets rid of candy used to denote multiplier amount.
        multiplierNumber = 0;
        multiUIText.text = multiplierNumber.ToString() + "x";
        multiUIText.gameObject.SetActive(false);
        foreach(GameObject candy in multiplierUICandy)
        {
            candy.SetActive(false);
        }
        candyObUI = null;
    }

    private void MultiplierUI()
    {
        //method used to set multiplier number (2,4,8)
        if(multiplierNumber != 0)
        {
            multiplierNumber *= 2;
        }
        else
        {
            multiplierNumber = 2;
        }
        multiUIText.text = multiplierNumber.ToString() + "x";
        multiUIText.gameObject.transform.DOPunchScale(multiplierCandyParent.transform.localScale*2,0.5f,0,0);

    }

    private void EndingForMultiplier (Pumpkin currentChest)
    {
        //Specific method for multiplier out of chest.
        Vector3 candyScale  =  candyObUI.transform.localScale;
        chestAmountCanvasGroup.alpha = 1f;
        chestAmountRect.transform.localPosition = new Vector3(0f, 0f, 0f);
        //moves candy to multiplier spot.
        candyObUI.transform.DOScale(candyObUI.transform.localScale*1.8f, 0.5f).SetEase(Ease.OutBack);
        candyObUI.transform.DOMove(new Vector3(candyObUIStartingPos.x,candyObUIStartingPos.y,candyObUIStartingPos.z), 0.5f).SetEase(Ease.InOutQuint).OnComplete(()=>
        {
            candyObUI.transform.DOScale(candyScale,0.25f).SetEase(Ease.InCubic).OnComplete(()=>
            {
                if(multiplierNumber == 0)
                {
                    multiUIText.gameObject.SetActive(true);
                }
                MultiplierUI(); //increases multiplier value.
                SoundManager.Instance.PlaySound(candyStick);
                chestAmountCanvasGroup.DOFade(0f, chestAmountAnimationDuration);
                chestAmountRect.DOAnchorPos(new Vector2(0f,-1000f), chestAmountAnimationDuration).SetEase(Ease.InOutQuint).OnComplete(()=>
                {
                    chestAmountCanvasGroup.alpha = 0f;
                    currentChest.CollectionEnd(); //after all candies have move to end position call Collection End on the current chest passed in method parameter. (the currently clicked chest)
                    SoundManager.Instance.PlaySound(woosh);
                    controller.ChangePumpkinColor(currentChest,multiUIText.text); //color for pumpkins change based on multiplier gained.
                }); 
            });
        });
    }

   private async Task MultiplierIncrement(Pumpkin currentChest, WinningSolver.AmountsInList currentAmount)
    {
        //Method for incrementing value, specfically for chestAmount with multiplier applied.
        float incrementTime;

        //specfic incrementTime amounts based on difference increment must make.
        if(Mathf.Abs(currentAmount.Number - (currentAmount.Number/currentAmount.AppliedMultiplier)) > 100f)
        {
            incrementTime = Mathf.Abs(currentAmount.Number - (currentAmount.Number/currentAmount.AppliedMultiplier)) * 0.005f;
        }
        else if(Mathf.Abs(currentAmount.Number - (currentAmount.Number/currentAmount.AppliedMultiplier)) > 25f)
        {
            incrementTime = Mathf.Abs(currentAmount.Number - (currentAmount.Number/currentAmount.AppliedMultiplier)) * 0.01f;
        }
        else if(Mathf.Abs(currentAmount.Number - (currentAmount.Number/currentAmount.AppliedMultiplier)) < 10f)
        {
            incrementTime = Mathf.Abs(currentAmount.Number - (currentAmount.Number/currentAmount.AppliedMultiplier)) * 0.25f;
        }
        else
        {
            incrementTime = Mathf.Abs(currentAmount.Number - (currentAmount.Number/currentAmount.AppliedMultiplier)) * 0.05f;
        }

        float time = 0;
        chestAmountText.text = (currentAmount.Number/currentAmount.AppliedMultiplier).ToString("C",new CultureInfo("en-US"));
        
        while ( time < incrementTime )
        {
            await Task.Yield();
            time += Time.deltaTime;
            float factor = time / incrementTime;
            chestAmountText.text = ((float) Mathf.Lerp(currentAmount.Number/currentAmount.AppliedMultiplier, currentAmount.Number, factor)).ToString("C",new CultureInfo("en-US"));
        }   
    }

    private GameObject[] SpawnCandy(int amount, WinningSolver.AmountsInList currentAmount, Pumpkin currentChest, GameObject candySpawn)
    {
        //Method for spawning candy based on amount given. (Amount gained from win decider).
        candy.Clear(); // clears list of previous candy.

        //varibles for changing size of candy to show larger values and reduce the amount of candy spawned.
        int twentyFiveNum = 0; 
        int oneNum = 0;

        if(amount > 20) //if amount is greater than 20 (20*0.05 = 1 dollar)
        {
            if(amount > 500) //if amount is greater than 500 (500*0.05 = 25 dollar)
            {
                twentyFiveNum = amount / 500;
                amount = amount - twentyFiveNum*500;
            }
            oneNum = amount / 20;
            amount = amount - oneNum*20;
        }

        Debug.Log(twentyFiveNum);
        for (int i = 0; i < twentyFiveNum; i++)
        {
            //for loop for spawning candy that represents 25 dollars.
            GameObject candyInstance = Instantiate(candySpawn, parent);
            float Positionx = (spawnLocation.position.x + UnityEngine.Random.Range(-1f,1f));
            float Positiony = (spawnLocation.position.y + UnityEngine.Random.Range(-1f,1f));
            candyInstance.transform.position = new Vector3(Positionx,Positiony,spawnLocation.position.z); // places each candy in slightly random position so not overlap and ability to see every candy.
            candyInstance.GetComponent<CandyPrefab>().candySize = 25.00f; //allows for script to read value of candy later on.
            candy.Add(candyInstance);
            candyInstance.transform.localScale = new Vector3(2f, 2f, 2f); //sets scale to double base to indicate larger value.
        }

        Debug.Log(oneNum);
        for (int i = 0; i < oneNum; i++)
        {
            //for loop for spawning candy that represents 1 dollars.
            GameObject candyInstance = Instantiate(candySpawn, parent);
            float Positionx = (spawnLocation.position.x + UnityEngine.Random.Range(-1f,1f));
            float Positiony = (spawnLocation.position.y + UnityEngine.Random.Range(-1f,1f));
            candyInstance.transform.position = new Vector3(Positionx,Positiony,spawnLocation.position.z); // places each candy in slightly random position so not overlap and ability to see every candy.
            candyInstance.GetComponent<CandyPrefab>().candySize = 1.00f; //allows for script to read value of candy later on.
            candy.Add(candyInstance);
            candyInstance.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f); //sets scale to slightly larger than base to indicate larger value.
        }

        for (int i = 0; i < amount; i++)
        {

            //for loop for spawning candy that represents 0.05 dollars.
            GameObject candyInstance = Instantiate(candySpawn, parent);
            float Positionx = (spawnLocation.position.x + UnityEngine.Random.Range(-1f,1f));        
            float Positiony = (spawnLocation.position.y + UnityEngine.Random.Range(-1f,1f));
            candyInstance.transform.position = new Vector3(Positionx,Positiony,spawnLocation.position.z); // places each candy in slightly random position so not overlap and ability to see every candy.
            candyInstance.GetComponent<CandyPrefab>().candySize = 0.05f; //allows for script to read value of candy later on.
            candy.Add(candyInstance);
            candyInstance.transform.localScale = new Vector3(1f, 1f, 1f); //sets scale to base.
        }

        GameObject[] candyArray =  candy.ToArray(); // adds spawned candyPrefabs of list candy to array.
        return candyArray;
    }

    private async void BaseCandy(WinningSolver.AmountsInList currentAmount, Pumpkin currentChest)
    {
        //Method for moving candy with no multipliers applied.
        ended = false;
        int amount = System.Convert.ToInt32(currentAmount.Number / 0.05f); //converts amount to int so it can be later used in for loop.
        GameObject[] candyArray = SpawnCandy(amount,currentAmount,currentChest,candyPrefab);
        candyAnimationPosition.position = new Vector3(candyAnimationPosition.position.x, candyAnimationPosition.position.y, spawnLocation.position.z); //making sure candies stay at same z position.
        SoundManager.Instance.PlaySound(winSound); 
        var riseAnim = new List<Task>();

        ChestAmountIn(currentAmount.Number); //method for show amount gained in chest.

        foreach (GameObject candy in candyArray)
        {
            Transform currentCandy = candy.transform; //finds transform of current candy for use in DO calls.
            Vector3 currentScale = currentCandy.localScale; //saves candies current scale  
            currentCandy.localScale = Vector3.zero; //sets candies scale to zero so they are able to go from 0 to currentScale using DoScale.   
            float duration = UnityEngine.Random.Range (.5f, 1.5f); //finds random duration for accomplishing DO calls so candies do not move all at same time.            
            //Moves object up on y axis to look like coming out of pumpkin chest.
            riseAnim.Add(currentCandy.DOMove(new Vector3(currentCandy.position.x,currentCandy.position.y+3f,currentCandy.position.z), .75f).SetEase (Ease.InOutBack).AsyncWaitForCompletion());
            //Scales candy up to current scale.
            riseAnim.Add(currentCandy.DOScale(currentScale, .90f).SetEase(Ease.OutBack).AsyncWaitForCompletion());
        }

        await Task.WhenAll(riseAnim); //when all animations for raising candy are done.
        var moveDone = new List<Task>();

        foreach (GameObject candy in candyArray)
        {   
            float size = candy.GetComponent<CandyPrefab>().candySize; //finds size of candy so script knows what value to add to last win number when object reaches destination.   
            Transform currentCandy = candy.transform; //finds transform of current candy for use in DO calls.
            Vector3 currentScale = currentCandy.localScale; //saves candies current scale       
            float duration = UnityEngine.Random.Range (.5f, 1.5f); //finds random duration for accomplishing DO calls so candies do not move all at same time.            
            currentCandy.DOMove (candyAnimationPosition.position, duration).SetEase (Ease.InOutBack).OnComplete(async()=>
            {                
                //adds value attached to candy to last win value (candyBag) and scales candy down to 0.
                balance.AddToLastWin(size);
                SoundManager.Instance.PlaySound(collectionSound);
                candyBag.transform.DOShakePosition(candyAnimationDuration, candyBagAnimationStrength);
                candyBag.transform.DOShakeRotation(candyAnimationDuration, candyBagAnimationStrength);
                candyBag.transform.DOShakeScale(candyAnimationDuration, candyBagAnimationStrength);
                moveDone.Add(currentCandy.DOScale(0f, 0.3f).SetEase(Ease.OutBack).AsyncWaitForCompletion());

                candyBag.transform.DOScale(candyBagDefaultScale,0.25f).SetEase(Ease.Linear);
                await Task.WhenAll(moveDone); 
                
                if(!ended)
                {
                    //ended bool used so repeat EndingWait calls are not made.
                    candyBag.transform.DOScale(candyBagDefaultScale,0.25f).SetEase(Ease.Linear);
                    EndingWait(currentChest); 
                }
            });                                                                                                      
        }
        
                   
    }
    

    private async void MultiCandy(WinningSolver.AmountsInList currentAmount, Pumpkin currentChest)
    {   
        //Method for moving candy with multipliers applied.  
        ended = false;
        int amount = System.Convert.ToInt32((currentAmount.Number/currentAmount.AppliedMultiplier) / 0.05f); //converts amount to int so it can be later used in for loop.
        GameObject[] firstArray = SpawnCandy(amount,currentAmount,currentChest,candyPrefab);
        candyAnimationPosition.position = new Vector3(candyAnimationPosition.position.x, candyAnimationPosition.position.y, spawnLocation.position.z); //making sure candies stay at same z position.  
        SoundManager.Instance.PlaySound(winSound); 
        var riseAnim = new List<Task>();
        ChestAmountIn(currentAmount.Number/currentAmount.AppliedMultiplier);
        foreach (GameObject candy in firstArray)
        {
            Transform currentCandy = candy.transform; //finds transform of current candy for use in DO calls.
            Vector3 currentScale = currentCandy.localScale; //saves candies current scale       
            currentCandy.localScale = Vector3.zero; //sets candies scale to zero so they are able to go from 0 to currentScale using DoScale.       
            float duration = UnityEngine.Random.Range (.5f, 1.5f); //finds random duration for accomplishing DO calls so candies do not move all at same time.            
            //Moves object up on y axis to look like coming out of pumpkin chest.
            riseAnim.Add(currentCandy.DOMove(new Vector3(currentCandy.position.x,currentCandy.position.y+3f,currentCandy.position.z), .75f).SetEase (Ease.InOutBack).AsyncWaitForCompletion());
            //Scales candy up to current scale.
            riseAnim.Add(currentCandy.DOScale(currentScale, .90f).SetEase(Ease.OutBack).AsyncWaitForCompletion());
        }
        await Task.WhenAll(riseAnim);     
        SoundManager.Instance.PlaySound(multiNotification);
        multiWinAmountTempText.text = multiplierNumber.ToString() + "x";
        //showing temporary multiplier indicator.
        multiWinAmountTempCanvasGroup.alpha = 0f;
        multiWinAmountTempText.transform.DOMove(new Vector2(multiWinAmountTempText.transform.position.x,multiWinAmountTempText.transform.position.y + .75f), 0.5f, false).SetEase(Ease.OutBounce); 
        multiWinAmountTempCanvasGroup.DOFade(1f, 0.5f);    
        chestAmountText.color = GreenTextColor;
        foreach (GameObject candy in multiplierUICandy)
        {
            candy.transform.DOPunchScale(new Vector3(candy.transform.localScale.x,candy.transform.localScale.y,candy.transform.localScale.z*2),.45f,0,0);
        }
        //indicator of multiplier candy use in UI
        await multiUIText.gameObject.transform.DOPunchScale(new Vector3( multiUIText.gameObject.transform.localScale.x, multiUIText.gameObject.transform.localScale.y, multiUIText.gameObject.transform.localScale.z*2),.45f,0,0).AsyncWaitForCompletion();           
        //method for incrementing multiplied amount
        await MultiplierIncrement(currentChest, currentAmount);

        GameObject[] secondArray = SpawnCandy(System.Convert.ToInt32(currentAmount.Number/0.05f) - amount,currentAmount,currentChest,candyPrefabExtra);
        riseAnim = new List<Task>(); 

        foreach (GameObject candy in secondArray)
        {
            Transform currentCandy = candy.transform; //finds transform of current candy for use in DO calls.
            Vector3 currentScale = currentCandy.localScale; //saves candies current scale        
            currentCandy.localScale = Vector3.zero; //sets candies scale to zero so they are able to go from 0 to currentScale using DoScale.               
            float duration = UnityEngine.Random.Range (.5f, 1.5f); //finds random duration for accomplishing DO calls so candies do not move all at same time.               
            //Moves object up on y axis to look like coming out of pumpkin chest.
            riseAnim.Add(currentCandy.DOMove(new Vector3(currentCandy.position.x,currentCandy.position.y+3f,currentCandy.position.z), .75f).SetEase (Ease.InOutBack).AsyncWaitForCompletion());
            //Scales candy up to current scale.
            riseAnim.Add(currentCandy.DOScale(currentScale, .90f).SetEase(Ease.OutBack).AsyncWaitForCompletion());
        }
        await Task.WhenAll(riseAnim);
        GameObject[] combinedCandy = new GameObject[firstArray.Length + secondArray.Length];
        //combines first and second array so they all can move together to candy bag.
        Array.Copy(firstArray, combinedCandy, firstArray.Length);
        Array.Copy(secondArray, 0, combinedCandy, firstArray.Length, secondArray.Length);
        var moveDone = new List<Task>();
        //gets rid of temporary indicator of multiplier applied to chest amount.
        multiWinAmountTempCanvasGroup.alpha = 1f;
        multiWinAmountTempText.transform.DOMove(new Vector2(multiWinAmountTempText.transform.position.x,multiWinAmountTempText.transform.position.y - .75f), 0.5f, false).SetEase(Ease.InOutElastic); 
        multiWinAmountTempCanvasGroup.DOFade(0f, 0.5f);

        foreach (GameObject candy in combinedCandy)
        {                                 
            float size = candy.GetComponent<CandyPrefab>().candySize; //finds size of candy so script knows what value to add to last win number when object reaches destination.   
            Transform currentCandy = candy.transform; //finds transform of current candy for use in DO calls.
            Vector3 currentScale = currentCandy.localScale; //saves candies current scale       
            float duration = UnityEngine.Random.Range (.5f, 1.5f); //finds random duration for accomplishing DO calls so candies do not move all at same time.
            
            currentCandy.DOMove (candyAnimationPosition.position, duration).SetEase (Ease.InOutBack).OnComplete(async()=>
            {               
                //adds value attached to candy to last win value and scales candy down to 0.
                balance.AddToLastWin(size);
                SoundManager.Instance.PlaySound(collectionSound);
                candyBag.transform.DOShakePosition(candyAnimationDuration, candyBagAnimationStrength);
                candyBag.transform.DOShakeRotation(candyAnimationDuration, candyBagAnimationStrength);
                candyBag.transform.DOShakeScale(candyAnimationDuration, candyBagAnimationStrength);
                moveDone.Add(currentCandy.DOScale(0f, 0.3f).SetEase(Ease.OutBack).AsyncWaitForCompletion());        
                candyBag.transform.DOScale(candyBagDefaultScale,0.25f).SetEase(Ease.Linear);    
                
                await Task.WhenAll(moveDone); 
                if(!ended)
                {
                    //ended bool used so repeat EndingWait calls are not made.
                    candyBag.transform.DOScale(candyBagDefaultScale,0.25f).SetEase(Ease.Linear);
                    EndingWait(currentChest); 
                }                            
            });                                                                                                        
        }                         
    }

    private void ChestAmountIn(float amount)
    { 
        //show amount gained from chest on screen
        chestAmountText.text = (amount).ToString("C",new CultureInfo("en-US"));
        chestAmountCanvasGroup.alpha = 0f;
        chestAmountRect.transform.localPosition = new Vector3(0f,+1000f,0f);
        chestAmountRect.DOAnchorPos(new Vector2(0f, 0f), chestAmountAnimationDuration, false).SetEase(Ease.OutBounce); 
        chestAmountCanvasGroup.DOFade(1f, chestAmountAnimationDuration);
    }

    public void CollectCandy(WinningSolver.AmountsInList currentAmount, Pumpkin currentChest)
    {
        //Method for spawning candy and moving it towards last win number.
        for(int i = 0; i < candy.Count; i++)
        {
            //Destroys previously summoned candy.
            Destroy(candy[i]);
        }
        candy.Clear(); // clears list of previous candy.

        if(currentAmount.Multiplier)
        {
            //there is a multiplier in the chest.
            Debug.Log("Multiplier");
            Debug.Log(currentAmount.Multiplier);
            chestAmountText.text = "2x";
            SoundManager.Instance.PlaySound(winSound); 
            //show amount gained from chest on screen
            
            switch (multiplierNumber) //using switch statment for different multiplier candies.
            {
            case 4:
                candyObUI = multiplierUICandy[2];
                candyPrefabExtra = candyPrefabExtras[2];
                break;   
            case 2:
                candyObUI = multiplierUICandy[1];
                candyPrefabExtra = candyPrefabExtras[1];
                break;
            case 0:
                candyObUI = multiplierUICandy[0];
                candyPrefabExtra = candyPrefabExtras[0];
                break;
            default:
                break;
            }
            candyObUIStartingPos = candyObUI.transform.position;
            candyObUI.SetActive(true);
            Transform currentCandy = candyObUI.transform; //finds transform of current candy for use in DO calls.
            Vector3 currentScale = currentCandy.localScale; //saves candies current scale
            currentCandy.position = spawnLocation.position;       
            currentCandy.localScale = Vector3.zero; //sets candies scale to zero so they are able to go from 0 to currentScale using DoScale.   
            float duration = UnityEngine.Random.Range (.5f, 1.5f); //finds random duration for accomplishing DO calls so candies do not move all at same time.
            //Moves object up on y axis to look like coming out of pumpkin chest.
            currentCandy.DOMove(new Vector3(currentCandy.position.x,currentCandy.position.y+3f,currentCandy.position.z), .75f).SetEase (Ease.InOutBack);
            //Scales candy up to current scale.
            currentCandy.DOScale(currentScale, .90f).SetEase(Ease.OutBack);
            chestAmountCanvasGroup.alpha = 0f;
            chestAmountRect.transform.localPosition = new Vector3(0f,+1000f,0f);
            chestAmountRect.DOAnchorPos(new Vector2(0f, 0f), chestAmountAnimationDuration, false).SetEase(Ease.OutBounce); 
            chestAmountCanvasGroup.DOFade(1f, chestAmountAnimationDuration).OnComplete( ()=>
            {
                EndingForMultiplier(currentChest);
            });
        }
        else
        {
            //there is a number in chest.
            if(currentAmount.AppliedMultiplier > 0)
            {
                //a multiplier is being applied to the number.
                MultiCandy(currentAmount, currentChest);
            }
            else
            {  
                //a multiplier is not being applied to the number.
                BaseCandy(currentAmount, currentChest);
            } 
        }
    }      
}