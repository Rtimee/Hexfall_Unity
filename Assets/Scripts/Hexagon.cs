using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Hexagon : MonoBehaviour
{
    public List<GameObject> neigbours = new List<GameObject>();
    public List<GameObject> selectedRandomGroup = new List<GameObject>();
    public List<GameObject> detectedSameColorBlock = new List<GameObject>();
    public List<GameObject> sameColumnList = new List<GameObject>();

    public Vector2Int gridPosition;

    public bool willExplode;
    public bool isBomb;

    Color hexagonColor;
    SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void LoadHexagon()
    {
        ClearAllList();
        willExplode = false;
        hexagonColor = ColorManager.Instance.GetColor();
        spriteRenderer.color = hexagonColor;
    }

    void ClearAllList()
    {
        neigbours.Clear();
        selectedRandomGroup.Clear();
        detectedSameColorBlock.Clear();
        sameColumnList.Clear();
    }

    void FindNeigbours()
    {
        neigbours.Clear();
        if (gridPosition.x % 2 == 0 && gridPosition.x != GridManager.Instance.GetWidth() - 1 && gridPosition.y != GridManager.Instance.GetHeight() - 1)
        {
            if (gridPosition.x > 0)
            {
                neigbours.Add(GridManager.Instance.grid[gridPosition.x - 1, gridPosition.y]);
                if (gridPosition.y < GridManager.Instance.GetHeight() - 1)
                    neigbours.Add(GridManager.Instance.grid[gridPosition.x - 1, gridPosition.y + 1]);
            }
            if (gridPosition.y < GridManager.Instance.GetHeight() - 1)
            {
                neigbours.Add(GridManager.Instance.grid[gridPosition.x, gridPosition.y + 1]);
                if (gridPosition.x < GridManager.Instance.GetWidth() - 1)
                    neigbours.Add(GridManager.Instance.grid[gridPosition.x + 1, gridPosition.y + 1]);
            }
            if (gridPosition.x < GridManager.Instance.GetWidth() - 1)
                neigbours.Add(GridManager.Instance.grid[gridPosition.x + 1, gridPosition.y]);
            if (gridPosition.y > 0)
                neigbours.Add(GridManager.Instance.grid[gridPosition.x, gridPosition.y - 1]);
        }
        else if (gridPosition.x == GridManager.Instance.GetWidth() - 1 && gridPosition.y == GridManager.Instance.GetHeight() - 1)
        {
            neigbours.Add(GridManager.Instance.grid[gridPosition.x, gridPosition.y - 1]);
            neigbours.Add(GridManager.Instance.grid[gridPosition.x - 1, gridPosition.y]);
        }
        else
        {
            if (gridPosition.x == GridManager.Instance.GetWidth() - 1 && gridPosition.y > 0)
            {
                
                neigbours.Add(GridManager.Instance.grid[gridPosition.x, gridPosition.y - 1]);
                neigbours.Add(GridManager.Instance.grid[gridPosition.x - 1, gridPosition.y - 1]);
                neigbours.Add(GridManager.Instance.grid[gridPosition.x - 1, gridPosition.y]);
                neigbours.Add(GridManager.Instance.grid[gridPosition.x, gridPosition.y + 1]);
            }
            else
            {
                if (gridPosition.x > 0)
                {
                    if (gridPosition.y > 0 && gridPosition.x % 2 != 0)
                        neigbours.Add(GridManager.Instance.grid[gridPosition.x - 1, gridPosition.y - 1]);
                    neigbours.Add(GridManager.Instance.grid[gridPosition.x - 1, gridPosition.y]);
                    if (gridPosition.y == 0 && gridPosition.x == GridManager.Instance.GetWidth() - 1 && gridPosition.y > 0)
                        neigbours.Add(GridManager.Instance.grid[gridPosition.x - 1, gridPosition.y + 1]);
                }
                if (gridPosition.y < GridManager.Instance.GetHeight() - 1)
                    neigbours.Add(GridManager.Instance.grid[gridPosition.x, gridPosition.y + 1]);
                if (gridPosition.x < GridManager.Instance.GetWidth() - 1)
                    neigbours.Add(GridManager.Instance.grid[gridPosition.x + 1, gridPosition.y]);
                if (gridPosition.y > 0)
                {
                    if (gridPosition.x < GridManager.Instance.GetWidth() - 1 && gridPosition.x > 0 && gridPosition.x % 2 != 0)
                        neigbours.Add(GridManager.Instance.grid[gridPosition.x + 1, gridPosition.y - 1]);
                    neigbours.Add(GridManager.Instance.grid[gridPosition.x, gridPosition.y - 1]);
                }
            }
        }
    }

    public List<GameObject> SelectARandomGroup()
    {
        FindNeigbours();

        selectedRandomGroup.Clear();
        int x = Random.Range(0, neigbours.Count);
        if (neigbours.Count == 5 && x == 1)
            x++;
        if(neigbours.Count == 3 && gridPosition.y == GridManager.Instance.GetHeight() - 1)
            if (x == 0)
                x++;
        selectedRandomGroup.Add(neigbours[x]);
        x++;
        if (x > neigbours.Count - 1)
        {
            if (neigbours.Count == 6 || neigbours.Count == 3 && gridPosition.y == GridManager.Instance.GetHeight() - 1)
                x = 0;
            else
                x -= 2;
        }
        selectedRandomGroup.Add(neigbours[x]);
        selectedRandomGroup.Add(gameObject);

        return selectedRandomGroup;
    }

    public void CheckMatching()
    {
        FindNeigbours();
        detectedSameColorBlock.Clear();
        for (int i = 0; i < neigbours.Count; i++)
        {
            if (i < neigbours.Count - 1)
            {
                if (neigbours[i].GetComponent<Hexagon>().GetColor() == hexagonColor && neigbours[i + 1].GetComponent<Hexagon>().GetColor() == hexagonColor)
                {
                    willExplode = true;
                    if (!neigbours[i].GetComponent<Hexagon>().willExplode && !neigbours[i + 1].GetComponent<Hexagon>().willExplode)
                    {
                        detectedSameColorBlock.Add(neigbours[i]);
                        detectedSameColorBlock.Add(neigbours[i + 1]);
                        detectedSameColorBlock.Add(gameObject);
                        i += 2;
                    }
                }
            }
            else if (i == neigbours.Count - 1 && gridPosition.y > 0 && gridPosition.x < GridManager.Instance.GetWidth() - 1)
            {
                if (neigbours[i].GetComponent<Hexagon>().GetColor() == hexagonColor && neigbours[0].GetComponent<Hexagon>().GetColor() == hexagonColor)
                {
                    if (!neigbours[i].GetComponent<Hexagon>().willExplode && !neigbours[0].GetComponent<Hexagon>().willExplode)
                    {
                        detectedSameColorBlock.Add(neigbours[i]);
                        detectedSameColorBlock.Add(neigbours[0]);
                        detectedSameColorBlock.Add(gameObject);
                    }
                }
            }
        }
        if (detectedSameColorBlock.Count >= 2)
            GameManager.Instance.ExplodeBlocks(detectedSameColorBlock);

    }

    public void FireRay(int MoveYValue)
    {
        sameColumnList.Clear();
        RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position, Vector2.up, 1000f);
        if (hit.Length > 1)
        {
            for (int i = 1; i < hit.Length; i++)
            {
                sameColumnList.Add(hit[i].transform.gameObject);
                if (i == hit.Length - 1)
                    hit[i].transform.GetComponent<Hexagon>().TriggerBlocks(MoveYValue, true);
                else
                    hit[i].transform.GetComponent<Hexagon>().TriggerBlocks(MoveYValue, false);
            }
        }
        if (sameColumnList.Count == 0)
        {
            Debug.Log("!!");
            CreateBlock(1, 0);
        }
        if (MoveYValue == 2 && sameColumnList.Count == 1)
        {
            if (gridPosition.y < GridManager.Instance.GetHeight() - 1)
                CreateBlock(2, 1);
        }

        Invoke("DestroyBlock",.2f);
    }

    public void DestroyBlock()
    {
        GameObject fx = GameManager.Instance.SpawnFx(transform.position);
        fx.GetComponent<Fx>().InvokeFunc();
        GridManager.Instance.ReturnHexagonToPool(gameObject);
        GameManager.Instance.UpdateScore();
    }

    IEnumerator MoveY(int MoveYValue, bool isLastPart)
    {
        yield return new WaitForSeconds(.2f);
        transform.DOMoveY(transform.position.y - 0.725f * MoveYValue, .2f, false).OnComplete(delegate {
            if (isLastPart)
                CreateBlock(MoveYValue, 0);
            GridManager.Instance.SetGrid(gameObject, new Vector2Int(gridPosition.x, gridPosition.y - MoveYValue));
        });
    }

    public void TriggerBlocks(int MoveYValue, bool isLastPart)
    {
        StartCoroutine(MoveY(MoveYValue, isLastPart));
    }

    public Color GetColor()
    {
        return hexagonColor;
    }

    void CreateBlock(int count, int y)
    {
        if (count == 2)
        {
            StartCoroutine(GridManager.Instance.CreateNewBlock(new Vector2Int(gridPosition.x, gridPosition.y - 1 + y), false));
            StartCoroutine(GridManager.Instance.CreateNewBlock(new Vector2Int(gridPosition.x, gridPosition.y + y), false));
        }
        else
            StartCoroutine(GridManager.Instance.CreateNewBlock(new Vector2Int(gridPosition.x, gridPosition.y), true));
    }
}
