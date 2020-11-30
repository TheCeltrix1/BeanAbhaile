using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reflection : MonoBehaviour
{

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.GetComponentInParent<Banshee>())
        {
            collision.gameObject.GetComponentInParent<Banshee>().reflection = false;
        }
    }

    private void OnTriggerStay(Collider collision)
    {
        if (collision.gameObject.GetComponentInParent<Banshee>() && collision.gameObject.GetComponentInParent<Banshee>().mirrorFace)
        {
            collision.gameObject.GetComponentInParent<Banshee>().reflection = true;
        }
        else if (collision.gameObject.GetComponentInParent<Banshee>() && !collision.gameObject.GetComponentInParent<Banshee>().mirrorFace) collision.gameObject.GetComponentInParent<Banshee>().reflection = false;
    }
}
