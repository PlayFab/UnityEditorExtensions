namespace PlayFab.Editor
{
    using UnityEngine;
    using UnityEditor;
    using System.Collections.Generic;

    public class ListDisplay : BaseUiComponent {
        public List<KvpItem> items;

        public string displayTitle = "New List Display";

        // this gets called after the Base draw loop
        public override void PostDraw()
        {
            EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(this.displayTitle);
                if(GUILayout.Button("Refresh"))
                {
                    //BaseUiAnimationController.StartAlphaFade(1, 0, listDisplay);
                }

                if(GUILayout.Button("+"))
                {
                    AddRecord();
                }

            EditorGUILayout.EndHorizontal();


            if(items.Count > 0)
            {
                EditorGUILayout.BeginVertical(PlayFabEditorHelper.uiStyle.GetStyle("listDisplayBox"));
                float inputBoxWidth = EditorGUIUtility.currentViewWidth - 100 > 0 ? (EditorGUIUtility.currentViewWidth - 100) / 2 : 50;
                        
                    for(var z = 0; z < this.items.Count; z++)
                    {
                        if(items[z].Value != null)
                        {
                            EditorGUILayout.BeginHorizontal();
                            items[z].Key = GUILayout.TextArea(items[z].Key, GUILayout.MaxWidth(inputBoxWidth));

                            EditorGUILayout.LabelField(":", GUILayout.MaxWidth(10));
                            items[z].Value = GUILayout.TextField(""+items[z].Value, GUILayout.MaxWidth(inputBoxWidth));  
                                
                            if(GUILayout.Button("X", GUILayout.MaxHeight(19)))
                            {
                                if(string.IsNullOrEmpty(items[z].Key))
                                {
                                    items[z].Value = null;
                                }
                            } 
                          
                            EditorGUILayout.EndHorizontal();
                        }
                    }


                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginHorizontal();
                    if(GUILayout.Button("Save"))
                    {
                        //BaseUiAnimationController.StartAlphaFade(1, 0, listDisplay);
                    }
                EditorGUILayout.EndHorizontal();
            }


            // draw code here.
            base.PostDraw();
        }


        public void AddRecord()
        {
            this.items.Add(new KvpItem("",""));
        }

        public void RefreshRecords()
        {
            
        }

        public void SaveRecords()
        {
            
        }


        public ListDisplay(List<KvpItem> i = null)
        {
            this.items = i ?? new List<KvpItem>();
        }

        public ListDisplay()
        {
            this.items = new List<KvpItem>();
        }
    }


    public class KvpItem
    {
        public string Key;
        public string Value;

        public KvpItem(string k, string v)
        {
            this.Key = k;
            this.Value = v;
        }
    }
}

