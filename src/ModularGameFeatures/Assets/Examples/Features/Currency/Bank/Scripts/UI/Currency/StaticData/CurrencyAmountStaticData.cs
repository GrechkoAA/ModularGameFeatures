using System;
using System.Collections.Generic;
using Modules.Features.Currency.Bank.Scripts;
using UnityEngine;

namespace Examples.Features.Currency.Bank.Scripts.UI.Currency.StaticData
{
    [Serializable]
    public struct CurrencyAmountSettings
    {
        [field: SerializeField] public CurrencyType CurrencyType { get; set; }
        [field: SerializeField] public int Amount { get; set; }

        public (CurrencyType, CurrencyCell) GetCell() => 
            (CurrencyType, new CurrencyCell(Amount));
    }
    
    [CreateAssetMenu(fileName = nameof(CurrencyAmountStaticData), menuName = "StaticData/UI/Currency" + nameof(CurrencyAmountStaticData), order = 0)]
    public class CurrencyAmountStaticData : ScriptableObject
    {
        [SerializeField] private CurrencyAmountSettings[] _settings;

        public IEnumerable<(CurrencyType, CurrencyCell)> GetCells()
        {
            foreach (var setting in _settings)
                yield return setting.GetCell();
        }
    }
}