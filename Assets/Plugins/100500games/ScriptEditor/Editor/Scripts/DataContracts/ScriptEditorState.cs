using System;

namespace ScriptEditor
{
    /// <summary>
    /// Serializable ScriptEditorState
    /// </summary>
    [Serializable]
    public class ScriptEditorState
    {
        /// <summary>
        /// Index from settings
        /// </summary>
        public int SettingsIndex;
        
        /// <summary>
        /// Supported Editor Type
        /// </summary>
        public ScriptEditorSupportType SupportType;

        /// <summary>
        /// Top indent
        /// </summary>
        public int TopIndent;

        /// <summary>
        /// Is inited
        /// </summary>
        public bool IsInited;
        
        /// <summary>
        /// Prev editor position
        /// </summary>
        public ScriptEditorRect PrevPos;
        
        /// <summary>
        /// Prev code window position
        /// </summary>
        public ScriptEditorRect PrevCodeEditorPos;
        
        /// <summary>
        /// Prev maximized flag
        /// </summary>
        public bool PrevMaximized;
        
        /// <summary>
        /// Has the project been opened
        /// </summary>
        public bool IsOpenProjectExecuted;

        /// <summary>
        /// Unity window hwnd
        /// </summary>
        public ScriptEditorIntPtr UnityHwnd = new ScriptEditorIntPtr();
        
        /// <summary>
        /// Code window hwnd
        /// </summary>
        public ScriptEditorIntPtr Hwnd = new ScriptEditorIntPtr();

        /// <summary>
        /// Attached windows
        /// </summary>
        public ScriptEditorIntPtr[] AttachedWindows;

        /// <summary>
        /// Last update ticks
        /// </summary>
        public long LastUpdateTicks;
        
        /// <summary>
        /// Is the editor window focused?
        /// </summary>
        public bool IsEditorWindowFocused;
    }
}