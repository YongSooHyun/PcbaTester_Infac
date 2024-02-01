using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace EOL_GND.Model
{
    /// <summary>
    /// EOL 테스트 시퀀스.
    /// </summary>
    public class EolSequence : ICloneable
    {
        /// <summary>
        /// 시퀀스 파일 설명.
        /// </summary>
        internal const string FileDescription = "EOL Sequence Files";

        /// <summary>
        /// 시퀀스 파일 확장자.
        /// </summary>
        internal const string FileExtension = "eol";

        /// <summary>
        /// 테스트를 중단할 때 Cleanup 스텝들을 실행할지 여부.
        /// </summary>
        public bool RunCleanup { get; set; }
        
        /// <summary>
        /// 테스트를 중단할 때 실행할 Cleanup 스텝 ID.
        /// </summary>
        public int[] CleanupSteps { get; set; }

        /// <summary>
        /// 시퀀스에서 이용하는 Variant들을 정의한다.
        /// </summary>
        public List<BoardVariant> Variants { get; set; }

        /// <summary>
        /// 스텝 리스트.
        /// </summary>
        public List<EolStep> Steps { get; set; } = new List<EolStep>();

        /// <summary>
        /// 시퀀스를 로딩한 파일 경로.
        /// </summary>
        [XmlIgnore]
        public string FilePath { get; internal set; }

        public EolSequence()
        {
        }

        public EolSequence(string filePath)
        {
            FilePath = filePath;
        }

        /// <summary>
        /// 시퀀스를 파일에 저장한다.
        /// 기존의 파일경로가 있어야 한다.
        /// </summary>
        public void Save()
        {
            using (var fileStream = new FileStream(FilePath, FileMode.Create, FileAccess.Write))
            {
                using (var writer = new StreamWriter(fileStream))
                {
                    var xmlSerializer = new XmlSerializer(typeof(EolSequence));
                    xmlSerializer.Serialize(writer, this);
                }
            }
        }

        /// <summary>
        /// 시퀀스를 지정한 파일에 저장한다.
        /// </summary>
        /// <param name="filePath"></param>
        public void SaveAs(string filePath)
        {
            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                using (var writer = new StreamWriter(fileStream))
                {
                    var xmlSerializer = new XmlSerializer(typeof(EolSequence));
                    xmlSerializer.Serialize(writer, this);
                }
            }

            FilePath = filePath;
        }

        /// <summary>
        /// 시퀀스를 XML 텍스트로 만든다.
        /// </summary>
        /// <returns></returns>
        public string ToXML()
        {
            using (var textWriter = new StringWriter())
            {
                var xmlSerializer = new XmlSerializer(typeof(EolSequence));
                xmlSerializer.Serialize(textWriter, this);
                return textWriter.ToString();
            }
        }

        /// <summary>
        /// XML 문자열로부터 시퀀스 생성.
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static EolSequence LoadFromXML(string xml)
        {
            var xmlSerializer = new XmlSerializer(typeof(EolSequence));
            var obj = xmlSerializer.Deserialize(new StringReader(xml));
            return obj as EolSequence;
        }

        /// <summary>
        /// 지정한 경로의 시퀀스 파일을 로딩한다.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static EolSequence Load(string path)
        {
            using (var stream = new FileStream(path, FileMode.Open))
            {
                var xmlSerializer = new XmlSerializer(typeof(EolSequence));
                var obj = xmlSerializer.Deserialize(stream) as EolSequence;
                obj.FilePath = path;
                return obj;
            }
        }

        public object Clone()
        {
            var clone = new EolSequence();
            clone.RunCleanup = RunCleanup;

            if (CleanupSteps != null)
            {
                clone.CleanupSteps = new int[CleanupSteps.Length];
                CleanupSteps.CopyTo(clone.CleanupSteps, 0);
            }

            if (Steps != null)
            {
                clone.Steps = new List<EolStep>();
                foreach (var step in Steps)
                {
                    clone.Steps.Add(step?.Clone() as EolStep);
                }
            }

            return clone;
        }

        /// <summary>
        /// 시퀀스에 있는 모든 스텝들의 채널 번호를 지정한 수만큼 증가/감소 시킨다.
        /// </summary>
        /// <param name="increment"></param>
        public void ModifyChannelNumbers(int increment)
        {
            var testChannels = new BindingList<int>[0];
            foreach (var step in Steps)
            {
                switch (step)
                {
                    case EolDmmStep _:
                    case EolElozStimulusStep _:
                    case EolOscopeStep _:
                    case EolWaveformGeneratorStep _:
                        if (step is EolDmmStep dmmStep)
                        {
                            testChannels = new[] { dmmStep.TestChannelHighInput, dmmStep.TestChannelLowInput, dmmStep.TestChannelHighSense,
                                dmmStep.TestChannelLowSense };
                        }
                        else if (step is EolElozStimulusStep elozStimulusStep)
                        {
                            testChannels = new[] { elozStimulusStep.TestChannelsHighForce, elozStimulusStep.TestChannelsLowForce,
                                elozStimulusStep.TestChannelsHighSense, elozStimulusStep.TestChannelsLowSense };
                        }
                        else if (step is EolOscopeStep oscopeStep)
                        {
                            testChannels = new[] { oscopeStep.TestChannel1High, oscopeStep.TestChannel1Low, oscopeStep.TestChannel2High, oscopeStep.TestChannel2Low, 
                                oscopeStep.TestChannel3High, oscopeStep.TestChannel3Low, oscopeStep.TestChannel4High, oscopeStep.TestChannel4Low };
                        }
                        else if (step is EolWaveformGeneratorStep wgenStep)
                        {
                            testChannels = new[] { wgenStep.TestChannel1High, wgenStep.TestChannel1Low, wgenStep.TestChannel2High, wgenStep.TestChannel2Low };
                        }

                        foreach (var channels in testChannels)
                        {
                            if (channels == null)
                            {
                                continue;
                            }

                            for (int i = 0; i < channels.Count; i++)
                            {
                                channels[i] += increment;
                            }
                        }
                        break;
                    case EolElozRelayStep elozRelayStep:
                        elozRelayStep.TestChannel += increment;
                        if (elozRelayStep.TestChannels != null)
                        {
                            for (int i = 0; i < elozRelayStep.TestChannels.Count; i++)
                            {
                                elozRelayStep.TestChannels[i] += increment;
                            }
                        }
                        break;
                    case EolElozVoltmeterStep elozVoltmeterStep:
                        var relayChannels = new[] { elozVoltmeterStep.TestChannelsHighInput, elozVoltmeterStep.TestChannelsLowInput };
                        foreach (var channels in relayChannels)
                        {
                            if (channels == null)
                            {
                                continue;
                            }

                            for (int i = 0; i < channels.Length; i++)
                            {
                                channels[i] += increment;
                            }
                        }
                        break;
                }
            }
        }
    }
}
