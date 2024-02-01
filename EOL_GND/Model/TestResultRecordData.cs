using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EOL_GND.Model
{
    /// <summary>
    /// 테스트 스텝 실행 결과. eloZ1 테스트 스텝과 EOL 테스트 스텝의 실행 결과를 나타낸다.
    /// </summary>
    public class TestResultRecordData
    {
        /// <summary>
        /// 레코드 발생 순서.
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// Board ID.
        /// </summary>
        public string BoardMultiPanelID { get; set; }

        /// <summary>
        /// EOL 스텝 NO.<br/>
        /// eloZ1 스텝 : EOL-{S|T}-&lt;StepNo&gt; / &lt;SectionName&gt; / &lt;StepName&gt;<br/>
        /// EOL 스텝 : EOL-T-&lt;StepNo&gt; / &lt;StartNumber&gt;-&lt;EndNumber&gt; / &lt;EOLFileName&gt;
        /// </summary>
        public int StepNo { get; }

        /// <summary>
        /// EOL 섹션 이름.
        /// </summary>
        public string SectionName { get; }

        /// <summary>
        /// EOL 스텝 이름.
        /// </summary>
        public string StepName { get; }

        /// <summary>
        /// 측정값.
        /// </summary>
        public double? MeasuredValue { get; }

        /// <summary>
        /// 측정값 + 단위.
        /// </summary>
        public string MeasuredValueDesc
        {
            get
            {
                if (MeasuredValue != null)
                {
                    var valueText = EolStep.GetPrefixExpression(MeasuredValue, MeasuredUnit, out MetricPrefix prefix);
                    return valueText + prefix.GetText() + MeasuredUnit.GetText();
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 측정값 단위.
        /// </summary>
        public PhysicalUnit MeasuredUnit { get; } = PhysicalUnit.None;

        /// <summary>
        /// 결과 정보.
        /// </summary>
        public string ResultInfo { get; }

        /// <summary>
        /// 결과 상태.
        /// </summary>
        public EolStep.ResultState ResultState { get; }

        public string ResultStateDesc => ResultState.GetText();

        /// <summary>
        /// 결과값 상태.
        /// </summary>
        public EolStep.ResultValueState ResultValueState { get; }

        /// <summary>
        /// 걸린 시간.
        /// </summary>
        public long ElapsedMilliseconds { get; set; }

        //
        // 로그를 위한 추가적인 데이터.
        //

        public int? ResultID { get; set; }
        public int? TestStepRecordID { get; set; }
        public string TestStepName { get; set; }
        public double? ResultNominalValue { get; set; }
        public double? ResultValueUpperLimit { get; set; }
        public double? ResultValueLowerLimit { get; set; }
        public object ResultData { get; set; }

        public TestResultRecordData(int index, EolStep.TestResult result)
        {
            Index = index;
            StepNo = result.Step.StepId;
            SectionName = result.Step.Section;
            StepName = result.Step.Name;
            MeasuredValue = result.ResultValue;
            MeasuredUnit = result.Unit;
            ResultInfo = result.ResultInfo;
            ResultState = result.ResultState;
            ResultValueState = result.ResultValueState;
            ElapsedMilliseconds = result.TotalMilliseconds;

            TestStepName = result.Step.Name;
            result.Step.GetNominalValues(out double? nominalValue, out double? upperLimit, out double? lowerLimit);
            ResultNominalValue = nominalValue;
            ResultValueUpperLimit = upperLimit;
            ResultValueLowerLimit = lowerLimit;
            ResultData = result.ResultData;
        }

        public TestResultRecordData(int index, TestFramework.PluginTestCell.TestResults.TestStepResultRecord.ItemTestResultRecord record)
        {
            Index = index;
            BoardMultiPanelID = record.BoardMultiPanelID;

            // 스텝 이름을 파싱하여 StepNo, SectionName, StepName 생성.
            var words = record.TestStepName.Split('/');
            if (words.Length >= 3)
            {
                // StepNo.
                var stepNoWords = words[0].Split('-');
                if (stepNoWords.Length >= 3 && int.TryParse(stepNoWords[2], out int parsed))
                {
                    StepNo = parsed;
                }

                // SectionName.
                SectionName = words[1].Trim();

                // StepName.
                StepName = words[2].Trim();
            }
            else
            {
                StepName = record.TestStepName;
            }

            MeasuredValue = record.ResultValue;
            MeasuredUnit = PhysicalUnitExtensions.From(record.ResultValueUnit);
            ResultInfo = record.ResultInfo;
            ResultState = ResultStateExtensions.From(record.ResultState);
            ResultValueState = ResultValueStateExtensions.From(record.ResultValueState);
            ElapsedMilliseconds = (int)(record.RunTime ?? 0) * 1000;

            ResultID = record.ResultID;
            TestStepRecordID = record.TestStepRecordID;
            TestStepName = record.TestStepName;
            ResultNominalValue = record.ResultNominalValue;
            ResultValueUpperLimit = record.ResultValueUpperLimit;
            ResultValueLowerLimit = record.ResultValueLowerLimit;
        }
    }
}
