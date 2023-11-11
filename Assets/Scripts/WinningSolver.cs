using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using JetBrains.Annotations;
using TMPro;
using System;
using Unity.VisualScripting;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml.Schema;
public class WinningSolver : MonoBehaviour
{
    float multiplier;

    [SerializeField] TextMeshProUGUI lastWinText;

    int multiplierChest = 0;

    AmountsInList[] amountsInChests;

     public class AmountsInList {
           
            public float Number;
            public bool Multiplier;

            public int AppliedMultiplier;
           
            public AmountsInList (float number, bool multiplier, int appliedMultiplier){
                Number = number;
                Multiplier = multiplier;    
                AppliedMultiplier = appliedMultiplier;
            }
        }
    

    void Start()
    {
        lastWinText.text = 0.00.ToString("C",new CultureInfo("en-US")); //making sure the last win number starts at zero.
    }

    



    public float FindMultiplier()
    {
        //method for finding random multiplier.
        
        float percent = UnityEngine.Random.Range(1, 101); // 1 to 100 
        if(percent <= 5) //if number is less than or equal to 5 (5% chance)
        {
            multiplier = UnityEngine.Random.Range(1,6)*100; //100 to 500
        }
        else if(percent <= 20) //if number is less than or equal to 20 (15% chance)
        {
            float randNum = UnityEngine.Random.Range(1,7);
            switch (randNum) //using switch statment to find random multiplier from the possbile 15% chance options
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
        else if (percent <= 50) //if number is less than or equal to 50 (30% chance)
        {
            
            multiplier = UnityEngine.Random.Range(1,11); //1 to 10
        }
        else //if number is greater than 50 (50% chance)
        {
            multiplier = 0;
            lastWinText.text = 0.00.ToString("C",new CultureInfo("en-US"));
            
        }
        
        return multiplier; 


        
    }

    public (float, AmountsInList[]) DecideWinAmounts(float denomAmount)
    {
        //Method decides win amount and its division amoungst chests.
        int randChestNum = UnityEngine.Random.Range(1,9); //random number of chest to split win amount into. 1 to 8
        Debug.Log("randChestNum");
        Debug.Log(randChestNum);
       /// List<float> chestAmountsList = new List<float>(); //list of amounts
        float smallAmount = (denomAmount*multiplier) / 0.05f; //small amount is used to seperate win amount easier. Because each possible num is divisable by 0.05.
        Debug.Log(denomAmount);
        Debug.Log(multiplier);
        int currentAmount = System.Convert.ToInt32(smallAmount); //setting small amount to int for use in random range.
        if(multiplier > 5)
        {
            float percent = UnityEngine.Random.Range(1, 101); // 1 to 100 
            if(percent <= 10) //if number is less than or equal to 10 (10% chance)
            {
                multiplierChest = 3;
            }
            else if(percent <= 25) //if number is less than or equal to 25 (15% chance)
            {
                multiplierChest = 2;
            }

            else if(percent <= 50) //if number is less than or equal to 50 (25% chance)
            {
                multiplierChest = 1;
            }
            else //if number is greater than 50 (50% chance)
            {
                multiplierChest = 0;
            }
        }
        else
        {
            multiplierChest = 0;
        }
        Debug.Log(multiplierChest);
        if(randChestNum < multiplierChest)
        {
            //if there are not enough chests to fit multipliers and win amount.
             switch (multiplierChest) //using switch statment for different multiplier amounts.
            {
            case 3:
                randChestNum = UnityEngine.Random.Range(4,9); //random number of chest to split win amount into. 4 to 8
                break;   
            case 2:
                randChestNum = UnityEngine.Random.Range(3,9); //random number of chest to split win amount into. 3 to 8
                break;
            case 1:
                randChestNum = UnityEngine.Random.Range(2,9); //random number of chest to split win amount into. 2 to 8
                break;
            default:
                break;
            }
        }
        
        amountsInChests = new AmountsInList[randChestNum]; //list of amounts
        for(int i = 0; i <amountsInChests.Length; i++)
        {
          amountsInChests[i] = new AmountsInList(0, false, 0);
        }

       
       // int winsInChests = randChestNum - multiplierChest;
       // if(winsInChests == 0)
       // {
       //     Debug.Log("chest Error");
       // }


        
        for (int i=0; i < multiplierChest; i++)
        {
            bool found = false;
            while(found == false)
            {
                int randPos = UnityEngine.Random.Range(0, amountsInChests.Length); //Do not want to put multiplier chest as last chest before pooper.   
                if(!amountsInChests[randPos].Multiplier)
                {
                    Debug.Log("multiplierAdded");
                    amountsInChests[randPos] = new AmountsInList(0f,true,0);
                    found = true;
                }  
            }
            
            
        }
        int multiplierAmount = 0;
        for (int i = 0; i < amountsInChests.Length-1; i++) 
        {
            
            //loops for a number of times equal to the random amount of chests decided -1 so every chest is given a value.
            
            if(amountsInChests[i].Multiplier)
            {
                if(multiplierAmount < 2)
                {
                    multiplierAmount = 2;
                }
                else
                {
                     multiplierAmount = multiplierAmount*2;
                }              
            }
            else
            {
                if(currentAmount > 0) //checker just in case value is so low that it causes 0
                {
                    Debug.Log(currentAmount);
                    int subtractionAmount = UnityEngine.Random.Range(1,((currentAmount+1)/2)); //takes number from 1 to currentAmount/2. Divided by two so there will always be some value left over.

           

           

            
                    float total = subtractionAmount*0.05f; //turns amount back into dollar amount

                   amountsInChests[i] =  new AmountsInList(total,false,multiplierAmount);
                    Debug.Log("totalAdded");
                   Debug.Log(amountsInChests[i].Number);

            
            
                    currentAmount = currentAmount-subtractionAmount; //takes away amount put in list from current amount
                }
            }
            
            
        }

        
        float finalAddition = currentAmount*0.05f; //any left over value is the final number added to the list, as long as its not zero.

        if (finalAddition > 0)
        {
             amountsInChests[amountsInChests.Length-1] = new AmountsInList(finalAddition,false,multiplierAmount);
        }
        else
        {
            Debug.Log("found zero");
        }
        

       /*
        for (int i = amountsInChests.Count - 1; i > 0; i--)
        {
            //randomizes list because method for finding values lead them in a predictable decreasing value sometimes.
            int j = UnityEngine.Random.Range(0, i + 1);
            float temp = chestAmountsList[i];
            chestAmountsList[i] = chestAmountsList[j];
            chestAmountsList[j] = temp;
        }
        */
        //AmountsInList[] chestAmounts = amountsInChests.ToArray(); // turning list into array as I am more aware of how to handle arrays and previously created method were created to take in array.
       

        float winAmount = denomAmount*multiplier;

        for (int i = 0; i < amountsInChests.Length; i++) 
        {
           Debug.Log(amountsInChests[i].Multiplier); 
        }
       

        

        return (winAmount, amountsInChests);




        





    }
}
