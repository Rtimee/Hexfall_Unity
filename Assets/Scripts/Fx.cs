using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fx : MonoBehaviour
{
    public void DestroyFx()
    {
        GameManager.Instance.fxPool.ReturnObjToPool(gameObject);
    }

    public void InvokeFunc()
    {
        Invoke("DestroyFx", 1f);
    }
}
