using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using JetBrains.Annotations;
using TMPro;
using System;
public class WinningSolver : MonoBehaviour
{
    float multiplier;

    [SerializeField] TextMeshProUGUI lastWinText;



    // Start is called before the first frame update
    void Start()
    {
        lastWinText.text = 0.00.ToString("C",new CultureInfo("en-US"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    



    public float FindMultiplier()
    {
        
        
        float percent = UnityEngine.Random.Range(1, 101); // 1 to 100 
        if(percent <= 5)
        {
            multiplier = UnityEngine.Random.Range(1,6)*100;
        }
        else if(percent <= 20)
        {
            float randNum = UnityEngine.Random.Range(1,7);
            switch (randNum)
            {
            case 6:
                multiplier = 64;
                break;   
            case 5:
                multiplier = 48;
                break;
            case 4:
                multiplier = 32;
                break;
            case 3:
                multiplier = 24;
                break;
            case 2:
                multiplier = 16;
                break;
            case 1:
                multiplier = 12;
                break;
            default:
                break;
            }
        }
        else if (percent <= 50)
        {
            
            multiplier = UnityEngine.Random.Range(1,11);
        }
        else
        {
            multiplier = 0;
            lastWinText.text = 0.00.ToString("C",new CultureInfo("en-US"));
            
        }
        
        return multiplier;


        
    }

    public (float, float[]) DecideWinAmounts(float denomAmount)
    {
        
        int randChestNum = UnityEngine.Random.Range(1,9);
        Debug.Log("randChestNum");
        Debug.Log(randChestNum);
        List<float> chestAmountsList = new List<float>();
        float smallAmount = (denomAmount*multiplier) / 0.05f;
        Debug.Log(denomAmount);
        Debug.Log(multiplier);
        int currentAmount = System.Convert.ToInt32(smallAmount);
        
        for (int i = 0; i < randChestNum-1; i++)
        {
            
            if(currentAmount > 0)
            {
            Debug.Log(currentAmount);
            int subtractionAmount = UnityEngine.Random.Range(1,((currentAmount+1)/2));

           

           

            
            float total = subtractionAmount*0.05f;

            chestAmountsList.Add(total);
            
            
            currentAmount = currentAmount-subtractionAmount;
            }
            
        }

       Debug.Log(currentAmount);
        float finalAddition = currentAmount*0.05f;

        if (finalAddition > 0)
        {
                chestAmountsList.Add(finalAddition);
        }
        else
        {
            Debug.Log("found zero");
        }
        

       
        for (int i = chestAmountsList.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            float temp = chestAmountsList[i];
            chestAmountsList[i] = chestAmountsList[j];
            chestAmountsList[j] = temp;
        }

        float[] chestAmounts = chestAmountsList.ToArray();
       

        float winAmount = denomAmount*multiplier;
       

        

        return (winAmount, chestAmounts);




        





    }
}
