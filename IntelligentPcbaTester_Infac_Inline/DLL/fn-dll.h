// The following ifdef block is the standard way of creating macros which make exporting 
// from a DLL simpler. All files within this DLL are compiled with the FNDLL_EXPORTS
// symbol defined on the command line. this symbol should not be defined on any project
// that uses this DLL. This way any other project whose source files include this file see 
// FNDLL_API functions as being imported from a DLL, whereas this DLL sees symbols
// defined with this macro as being exported.
#ifdef FNDLL_EXPORTS
#define FNDLL_API __declspec(dllexport)
#else
#define FNDLL_API __declspec(dllimport)
#endif

#ifndef __FN_DLL_H__
#define __FN_DLL_H__

#include <tchar.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <winsock2.h>
#include <ws2tcpip.h>


#define FILE_TYPE_GRP		0
#define FILE_TYPE_DRV		1
#define FILE_TYPE_DUMP		2
#define FILE_TYPE_BATCH		3
#define FILE_TYPE_LOG		4


typedef int (WINAPI *fnProgressEvent_p)(void *clientp, double dltotal, double dlnow, double ultotal, double ulnow);

#ifdef FNDLL_EXPORTS
#include "fnTypes.h"
#else
typedef void *fnConnHandle_p;
#endif  //FNDLL_EXPORTS


/////////////////////////////////////////////////////////////////////////////
#ifdef __cplusplus
extern "C" {
#endif
// ---- UNICODE	---------------------------------------------
FNDLL_API fnConnHandle_p fn_openLanConnection(const WCHAR *ip);
FNDLL_API fnConnHandle_p fn_openSerialConnection(const WCHAR *device, int baudRate);
FNDLL_API int fn_sendFile(fnConnHandle_p handle, int fileType, const WCHAR *srcFilePath, const WCHAR *destFileName);
FNDLL_API int fn_getFile(fnConnHandle_p handle, int fileType, const WCHAR *dstFilePath, const WCHAR *srcFileName);
// ----------------------------------------------------------

// ---- ASCII -----------------------------------------------
FNDLL_API fnConnHandle_p fn_openSerialConnectionA(const char *device, int baudRate);
FNDLL_API fnConnHandle_p fn_openLanConnectionA(const char *ip);
FNDLL_API int fn_sendFileA(fnConnHandle_p handle, int fileType, const char *srcFilePath, const char *destFileName);
FNDLL_API int fn_getFileA(fnConnHandle_p handle, int fileType, const char *dstFilePath, const char *srcFileName);
// ----------------------------------------------------------

FNDLL_API void fn_CloseConnection(fnConnHandle_p handle);

FNDLL_API int fn_sendCommand(fnConnHandle_p handle, const char *cmd);
FNDLL_API int fn_waitEndCommand(fnConnHandle_p handle, int timeOut, unsigned long *retCode);
FNDLL_API int fn_getRespLine(fnConnHandle_p handle, const char **line);
FNDLL_API int fn_getRespLineVI(fnConnHandle_p handle, char * const buff, int maxSize);
FNDLL_API int fn_clearResp(fnConnHandle_p handle);
FNDLL_API int fn_execCommand(fnConnHandle_p handle, const char *cmd, int timeOut, unsigned long *retCode);


FNDLL_API void fn_setProgressCb(fnConnHandle_p handle, fnProgressEvent_p cb, void *clientp);

#ifdef __cplusplus
}
#endif


#endif  //__FN_DLL_H__
