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

    //AmountsInList[] amountsInChests;

    List<AmountsInList> amountsInChests;

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
        
       /// List<float> chestAmountsList = new List<float>(); //list of amounts
        float smallAmount = (denomAmount*multiplier) / 0.05f; //small amount is used to seperate win amount easier. Because each possible num is divisable by 0.05.
        
        int currentAmount = System.Convert.ToInt32(smallAmount); //setting small amount to int for use in random range.
        //float currentAmount = smallAmount;
        int divisorValue = 0;
        if(multiplier > 5)
        {
            float percent = UnityEngine.Random.Range(1, 101); // 1 to 100 
            if(percent <= 10) //if number is less than or equal to 10 (10% chance)
            {
                multiplierChest = 3;
                divisorValue = 8;
                
            }
            else if(percent <= 25) //if number is less than or equal to 25 (15% chance)
            {
                multiplierChest = 3;
                divisorValue = 8;
            }

            else if(percent <= 50) //if number is less than or equal to 50 (25% chance)
            {
                multiplierChest = 3;
                divisorValue = 8;
            }
            else //if number is greater than 50 (50% chance)
            {
                multiplierChest = 3;
                divisorValue = 8;
            }
        }
        else
        {
            multiplierChest = 0;
        }
        Debug.Log("multiplierChestBelow");
        Debug.Log(multiplierChest);
        if(randChestNum <= multiplierChest)
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
        
        amountsInChests = new List<AmountsInList>(); //list of amounts
        for(int i = 0; i < randChestNum; i++)
        {
          amountsInChests.Add(new AmountsInList(0, false, 0));
        }

       
       // int winsInChests = randChestNum - multiplierChest;
       // if(winsInChests == 0)
       // {
       //     Debug.Log("chest Error");
       // }


       


        Debug.Log(randChestNum);
        for (int i=1; i <= multiplierChest; i++)
        {
            int randPos;
            do
            {
                randPos = UnityEngine.Random.Range(0, amountsInChests.Count-1); //Do not want to put multiplier chest as last chest before pooper.
                
            }while(amountsInChests[randPos].Multiplier);
                   
                Debug.Log(randPos);
            Debug.Log("multiplierAdded");
            amountsInChests[randPos] = new AmountsInList(0f,true,0);
                    
        }  
            
            
            
        
        int multiplierAmount = divisorValue;
        Debug.Log(currentAmount);
        float combinedTotal;
        do{
            multiplierAmount = divisorValue;
            currentAmount = System.Convert.ToInt32(smallAmount);
            combinedTotal = 0;
            for (int i = amountsInChests.Count-1; i >= 0; i--) 
            {
                    Debug.Log(i);
                //loops for a number of times equal to the random amount of chests decided -1 so every chest is given a value.
                if(amountsInChests[i].Multiplier)
                {
                    multiplierAmount -= 2;            
                }
                else
                {
                    if(currentAmount > 0) //checker just in case value is so low that it causes 0
                    {
                        
                        float total = 0;
                        int subtractionAmount = 0;
                        
                    
                                
                        //subtractionAmount = UnityEngine.Random.Range(1,(((currentAmount/multiplierAmount)+1)/2))*multiplierAmount;
                        
                        if(multiplierAmount == 0)
                        {
                            
                            subtractionAmount = UnityEngine.Random.Range(1, (currentAmount + 1)); 
                        }
                        else
                        {
                            
                            do
                            {
                                subtractionAmount = multiplierAmount * UnityEngine.Random.Range(1, ((currentAmount/multiplierAmount)+1));                        
                            } while (subtractionAmount % multiplierAmount != 0);         
                        }

                                
                            //takes number from 1 to currentAmount/2. Divided by two so there will always be some value left over.

                            
                        total = (subtractionAmount)*0.05f; //turns amount back into dollar amount
                        Debug.Log(subtractionAmount);
                        Debug.Log(total);
                        combinedTotal += total; 

                        amountsInChests[i] =  new AmountsInList(total,false,multiplierAmount);
                        
                
                        currentAmount = currentAmount-subtractionAmount; //takes away amount put in list from current amount
                    }
                    else
                    {
                        amountsInChests[i] =  new AmountsInList(0f,false,0);
                    }
                }
                
                
            }
        }while(currentAmount != 0 && (combinedTotal*0.05f) != smallAmount);
        Debug.Log(currentAmount);
        Debug.Log("removing");
        amountsInChests.RemoveAll(item => item.Number == 0 && !item.Multiplier);

        
        
        
        /*
        
        float firstAddition;

             
        if(multiplierAmount > 0)
        {
            Debug.Log("multiplier is first");
            bool foundStorage = false;
            for(int i = 0;  i < amountsInChests.Length; i++)
            {
                if(!amountsInChests[i].Multiplier)
                {
                    firstAddition = (currentAmount)*0.05f;
                    
                    if((amountsInChests[i].Number + firstAddition)/amountsInChests[i].AppliedMultiplier % 0.05 == 0)
                    {
                        amountsInChests[i].Number += firstAddition;
                        foundStorage = true;
                        break;         
                    }

                   
                }
            }
            if(foundStorage == false)
            {
                Debug.Log("oops");
                if(amountsInChests.Length < 8)
                {
                    Array.Resize(ref amountsInChests, amountsInChests.Length+1);
                    Debug.Log(amountsInChests.Length);
                    for(int i = amountsInChests.Length-1; i > 0; i --)
                    {
                        amountsInChests[i] = amountsInChests[i-1];
                    }
                    firstAddition = (currentAmount)*0.05f;
                    amountsInChests[0] = new AmountsInList(firstAddition,false,0);
                }
                else
                {
                    DecideWinAmounts(denomAmount);
                }
                


            }
            
        }
        else
        {
            firstAddition = (currentAmount)*0.05f;
             amountsInChests[0] = new AmountsInList(firstAddition,false,multiplierAmount);
        }



        */




        /*
        else
        {
            Debug.Log("found zero");
            Array.Resize(ref amountsInChests, amountsInChests.Length-1);
            if(amountsInChests[amountsInChests.Length-1].Multiplier)
            {
               Debug.Log("and it caused multiplier to be last");
               for(int i = amountsInChests.Length-1; i > 0; i--)
               {
                    if(!amountsInChests[i].Multiplier && (amountsInChests[i].Number/8f) % 0.05f == 0f && amountsInChests[i].Number !=0 )
                    {
                        amountsInChests[i].Multiplier = true;
                        amountsInChests[amountsInChests.Length].Multiplier = false;
                        amountsInChests[amountsInChests.Length].Number = amountsInChests[i].Number;
                        amountsInChests[amountsInChests.Length].AppliedMultiplier = multiplierAmount;
                        amountsInChests[i].Number = 0f;
                        amountsInChests[i].AppliedMultiplier = 0;

                    }
               }
            }
        }
*/
/*
        
        for (int i = 0; i < amountsInChests.Length; i++) 
        {
            if(amountsInChests[i].Number/amountsInChests[i].AppliedMultiplier < 0.05f && !amountsInChests[i].Multiplier)
            {
                for (int j = 0; j < amountsInChests.Length; j++) 
                {
                    if(amountsInChests[j].Number/amountsInChests[i].AppliedMultiplier > 0.05f)
                    {
                        Debug.Log("foundLowNum");
                        Debug.Log(amountsInChests[i].Number);
                        Debug.Log(amountsInChests[j].Number);
                        float temp = amountsInChests[i].Number;
                        amountsInChests[i].Number = amountsInChests[j].Number;
                        amountsInChests[j].Number = temp;
                        break;

                    }

                }
            }
        }
/*
        for (int i = 0; i < amountsInChests.Length; i++) 
        {
            if(amountsInChests[i].Number/amountsInChests[i].AppliedMultiplier % 0.05f != 0 && !amountsInChests[i].Multiplier)
            {
                for (int j = 0; j < amountsInChests.Length; j++) 
                {
                    if(amountsInChests[j].Number/amountsInChests[i].AppliedMultiplier % 0.05f == 0 && !amountsInChests[j].Multiplier)
                    {
                        Debug.Log("foundNotDivisable");
                        Debug.Log(amountsInChests[i].Number);
                        Debug.Log(amountsInChests[j].Number);
                        float temp = amountsInChests[i].Number;
                        amountsInChests[i].Number = amountsInChests[j].Number;
                        amountsInChests[j].Number = temp;
                        break;

                    }

                }
            }
        }

*/

        

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

        for (int i = 0; i < amountsInChests.Count; i++) 
        {
            Debug.Log("start"); 
           Debug.Log(amountsInChests[i].Number); 
           if(amountsInChests[i].Number == 0f && amountsInChests[i].Multiplier == false)
           {
            Debug.Log("still have zeros");
           }
           Debug.Log(amountsInChests[i].Multiplier);
           Debug.Log(amountsInChests[i].AppliedMultiplier);
           Debug.Log("end"); 
        }
       
        AmountsInList[] amountsArray = amountsInChests.ToArray();
        

        return (winAmount, amountsArray);




        





    }

 
}
