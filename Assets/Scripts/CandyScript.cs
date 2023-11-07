
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
using UnityEditor.Callbacks;

public class CandyScript : MonoBehaviour
{


    [SerializeField] float chestAmountAnimationDuration;

    [SerializeField] CanvasGroup chestAmountCanvasGroup;
    
    [SerializeField] TextMeshProUGUI chestAmountText;

    [SerializeField] RectTransform chestAmountRect;


    RectTransform chestAmountStartingPos;
    
    [SerializeField] GameObject candyBag;

    [SerializeField] float candyBagAnimationDuration;

    [SerializeField] float candyBagAnimationStrength;

    Vector3 candyBagDefaultScale;
    
    [Category("Balance")]
    [SerializeField] BalanceScript balance;

    [Category("Candy Stats")]
    [SerializeField] private GameObject candyPrefab;

    [SerializeField] int candyAnimationDuration;

     [SerializeField] Transform candyAnimationPosition;

    [SerializeField] private Transform spawnLocation;

    List<GameObject> candy = new List<GameObject>();
    [SerializeField] private Transform parent;

    [SerializeField] AudioClip collectionSound;


    void Start()
    {
        candyBagDefaultScale = candyBag.transform.localScale;
        chestAmountCanvasGroup.alpha = 0f;
        chestAmountStartingPos = chestAmountRect;
        
    }

    IEnumerator EndingWait(Pumpkin currentChest)
    {
        yield return new WaitForSeconds(1f);
        //remove amount gained from chest on screen
        chestAmountCanvasGroup.alpha = 1f;
        chestAmountRect.transform.localPosition = new Vector3(0f, 0f, 0f);
        chestAmountRect.DOAnchorPos(new Vector3(0f,-1000f), chestAmountAnimationDuration).SetEase(Ease.InOutQuint).OnComplete(()=>{
            Debug.Log("moving text");
            chestAmountCanvasGroup.alpha = 0f;
            currentChest.CollectionEnd();
        }); 
        chestAmountCanvasGroup.DOFade(0f, chestAmountAnimationDuration);
        
            
        
        //after all candies have move to end position call Collection End on the current chest passed in method parameter. (the currently clicked chest)
        

    }

  
    public void CollectCandy(float currentAmount, Pumpkin currentChest)
    {

        //Method for spawning candy and moving it towards last win number.
        
        bool ended = false;
        for(int i = 0; i < candy.Count; i++)
        {
            //Destroys previously summoned candy.
            Destroy(candy[i]);
        }
        candy.Clear(); // clears list of previous candy.



       



        int amount = System.Convert.ToInt32(currentAmount / 0.05f); //converts amount to int so it can be later used in for loop.

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
            GameObject candyInstance = Instantiate(candyPrefab, parent);
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
            GameObject candyInstance = Instantiate(candyPrefab, parent);
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
            GameObject candyInstance = Instantiate(candyPrefab, parent);
            float Positionx = (spawnLocation.position.x + UnityEngine.Random.Range(-1f,1f));
            
            float Positiony = (spawnLocation.position.y + UnityEngine.Random.Range(-1f,1f));
            
            candyInstance.transform.position = new Vector3(Positionx,Positiony,spawnLocation.position.z); // places each candy in slightly random position so not overlap and ability to see every candy.
            candyInstance.GetComponent<CandyPrefab>().candySize = 0.05f; //allows for script to read value of candy later on.
            candy.Add(candyInstance);
            candyInstance.transform.localScale = new Vector3(1f, 1f, 1f); //sets scale to base.
        }

        

        GameObject[] candyArray =  candy.ToArray(); // adds spawned candyPrefabs of list candy to array.

        candyAnimationPosition.position = new Vector3(candyAnimationPosition.position.x, candyAnimationPosition.position.y, spawnLocation.position.z); //making sure candies stay at same z position.
      
        
        for (int i=0; i < candyArray.Length; i++)
        {
            //for loop for moving candy to last win number.

            float size = candy[i].GetComponent<CandyPrefab>().candySize; //finds size of candy so script knows what value to add to last win number when object reaches destination.

            Transform currentCandy = candy[i].transform; //finds transform of current candy for use in DO calls.

            Vector3 currentScale = currentCandy.localScale; //saves candies current scale
            
            currentCandy.localScale = Vector3.zero; //sets candies scale to zero so they are able to go from 0 to currentScale using DoScale.
            
            
            float duration = UnityEngine.Random.Range (.5f, 1.5f); //finds random duration for accomplishing DO calls so candies do not move all at same time.

            
                //Moves object up on y axis to look like coming out of pumpkin chest.
                currentCandy.DOMove(new Vector3(currentCandy.position.x,currentCandy.position.y+3f,currentCandy.position.z), .75f).SetEase (Ease.InOutBack);
                //Scales candy up to current scale.
                currentCandy.DOScale(currentScale, .90f).SetEase(Ease.OutBack).OnComplete(()=>
                {

                    //show amount gained from chest on screen
                    chestAmountText.text = currentAmount.ToString("C",new CultureInfo("en-US"));
                    chestAmountCanvasGroup.alpha = 0f;
                    chestAmountRect.transform.localPosition = new Vector3(0f,+1000f,0f);
                    chestAmountRect.DOAnchorPos(new Vector2(0f, 0f), chestAmountAnimationDuration, false).SetEase(Ease.OutBounce); 
                    chestAmountCanvasGroup.DOFade(1f, chestAmountAnimationDuration);
                    
                    //On completion of scaling to current scale moving to last win number (location of candyAnimationPosition)
                    currentCandy.DOMove (candyAnimationPosition.position, duration).SetEase (Ease.InOutBack).OnComplete (() =>
                        {

                             

                            //adds value attached to candy to last win value and scales candy down to 0.
                            balance.AddToLastWin(size);
                            SoundManager.Instance.PlaySound(collectionSound);
                            candyBag.transform.DOShakePosition(candyAnimationDuration, candyBagAnimationStrength);
                            candyBag.transform.DOShakeRotation(candyAnimationDuration, candyBagAnimationStrength);
                            candyBag.transform.DOShakeScale(candyAnimationDuration, candyBagAnimationStrength).OnComplete(()=>{
                                candyBag.transform.DOScale(candyBagDefaultScale,0.25f).SetEase(Ease.Linear);
                            
                            
                            
                            });
                            currentCandy.DOScale(0f, 0.3f).SetEase(Ease.OutBack).OnComplete (() => 
                                {
                     
                        
                                    
                        
                                    if(i == candyArray.Length)
                                        {
                                            //if statment to check and make sure Coroutine is not started more than once.
                                            if(ended == false)
                                            {
                                                //Coroutine that waits and allows user to read gained value.
                                                ended = true;
                                                StartCoroutine(EndingWait(currentChest));
                                            }
                                            
                                   
                            
                                        }
                        
                                });

                        });


                });    
           
            

            
            
            
        }
        
        
    }
}
