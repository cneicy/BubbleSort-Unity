using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ScriptEditor
{
    /// <summary>
    /// Script editor window
    /// </summary>
    public class ScriptEditorWindow : EditorWindow, IHasCustomMenu
    {
        /// <summary>
        /// Menu item for opening a project in editor
        /// </summary>
        [MenuItem("Assets/Open C# Project in Editor")]
        public static void OpenProjectInEditor()
        {
            var settings = ScriptEditorSettings.LoadOrCreate();
            var projects = settings.Items;

            for (int settingsIndex = 0; settingsIndex < projects.Length; settingsIndex++)
            {
                var project = projects[settingsIndex];

                if (project.Window == null)
                {
                    var windows = Resources.FindObjectsOfTypeAll<ScriptEditorWindow>();

                    foreach (var current in windows)
                    {
                        if (current.state != null
                            && current.state.SettingsIndex == settingsIndex)
                        {
                            project.Window = current;
                            project.Window.SetSettings(settings, settingsIndex);
                            current.Show();
                            break;
                        }
                    }

                    if (project.Window == null)
                    {
#if UNITY_2019_1_OR_NEWER
                        project.Window = CreateWindow<ScriptEditorWindow>();
#else
                        project.Window = GetWindow<ScriptEditorWindow>();
#endif
                        project.Window.SetSettings(settings, settingsIndex);

                        project.Window.Show();
                    }
                }

                if (project.Type == ScriptEditorSettingsType.Project
                    && string.IsNullOrEmpty(project.Path))
                {
                    project.Window.Focus();
                }
            }
        }

        /// <summary>
        /// Menu item for opening a project in editor
        /// </summary>
        [MenuItem("Window/Script editor/Settings", false, 0)]
        public static void OpenSettings()
        {
            var settings = ScriptEditorSettings.LoadOrCreate();

            Selection.activeObject = settings;
        }

        /// <summary>
        /// Menu item for opening a project in editor
        /// </summary>
        [MenuItem("Window/Script editor/Open Custom C# Project in Editor", false, 100)]
        public static void OpenCustomProjectInEditor()
        {
            string projectFolder = ScriptEditorFileTool.GetProjectFolder();

            string path = EditorUtility.OpenFilePanel("Select custom project", projectFolder, "sln");

            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            var fileInfo = new FileInfo(path);

            if (fileInfo.Directory == null)
            {
                return;
            }

            string relativePath = ScriptEditorFileTool.GetRelativePath(projectFolder, fileInfo.Directory.FullName);
            string projectName = Path.GetFileNameWithoutExtension(fileInfo.Name);

            var settings = ScriptEditorSettings.LoadOrCreate();

            foreach (var current in settings.Items)
            {
                if (current.Path == relativePath
                    && current.ProjectName == projectName)
                {
                    EditorUtility.DisplayDialog("Error",
                        "Project " + projectName + " already exists in settings",
                        "OK");

                    return;
                }
            }

            List<ScriptEditorSettingsItem> items = new List<ScriptEditorSettingsItem>(settings.Items);

            items.Add(new ScriptEditorSettingsItem
            {
                Path = relativePath,
                Type = ScriptEditorSettingsType.Custom,
                ProjectName = projectName
            });

            settings.Items = items.ToArray();
        }

        /// <summary>
        /// Current state field
        /// </summary>
        [SerializeField]
        ScriptEditorState state;

        /// <summary>
        /// Settings field
        /// </summary>
        [SerializeField]
        ScriptEditorSettings settings;

        /// <summary>
        /// Last screenshot time 
        /// </summary>
        [SerializeField]
        double lastScreenShotTime;

        /// <summary>
        /// Is fast mode field
        /// </summary>
        [SerializeField]
        bool isFastMode;

        /// <summary>
        /// Current texture
        /// </summary>
        Texture2D currentTex;

        /// <summary>
        /// Is tab was focused
        /// </summary>
        bool isFocused;

        /// <summary>
        /// Is Visual Studio 2022
        /// </summary>
        static bool? isVisualStudio2022;

        /// <summary>
        /// Is Visual Studio 2022 property
        /// </summary>
        private static bool IsVisualStudio2022
        {
            get
            {
                if (isVisualStudio2022 == null)
                {
                    var editorPathString = GetScriptsDefaultApp();

                    if (string.IsNullOrEmpty(editorPathString))
                    {
                        isVisualStudio2022 = false;
                    }
                    else
                    {
                        isVisualStudio2022 = editorPathString.Contains("2022");
                    }
                }

                return isVisualStudio2022.Value;
            }
        }

        /// <summary>
        /// Set settings
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="settingsIndex"></param>
        void SetSettings(ScriptEditorSettings settings, int settingsIndex)
        {
            this.settings = settings;
            state.SettingsIndex = settingsIndex;
        }

        /// <summary>
        /// Reset state on awake
        /// </summary>
        private void Awake()
        {
            ResetState();
        }

        /// <summary>
        /// Save current state
        /// </summary>
        private void ApplyState()
        {
            state.TopIndent = 0;

#if UNITY_2018_1_OR_NEWER
            state.TopIndent = 21;
#endif
        }

        /// <summary>
        /// Reset current state
        /// </summary>
        private void ResetState()
        {
            if (state == null)
            {
                state = new ScriptEditorState();
            }

            ScriptEditorInternal.ResetState(state);
        }

        /// <summary>
        /// Add fast mode menu item to custom menu
        /// </summary>
        /// <param name="menu">Custom menu</param>
        void IHasCustomMenu.AddItemsToMenu(GenericMenu menu)
        {
            GUIContent content = new GUIContent("Is fast mode");
            menu.AddItem(content, isFastMode, OnFastModeClick);
        }

        /// <summary>
        /// On fast mode menu item click
        /// </summary>
        void OnFastModeClick()
        {
            isFastMode = !isFastMode;

            SetTitle();
        }

        /// <summary>
        /// On enable
        /// </summary>
        private void OnEnable()
        {
            lastScreenShotTime = 0;

            EditorApplication.update -= OnUnityUpdate;
            EditorApplication.update += OnUnityUpdate;
        }

        /// <summary>
        /// On destroy
        /// </summary>
        private void OnDestroy()
        {
            EditorApplication.update -= OnUnityUpdate;

            ScriptEditorInternal.DeactivateCodeWindow(state);

            ResetState();
        }

        /// <summary>
        /// Execute default open C# project menu item
        /// </summary>
        private void OpenProject()
        {
            var settingsItem = settings.GetItem(state.SettingsIndex);

            if (settingsItem.Type == ScriptEditorSettingsType.Project
                && string.IsNullOrEmpty(settingsItem.Path))
            {
                EditorApplication.ExecuteMenuItem("Assets/Open C# Project");
            }
            else
            {
                var absoluteFileName = "\"" +
                    ScriptEditorFileTool.GetAbsoluteFileName(settingsItem.Path, settingsItem.ProjectName) + "\"";

                System.Diagnostics.Process.Start(GetScriptsDefaultApp(), absoluteFileName);
            }
        }

        /// <summary>
        /// Get scripts Default App
        /// </summary>
        /// <returns></returns>
        private static string GetScriptsDefaultApp()
        {
            return EditorPrefs.GetString("kScriptsDefaultApp");
        }

        /// <summary>
        /// Get current project type
        /// </summary>
        /// <returns></returns>
        ScriptEditorSupportType GetPrefferedSupportType()
        {
            var path = GetScriptsDefaultApp();

            if (string.IsNullOrEmpty(path))
            {
                return ScriptEditorSupportType.None;
            }

            if (path.ToLower().Contains("code.exe"))
            {
                return ScriptEditorSupportType.VsCode;
            }

            if (path.Contains("devenv.exe"))
            {
                return ScriptEditorSupportType.MsVisualStudio;
            }

            if (path.Contains("JetBrains"))
            {
                return ScriptEditorSupportType.JetBrainsRider;
            }

            return ScriptEditorSupportType.None;
        }

        /// <summary>
        /// On unity update
        /// </summary>
        void OnUnityUpdate()
        {
            if (state == null
                || settings == null)
            {
                return;
            }

            if (state.IsInited == false)
            {
                var settingsItem = settings.GetItem(state.SettingsIndex);

                // Settings Item not found, close the window
                if (settingsItem == null)
                {
                    Close();

                    return;
                }

                settingsItem.Window = this;

                SetTitle("Loading...");

                if (ScriptEditorInternal.Init(state, GetPrefferedSupportType(), settingsItem.ProjectName) == false)
                {
                    if (state.IsOpenProjectExecuted == false)
                    {
                        OpenProject();

                        state.IsOpenProjectExecuted = true;
                    }
                    return;
                }

                SetTitle();

                ApplyState();
            }

            bool isEditorWindowFocused =
                (DateTime.Now.Ticks - state.LastUpdateTicks) < (TimeSpan.TicksPerMillisecond * 300);

            if (isEditorWindowFocused == false
                && ScriptEditorInternal.IsCodeWindowForeground(state))
            {
                Focus();

                ScriptEditorCodeWindow.CaptureCodeTexture(ref currentTex, state);
            }

            if (state.IsEditorWindowFocused != isEditorWindowFocused)
            {
                state.IsEditorWindowFocused = isEditorWindowFocused;

                if (isEditorWindowFocused == false)
                {
                    OnEditorWindowLostFocus();
                }
                else
                {
                    if (isFocused)
                    {
                        ScriptEditorInternal.ActivateCodeWindow(state);
                    }
                }

                isFocused = false;
            }

            if (EditorApplication.timeSinceStartup > lastScreenShotTime)
            {
                if (isEditorWindowFocused && isFastMode == false)
                {
                    ScriptEditorCodeWindow.CaptureCodeTexture(ref currentTex, state);
                }

                Repaint();

                lastScreenShotTime = EditorApplication.timeSinceStartup + 0.3f;
            }
        }

        /// <summary>
        /// On editor tab focus
        /// </summary>
        void OnFocus()
        {
            isFocused = true;
        }

        /// <summary>
        /// On frame update
        /// </summary>
        void Update()
        {
            if (state == null)
            {
                return;
            }

            state.LastUpdateTicks = DateTime.Now.Ticks;
        }

        /// <summary>
        /// On gui
        /// </summary>
        private void OnGUI()
        {
            if ((state?.IsInited ?? false) == false)
            {
                return;
            }

            if (Event.current != null
                && Event.current.isMouse)
            {
                Event.current.Use();

                if (ScriptEditorInternal.IsCodeWindowForeground(state) == false)
                {
                    OnEditorWindowGotFocus();
                }
            }

            ScriptEditorInternal.CheckAttachedWindows(state);

            if (OnPositionChanged()
                || currentTex == null)
            {
                ScriptEditorCodeWindow.CaptureCodeTexture(ref currentTex, state);
            }

            if (currentTex != null)
            {
#if UNITY_5
                int xOffset = 0;
                int yOffset = 0;

#elif UNITY_2018
                 int xOffset = -1;
                 int yOffset = 1;
#else
                int xOffset = 0;
                int yOffset = 1;
#endif
                var mod = 1 / CalcDpi();

                float width = currentTex.width * mod;
                float height = currentTex.height * mod;

                GUI.DrawTexture(new Rect(xOffset, yOffset, width, height), currentTex);
            }
        }

        /// <summary>
        /// On editor script window got focus
        /// </summary>
        private void OnEditorWindowGotFocus()
        {
            ScriptEditorInternal.ActivateCodeWindow(state);
        }

        /// <summary>
        /// On editor script window lost focus
        /// </summary>
        private void OnEditorWindowLostFocus()
        {
            ScriptEditorCodeWindow.CaptureCodeTexture(ref currentTex, state);
            ScriptEditorInternal.DeactivateCodeWindow(state);
        }

        /// <summary>
        /// Set editor window title
        /// </summary>
        /// <param name="text">Text to add</param>
        private void SetTitle(string text = "")
        {
            var proj = settings.GetItem(state.SettingsIndex);

            if (proj == null)
            {
                Close();

                return;
            }

            string label;

            if (string.IsNullOrEmpty(text) == false)
            {
                if (string.IsNullOrEmpty(text) == false)
                {
                    label = "Script Editor:" + text;
                }
                else
                {
                    label = "Script Editor";
                }
            }
            else
            {
                label = "SE: " + proj.ProjectName;
            }

            titleContent = new GUIContent(label);

            Repaint();
        }

        /// <summary>
        /// Calculate dpi
        /// </summary>
        /// <returns></returns>
        private float CalcDpi()
        {
            //Screen.dpi / 96f

            return EditorGUIUtility.pixelsPerPoint;
        }

        /// <summary>
        /// Get visual studio dependent offset
        /// </summary>
        private Rect GetVisualStudioOffset(float dpi)
        {
            int x = 0;
            int y = 0;
            int width = 0;
            int height = 0;

            switch (state.SupportType)
            {
                case ScriptEditorSupportType.MsVisualStudio:
                    if (IsVisualStudio2022)
                    {
                        x = -5;
                        y = -1;
                        width = 12;
                        height = 6;
                    }
                    break;
                case ScriptEditorSupportType.JetBrainsRider:
                    x = -7;
                    y = -1;
                    width = 16;
                    height = 6;
                    break;
                case ScriptEditorSupportType.VsCode:
                    x = 1;
                    y = -1;
                    width = 2;
                    height = 0;
                    break;
            }

            y += Mathf.FloorToInt(dpi / 0.25f - 4) * 5;

            if (dpi >= 1.5f)
            {
                x += 1;
                y += 1;
            }

            if (dpi >= 1.75f)
            {
                y -= 1;
            }

            return new Rect(x, y, width, height);
        }

        /// <summary>
        /// Get current position
        /// </summary>
        /// <returns>Rect</returns>
        private ScriptEditorRect GetCurrentPos()
        {
            // If position not valid
            if ((position.x == 0
                && position.y == 0)
                || state == null
                || state.IsInited == false)
            {
                return new ScriptEditorRect(0, 0, 0, 0);
            }

            var dpi = CalcDpi();

            Rect offset = GetVisualStudioOffset(dpi);

            var x = Mathf.FloorToInt(position.x * dpi + offset.x);
            var y = Mathf.FloorToInt(position.y * dpi + offset.y);

            var width = Mathf.FloorToInt((position.width) * dpi + offset.width);
            var height = Mathf.FloorToInt((position.height) * dpi + offset.height);

            return new ScriptEditorRect(x, y, width, height);
        }

        /// <summary>
        /// On position changed
        /// </summary>
        /// <param name="forceActivate">forcefully change position</param>
        /// <returns></returns>
        private bool OnPositionChanged(bool forceActivate = false)
        {
            if ((state?.IsInited ?? false) == false)
            {
                return false;
            }

            var currentPos = GetCurrentPos();

            if (currentPos.X == 0
                && currentPos.Y == 0)
            {
                return false;
            }

            bool isError;

            bool isChanged = ScriptEditorInternal.OnPositionChanged(state, currentPos, false, forceActivate, out isError);

            if (isError)
            {
                ResetState();
            }

            return isChanged;
        }
    }
}