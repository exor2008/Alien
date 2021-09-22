using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeadBodiesControl : MonoBehaviour
{
    public GameObject deadBodyPrefab;
    void Start()
    {
        
    }

    public void SpawnDeadBody(Transform operativeTransform)
    {
        GameObject deadBody = Instantiate(
            deadBodyPrefab, operativeTransform.position, operativeTransform.rotation, transform);
    }

    public GameObject[] GetDeadBodies()
    {
        DeadBody[] deadBodies = GetComponentsInChildren<DeadBody>();
        return deadBodies.ToList().Select(x => x.gameObject).ToArray();
    }

    public void Aquire(GameObject deadBody)
    {
        deadBody.gameObject.transform.SetParent(transform);
    }
}
