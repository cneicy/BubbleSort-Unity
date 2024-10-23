using System;

namespace ScriptEditor
{
    /// <summary>
    /// Window info
    /// </summary>
    public class ScriptEditorWindowInfo
    {
        /// <summary>
        /// Hwnd
        /// </summary>
        public IntPtr Hwnd;
        /// <summary>
        /// Text
        /// </summary>
        public string Text;

        public override string ToString()
        {
            return $"{Text} {Hwnd}";
        }
    }
}