using System;
using System.Collections.Generic;
using System.Linq;
using Modules.Features.Currency.Bank.Scripts;
using UnityEngine;

namespace Examples.Features.Currency.Bank.Scripts.UI.Currency.StaticData
{
    [Serializable]
    public struct CurrencyIconSettings
    {
        [field: SerializeField] public CurrencyType CurrencyType { get; set; }
        [field: SerializeField] public Sprite Icon { get; set; }
    }
    
    [CreateAssetMenu(fileName = nameof(CurrencyIconStaticData), menuName = "StaticData/UI/Currency" + nameof(CurrencyIconStaticData), order = 0)]
    public class CurrencyIconStaticData : ScriptableObject
    {
        [SerializeField] private CurrencyIconSettings[] _settings;
        
        private Dictionary<CurrencyType, CurrencyIconSettings> _settingsMap;

        public CurrencyIconSettings GetCurrencySettings(CurrencyType currencyType)
        {
            _settingsMap ??= _settings.ToDictionary(x => x.CurrencyType, x => x);

            return _settingsMap.TryGetValue(currencyType, out var settings)
                ? settings
                : throw new ArgumentException($"Unknown currency type {currencyType}");
        }
    }
}