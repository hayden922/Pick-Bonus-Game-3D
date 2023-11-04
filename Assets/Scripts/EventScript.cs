using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventScript : MonoBehaviour
{

    [SerializeField] GameObject playMenu;
    GameObject pumpkin;

    

    [SerializeField] EventSystem eventSystem;
    // Start is called before the first frame update
    
    
    
    
    void Start()
    {
       

    }

    // Update is called once per frame
    void Update()
    {
        
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
