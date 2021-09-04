using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public bool isPowered;
    public bool isOpened;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void Open()
    {
        if (!isOpened)
        {
            transform.position += Vector3.up * 4;
            isOpened = true;
        }
    }

    public void Close()
    {
        if (isOpened)
        {
            transform.position -= Vector3.up * 4;
            isOpened = false;
        }
    }

    public void Switch()
    {
        if (isOpened)
        {
            Close();
        }
        else 
        {
            Open();
        }
    }
}
