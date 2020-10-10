using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombManager : MonoBehaviour
{
    public static BombManager Instance;

    public List<GameObject> bombs = new List<GameObject>();
    public GameObject fx;
    public bool canSpawnBomb;

    [SerializeField] PoolManager bombPool;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        bombPool.InitPool(10);
    }

    public GameObject SpawnBomb(Vector3 pos,Quaternion rot)
    {
        canSpawnBomb = false;
        GameObject _bomb = bombPool.GetObjFromPool(pos, rot);
        _bomb.GetComponent<Bomb>().LoadBomb();
        AddBombToList(_bomb);

        return _bomb;
    }

    void AddBombToList(GameObject bomb)
    {
        bombs.Add(bomb);
    }

    public void RemoveBombFromList(GameObject bomb)
    {
        StartCoroutine(CalculateSameColors(bomb));
        bomb.GetComponent<Hexagon>().FireRay(1);
        bombs.Remove(bomb);
        bombPool.ReturnObjToPool(bomb);
    }

    public void MoveCounter()
    {
        foreach (GameObject bombObj in bombs)
        {
            bombObj.GetComponent<Bomb>().SetTimer();
        }
    }

    IEnumerator CalculateSameColors(GameObject bomb)
    {
        foreach (GameObject hexa in GridManager.Instance.grid)
        {
            if (bomb.GetComponent<Hexagon>().GetColor() == hexa.GetComponent<Hexagon>().GetColor())
            {
                yield return new WaitForSeconds(.1f);
                hexa.GetComponent<Hexagon>().FireRay(1);
            }
        }
    }

    public void SpawnFx(GameObject bomb)
    {
        Instantiate(fx, bomb.transform.position, Quaternion.identity);
    }
}
