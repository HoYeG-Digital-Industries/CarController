using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreCollectable : CollectableItem
{
    public int scoreValue;

    void Start()
    {
        // Bool check
        // Either randomise the value 
        // OR
        // Set the value by hand
    }

    private void OnTriggerEnter(Collider other) 
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.AddScore(scoreValue);

            HasBeenCollected();
        }
    }
}
