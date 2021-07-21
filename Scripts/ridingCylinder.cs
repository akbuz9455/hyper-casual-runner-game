using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ridingCylinder : MonoBehaviour
{


    private bool _filled; //silindirin tamamen dolup dolmadiğini tutacağiz

    private float _value; //silindirin ne kadar dolup dolmadiğini gösterecek 
  
     public void IncrementCylinderVolume(float value)
    {

        _value += value;
        if (_value > 1)
        {
            float leftValue = _value - 1;
            int cylinderCount = playerController.Current.cylinders.Count;
            transform.localPosition = new Vector3(transform.localPosition.x,-0.5f*(cylinderCount-1)+(-0.25f),transform.localPosition.z);
            //yedek//  transform.localScale = new Vector3(0.5f,transform.localScale.y,0.5f);
            transform.localScale = new Vector3(0.75f, transform.localScale.y,0.75f);
            //silindirin boyutunu tam olarak 1 yap
            //1 den ne kadar büyükse o büyüklükte silindir oluştur
            playerController.Current.CreateCylinder(leftValue);


        }
        else if (_value<0)
        {
            //karakterimize bu silindiri yoketmemizi söyliyeceğiz.

            playerController.Current.DestroyCylinder(this);
        }
        else
        {
            //silindirin boyutunu güncelleyeceğiz.
            int cylinderCount = playerController.Current.cylinders.Count;
            transform.localPosition = new Vector3(transform.localPosition.x, -0.5f * (cylinderCount - 1) + (-0.25f*_value), transform.localPosition.z);
            transform.localScale = new Vector3(0.5f * _value, transform.localScale.y, 0.5f * _value);

        }
    }
}
