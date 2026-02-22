using UnityEngine;

namespace Examples.Features.Currency.Bank.Scripts.UI.Currency.Presenters.CreatorCurrencyOperationPanel
{
    public class CurrencyOperationPanelCreator : MonoBehaviour
    {
        [SerializeField] private View.CurrencyOperationPanel _currencyOperationPanelPrefab;
        [SerializeField] private Transform _content;

        public View.CurrencyOperationPanel Spawn() => 
            Instantiate(_currencyOperationPanelPrefab, _content);

        public void Unspawn(View.CurrencyOperationPanel currencyView)
        {
            if (currencyView)
            {
                Destroy(currencyView);
            }
            else
            {
                Debug.LogError($"Was called with a null reference");
            }
        }
    }
}