using EOL_GND.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestFramework.VirtualTestSets.TestSet_A1;
using TestFramework.VirtualTestSets.TestSet_A1.Devices;
using TestFramework.VirtualTestSets.TestSet_A1.Devices.Templates;

namespace EOL_GND.Device
{
    public class ElozDevice
    {
        public static readonly ElozDevice SharedInstance = new ElozDevice();

        // Requested bus lines.
        private readonly List<Matrix.BusLine> matrixBusLines = new List<Matrix.BusLine>();

        private ElozDevice()
        {
        }

        /// <summary>
        /// eloZ1 Relay Matrix를 이용, 지정한 채널들을 연결한다.
        /// </summary>
        /// <param name="elozTestSet"></param>
        /// <param name="busLines"></param>
        public void ConnectChannels(object elozTestSet, params IEnumerable<int>[] busLines)
        {
            //token.Register(() => testSet.Dispose());

            var testSet = elozTestSet as TestSet;
            if (testSet == null)
            {
                throw new Exception("eloZ1 채널 연결 오류: eloZ1 디바이스가 연결되지 않았습니다.");
            }

            bool connected = false;
            foreach (var channels in busLines)
            {
                if (channels != null && channels.Count() > 0)
                {
                    // 먼저 해당 채널이 연결되어 있는가 검사한다.
                    Matrix.BusLine busLine = null;
                    var newChannels = new List<int>();
                    foreach (var channel in channels)
                    {
                        try
                        {
                            // 채널이 연결되어 있지 않으면 예외를 발생한다.
                            var connection = testSet.Matrix.GetTestChannelConnection(channel);

                            if (connection?.BusConnection?.OddBusLine != null)
                            {
                                busLine = connection.BusConnection.OddBusLine;
                            }
                            else if (connection?.BusConnection?.EvenBusLine != null)
                            {
                                busLine = connection.BusConnection.EvenBusLine;
                            }
                            else
                            {
                                newChannels.Add(channel);
                            }
                        }
                        catch
                        {
                            // 채널 연결 안됨.
                            newChannels.Add(channel);
                        }
                    }

                    if (newChannels.Count > 0)
                    {
                        if (busLine == null)
                        {
                            busLine = testSet.Matrix.RequestBusLine();
                            matrixBusLines.Add(busLine);
                        }

                        testSet.Matrix.SetTestChannelConnection(newChannels.ToArray(), busLine);
                        connected = true;
                    }
                }
            }

            if (connected)
            {
                testSet.Matrix.CommitConnections();

                // Let all settings take effect.
                testSet.Apply();
            }
        }

        /// <summary>
        /// eloZ1 Relay Matrix 전체 채널 연결을 해제한다.
        /// </summary>
        /// <param name="elozTestSet"></param>
        public void RelayOff(object elozTestSet)
        {
            // Clear all matrix connection
            var testSet = elozTestSet as TestSet;
            if (testSet == null)
            {
                throw new Exception("eloZ1 채널 연결 클리어 오류: eloZ1 디바이스가 연결되지 않았습니다.");
            }

            // Release bus lines.
            foreach (var busLine in matrixBusLines)
            {
                try
                {
                    busLine?.Release();
                }
                catch (Exception ex)
                {
                    Logger.LogWarning($"Releasing bus line {busLine?.Number} error: {ex.Message}");
                }
            }
            matrixBusLines.Clear();

            // Clear all connections.
            try
            {
                //testSet.Matrix.ClearConnections();
                testSet.ClearConnections();
            }
            catch (Exception ex)
            {
                Logger.LogError($"eloZ1 ClearConnections Error: {ex.Message}");

                // 한번 더 클리어.
                try
                {
                    //testSet.Matrix.ClearConnections();
                    testSet.ClearConnections();
                }
                catch (Exception exex)
                {
                    Logger.LogError($"eloZ1 ClearConnections Error: {exex.Message}");
                }
            }

            // Let all settings take effect.
            testSet.Commit();
            testSet.Apply();
        }

        /// <summary>
        /// eloZ1 Relay Matrix의 지정한 채널들의 연결을 해제한다.
        /// </summary>
        /// <param name="elozTestSet"></param>
        /// <param name="channels"></param>
        public void DisconnectChannels(object elozTestSet, IEnumerable<int> channels)
        {
            var testSet = elozTestSet as TestSet;
            if (testSet == null)
            {
                throw new Exception("eloZ1 채널 연결 해제 오류: eloZ1 디바이스가 연결되지 않았습니다.");
            }

            testSet.Matrix.ClearTestChannelConnection(channels.ToArray());
        }

        /// <summary>
        /// eloZ1 Relay Matrix 전체 채널 연결 정보를 리턴한다.
        /// </summary>
        /// <param name="elozTestSet"></param>
        /// <returns></returns>
        public string GetConnectionInfo(object elozTestSet)
        {
            var testSet = elozTestSet as TestSet;
            if (testSet == null)
            {
                throw new Exception("eloZ1 채널 연결 전체 정보 오류: eloZ1 디바이스가 연결되지 않았습니다.");
            }

            return testSet.GetConnectionInfo();
        }

        /// <summary>
        /// eloZ1 Relay Matrix 지정한 채널의 연결 정보를 리턴한다.
        /// </summary>
        /// <param name="elozTestSet"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public string GetChannelConnectionInfo(object elozTestSet, int channel)
        {
            var testSet = elozTestSet as TestSet;
            if (testSet == null)
            {
                throw new Exception("eloZ1 채널 연결 정보 오류: eloZ1 디바이스가 연결되지 않았습니다.");
            }

            try
            {
                var connection = testSet.Matrix.GetTestChannelConnection(channel);
                Matrix.BusLine busLine;
                if (connection.BusConnection.OddBusLine != null)
                {
                    busLine = connection.BusConnection.OddBusLine;
                }
                else
                {
                    busLine = connection.BusConnection.EvenBusLine;
                }

                if (busLine != null)
                {
                    return $"TestChannel {channel} connected: Bus={busLine.Number}";
                }
            }
            catch
            {
                // Not connected.
            }

            return $"TestChannel {channel} not connected";
        }

        /// <summary>
        /// eloZ1 디바이스의 Stimulus 유닛을 이용, 전원 인가.
        /// </summary>
        /// <param name="elozTestSet"></param>
        /// <param name="voltage"></param>
        /// <param name="current"></param>
        /// <param name="highForceChannels"></param>
        /// <param name="lowForceChannels"></param>
        /// <exception cref="Exception"></exception>
        public void Stimulate(int deviceNumber, bool is10VUnit, object elozTestSet, double voltage, double current, int[] highForceChannels, int[] lowForceChannels)
        {
            var testSet = elozTestSet as TestSet;
            if (testSet == null)
            {
                throw new Exception("eloZ1 Stimulus 오류: eloZ1 디바이스가 연결되지 않았습니다.");
            }

            // Setup and connect stimulus.
            int deviceIndex = Math.Max(deviceNumber - 1, 0);
            StimulusUnit stimulusUnit;
            if (is10VUnit)
            {
                stimulusUnit = testSet.Stimulus10V[deviceIndex];
            }
            else
            {
                stimulusUnit = testSet.Stimulus60V[deviceIndex];
            }
            stimulusUnit.SetMatrixConnection(highForceChannels ?? new int[0], lowForceChannels ?? new int[0]);
            stimulusUnit.CommitConnections();
            stimulusUnit.SetStimulationCurrent(current);
            stimulusUnit.SetStimulationVoltage(voltage);
            stimulusUnit.SetPermanentStimulationEnabled();
            stimulusUnit.Commit();

            Logger.LogInfo($"eloZ1 Stimulus {stimulusUnit.Name} settings: {testSet.GetSettingsInfo()}");
            Logger.LogInfo($"eloZ1 Stimulus {stimulusUnit.Name} connection: {testSet.GetConnectionInfo()}");

            // Apply all settings.
            testSet.Apply();
        }

        /// <summary>
        /// eloZ1 디바이스의 Stimulus 유닛 전원 인가 해제.
        /// </summary>
        /// <param name="elozTestSet"></param>
        /// <param name="dischargeDelay"></param>
        /// <exception cref="Exception"></exception>
        public void Unstimulate(int deviceNumber, bool is10VUnit, object elozTestSet, double dischargeDelay)
        {
            var testSet = elozTestSet as TestSet;
            if (testSet == null)
            {
                throw new Exception("eloZ1 Stimulus 오류: eloZ1 디바이스가 연결되지 않았습니다.");
            }

            // Reset stimulus.
            int deviceIndex = Math.Max(deviceNumber - 1, 0);
            StimulusUnit stimulusUnit;
            if (is10VUnit)
            {
                stimulusUnit = testSet.Stimulus10V[deviceIndex];
            }
            else
            {
                stimulusUnit = testSet.Stimulus60V[deviceIndex];
            }
            stimulusUnit.SetPermanentStimulationDisabled();
            stimulusUnit.SetStimulationCurrent(0);
            stimulusUnit.SetStimulationVoltage(0);
            stimulusUnit.Commit();

            // Discharge.
            if (dischargeDelay > 0)
            {
                testSet.Timer.PutDelay(dischargeDelay);
            }

            stimulusUnit.ClearConnections();
            stimulusUnit.Commit();

            // Disconnect.
            //testSet.ClearConnections();
            //testSet.Commit();

            // Apply all settings.
            testSet.Apply();
        }

        /// <summary>
        /// 모든 eloZ1 Stimulus 디바이스의 Stimulation 제거.
        /// </summary>
        /// <param name="elozTestSet"></param>
        /// <param name="dischargeDelay"></param>
        /// <exception cref="Exception"></exception>
        public void UnstimulateAll(object elozTestSet, double dischargeDelay)
        {
            var testSet = elozTestSet as TestSet;
            if (testSet == null)
            {
                throw new Exception("eloZ1 Stimulus 오류: eloZ1 디바이스가 연결되지 않았습니다.");
            }

            var totalLength = (testSet.Stimulus10V?.Length ?? 0) + (testSet.Stimulus60V?.Length ?? 0);
            if (totalLength > 0)
            {
                var units = new StimulusUnit[totalLength];
                testSet.Stimulus10V?.CopyTo(units, 0);
                testSet.Stimulus60V?.CopyTo(units, testSet.Stimulus10V?.Length ?? 0);
                foreach (var stimulusUnit in units)
                {
                    Logger.LogInfo($"Unstimulate {stimulusUnit.Name}");

                    stimulusUnit.SetPermanentStimulationDisabled();
                    stimulusUnit.SetStimulationCurrent(0);
                    stimulusUnit.SetStimulationVoltage(0);
                    stimulusUnit.Commit();

                    // Discharge.
                    if (dischargeDelay > 0)
                    {
                        testSet.Timer.PutDelay(dischargeDelay);
                    }

                    stimulusUnit.ClearConnections();
                    stimulusUnit.Commit();
                }

                testSet.Apply();
            }
        }

        /// <summary>
        /// eloZ1 디바이스의 Stimulus 유닛 최대 인가 전압/전류 를 리턴한다.
        /// </summary>
        /// <param name="is10VUnit"></param>
        /// <param name="elozTestSet"></param>
        /// <param name="isVoltage"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public double GetMaxStimulation(int deviceNumber, bool is10VUnit, object elozTestSet, bool isVoltage)
        {
            var testSet = elozTestSet as TestSet;
            if (testSet == null)
            {
                throw new Exception("eloZ1 Stimulus 오류: eloZ1 디바이스가 연결되지 않았습니다.");
            }

            // Setup and connect stimulus.
            int deviceIndex = Math.Max(deviceNumber - 1, 0);
            StimulusUnit stimulusUnit;
            if (is10VUnit)
            {
                stimulusUnit = testSet.Stimulus10V[deviceIndex];
            }
            else
            {
                stimulusUnit = testSet.Stimulus60V[deviceIndex];
            }

            if (isVoltage)
            {
                return stimulusUnit.MaxStimulationVoltage;
            }
            else
            {
                return stimulusUnit.MaxStimulationCurrent;
            }
        }

        /// <summary>
        /// eloZ1 Stimulus 유닛의 연결 정보 리턴.
        /// </summary>
        /// <param name="deviceNumber"></param>
        /// <param name="is10VUnit"></param>
        /// <param name="elozTestSet"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public string GetStimulusUnitConnectionInfo(int deviceNumber, bool is10VUnit, object elozTestSet)
        {
            var testSet = elozTestSet as TestSet;
            if (testSet == null)
            {
                throw new Exception("eloZ1 Stimulus 오류: eloZ1 디바이스가 연결되지 않았습니다.");
            }

            // Setup and connect stimulus.
            int deviceIndex = Math.Max(deviceNumber - 1, 0);
            StimulusUnit stimulusUnit;
            if (is10VUnit)
            {
                stimulusUnit = testSet.Stimulus10V[deviceIndex];
            }
            else
            {
                stimulusUnit = testSet.Stimulus60V[deviceIndex];
            }

            return stimulusUnit.GetConnectionInfo();
        }

        /// <summary>
        /// eloZ1 Stimulus 유닛의 설정 정보 리턴.
        /// </summary>
        /// <param name="deviceNumber"></param>
        /// <param name="is10VUnit"></param>
        /// <param name="elozTestSet"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public string GetStimulusUnitSettingsInfo(int deviceNumber, bool is10VUnit, object elozTestSet)
        {
            var testSet = elozTestSet as TestSet;
            if (testSet == null)
            {
                throw new Exception("eloZ1 Stimulus 오류: eloZ1 디바이스가 연결되지 않았습니다.");
            }

            // Setup and connect stimulus.
            int deviceIndex = Math.Max(deviceNumber - 1, 0);
            StimulusUnit stimulusUnit;
            if (is10VUnit)
            {
                stimulusUnit = testSet.Stimulus10V[deviceIndex];
            }
            else
            {
                stimulusUnit = testSet.Stimulus60V[deviceIndex];
            }

            return stimulusUnit.GetSettingsInfo();
        }

        /// <summary>
        /// eloZ1 Stimulus 유닛 인가 전압 리턴.
        /// </summary>
        /// <param name="deviceNumber"></param>
        /// <param name="is10VUnit"></param>
        /// <param name="elozTestSet"></param>
        /// <param name="isVoltage"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public double GetStimulation(int deviceNumber, bool is10VUnit, object elozTestSet, bool isVoltage)
        {
            var testSet = elozTestSet as TestSet;
            if (testSet == null)
            {
                throw new Exception("eloZ1 Stimulus 오류: eloZ1 디바이스가 연결되지 않았습니다.");
            }

            // Setup and connect stimulus.
            int deviceIndex = Math.Max(deviceNumber - 1, 0);
            StimulusUnit stimulusUnit;
            if (is10VUnit)
            {
                stimulusUnit = testSet.Stimulus10V[deviceIndex];
            }
            else
            {
                stimulusUnit = testSet.Stimulus60V[deviceIndex];
            }

            if (isVoltage)
            {
                return stimulusUnit.GetStimulationVoltage();
            }
            else
            {
                return stimulusUnit.GetStimulationCurrent();
            }
        }

        /// <summary>
        /// eloZ1 Stimulus 유닛 측정범위(최대전압 또는 최대전류)를 리턴한다.
        /// </summary>
        /// <param name="deviceNumber"></param>
        /// <param name="is10VUnit"></param>
        /// <param name="elozTestSet"></param>
        /// <param name="isVoltage"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public double GetStimulusUnitMeasurementRange(int deviceNumber, bool is10VUnit, object elozTestSet, bool isVoltage)
        {
            var testSet = elozTestSet as TestSet;
            if (testSet == null)
            {
                throw new Exception("eloZ1 Stimulus 오류: eloZ1 디바이스가 연결되지 않았습니다.");
            }

            // Setup and connect stimulus.
            int deviceIndex = Math.Max(deviceNumber - 1, 0);
            StimulusUnit stimulusUnit;
            if (is10VUnit)
            {
                stimulusUnit = testSet.Stimulus10V[deviceIndex];
            }
            else
            {
                stimulusUnit = testSet.Stimulus60V[deviceIndex];
            }

            if (isVoltage)
            {
                return stimulusUnit.GetMeasurementVoltageRange();
            }
            else
            {
                return stimulusUnit.GetMeasurementCurrentRange();
            }
        }

        /// <summary>
        /// eloZ1 Stimulus 유닛 측정범위(최대전압 또는 최대전류)를 설정한다.
        /// </summary>
        /// <param name="deviceNumber"></param>
        /// <param name="is10VUnit"></param>
        /// <param name="elozTestSet"></param>
        /// <param name="isVoltage"></param>
        /// <param name="value"></param>
        /// <exception cref="Exception"></exception>
        public void SetStimulusUnitMeasurementRange(int deviceNumber, bool is10VUnit, object elozTestSet, bool isVoltage, double value)
        {
            var testSet = elozTestSet as TestSet;
            if (testSet == null)
            {
                throw new Exception("eloZ1 Stimulus 오류: eloZ1 디바이스가 연결되지 않았습니다.");
            }

            // Setup and connect stimulus.
            int deviceIndex = Math.Max(deviceNumber - 1, 0);
            StimulusUnit stimulusUnit;
            if (is10VUnit)
            {
                stimulusUnit = testSet.Stimulus10V[deviceIndex];
            }
            else
            {
                stimulusUnit = testSet.Stimulus60V[deviceIndex];
            }

            if (isVoltage)
            {
                stimulusUnit.SetMeasurementVoltageRange(value);
            }
            else
            {
                stimulusUnit.SetMeasurementCurrentRange(value);
            }
        }

        /// <summary>
        /// eloZ1 Stimulus 유닛을 이용해 전압과 전류를 측정한다.
        /// </summary>
        /// <param name="deviceNumber"></param>
        /// <param name="is10VUnit"></param>
        /// <param name="elozTestSet"></param>
        /// <param name="voltage"></param>
        /// <param name="current"></param>
        /// <param name="highForceChannels"></param>
        /// <param name="lowForceChannels"></param>
        /// <param name="highSenseChannels"></param>
        /// <param name="lowSenseChannels"></param>
        /// <exception cref="Exception"></exception>
        public void StimulusUnitMeasure(int deviceNumber, bool is10VUnit, object elozTestSet, out double voltage, out double current, 
            int[] highForceChannels, int[] lowForceChannels, int[] highSenseChannels, int[] lowSenseChannels)
        {
            var testSet = elozTestSet as TestSet;
            if (testSet == null)
            {
                throw new Exception("eloZ1 Stimulus 오류: eloZ1 디바이스가 연결되지 않았습니다.");
            }

            // Setup and connect stimulus.
            int deviceIndex = Math.Max(deviceNumber - 1, 0);
            StimulusUnit stimulusUnit;
            if (is10VUnit)
            {
                stimulusUnit = testSet.Stimulus10V[deviceIndex];
            }
            else
            {
                stimulusUnit = testSet.Stimulus60V[deviceIndex];
            }

            // 채널 연결.
            stimulusUnit.SetMatrixConnection(highForceChannels ?? new int[0], highSenseChannels ?? new int[0], lowForceChannels ?? new int[0], lowSenseChannels ?? new int[0]);
            stimulusUnit.Commit();

            Logger.LogInfo($"eloZ1 Stimulus Measure {stimulusUnit.Name} settings: {testSet.GetSettingsInfo()}");
            Logger.LogInfo($"eloZ1 Stimulus Measure {stimulusUnit.Name} connection: {testSet.GetConnectionInfo()}");

            // Apply all settings.
            testSet.Apply();

            // 측정.
            stimulusUnit.Measure(out current, out voltage);

            // 연결 해제.
            stimulusUnit.ClearConnections();
            stimulusUnit.Commit();
            testSet.Apply();
        }

        /// <summary>
        /// eloZ1 MeasurementUnit의 연결 정보를 리턴한다.
        /// </summary>
        /// <param name="deviceNumber"></param>
        /// <param name="elozTestSet"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public string GetMeasurementUnitConnectionInfo(int deviceNumber, object elozTestSet)
        {
            var testSet = elozTestSet as TestSet;
            if (testSet == null)
            {
                throw new Exception("eloZ1 Measurement 오류: eloZ1 디바이스가 연결되지 않았습니다.");
            }

            int deviceIndex = Math.Max(deviceNumber - 1, 0);
            return testSet.MeasurementUnit[deviceIndex].GetConnectionInfo();
        }

        /// <summary>
        /// eloZ1 MeasurementUnit의 설정 정보를 리턴한다.
        /// </summary>
        /// <param name="deviceNumber"></param>
        /// <param name="elozTestSet"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public string GetMeasurementUnitSettingsInfo(int deviceNumber, object elozTestSet)
        {
            var testSet = elozTestSet as TestSet;
            if (testSet == null)
            {
                throw new Exception("eloZ1 Measurement 오류: eloZ1 디바이스가 연결되지 않았습니다.");
            }

            int deviceIndex = Math.Max(deviceNumber - 1, 0);
            return testSet.MeasurementUnit[deviceIndex].GetSettingsInfo();
        }

        /// <summary>
        /// eloZ1 MeasurementUnit을 Reset한다(모든 설정을 default로 하고, 모든 연결을 해제한다).
        /// </summary>
        /// <param name="deviceNumber"></param>
        /// <param name="elozTestSet"></param>
        /// <exception cref="Exception"></exception>
        public void ResetMeasurementUnit(int deviceNumber, object elozTestSet)
        {
            var testSet = elozTestSet as TestSet;
            if (testSet == null)
            {
                throw new Exception("eloZ1 Measurement 오류: eloZ1 디바이스가 연결되지 않았습니다.");
            }

            int deviceIndex = Math.Max(deviceNumber - 1, 0);
            testSet.MeasurementUnit[deviceIndex].Reset();
            testSet.Apply();
        }

        /// <summary>
        /// eloZ1 MeasurementUnit의 Voltmeter 모듈 측정범위(최대전압)를 설정한다.
        /// </summary>
        /// <param name="deviceNumber"></param>
        /// <param name="elozTestSet"></param>
        /// <param name="maxVoltage"></param>
        /// <exception cref="Exception"></exception>
        public void SetVoltmeterMeasuringRange(int deviceNumber, object elozTestSet, double maxVoltage)
        {
            var testSet = elozTestSet as TestSet;
            if (testSet == null)
            {
                throw new Exception("eloZ1 Measurement 오류: eloZ1 디바이스가 연결되지 않았습니다.");
            }

            int deviceIndex = Math.Max(deviceNumber - 1, 0);
            var measUnit = testSet.MeasurementUnit[deviceIndex];
            measUnit.Voltmeter.SetMeasuringRange(maxVoltage);
            measUnit.Commit();
            testSet.Apply();
        }

        /// <summary>
        /// eloZ1 MeasurementUnit의 Voltmeter 모듈 측정범위(최대전압)를 리턴한다.
        /// </summary>
        /// <param name="deviceNumber"></param>
        /// <param name="elozTestSet"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public double GetVoltmeterMeasuringRange(int deviceNumber, object elozTestSet)
        {
            var testSet = elozTestSet as TestSet;
            if (testSet == null)
            {
                throw new Exception("eloZ1 Measurement 오류: eloZ1 디바이스가 연결되지 않았습니다.");
            }

            int deviceIndex = Math.Max(deviceNumber - 1, 0);
            return testSet.MeasurementUnit[deviceIndex].Voltmeter.GetMeasuringRange();
        }

        /// <summary>
        /// eloZ1 MeasurementUnit의 Voltmeter 모듈 설정정보를 리턴한다.
        /// </summary>
        /// <param name="deviceNumber"></param>
        /// <param name="elozTestSet"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public string GetVoltmeterSettingsInfo(int deviceNumber, object elozTestSet)
        {
            var testSet = elozTestSet as TestSet;
            if (testSet == null)
            {
                throw new Exception("eloZ1 Measurement 오류: eloZ1 디바이스가 연결되지 않았습니다.");
            }

            int deviceIndex = Math.Max(deviceNumber - 1, 0);
            return testSet.MeasurementUnit[deviceIndex].Voltmeter.GetSettingsInfo();
        }

        /// <summary>
        /// eloZ1 MeasurementUnit의 Voltmeter 모듈을 이용해 전압을 측정한다.
        /// </summary>
        /// <param name="deviceNumber"></param>
        /// <param name="elozTestSet"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public double VoltmeterMeasure(int deviceNumber, object elozTestSet, int[] inputHighChannels, int[] inputLowChannels)
        {
            var testSet = elozTestSet as TestSet;
            if (testSet == null)
            {
                throw new Exception("eloZ1 Measurement 오류: eloZ1 디바이스가 연결되지 않았습니다.");
            }

            int deviceIndex = Math.Max(deviceNumber - 1, 0);
            var measUnit = testSet.MeasurementUnit[deviceIndex];

            measUnit.SetMatrixConnection(inputHighChannels, inputLowChannels);
            measUnit.Commit();
            testSet.Apply();

            measUnit.Voltmeter.Measure(out double measured);
            return measured;
        }
    }
}
