using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raycasting : MonoBehaviour
{
    RaycastHit2D hit;

    Camera camera;
    List<GameObject> tempList = new List<GameObject>();

    private void Awake()
    {
        camera = GetComponent<Camera>();
    }

    void Raycast()
    {
        hit = Physics2D.Raycast(camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (hit != null && hit.transform.CompareTag("Hexagon"))
        {
            GameManager.Instance.SelectHexagon(hit.transform.GetComponent<Hexagon>().SelectARandomGroup());     
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0) && GameManager.Instance.canMove && GameManager.Instance.isGameStart && !GameManager.Instance.isGameOver)
            Raycast();
    }
}
