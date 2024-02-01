//<?xml version="1.0" standalone="yes"?>
//<EmbeddedScriptDataSet xmlns="http://tempuri.org/EmbeddedScriptDataSet.xsd">
//  <EMBEDDED_SCRIPTS>
//    <CODE_TEXT>using TestFramework.PluginTestMethod;
using TestFramework.Script.Diagnostics;
using TestFramework.VirtualTestSets.TestSet_A1;
using TestFramework.VirtualTestSets.TestSet_A1.Devices;
using Peak.Can.Basic;
using System.Text;
using System;

/*
This example is intended to be the basis for two different possible
application scenarios with external devices.

Scenario 1
The eloZ1 stimulates a voltage which is measured by an external device.
The measurement result is read in e.g. via CAN bus.

Scenario 2
A voltage is stimulated by an external device, which is measured by the eloZ1.
The external device is controlled e.g. via CAN bus.

To make the example work without external hardware, both scenarios were combined.
The eloZ1 stimulates the voltage and then also measures it itself. For your own
application, unneeded code parts must be removed and the control of the external
device must be added. The CAN bus control is only represented by dummy commands.

The CAN bus device is used here as a representative for external devices.
An integration of other devices (e.g. Boundary Scan) would be possible as well.
*/

public class TestMethod
{
    // Configuration of the TestSet (especially the MatrixSize should be adjusted):
    private const string config = "MatrixSize:1024, Stimulus10V, MeasurementUnit";

    public void RequestResources(int ProcessID, TestFramework.PluginTestMethod.TestStep ThisTestStep)
    {
        // request the TestSet
        TestSet.RequestResources(ProcessID, config, TestFramework.VirtualTestSets.TestSet_A1.TestSet.EvaluationMode.EarlyEvaluation);
        // 'EarlyEvaluation' ensures that measurement results can already be obtained within Execute.
    }

    public void Execute(int ProcessID, TestFramework.PluginTestMethod.TestStep ThisTestStep)
    {
        // CAN bus settings
        Peak.Can.Basic.TPCANStatus status;
        ushort          channel   = 81;
        uint            port      = 400;
        ushort          interrupt = 3;

        // The test channels for stimulation or measurement
        int             test_channel_high = 1;
        int             test_channel_low  = 2;

        // Stimulation settings
        double          stim_voltage = 200 * mV;
        double          stim_current =  10 * mA;

        // Measurement settings
        double          meas_range   = 200 * mV;

        double          meas_voltage;
        double          lower_bound;
        double          upper_bound;
        TestSet         ts = null;
        Stimulus10V     stim;
        MeasurementUnit meas;
        TestStep.ResultValueState   state;
        TestStep.ResultState        result;

        try
        {
            // get the TestSet
            ts = TestSet.GetDevice(ProcessID, config);

            // setup and connect Stimulus (this code is for Scenario 1 only)
            stim = ts.Stimulus10V[0];
            stim.SetMatrixConnection(test_channel_high, test_channel_low);
            stim.CommitConnections();
            stim.SetStimulationCurrent(stim_current);
            stim.SetStimulationVoltage(stim_voltage);
            stim.SetPermanentStimulationEnabled();
            stim.Commit();

            // show settings for debug
            Debug.WriteLine(ts.GetSettingsInfo());
            Debug.WriteLine(ts.GetConnectionInfo());

            // apply all settings
            ts.Apply();

            try
            {
                // CAN bus commands
                status = Peak.Can.Basic.PCANBasic.Initialize(
                                        channel,
                                        Peak.Can.Basic.TPCANBaudrate.PCAN_BAUD_500K,
                                        Peak.Can.Basic.TPCANType.PCAN_TYPE_ISA,
                                        port,
                                        interrupt);

                if (status != Peak.Can.Basic.TPCANStatus.PCAN_ERROR_OK)
                    if (status != Peak.Can.Basic.TPCANStatus.PCAN_ERROR_CAUTION)
                        throw new Exception(GetFormatedError(status));

                /*
                Scenario 1:
                Insert the code here to measure the stimulated voltage with the external device.
                Assign the measured value to the variable meas_voltage.

                Scenario 2:
                Insert the code here to generate a voltage with the external device.
                */

                // as a dummy command we get the software version of the API
                StringBuilder str_temp;
                str_temp = new StringBuilder(256);
                status = Peak.Can.Basic.PCANBasic.GetValue(Peak.Can.Basic.PCANBasic.PCAN_NONEBUS, Peak.Can.Basic.TPCANParameter.PCAN_API_VERSION, str_temp, 256);
                if (status == Peak.Can.Basic.TPCANStatus.PCAN_ERROR_OK)
                    Debug.WriteLine("PCAN API Version: " + str_temp.ToString());

                // measure the stimulated voltage with the Measurement Unit (this code is for Scenario 2 only)
                meas = ts.MeasurementUnit[0];
                meas.SetMatrixConnection(test_channel_high, test_channel_low);
                meas.Voltmeter.SetMeasuringRange(meas_range);
                meas.Commit();
                ts.Apply();
                meas.Voltmeter.Measure(out meas_voltage);
                Debug.WriteLine("Meas: " + meas_voltage.ToString() + "V");

                // evaluate the measurement result
                lower_bound = stim_voltage * 0.9;
                upper_bound = stim_voltage * 1.1;

                if (double.IsNaN(meas_voltage))
                    state = TestStep.ResultValueState.Bad;
                if (meas_voltage < lower_bound)
                    state = TestStep.ResultValueState.BadLow;
                else if (meas_voltage > upper_bound)
                    state = TestStep.ResultValueState.BadHigh;
                else
                    state = TestStep.ResultValueState.Good;

                if (state == TestStep.ResultValueState.Good)
                    result = TestStep.ResultState.Pass;
                else
                    result = TestStep.ResultState.Fail;

                // set the test step's result
                ThisTestStep.SetResult(result, "", meas_voltage, lower_bound, upper_bound, TestStep.PhysicalUnit.Volt, state);
            }
            catch (Exception ex)
            {
                ThisTestStep.SetError(ex.Message);
            }
            finally
            {
                Peak.Can.Basic.PCANBasic.Uninitialize(channel);
            }

            // reset the Stimulus and discharge (this code is for Scenario 1 only)
            stim.SetStimulationCurrent(500 * mA);
            stim.SetStimulationVoltage(  0 *  V);
            stim.Commit();
            ts.Timer.PutDelay(1);  // 1sec discharge (or what ever is needed)
            stim.SetPermanentStimulationDisabled();
            ts.Commit();

            // disconnect
            ts.ClearConnections();
            ts.Commit();

            // apply all settings
            ts.Apply();
        }
        catch (Exception ex)
        {
            ThisTestStep.SetError(ex.Message);
        }
        finally
        {
            // release the TestSet
            if (ts != null)
                ts.Dispose();
        }
    }

    private string GetFormatedError(Peak.Can.Basic.TPCANStatus error)
    {
        StringBuilder strTemp = new StringBuilder(256);
        if (Peak.Can.Basic.PCANBasic.GetErrorText(error, 0, strTemp) != Peak.Can.Basic.TPCANStatus.PCAN_ERROR_OK)
            return string.Format("PCAN: An error occurred. Error-code's text ({0:X}) couldn't be retrieved", error);
        else
            return "PCAN: " + strTemp.ToString();
    }

    public void Evaluate(int ProcessID, TestFramework.PluginTestMethod.TestStep ThisTestStep)
    {
        // see Execute
    }

    public string Info()
    {
        return "";
    }

    public void Validate(int ProcessID, TestFramework.PluginTestMethod.TestStep ThisTestStep)
    {
        // insert code to validate the parameters
    }

    // physical unit factors
    const double n      = 1e-9;
    const double nano   = 1e-9;
    const double u      = 1e-6;
    const double micro  = 1e-6;
    const double m      = 1e-3;
    const double milli  = 1e-3;
    const double k      = 1e3;
    const double kilo   = 1e3;
    const double M      = 1e6;
    const double Mega   = 1e6;
    const double G      = 1e9;
    const double Giga   = 1e9;
    const double A      = 1;
    const double mA     = 1e-3;
    const double uA     = 1e-6;
    const double nA     = 1e-9;
    const double V      = 1;
    const double mV     = 1e-3;
    const double uV     = 1e-6;
    const double nV     = 1e-9;
    const double s      = 1;
    const double ms     = 1e-3;
    const double us     = 1e-6;
    const double ns     = 1e-9;
    const double Hz     = 1;
    const double kHz    = 1e3;
    const double MHz    = 1e6;
    const double GHz    = 1e9;
}
//</CODE_TEXT>
//    <LANGUAGE>CSharp</LANGUAGE>
//    <EMBEDDED_SCRIPT_ID>-1</EMBEDDED_SCRIPT_ID>
//    <STANDARD_NAME_SPACE />
//    <ENTRY_METHOD_NAME>Main</ENTRY_METHOD_NAME>
//    <ENTRY_CLASS_NAME>TestMethod</ENTRY_CLASS_NAME>
//    <EDITOR_TAB_SIZE>4</EDITOR_TAB_SIZE>
//    <NAME>Example External Device</NAME>
//    <ASSEMBLY_PATH />
//    <ASSEMBLY_FILE_NAME />
//    <SOURCE_FILE_NAME />
//    <EDITOR_FONT_SIZE>11</EDITOR_FONT_SIZE>
//    <MAJOR_VERSION>1</MAJOR_VERSION>
//    <MINOR_VERSION>0</MINOR_VERSION>
//    <REVISION_NUMBER>0</REVISION_NUMBER>
//    <IS_NAME_EDITABLE>true</IS_NAME_EDITABLE>
//    <LAST_CHANGE_DATE>2021-10-21T08:45:37.0766518+02:00</LAST_CHANGE_DATE>
//    <SYNTAX_HIGHLIGHTING>true</SYNTAX_HIGHLIGHTING>
//    <BREAKS_ENABLED>true</BREAKS_ENABLED>
//    <CATEGORY>TestStepScript</CATEGORY>
//    <SCRIPT_ID>37fb000c-f678-453e-8aaf-dc793adfd3f5</SCRIPT_ID>
//    <CONTEXT>TestProject</CONTEXT>
//    <UNSAFE_CODE_ENABLED>false</UNSAFE_CODE_ENABLED>
//  </EMBEDDED_SCRIPTS>
//  <ASSEMBLY_REFERENCES>
//    <REFERENCE_ID>-1</REFERENCE_ID>
//    <EMBEDDED_SCRIPT_ID>-1</EMBEDDED_SCRIPT_ID>
//    <ASSEMBLY_VERSION>4.0.0.0</ASSEMBLY_VERSION>
//    <ASSEMBLY_NAME>mscorlib</ASSEMBLY_NAME>
//    <ASSEMBLY_SOURCE>Global Assembly Cache</ASSEMBLY_SOURCE>
//    <ASSEMBLY_FULL_NAME>mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</ASSEMBLY_FULL_NAME>
//    <IS_DEFAULT>true</IS_DEFAULT>
//    <IS_REMOVABLE>false</IS_REMOVABLE>
//    <IS_HIDDEN>true</IS_HIDDEN>
//  </ASSEMBLY_REFERENCES>
//  <ASSEMBLY_REFERENCES>
//    <REFERENCE_ID>-3</REFERENCE_ID>
//    <EMBEDDED_SCRIPT_ID>-1</EMBEDDED_SCRIPT_ID>
//    <ASSEMBLY_VERSION>1.0.1.0</ASSEMBLY_VERSION>
//    <ASSEMBLY_NAME>TestFramework.Script</ASSEMBLY_NAME>
//    <ASSEMBLY_SOURCE>Global Assembly Cache</ASSEMBLY_SOURCE>
//    <ASSEMBLY_FULL_NAME>TestFramework.Script, Version=1.0.1.0, Culture=neutral, PublicKeyToken=5fd1a43ef99813ff</ASSEMBLY_FULL_NAME>
//    <IS_DEFAULT>false</IS_DEFAULT>
//    <IS_REMOVABLE>true</IS_REMOVABLE>
//    <IS_HIDDEN>false</IS_HIDDEN>
//  </ASSEMBLY_REFERENCES>
//  <ASSEMBLY_REFERENCES>
//    <REFERENCE_ID>-4</REFERENCE_ID>
//    <EMBEDDED_SCRIPT_ID>-1</EMBEDDED_SCRIPT_ID>
//    <ASSEMBLY_VERSION>1.1.0.42</ASSEMBLY_VERSION>
//    <ASSEMBLY_NAME>TestFramework.Common</ASSEMBLY_NAME>
//    <ASSEMBLY_SOURCE>Global Assembly Cache</ASSEMBLY_SOURCE>
//    <ASSEMBLY_FULL_NAME>TestFramework.Common, Version=1.1.0.42, Culture=neutral, PublicKeyToken=91d36d3ec2aa0933</ASSEMBLY_FULL_NAME>
//    <IS_DEFAULT>false</IS_DEFAULT>
//    <IS_REMOVABLE>true</IS_REMOVABLE>
//    <IS_HIDDEN>false</IS_HIDDEN>
//  </ASSEMBLY_REFERENCES>
//  <ASSEMBLY_REFERENCES>
//    <REFERENCE_ID>-5</REFERENCE_ID>
//    <EMBEDDED_SCRIPT_ID>-1</EMBEDDED_SCRIPT_ID>
//    <ASSEMBLY_VERSION>1.1.0.0</ASSEMBLY_VERSION>
//    <ASSEMBLY_NAME>TestFramework.PluginTestMethod</ASSEMBLY_NAME>
//    <ASSEMBLY_SOURCE>Global Assembly Cache</ASSEMBLY_SOURCE>
//    <ASSEMBLY_FULL_NAME>TestFramework.PluginTestMethod, Version=1.1.0.0, Culture=neutral, PublicKeyToken=9259262fad3c89ad</ASSEMBLY_FULL_NAME>
//    <IS_DEFAULT>false</IS_DEFAULT>
//    <IS_REMOVABLE>true</IS_REMOVABLE>
//    <IS_HIDDEN>false</IS_HIDDEN>
//  </ASSEMBLY_REFERENCES>
//  <ASSEMBLY_REFERENCES>
//    <REFERENCE_ID>-11</REFERENCE_ID>
//    <EMBEDDED_SCRIPT_ID>-1</EMBEDDED_SCRIPT_ID>
//    <ASSEMBLY_VERSION>1.0.0.0</ASSEMBLY_VERSION>
//    <ASSEMBLY_NAME>elowerk.Wrapper.CAN.Peak</ASSEMBLY_NAME>
//    <ASSEMBLY_SOURCE>Global Assembly Cache</ASSEMBLY_SOURCE>
//    <ASSEMBLY_FULL_NAME>elowerk.Wrapper.CAN.Peak, Version=1.0.0.0, Culture=neutral, PublicKeyToken=83f7027c1e6d2334</ASSEMBLY_FULL_NAME>
//    <IS_DEFAULT>false</IS_DEFAULT>
//    <IS_REMOVABLE>true</IS_REMOVABLE>
//    <IS_HIDDEN>false</IS_HIDDEN>
//  </ASSEMBLY_REFERENCES>
//  <ASSEMBLY_REFERENCES>
//    <REFERENCE_ID>-12</REFERENCE_ID>
//    <EMBEDDED_SCRIPT_ID>-1</EMBEDDED_SCRIPT_ID>
//    <ASSEMBLY_VERSION>1.0.0.0</ASSEMBLY_VERSION>
//    <ASSEMBLY_NAME>TestFramework.VirtualTestSets.TestSet_A1</ASSEMBLY_NAME>
//    <ASSEMBLY_SOURCE>Global Assembly Cache</ASSEMBLY_SOURCE>
//    <ASSEMBLY_FULL_NAME>TestFramework.VirtualTestSets.TestSet_A1, Version=1.0.0.0, Culture=neutral, PublicKeyToken=499c14d56e3b0373</ASSEMBLY_FULL_NAME>
//    <IS_DEFAULT>false</IS_DEFAULT>
//    <IS_REMOVABLE>true</IS_REMOVABLE>
//    <IS_HIDDEN>false</IS_HIDDEN>
//  </ASSEMBLY_REFERENCES>
//  <ASSEMBLY_REFERENCES>
//    <REFERENCE_ID>-13</REFERENCE_ID>
//    <EMBEDDED_SCRIPT_ID>-1</EMBEDDED_SCRIPT_ID>
//    <ASSEMBLY_VERSION>4.0.0.0</ASSEMBLY_VERSION>
//    <ASSEMBLY_NAME>System</ASSEMBLY_NAME>
//    <ASSEMBLY_SOURCE>Global Assembly Cache</ASSEMBLY_SOURCE>
//    <ASSEMBLY_FULL_NAME>System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</ASSEMBLY_FULL_NAME>
//    <IS_DEFAULT>false</IS_DEFAULT>
//    <IS_REMOVABLE>true</IS_REMOVABLE>
//    <IS_HIDDEN>false</IS_HIDDEN>
//  </ASSEMBLY_REFERENCES>
//</EmbeddedScriptDataSet>
