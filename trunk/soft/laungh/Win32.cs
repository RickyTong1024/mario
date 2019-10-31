using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Win32
{
	
	/// <summary>
	/// Structures to interoperate with the Windows 32 API  
	/// </summary>

	#region POINT
	[StructLayout(LayoutKind.Sequential)]
	public struct POINT
	{
		public int x;
		public int y;
	}
	#endregion

    public class Win32API
    {
        #region .ctor()
        // No need to construct this object
        private Win32API()
        {
        }
        #endregion

        #region User32.dll functions
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static public extern bool ClientToScreen(IntPtr hWnd, ref POINT pt);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        static public extern bool MoveWindow(IntPtr hWnd, int x, int y, int width, int height, bool repaint);
        #endregion
    }
}

