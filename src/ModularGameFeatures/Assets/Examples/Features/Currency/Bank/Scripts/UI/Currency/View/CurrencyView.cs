using UnityEngine;
using UnityEngine.UI;

namespace Examples.Features.Currency.Bank.Scripts.UI.Currency.View
{
    public class CurrencyView : MonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshProUGUI _amountCurrency;
        [SerializeField] private Image _icon;

        public void SetIcon(Sprite sprite)
        {
            _icon.sprite = sprite;
        }

        public void Add(string amount)
        {
            _amountCurrency.text = amount;
        }
        
        public void Spent(string amount)
        {
            _amountCurrency.text = amount;
        }
        
        public void Set(string amount)
        {
            _amountCurrency.text = amount;
        }
    }
}