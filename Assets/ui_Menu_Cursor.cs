using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ui_Menu_Cursor : MonoBehaviour
{
    public CursorMode cursorMode = CursorMode.Auto;
    public bool autoCenterHotSpot = false;
    public Vector2 hotSpotCustom = Vector2.zero;
    public GameObject cursorImage;
    private Vector2 hotSpotAuto;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;

        Vector2 hotSpot;
        if (autoCenterHotSpot)
        {
            hotSpot = hotSpotAuto;
        }
        else { hotSpot = hotSpotCustom; }

    }
    private void Update()
    {
        cursorImage.transform.position = Input.mousePosition;
    }
}
