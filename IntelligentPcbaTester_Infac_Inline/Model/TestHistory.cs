using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentPcbaTester
{
    /// <summary>
    /// 테스트 이력을 나타내는 Entity 클래스.
    /// </summary>
    public class TestHistory
    {
        /// <summary>
        /// 유일 식별자.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 테스트한 프로젝트 이름.
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// 테스트한 픽스처 ID.
        /// </summary>
        public int FixtureId { get; set; }

        /// <summary>
        /// 테스트한 ICT 프로젝트 이름.
        /// </summary>
        public string IctProject { get; set; }

        /// <summary>
        /// 테스트한 모델.
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// 테스트한 보드의 시리얼 넘버.
        /// </summary>
        public string SerialNumber { get; set; }

        /// <summary>
        /// 테스트 결과. true => Pass, false => Fail.
        /// </summary>
        public bool Result { get; set; }

        public string ResultText => Result ? "PASS" : "FAIL";

        /// <summary>
        /// 테스트 로그 파일 경로.
        /// </summary>
        public string Board1LogFile { get; set; }
        public string Board2LogFile { get; set; }
        public string Board3LogFile { get; set; }
        public string Board4LogFile { get; set; }

        /// <summary>
        /// 테스트 중 화면에 출력한 로그를 저장한 파일 경로.
        /// </summary>
        public string PrintLogFile { get; set; }

        /// <summary>
        /// 테스트 시작 시간.
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 테스트 종료 시간.
        /// </summary>
        public DateTime FinishTime { get; set; }

        /// <summary>
        /// 테스트에 걸린 전체 시간. 단위는 초.
        /// </summary>
        public int TestDuration => (int)(FinishTime - StartTime).TotalSeconds;

        /// <summary>
        /// eloZ 테스트에 걸린 시간. 단위는 초.
        /// </summary>
        public int ElozDuration { get; set; }

        /// <summary>
        /// 프레스 업다운 등 간접시간 포함 테스트시간.
        /// </summary>
        public int TotalDuration { get; set; }

        /// <summary>
        /// Group ID.
        /// </summary>
        public int HistoryGroupId { get; set; }

        /// <summary>
        /// Group.
        /// </summary>
        public virtual HistoryGroup HistoryGroup { get; set; }

        public DateTime GroupDate => HistoryGroup.StartTime;

        /// <summary>
        /// 화면 표시를 위해 사용됨.
        /// </summary>
        [NotMapped]
        public int RowIndex { get; set; }
    }
}
