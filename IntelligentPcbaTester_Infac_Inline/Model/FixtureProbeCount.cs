using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace IntelligentPcbaTester
{
    /// <summary>
    /// Fixture Probe Count 를 저장한다.
    /// </summary>
    public class FixtureProbeCount
    {
        /// <summary>
        /// Fixture ID.
        /// </summary>
        public int FixtureId { get; set; }

        /// <summary>
        /// 최대 검사 횟수.
        /// </summary>
        internal int MaxProbeCount { get; set; }

        /// <summary>
        /// 총 프로브 수.
        /// </summary>
        public int TotalProbeCount { get; set; }

        /// <summary>
        /// 오늘 테스트 수.
        /// </summary>
        internal int TodayTestCount
        {
            get => _todayProbeCount;
            set
            {
                _todayProbeCount = value;
            }
        }
        public int _todayProbeCount { get; set; }

        /// <summary>
        /// 오늘 양품 수.
        /// </summary>
        internal int TodayPassCount
        {
            get => _todayPassCount;
            set
            {
                _todayPassCount = value;
            }
        }
        public int _todayPassCount { get; set; }

        /// <summary>
        /// 오늘 불량 수.
        /// </summary>
        internal int TodayFailCount => TodayTestCount - TodayPassCount;

        /// <summary>
        /// 오늘 날짜 표시. 오늘 테스트/합격/불량 수 관리에 이용된다.
        /// </summary>
        internal DateTime Today
        {
            get
            {
                if (DateTime.TryParseExact(TodayString, DateTimeFormat, null, System.Globalization.DateTimeStyles.None, out var parsed))
                {
                    return parsed;
                }
                else
                {
                    var now = DateTime.Now;
                    TodayString = now.ToString(DateTimeFormat, null);
                    return now;
                }
            }
            set
            {
                TodayString = value.ToString(DateTimeFormat, null);
            }
        }

        /// <summary>
        /// <see cref="DateTime"/>이 Serialize되지 않으므로, 이를 보완하기 위해 사용된다.
        /// </summary>
        public string TodayString { get; set; }

        // DateTime을 문자열로 변환 및 파싱에 사용하는 형식 문자열.
        private const string DateTimeFormat = "yyyy-MM-dd";

        /// <summary>
        /// 테스트 결과 정보.
        /// </summary>
        public class TestResult
        {
            /// <summary>
            /// 테스트한 보드의 바코드.
            /// </summary>
            public string Barcode { get; set; }

            /// <summary>
            /// 마지막 테스트 결과.
            /// </summary>
            public bool Passed { get; set; }

            /// <summary>
            /// 테스트 결과가 최종인지 여부.
            /// </summary>
            public bool Final { get; set; }
        }

        /// <summary>
        /// Barcode에 따라 검사수량을 관리하기 위한 Barcode 리스트.
        /// </summary>
        public List<TestResult> TestedResults { get; set; }

        /// <summary>
        /// 바코드 비교로 같은 보드의 중복검사는 검사횟수를 증가시키지 않는다.
        /// </summary>
        /// <param name="barcode"></param>
        /// <param name="passed"></param>
        public void IncreaseTodayTestCount(string barcode, bool passed, int count)
        {
            // 바코드가 없으면 무조건 증가.
            if (string.IsNullOrEmpty(barcode))
            {
                TodayTestCount += count;
                if (passed)
                {
                    TodayPassCount += count;
                }
                return;
            }

            // 바코드가 이미 있는가 검사.
            if (TestedResults != null)
            {
                foreach (var existingResult in TestedResults)
                {
                    if (barcode.Equals(existingResult.Barcode, StringComparison.OrdinalIgnoreCase))
                    {
                        // 이전의 검사결과와 새 검사결과를 비교한다.
                        if (!existingResult.Final && existingResult.Passed != passed)
                        {
                            TodayTestCount += count;
                            if (passed)
                            {
                                TodayPassCount += count;
                            }
                            existingResult.Final = true;
                            existingResult.Passed = passed;
                        }
                        return;
                    }
                }
            }
            else
            {
                TestedResults = new List<TestResult>();
            }

            // 새로운 바코드인 경우.
            TestedResults.Add(new TestResult { Barcode = barcode, Passed = passed, Final = false });
            TodayTestCount += count;
            if (passed)
            {
                TodayPassCount += count;
            }
        }

        /// <summary>
        /// 오늘 테스트 결과를 초기화한다.
        /// </summary>
        public void ResetTodayTestCount()
        {
            TodayTestCount = 0;
            TodayPassCount = 0;
            TestedResults?.Clear();
        }

        /// <summary>
        /// 양품비율을 계산한다.
        /// </summary>
        /// <param name="total"></param>
        /// <param name="passed"></param>
        /// <returns></returns>
        public static float CalcYieldPercent(int total, int passed)
        {
            if (total == 0)
            {
                return 0;
            }

            return (float)passed * 100 / total;
        }
    }
}
