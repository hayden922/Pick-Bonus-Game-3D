using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class EventScript : MonoBehaviour
{

    [SerializeField] GameObject playMenu;

    GameObject pumpkin;

    [SerializeField] EventSystem eventSystem; //object this script is attached to.

    private Vector3 tmpMousePosition;
    
    
    
    
    void Start()
    {
       tmpMousePosition = Input.mousePosition; //setting temporary mouse position to the inital input mouse position to check whether user uses their mouse or not.

    }

   
    void Update()
    {

        //if statment for if the user decideds to use keyboard for navigation.
       if (Input.GetKeyDown(KeyCode.UpArrow)||Input.GetKeyDown(KeyCode.DownArrow)||Input.GetKeyDown(KeyCode.RightArrow)||Input.GetKeyDown(KeyCode.LeftArrow)||Input.GetKeyDown(KeyCode.W)||Input.GetKeyDown(KeyCode.S)||Input.GetKeyDown(KeyCode.D)||Input.GetKeyDown(KeyCode.A))
       {
        
        if(eventSystem.currentSelectedGameObject == null)
        {
            //if not object is currently selected when switching to using keyboard use SwitchToPlayMenu() or SwitchToPumpkins() to select correct button.
             if(GameController.inGame == false)
            {
            SwitchToPlayMenu();
            }
            
            else
            {
             SwitchToPumpkins();
            }


        }

       

       

    }


    
        if (tmpMousePosition != Input.mousePosition){
            //if user moves mouse switch to mouse navigation, calling SwitchToNull() to unselect current button used in keyboard navigation.
            SwitchToNull();
            tmpMousePosition = Input.mousePosition;
    
        }
        
    }
    public void SwitchToNull()
    {
        //method used to reset selection of event system.
        eventSystem.SetSelectedGameObject(null);
    }

    public void SwitchToPumpkins()
    {
        //if in game
        //method loops through selectablePumpkins array and finds first index that does not contain null object and sets that to currently selected pumpkin button.
        //allows for automatic selection of first available pumpkin if user is using keyboard navigation.
        
        for (int i = 0; i < GameController.selectablePumpkins.Length; i++)
        {
            if(GameController.selectablePumpkins[i] != null)
            {
                eventSystem.SetSelectedGameObject(GameController.selectablePumpkins[i]);
                return;
            }
        }
        
        
    }
    
    public void SwitchToPlayMenu()
    {
        //switchs selection of event system to play button.

        eventSystem.SetSelectedGameObject(playMenu);

    }


}
