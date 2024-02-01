using EOL_GND.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using TestFramework.PluginTestCell;
using TestFramework.PluginTestCell.TestResults;

namespace IntelligentPcbaTester
{
    /* Format:
    Each row contains one result.
    Each column of a row is separated by a tab character.
    "0" in first column means 'this is a comment, the row contains no data'
    "1" in first column means 'this row contains data'
    The next columns are
    "Result"                  -> the test result (PASS, FAIL, UNDEFINED)
    "BoardName"               -> the name of the board in case of a multi panel
    "PartRefDes"              -> the part reference designator the tested part
    "TestStep"                -> the name of the test step
    "NominalValue"            -> the nominal value of the result value in floating point representation
    "ResultValue"             -> the result value in floating point representation
    "ResultValueUnit"         -> the result value's physical unit
    "ResultValueLowerLimit"   -> the result value's lower limit in floating point representation
    "ResultValueUpperLimit"   -> the result value's upper limit in floating point representation
    "ResultValueState"        -> the state of the result value (Good, Bad, BadHigh, BadLow, Invalid)
    "ResultInfo"              -> the result info text
    "LogInfo"                 -> the result log info text defined in the test step
    "Nail"                    -> the list of fixture nail numbers used for stimulation (separated by ';')
    "Net"                     -> the list of net names corresponding to the nail numbers (separated by ';')
    "NailGuard"               -> the list of fixture nail numbers used for guard (separated by ';')
    "NetGuard"                -> the list of net names corresponding to the guard nail numbers (separated by ';')
    "NominalOffset"           -> the offset value added to calculate the result
    "NominalOffsetUnit"       -> the offset value's physical unit
    */
    public static class ElozResultLogFormatter
    {
        public static string HumanReadableFormat(double value, TestFramework.PluginTestCell.TestResults.PhysicalUnit unit, out MetricPrefix prefix)
        {
            var measurementUnit = PhysicalUnitExtensions.From(unit);
            return EolStep.GetPrefixExpression(value, measurementUnit, out prefix);
        }

        public static StringBuilder CreateLog(Project elozProject, TestResult ictResult, List<EOL_GND.Model.EolStep.TestResult> eolResults, bool addHeader,
            StringBuilder headerMessage, List<string> failMessages)
        {
            var log = new StringBuilder();

            // 칼럼 헤더.
            const int resultLength = 6;
            const int boardLength = 5;
            const int totalLength = 5;
            const int typeLength = 20;
            const int partLength = 20;
            const int remarkLength = 40;
            const int actValLength = 12;
            const int stdValLength = 12;
            const int hlLength = 12;
            const int llLength = 12;
            const int testValLength = 12;
            const int devLength = 10;
            const int runTimeLength = 7;

            string fieldSeparator;
            var logMode = AppSettings.LogFileMode;
            if (logMode == LogFileMode.CSV)
            {
                fieldSeparator = ",";
            }
            else
            {
                fieldSeparator = "\t";
            }

            if (addHeader)
            {
                if (logMode != LogFileMode.CSV)
                {
                    log.Append($"{"Result",resultLength}");
                    log.Append($"{fieldSeparator}{"Board",boardLength}");
                    log.Append($"{fieldSeparator}{"Total",totalLength}");
                    log.Append($"{fieldSeparator}{"Type",typeLength}");
                    log.Append($"{fieldSeparator}{"Part",partLength}");
                    log.Append($"{fieldSeparator}{"Remark",remarkLength}");
                    log.Append($"{fieldSeparator}{"ActVal",actValLength}");
                    log.Append($"{fieldSeparator}{"StdVal",stdValLength}");
                    log.Append($"{fieldSeparator}{"HL",hlLength}");
                    log.Append($"{fieldSeparator}{"LL",llLength}");
                    log.Append($"{fieldSeparator}{"TestVal",testValLength}");
                    log.Append($"{fieldSeparator}{"DEV",devLength}");
                    log.Append($"{fieldSeparator}{"RunTime",runTimeLength}");
                    log.Append($"{fieldSeparator}Note");
                    log.AppendLine();
                }
                else
                {
                    log.Append($"Result");
                    log.Append($"{fieldSeparator}Board");
                    log.Append($"{fieldSeparator}Total");
                    log.Append($"{fieldSeparator}Type");
                    log.Append($"{fieldSeparator}Part");
                    log.Append($"{fieldSeparator}Remark");
                    log.Append($"{fieldSeparator}ActVal");
                    log.Append($"{fieldSeparator}StdVal");
                    log.Append($"{fieldSeparator}HL");
                    log.Append($"{fieldSeparator}LL");
                    log.Append($"{fieldSeparator}TestVal");
                    log.Append($"{fieldSeparator}DEV");
                    log.Append($"{fieldSeparator}RunTime");
                    log.Append($"{fieldSeparator}Note");
                    log.AppendLine();
                }
            }

            bool limitAsPercent = AppSettings.ResultValueLimitAsPercent;

            // 로그의 제일 위에 보여지는 추가 정보.
            const string headerStart = "HEADER:";   // eloZ1 스크립트 내에서 추가로 헤더에 정보를 보여주려고 할 때 사용.
            const string failMsgStart = "FAIL_MSG:";
            const string headerSeparator = "|#|";

            // 테스트 결과 추가.
            for (int s = 0; s < ictResult.TestRunResult.Count; s++)
            {
                TestResultRecord resultRecord = ictResult.TestRunResult[s];

                switch (resultRecord)
                {
                    //case BeginProgramTestResultRecord programResultRecord:
                    //    projectName = programResultRecord.ProjectName;
                    //    break;
                    case TestStepResultRecord testStepResultRecord:
                        // a step can return more than one result
                        for (int r = 0; r < testStepResultRecord.Count; r++)
                        {
                            TestStepResultRecord.ItemTestResultRecord itemResult = testStepResultRecord[r];

                            // 헤더 메시지 체크.
                            if (!string.IsNullOrWhiteSpace(itemResult.ResultValueInfo))
                            {
                                var words = itemResult.ResultValueInfo.Split(new string[] { headerSeparator }, StringSplitOptions.None);
                                for (int i = 0; i < words.Length; i++)
                                {
                                    var word = words[i].Trim();
                                    if (word.StartsWith(headerStart, StringComparison.OrdinalIgnoreCase))
                                    {
                                        headerMessage.AppendLine(word.Substring(headerStart.Length));
                                    }
                                    else if (word.StartsWith(failMsgStart, StringComparison.OrdinalIgnoreCase))
                                    {
                                        failMessages.Add(word.Substring(failMsgStart.Length));
                                    }
                                }
                            }

                            if (logMode != LogFileMode.CSV)
                            {
                                // Result.
                                var result = Utils.Truncate(itemResult.ResultState.ToString().ToUpper(), resultLength);
                                log.Append($"{result,resultLength}");

                                // Board.
                                var lastNumber = Regex.Match(itemResult.BoardMultiPanelID, "(\\d+)(?!.*\\d)").Value;
                                var board = Utils.Truncate(lastNumber, boardLength);
                                log.Append($"{fieldSeparator}{board,boardLength}");

                                // Total.
                                var total = Utils.Truncate(itemResult.ResultID.ToString(), totalLength);
                                log.Append($"{fieldSeparator}{total,totalLength}");

                                // Type.
                                var testData = elozProject.GetItem("TestData");
                                var testStep = testData.GetItem("TestStep", itemResult.TestStepRecordID);
                                string templateName = Utils.Truncate(testStep.GetProperty("TemplateName").ToString(), typeLength);
                                log.Append($"{fieldSeparator}{templateName,typeLength}");

                                // Part.
                                //var part = Utils.Truncate(itemResult.PartReferenceDesignator, partLength);
                                string part = "";
                                StringComparison templateComparison = StringComparison.OrdinalIgnoreCase;
                                if (templateName.Equals("Script", templateComparison) ||
                                    templateName.Equals("DischargeTest", templateComparison) ||
                                    templateName.Equals("ContactTest", templateComparison) ||
                                    templateName.Equals("ShortsTest", templateComparison) ||
                                    templateName.Equals("OpensTest", templateComparison))
                                {
                                    part = "";
                                }
                                else
                                {
                                    var delimiters = new string[] { "+", "-", "_", "/", " " };
                                    var splitted = itemResult.TestStepName.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                                    if (splitted?.Length > 0)
                                    {
                                        part = splitted[0].Trim();
                                    }
                                }
                                log.Append($"{fieldSeparator}{part,partLength}");

                                // Remark.
                                var remark = Utils.Truncate(itemResult.TestStepName, remarkLength);
                                log.Append($"{fieldSeparator}{remark,remarkLength}");

                                // ActVal, StdVal, HL, LL.
                                string unit = "";
                                double multiplier = 1;
                                if (itemResult.ResultNominalValue != null)
                                {
                                    HumanReadableFormat(itemResult.ResultNominalValue ?? 0, itemResult.ResultValueUnit, out MetricPrefix prefix);
                                    var measurementUnit = PhysicalUnitExtensions.From(itemResult.ResultValueUnit);
                                    unit = prefix.GetText() + measurementUnit.GetText();
                                    multiplier = prefix.GetMultiplier();
                                }

                                double? stdValue = itemResult.ResultNominalValue;
                                string hl, ll;
                                if (itemResult.ResultNominalValue == null)
                                {
                                    hl = "";
                                    ll = "";
                                }
                                else if (itemResult.ResultValueUpperLimit == null)
                                {
                                    hl = "P";
                                    if (itemResult.ResultValueLowerLimit == null)
                                    {
                                        ll = "P";
                                    }
                                    else
                                    {
                                        stdValue = itemResult.ResultValueLowerLimit;
                                        ll = "1";
                                    }
                                }
                                else if (itemResult.ResultValueLowerLimit == null)
                                {
                                    ll = "P";
                                    hl = "1";
                                    stdValue = itemResult.ResultValueUpperLimit;
                                }
                                else
                                {
                                    double nominalValue = itemResult.ResultNominalValue.GetValueOrDefault();
                                    double upperLimit = itemResult.ResultValueUpperLimit.GetValueOrDefault();
                                    double lowerLimit = itemResult.ResultValueLowerLimit.GetValueOrDefault();
                                    if (nominalValue == 0)
                                    {
                                        // 기준값이 0이면 퍼센트 표시 불가이므로 +/- 절댓값 표시.
                                        if (limitAsPercent)
                                        {
                                            hl = $"{upperLimit / multiplier,hlLength:0.####}{unit}";
                                            ll = $"{-lowerLimit / multiplier,llLength:0.####}{unit}";
                                        }
                                        else
                                        {
                                            hl = $"{upperLimit / multiplier,hlLength:0.####}{unit}";
                                            ll = $"{lowerLimit / multiplier,llLength:0.####}{unit}";
                                        }
                                    }
                                    else
                                    {
                                        if (limitAsPercent)
                                        {
                                            double plusPercent = (upperLimit - nominalValue) / nominalValue * 100.0;
                                            hl = $"{plusPercent:0.#}%";
                                            double minusPercent = (nominalValue - lowerLimit) / nominalValue * 100.0;
                                            ll = $"{minusPercent:0.#}%";
                                        }
                                        else
                                        {
                                            hl = $"{upperLimit / multiplier,hlLength:0.####}{unit}";
                                            ll = $"{lowerLimit / multiplier,llLength:0.####}{unit}";
                                        }
                                    }
                                }
                                string stdValStr = "";
                                if (stdValue != null)
                                {
                                    stdValStr = $"{stdValue / multiplier,stdValLength:0.####}{unit}";
                                }
                                log.Append($"{fieldSeparator}{stdValStr,actValLength}");
                                log.Append($"{fieldSeparator}{stdValStr,stdValLength}");
                                log.Append($"{fieldSeparator}{hl,hlLength}");
                                log.Append($"{fieldSeparator}{ll,llLength}");

                                // TestVal.
                                string testValStr = "";
                                if (itemResult.ResultValue != null)
                                {
                                    testValStr = $"{itemResult.ResultValue / multiplier:0.####}";
                                }
                                log.Append($"{fieldSeparator}{testValStr,testValLength}");

                                // DEV = (TestVal - StdVal) / StdVal * 100.
                                string devStr = "";
                                if (itemResult.ResultValue != null && stdValue != null && stdValue != 0)
                                {
                                    double dev = (itemResult.ResultValue.GetValueOrDefault() - stdValue.GetValueOrDefault()) / stdValue.GetValueOrDefault() * 100;
                                    devStr = $"{dev:0.####}";
                                }
                                log.Append($"{fieldSeparator}{devStr,devLength}");

                                // Run time.
                                int runTimeMilliseconds = (int)(itemResult.RunTime.GetValueOrDefault() * 1000);
                                var runTime = Utils.Truncate(runTimeMilliseconds.ToString(), runTimeLength);
                                log.Append($"{fieldSeparator}{runTime,runTimeLength}");
                            }
                            else
                            {
                                // Result.
                                log.Append(itemResult.ResultState.ToString().ToUpper());

                                // Board.
                                var lastNumber = Regex.Match(itemResult.BoardMultiPanelID, "(\\d+)(?!.*\\d)").Value;
                                log.Append($"{fieldSeparator}{lastNumber}");

                                // Total.
                                log.Append($"{fieldSeparator}{itemResult.ResultID}");

                                // Type.
                                var testData = elozProject.GetItem("TestData");
                                var testStep = testData.GetItem("TestStep", itemResult.TestStepRecordID);
                                string templateName = testStep.GetProperty("TemplateName").ToString();
                                log.Append($"{fieldSeparator}{templateName}");

                                // Part.
                                //var part = Utils.Truncate(itemResult.PartReferenceDesignator, partLength);
                                string part = "";
                                StringComparison templateComparison = StringComparison.OrdinalIgnoreCase;
                                if (templateName.Equals("Script", templateComparison) ||
                                    templateName.Equals("DischargeTest", templateComparison) ||
                                    templateName.Equals("ContactTest", templateComparison) ||
                                    templateName.Equals("ShortsTest", templateComparison) ||
                                    templateName.Equals("OpensTest", templateComparison))
                                {
                                    part = "";
                                }
                                else
                                {
                                    var delimiters = new string[] { "+", "-", "_", "/", " " };
                                    var splitted = itemResult.TestStepName.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                                    if (splitted?.Length > 0)
                                    {
                                        part = splitted[0].Trim();
                                    }
                                }
                                log.Append($"{fieldSeparator}{part}");

                                // Remark.
                                var remark = "\"" + itemResult.TestStepName.Replace("\"", "\"\"") + "\"";
                                log.Append($"{fieldSeparator}{remark}");

                                // ActVal, StdVal, HL, LL.
                                string unit = "";
                                double multiplier = 1;
                                if (itemResult.ResultNominalValue != null)
                                {
                                    HumanReadableFormat(itemResult.ResultNominalValue ?? 0, itemResult.ResultValueUnit, out MetricPrefix prefix);
                                    var measurementUnit = PhysicalUnitExtensions.From(itemResult.ResultValueUnit);
                                    unit = prefix.GetText() + measurementUnit.GetText();
                                    multiplier = prefix.GetMultiplier();
                                }

                                double? stdValue = itemResult.ResultNominalValue;
                                string hl, ll;
                                if (itemResult.ResultNominalValue == null)
                                {
                                    hl = "";
                                    ll = "";
                                }
                                else if (itemResult.ResultValueUpperLimit == null)
                                {
                                    hl = "P";
                                    if (itemResult.ResultValueLowerLimit == null)
                                    {
                                        ll = "P";
                                    }
                                    else
                                    {
                                        stdValue = itemResult.ResultValueLowerLimit;
                                        ll = "1";
                                    }
                                }
                                else if (itemResult.ResultValueLowerLimit == null)
                                {
                                    ll = "P";
                                    hl = "1";
                                    stdValue = itemResult.ResultValueUpperLimit;
                                }
                                else
                                {
                                    double nominalValue = itemResult.ResultNominalValue.GetValueOrDefault();
                                    double upperLimit = itemResult.ResultValueUpperLimit.GetValueOrDefault();
                                    double lowerLimit = itemResult.ResultValueLowerLimit.GetValueOrDefault();
                                    if (nominalValue == 0)
                                    {
                                        // 기준값이 0이면 퍼센트 표시 불가이므로 +/- 절댓값 표시.
                                        if (limitAsPercent)
                                        {
                                            hl = $"{upperLimit / multiplier,hlLength:0.####}{unit}";
                                            ll = $"{-lowerLimit / multiplier,llLength:0.####}{unit}";
                                        }
                                        else
                                        {
                                            hl = $"{upperLimit / multiplier,hlLength:0.####}{unit}";
                                            ll = $"{lowerLimit / multiplier,llLength:0.####}{unit}";
                                        }
                                    }
                                    else
                                    {
                                        if (limitAsPercent)
                                        {
                                            double plusPercent = (upperLimit - nominalValue) / nominalValue * 100.0;
                                            hl = $"{plusPercent:0.#}%";
                                            double minusPercent = (nominalValue - lowerLimit) / nominalValue * 100.0;
                                            ll = $"{minusPercent:0.#}%";
                                        }
                                        else
                                        {
                                            hl = $"{upperLimit / multiplier:0.####}{unit}";
                                            ll = $"{lowerLimit / multiplier:0.####}{unit}";
                                        }
                                    }
                                }
                                string stdValStr = "";
                                if (stdValue != null)
                                {
                                    stdValStr = $"{stdValue / multiplier:0.####}{unit}";
                                }
                                log.Append($"{fieldSeparator}{stdValStr}");
                                log.Append($"{fieldSeparator}{stdValStr}");
                                log.Append($"{fieldSeparator}{hl}");
                                log.Append($"{fieldSeparator}{ll}");

                                // TestVal.
                                string testValStr = "";
                                if (itemResult.ResultValue != null)
                                {
                                    testValStr = $"{itemResult.ResultValue / multiplier:0.####}";
                                }
                                log.Append($"{fieldSeparator}{testValStr}");

                                // DEV = (TestVal - StdVal) / StdVal * 100.
                                string devStr = "";
                                if (itemResult.ResultValue != null && stdValue != null && stdValue != 0)
                                {
                                    double dev = (itemResult.ResultValue.GetValueOrDefault() - stdValue.GetValueOrDefault()) / stdValue.GetValueOrDefault() * 100;
                                    devStr = $"{dev:0.####}";
                                }
                                log.Append($"{fieldSeparator}{devStr}");

                                // Run time.
                                int runTimeMilliseconds = (int)(itemResult.RunTime.GetValueOrDefault() * 1000);
                                log.Append($"{fieldSeparator}{runTimeMilliseconds}");
                            }

                            log.AppendLine();
                        }
                        break;
                }
            }

            // EOL 결과 추가.
            int eolResultCount = eolResults?.Count ?? 0;
            for (int i = 0; i < eolResultCount; i++)
            {
                var eolResult = eolResults[i];

                // 헤더에 표시되는 정보.
                if (eolResult.Step.ResultLogInfo && !string.IsNullOrWhiteSpace(eolResult.ResultLogInfo))
                {
                    headerMessage.AppendLine(eolResult.ResultLogInfo);
                }

                if (eolResult.ResultState != EolStep.ResultState.Pass && !string.IsNullOrWhiteSpace(eolResult.Step.ResultFailLogMessage))
                {
                    failMessages.Add(eolResult.Step.ResultFailLogMessage);
                }

                // 스텝 로그 데이터.
                if (logMode != LogFileMode.CSV)
                {
                    // Result.
                    var result = Utils.Truncate(eolResult.ResultState.ToString().ToUpper(), resultLength);
                    log.Append($"{result,resultLength}");

                    // Board.
                    log.Append($"{fieldSeparator}{"",boardLength}");

                    // Total.
                    var total = Utils.Truncate((i + 1).ToString(), totalLength);
                    log.Append($"{fieldSeparator}{total,totalLength}");

                    // Type - Section.
                    var section = Utils.Truncate(eolResult.Step.Section, typeLength);
                    log.Append($"{fieldSeparator}{section,typeLength}");

                    // Part - Step.
                    var step = Utils.Truncate(eolResult.Step.Name, partLength);
                    log.Append($"{fieldSeparator}{step,partLength}");

                    // Remark.
                    var remark = Utils.Truncate(eolResult.Step.Remarks, remarkLength);
                    log.Append($"{fieldSeparator}{remark,remarkLength}");

                    // ActVal, StdVal, HL, LL.
                    eolResult.Step.GetNominalValues(out double? resultNominalValue, out double? resultValueUpperLimit, out double? resultValueLowerLimit);
                    string unit = "";
                    double multiplier = 1;
                    if (resultNominalValue != null)
                    {
                        EolStep.GetPrefixExpression(resultNominalValue, eolResult.Unit, out MetricPrefix prefix);
                        unit = prefix.GetText() + eolResult.Unit.GetText();
                        multiplier = prefix.GetMultiplier();
                    }

                    double? stdValue = resultNominalValue;
                    string hl, ll;
                    if (resultNominalValue == null)
                    {
                        hl = "";
                        ll = "";
                    }
                    else if (resultValueUpperLimit == null)
                    {
                        hl = "P";
                        if (resultValueLowerLimit == null)
                        {
                            ll = "P";
                        }
                        else
                        {
                            stdValue = resultValueLowerLimit;
                            ll = "1";
                        }
                    }
                    else if (resultValueLowerLimit == null)
                    {
                        ll = "P";
                        hl = "1";
                        stdValue = resultValueUpperLimit;
                    }
                    else
                    {
                        double nominalValue = resultNominalValue.GetValueOrDefault();
                        double upperLimit = resultValueUpperLimit.GetValueOrDefault();
                        double lowerLimit = resultValueLowerLimit.GetValueOrDefault();
                        if (nominalValue == 0)
                        {
                            // 기준값이 0이면 퍼센트 표시 불가이므로 +/- 절댓값 표시.
                            if (limitAsPercent)
                            {
                                hl = $"{upperLimit / multiplier,hlLength:0.####}{unit}";
                                ll = $"{-lowerLimit / multiplier,llLength:0.####}{unit}";
                            }
                            else
                            {
                                hl = $"{upperLimit / multiplier,hlLength:0.####}{unit}";
                                ll = $"{lowerLimit / multiplier,llLength:0.####}{unit}";
                            }
                        }
                        else
                        {
                            if (limitAsPercent)
                            {
                                double plusPercent = (upperLimit - nominalValue) / nominalValue * 100.0;
                                hl = $"{plusPercent:0.#}%";
                                double minusPercent = (nominalValue - lowerLimit) / nominalValue * 100.0;
                                ll = $"{minusPercent:0.#}%";
                            }
                            else
                            {
                                hl = $"{upperLimit / multiplier,hlLength:0.####}{unit}";
                                ll = $"{lowerLimit / multiplier,llLength:0.####}{unit}";
                            }
                        }
                    }
                    string stdValStr = "";
                    if (stdValue != null)
                    {
                        stdValStr = $"{stdValue / multiplier,stdValLength:0.####}{unit}";
                    }
                    log.Append($"{fieldSeparator}{stdValStr,actValLength}");
                    log.Append($"{fieldSeparator}{stdValStr,stdValLength}");
                    log.Append($"{fieldSeparator}{hl,hlLength}");
                    log.Append($"{fieldSeparator}{ll,llLength}");

                    // TestVal.
                    string testValStr = "";
                    if (eolResult.ResultValue != null)
                    {
                        testValStr = $"{eolResult.ResultValue / multiplier:0.####}";
                    }
                    log.Append($"{fieldSeparator}{testValStr,testValLength}");

                    // DEV = (TestVal - StdVal) / StdVal * 100.
                    string devStr = "";
                    if (eolResult.ResultValue != null && stdValue != null && stdValue != 0)
                    {
                        double dev = (eolResult.ResultValue.GetValueOrDefault() - stdValue.GetValueOrDefault()) / stdValue.GetValueOrDefault() * 100;
                        devStr = $"{dev:0.####}";
                    }
                    log.Append($"{fieldSeparator}{devStr,devLength}");

                    // Run time.
                    var runTime = Utils.Truncate(eolResult.TotalMilliseconds.ToString(), runTimeLength);
                    log.Append($"{fieldSeparator}{runTime,runTimeLength}");
                }
                else
                {
                    // Result.
                    log.Append(eolResult.ResultState.ToString().ToUpper());

                    // Board.
                    log.Append($"{fieldSeparator}");

                    // Total.
                    log.Append($"{fieldSeparator}{i + 1}");

                    // Type - Section.
                    log.Append($"{fieldSeparator}{eolResult.Step.Section}");

                    // Part - Step.
                    log.Append($"{fieldSeparator}{eolResult.Step.Name}");

                    // Remark.
                    log.Append($"{fieldSeparator}{eolResult.Step.Remarks}");

                    // ActVal, StdVal, HL, LL.
                    eolResult.Step.GetNominalValues(out double? resultNominalValue, out double? resultValueUpperLimit, out double? resultValueLowerLimit);
                    string unit = "";
                    double multiplier = 1;
                    if (resultNominalValue != null)
                    {
                        EolStep.GetPrefixExpression(resultNominalValue, eolResult.Unit, out MetricPrefix prefix);
                        unit = prefix.GetText() + eolResult.Unit.GetText();
                        multiplier = prefix.GetMultiplier();
                    }

                    double? stdValue = resultNominalValue;
                    string hl, ll;
                    if (resultNominalValue == null)
                    {
                        hl = "";
                        ll = "";
                    }
                    else if (resultValueUpperLimit == null)
                    {
                        hl = "P";
                        if (resultValueLowerLimit == null)
                        {
                            ll = "P";
                        }
                        else
                        {
                            stdValue = resultValueLowerLimit;
                            ll = "1";
                        }
                    }
                    else if (resultValueLowerLimit == null)
                    {
                        ll = "P";
                        hl = "1";
                        stdValue = resultValueUpperLimit;
                    }
                    else
                    {
                        double nominalValue = resultNominalValue.GetValueOrDefault();
                        double upperLimit = resultValueUpperLimit.GetValueOrDefault();
                        double lowerLimit = resultValueLowerLimit.GetValueOrDefault();
                        if (nominalValue == 0)
                        {
                            // 기준값이 0이면 퍼센트 표시 불가이므로 +/- 절댓값 표시.
                            if (limitAsPercent)
                            {
                                hl = $"{upperLimit / multiplier,hlLength:0.####}{unit}";
                                ll = $"{-lowerLimit / multiplier,llLength:0.####}{unit}";
                            }
                            else
                            {
                                hl = $"{upperLimit / multiplier,hlLength:0.####}{unit}";
                                ll = $"{lowerLimit / multiplier,llLength:0.####}{unit}";
                            }
                        }
                        else
                        {
                            if (limitAsPercent)
                            {
                                double plusPercent = (upperLimit - nominalValue) / nominalValue * 100.0;
                                hl = $"{plusPercent:0.#}%";
                                double minusPercent = (nominalValue - lowerLimit) / nominalValue * 100.0;
                                ll = $"{minusPercent:0.#}%";
                            }
                            else
                            {
                                hl = $"{upperLimit / multiplier:0.####}{unit}";
                                ll = $"{lowerLimit / multiplier:0.####}{unit}";
                            }
                        }
                    }
                    string stdValStr = "";
                    if (stdValue != null)
                    {
                        stdValStr = $"{stdValue / multiplier:0.####}{unit}";
                    }
                    log.Append($"{fieldSeparator}{stdValStr}");
                    log.Append($"{fieldSeparator}{stdValStr}");
                    log.Append($"{fieldSeparator}{hl}");
                    log.Append($"{fieldSeparator}{ll}");

                    // TestVal.
                    string testValStr = "";
                    if (eolResult.ResultValue != null)
                    {
                        testValStr = $"{eolResult.ResultValue / multiplier:0.####}";
                    }
                    log.Append($"{fieldSeparator}{testValStr}");

                    // DEV = (TestVal - StdVal) / StdVal * 100.
                    string devStr = "";
                    if (eolResult.ResultValue != null && stdValue != null && stdValue != 0)
                    {
                        double dev = (eolResult.ResultValue.GetValueOrDefault() - stdValue.GetValueOrDefault()) / stdValue.GetValueOrDefault() * 100;
                        devStr = $"{dev:0.####}";
                    }
                    log.Append($"{fieldSeparator}{devStr}");

                    // Run time.
                    log.Append($"{fieldSeparator}{eolResult.TotalMilliseconds}");
                }

                log.AppendLine($"{fieldSeparator}{eolResult.ResultInfo}");

                // 다음 라인에 덧붙여지는 추가 정보.
                if (!string.IsNullOrWhiteSpace(eolResult.ResultLogBody))
                {
                    log.AppendLine(eolResult.ResultLogBody);
                }
            }

            return log;
        }

        /// <summary>
        /// EOL 사양서 비교를 위한 로그를 생성한다.
        /// </summary>
        /// <param name="addHeader">CSV 헤더 생성 여부.</param>
        /// <param name="ictResult">EOL 실행결과.</param>
        /// <param name="resultCount">테스트 번호. 이 번호를 증가시키며 테스트 스텝 결과 추가.</param>
        /// <param name="totalMilliseconds">테스트 총 시간. 이 시간을 증가시키며 테스트 스텝 결과 추가.</param>
        /// <returns></returns>
        public static StringBuilder CreateEOLSpecLog(bool addHeader, TestResult ictResult, ref int resultCount, ref long totalMilliseconds)
        {
            return EOL_GND.ViewModel.SequenceViewModel.CreateEOLSpecLog(addHeader, ictResult, ref resultCount, ref totalMilliseconds);
        }

        /// <summary>
        /// EOL 사양서 비교를 위한 로그를 생성한다.
        /// </summary>
        /// <param name="addHeader">CSV 헤더 생성 여부.</param>
        /// <param name="results">EOL 실행결과.</param>
        /// <param name="resultCount">테스트 번호. 이 번호를 증가시키며 테스트 스텝 결과 추가.</param>
        /// <param name="totalMilliseconds">테스트 총 시간. 이 시간을 증가시키며 테스트 스텝 결과 추가.</param>
        /// <returns></returns>
        public static StringBuilder CreateEOLSpecLog(bool addHeader, IEnumerable<EolStep.TestResult> results, ref int resultCount, ref long totalMilliseconds)
        {
            return EOL_GND.ViewModel.SequenceViewModel.CreateEOLSpecLog(addHeader, results, ref resultCount, ref totalMilliseconds);
        }

        public static string CreateDetailLog(TestResult testResult)
        {
            var log = new StringBuilder();

            log.AppendLine("TotalResult");
            if (testResult != null)
            {
                ResultState total_result = testResult.TotalTestState;
                log.AppendLine(total_result.ToString().ToUpper());

                log.AppendLine("ResultLog");
                string line = NewCommentLine();
                AppendField(ref line, "Result");
                AppendField(ref line, "BoardName");
                AppendField(ref line, "PartRefDes");
                AppendField(ref line, "TestStep");
                AppendField(ref line, "NominalValue");
                AppendField(ref line, "ResultValue");
                AppendField(ref line, "ResultValueUnit");
                AppendField(ref line, "ResultValueLowerLimit");
                AppendField(ref line, "ResultValueUpperLimit");
                AppendField(ref line, "ResultValueState");
                AppendField(ref line, "ResultInfo");
                AppendField(ref line, "LogInfo");
                //AppendField(ref line, "Nail");
                //AppendField(ref line, "Net");
                //AppendField(ref line, "NailGuard");
                //AppendField(ref line, "NetGuard");
                //AppendField(ref line, "NominalOffset");
                //AppendField(ref line, "NominalOffsetUnit");
                log.AppendLine(line);

                for (int s = 0; s < testResult.TestRunResult.Count; s++)
                {
                    TestResultRecord stepResult = testResult.TestRunResult[s];

                    // Debugging.
                    //log.AppendLine($"Record {s}: " + stepResult.ToString());

                    switch (stepResult)
                    {
                        case TestStepResultRecord testStepResultRecord:
                            // a step can return more than one result
                            for (int r = 0; r < testStepResultRecord.Count; r++)
                            {
                                TestStepResultRecord.ItemTestResultRecord itemResult = testStepResultRecord[r];
                                line = NewDataLine();
                                // Result
                                AppendField(ref line, itemResult.ResultState.ToString().ToUpper());
                                // BoardName
                                AppendField(ref line, itemResult.BoardMultiPanelID);
                                // PartRefDes
                                AppendField(ref line, itemResult.PartReferenceDesignator);
                                // TestStep
                                AppendField(ref line, itemResult.TestStepName);
                                // NominalValue
                                AppendField(ref line, itemResult.ResultNominalValue?.ToString());
                                // ResultValue
                                AppendField(ref line, itemResult.ResultValue?.ToString());
                                // ResultValueUnit
                                AppendField(ref line, itemResult.ResultValueUnit.ToString());
                                // ResultValueLowerLimit
                                AppendField(ref line, itemResult.ResultValueLowerLimit?.ToString());
                                // ResultValueUpperLimit
                                AppendField(ref line, itemResult.ResultValueUpperLimit?.ToString());
                                // ResultValueState
                                AppendField(ref line, itemResult.ResultValueState.ToString());
                                // ResultInfo
                                AppendField(ref line, itemResult.ResultInfo);
                                // LogInfo
                                AppendField(ref line, itemResult.TestStepResultLogInfo);

                                // Nail, Net, NailGuard, NetGuard
                                //AppendNailNetFields(ref line, test_data, fixture_data, result);

                                // NominalOffset, NominalOffsetUnit
                                //AppendOffset(ref line, test_data, result, unit); // offset unit must be the same as for the result

                                log.AppendLine(line);
                            }
                            break;
                    }
                }
            }
            else
            {
                log.AppendLine("NOTEST");
            }

            return log.ToString();
        }

        //private void AppendNailNetFields(ref string line, IDataItem test_data, IDataItem fixture_data, IDataItem result)
        //{
        //    int[] stim_nail_list = null;
        //    string[] stim_net_list = null;
        //    int[] guard_nail_list = null;
        //    string[] guard_net_list = null;


        //    if (!(bool)result.GetProperty("FixtureNail", "#IsNull"))
        //    {
        //        // ContactTest
        //        stim_nail_list = new int[1];
        //        stim_nail_list[0] = (int)result.GetProperty("FixtureNail");
        //        stim_net_list = GetNetList(fixture_data, stim_nail_list);
        //    }
        //    else if (!(bool)result.GetProperty("NodeA", "#IsNull") || !(bool)result.GetProperty("NodeB", "#IsNull"))
        //    {
        //        // ShortsTest
        //        int count = 0;
        //        if (!(bool)result.GetProperty("NodeA", "#IsNull"))
        //            count++;
        //        if (!(bool)result.GetProperty("NodeB", "#IsNull"))
        //            count++;
        //        stim_net_list = new string[count];
        //        stim_nail_list = new int[count];
        //        count = 0;
        //        if (!(bool)result.GetProperty("NodeA", "#IsNull"))
        //        {
        //            SplitShortsNode((string)result.GetProperty("NodeA"), out stim_net_list[count], out stim_nail_list[count]);
        //            count++;
        //        }
        //        if (!(bool)result.GetProperty("NodeB", "#IsNull"))
        //        {
        //            SplitShortsNode((string)result.GetProperty("NodeB"), out stim_net_list[count], out stim_nail_list[count]);
        //            count++;
        //        }
        //    }
        //    else
        //    {
        //        // other test
        //        stim_nail_list = GetNailList(test_data, result, "Stimulus");
        //        stim_net_list = GetNetList(fixture_data, stim_nail_list);
        //        guard_nail_list = GetNailList(test_data, result, "Guard");
        //        guard_net_list = GetNetList(fixture_data, guard_nail_list);
        //    }

        //    AppendField(ref line, ListToString(stim_nail_list));
        //    AppendField(ref line, ListToString(stim_net_list));
        //    AppendField(ref line, ListToString(guard_nail_list));
        //    AppendField(ref line, ListToString(guard_net_list));
        //}

        //private void AppendOffset(ref string line, IDataItem test_data, IDataItem result, string unit_field)
        //{
        //    string field = "";
        //    IDataItem test_step;
        //    IDataItem properties;

        //    if (!(bool)result.GetProperty("TestStepRecordID", "#IsNull"))
        //    {
        //        test_step = test_data.GetItem("TestStep", result.GetProperty("TestStepRecordID"));
        //        properties = test_step.GetItem("Properties");
        //        switch (unit_field)
        //        {
        //            case "Ohm": field = GetDoubleField(properties, "ResistanceOffset"); break;
        //            case "F": field = GetDoubleField(properties, "CapacitanceOffset"); break;
        //            case "H": field = GetDoubleField(properties, "InductanceOffset"); break;
        //        }
        //    }
        //    AppendField(ref line, field);
        //    if (string.IsNullOrEmpty(field))
        //        AppendField(ref line, "");
        //    else
        //        AppendField(ref line, unit_field);
        //}

        //private void SplitShortsNode(string node, out string net_name, out int nail_number)
        //{
        //    // shorts test log:  net name '{<' nail_number '>}'
        //    // example:          Net123{<14>}
        //    int start_index;
        //    int end_index;
        //    start_index = node.IndexOf("{<");
        //    if (start_index > 0)
        //        net_name = node.Substring(0, start_index);
        //    else
        //        net_name = "";
        //    end_index = node.LastIndexOf(">}");
        //    if (end_index > start_index)
        //    {
        //        try
        //        {
        //            nail_number = System.Convert.ToInt32(node.Substring(start_index + 2, end_index - (start_index + 2)));
        //        }
        //        catch
        //        {
        //            nail_number = 0;
        //        }
        //    }
        //    else
        //        nail_number = 0;
        //}

        //private int[] GetNailList(IDataItem test_data, IDataItem result, string nail_selection)
        //{
        //    int[] nail_list = null;
        //    IDataItem test_step;

        //    if (!(bool)result.GetProperty("TestStepRecordID", "#IsNull"))
        //    {
        //        test_step = test_data.GetItem("TestStep", result.GetProperty("TestStepRecordID"));
        //        nail_list = test_step.GetItem("TestChannels").GetProperty(nail_selection) as int[];
        //    }
        //    return nail_list;
        //}

        //private string[] GetNetList(IDataItem fixture_data, int[] nail_list)
        //{
        //    string[] net_list = null;

        //    if (nail_list != null)
        //    {
        //        net_list = new string[nail_list.Length];
        //        for (int i = 0; i < nail_list.Length; i++)
        //            net_list[i] = fixture_data.GetItem("Nail", nail_list[i]).GetProperty("NetName").ToString();
        //    }
        //    return net_list;
        //}

        //private string ListToString(int[] list)
        //{
        //    string str = "";
        //    if (list != null)
        //    {
        //        for (int i = 0; i < list.Length; i++)
        //        {
        //            if (i > 0)
        //                str += ";";
        //            str += list[i].ToString();
        //        }
        //    }
        //    return str;
        //}

        //private string ListToString(string[] list)
        //{
        //    string str = "";
        //    if (list != null)
        //    {
        //        for (int i = 0; i < list.Length; i++)
        //        {
        //            if (i > 0)
        //                str += ";";
        //            str += list[i].ToString();
        //        }
        //    }
        //    return str;
        //}

        private static void AppendField(ref string line, string field)
        {
            line += "\t" + field;
        }

        private static string NewDataLine()
        {
            return "1";
        }

        private static string NewCommentLine()
        {
            return "0";
        }

        public static void CreateKerfix(Project elozProject, TestResult testResult)
        {
            // =============================================================================
            string result_folder = @"C:\Temp\Results\KERFIX";    // <--- output folder for log files
            string client_name = "elowerk";
            // =============================================================================

            ResultState total_result = ResultState.NoState;
            int counter = 0;
            System.DateTime EndTime = System.DateTime.Now;

            var log = new System.Text.StringBuilder();
            var total_log = new System.Text.StringBuilder();
            var test_data = elozProject.GetItem("TestData");
            var fixture_data = elozProject.GetItem("FixtureData");
//            var test_cell = args.Data.GetItem("TestCell");

            if (testResult != null)
            {
                total_result = testResult.TotalTestState;

                for (int s = 0; s < testResult.TestRunResult.Count; s++)
                {
                    if (testResult.TestRunResult[s] is TestStepResultRecord stepRecord)
                    {
                        var step_item = stepRecord;
                        var step_result_state = step_item.ResultState;
                        int result_count = step_item.Count;
                        // a step can return more than one result
                        for (int r = 0; r < result_count; r++)
                        {
                            string TestType;
                            string TestMessage = "";
                            string SubTestDesignator;
                            bool is_pass;

                            var result = step_item[r];
                            var result_state = result.ResultState;
                            var board_multi_panel_id = result.BoardMultiPanelID;

                            switch (result_state)
                            {
                                case ResultState.Pass: is_pass = true; break;
                                case ResultState.Fail: is_pass = false; break;
                                default: is_pass = false; TestMessage += "Aborted "; break;
                            }

                            var ResultInfo = result.ResultInfo;
                            var ResultValueInfo = result.ResultValueInfo;
                            var ResultValueState = result.ResultValueState.ToString();
                            var LogInfo = result.TestStepResultLogInfo;

                            if (ResultInfo.ToLower() == "short" || ResultInfo.ToLower().StartsWith("short "))
                                TestType = "SHORTS";
                            else if (ResultInfo.ToLower() == "open" || ResultInfo.ToLower().StartsWith("open "))
                                TestType = "OPEN";
                            else
                                TestType = "COMP";

                            if (result_count > 1)
                            {
                                SubTestDesignator = (r + 1).ToString();
                                if (!string.IsNullOrEmpty(ResultInfo))
                                    SubTestDesignator += ":" + ResultInfo;
                            }
                            else
                                SubTestDesignator = result.TestStepName;

                            TestMessage += ResultInfo;
                            if (!string.IsNullOrEmpty(ResultValueInfo))
                            {
                                if (!string.IsNullOrEmpty(TestMessage))
                                    TestMessage += " - ";
                                TestMessage += ResultValueInfo;
                            }
                            if (!string.IsNullOrEmpty(ResultValueState))
                            {
                                if (!string.IsNullOrEmpty(TestMessage))
                                    TestMessage += " - ";
                                TestMessage += ResultValueState;
                            }
                            if (!string.IsNullOrEmpty(LogInfo))
                            {
                                if (!string.IsNullOrEmpty(TestMessage))
                                    TestMessage += " - ";
                                TestMessage += LogInfo;
                            }

                            // === Fill Nail and Net ===
                            int? FixtureNail = null;
                            string NodeName = "";
                            string NetA = "";
                            string NetB = "";
                            string Nail;
                            List<int> NailList = new List<int>();

                            Nail = "";
                            NailList.Clear();

                            // read nail numbers from TestStepData
                            var test_step_data = test_data.GetItem("TestStep", result.TestStepRecordID);
                            if (test_step_data != null)
                                AppendNails(test_step_data, ref NailList);

                            // append nail numbers from result record
                            FixtureNail = result.FixtureNailNumber;
                            if (FixtureNail.HasValue && !NailList.Contains(FixtureNail.Value))
                                NailList.Add(FixtureNail.Value);

                            // append nail numbers corresponding to net names in result record
                            NodeName = result.NodeAName;
                            SplitNodeName(NodeName, out NetA, out FixtureNail);
                            if (FixtureNail.HasValue && !NailList.Contains(FixtureNail.Value))
                                NailList.Add(FixtureNail.Value);

                            NodeName = result.NodeBName;
                            SplitNodeName(NodeName, out NetB, out FixtureNail);
                            if (FixtureNail.HasValue && !NailList.Contains(FixtureNail.Value))
                                NailList.Add(FixtureNail.Value);

                            // fill Nail out of NailList
                            for (int n = 0; n < NailList.Count && n < 6; n++)
                            {
                                if (!string.IsNullOrEmpty(Nail))
                                    Nail += "-";
                                Nail += NailList[n].ToString();
                            }

                            counter++;
                            AppendField(ref log, "EVENT_DATE", EndTime.ToString("MM") + "/" + EndTime.ToString("dd") + "/" + EndTime.ToString("yyyy"), 0);
                            AppendField(ref log, "EVENT_TIME", EndTime.ToString("HH:mm:ss") + ":" + counter.ToString("000"), 0);
                            AppendField(ref log, "CLIENT_NAME", client_name, 0);
                            AppendField(ref log, "BOARD_NAME", result.BoardTypeName, 20);
                            AppendField(ref log, "BOARD_REVISION", "CAD", 50);
                            AppendField(ref log, "BOARD_OPTION", "", 10);
                            //AppendField(ref log, "SERIAL_NUMBER", test_cell.GetProperty("BoardID").ToString(), 20);
                            AppendField(ref log, "TESTER_NAME", "eloZ1", 20);
                            AppendField(ref log, "TESTER_TYPE", "FIXTURE", 20);
                            AppendField(ref log, "EVENT_TYPE", "ELO", 20);
                            AppendField(ref log, "TEST_PROGRAM_NAME", test_data.GetProperty("ProjectName").ToString(), 50);
                            log.AppendLine("");

                            if (is_pass)
                                log.AppendLine("PASS_START");
                            else
                                log.AppendLine("FAIL_START");
                            AppendField(ref log, "TEST_TYPE", TestType, 20);
                            AppendField(ref log, "TEST_DESIGNATOR", result.TestStepName, 0);
                            AppendField(ref log, "SUBTEST_DESIGNATOR", SubTestDesignator, 0);
                            AppendField(ref log, "REFERENCE_DESIGNATOR", result.PartReferenceDesignator, 0);
                            AppendField(ref log, "UNIT", GetUnitOf(result.ResultValueUnit.ToString()), 0);
                            AppendField(ref log, "NOMINAL_VALUE", ConvertDoubleToString(result.ResultNominalValue), 10);
                            AppendField(ref log, "MIN_VALUE", ConvertDoubleToString(result.ResultValueLowerLimit), 10);
                            AppendField(ref log, "MAX_VALUE", ConvertDoubleToString(result.ResultValueUpperLimit), 10);
                            AppendField(ref log, "MEASURED_VALUE", ConvertDoubleToString(result.ResultValue), 10);
                            AppendField(ref log, "TESTER_CHANNEL", Nail, 0);
                            AppendField(ref log, "TEST_SIGNAL1", NetA, 50);
                            AppendField(ref log, "TEST_SIGNAL2", NetB, 50);
                            AppendField(ref log, "TEST_PIN", "", 20);
                            AppendField(ref log, "TEST_MESSAGE", TestMessage, 100);
                            AppendField(ref log, "VISION", "", 50);
                            if (is_pass)
                                log.AppendLine("PASS_END");
                            else
                                log.AppendLine("FAIL_END");
                            log.AppendLine("");

                            total_log.Append(log);
                            Save(result_folder, EndTime, counter, log); // Writes the log to file
                            log.Length = 0;
                        }
                    }
                }
            }

            //args.OutputText = total_log.ToString(); // Returns the log
        }

        private static void AppendField(ref System.Text.StringBuilder Log, string FieldName, string FieldValue, int Width)
        {
            string Line = FieldName + " = ";
            if (!string.IsNullOrEmpty(FieldValue))
            {
                if (Width > 0 && FieldValue.Length > Width)
                    Line += FieldValue.Substring(0, Width);
                else
                    Line += FieldValue;
            }
            Log.AppendLine(Line);
        }

        private static string GetUnitOf(string unit)
        {
            switch (unit)
            {
                case "Watt": return "W";
                case "Volt": return "V";
                case "Ampere": return "A";
                case "Ohm": return "O";
                case "Farad": return "F";
                case "Henry": return "H";
                case "Second": return "s";
                default: return "";
            }
        }

        private static void SplitNodeName(string NodeName, out string NetName, out int? NailNumber)
        {
            int Index1;
            int Index2;
            string Num;

            Index1 = NodeName.LastIndexOf("{<");
            Index2 = NodeName.LastIndexOf(">}");
            if (Index1 >= 0 && Index2 > Index1)
            {
                NetName = NodeName.Substring(0, Index1);
                NailNumber = null;
                Index1 += 2;
                // skip "{<"
                try
                {
                    Num = NodeName.Substring(Index1, Index2 - Index1);
                    NailNumber = System.Convert.ToInt32(Num);
                }
                catch { }
            }
            else
            {
                NetName = NodeName;
                NailNumber = null;
            }
        }

        private static void AppendNails(Project.Item TestStepData, ref List<int> NailList)
        {
            int[] NailNumbers = null;
            Project.Item TestChannels;
            if (TestStepData != null)
            {
                TestChannels = TestStepData.GetItem("TestChannels");
                if (TestChannels != null)
                    NailNumbers = TestChannels.GetProperty("Stimulus") as int[];
                if (NailNumbers != null)
                {
                    foreach (int nail in NailNumbers)
                    {
                        if (!NailList.Contains(nail))
                            NailList.Add(nail);
                    }
                }
            }
        }

        private static void Save(string ResultFolder, System.DateTime EndTime, int counter, System.Text.StringBuilder Log)
        {
            string TargetPath;
            string FileName;

            TargetPath = ResultFolder;
            if (!System.IO.Directory.Exists(TargetPath))
                System.IO.Directory.CreateDirectory(TargetPath);

            // "EVENT"+year+month+day+hour+minute+second+1/100 second
            FileName = GetUniqueFilename(TargetPath, "Event" + EndTime.ToString("yyyyMMddHHmmss") + counter.ToString("000"), "evi");
            System.IO.File.AppendAllText(FileName, Log.ToString(), System.Text.Encoding.ASCII);
        }

        private static string GetUniqueFilename(string DirectoryPath, string DefaultName, string Extension)
        {
            string Filename;
            int Count = 0;
            if (string.IsNullOrEmpty(DefaultName))
                DefaultName = System.Guid.NewGuid().ToString("N");
            Filename = System.IO.Path.Combine(DirectoryPath, DefaultName + "." + Extension);
            while (System.IO.File.Exists(Filename))
            {
                Count++;
                Filename = System.IO.Path.Combine(DirectoryPath, DefaultName + "_(" + Count.ToString() + ")" + "." + Extension);
            }
            return Filename;
        }

        private static string ConvertDoubleToString(double? Value)
        {
            if (Value.HasValue)
            {
                double tmp = Value.Value;
                return tmp.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
            }
            return "";
        }
    }
}
