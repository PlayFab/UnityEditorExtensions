using System;
using UnityEditor;
using UnityEngine;

namespace PlayFab.PfEditor
{
    /// <summary>
    /// A disposable wrapper for Verticals, to ensure they're paired properly, and to make the code visually block together within them
    /// </summary>
    public class UnityHorizontal : IDisposable
    {
        public UnityHorizontal(params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal(options);
        }

        public UnityHorizontal(GUIStyle style, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal(style, options);
        }

        public void Dispose()
        {
            EditorGUILayout.EndHorizontal();
        }
    }

    /// <summary>
    /// A disposable wrapper for Horizontals, to ensure they're paired properly, and to make the code visually block together within them
    /// </summary>
    public class UnityVertical : IDisposable
    {
        public UnityVertical(params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginVertical(options);
        }

        public UnityVertical(GUIStyle style, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginVertical(style, options);
        }

        public void Dispose()
        {
            EditorGUILayout.EndVertical();
        }
    }

    //FixedWidthLabel class. Extends IDisposable, so that it can be used with the "using" keyword.
    public class FixedWidthLabel : IDisposable
    {
        private readonly ZeroIndent indentReset; //helper class to reset and restore indentation
        public float fieldWidth = 0;

        public FixedWidthLabel(GUIContent label, GUIStyle style) //	constructor.
        {
            //state changes are applied here.

            this.fieldWidth = style.CalcSize(label).x + 9 * EditorGUI.indentLevel;
            EditorGUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleClear")); // create a new horizontal group
            EditorGUILayout.LabelField(label, style, GUILayout.Width(fieldWidth));
            // indentation from the left side. It's 9 pixels per indent level

            indentReset = new ZeroIndent(); //helper class to have no indentation after the label
        }

        public FixedWidthLabel(string label)
            : this(new GUIContent(label), PlayFabEditorHelper.uiStyle.GetStyle("labelStyle")) //alternative constructor, if we don't want to deal with GUIContents
        {
        }

        public void Dispose() //restore GUI state
        {
            indentReset.Dispose(); //restore indentation
            EditorGUILayout.EndHorizontal(); //finish horizontal group
        }
    }

    class ZeroIndent : IDisposable //helper class to clear indentation
    {
        private readonly int originalIndent; //the original indentation value before we change the GUI state

        public ZeroIndent()
        {
            originalIndent = EditorGUI.indentLevel; //save original indentation
            EditorGUI.indentLevel = 0; //clear indentation
        }

        public void Dispose()
        {
            EditorGUI.indentLevel = originalIndent; //restore original indentation
        }
    }
}
