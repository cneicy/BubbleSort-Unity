using UnityEditor;
using UnityEngine;

namespace ScriptEditor
{
    /// <summary>
    /// Screenshot creation and storage
    /// </summary>
    public static class ScriptEditorCodeWindow
    {
        /// <summary>
        /// Take a screenshot
        /// </summary>
        /// <param name="currentTex">Current texture</param>
        /// <param name="state">State</param>
        public static void CaptureCodeTexture(ref Texture2D currentTex, ScriptEditorState state)
        {
            if (ScriptEditorInternal.IsCodeWindowMinimized(state))
            {
                return;
            }

            var bytes = ScriptEditorPrintWindow.PrintWindow(state.Hwnd.Value);

            if (bytes == null
                || bytes.Length == 0)
            {
                return;
            }

            if (currentTex == null)
            {
                currentTex = new Texture2D(2, 2, TextureFormat.RGBA32, false, PlayerSettings.colorSpace != ColorSpace.Linear);
            }

            currentTex.LoadImage(bytes);
        }
    }
}
