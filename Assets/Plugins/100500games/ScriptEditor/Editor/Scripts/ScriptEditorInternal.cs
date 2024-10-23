using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptEditor
{
    /// <summary>
    /// Internal script editor class
    /// </summary>
    public static class ScriptEditorInternal
    {
        /// <summary>
        /// Hide attached Visual Studio glow windows
        /// </summary>
        /// <param name="state"></param>
        public static void CheckAttachedWindows(ScriptEditorState state)
        {
            if (state.SupportType == ScriptEditorSupportType.MsVisualStudio
                && state.AttachedWindows != null)
            {
                for (int i = 0; i < state.AttachedWindows.Length; i++)
                {
                    ScriptEditorInternalWinApi.ShowWindow(state.AttachedWindows[i].Value, ScriptEditorInternalWinApi.SW_HIDE);
                }
            }
        }

        /// <summary>
        /// Check position changed
        /// </summary>
        /// <param name="state"></param>
        /// <param name="currentPos"></param>
        /// <param name="isUnityEditorMaximized"></param>
        /// <param name="forceActivate"></param>
        /// <param name="isError"></param>
        /// <returns></returns>
        public static bool OnPositionChanged(ScriptEditorState state, ScriptEditorRect currentPos, bool isUnityEditorMaximized, bool forceActivate, out bool isError)
        {
            if (IsMaximized(state))
            {
                ScriptEditorInternalWinApi.ShowWindow(state.Hwnd.Value, ScriptEditorInternalWinApi.SW_RESTORE);
            }

            isError = false;

            bool isUnityHasSamePosition = state.PrevPos != null 
                && state.PrevPos.X == currentPos.X
                && state.PrevPos.Y == currentPos.Y
                && state.PrevPos.Width == currentPos.Width
                && state.PrevPos.Height == currentPos.Height;

            ScriptEditorInternalRect currentCodeScriptEditorInternalRect;

            ScriptEditorInternalWinApi.GetWindowRect(state.Hwnd.Value, out currentCodeScriptEditorInternalRect);

            var newCodeRect = new ScriptEditorRect(currentCodeScriptEditorInternalRect);

            var isCodeEditorHasSamePosition = state.PrevCodeEditorPos != null 
                && state.PrevCodeEditorPos.X == newCodeRect.X
                && state.PrevCodeEditorPos.Y == newCodeRect.Y
                && state.PrevCodeEditorPos.Width == newCodeRect.Width
                && state.PrevCodeEditorPos.Height == newCodeRect.Height;

            if (forceActivate == false)
            {
                if (isUnityHasSamePosition
                    && isCodeEditorHasSamePosition
                    && state.PrevMaximized == isUnityEditorMaximized)
                {
                    return false;
                }
            }

            int posY = currentPos.Y;

            if (isUnityEditorMaximized == false)
            {
                posY += state.TopIndent;
            }

            if (ScriptEditorInternalWinApi.SetWindowPos(state.Hwnd.Value, IntPtr.Zero, 
                currentPos.X, posY, 
                currentPos.Width, currentPos.Height, ScriptEditorInternalWinApi.SWP_NOACTIVATE | ScriptEditorInternalWinApi.SWP_NOZORDER) == false)
            {
                isError = true;

                return false;
            }

            ScriptEditorInternalWinApi.GetWindowRect(state.Hwnd.Value, out currentCodeScriptEditorInternalRect);

            newCodeRect = new ScriptEditorRect(currentCodeScriptEditorInternalRect);

            state.PrevPos = currentPos;
            state.PrevCodeEditorPos = newCodeRect;
            state.PrevMaximized = isUnityEditorMaximized;

            return true;
        }

        /// <summary>
        /// Check if code window is minimized
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public static bool IsCodeWindowMinimized(ScriptEditorState state)
        {
            return ScriptEditorInternalWinApi.IsIconic(state.Hwnd.Value);
        }

        /// <summary>
        /// Check if code window is foreground
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public static bool IsCodeWindowForeground(ScriptEditorState state)
        {
            return ScriptEditorInternalWinApi.GetForegroundWindow() == state.Hwnd.Value;
        }

        /// <summary>
        /// Restore code window if iconic
        /// </summary>
        /// <param name="state"></param>
        public static void RestoreIfIconic(ScriptEditorState state)
        {
            if (ScriptEditorInternalWinApi.IsIconic(state.Hwnd.Value))
            {
                ScriptEditorInternalWinApi.ShowWindow(state.Hwnd.Value, ScriptEditorInternalWinApi.SW_RESTORE);
            }
        }

        /// <summary>
        /// Activate code window
        /// </summary>
        /// <param name="state"></param>
        public static void ActivateCodeWindow(ScriptEditorState state)
        {
            RestoreIfIconic(state);

            var topIndent = state.TopIndent;

            ScriptEditorInternalWinApi.SetWindowPos(state.Hwnd.Value, IntPtr.Zero, 0, 0, 0, 0, ScriptEditorInternalWinApi.SWP_NOMOVE | ScriptEditorInternalWinApi.SWP_NOSIZE);

            ScriptEditorInternalPoint lpScriptEditorInternalPoint;
            ScriptEditorInternalWinApi.GetCursorPos(out lpScriptEditorInternalPoint);

            ScriptEditorInternalWinApi.ScreenToClient(state.Hwnd.Value, ref lpScriptEditorInternalPoint);

            int coordinates = lpScriptEditorInternalPoint.X | (lpScriptEditorInternalPoint.Y << 16);

            ScriptEditorInternalWinApi.PostMessage(state.Hwnd.Value, ScriptEditorInternalWinApi.WM_LBUTTONDOWN, (IntPtr)0x1, (IntPtr)coordinates);
            ScriptEditorInternalWinApi.PostMessage(state.Hwnd.Value, ScriptEditorInternalWinApi.WM_LBUTTONUP, (IntPtr)0x1, (IntPtr)coordinates);
        }

        /// <summary>
        /// Deactivate code window
        /// </summary>
        /// <param name="state"></param>
        public static void DeactivateCodeWindow(ScriptEditorState state)
        {
        }

        /// <summary>
        /// Check if window is maximized
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        static bool IsMaximized(ScriptEditorState state)
        {
            int exStyle = ScriptEditorInternalWinApi.GetWindowLong(state.Hwnd.Value, ScriptEditorInternalWinApi.GWL_STYLE);

            return (exStyle & ScriptEditorInternalWinApi.WS_MAXIMIZE) == ScriptEditorInternalWinApi.WS_MAXIMIZE;
        }

        /// <summary>
        /// Init code window
        /// </summary>
        /// <param name="state"></param>
        /// <param name="prefferedSupportType"></param>
        /// <param name="productName"></param>
        /// <param name="openProjectCallback"></param>
        /// <returns></returns>
        public static bool Init(ScriptEditorState state, ScriptEditorSupportType prefferedSupportType, string productName)
        {
            var vsWindows = GetWindows(prefferedSupportType);

            ScriptEditorWindowInfo info = null;

            if (string.IsNullOrEmpty(productName) == false)
            {
                foreach (var current in vsWindows)
                {
                    switch (prefferedSupportType)
                    {
                        case ScriptEditorSupportType.MsVisualStudio:
                            if (current.Text.Contains("Microsoft Visual Studio")
                                && current.Text.Contains(productName))
                            {
                                info = current;
                            }
                            break;
                        case ScriptEditorSupportType.JetBrainsRider:
                            if (current.Text == productName
                                || current.Text.StartsWith(productName + " "))
                            {
                                info = current;
                            }
                            break;
                        case ScriptEditorSupportType.VsCode:
                            if (current.Text.Contains("Visual Studio Code")
                                &&current.Text.Contains(productName))
                            {
                                info = current;
                            }
                            break;
                    }
                }
            }

            if (info == null)
            {
                return false;
            }

            state.Hwnd.Value = info.Hwnd;

            state.SupportType = prefferedSupportType;

            //if (state.SupportType == ScriptEditorSupportType.MsVisualStudio)
            //{
            //    state.AttachedWindows = GetGlowBorders(state);
            //}

            state.IsInited = true;

            return true;
        }

        /// <summary>
        /// Reset state
        /// </summary>
        /// <param name="state"></param>
        public static void ResetState(ScriptEditorState state)
        {
            state.IsInited = false;

            state.PrevPos = new ScriptEditorRect();
            state.PrevCodeEditorPos = new ScriptEditorRect();
            state.IsOpenProjectExecuted = false;
            state.Hwnd = new ScriptEditorIntPtr();
            
            state.AttachedWindows = null;
        }

        /// <summary>
        /// Get glow borders from Visual Studio window
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        private static ScriptEditorIntPtr[] GetGlowBorders(ScriptEditorState state)
        {
            List<ScriptEditorIntPtr> glowPtrs = new List<ScriptEditorIntPtr>();

            uint processId;

            ScriptEditorInternalWinApi.GetWindowThreadProcessId(state.Hwnd.Value, out processId);

            ScriptEditorInternalWinApi.EnumWindows((hWnd, lParam) =>
            {
                uint glowWindowProcessId;

                ScriptEditorInternalWinApi.GetWindowThreadProcessId(hWnd, out glowWindowProcessId);

                if (glowWindowProcessId != processId)
                {
                    return true;
                }

                const int capacity = 100;

                StringBuilder builder = new StringBuilder(capacity);
                ScriptEditorInternalWinApi.GetClassName(hWnd, builder, capacity);

                if (builder != null
                    && builder.ToString() == "VisualStudioGlowWindow")
                {
                    glowPtrs.Add(new ScriptEditorIntPtr(hWnd));
                }

                return true;
            }, IntPtr.Zero);

            return glowPtrs.ToArray();
        }
        
        /// <summary>
        /// Get window text
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        private static string GetWindowText(IntPtr hWnd)
        {
            int size = ScriptEditorInternalWinApi.GetWindowTextLength(hWnd);
            if (size > 0)
            {
                var builder = new StringBuilder(size + 1);
                ScriptEditorInternalWinApi.GetWindowText(hWnd, builder, builder.Capacity);
                return builder.ToString();
            }

            return string.Empty;
        }

        /// <summary>
        /// Get window text with timeout
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static string GetWindowTextTimeout(IntPtr hWnd, uint timeout)
        {
            const int WM_GETTEXT       = 0x000D;
            const int WM_GETTEXTLENGTH = 0x000E;

            int length;

            if (ScriptEditorInternalWinApi.SendMessageTimeout(hWnd, WM_GETTEXTLENGTH, 0, null, 2, timeout, out length) == 0)
            {
                return string.Empty;
            }

            if (length == 0)
            {
                return string.Empty;
            }

            var builder = new StringBuilder(length + 1);
            ScriptEditorInternalWinApi.GetWindowText(hWnd, builder, builder.Capacity);
            return builder.ToString();
        }

        /// <summary>
        /// Get windows
        /// </summary>
        /// <returns></returns>
        private static List<ScriptEditorWindowInfo> GetWindows(ScriptEditorSupportType supportType,
                                                               bool withText = true)
        {
            List<ScriptEditorWindowInfo> result = new List<ScriptEditorWindowInfo>();

            ScriptEditorInternalWinApi.EnumWindows((wnd, param) =>
            {
                if (supportType == ScriptEditorSupportType.JetBrainsRider)
                {
                    var sb = new StringBuilder(100);
                    ScriptEditorInternalWinApi.GetClassName(wnd, sb, 100);
                    
                    if (sb.ToString() != "SunAwtFrame")
                    {
                        return true;
                    }
                }
                
                if (withText)
                {
                    string text = GetWindowTextTimeout(wnd, 10);

                    if (string.IsNullOrEmpty(text) == false
                        && text.Contains(" Unity ") == false)
                    {
                        result.Add(new ScriptEditorWindowInfo
                        {
                            Text = text,
                            Hwnd = wnd
                        });
                    }
                }
                else
                {
                    result.Add(new ScriptEditorWindowInfo
                    {
                        Text = string.Empty,
                        Hwnd = wnd
                    });
                }

                return true;
            }, IntPtr.Zero);

            return result;
        }
    }
}
