using System;
using UnityEditor.U2D.Sprites;
using UnityEngine;
using TMPro;

public class HeightScore : MonoBehaviour
{
    public GameObject player;
    public TextMeshProUGUI HeightMeter;
    
    [SerializeField] private float highestHeight = 0f;
    [SerializeField] private float currentHeight = 0f;
    [SerializeField] private float multiplier = 10f;
    

    private float score;
    private bool isGameEnd;

    private void Update()
    {
        if (isGameEnd) return;

        currentHeight = player.transform.position.y;
        HeightMeter.text = Mathf.FloorToInt(currentHeight).ToString("0000");
        if (currentHeight > highestHeight)
        {
            highestHeight = currentHeight;
        }
    }

    public void EndGame()
    {
        isGameEnd = true;
        ScoreConvert();
    }

    private void ScoreConvert()
    {
        score = Mathf.Floor(highestHeight * multiplier);
        Debug.Log("Final Score: " + score);
    }

    public float GetScore()
    {
        return score;
    }
    
    
}
