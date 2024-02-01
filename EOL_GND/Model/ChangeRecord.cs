using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace EOL_GND.Model
{
    /// <summary>
    /// 텍스트 파일의 변경 정보 하나를 표현한다.
    /// </summary>
    public class ChangeRecord
    {
        /// <summary>
        /// 파일 변경 시간.
        /// </summary>
        [XmlIgnore]
        public DateTime ModificationTime { get; set; }

        /// <summary>
        /// <see cref="ModificationTime"/>의 XML serialization을 위한 프로퍼티.
        /// </summary>
        [XmlElement(nameof(ModificationTime))]
        public long ModificationTimeLong
        {
            get => ModificationTime.Ticks;
            set => ModificationTime = new DateTime(value);
        }

        /// <summary>
        /// 파일을 변경한 프로그램 사용자 이름.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 파일을 변경한 프로그램 사용자 권한.
        /// </summary>
        public string UserRole { get; set; }

        /// <summary>
        /// 사용자 정보.
        /// </summary>
        [XmlIgnore]
        public string UserInfo
        {
            get
            {
                string info = "";
                if (!string.IsNullOrWhiteSpace(UserName))
                {
                    info = UserName;
                }

                if (!string.IsNullOrWhiteSpace(UserRole))
                {
                    info += $"({UserRole})";
                }

                return info;
            }
        }

        /// <summary>
        /// 파일을 변경한 프로그램 버전.
        /// </summary>
        public string EditorVersion { get; set; }

        /// <summary>
        /// 추가 설명.
        /// </summary>
        public string Remarks { get; set; }

        /// <summary>
        /// 파일 변경 내용. GNU diff/patch 포맷과 매우 유사.
        /// </summary>
        public string PatchText { get; set; }
    }
}
