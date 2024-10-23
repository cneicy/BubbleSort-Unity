using System.Runtime.InteropServices;

namespace ScriptEditor
{
    /// <summary>
    /// Point Win Api class
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct ScriptEditorInternalPoint
    {
        public ScriptEditorInternalPoint(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// X position
        /// </summary>
        public int X;
        /// <summary>
        /// Y position
        /// </summary>
        public int Y;

        public override string ToString()
        {
            return X + " " + Y;
        }
    }
}