using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    public GameObject[,] grid;

    [SerializeField] PoolManager hexagonPool;

    [SerializeField] int width;
    [SerializeField] int height;

    float offsetX = .66f;
    float offsetY = .35f;
    float offsetYNextRow = .725f;

    Vector2 startPosition;
    Vector2 currentPosition;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        startPosition = transform.position;
        currentPosition = startPosition;
        hexagonPool.InitPool(width * height);
        CreateGrid();
    }

    void CreateGrid()
    {
        grid = new GameObject[width, height];
        for (int y = 0; y < height; y++)
        {
            currentPosition = new Vector2(currentPosition.x, startPosition.y + offsetYNextRow * y);
            for (int x = 0; x < width; x++)
            {
                if (x % 2 == 0)
                    currentPosition = new Vector2(startPosition.x + x * offsetX, currentPosition.y + offsetY);
                else
                    currentPosition = new Vector2(startPosition.x + x * offsetX, currentPosition.y - offsetY);

                GameObject hexa = hexagonPool.GetObjFromPool(currentPosition, Quaternion.identity);
                hexa.GetComponent<Hexagon>().LoadHexagon();
                SetGrid(hexa, new Vector2Int(x, y));
            }
        }
        //StartCoroutine(CheckAllHexagons());
    }

    public void SetGrid(GameObject hexa,Vector2Int gridPosition)
    {
        grid[gridPosition.x, gridPosition.y] = hexa;
        hexa.GetComponent<Hexagon>().gridPosition = gridPosition;
    }

    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }

    public IEnumerator CreateNewBlock(Vector2Int gridPosition, bool isSingle)
    {
        yield return new WaitForSeconds(.1f);
        Vector2 position;
        if (gridPosition.x % 2 == 0)
        {
            if (isSingle)
                position = new Vector2(startPosition.x + gridPosition.x * offsetX, startPosition.y + offsetYNextRow * gridPosition.y + (offsetYNextRow / 2));
            else
                position = new Vector2(startPosition.x + gridPosition.x * offsetX, startPosition.y + offsetYNextRow * gridPosition.y + (offsetYNextRow / 2));
        }

        else
        {
            if (isSingle)
                position = new Vector2(startPosition.x + gridPosition.x * offsetX, startPosition.y + offsetYNextRow * gridPosition.y);
            else
                position = new Vector2(startPosition.x + gridPosition.x * offsetX, startPosition.y + offsetYNextRow * gridPosition.y);
        }
        GameObject hexa;
        if (!BombManager.Instance.canSpawnBomb)
            hexa = hexagonPool.GetObjFromPool(new Vector2(0, 5), Quaternion.identity);
        else
            hexa = BombManager.Instance.SpawnBomb(new Vector2(0, 5), Quaternion.identity);

        SetGrid(hexa, new Vector2Int(gridPosition.x, gridPosition.y));
        hexa.GetComponent<Hexagon>().LoadHexagon();
        hexa.transform.DOMove(position, 1f, false).OnComplete(delegate {
            StartCoroutine(CheckAllHexagons());
        });

        if (gridPosition.y > height - 1)
            gridPosition.y = height - 1;
    }

    public void ReturnHexagonToPool(GameObject hexa)
    {
        hexagonPool.ReturnObjToPool(hexa);
    }
    
    IEnumerator CheckAllHexagons()
    {
        yield return new WaitForSeconds(1f);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y].GetComponent<Hexagon>().willExplode)
                    grid[x, y].GetComponent<Hexagon>().willExplode = false;
            }
        }
    }
}
