using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonScript : MonoBehaviour
{


    [SerializeField] EventSystem eventSystem;

    

     public void Selected()
    {
            eventSystem.SetSelectedGameObject(this.gameObject);
    }


 
   
}
