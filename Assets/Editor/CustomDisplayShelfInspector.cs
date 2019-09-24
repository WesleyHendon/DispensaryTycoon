using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StoreObjectFunction_DisplayShelf))]
public class CustomDisplayShelfInspector : Editor
{
    public override void OnInspectorGUI()
    {
        StoreObjectFunction_DisplayShelf script = (StoreObjectFunction_DisplayShelf)target;

        // Custom shelf lists
        script.editorType = (StoreObjectFunction_DisplayShelf.EditorType)EditorGUILayout.EnumPopup("Custom shelf list", script.editorType);

        if (script.editorType == StoreObjectFunction_DisplayShelf.EditorType.CustomShelfLists)
        {
            if (script.totalShelfListsCount > 0 && script.shelfLists.Count < script.totalShelfListsCount)
            {
                for (int i = script.shelfLists.Count; i < script.totalShelfListsCount; i++)
                {
                    script.shelfLists.Add(new ShelfList());
                }
            }
        }
        else
        {
            script.ClearShelfLists();
        }
        if (script.editorType == StoreObjectFunction_DisplayShelf.EditorType.CustomShelfLists)
        {
            script.totalShelfListsCount = EditorGUILayout.DelayedIntField("Number of Shelf Lists", script.totalShelfListsCount);
            if (script.totalShelfListsCount > 0)
            {
                for (int i = 0; i < script.totalShelfListsCount; i++)
                {
                    EditorGUILayout.Separator();
                    int value = i;
                    try
                    {
                        script.shelfLists[value].SetShelfListName(EditorGUILayout.TextField("Shelf List Name", script.shelfLists[value].GetShelfListName()));
                        script.shelfLists[value].SetShelfListCount(EditorGUILayout.DelayedIntField("  " + script.shelfLists[value].GetShelfListName() + " # of Shelves", script.shelfLists[i].GetShelfListCount()));
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        script.shelfLists.Add(new ShelfList());
                        script.shelfLists[value].SetShelfListName(EditorGUILayout.TextField("Shelf List Name", script.shelfLists[value].GetShelfListName()));
                        script.shelfLists[value].SetShelfListCount(EditorGUILayout.DelayedIntField("  " + script.shelfLists[value].GetShelfListName() + " # of Shelves", script.shelfLists[i].GetShelfListCount()));
                    }
                    script.OnSetListCount();
                    if (script.shelfLists[i].GetShelfListCount() > 0)
                    {
                        for (int j = 0; j < script.shelfLists[i].GetShelfListCount(); j++)
                        {
                            script.shelfLists[i].shelfLayoutPositions[j] = (ShelfLayoutPosition)EditorGUILayout.ObjectField("  Shelf Position " + j, script.shelfLists[i].shelfLayoutPositions[j], typeof(ShelfLayoutPosition), true);
                        }
                    }
                }
            }
        }
        if (script.totalShelfListsCount == 0)
        {
            if (script.shelfLists.Count > 0)
            {
                script.ClearShelfLists();
            }
        }

        script.priority = EditorGUILayout.Toggle("Priority Object?", script.priority);

        Undo.RecordObject(target, "Modified Shelf Function");
        EditorUtility.SetDirty(target);
    }
}
