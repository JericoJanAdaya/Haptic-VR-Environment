using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

public class ColScript : MonoBehaviour
{
    bool isInContact = false;
    List<string> validTags = new List<string> { "Object", "Cube", "Cylinder", "Pyramid" };

    void OnCollisionStay(Collision col)
    {
        if (validTags.Contains(col.gameObject.tag))
        {
            isInContact = true;
        }
    }

    void OnCollisionExit(Collision col)
    {
        if (validTags.Contains(col.gameObject.tag))
        {
            isInContact = false;
        }
    }

    public bool IsInContact()
    {
        return isInContact;
    }
}
