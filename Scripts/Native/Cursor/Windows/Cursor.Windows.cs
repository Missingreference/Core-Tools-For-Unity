using System.Runtime.InteropServices;

namespace Elanetic.Native.Windows
{
    /// <summary>
    /// Set the cursor to native icons for Windows. These functions will only work on Windows otherwise exceptions will be thrown. 
    /// A note for Unity: On mouse move Unity will reset the system cursor to the arrow cursor so these functions will need to be called again upon any mouse movement. MacOS does not have this issue.
    /// </summary>
    static public class Cursor
	{
		[DllImport("NativeCursor")]
		static public extern void SetAppStartingCursor();
        [DllImport("NativeCursor")]
        static public extern void SetArrowCursor();
        [DllImport("NativeCursor")]
        static public extern void SetCrossCursor();
        [DllImport("NativeCursor")]
        static public extern void SetHandCursor();
        [DllImport("NativeCursor")]
        static public extern void SetHelpCursor();
        [DllImport("NativeCursor")]
        static public extern void SetIBeamCursor();
        [DllImport("NativeCursor")]
        static public extern void SetIconCursor();
        [DllImport("NativeCursor")]
        static public extern void SetNoCursor();
        [DllImport("NativeCursor")]
        static public extern void SetSizeCursor();
        [DllImport("NativeCursor")]
        static public extern void SetSizeAllCursor();
        [DllImport("NativeCursor")]
        static public extern void SetSizeNESWCursor();
        [DllImport("NativeCursor")]
        static public extern void SetSizeNSCursor();
        [DllImport("NativeCursor")]
        static public extern void SetSizeNWSECursor();
        [DllImport("NativeCursor")]
        static public extern void SetSizeWECursor();
        [DllImport("NativeCursor")]
        static public extern void SetUpArrowCursor();
        [DllImport("NativeCursor")]
        static public extern void SetWaitCursor();
    }
}