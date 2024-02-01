using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace IntelligentPcbaTester
{
    partial class Novaflash
    {
        // FNDLL_API fnConnHandle_p fn_openLanConnection(const WCHAR *ip);
        [DllImport("fn-dll.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "fn_openLanConnection")]
        private static extern IntPtr OpenLanConnection(string ip);

        // FNDLL_API fnConnHandle_p fn_openSerialConnection(const WCHAR *device, int baudRate);
        [DllImport("fn-dll.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "fn_openSerialConnection")]
        private static extern IntPtr OpenSerialConnection(string device, int baudRate);

        // FNDLL_API void fn_CloseConnection(fnConnHandle_p handle);
        [DllImport("fn-dll.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "fn_CloseConnection")]
        private static extern void CloseConnection(IntPtr handle);

        // FNDLL_API int fn_sendFile(fnConnHandle_p handle, int fileType, const WCHAR *srcFilePath, const WCHAR *destFileName);
        [DllImport("fn-dll.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "fn_sendFile")]
        private static extern int SendFile(IntPtr handle, FileType fileType, string srcFilePath, string destFileName);

        // FNDLL_API int fn_getFile(fnConnHandle_p handle, int fileType, const WCHAR *dstFilePath, const WCHAR *srcFileName);
        [DllImport("fn-dll.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "fn_getFile")]
        private static extern int GetFile(IntPtr handle, FileType fileType, string dstFilePath, string srcFileName);

        // FNDLL_API int fn_sendCommand(fnConnHandle_p handle, const char *cmd);
        [DllImport("fn-dll.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "fn_sendCommand")]
        private static extern int SendCommand(IntPtr handle, string cmd);

        // FNDLL_API int fn_waitEndCommand(fnConnHandle_p handle, int timeOut, unsigned long *retCode);
        [DllImport("fn-dll.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "fn_waitEndCommand")]
        private static extern int WaitEndCommand(IntPtr handle, int timeout, out ulong retCode);

        // FNDLL_API int fn_getRespLine(fnConnHandle_p handle, const char **line);
        [DllImport("fn-dll.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "fn_getRespLine")]
        private static extern int GetRespLine(IntPtr handle, out IntPtr ptrLine);

        // FNDLL_API int fn_clearResp(fnConnHandle_p handle);
        [DllImport("fn-dll.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "fn_clearResp")]
        private static extern int ClearResp(IntPtr handle);

        // FNDLL_API int fn_execCommand(fnConnHandle_p handle, const char *cmd, int timeOut, unsigned long *retCode);
        [DllImport("fn-dll.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "fn_execCommand")]
        private static extern int ExecCommand(IntPtr handle, string cmd, int timeout, out ulong retCode);

        // FNDLL_API void fn_setProgressCb(fnConnHandle_p handle, fnProgressEvent_p cb, void* clientp);
        public delegate int ProgressEvent(IntPtr clienttp, double dltotal, double dlnow, double ultotal, double ulnow);
        [DllImport("fn-dll.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "fn_setProgressCb")]
        private static extern void SetProgressCb(IntPtr handle, ProgressEvent cb, IntPtr clientp);
    }
}
