using UnityEngine;
using TMPro;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public GameObject tapToPlayUI;
    public GameObject MainMenu;
    public GameObject InGameUI;
    public GameObject player;
    
    public float scaleUpSize = 1.2f;
    public float duration = 0.5f;

    private bool gameStarted = false;
    private PogoJump pogo;

    private void Start()
    {
        Time.timeScale = 0f;
        pogo = player.GetComponent<PogoJump>();
        pogo.enabled = false;
        
        if (tapToPlayUI != null)
        {
            tapToPlayUI.transform.DOScale(scaleUpSize, duration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine)
                .SetUpdate(true);
        }
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
        MainMenu.SetActive(false);
        InGameUI.SetActive(true);
        pogo.enabled = true;
        
        Time.timeScale = 1f;
    }
}
