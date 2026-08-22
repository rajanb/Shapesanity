using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hole : MonoBehaviour
{
    public Shape.Type type;
    public bool unlocked = true;

    public void UnlockHole()
    {
        unlocked = true;
        GetComponent<Image>().color = new Color(0, 0, 0, 1);
    }

    public void LockHole()
    {
        unlocked = false;
        GetComponent<Image>().color = new Color(1, 1, 1, 0);
    }
}
