using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Interactable
{
    //public GameObject[] approaches { get; set; }
    public void Interact(GameObject who);
}

public interface Breakable
{
    //public GameObject[] approaches { get; set; }
    public void Break(GameObject who);
}
