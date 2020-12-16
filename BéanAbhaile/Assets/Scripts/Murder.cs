using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Murder : MonoBehaviour
{
    public GameObject gameManager;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player") gameManager.GetComponent<LoadScene>().EndLoad();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player") gameManager.GetComponent<LoadScene>().EndLoad();
    }
}
