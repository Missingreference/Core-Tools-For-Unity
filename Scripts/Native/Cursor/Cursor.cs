#if WINDOWS || WIN64 || _WIN64 || UNITY_STANDALONE_WIN
#define NATIVE_WINDOWS
#elif OSX || MACOS || __APPLE__ || __MACH__ || UNITY_STANDALONE_OSX
#define NATIVE_MACOS
#endif

namespace Elanetic.Native
{
    /// <summary>
    /// Set the hardware cursor to predefined common icons.
    /// </summary>
    static public class Cursor
    {
        //Default arrow cursor
        static public void SetArrowCursor()
        {
#if NATIVE_WINDOWS
            Windows.Cursor.SetArrowCursor();
#elif NATIVE_MACOS
            MacOS.Cursor.SetArrowCursor();
#endif
        }

        //IBeam cursor for selecting text
        static public void SetIBeamCursor()
        {
#if NATIVE_WINDOWS
            Windows.Cursor.SetIBeamCursor();
#elif NATIVE_MACOS
            MacOS.Cursor.SetIBeamCursor();
#endif
        }

        //Resize left-right arrow cursor
        static public void SetResizeLeftRightCursor()
        {
#if NATIVE_WINDOWS
            Windows.Cursor.SetSizeWECursor();
#elif NATIVE_MACOS
            MacOS.Cursor.SetResizeLeftRightArrowCursor();
#endif
        }

        //Resize up-down arrow cursor
        static public void SetResizeUpDownCursor()
        {
#if NATIVE_WINDOWS
            Windows.Cursor.SetSizeNSCursor();
#elif NATIVE_MACOS
            MacOS.Cursor.SetResizeUpDownArrowCursor();
#endif
        }

        //Resize topleft-bottomright arrow cursor
        static public void SetResizeTLBRCursor()
        {
#if NATIVE_WINDOWS
            Windows.Cursor.SetSizeNWSECursor();
#elif NATIVE_MACOS
            MacOS.Cursor.SetResizeTLBRArrowCursor();
#endif
        }

        //Resize topright-bottomleft arrow cursor
        static public void SetResizeTRBLCursor()
        {
#if NATIVE_WINDOWS
            Windows.Cursor.SetSizeNESWCursor();
#elif NATIVE_MACOS
            MacOS.Cursor.SetResizeTRBLArrowCursor();
#endif
        }
    }
}
