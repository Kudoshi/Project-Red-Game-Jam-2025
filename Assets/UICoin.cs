using TMPro;
using UnityEngine;

public class UICoin : Singleton<UICoin>
{
    [SerializeField] private TextMeshProUGUI _coinTxt;

    private int _coinAmt;

    public int CoinAmt { get => _coinAmt; }

    public void AddCoin()
    {
        _coinAmt++;
        _coinTxt.text = "X" + _coinAmt.ToString();
    }

    public void RemoveCoin(int amt)
    {
        _coinAmt -= amt;
        _coinTxt.text = "X" + _coinAmt.ToString();
    }

}
