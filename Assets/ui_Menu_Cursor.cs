using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ui_Menu_Cursor : MonoBehaviour
{
    public GameObject cursorImage;
    public Vector3 clickerOffset = new Vector3(0f, -300f, 0f);

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;

    }
    private void Update()
    {
        cursorImage.transform.position = Input.mousePosition + clickerOffset;
    }
}
