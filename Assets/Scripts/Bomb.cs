using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Bomb : MonoBehaviour
{
    [SerializeField] TextMesh timerText;
    [SerializeField] int count;

    int timer;

    public void LoadBomb()
    {
        timer = count;
        timerText.text = timer.ToString();
    }

    public void SetTimer()
    {
        timer--;
        timerText.text = timer.ToString();
        // DOTween
        if (timer <= 0)
            ExplodeBomb();
    }

    void ExplodeBomb()
    {
        //BombManager.Instance.RemoveBombFromList(gameObject);
        GameManager.Instance.GameOver(gameObject);
    }
}
