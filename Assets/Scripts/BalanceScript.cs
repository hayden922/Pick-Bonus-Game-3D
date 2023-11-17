using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using JetBrains.Annotations;
using TMPro;
using System;
using System.ComponentModel;


public class BalanceScript : MonoBehaviour
{

    
    [field: Header("Balance")]
    private float startingBalance = 10.00f;
    static public float currentBalance{get; private set;}
    [SerializeField] TextMeshProUGUI balanceText;
    float tempBalance;

     
    [field: Header("Last Win")]
    static public float currentLastWin{get; private set;}
    [SerializeField] TextMeshProUGUI lastWinText;

    

    void Start()
    {
        currentBalance = startingBalance;
        tempBalance = currentBalance;
        balanceText.text = currentBalance.ToString("C",new CultureInfo("en-US"));
        
        
    }

    private IEnumerator IncrementCoroutine ()
	{
        //Method for incrementing value, specfically for balance.
        float incrementTime = 4;
		float time = 0;
		balanceText.text = currentBalance.ToString("C",new CultureInfo("en-US"));
		while ( time < incrementTime )
		{
			yield return null;
			time += Time.deltaTime;
			float factor = time / incrementTime;
			balanceText.text = ((float) Mathf.Lerp(currentBalance, tempBalance, factor)).ToString("C",new CultureInfo("en-US"));
		}
        currentBalance = tempBalance;
		yield break;
	}

    private IEnumerator DecrementCoroutine ()
	{
        //Method for decrementing value, specfically for balance.
        float incrementTime = 1f;
		float time = 0;
		balanceText.text = currentBalance.ToString("C",new CultureInfo("en-US"));
		while ( time < incrementTime )
		{
			yield return null;
			time += Time.deltaTime;
			float factor = time / incrementTime;
			balanceText.text = ((float) Mathf.Lerp( currentBalance, tempBalance, factor)).ToString("C",new CultureInfo("en-US"));
		}
        currentBalance = tempBalance;
		yield break;
	}





    public void AddToBalance(float add)
    {
        //method for adding to static value currentBalance. Calls increment coroutine to allow user to see value increase.
        tempBalance = currentBalance + add;
        StartCoroutine(IncrementCoroutine());
        

        
    }
    public void SubtractFromBalance(float subtract)
    {
        //method for subtracting to static value currentBalance. Calls decrement coroutine to allow user to see value decrease.
        tempBalance = currentBalance - subtract;
        StartCoroutine(DecrementCoroutine());
        
        
        
    }

    public void AddToLastWin(float add)
    {
        //Method for adding to last win. Does not call increment or decrement because nature of collection method in candy script allows for a similar event.
        currentLastWin += add;
        lastWinText.text = currentLastWin.ToString("C",new CultureInfo("en-US"));
    }
    public void ResetLastWin()
    {
        //Method to reset last win when necessary.
        currentLastWin = 0.00f;
        lastWinText.text = currentLastWin.ToString("C",new CultureInfo("en-US"));
    }

    public void CheckBalance()
    {
        if(currentBalance != tempBalance)
        {
            currentBalance = tempBalance;
            balanceText.text = currentBalance.ToString("C",new CultureInfo("en-US"));

        }
    }

}
