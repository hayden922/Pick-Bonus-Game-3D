
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

public class CandyScript : MonoBehaviour
{

    [SerializeField] BalanceScript balance;
    [SerializeField] private GameObject candyPrefab;
    [SerializeField] private Transform spawnLocation;

    [SerializeField] private Transform parent;

    List<GameObject> candy = new List<GameObject>();

    [SerializeField] int candyAnimationDuration;

     [SerializeField] Transform candyAnimationPosition;

     
  

    void Start()
    {

        
    }

    
    void Update()
    {
        
    }

    public void CollectCandy(float currentAmount, Pumpkin currentChest)
    {
        

        for(int i = 0; i < candy.Count; i++)
        {
            Destroy(candy[i]);
        }
        candy.Clear();


        int amount = System.Convert.ToInt32(currentAmount / 0.05f);

        int twentyFiveNum = 0;
        int oneNum = 0;

        if(amount > 20)
        {
            if(amount > 500)
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
            GameObject candyInstance = Instantiate(candyPrefab, parent);
            float Positionx = (spawnLocation.position.x + UnityEngine.Random.Range(-1f,1f));
            float Positiony = (spawnLocation.position.y + UnityEngine.Random.Range(-1f,1f));
            candyInstance.transform.position = new Vector3(Positionx,Positiony,spawnLocation.position.z);
            candyInstance.GetComponent<CandyPrefab>().candySize = 25.00f;
            candy.Add(candyInstance);
            candyInstance.transform.localScale = new Vector3(2, 2, 2);
        }

        Debug.Log(oneNum);
        for (int i = 0; i < oneNum; i++)
        {
            
            GameObject candyInstance = Instantiate(candyPrefab, parent);
            float Positionx = (spawnLocation.position.x + UnityEngine.Random.Range(-1f,1f));
            float Positiony = (spawnLocation.position.y + UnityEngine.Random.Range(-1f,1f));
            candyInstance.transform.position = new Vector3(Positionx,Positiony,spawnLocation.position.z);
            candyInstance.GetComponent<CandyPrefab>().candySize = 1.00f;
            candy.Add(candyInstance);
            candyInstance.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        }
        
        
        for (int i = 0; i < amount; i++)
        {
            GameObject candyInstance = Instantiate(candyPrefab, parent);
            float Positionx = (spawnLocation.position.x + UnityEngine.Random.Range(-1f,1f));
            
            float Positiony = (spawnLocation.position.y + UnityEngine.Random.Range(-1f,1f));
            
            candyInstance.transform.position = new Vector3(Positionx,Positiony,spawnLocation.position.z);
            candyInstance.GetComponent<CandyPrefab>().candySize = 0.05f;
            candy.Add(candyInstance);
            candyInstance.transform.localScale = new Vector3(1f, 1f, 1f);
        }

        

        GameObject[] candyArray =  candy.ToArray();

        candyAnimationPosition.position = new Vector3(candyAnimationPosition.position.x, candyAnimationPosition.position.y, spawnLocation.position.z);
      
        
        for (int i=0; i < candyArray.Length; i++)
        {
            float size = candy[i].GetComponent<CandyPrefab>().candySize;

            Transform currentCandy = candy[i].transform;

            Vector3 currentScale = currentCandy.localScale;
            
            currentCandy.localScale = Vector3.zero;
            
            
            float duration = UnityEngine.Random.Range (.5f, 1.5f);


            

            
            
            currentCandy.DOMove(new Vector3(currentCandy.position.x,currentCandy.position.y+3f,currentCandy.position.z), .75f).SetEase (Ease.InOutBack);
            
            currentCandy.DOScale(currentScale, .90f).SetEase(Ease.OutBack).OnComplete(()=>
            {

                currentCandy.DOMove (candyAnimationPosition.position, duration).SetEase (Ease.InOutBack).OnComplete (() =>
                {
                    balance.AddToLastWin(size);
                    currentCandy.DOScale(0f, 0.3f).SetEase(Ease.OutBack).OnComplete (() => 
                    {
                     //   candyArray[i].gameObject.SetActive(false);
                    //    Destroy(candyArray[i].gameObject);
                        
                        
                        if(i == candyArray.Length)
                        {
                            currentChest.CollectionEnd();
                        }
                        
                    });

            });


            });    
            
        }
        
        
    }
}
