using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Modules.Features.Currency.Bank.Scripts;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class CurrencyEnumGeneratorWindow : EditorWindow
{
    private List<string> currencies;
    private ReorderableList list;

    [MenuItem("Tools/CodeGenerator/💰 Currency")]
    public static void Open() => GetWindow<CurrencyEnumGeneratorWindow>("Currency Generator");

    private void OnEnable()
    {
        LoadCurrenciesFromEnum();
        SetupList();
    }

    private void LoadCurrenciesFromEnum()
    {
        currencies = Enum.GetNames(typeof(CurrencyType)).ToList();

        if (!currencies.Contains("None"))
            currencies.Insert(0, "None");
        else if (currencies.IndexOf("None") != 0)
        {
            currencies.Remove("None");
            currencies.Insert(0, "None");
        }
    }

    private void SetupList()
    {
        list = new ReorderableList(currencies, typeof(string), false, true, true, true);

        list.drawHeaderCallback = rect =>
        {
            EditorGUI.LabelField(rect, "Currencies");
        };

        list.drawElementCallback = (rect, index, active, focused) =>
        {
            Rect numberRect = new Rect(rect.x, rect.y, 25, rect.height);
            EditorGUI.LabelField(numberRect, index.ToString()); // ID с None = 0

            Rect textRect = new Rect(rect.x + 25, rect.y, rect.width - 25, rect.height);

            if (index == 0)
                EditorGUI.LabelField(textRect, currencies[index]); // None нельзя редактировать
            else
                currencies[index] = EditorGUI.TextField(textRect, currencies[index]);
        };

        list.onRemoveCallback = l =>
        {
            if (l.index == 0) return; // нельзя удалить None
            currencies.RemoveAt(l.index);
        };

        list.onAddCallback = l =>
        {
            currencies.Add("NewCurrency");
        };
    }

    private void OnGUI()
    {
        GUILayout.Space(10);
        list.DoLayoutList();
        GUILayout.Space(10);

        if (GUILayout.Button("Generate Enum"))
        {
            GenerateEnum();
        }
    }

    private void GenerateEnum()
    {
        var uniqueCurrencies = currencies
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct()
            .ToList();

        string currentDir = Path.GetDirectoryName(AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this)));
        string parentDir = Path.Combine(currentDir, "..");
        string targetPath = Path.Combine(parentDir, "CurrencyType.cs");
        targetPath = Path.GetFullPath(targetPath).Replace("\\", "/");

        Directory.CreateDirectory(parentDir);

        string enumValues = "";
        for (int i = 1; i < uniqueCurrencies.Count; i++)
        {
            enumValues += $"        {uniqueCurrencies[i]} = {i},\n";
        }

        string code =
$@"namespace Modules.Features.Currency.Bank.Scripts
{{
    public enum CurrencyType : byte
    {{
        None = 0,
{enumValues}    }}
}}";

        File.WriteAllText(targetPath, code);
        AssetDatabase.Refresh();

        Debug.Log("<color=green>CurrencyType enum generated!</color>");
    }
}