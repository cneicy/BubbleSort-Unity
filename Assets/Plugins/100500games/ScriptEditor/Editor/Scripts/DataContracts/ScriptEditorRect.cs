using System;

namespace ScriptEditor
{
    /// <summary>
    /// Serializable ScriptEditorRect
    /// </summary>
    [Serializable]
    public class ScriptEditorRect
    {
        /// <summary>
        /// Default ctor
        /// </summary>
        public ScriptEditorRect()
        {

        }

        /// <summary>
        /// Rect ctor
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public ScriptEditorRect(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Serializable Rect from Win Api rect
        /// </summary>
        /// <param name="scriptEditorInternalRect"></param>
        public ScriptEditorRect(ScriptEditorInternalRect scriptEditorInternalRect)
        {
            X = scriptEditorInternalRect.Left;
            Y = scriptEditorInternalRect.Top;
            Width = scriptEditorInternalRect.Right - scriptEditorInternalRect.Left;
            Height = scriptEditorInternalRect.Bottom - scriptEditorInternalRect.Top;
        }

        /// <summary>
        /// X position
        /// </summary>
        public int X;
        /// <summary>
        /// Y position
        /// </summary>
        public int Y;
        /// <summary>
        /// Width
        /// </summary>
        public int Width;
        /// <summary>
        /// Height
        /// </summary>
        public int Height;

        public override string ToString()
        {
            return X.ToString() + " " + Y.ToString() + " " + Width.ToString() + " " + Height.ToString();
        }
    }
}