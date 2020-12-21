using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIEnabled : MonoBehaviour
{
    public GameObject banshee;
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.GetComponent<Player>())
        {
            banshee.GetComponent<Banshee>().AIEnable();
            Destroy(this.gameObject);
        }
    }
}
