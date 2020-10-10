using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PoolManager : MonoBehaviour
{
    // Parent
    public Transform grid;
    // Pool
    public List<GameObject> objectPool;
    // Prefab
    public GameObject Prefab;

    public void InitPool(int size)
    {
        objectPool = new List<GameObject>();
        for(int i = 0; i < size * 2; i++)
        {
            GameObject go = Instantiate(Prefab, transform.position, Quaternion.identity);
            go.transform.parent = grid;
            objectPool.Add(go);
            objectPool[i].SetActive(false);
        }
    }

    public GameObject GetObjFromPool(Vector3 pos,Quaternion rot)
    {
        GameObject newObject = objectPool[objectPool.Count - 1];
        newObject.SetActive(true);
        if (newObject.CompareTag("Fx"))
            newObject.transform.position = pos;
        else
            newObject.transform.DOMove(pos, 1f, false);
        newObject.transform.rotation = rot;
        objectPool.RemoveAt(objectPool.Count - 1);
        return newObject;
    }

    public void ReturnObjToPool(GameObject go)
    {
        go.SetActive(false);
        go.transform.position = transform.position;
        objectPool.Add(go);
    }
}
