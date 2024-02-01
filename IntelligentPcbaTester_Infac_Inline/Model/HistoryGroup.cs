using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentPcbaTester
{
    public class HistoryGroup
    {
        /// <summary>
        /// 유일식별 ID.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 그룹 시작 시간.
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 전체 테스트 수.
        /// </summary>
        public int TestCount { get; set; }

        /// <summary>
        /// Pass한 수.
        /// </summary>
        public int PassCount { get; set; }

        /// <summary>
        /// Fail한 수.
        /// </summary>
        public int FailCount => TestCount - PassCount;
    }
}
