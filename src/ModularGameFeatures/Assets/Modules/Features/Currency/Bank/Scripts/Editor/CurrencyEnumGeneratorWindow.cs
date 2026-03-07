using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class CurrencyEnumGeneratorWindow : EditorWindow
{
    private const string PATH = "Assets/Modules/Features/Currency/Bank/Scripts/CurrencyType.cs";

    private List<string> currencies = new();
    private ReorderableList list;

    [MenuItem("Tools/CodeGenerator/💰 Currency")]
    public static void Open()
    {
        GetWindow<CurrencyEnumGeneratorWindow>("Currency Generator");
    }

    private void OnEnable()
    {
        LoadExistingCurrencies();
        SetupList();
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
            EditorGUI.LabelField(numberRect, (index + 1).ToString());

            Rect textRect = new Rect(rect.x + 25, rect.y, rect.width - 25, rect.height);
            currencies[index] = EditorGUI.TextField(textRect, currencies[index]);
        };

        list.onAddCallback = l =>
        {
            currencies.Add("NewCurrency");
        };

        list.onRemoveCallback = l =>
        {
            currencies.RemoveAt(l.index);
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

    private void LoadExistingCurrencies()
    {
        currencies.Clear();

        if (!File.Exists(PATH))
            return;

        var lines = File.ReadAllLines(PATH);

        var values = lines
            .Select(l => l.Trim())
            .Where(l => l.Contains("=") && !l.Contains("None"))
            .Select(l => Regex.Match(l, @"^\w+").Value)
            .Where(v => !string.IsNullOrEmpty(v));

        currencies = values.ToList();
    }

    private void GenerateEnum()
    {
        currencies = currencies
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct()
            .ToList();

        Directory.CreateDirectory(Path.GetDirectoryName(PATH));

        string enumValues = "";

        for (int i = 0; i < currencies.Count; i++)
        {
            enumValues += $"        {currencies[i]} = {i + 1},\n";
        }

        string code =
$@"namespace Modules.Features.Currency.Bank.Scripts
{{
    public enum CurrencyType : byte
    {{
        None = 0,
{enumValues}    }}
}}";

        File.WriteAllText(PATH, code);

        AssetDatabase.Refresh();

        Debug.Log("CurrencyType enum generated!");
    }
}