namespace ScriptEditor
{
    [System.Serializable]
    public class ScriptEditorSettingsItem
    {
        public ScriptEditorSettingsType Type;
        public string                   Path;
        public string                   ProjectName;
        public ScriptEditorWindow       Window;
    }
}