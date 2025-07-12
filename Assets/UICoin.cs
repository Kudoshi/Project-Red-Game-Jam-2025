using TMPro;
using UnityEngine;

public class UICoin : Singleton<UICoin>
{
    [SerializeField] private TextMeshProUGUI _coinTxt;

    private int _coinAmt;

    public void AddCoin()
    {
        _coinAmt++;
        _coinTxt.text = "X" + _coinAmt.ToString();
    }
}
