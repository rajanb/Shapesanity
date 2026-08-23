using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ui_Menu_Cursor : MonoBehaviour
{
    public GameObject cursorImage;
    public GameObject cursorObject;
    public Sprite[] cursorSprite; 

    // Start is called before the first frame update
    void Start()
    {
       // Cursor.visible = false;

    }
    private void Update()
    {
        cursorObject.transform.position = Input.mousePosition;
    }
    public void UpdateCursor(int spriteID)
    {
        cursorImage.GetComponent<Image>().sprite = cursorSprite[spriteID];
    }
}
