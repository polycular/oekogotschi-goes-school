using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System;

namespace SharedAssets.GuiMain
{
    /// <summary>
    /// This editor script renders an dropbown with all possible sprites for each value in the interaction type enum
    /// </summary>
    [System.Serializable]
    [CustomEditor(typeof(InteractionSpriteListBehaviour), false)]
    public class InteractionSpriteListEditor : Editor
    {
        //renders the custom gui everytime the it's focused
        public override void OnInspectorGUI()
        {
            //the behaviour this editor script refers to - mapping is done by the script's name and the suffix 'Editor'
            InteractionSpriteListBehaviour baseBahaviour = (InteractionSpriteListBehaviour)this.target;

            //draws the custom inspector elements
            DrawDefaultInspector();

            //this array contains all enum values
            Array enumVals = Enum.GetValues(typeof(InteractionSpriteListBehaviour.InteractionType));

            //if the selection array isn't initialized, or the length doesn't match with the length of possible enum values, do the init
            if (baseBahaviour.Selected == null || baseBahaviour.Selected.Length != enumVals.Length)
            {
                baseBahaviour.Selected = new int[enumVals.Length];
            }

            List<UISpriteData> atlasSpriteData;
            if (baseBahaviour.Atlas == null)
            {
                baseBahaviour.Selected = new int[enumVals.Length];
                return;
            }
            else
            {
                atlasSpriteData = baseBahaviour.Atlas.spriteList;
            }

            //list of all UISprite names - needed as values for the dropdowns
            string[] atlasSpriteNames = new string[atlasSpriteData.Count];

            for (int i = 0; i < atlasSpriteData.Count; i++)
            {
                atlasSpriteNames[i] = atlasSpriteData[i].name;
            }

            //resize the mapping array when it's size doesn't match the enum's size and init the values to null
            if (baseBahaviour.Mapping.Length != enumVals.Length)
            {
                Array.Resize(ref baseBahaviour.Mapping, enumVals.Length);

                for (int i = 0; i < baseBahaviour.Mapping.Length; i++)
                {
                    baseBahaviour.Mapping[i] = null;
                }
            }

            EditorGUI.BeginChangeCheck();

            //render the dropdown for each enum value
            foreach (InteractionSpriteListBehaviour.InteractionType val in enumVals)
            {
                baseBahaviour.Selected[(int)val] = EditorGUILayout.Popup(val.ToString(), baseBahaviour.Selected[(int)val], atlasSpriteNames);

                //save the selected dropdown value for each enum value in the Mapping array, which is stored in the target
                baseBahaviour.Mapping[(int)val] = atlasSpriteData[baseBahaviour.Selected[(int)val]];
            }

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
            }
        }
    }
}
