using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Include this namespace to use TextMeshPro components

public class ScoreManager : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        string expectedTag = gameObject.tag.Replace("Box", "");

        if (other.CompareTag(expectedTag))
        {
            GlobalScoreManager.Instance.UpdateScore(1); // Increase score globally
            Debug.Log("Score: " + GlobalScoreManager.Instance.score);
        }
        else
        {
            GlobalScoreManager.Instance.UpdateScore(0);
            Debug.Log("Incorrect shape. No score addded.");
        }

        Destroy(other.gameObject, 3f); // Destroy the object after 3 seconds
    }
}
