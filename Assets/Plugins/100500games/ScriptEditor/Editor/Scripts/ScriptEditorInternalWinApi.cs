using System;
using System.Runtime.InteropServices;
using System.Text;
// ReSharper disable InconsistentNaming

namespace ScriptEditor
{
    /// <summary>
    /// Winapi functions
    /// </summary>
    public static class ScriptEditorInternalWinApi
    {
        /// <summary>
        /// EnumWindowsProc delegate
        /// </summary>
        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
        
        /// <summary>
        /// GWL_STYLE Win Api constant
        /// </summary>
        public  const int GWL_STYLE = -16;
        /// <summary>
        /// SW_HIDE Win Api constant
        /// </summary>
        public  const int SW_HIDE = 0;
        /// <summary>
        /// HWND_TOP Win Api constant
        /// </summary>
        public const int HWND_TOP = 0;
        /// <summary>
        /// HWND_TOPMOST Win Api constant
        /// </summary>
        public const int HWND_TOPMOST = -1;
        /// <summary>
        /// HWND_BOTTOM Win Api constant
        /// </summary>
        public const int HWND_BOTTOM = 1;
        /// <summary>
        /// SW_MINIMIZE Win Api constant
        /// </summary>
        public const int SW_MINIMIZE = 6;
        /// <summary>
        /// SWP_NOACTIVATE Win Api constant
        /// </summary>
        public const int SWP_NOACTIVATE = 0x0010;
        /// <summary>
        /// SW_RESTORE Win Api constant
        /// </summary>
        public const int SW_RESTORE = 9;
        /// <summary>
        /// SWP_NOZORDER Win Api constant
        /// </summary>
        public const int SWP_NOZORDER = 0x0004;
        /// <summary>
        /// HWND_NOTOPMOST Win Api constant
        /// </summary>
        public const int HWND_NOTOPMOST = -2;
        /// <summary>
        /// SW_SHOW Win Api constant
        /// </summary>
        public const int SW_SHOW = 2;
        /// <summary>
        /// WS_MAXIMIZE Win Api constant
        /// </summary>
        public const int WS_MAXIMIZE = 0x1000000;
        /// <summary>
        /// GWL_EXSTYLE Win Api constant
        /// </summary>
        public const int GWL_EXSTYLE = -20;
        /// <summary>
        /// WS_EX_TOPMOST Win Api constant
        /// </summary>
        public const int WS_EX_TOPMOST = 0x0008;
        /// <summary>
        /// SWP_NOSIZE Win Api constant
        /// </summary>
        public const int SWP_NOSIZE = 0x0001;
        /// <summary>
        /// SWP_NOMOVE Win Api constant
        /// </summary>
        public const int SWP_NOMOVE = 0x0002;
        /// <summary>
        /// WM_LBUTTONDOWN Win Api constant
        /// </summary>
        public const int WM_LBUTTONDOWN = 0x201;
        /// <summary>
        /// WM_LBUTTONUP Win Api constant
        /// </summary>
        public const int WM_LBUTTONUP = 0x202;
        /// <summary>
        /// SWP_SHOWWINDOW Win Api constant
        /// </summary>
        public const int SWP_SHOWWINDOW = 0x0040;

        /// <summary>
        /// ShowWindow Win Api Function
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="nCmdShow"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        
        /// <summary>
        /// GetWindowTextLength Win Api Function
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        /// <summary>
        /// SetWindowPos Win Api Function
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="hWndInsertAfter"></param>
        /// <param name="x"></param>
        /// <param name="Y"></param>
        /// <param name="cx"></param>
        /// <param name="cy"></param>
        /// <param name="wFlags"></param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

        /// <summary>
        /// GetCurrentThreadId Win Api Function
        /// </summary>
        /// <returns></returns>
        [DllImport("Kernel32.dll")]
        public static extern uint GetCurrentThreadId();

        /// <summary>
        /// EnumThreadWindows Win Api Function
        /// </summary>
        /// <param name="dwThreadId"></param>
        /// <param name="lpfn"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern bool EnumThreadWindows(int dwThreadId, EnumWindowsProc lpfn, IntPtr lParam);

        /// <summary>
        /// EnumWindows Win Api Function
        /// </summary>
        /// <param name="enumProc"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);
        /// <summary>
        /// Send message timout
        /// </summary>
        /// <returns></returns>
        [DllImport("User32.dll", SetLastError = true)]
        public static extern int SendMessageTimeout(IntPtr hWnd, uint uMsg, uint wParam, StringBuilder lParam, uint fuFlags, uint uTimeout, out int lpdwResult);
        /// <summary>
        /// Send message timout
        /// </summary>
        /// <returns></returns>
        [DllImport("User32.dll", SetLastError = true)]
        public static extern int SendMessageTimeout(IntPtr hWnd, uint uMsg, uint wParam, StringBuilder lParam, uint fuFlags, uint uTimeout, int? ldow);

        /// <summary>
        /// GetWindowText Win Api Function
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="strText"></param>
        /// <param name="maxCount"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder strText, int maxCount);

        /// <summary>
        /// EnumChildWindows Win Api Function
        /// </summary>
        /// <param name="window"></param>
        /// <param name="callback"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumChildWindows(IntPtr window, EnumWindowsProc callback, IntPtr lParam);

        /// <summary>
        /// IsIconic Win Api Function
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern bool IsIconic(IntPtr hWnd);

        /// <summary>
        /// GetForegroundWindow Win Api Function
        /// </summary>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetForegroundWindow();

        /// <summary>
        /// GetWindowRect Win Api Function
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="lpScriptEditorInternalRect"></param>
        /// <returns></returns>
        [DllImport("USER32.DLL")]
        public static extern bool GetWindowRect(IntPtr hWnd, out ScriptEditorInternalRect lpScriptEditorInternalRect);

        /// <summary>
        /// GetKeyState Win Api Function
        /// </summary>
        /// <param name="keyCode"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern short GetKeyState(int keyCode);

        /// <summary>
        /// GetClassName Win Api Function
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="lpClassName"></param>
        /// <param name="nMaxCount"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        /// <summary>
        /// SetForegroundWindow Win Api Function
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        /// <summary>
        /// GetWindowLong Win Api Function
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="nIndex"></param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        /// <summary>
        /// PostMessage Win Api Function
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="Msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "PostMessageA", SetLastError = true)]
        public static extern bool PostMessage(IntPtr hwnd, uint Msg, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// GetCursorPos Win Api Function
        /// </summary>
        /// <param name="lpScriptEditorInternalPoint"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out ScriptEditorInternalPoint lpScriptEditorInternalPoint);

        /// <summary>
        /// ScreenToClient Win Api Function
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="lpScriptEditorInternalPoint"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern bool ScreenToClient(IntPtr hWnd, ref ScriptEditorInternalPoint lpScriptEditorInternalPoint);
        /// <summary>
        /// GetWindowThreadProcessId Win Api Function
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="lpdwProcessId"></param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);
    }
}