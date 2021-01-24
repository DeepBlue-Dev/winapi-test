using System;
using System.Runtime.InteropServices;

namespace notepad
{
    class Program
    {
        [StructLayout(LayoutKind.Sequential)]
    public struct SECURITY_ATTRIBUTES
    {
        public int nLength;
        public IntPtr lpSecurityDescriptor;
        public int bInheritHandle;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    struct STARTUPINFO
    {
        public Int32 cb;    //  the size of the structure in bytes
        public IntPtr lpReserved;   //  reserved, has to be NULL
        public IntPtr lpDesktop;    //  name of the desktop and or windows station for the process (if there is a / it is both)
        public IntPtr lpTitle;  //  only used if console process, this is the title in the title bar 
        public Int32 dwX;   //  if dwFlags specifies STARTF_USEPOSITION, then this is the x offset, otherwise ignored
        public Int32 dwY;   //  same but for y offset
        public Int32 dwXSize;   //  if dwFlags specifies STARTF_USESIZE then this is the width of the window in pixels
        public Int32 dwYSize;   //  same but for the lenght  y in pixels
        public Int32 dwXCountChars;   //    if dwFlags specifies STARTF_USECOUNTCHARS && a new console window is created in the console process, this is the screen buffer width in char columns
        public Int32 dwYCountChars;   //    same, but for the y buffer width
        public Int32 dwFillAttribute; //    if dwFlags specifies STARTF_USEFILLATTRIBUTE, this is the initialtext and background colors if a new console windows is created in a console application (same as process?)
        public Int32 dwFlags;   // bitfield that specifies what STARTUPINFO members will be used when the process creates a window, for possible values https://docs.microsoft.com/en-us/windows/win32/api/processthreadsapi/ns-processthreadsapi-startupinfoa
        public Int16 wShowWindow;   //  if dwFlags specifies STARTF_USESHOWWINDOW this can have any of the values that can be specified in, nCmdShow
        public Int16 cbReserved2;   //  reserved for c runtime, must be zero
        public IntPtr lpReserved2;  //  reserved for c runtime, must be NULL
        public IntPtr hStdInput;    //  if dwFlags specifies STARTF_USEHANDLES, this is the standard input handle for the process (if STARTF_USESTDHANDLES is not specified, stdin will be keyboard buffer)
        public IntPtr hStdOutput;   //  if dwFlags specifies STARTF_USESTDHANDLES this is the standard output handle for the process, otherwise this member is ignored and stdout will be console window's buffer
        public IntPtr hStdError;    //  if dwFlags specifies STARTF_USESTDHANDLES  this is the standard error handle for the process, ohterwise this is ignoder and stderr will be console window's buffer
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct PROCESS_INFORMATION
    {
        public IntPtr hProcess; //  handle to the newlie created process, used to specify the process in all functions that perform operations on the process object
        public IntPtr hThread;  //  handle to the primary thread of the newly created process, used to specify thread to perform operations on it
        public int dwProcessId; //  value that can be used to indentify process, valid from the moment the process is created till all handles are closed and the object is freed.
        public int dwThreadId;  //  value that can be used to identify the thread, valid from the moment the process is created till all handles are closed and the object is freed.    
    }

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    static extern bool CreateProcess(
        string lpApplicationName,
        string lpCommandLine,
        IntPtr lpProcessAttributes,
        IntPtr lpThreadAttributes,
        bool bInheritHandles,
        uint dwCreationFlags,
        IntPtr lpEnvironment,
        string lpCurrentDirectory,
        [In] ref STARTUPINFO lpStartupInfo,
        out PROCESS_INFORMATION lpProcessInformation
    );

    static void Main(string[] args)
    {
        const uint NORMAL_PRIORITY_CLASS = 0x0020;  //  no special scheduling needs for the program (it hasn't any significant priority)

        bool retValue;
        string Application = Environment.GetEnvironmentVariable("windir") + @"\Notepad.exe";
        string CommandLine = @"pwnd";
        PROCESS_INFORMATION pInfo = new PROCESS_INFORMATION();
        STARTUPINFO sInfo = new STARTUPINFO();

        SECURITY_ATTRIBUTES pSec = new SECURITY_ATTRIBUTES();   //  PROCESS_INFORMATION is basically the same as a uint (not sure if it is uint or int)
        SECURITY_ATTRIBUTES tSec = new SECURITY_ATTRIBUTES();
        pSec.nLength = Marshal.SizeOf(pSec);    //  returns the unmanaged size of the memory in bytes
        tSec.nLength = Marshal.SizeOf(tSec); 
        
        retValue = CreateProcess(Application, CommandLine, pSec.lpSecurityDescriptor, tSec.lpSecurityDescriptor, false, NORMAL_PRIORITY_CLASS, IntPtr.Zero, null, ref sInfo, out pInfo);

        Console.WriteLine("PID: {0}", pInfo.dwProcessId);
        Console.WriteLine("Process Handle: {0}", pInfo.hProcess);

    }
        
    }
}
