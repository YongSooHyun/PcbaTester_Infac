using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace EOL_GND.Model
{
    /// <summary>
    /// 테스트 스텝 종류에 대한 정보를 나타낸다.
    /// </summary>
    public class StepCategoryInfo
    {
        /// <summary>
        /// 테스트 스텝 종류.
        /// </summary>
        public StepCategory Category { get; set; } = StepCategory.Power;

        /// <summary>
        /// 스텝의 종류를 표현하는 이미지 파일 이름.
        /// </summary>
        [XmlIgnore]
        public string ImageName => Category.GetImageName();

        /// <summary>
        /// 스텝의 종류 이름.
        /// </summary>
        [XmlIgnore]
        public string CategoryName => Category.GetText();

        internal int StartChannel { get; set; }
        internal int EndChannel { get; set; }
        public string ChannelDesc => $"{StartChannel} ~ {EndChannel}";

        /// <summary>
        /// 시스템에서 지원하는 스텝 종류들을 리턴한다.
        /// </summary>
        /// <returns></returns>
        public static StepCategoryInfo[] GetCategoryInfos()
        {
            var categories = new[] {
                StepCategory.Power,
                StepCategory.DMM,
                StepCategory.Oscilloscope,
                StepCategory.WaveformGenerator,
                StepCategory.Amplifier,
                StepCategory.DIO,
                StepCategory.CAN,
                StepCategory.LIN,
                StepCategory.ElozRelay,
                StepCategory.ElozStimulus,
                StepCategory.ElozVoltmeter,
                StepCategory.AbortOnFail,
                StepCategory.Dummy,
#if GQT
                StepCategory.GloquadSECC,
#endif
                StepCategory.SerialPort,
#if MOTION
                StepCategory.AlphaMotion,
                StepCategory.MightyZap,
#endif
            };

            //var categories = Enum.GetValues(typeof(StepCategory)) as StepCategory[];

            var infos = new StepCategoryInfo[categories.Length];
            for (int i = 0; i < categories.Length; i++)
            {
                infos[i] = new StepCategoryInfo(categories[i]);
            }

            return infos;
        }

        private StepCategoryInfo()
        {
        }

        public StepCategoryInfo(StepCategory category)
        {
            Category = category;
        }
    }
}
