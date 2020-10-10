using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject infoPanel;
    public GameObject gamePanel;
    public GameObject gameOverScreen;
    public Text scoreText;
    public Text gameOverScoreText;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        infoPanel.GetComponent<Image>().DOFade(0f,2.5f).From(1f);
    }
}
