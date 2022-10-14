using System.Runtime.InteropServices;

namespace Elanetic.Native.MacOS
{
    static public class Cursor
    {

        [DllImport("NativeCursor")]
        static public extern void SetArrowCursor();

        [DllImport("NativeCursor")]
        static public extern void SetIBeamCursor();

        [DllImport("NativeCursor")]
        static public extern void SetPointingHandCursor();

        [DllImport("NativeCursor")]
        static public extern void SetClosedHandCursor();

        [DllImport("NativeCursor")]
        static public extern void SetOpenHandCursor();

        [DllImport("NativeCursor")]
        static public extern void SetResizeLeftCursor();

        [DllImport("NativeCursor")]
        static public extern void SetResizeRightCursor();

        [DllImport("NativeCursor")]
        static public extern void SetResizeLeftRightCursor();

        [DllImport("NativeCursor")]
        static public extern void SetResizeUpCursor();

        [DllImport("NativeCursor")]
        static public extern void SetResizeDownCursor();

        [DllImport("NativeCursor")]
        static public extern void SetResizeUpDownCursor();

        [DllImport("NativeCursor")]
        static public extern void SetResizeLeftRightArrowCursor();

        [DllImport("NativeCursor")]
        static public extern void SetResizeUpDownArrowCursor();

        [DllImport("NativeCursor")]
        static public extern void SetResizeTLBRArrowCursor();

        [DllImport("NativeCursor")]
        static public extern void SetResizeTRBLArrowCursor();

        [DllImport("NativeCursor")]
        static public extern void SetCrosshairCursor();

        [DllImport("NativeCursor")]
        static public extern void SetDisappearingItemCursor();

        [DllImport("NativeCursor")]
        static public extern void SetOperationNotAllowedCursor();

        [DllImport("NativeCursor")]
        static public extern void SetDragLinkCursor();

        [DllImport("NativeCursor")]
        static public extern void SetDragCopyCursor();

        [DllImport("NativeCursor")]
        static public extern void SetContextualMenuCursor();

        [DllImport("NativeCursor")]
        static public extern void SetIBeamForVerticalLayourCursor();
    }
}
