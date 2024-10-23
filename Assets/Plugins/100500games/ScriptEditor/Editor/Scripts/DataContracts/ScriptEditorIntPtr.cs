using System;

namespace ScriptEditor
{
    /// <summary>
    /// Serializable ScriptEditorIntPtr class
    /// </summary>
    [System.Serializable]
    public class ScriptEditorIntPtr
    {
        /// <summary>
        /// Log IntPtr value
        /// </summary>
        public long LongValue;

        /// <summary>
        /// Default ctor
        /// </summary>
        public ScriptEditorIntPtr()
        {

        }

        /// <summary>
        /// IntPtr ctor
        /// </summary>
        /// <param name="value"></param>
        public ScriptEditorIntPtr(IntPtr value)
        {
            Value = value;
        }

        /// <summary>
        /// Not created IntPtr value
        /// </summary>
        [NonSerialized]
        IntPtr? nullableValue;

        /// <summary>
        /// Created IntPtr Value
        /// </summary>
        public IntPtr Value
        {
            get
            {
                if (nullableValue == null)
                {
                    nullableValue = new IntPtr(LongValue);
                }

                return nullableValue.Value;
            }
            set
            {
                nullableValue = value;
                LongValue = value.ToInt64();
            }
        }
    }
}