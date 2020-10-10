using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorManager : MonoBehaviour
{
    public static ColorManager Instance;

    [SerializeField] Color[] colors;

    private void Awake()
    {
        Instance = this;
    }

    public Color GetColor()
    {
        int random = Random.Range(0, colors.Length);
        Color newColor = colors[random];

        return newColor;
    }
}
