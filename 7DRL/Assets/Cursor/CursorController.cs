using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    public Texture2D cursorNormal;
    public Texture2D cursorClick;
  
    // Start is called before the first frame update
    void Start()
    {
        Cursor.SetCursor(cursorNormal, Vector2.zero, CursorMode.ForceSoftware);
        GetComponent<AudioSource>().Stop();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0)){
            Cursor.SetCursor(cursorClick, Vector2.zero, CursorMode.ForceSoftware);
            GetComponent<AudioSource>().Play();
        } else if (Input.GetMouseButtonUp(0)){
            Cursor.SetCursor(cursorNormal, Vector2.zero, CursorMode.ForceSoftware);
            GetComponent<AudioSource>().Stop();
        }
    }
}
