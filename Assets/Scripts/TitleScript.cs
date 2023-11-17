using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using DG.Tweening;
using UnityEngine;

public class TitleScript : MonoBehaviour
{
    void Start()
    {
        BobDown();
    }

    void BobUp()
    {
        this.gameObject.transform.DOMove(new UnityEngine.Vector3(this.gameObject.transform.position.x,this.gameObject.transform.position.y+1f,this.gameObject.transform.position.z),2f).SetEase(Ease.OutSine).OnComplete(()=> 
        {
            BobDown();
        });
    }
    void BobDown()
    {
        this.gameObject.transform.DOMove(new UnityEngine.Vector3(this.gameObject.transform.position.x,this.gameObject.transform.position.y-1f,this.gameObject.transform.position.z),2f).SetEase(Ease.InSine).OnComplete(()=> 
        {
            BobUp();
        });
    }

}
