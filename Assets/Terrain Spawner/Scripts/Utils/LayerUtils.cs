using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;



/*

https://stackoverflow.com/questions/61085096/how-can-i-create-a-new-layer-using-a-script-in-unity
https://forum.unity.com/threads/create-tags-and-layers-in-the-editor-using-script-both-edit-and-runtime-modes.732119/

*/

public class LayerUtils 
{
    private static int maxLayers = 31;
    public static bool CheckLayerExists(string layerName)
    {
        // Open tag manager
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);

        // Layers Property
        SerializedProperty layersProp = tagManager.FindProperty("layers");
        return CheckPropertyExists(layersProp, 0, maxLayers, layerName);
    }

    private static bool CheckPropertyExists(SerializedProperty property, int start, int end, string value)
    {
        for (int i = start; i < end; i++)
        {
            SerializedProperty t = property.GetArrayElementAtIndex(i);
            if (t.stringValue.Equals(value))
            {
                return true;
            }
        }
        return false;
    }

    public static bool CreateLayer(string layerName)
    {
        // Open tag manager
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        // Layers Property
        SerializedProperty layersProp = tagManager.FindProperty("layers");
        if (!CheckPropertyExists(layersProp, 0, maxLayers, layerName))
        {
            SerializedProperty sp;
            // Start at layer 9th index -> 8 (zero based) => first 8 reserved for unity / greyed out
            for (int i = 8, j = maxLayers; i < j; i++)
            {
                sp = layersProp.GetArrayElementAtIndex(i);
                if (sp.stringValue == "")
                {
                    // Assign string value to layer
                    sp.stringValue = layerName;
                    Debug.Log("Layer: " + layerName + " has been added");
                    // Save settings
                    tagManager.ApplyModifiedProperties();
                    return true;
                }
                if (i == j)
                    Debug.Log("All allowed layers have been filled");
            }
        }
        else
        {
            //Debug.Log ("Layer: " + layerName + " already exists");
        }
        return false;
    }

    public static bool CreateLayer(string layerName, int idx)
    {
        // Open tag manager
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        // Layers Property
        SerializedProperty layersProp = tagManager.FindProperty("layers");
        if (!CheckPropertyExists(layersProp, 0, maxLayers, layerName))
        {
            SerializedProperty sp;
            sp = layersProp.GetArrayElementAtIndex(idx);
            if (sp.stringValue == "")
            {
                // Assign string value to layer
                sp.stringValue = layerName;
                Debug.Log("Layer: " + layerName + " has been added");
                // Save settings
                tagManager.ApplyModifiedProperties();
                return true;
            }
        }
        else
        {
            //Debug.Log ("Layer: " + layerName + " already exists");
        }
        return false;
    }
}
