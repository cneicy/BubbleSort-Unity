using System.Runtime.InteropServices;

namespace ScriptEditor
{
    /// <summary>
    /// Win Api Rect class
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct ScriptEditorInternalRect
    {
        /// <summary>
        /// Left
        /// </summary>
        public int Left;        // x position of upper-left corner
        /// <summary>
        /// Top
        /// </summary>
        public int Top;         // y position of upper-left corner
        /// <summary>
        /// Right
        /// </summary>
        public int Right;       // x position of lower-right corner
        /// <summary>
        /// Bottom
        /// </summary>
        public int Bottom;      // y position of lower-right corner

        public override string ToString()
        {
            return Left + " " + Top + " " + Right + " " + Bottom;
        }
    }
}