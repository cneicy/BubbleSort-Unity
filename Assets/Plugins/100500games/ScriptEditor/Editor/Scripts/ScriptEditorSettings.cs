using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ScriptEditor
{
    public class ScriptEditorSettings : ScriptableObject
    {
        const string AssetFolder = "Assets/Plugins/100500games/ScriptEditor/Editor/Settings/";
        const string AssetPath   = AssetFolder + "ScriptEditorSettings.asset";
        
        public static ScriptEditorSettings LoadOrCreate()
        {
            var settings =
                (ScriptEditorSettings) AssetDatabase.LoadAssetAtPath(AssetPath, typeof(ScriptEditorSettings));
            
            // ReSharper disable once Unity.NoNullPropagation
            var projectItem = settings?.GetProject();

            if (projectItem == null)
            {
                string projectFolder = System.IO.Path.Combine(Application.dataPath, "../");

                string fullAssetFolder = System.IO.Path.Combine(projectFolder, AssetFolder);

                System.IO.Directory.CreateDirectory(fullAssetFolder);

                string projectName = Application.productName;

                var files = System.IO.Directory.GetFiles(projectFolder, "*.sln")
                                  .Select(q => new System.IO.FileInfo(q))
                                  .OrderByDescending(q => q.LastWriteTime)
                                  .ToArray();

                if (files.Length > 0)
                {
                    projectName = System.IO.Path.GetFileNameWithoutExtension(files[0].FullName);
                }

                settings = ScriptableObject.CreateInstance<ScriptEditorSettings>();

                settings.Items = new ScriptEditorSettingsItem[]
                {
                    new ScriptEditorSettingsItem
                    {
                        Type        = ScriptEditorSettingsType.Project,
                        ProjectName = projectName
                    }
                };

                AssetDatabase.CreateAsset(settings, AssetPath);
            }

            return settings;
        }

        public ScriptEditorSettingsItem[] Items;

        public ScriptEditorSettingsItem GetItem(int index)
        {
            if (Items != null)
            {
                for (int i = 0; i < Items.Length; i++)
                {
                    var item = Items[i];

                    if (i == index)
                    {
                        return item;
                    }
                }
            }

            return null;
        }
        
        public ScriptEditorSettingsItem GetProject()
        {
            if (Items != null)
            {
                for (int i = 0; i < Items.Length; i++)
                {
                    var item = Items[i];

                    if (item != null
                        && item.Type == ScriptEditorSettingsType.Project)
                    {
                        return item;
                    }
                }
            }

            return null;
        }
    }
}