using DG.Tweening;
using TMPro;
using UnityEngine;

public class ShopSystem : Singleton<ShopSystem>
{
    [SerializeField] private RectTransform _ingameBtnObj;
    [SerializeField] private RectTransform _shopCanvasObj;

    [Header("Shop System")]
    [SerializeField] private TextMeshProUGUI _coinAmtTxt;

    private float _originalAnchorYShopCanvas;

    private void Start()
    {
        //_shopCanvasObj.anchoredPosition = new Vector2(_shopCanvasObj.anchoredPosition.x, -500f);
        _originalAnchorYShopCanvas = _shopCanvasObj.anchoredPosition.y;
        _shopCanvasObj.DOAnchorPosY(-2000, 0.01f);
        _ingameBtnObj.gameObject.SetActive(true);
        _shopCanvasObj.gameObject.SetActive(false);
    }

    public void UI_OpenShop()
    {
        //_ingameBtnObj.gameObject.SetActive(false);
        _shopCanvasObj.gameObject.SetActive(true);


        _shopCanvasObj.gameObject.SetActive(true);
        //_shopCanvasObj.anchoredPosition = new Vector2(_shopCanvasObj.anchoredPosition.x, -500f);
        _shopCanvasObj.DOAnchorPosY(_originalAnchorYShopCanvas, 0.5f).SetEase(Ease.OutCubic);

        _coinAmtTxt.text = UICoin.Instance.CoinAmt.ToString();

        Util.WaitForSeconds(this, () => PogoJump.Instance.enabled = false, 1.0f);
    }

    public void UI_CloseShop()
    {
        //_ingameBtnObj.gameObject.SetActive(true);
        _shopCanvasObj.gameObject.SetActive(false);

        _shopCanvasObj.gameObject.SetActive(true);
        //_shopCanvasObj.anchoredPosition = new Vector2(_shopCanvasObj.anchoredPosition.x, -500f);
        _shopCanvasObj.DOAnchorPosY(-2000, 0.5f).SetEase(Ease.OutCubic);

        Util.WaitForSeconds(this, () => PogoJump.Instance.enabled = true, 1.0f);
        
    }

    public void PurchaseItem(string itemID, int cost)
    {
        if (UICoin.Instance.CoinAmt < cost)
        {
            //Play fail buy item sfx
            return;
        }

        UICoin.Instance.RemoveCoin(cost);
        _coinAmtTxt.text = UICoin.Instance.CoinAmt.ToString();
    }
}
