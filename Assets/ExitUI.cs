using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitUI : Singleton<ExitUI>
{
    [SerializeField] private GameObject _parent;
    [SerializeField] private TextMeshProUGUI _heightTxt;
    [SerializeField] private TextMeshProUGUI _coinTxt;
    [SerializeField] private TextMeshProUGUI _jumpTxt;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _parent.SetActive(false);
    }

    public void GameEnd()
    {
        _parent.SetActive(true);

    }

    public void UI_Exit()
    {
        SceneManager.LoadScene(0);
    }
}
