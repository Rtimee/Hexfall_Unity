using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public List<GameObject> selectedGroup = new List<GameObject>();
    public List<Vector2> selectedGroupPosition = new List<Vector2>();
    public List<Vector2Int> selectedGroupsGridPosition = new List<Vector2Int>();
    public List<GameObject> explosionList = new List<GameObject>();

    public GameObject[] pointers;

    public Camera camera;

    public bool canMove = true;
    public bool isGameOver;
    public bool isGameStart;

    public int score;
    public int scoreMultiplier;
    public int bombSpawnScore;

    public PoolManager fxPool;

    Vector2 pointerStartPosition;

    int spawnedBomb = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        pointerStartPosition = pointers[0].transform.position;
        fxPool.InitPool(10);
    }

    public void SelectHexagon(List<GameObject> hexagons)
    {
        selectedGroup.Clear();
        for (int i = 0; i < hexagons.Count; i++)
        {
            selectedGroup.Add(hexagons[i]);
            pointers[i].SetActive(true);
            pointers[i].transform.position = hexagons[i].transform.position;
            pointers[i].transform.parent = hexagons[i].transform;           
        }
    }

    private void Update()
    {
        if (isGameStart && !isGameOver)
        {
            if (SwipeManager.swipeLeft && canMove && selectedGroup.Count > 0 || SwipeManager.swipeDown && canMove && selectedGroup.Count > 0)
            {
                StartCoroutine(TurnLeft(false));
            }
            else if (SwipeManager.swipeRight && canMove && selectedGroup.Count > 0 || SwipeManager.swipeUp && canMove && selectedGroup.Count > 0)
            {
                StartCoroutine(TurnRight(false));
            }
        }
        else if (!isGameStart)
        {
            if (Input.GetMouseButtonDown(0))
                StartGame();
        }
        if (isGameOver)
            if (Input.GetMouseButtonDown(0))
                RestartGame();
    }

    void StartGame()
    {
        UIManager.Instance.infoPanel.SetActive(false);
        UIManager.Instance.gamePanel.SetActive(true);
        isGameStart = true;
    }

    IEnumerator TurnLeft(bool revert)
    {
        canMove = false;

        CalculateCoords();

        selectedGroup[0].transform.DOMove(selectedGroupPosition[1], .5f);
        selectedGroup[1].transform.DOMove(selectedGroupPosition[2], .5f);
        selectedGroup[2].transform.DOMove(selectedGroupPosition[0], .5f);

        GridManager.Instance.SetGrid(selectedGroup[2], new Vector2Int(selectedGroupsGridPosition[0].x, selectedGroupsGridPosition[0].y));
        GridManager.Instance.SetGrid(selectedGroup[0], new Vector2Int(selectedGroupsGridPosition[1].x, selectedGroupsGridPosition[1].y));
        GridManager.Instance.SetGrid(selectedGroup[1], new Vector2Int(selectedGroupsGridPosition[2].x, selectedGroupsGridPosition[2].y));

        CalculateCoords();
        if (!revert)
        {
            DetectMatching();
            if (explosionList.Count == 0)
            {
                yield return new WaitForSeconds(.5f);
                StartCoroutine(TurnRight(true));
            }
            else
            {
                StartCoroutine(Explode());
            }
        }
        else
            StartCoroutine(CanMove());
    }

    IEnumerator TurnRight(bool revert)
    {
        canMove = false;
        CalculateCoords();

        selectedGroup[0].transform.DOMove(selectedGroupPosition[2], .5f);
        selectedGroup[1].transform.DOMove(selectedGroupPosition[0], .5f);
        selectedGroup[2].transform.DOMove(selectedGroupPosition[1], .5f);

        GridManager.Instance.SetGrid(selectedGroup[1], new Vector2Int(selectedGroupsGridPosition[0].x, selectedGroupsGridPosition[0].y));
        GridManager.Instance.SetGrid(selectedGroup[2], new Vector2Int(selectedGroupsGridPosition[1].x, selectedGroupsGridPosition[1].y));
        GridManager.Instance.SetGrid(selectedGroup[0], new Vector2Int(selectedGroupsGridPosition[2].x, selectedGroupsGridPosition[2].y));

        CalculateCoords();
        if (!revert)
        {
            DetectMatching();
            if (explosionList.Count == 0)
            {
                yield return new WaitForSeconds(.5f);
                StartCoroutine(TurnLeft(true));
            }
            else
            {
                StartCoroutine(Explode());
            }
        }
        else
            StartCoroutine(CanMove());
    }

    void CalculateCoords()
    {
        selectedGroupPosition.Clear();
        selectedGroupsGridPosition.Clear();
        foreach (GameObject hexa in selectedGroup)
        {
            selectedGroupPosition.Add(hexa.transform.position);
            selectedGroupsGridPosition.Add(new Vector2Int(hexa.GetComponent<Hexagon>().gridPosition.x, hexa.GetComponent<Hexagon>().gridPosition.y));
        }
    }

    IEnumerator CanMove()
    {
        yield return new WaitForSeconds(.5f);
        explosionList.Clear();
        canMove = true;
    }

    public void ExplodeBlocks(List<GameObject> hexagons)
    {
        explosionList.Clear();
        foreach (GameObject hexa in hexagons)
        {
            explosionList.Add(hexa);
        }
    }

    void DetectMatching()
    {
        foreach (GameObject hexa in selectedGroup)
        {
            hexa.GetComponent<Hexagon>().CheckMatching();
        }
    }

    IEnumerator Explode()
    {
        yield return new WaitForSeconds(.5f);

        SetPointerPosition();
        PrepareForRay();
        StartCoroutine(CanMove());
        BombManager.Instance.MoveCounter();
    }

    void PrepareForRay()
    {
        int xValue1 = explosionList[0].GetComponent<Hexagon>().gridPosition.x;
        int xValue2 = explosionList[1].GetComponent<Hexagon>().gridPosition.x;
        int xValue3 = explosionList[2].GetComponent<Hexagon>().gridPosition.x;

        int yValue1 = explosionList[0].GetComponent<Hexagon>().gridPosition.y;
        int yValue2 = explosionList[1].GetComponent<Hexagon>().gridPosition.y;
        int yValue3 = explosionList[2].GetComponent<Hexagon>().gridPosition.y;

        // If x1 equal x2
        if (xValue1 == xValue2)
        {
            if (yValue1 < yValue2)
            {
                explosionList[0].GetComponent<Hexagon>().FireRay(2);
                explosionList[2].GetComponent<Hexagon>().FireRay(1);
                explosionList[1].GetComponent<Hexagon>().DestroyBlock();

            }
            else
            {
                explosionList[1].GetComponent<Hexagon>().FireRay(2);
                explosionList[2].GetComponent<Hexagon>().FireRay(1);
                explosionList[0].GetComponent<Hexagon>().DestroyBlock();
            }
            return;
        }
        // If x1 equal x3
        if (xValue1 == xValue3)
        {
            if (yValue1 < yValue3)
            {
                explosionList[0].GetComponent<Hexagon>().FireRay(2);
                explosionList[1].GetComponent<Hexagon>().FireRay(1);
                explosionList[2].GetComponent<Hexagon>().DestroyBlock();
            }
            else
            {
                explosionList[2].GetComponent<Hexagon>().FireRay(2);
                explosionList[1].GetComponent<Hexagon>().FireRay(1);
                explosionList[0].GetComponent<Hexagon>().DestroyBlock();
            }
            return;
        }
        // If x2 equal x3
        else
        {
            if (yValue2 < yValue3)
            {
                explosionList[1].GetComponent<Hexagon>().FireRay(2);
                explosionList[0].GetComponent<Hexagon>().FireRay(1);
                explosionList[2].GetComponent<Hexagon>().DestroyBlock();
            }
            else
            {
                explosionList[2].GetComponent<Hexagon>().FireRay(2);
                explosionList[0].GetComponent<Hexagon>().FireRay(1);
                explosionList[1].GetComponent<Hexagon>().DestroyBlock();
            }
            return;
        }
    }

    void SetPointerPosition()
    {
        GameManager.Instance.selectedGroup.Clear();
        foreach (GameObject pointer in pointers)
        {
            pointer.SetActive(false);
            pointer.transform.parent = null;
            pointer.transform.position = pointerStartPosition;
        }
    }

    public void UpdateScore()
    {
        score += scoreMultiplier;
        UIManager.Instance.scoreText.text = "Score : " + score;

        if (score >= bombSpawnScore + (bombSpawnScore * spawnedBomb))
        {
            BombManager.Instance.canSpawnBomb = true;
            spawnedBomb++;
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadSceneAsync(0);
    }

    public GameObject SpawnFx(Vector3 pos)
    {
        return fxPool.GetObjFromPool(pos, Quaternion.identity);
    }

    public void GameOver(GameObject bomb)
    {
        isGameOver = true;
        BombManager.Instance.SpawnFx(bomb);
        camera.transform.DOShakePosition(1f, .5f, 10, 90, false, true).OnComplete(delegate {
            UIManager.Instance.gamePanel.SetActive(false);
            UIManager.Instance.gameOverScoreText.text = "Your Score : " + score;
            UIManager.Instance.gameOverScreen.SetActive(true);
        });
    }
}
