/*
 * Keysight VISA.NET Example in C#
 * -------------------------------------------------------------------
 * This program illustrates a few commonly used programming
 * features of your Keysight InfiniiVision oscilloscope.
 * -------------------------------------------------------------------
 */
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Ivi.Visa;
using Ivi.Visa.FormattedIO;

namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            // Change this variable to the address of your instrument
            string VISA_ADDRESS = "TCPIP0::141.121.230.6::inst0::INSTR";

            // Create a connection (session) to the instrument
            IMessageBasedSession session;
            try
            {
                session = GlobalResourceManager.Open(VISA_ADDRESS) as
                IMessageBasedSession;
            }
            catch (NativeVisaException visaException)
            {
                Console.WriteLine("Couldn't connect.");
                Console.WriteLine("Error is:\r\n{0}\r\n", visaException);
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                return;
            }

            // Create a formatted I/O object which will help us format the
            // data we want to send/receive to/from the instrument
            MessageBasedFormattedIO myScope =
            new MessageBasedFormattedIO(session);

            // For Serial and TCP/IP socket connections enable the read
            // Termination Character, or read's will timeout
            if (session.ResourceName.Contains("ASRL") ||
            session.ResourceName.Contains("SOCKET"))
                session.TerminationCharacterEnabled = true;
            session.TimeoutMilliseconds = 20000;

            // Initialize - start from a known state.
            // ==============================================================
            string strResults;
            FileStream fStream;

            // Get and display the device's *IDN? string.
            myScope.WriteLine("*IDN?");
            strResults = myScope.ReadLine();
            Console.WriteLine("*IDN? result is: {0}", strResults);

            // Clear status and load the default setup.
            myScope.WriteLine("*CLS");
            myScope.WriteLine("*RST");

            // Capture data.
            // ==============================================================

            // Use auto - scale to automatically configure oscilloscope.
            myScope.WriteLine(":AUToscale");

            // Set trigger mode (EDGE, PULSe, PATTern, etc., and input source.
            myScope.WriteLine(":TRIGger:MODE EDGE");
            myScope.WriteLine(":TRIGger:MODE?");
            strResults = myScope.ReadLine();
            Console.WriteLine("Trigger mode: {0}", strResults);

            // Set EDGE trigger parameters.
            myScope.WriteLine(":TRIGger:EDGE:SOURce CHANnel1");
            myScope.WriteLine(":TRIGger:EDGE:SOURce?");
            strResults = myScope.ReadLine();
            Console.WriteLine("Trigger edge source: {0}", strResults);

            myScope.WriteLine(":TRIGger:EDGE:LEVel 1.5");
            myScope.WriteLine(":TRIGger:EDGE:LEVel?");
            strResults = myScope.ReadLine();
            Console.WriteLine("Trigger edge level: {0}", strResults);

            myScope.WriteLine(":TRIGger:EDGE:SLOPe POSitive");
            myScope.WriteLine(":TRIGger:EDGE:SLOPe?");
            strResults = myScope.ReadLine();
            Console.WriteLine("Trigger edge slope: {0}", strResults);

            // Save oscilloscope configuration.
            byte[] ResultsArray; // Results array.
            int nLength; // Number of bytes returned from instrument.
            string strPath;

            // Query and read setup string.
            myScope.WriteLine(":SYSTem:SETup?");
            ResultsArray = myScope.ReadLineBinaryBlockOfByte();
            nLength = ResultsArray.Length;

            // Write setup string to file.
            strPath = "c:\\scope\\config\\setup.stp";
            fStream = File.Open(strPath, FileMode.Create);
            fStream.Write(ResultsArray, 0, nLength);
            fStream.Close();
            Console.WriteLine("Setup bytes saved: {0}", nLength);

            // Change settings with individual commands:

            // Set vertical scale and offset.
            myScope.WriteLine(":CHANnel1:SCALe 0.05");
            myScope.WriteLine(":CHANnel1:SCALe?");
            strResults = myScope.ReadLine();
            Console.WriteLine("Channel 1 vertical scale: {0}", strResults);

            myScope.WriteLine(":CHANnel1:OFFSet -1.5");
            myScope.WriteLine(":CHANnel1:OFFSet?");
            strResults = myScope.ReadLine();
            Console.WriteLine("Channel 1 vertical offset: {0}", strResults);

            // Set horizontal scale and offset.
            myScope.WriteLine(":TIMebase:SCALe 0.0002");
            myScope.WriteLine(":TIMebase:SCALe?");
            strResults = myScope.ReadLine();
            Console.WriteLine("Timebase scale: {0}", strResults);

            myScope.WriteLine(":TIMebase:POSition 0.0");
            myScope.WriteLine(":TIMebase:POSition?");
            strResults = myScope.ReadLine();
            Console.WriteLine("Timebase position: {0}", strResults);

            // Set the acquisition type (NORMal, PEAK, AVERage, or HRESolution).
            myScope.WriteLine(":ACQuire:TYPE NORMal");
            myScope.WriteLine(":ACQuire:TYPE?");
            strResults = myScope.ReadLine();
            Console.WriteLine("Acquire type: {0}", strResults);

            // Or, configure by loading a previously saved setup.
            byte[] DataArray;
            int nBytesWritten;

            // Read setup string from file.
            strPath = "c:\\scope\\config\\setup.stp";
            DataArray = File.ReadAllBytes(strPath);
            nBytesWritten = DataArray.Length;

            // Restore setup string.
            myScope.Write(":SYSTem:SETup ");
            myScope.WriteBinary(DataArray);
            myScope.WriteLine("");
            Console.WriteLine("Setup bytes restored: {0}", nBytesWritten);

            // Capture an acquisition using :DIGitize.
            myScope.WriteLine(":DIGitize CHANnel1");

            // Analyze the captured waveform.
            // ==============================================================

            // Make a couple of measurements.
            // -----------------------------------------------------------
            myScope.WriteLine(":MEASure:SOURce CHANnel1");
            myScope.WriteLine(":MEASure:SOURce?");
            strResults = myScope.ReadLine();
            Console.WriteLine("Measure source: {0}", strResults);

            double fResult;
            myScope.WriteLine(":MEASure:FREQuency");
            myScope.WriteLine(":MEASure:FREQuency?");
            fResult = myScope.ReadLineDouble();
            Console.WriteLine("Frequency: {0:F4} kHz", fResult / 1000);

            myScope.WriteLine(":MEASure:VAMPlitude");
            myScope.WriteLine(":MEASure:VAMPlitude?");
            fResult = myScope.ReadLineDouble();
            Console.WriteLine("Vertical amplitude: {0:F2} V", fResult);

            // Download the screen image.
            // -----------------------------------------------------------
            myScope.WriteLine(":HARDcopy:INKSaver OFF");

            // Get the screen data.
            myScope.WriteLine(":DISPlay:DATA? PNG, COLor");
            ResultsArray = myScope.ReadLineBinaryBlockOfByte();
            nLength = ResultsArray.Length;

            // Store the screen data to a file.
            strPath = "c:\\scope\\data\\screen.png";
            fStream = File.Open(strPath, FileMode.Create);
            fStream.Write(ResultsArray, 0, nLength);
            fStream.Close();
            Console.WriteLine("Screen image ({0} bytes) written to {1}",
            nLength, strPath);

            // Download waveform data.
            // -----------------------------------------------------------

            // Set the waveform points mode.
            myScope.WriteLine(":WAVeform:POINts:MODE RAW");
            myScope.WriteLine(":WAVeform:POINts:MODE?");
            strResults = myScope.ReadLine();
            Console.WriteLine("Waveform points mode: {0}", strResults);

            // Get the number of waveform points available.
            myScope.WriteLine(":WAVeform:POINts?");
            strResults = myScope.ReadLine();
            Console.WriteLine("Waveform points available: {0}", strResults);

            // Set the waveform source.
            myScope.WriteLine(":WAVeform:SOURce CHANnel1");
            myScope.WriteLine(":WAVeform:SOURce?");
            strResults = myScope.ReadLine();
            Console.WriteLine("Waveform source: {0}", strResults);

            // Choose the format of the data returned (WORD, BYTE, ASCII):
            myScope.WriteLine(":WAVeform:FORMat BYTE");
            myScope.WriteLine(":WAVeform:FORMat?");
            strResults = myScope.ReadLine();
            Console.WriteLine("Waveform format: {0}", strResults);

            // Display the waveform settings:
            double[] fResultsArray;
            myScope.WriteLine(":WAVeform:PREamble?");
            fResultsArray = myScope.ReadLineListOfDouble();

            double fFormat = fResultsArray[0];
            if (fFormat == 0.0)
            {
                Console.WriteLine("Waveform format: BYTE");
            }
            else if (fFormat == 1.0)
            {
                Console.WriteLine("Waveform format: WORD");
            }
            else if (fFormat == 2.0)
            {
                Console.WriteLine("Waveform format: ASCii");
            }

            double fType = fResultsArray[1];
            if (fType == 0.0)
            {
                Console.WriteLine("Acquire type: NORMal");
            }
            else if (fType == 1.0)
            {
                Console.WriteLine("Acquire type: PEAK");
            }
            else if (fType == 2.0)
            {
                Console.WriteLine("Acquire type: AVERage");
            }
            else if (fType == 3.0)
            {
                Console.WriteLine("Acquire type: HRESolution");
            }

            double fPoints = fResultsArray[2];
            Console.WriteLine("Waveform points: {0:e}", fPoints);

            double fCount = fResultsArray[3];
            Console.WriteLine("Waveform average count: {0:e}", fCount);

            double fXincrement = fResultsArray[4];
            Console.WriteLine("Waveform X increment: {0:e}", fXincrement);

            double fXorigin = fResultsArray[5];
            Console.WriteLine("Waveform X origin: {0:e}", fXorigin);

            double fXreference = fResultsArray[6];
            Console.WriteLine("Waveform X reference: {0:e}", fXreference);

            double fYincrement = fResultsArray[7];
            Console.WriteLine("Waveform Y increment: {0:e}", fYincrement);

            double fYorigin = fResultsArray[8];
            Console.WriteLine("Waveform Y origin: {0:e}", fYorigin);

            double fYreference = fResultsArray[9];
            Console.WriteLine("Waveform Y reference: {0:e}", fYreference);

            // Read waveform data.
            myScope.WriteLine(":WAVeform:DATA?");
            ResultsArray = myScope.ReadLineBinaryBlockOfByte();
            nLength = ResultsArray.Length;
            Console.WriteLine("Number of data values: {0}", nLength);

            // Set up output file:
            strPath = "c:\\scope\\data\\waveform_data.csv";
            if (File.Exists(strPath)) File.Delete(strPath);

            // Open file for output.
            StreamWriter writer = File.CreateText(strPath);

            // Output waveform data in CSV format.
            for (int i = 0; i < nLength - 1; i++)
                writer.WriteLine("{0:f9}, {1:f6}",
                fXorigin + ((float)i * fXincrement),
                (((float)ResultsArray[i] - fYreference)
                * fYincrement) + fYorigin);

            // Close output file.
            writer.Close();
            Console.WriteLine("Waveform format BYTE data written to {0}",
            strPath);

            // Close the connection to the instrument
            // --------------------------------------------------------------
            session.Dispose();

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
