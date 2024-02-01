using CasRnf;
using System;
using System.Collections.Generic;
using System.Text;

namespace CasRnf32NET
{
    /// <summary>
    /// CasRnf32.dll wrapper class for .NET.
    /// </summary>
    public class Cascon
    {
        /// <summary>
        /// The pointer is set to a tCascon structure and has to be provided to every
        /// other function.
        /// The application should not change this pointer later.
        /// </summary>
        static unsafe protected uint p_cas = 0;

        static private CCasRnf.FCheckBreakProc f_check_break = null;
        static private CCasRnf.FReadKeyProc f_read_key = null;
        static private CCasRnf.FMsgBoxProc f_msg_box = null;

        private const string notInitialized = "CASCON is not initialized.";

        /// <summary>
        /// Returns error message according to an error code.
        /// </summary>
        /// <param name="errorCode"></param>
        /// <returns></returns>
        internal static string GetErrorMessage(int errorCode)
        {
            string message;
            switch (errorCode)
            {
                case CCasRnf.casOk:
                    message = "The function was successful.";
                    break;
                case CCasRnf.casError:
                    message = "Faulty call of the function.";
                    break;
                case CCasRnf.casInvalidCall:
                    message = "Call of the function is not allowed here.";
                    break;
                case CCasRnf.casInternalError:
                    // For Client DLLs only.
                    message = "There is no connection to the server process.";
                    break;
                //case CCasRnf.casYes:
                //    // For CasGetNextMsgBox only.
                //    message = "The supplied string is not empty.";
                //    break;
                case CCasRnf.casUutError:
                    message = "Execution of a test or CasIrShift or CasDrShift function detected at least one actual deviation.";
                    break;
                //case CCasRnf.casWildChars:
                //    message = "CasGetFileName returned a file name with wildcards '*' or '?'.";
                //    break;
                case CCasRnf.casVirtualPass:
                    message = "The test was executed using a virtual controller.";
                    break;
                default:
                    message = "Unknown error.";
                    break;
            }

            return message;
        }

        /// <summary>
        /// Initialize CASCON.
        /// </summary>
        /// <exception cref="CasconException"></exception>
        /// <remarks>Call on Level 0.</remarks>
        public static void Init()
        {
            var msg = new StringBuilder(256);
            int returnCode = 0;

            unsafe
            {
                fixed (uint* pp_cas = &p_cas)
                {
                    returnCode = CCasRnf.InitCasRunF(pp_cas, "", 0, f_check_break, f_read_key, f_msg_box, msg);
                }
            }

            if (returnCode != CCasRnf.casOk)
            {
                throw new CasconException(returnCode, msg.ToString());
            }
        }

        /// <summary>
        /// Close CASCON.
        /// </summary>
        /// <remarks>Call on Level 1.</remarks>
        public static void Close()
        {
            CCasRnf.DoneCasRunF(p_cas);
            p_cas = 0;
        }

        /// <summary>
        /// Use this function to modify the name for UUT in the CASCON DLL.
        /// </summary>
        /// <param name="name">UUT name to select.</param>
        /// <exception cref="CasconException"></exception>
        /// <remarks>Call on Level 1.</remarks>
        public static void SelectUut(string name)
        {
            if (p_cas == 0)
            {
                throw new CasconException(CCasRnf.casError, notInitialized);
            }

            int returnCode = CCasRnf.CasSelectUUT(p_cas, name, "", "", CCasRnf.sutUutName);
            if (returnCode != CCasRnf.casOk)
            {
                throw new CasconException(returnCode, GetErrorMessage(returnCode));
            }
        }

        /// <summary>
        /// Selets a batch name.
        /// </summary>
        /// <param name="name">Batch name to select.</param>
        /// <exception cref="CasconException"></exception>
        /// <remarks>Call on Level 1 and 2.</remarks>
        public static void SelectBatch(string name)
        {
            if (p_cas == 0)
            {
                throw new CasconException(CCasRnf.casError, notInitialized);
            }

            int returnCode = CCasRnf.CasSelectBatch(p_cas, name);
            if (returnCode != CCasRnf.casOk)
            {
                throw new CasconException(returnCode, GetErrorMessage(returnCode));
            }
        }

        /// <summary>
        /// Selects a test name.
        /// </summary>
        /// <param name="name">Test name to select.</param>
        /// <exception cref="CasconException"></exception>
        /// <remarks>Call on Level 1 and 2.</remarks>
        public static void SelectTest(string name)
        {
            if (p_cas == 0)
            {
                throw new CasconException(CCasRnf.casError, notInitialized);
            }

            int returnCode = CCasRnf.CasSelectTest(p_cas, name);
            if (returnCode != CCasRnf.casOk)
            {
                throw new CasconException(returnCode, GetErrorMessage(returnCode));
            }
        }

        /// <summary>
        /// Execute the set batch program.
        /// </summary>
        /// <returns>true if batch passed, false otherwise.</returns>
        /// <exception cref="CasconException"></exception>
        /// <remarks>Call on Level 1, not in case of CASCON 3.x projects.</remarks>
        public static bool ExecuteBatch()
        {
            if (p_cas == 0)
            {
                throw new CasconException(CCasRnf.casError, notInitialized);
            }

            int returnCode = CCasRnf.CasExecuteBatch(p_cas);
            switch (returnCode)
            {
                case CCasRnf.casOk:
                    // "Batch passed."
                    break;
                case CCasRnf.casUutError:
                    // "Batch detected actual deviations."
                    break;
                case CCasRnf.casError:
                    throw new CasconException(returnCode, "Batch not run.");
                case CCasRnf.casVirtualPass:
                    // "The batch was executed using a virtual controller."
                    break;
                default:
                    throw new CasconException(returnCode, "Unknown error.");
            }

            return returnCode == CCasRnf.casOk;
        }

        /// <summary>
        /// Independent of a possibly set batch, the set test can be executed by this function.
        /// </summary>
        /// <returns>true if test passed, false otherwise.</returns>
        /// <exception cref="CasconException"></exception>
        /// <remarks>Call on Level 1, not in case of CASCON 3.x projects.</remarks>
        public static bool ExecuteTest()
        {
            if (p_cas == 0)
            {
                throw new CasconException(CCasRnf.casError, notInitialized);
            }

            int returnCode = CCasRnf.CasExecuteTest(p_cas);
            switch (returnCode)
            {
                case CCasRnf.casOk:
                    // "Test passed."
                    break;
                case CCasRnf.casUutError:
                    // "Test detected actual deviations."
                    break;
                case CCasRnf.casError:
                    throw new CasconException(returnCode, "Test not run.");
                case CCasRnf.casVirtualPass:
                    // "The test was executed using a virtual controller."
                    break;
                default:
                    throw new CasconException(returnCode, "Unknown error.");
            }

            return returnCode == CCasRnf.casOk;
        }

        /// <summary>
        /// Generates a list of UUTs contained in the set UUT base directory. The return value
        /// indicates the number of found UUTs, i.e.the number of entries in the list. The list is managed within the
        /// CASCON DLL.
        /// Use the CasUutListEntry and CasUutListEntryComment functions to query the list entries.
        /// Changing the UUT base directory, this list is not automatically updated. The entries are invalid then.
        /// </summary>
        /// <returns>Number of found UUTs.</returns>
        /// <exception cref="CasconException"></exception>
        /// <remarks>Call on Level 1, 2, 3.</remarks>
        public static int UutList()
        {
            if (p_cas == 0)
            {
                throw new CasconException(CCasRnf.casError, notInitialized);
            }

            return CCasRnf.CasUutList(p_cas);
        }

        /// <summary>
        /// Returns a UUT name from the UUT list generated with CasUutList. This name can be
        /// provided as UUT parameter for the CasSelectUUT function.
        /// </summary>
        /// <param name="index">Indicates the entry to be read from the UUT list. The valid range is 0 to CasUutList() - 1.</param>
        /// <param name="name">The application has to provide the address of a (undefined) character array here(256 characters).
        /// The UUT list entry indicated by <paramref name="index"/> is provided as UUT name.</param>
        /// <exception cref="CasconException"></exception>
        /// <remarks>Call on Level 1, 2, 3.</remarks>
        public static string UutListEntry(int index)
        {
            if (p_cas == 0)
            {
                throw new CasconException(CCasRnf.casError, notInitialized);
            }

            var uutName = new StringBuilder(256);
            int returnCode = CCasRnf.CasUutListEntry(p_cas, index, uutName);
            if (returnCode != CCasRnf.casOk)
            {
                throw new CasconException(CCasRnf.casError, "The index was outside the valid range.");
            }

            return uutName.ToString();
        }

        /// <summary>
        /// Returns UUT names from the UUT list generated with CasUutList.
        /// </summary>
        /// <returns>UUT names.</returns>
        /// <remarks>Call on Level 1, 2, 3.</remarks>
        public static List<string> GetUutNames()
        {
            var nameList = new List<string>();
            int count = UutList();
            for (int i = 0; i < count; i++)
            {
                nameList.Add(UutListEntry(i));
            }
            return nameList;
        }

        /// <summary>
        /// Query the UUT base directory used by the CASCON DLL.
        /// </summary>
        /// <returns>UUT base directory.</returns>
        /// <remarks>Call on Level 1, 2, 3.</remarks>
        public static string GetUutBaseDir()
        {
            if (p_cas == 0)
            {
                throw new CasconException(CCasRnf.casError, notInitialized);
            }

            var baseDir = new StringBuilder(256);
            int returnCode = CCasRnf.CasGetUutBaseDir(p_cas, baseDir);
            if (returnCode != CCasRnf.casOk)
            {
                throw new CasconException(returnCode, GetErrorMessage(returnCode));
            }
            return baseDir.ToString();
        }

#if CASAPICLIENT || CASAPICLIENT64
        public static void UnloadAPIClient()
        {
            CCasRnf.UnloadCasAPIClient();
        }
#endif
    }

    /// <summary>
    /// Error information of CasRnf32.dll APIs.
    /// </summary>
    public class CasconException : Exception
    {
        public CasconException()
        {
        }

        public CasconException(int errorCode, string message) : base(message)
        {
            ErrorCode = errorCode;
        }

        /// <summary>
        /// CASCON DLL error code.
        /// </summary>
        public int ErrorCode { get; internal set; }
    }
}
