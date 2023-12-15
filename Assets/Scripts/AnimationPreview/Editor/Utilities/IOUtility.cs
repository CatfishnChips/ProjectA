using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System;
using System.Linq;

public static class IOUtility
{
    private static string blackboardFileName;
    private static string containerFolderPath;
    private static AnimationPreviewEditorWindow editorWindow;

    // private static List<BlackboardEntry> entries = new List<BlackboardEntry>();
    // private static List<BlackboardEntry> createdEntries = new List<BlackboardEntry>();
    // private static List<BlackboardEntry> loadedEntries = new List<BlackboardEntry>();
        
    public static void Initialize(string blackboardName, AnimationPreviewEditorWindow blackboardEditorWindow)
    {
        blackboardFileName = blackboardName;
        containerFolderPath = $"Assets/Blackboards/{blackboardName}";
        editorWindow = blackboardEditorWindow;

        // entries = new List<BlackboardEntry>();
        // createdEntries = new List<BlackboardEntry>();
        // loadedEntries = new List<BlackboardEntry>();
    }

    public static void Save()
    {
        // CreateDefaultFolders();

        // GetEntriesFromBlackboard();

        //BlackboardSO blackboardContainer = CreateAsset<BlackboardSO>(containerFolderPath, blackboardFileName);

        // blackboardContainer.Initialize(blackboardFileName);

        // SaveEntries(blackboardContainer);

        // SaveAsset(blackboardContainer);
    }

    private static void GetEntriesFromBlackboard()
    {
        // editorWindow.GetEntries().ForEach(entry =>
        // {
        //     entries.Add(entry);
        // });
    }

    // private static void SaveEntries(BlackboardSO blackboardContainer)
    // {
    //     // List<string> entryNames = new List<string>();

    //     // foreach (BlackboardEntry entry in entries)
    //     // {
    //     //     SaveEntryToBlackboard(entry, blackboardContainer);
    //     //     entryNames.Add(entry.Key);
    //     // }

    //     // UpdateOldEntries(entryNames, blackboardContainer);
    // }

    // private static void SaveEntryToBlackboard(BlackboardEntry entry, BlackboardSO blackboardContainer)
    // {
    //     // BlackboardVariable variable = entry.Data.GetCopy();
    //     // variable.Dictionary = blackboardContainer.FileName;
    //     // variable.Key = entry.Key;
        
    //     // //variable.hideFlags = HideFlags.HideInHierarchy;

    //     // AssetDatabase.CreateAsset(variable, $"{containerFolderPath}/Variables/{entry.Key}.asset");

    //     // BlackboardEntry createdEntry = new BlackboardEntry(){
    //     //     Key = entry.Key,
    //     //     Data = variable
    //     // };

    //     // blackboardContainer.Entries.Add(createdEntry);

    //     // createdEntries.Add(createdEntry);
    //     // SaveAsset(variable);
    // }

    // private static void UpdateOldEntries(List<string> entryNames, BlackboardSO blackboardContainer)
    // {
    //     // if (blackboardContainer.OldEntryNames != null && blackboardContainer.OldEntryNames.Count != 0)
    //     // {
    //     //     List<string> nodesToRemove = blackboardContainer.OldEntryNames.Except(entryNames).ToList();

    //     //     foreach (string nodeToRemove in nodesToRemove)
    //     //     {
    //     //         RemoveAsset($"{containerFolderPath}/Variables", nodeToRemove);
    //     //     }
    //     // }

    //     // blackboardContainer.OldEntryNames = new List<string>(entryNames);
    // }

    public static void Load()
    {
        UnityEngine.GameObject blackboardData = LoadAsset<UnityEngine.GameObject>($"Assets", blackboardFileName);

        if (blackboardData == null)
        {
            EditorUtility.DisplayDialog(
                "Could not find the file!",
                "The file at the following path could not be found:\n\n" +
                $"\"Assets/Blackboards/{blackboardFileName}\".\n\n" +
                "Make sure you chose the right file and it's placed at the folder path mentioned above.",
                "Thanks!"
            );

            return;
        }

        editorWindow._asset = blackboardData as GameObject;

        // BlackboardEditorWindow.UpdateFileName(blackboardData.FileName);

        // LoadEntries(blackboardData.Entries);
    }

    // private static void LoadEntries(List<BlackboardEntry> blackboardEntries)
    // {
    //     // foreach (BlackboardEntry entryData in blackboardEntries)
    //     // {
    //     //     loadedEntries.Add(entryData);
            
    //     //     BlackboardEntry entry = new BlackboardEntry(){
    //     //         Key = entryData.Key,
    //     //         Data = entryData.Data.GetCopy()
    //     //     };

    //     //     editorWindow.CreateEntry(entry);
    //     //     entries.Add(entry);
    //     // }
    // }

    private static void CreateDefaultFolders()
    {
        CreateFolder("Assets/Blackboards", blackboardFileName);
        CreateFolder(containerFolderPath, "Variables");
    }

    public static void CreateFolder(string parentFolderPath, string newFolderName)
    {
        if (AssetDatabase.IsValidFolder($"{parentFolderPath}/{newFolderName}"))
        {
            return;
        }

        AssetDatabase.CreateFolder(parentFolderPath, newFolderName);
    }

    public static void RemoveFolder(string path)
    {
        FileUtil.DeleteFileOrDirectory($"{path}.meta");
        FileUtil.DeleteFileOrDirectory($"{path}/");
    }

    public static T CreateAsset<T>(string path, string assetName) where T : ScriptableObject
    {
        string fullPath = $"{path}/{assetName}.asset";

        T asset = LoadAsset<T>(path, assetName);

        if (asset == null)
        {
            asset = ScriptableObject.CreateInstance<T>();

            AssetDatabase.CreateAsset(asset, fullPath);
        }

        return asset;
    }

    public static T LoadAsset<T>(string path, string assetName) where T : UnityEngine.Object
    {
        string fullPath = $"{path}/{assetName}.prefab";

        return AssetDatabase.LoadAssetAtPath<T>(fullPath);
    }

    public static void SaveAsset(UnityEngine.Object asset)
    {
        EditorUtility.SetDirty(asset);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public static void RemoveAsset(string path, string assetName)
    {
        AssetDatabase.DeleteAsset($"{path}/{assetName}.asset");
    }
}