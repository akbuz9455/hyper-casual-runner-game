using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeSpawner : MonoBehaviour
{
    public GameObject startReferance, endReferance;
    public BoxCollider hiddenPlatform;
    void Start()
    {
        Vector3 direction = endReferance.transform.position - startReferance.transform.position;
        //yön vektörü elde edildi
        float distance = direction.magnitude; //iki vectör arasi uzunluk

        direction = direction.normalized; //hesaplanabilmesi için aradaki mesafeyi normal hale getiriyoruz

        hiddenPlatform.transform.forward = direction;

        //buraya kadar yazdımız kodlar colliderin yönünü ayarlamak içindi....



        hiddenPlatform.size = new Vector3(hiddenPlatform.size.x,hiddenPlatform.size.y,distance);
        //boyutlandırma işlemi tamamlandı


        hiddenPlatform.transform.position = startReferance.transform.position + (direction * distance / 2)+(new Vector3(0,-direction.z,direction.y)*hiddenPlatform.size.y/2);
        hiddenPlatform.size = new Vector3(hiddenPlatform.size.x,hiddenPlatform.size.y,hiddenPlatform.size.z);
        //collider iki referans noktasının ortasına getirildi
    }

   
}
