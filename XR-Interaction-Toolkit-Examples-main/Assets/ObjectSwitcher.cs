using UnityEngine;

public class ObjectSwitcher : MonoBehaviour
{
    public GameObject objectA;
    public GameObject objectB;
    public GameObject objectC;
    public GameObject objectD;
    public GameObject objectE;
    public GameObject objectF;
    private int currentObject = 0;

    // Method to switch the objects
    public void SwitchObjects()
    {
        // Deactivate all objects
        objectA.SetActive(false);
        objectB.SetActive(false);
        objectC.SetActive(false);
        objectD.SetActive(false);
        objectE.SetActive(false);
        objectF.SetActive(false);

        // Determine which object to activate
        currentObject = (currentObject + 1) % 6; // Cycle through them

        switch (currentObject)
        {
            case 0:
                objectA.SetActive(true);
                break;
            case 1:
                objectB.SetActive(true);
                break;
            case 2:
                objectC.SetActive(true);
                break;
            case 3:
                objectD.SetActive(true);
                break;
            case 4:
                objectE.SetActive(true);
                break;
            case 5:
                objectF.SetActive(true);
                break;
        }
    }
}
