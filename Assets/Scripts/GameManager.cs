using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject tapToPlayUI;
    public GameObject player;

    private bool gameStarted = false;
    private PogoJump pogo;

    private void Start()
    {
        Time.timeScale = 0f;
        pogo = player.GetComponent<PogoJump>();
        pogo.enabled = false;
    }

    private void Update()
    {
        if (!gameStarted && Input.touchCount > 0 || Input.GetMouseButtonDown(0))
        {
            StartGame();
        }
    }
    
    private void StartGame()
    {
        gameStarted = true;
        tapToPlayUI.SetActive(false);
        pogo.enabled = true;
        
        Time.timeScale = 1f;
    }
}
