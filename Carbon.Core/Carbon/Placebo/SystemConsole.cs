#if LINUX
using System.IO;
using System.Text;

namespace System
{
    public static class Console
    {
        public static bool IsInputRedirected => false;

        public static bool IsOutputRedirected => false;

        public static bool IsErrorRedirected => false;

        public static TextReader In => null;
        public static TextWriter Out => null;
        public static TextWriter Error => null;
        public static Encoding InputEncoding => null;
        public static Encoding OutputEncoding => null;

        public static void Beep ()
        {
        }

        public static void Beep ( int frequency, int duration )
        {
        }

        public static void Clear ()
        {
        }

        public static ConsoleColor BackgroundColor
        {
            get => ConsoleColor.Black;
            set { }
        }

        public static ConsoleColor ForegroundColor
        {
            get => ConsoleColor.White;
            set { }
        } // public static ConsoleColor ForegroundColor

        public static void ResetColor ()
        {
        }

        public static void MoveBufferArea ( int sourceLeft, int sourceTop,
            int sourceWidth, int sourceHeight, int targetLeft, int targetTop )
        {
        }

        public unsafe static void MoveBufferArea ( int sourceLeft, int sourceTop,
            int sourceWidth, int sourceHeight, int targetLeft, int targetTop,
            char sourceChar, ConsoleColor sourceForeColor,
            ConsoleColor sourceBackColor )
        {
        }

        public static int BufferHeight
        {
            get => 40;
            set { }
        }

        public static int BufferWidth
        {
            get => 80;
            set { }
        }

        public static void SetBufferSize ( int width, int height )
        {
        }

        public static int WindowHeight
        {
            get => 50;
            set { }
        }

        public static int WindowWidth
        {
            get => 86;
            set { }
        }

        public static unsafe void SetWindowSize ( int width, int height )
        {
        }

        public static int LargestWindowWidth => 0;

        public static int LargestWindowHeight => 0;

        public static int WindowLeft
        {
            get => 0;
            set => SetWindowPosition ( value, WindowTop );
        }

        public static int WindowTop
        {
            get => 0;
            set => SetWindowPosition ( WindowLeft, value );
        }


        public static unsafe void SetWindowPosition ( int left, int top )
        {

        }

        public static int CursorLeft
        {
            get => 0;
            set => SetCursorPosition ( value, CursorTop );
        }

        public static int CursorTop
        {

            get => 0;
            set => SetCursorPosition ( CursorLeft, value );
        }

        public static void SetCursorPosition ( int left, int top )
        {

        }

        public static int CursorSize
        {
            get => 0;
            set
            {

            }
        }

        public static bool CursorVisible
        {
            get => false;
            set
            {

            }
        }

        public static String Title
        {
            get => String.Empty;
            set
            {

            }
        }


        public static ConsoleKeyInfo ReadKey ()
        {
            return ReadKey ( false );
        }

        public static ConsoleKeyInfo ReadKey ( bool intercept )
        {
            return new ConsoleKeyInfo ();
        }

        public static bool KeyAvailable => false;

        public static bool NumberLock => false;

        public static bool CapsLock => false;

        public static bool TreatControlCAsInput
        {
            get => false;
            set
            {
            }
        }


        public static event ConsoleCancelEventHandler CancelKeyPress;

        public static Stream OpenStandardError ()
        {
            return null;
        }

        public static Stream OpenStandardError ( int bufferSize )
        {
            return null;
        }

        public static Stream OpenStandardInput ()
        {
            return null;
        }

        public static Stream OpenStandardInput ( int bufferSize )
        {
            return null;
        }

        public static Stream OpenStandardOutput ()
        {
            return null;
        }

        public static Stream OpenStandardOutput ( int bufferSize )
        {
            return null;
        }

        public static void SetIn ( TextReader newIn )
        {
        }

        public static void SetOut ( TextWriter newOut )
        {
        }


        public static void SetError ( TextWriter newError )
        {
        }

        public static int Read ()
        {
            return 0;
        }

        public static String ReadLine ()
        {
            return null;
        }

        public static void WriteLine ()
        {
        }

        public static void WriteLine ( bool value )
        {

        }

        public static void WriteLine ( char value )
        {
        }

        public static void WriteLine ( char [] buffer )
        {

        }

        public static void WriteLine ( char [] buffer, int index, int count )
        {

        }

        public static void WriteLine ( decimal value )
        {

        }

        public static void WriteLine ( double value )
        {

        }

        public static void WriteLine ( float value )
        {

        }

        public static void WriteLine ( int value )
        {

        }

        public static void WriteLine ( uint value )
        {

        }

        public static void WriteLine ( long value )
        {

        }

        public static void WriteLine ( ulong value )
        {

        }

        public static void WriteLine ( Object value )
        {

        }

        public static void WriteLine ( String value )
        {

        }

        public static void WriteLine ( String format, Object arg0 )
        {

        }

        public static void WriteLine ( String format, Object arg0, Object arg1 )
        {

        }

        public static void WriteLine ( String format, Object arg0, Object arg1, Object arg2 )
        {

        }

        public static void WriteLine ( String format, Object arg0, Object arg1, Object arg2, Object arg3, __arglist )
        {

        }

        public static void WriteLine ( String format, params Object [] arg )
        {

        }

        public static void Write ( String format, Object arg0 )
        {

        }

        public static void Write ( String format, Object arg0, Object arg1 )
        {

        }

        public static void Write ( String format, Object arg0, Object arg1, Object arg2 )
        {

        }

        public static void Write ( String format, Object arg0, Object arg1, Object arg2, Object arg3, __arglist )
        {

        }

        public static void Write ( String format, params Object [] arg )
        {

        }

        public static void Write ( bool value )
        {

        }


        public static void Write ( char value )
        {

        }

        public static void Write ( char [] buffer )
        {

        }

        public static void Write ( char [] buffer, int index, int count )
        {

        }

        public static void Write ( double value )
        {

        }

        public static void Write ( decimal value )
        {

        }

        public static void Write ( float value )
        {

        }

        public static void Write ( int value )
        {

        }

        public static void Write ( uint value )
        {

        }

        public static void Write ( long value )
        {

        }

        public static void Write ( ulong value )
        {

        }

        public static void Write ( Object value )
        {

        }

        public static void Write ( String value )
        {

        }
    }
}
#endif