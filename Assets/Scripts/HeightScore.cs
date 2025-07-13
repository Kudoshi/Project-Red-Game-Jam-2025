using UnityEngine;
using TMPro;

public class HeightScore : MonoBehaviour
{
    public GameObject player;
    public TextMeshProUGUI HeightMeter;
    public TextMeshProUGUI HeightResult;
    public TextMeshProUGUI Score;
    public TextMeshProUGUI JumpTotal;
    public TextMeshProUGUI TicketTotal;
    
    [SerializeField] private float highestHeight = 0f;
    [SerializeField] private float currentHeight = 0f;
    [SerializeField] private float multiplier = 10f;
    

    private float score;
    public bool isGameEnd;

    private void Update()
    {
        if (isGameEnd)
        {
            HeightResult.text = Mathf.FloorToInt(highestHeight).ToString("0000") + "Meter";
            TicketTotal.text = UICoin.Instance.CoinAmt.ToString();
            JumpTotal.text = "11";
            ScoreConvert();
        }

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
    }

    private void ScoreConvert()
    {
        score = Mathf.Floor(highestHeight * multiplier);
        Score.text = Mathf.FloorToInt(score).ToString();
        Debug.Log("Final Score: " + score);
    }

    public float GetScore()
    {
        return score;
    }
    
    
}
