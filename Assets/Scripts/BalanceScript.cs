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
    private float startingBalance = 10.00f;
    static public float currentBalance{get; private set;}


    static public float currentLastWin{get; private set;}
    [SerializeField] TextMeshProUGUI balanceText;

    [SerializeField] TextMeshProUGUI lastWinText;

    float tempBalance;

    void Start()
    {
        currentBalance = startingBalance;
        tempBalance = currentBalance;
        Debug.Log(currentBalance);
        balanceText.text = currentBalance.ToString("C",new CultureInfo("en-US"));
        
        
    }

    
    void Update()
    {
        
    }
/*
    void FixedUpdate()
    {
        
        
        if(currentBalance > tempBalance)
        {
            if(currentBalance - tempBalance <0.01)
            {
                currentBalance = tempBalance;
                balanceText.text = currentBalance.ToString("C",new CultureInfo("en-US"));
            }


            if((currentBalance-tempBalance)>2.00f)
            {
                
                currentBalance -= 1.00f * Time.deltaTime * 10.00f;
                balanceText.text = currentBalance.ToString("C",new CultureInfo("en-US"));
            }
            else if((currentBalance-tempBalance)>1.00f)
            {
                currentBalance -= 0.25f * Time.deltaTime * 10.00f;
                balanceText.text = currentBalance.ToString("C",new CultureInfo("en-US"));
            }
            else if((currentBalance-tempBalance)>0.25f)
            {
                currentBalance -= 0.10f * Time.deltaTime * 10.00f;
                balanceText.text = currentBalance.ToString("C",new CultureInfo("en-US"));
            }

            else
            {
                currentBalance -= 0.05f * Time.deltaTime * 10.00f;
                balanceText.text = currentBalance.ToString("C",new CultureInfo("en-US"));
            }
            
           
            
            
        }
        else if(currentBalance < tempBalance)
        {
            if(tempBalance-currentBalance<0.01)
            {
                currentBalance = tempBalance;
                balanceText.text = currentBalance.ToString("C",new CultureInfo("en-US"));
            }

            Debug.Log(currentBalance);
            Debug.Log(tempBalance);
            if((tempBalance-currentBalance)>50.00f)
            {
                currentBalance += 25.00f * Time.deltaTime * 10.00f;
                balanceText.text = currentBalance.ToString("C",new CultureInfo("en-US"));
            }
            else if((tempBalance-currentBalance)>2.00f)
            {
                currentBalance += 1.00f * Time.deltaTime * 10.00f;
                balanceText.text = currentBalance.ToString("C",new CultureInfo("en-US"));
            }
            else if((tempBalance-currentBalance)>1.00f)
            {
                currentBalance += 0.25f * Time.deltaTime * 10.00f;
                balanceText.text = currentBalance.ToString("C",new CultureInfo("en-US"));
            }
            else if((tempBalance-currentBalance)>0.25f)
            {
                currentBalance += 0.10f * Time.deltaTime * 10.00f;
                balanceText.text = currentBalance.ToString("C",new CultureInfo("en-US"));
            }
            else
            {
                
                currentBalance += 0.05f * Time.deltaTime * 10.00f;
                balanceText.text = currentBalance.ToString("C",new CultureInfo("en-US"));

            }
            

            
            
        }
        else
        {
            currentBalance = tempBalance;
            balanceText.text = currentBalance.ToString("C",new CultureInfo("en-US"));
        }
        
        
    }


*/
/*

    private IEnumerator ScoreUpdater()
{
    while(true)
    {
        if(tempBalance < currentBalance)
        {
            currentBalance -= 0.05f; 
            balanceText.text = currentBalance.ToString("C",new CultureInfo("en-US")); 
        }
        
        if(tempBalance > currentBalance)
        {
            currentBalance += 0.05f; 
            balanceText.text = currentBalance.ToString("C",new CultureInfo("en-US")); 
        }
        yield return new WaitForSeconds(0.05f); 
    }

    


}

*/

private IEnumerator IncrementCoroutine ()
	{
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
        tempBalance = currentBalance + add;
        
        StartCoroutine(IncrementCoroutine());
        

        
    }
    public void SubtractFromBalance(float subtract)
    {
        tempBalance = currentBalance - subtract;

        StartCoroutine(DecrementCoroutine());
        
        
        
    }

    public void AddToLastWin(float add)
    {
        currentLastWin += add;
        lastWinText.text = currentLastWin.ToString("C",new CultureInfo("en-US"));
    }
    public void ResetLastWin()
    {
        currentLastWin = 0.00f;
        lastWinText.text = currentLastWin.ToString("C",new CultureInfo("en-US"));
    }
    

   

   

}
