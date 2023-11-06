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

    

    [SerializeField] EventSystem eventSystem;

    private Vector3 tmpMousePosition;
    // Start is called before the first frame update
    
    
    
    
    void Start()
    {
       tmpMousePosition = Input.mousePosition;

    }

    // Update is called once per frame
    void Update()
    {

       if (Input.GetKeyDown(KeyCode.UpArrow)||Input.GetKeyDown(KeyCode.DownArrow)||Input.GetKeyDown(KeyCode.RightArrow)||Input.GetKeyDown(KeyCode.LeftArrow)||Input.GetKeyDown(KeyCode.W)||Input.GetKeyDown(KeyCode.S)||Input.GetKeyDown(KeyCode.D)||Input.GetKeyDown(KeyCode.A))
       {

        if(eventSystem.currentSelectedGameObject == null)
        {

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
            
            SwitchToNull();
            tmpMousePosition = Input.mousePosition;
    
        }
        
    }
    public void SwitchToNull()
    {
        eventSystem.SetSelectedGameObject(null);
    }

    public void SwitchToPumpkins()
    {
        
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

        eventSystem.SetSelectedGameObject(playMenu);

    }


}
