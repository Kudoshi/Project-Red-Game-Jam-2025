using TMPro;
using UnityEngine;

public class UIJumpCanvas : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _jumpCountTxt;


    // Update is called once per frame
    void Update()
    {
        _jumpCountTxt.text = PogoJump.Instance.JumpAmount.ToString();
    }
}
