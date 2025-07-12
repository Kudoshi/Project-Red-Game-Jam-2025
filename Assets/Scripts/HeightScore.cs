using System;
using UnityEditor.U2D.Sprites;
using UnityEngine;

public class HeightScore : MonoBehaviour
{
    public GameObject player;

    [SerializeField] private float highestHeight = 0f;
    [SerializeField] private float multiplier = 10f;

    private float score;
    private bool isGameEnd;

    private void Update()
    {
        if (isGameEnd) return;

        float currentHeight = player.transform.position.y;
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
