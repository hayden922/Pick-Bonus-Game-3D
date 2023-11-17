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
using System.Threading.Tasks;
using System.IO;
public class WinningSolver : MonoBehaviour
{
    float multiplier;
    [SerializeField] TextMeshProUGUI lastWinText;
    int multiplierChest = 0;
    List<AmountsInList> amountsInChests;
    public class AmountsInList 
    {    
        public float Number;
        public bool Multiplier;
        public int AppliedMultiplier;
        public AmountsInList (float number, bool multiplier, int appliedMultiplier)
        {
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
        int randChestNum  = UnityEngine.Random.Range(1,9); //random number of chest to split win amount into. 1 to 8  
        float smallAmount = (denomAmount*multiplier) / 0.05f; //small amount is used to seperate win amount easier. Because each possible num is divisable by 0.05.
        int currentAmount = System.Convert.ToInt32(smallAmount); //setting small amount to int for use in random range.
        int divisorValue = 0; //divisorValue used to tell in total win amount is divisible by the largest applied multiplier in game.
        if(multiplier > 5)
        {
            float multiChestPercent = UnityEngine.Random.Range(1, 101); // 1 to 100 
            if(multiChestPercent <= 10) //if number is less than or equal to 10 (10% chance) 3 multiplier chests.
            {
                multiplierChest = 3;
                divisorValue = 8;
                
            }
            else if(multiChestPercent <= 25) //if number is less than or equal to 25 (15% chance) 2 multiplier chests.
            {
                multiplierChest = 2;
                divisorValue = 4;
            }

            else if(multiChestPercent <= 50) //if number is less than or equal to 50 (25% chance) 1 multiplier chests.
            {   
                multiplierChest = 1;
                divisorValue = 2;
            }
            else //if number is greater than 50 (50% chance) 0 multiplier chests.
            {
                multiplierChest = 0;
                divisorValue = 0;
            }
        }
        else
        {
            multiplierChest = 0;
        }
        if(randChestNum <= multiplierChest+1)
        {
            //if there are not enough chests to fit multipliers and win amount.
            switch (multiplierChest) //using switch statment for different multiplier amounts.
            {
                case 3:
                    randChestNum = UnityEngine.Random.Range(5,9); //random number of chest to split win amount into. 5 to 8
                    break;   
                case 2:
                    randChestNum = UnityEngine.Random.Range(4,9); //random number of chest to split win amount into. 4 to 8
                    break;
                case 1:
                    randChestNum = UnityEngine.Random.Range(3,9); //random number of chest to split win amount into. 3 to 8
                    break;
                default:
                    break;
            }
        }

        amountsInChests = new List<AmountsInList>(); //list of amounts
        for(int i = 0; i < randChestNum; i++)
        {
          amountsInChests.Add(new AmountsInList(0, false, 0)); //adds empty object AmountsInList to list.
        }
        for (int i=1; i <= multiplierChest; i++)
        {   
            //for ever multiplier chest in the scene.
            if(divisorValue > 0)
            {
                //statment double checking that we don't divide by zero.
                int randPos;
                do
                {
                    if(smallAmount%divisorValue != 0) //if amount is not divisible by the largest applied multiplier.
                    {
                        if(amountsInChests.Count <= 2)
                        {
                            //for debugging.
                            Debug.Log("errorWithMultiChests");
                            randPos = 0;
                            break;
                        }
                        else
                        {
                            //if amount not divisible do not put multiplierChestin index 0 so laft over can be put there without needing to be divided for multiplier.
                            randPos = UnityEngine.Random.Range(1, amountsInChests.Count-1); //Do not want to put multiplier chest as last chest before pooper.  
                        }
                    
                    }
                    else
                    {
                        //add multiplier chest
                        randPos = UnityEngine.Random.Range(0, amountsInChests.Count-1); //Do not want to put multiplier chest as last chest before pooper. 
                    } 
                }while(amountsInChests[randPos].Multiplier); //while there is already a multiplier chest at that location.

                amountsInChests[randPos] = new AmountsInList(0f,true,0);
                        
            }  
        }

        float combinedTotal;

        do
        {
            int multiplierAmount = divisorValue;
            currentAmount = System.Convert.ToInt32(smallAmount);
            combinedTotal = 0;
            for (int i = amountsInChests.Count-1; i >= 0; i--) 
            {      
                //loops for a number of times equal to the random amount of chests decided -1 so every chest is given a value.
                if(amountsInChests[i].Multiplier)
                {
                    //if the object contains bool Multiplier = true then make required mutliplier changes and skip adding number to that chest.
                    if(multiplierAmount == 2)
                    {  
                        multiplierAmount -= 2;
                    }
                    else if(multiplierAmount == 0)
                    {
                        multiplierAmount = 0; 
                    }
                    else
                    {
                        multiplierAmount /= 2;    
                    }          
                }
                else
                {
                    if(currentAmount > 0) //checker just in case value is so low that it causes 0
                    { 
                        float total = 0;
                        int subtractionAmount = 0;         
                        if(multiplierAmount == 0)
                        {
                            //no need to take divisibility into consideration.
                            if(i == 0)
                            {
                                subtractionAmount = currentAmount;
                            }

                            else
                            {
                                subtractionAmount = UnityEngine.Random.Range(1, (currentAmount + 1)); 
                            }  
                        }
                        else
                        {
                            do
                            {
                                //find subtractionAmount until it can be divisible by the current applied multiplier amount.
                                subtractionAmount = multiplierAmount * UnityEngine.Random.Range(1, ((currentAmount/multiplierAmount)+1));                        
                            } while (subtractionAmount % multiplierAmount != 0);         
                        }          
                        total = (subtractionAmount)*0.05f; //turns amount back into dollar amount
                        combinedTotal += total; 
                        amountsInChests[i] =  new AmountsInList(total,false,multiplierAmount);
                        currentAmount = currentAmount-subtractionAmount; //takes away amount put in list from current amount
                    }
                    else
                    {
                        amountsInChests[i] =  new AmountsInList(0f,false,0); //if currentAmount is 0 just add 0 to object number. (will be deleted later.)
                    }
                }
                
                
            }
        
        }while(currentAmount != 0 && (combinedTotal/0.05f) != smallAmount); //while the currentAmount is still larger than zero and the total of values added to chest does not equal the orginal amount keep looping.
   
        amountsInChests.RemoveAll(item => item.Number == 0 && !item.Multiplier); //gets rid of all objects in list that contain a number of 0 and are not multipliers.
        float winAmount = denomAmount*multiplier;
        AmountsInList[] amountsArray = amountsInChests.ToArray();
        return (winAmount, amountsArray);
    }
}
