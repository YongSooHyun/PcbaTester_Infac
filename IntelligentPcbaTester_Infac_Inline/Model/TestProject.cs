using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Xml.Serialization;

namespace IntelligentPcbaTester
{
    /// <summary>
    /// 멀티패널 대표 바코드의 시리얼 넘버가 어떤 수로 시작하는지에 대한 정보.
    /// </summary>
    public enum MultiPanelBarcodeType
    {
        /// <summary>
        /// 제한없음.
        /// </summary>
        None,

        /// <summary>
        /// 홀수.
        /// </summary>
        Odd,

        /// <summary>
        /// 짝수.
        /// </summary>
        Even,
    }

    /// <summary>
    /// 테스트 프로젝트 관련 각종 정보를 저장하는 Model.
    /// </summary>
    public class TestProject
    {
        /// <summary>
        /// 멀티패널 테스트 시 보드별 섹션이름에 포함되는 접두사.
        /// </summary>
        internal const string BoardPrefix = "Board";

        /// <summary>
        /// 프로젝트 이름. 파일 이름이 프로젝트 이름으로 된다.
        /// </summary>
        [DisplayName("프로젝트 이름")]
        public string Path { get; set; }

        /// <summary>
        /// 모델.
        /// </summary>
        [DisplayName("모델")]
        public string Model { get; set; }

        /// <summary>
        /// FID. 1 ~ 255.
        /// </summary>
        [DisplayName("FID")]
        public List<int> FIDs { get; set; } = new List<int>();

        /// <summary>
        /// 연배열 속성. 1 ~ 4.
        /// </summary>
        [DisplayName("Panel")]
        public int Panel { get; set; } = 1;

        public bool Board1Checked { get; set; } = true;
        public bool Board2Checked { get; set; } = false;
        public bool Board3Checked { get; set; } = false;
        public bool Board4Checked { get; set; } = false;

        /// <summary>
        /// 2연배열인 경우 보드를 좌우로 배치할 것인지 상하로 배치할 것인지 여부.
        /// </summary>
        public bool TwoBoardsLeftRight { get; set; } = true;

        /// <summary>
        /// 2연배열인 경우 상/하 배치일 때 밑의 보드가 첫 보드인지 여부.
        /// </summary>
        public bool BottomBoardFirst { get; set; } = false;

        /// <summary>
        /// 멀티패널 대표 바코드의 시리얼 넘버가 어떤 수로 시작하는지에 대한 정보.
        /// </summary>
        public MultiPanelBarcodeType MultiPanelBarcodeType { get; set; } = MultiPanelBarcodeType.None;

        /// <summary>
        /// eloZ1 용 *.tpr 파일 이름.
        /// </summary>
        [DisplayName("ICT")]
        public string IctProjectName { get; set; }

        /// <summary>
        /// Master Good 프로젝트를 로딩할 S/N.
        /// </summary>
        public string IctMasterGoodSerialNumber { get; set; }

        /// <summary>
        /// Master Good 프로젝트 이름.
        /// </summary>
        public string IctMasterGoodProjectName { get; set; }

        /// <summary>
        /// Master Fail 프로젝트를 로딩할 S/N.
        /// </summary>
        public string IctMasterFailSerialNumber { get; set; }

        /// <summary>
        /// Master Fail 프로젝트 이름.
        /// </summary>
        public string IctMasterFailProjectName { get; set; }

        /// <summary>
        /// Variant 이름.
        /// </summary>
        public string IctVariantName { get; set; }

        //
        // ICT-unpower
        //
        public bool IctUnpowerEnabled { get; set; } = true;
        public string IctUnpowerSectionTitle { get; set; } = "ICT";
        public string IctUnpowerSectionName { get; set; }
        public bool IctUnpowerRetryEnabled { get; set; } = true;
        public bool IctUnpowerPressDown { get; set; } = true;
        public bool IctUnpowerPressUp { get; set; } = false;
        public int IctUnpowerRunOrder { get; set; } = 1;
        public bool IctUnpowerIsEOL { get; set; } = false;
        internal SectionInfo IctUnpowerSection
        {
            get
            {
                return new SectionInfo
                {
                    Enabled = IctUnpowerEnabled,
                    SectionTitle = IctUnpowerSectionTitle,
                    SectionName = IctUnpowerSectionName,
                    RetryEnabled = IctUnpowerRetryEnabled,
                    PressDown = IctUnpowerPressDown,
                    PressUp = IctUnpowerPressUp,
                    RunOrder = IctUnpowerRunOrder,
                    IsEOL = IctUnpowerIsEOL,
                };
            }
        }

        //
        // ICT-power
        //
        public bool IctPowerEnabled { get; set; } = true;
        public string IctPowerSectionTitle { get; set; } = "power";
        public string IctPowerSectionName { get; set; }
        public bool IctPowerRetryEnabled { get; set; } = true;
        public bool IctPowerPressDown { get; set; } = false;
        public bool IctPowerPressUp { get; set; } = false;
        public int IctPowerRunOrder { get; set; } = 2;
        public bool IctPowerIsEOL { get; set; } = false;
        internal SectionInfo IctPowerSection
        {
            get
            {
                return new SectionInfo
                {
                    Enabled = IctPowerEnabled,
                    SectionTitle = IctPowerSectionTitle,
                    SectionName = IctPowerSectionName,
                    RetryEnabled = IctPowerRetryEnabled,
                    PressDown = IctPowerPressDown,
                    PressUp = IctPowerPressUp,
                    RunOrder = IctPowerRunOrder,
                    IsEOL = IctPowerIsEOL,
                };
            }
        }

        //
        // ISP(Novaflash)
        //
        public bool NovaEnabled { get; set; } = true;
        public string NovaSectionTitle { get; set; } = "ISP";
        public string NovaSectionName { get; set; }
        public bool NovaRetryEnabled { get; set; } = true;
        public bool NovaPressDown { get; set; } = false;
        public bool NovaPressUp { get; set; } = false;
        public int NovaRunOrder { get; set; } = 3;
        public bool NovaIsEOL { get; set; } = false;
        internal SectionInfo NovaSection
        {
            get
            {
                return new SectionInfo
                {
                    Enabled = NovaEnabled,
                    SectionTitle = NovaSectionTitle,
                    SectionName = NovaSectionName,
                    RetryEnabled = NovaRetryEnabled,
                    PressDown = NovaPressDown,
                    PressUp = NovaPressUp,
                    RunOrder = NovaRunOrder,
                    IsISP = true,
                    IsEOL = NovaIsEOL,
                };
            }
        }

        //
        // JTAG
        //
        public bool JtagEnabled { get; set; } = true;
        public string JtagSectionTitle { get; set; } = "JTAG";
        public string JtagSectionName { get; set; }
        public bool JtagRetryEnabled { get; set; } = true;
        public bool JtagPressDown { get; set; } = false;
        public bool JtagPressUp { get; set; } = false;
        public int JtagRunOrder { get; set; } = 4;
        public bool JtagIsEOL { get; set; } = false;
        internal SectionInfo JtagSection
        {
            get
            {
                return new SectionInfo
                {
                    Enabled = JtagEnabled,
                    SectionTitle = JtagSectionTitle,
                    SectionName = JtagSectionName,
                    RetryEnabled = JtagRetryEnabled,
                    PressDown = JtagPressDown,
                    PressUp = JtagPressUp,
                    RunOrder = JtagRunOrder,
                    IsEOL = JtagIsEOL,
                };
            }
        }
        public string JtagProjectName { get; set; }

        //
        // Function
        //
        public bool FunctionEnabled { get; set; } = true;
        public string FunctionSectionTitle { get; set; } = "FUNC";
        public string FunctionSectionName { get; set; }
        public bool FunctionRetryEnabled { get; set; } = true;
        public bool FunctionPressDown { get; set; } = false;
        public bool FunctionPressUp { get; set; } = false;
        public int FunctionRunOrder { get; set; } = 5;
        public bool FunctionIsEOL { get; set; } = false;
        internal SectionInfo FunctionSection
        {
            get
            {
                return new SectionInfo
                {
                    Enabled = FunctionEnabled,
                    SectionTitle = FunctionSectionTitle,
                    SectionName = FunctionSectionName,
                    RetryEnabled = FunctionRetryEnabled,
                    PressDown = FunctionPressDown,
                    PressUp = FunctionPressUp,
                    RunOrder = FunctionRunOrder,
                    IsEOL = FunctionIsEOL,
                };
            }
        }

        //
        // EOL
        //
        public bool EolEnabled { get; set; } = true;
        public string EolSectionTitle { get; set; } = "EOL";
        public string EolSectionName { get; set; }
        public bool EolRetryEnabled { get; set; } = true;
        public bool EolPressDown { get; set; } = false;
        public bool EolPressUp { get; set; } = false;
        public int EolRunOrder { get; set; } = 6;
        public bool EolIsEOL { get; set; } = true;
        internal SectionInfo EolSection
        {
            get
            {
                return new SectionInfo
                {
                    Enabled = EolEnabled,
                    SectionTitle = EolSectionTitle,
                    SectionName = EolSectionName,
                    RetryEnabled = EolRetryEnabled,
                    PressDown = EolPressDown,
                    PressUp = EolPressUp,
                    RunOrder = EolRunOrder,
                    IsEOL = EolIsEOL,
                };
            }
        }

        //
        // Extended section 1.
        //
        public bool Ext1Enabled { get; set; } = true;
        public string Ext1SectionTitle { get; set; } = "EXT1";
        public string Ext1SectionName { get; set; }
        public bool Ext1RetryEnabled { get; set; } = true;
        public bool Ext1PressDown { get; set; } = false;
        public bool Ext1PressUp { get; set; } = false;
        public int Ext1RunOrder { get; set; } = 7;
        public bool Ext1IsEOL { get; set; } = true;
        internal SectionInfo Ext1Section
        {
            get
            {
                return new SectionInfo
                {
                    Enabled = Ext1Enabled,
                    SectionTitle = Ext1SectionTitle,
                    SectionName = Ext1SectionName,
                    RetryEnabled = Ext1RetryEnabled,
                    PressDown = Ext1PressDown,
                    PressUp = Ext1PressUp,
                    RunOrder = Ext1RunOrder,
                    IsEOL = Ext1IsEOL,
                };
            }
        }

        //
        // Extended section 2.
        //
        public bool Ext2Enabled { get; set; } = true;
        public string Ext2SectionTitle { get; set; } = "EXT2";
        public string Ext2SectionName { get; set; }
        public bool Ext2RetryEnabled { get; set; } = true;
        public bool Ext2PressDown { get; set; } = false;
        public bool Ext2PressUp { get; set; } = false;
        public int Ext2RunOrder { get; set; } = 8;
        public bool Ext2IsEOL { get; set; } = true;
        internal SectionInfo Ext2Section
        {
            get
            {
                return new SectionInfo
                {
                    Enabled = Ext2Enabled,
                    SectionTitle = Ext2SectionTitle,
                    SectionName = Ext2SectionName,
                    RetryEnabled = Ext2RetryEnabled,
                    PressDown = Ext2PressDown,
                    PressUp = Ext2PressUp,
                    RunOrder = Ext2RunOrder,
                    IsEOL = Ext2IsEOL,
                };
            }
        }

        /// <summary>
        /// 실행할 GRP 파일 리스트.
        /// </summary>
        public List<GrpInfo> GrpFiles { get; set; }

        public float Power1Voltage { get; set; }
        public float Power1Current { get; set; }
        public bool Power1Enabled { get; set; } = true;
        public float Power2Voltage { get; set; }
        public float Power2Current { get; set; }
        public bool Power2Enabled { get; set; } = true;

        /// <summary>
        /// 부품정보 파일 경로.
        /// </summary>
        public string PcbInfoFile { get; set; }

        /// <summary>
        /// 추가 설명.
        /// </summary>
        [DisplayName("비고")]
        public string Description { get; set; }

        internal void CreateEmptyGrpInfos()
        {
            GrpFiles = new List<GrpInfo>();
            for (int i = 0; i < 12; i++)
            {
                GrpFiles.Add(new GrpInfo
                {
                    Channel = i / 3,
                });
            }
        }

        /// <summary>
        /// 바코드에 따라 마스터 또는 원래 프로젝트를 리턴한다.
        /// </summary>
        /// <param name="barcode"></param>
        /// <param name="ictProjectName"></param>
        /// <param name="suffix"></param>
        internal void GetIctProjectName(string barcode, out string ictProjectName, out string suffix, out bool? isMasterGood)
        {
            ictProjectName = "";
            suffix = "";
            isMasterGood = null;
            if (!string.IsNullOrEmpty(barcode))
            {
                if (barcode.Equals(IctMasterGoodSerialNumber, StringComparison.OrdinalIgnoreCase))
                {
                    ictProjectName = IctMasterGoodProjectName;
                    suffix = "(0)";
                    isMasterGood = true;
                }
                else if (barcode.Equals(IctMasterFailSerialNumber, StringComparison.OrdinalIgnoreCase))
                {
                    ictProjectName = IctMasterFailProjectName;
                    suffix = "(1)";
                    isMasterGood = false;
                }
            }

            if (string.IsNullOrEmpty(ictProjectName))
            {
                ictProjectName = IctProjectName;
                suffix = "";
                isMasterGood = null;
            }
        }

        /// <summary>
        /// 실행순서에 따라 배열된 섹션 실행 정보를 리턴한다.
        /// </summary>
        /// <returns></returns>
        internal List<SectionInfo> GetOrderedSectionInfos(out int ispSectionIndex, out int eolFirstSectionIndex, out int ictFirstSectionIndex)
        {
            var sectionInfos = new List<SectionInfo>()
            {
                IctUnpowerSection, IctPowerSection, NovaSection, JtagSection, FunctionSection, EolSection, Ext1Section, Ext2Section,
            };

            var sortedInfos = sectionInfos.OrderBy(si => si.RunOrder).ToList();

            ispSectionIndex = 2;
            eolFirstSectionIndex = -1;
            ictFirstSectionIndex = -1;
            for (int i = 0; i < sortedInfos.Count; i++)
            {
                var info = sortedInfos[i];

                if (info.IsISP)
                {
                    ispSectionIndex = i;
                }

                if (eolFirstSectionIndex < 0 && info.IsEOL && info.Enabled && !string.IsNullOrEmpty(info.SectionName))
                {
                    eolFirstSectionIndex = i;
                }
                else if (ictFirstSectionIndex < 0 && !info.IsEOL && info.Enabled && !string.IsNullOrEmpty(info.SectionName))
                {
                    ictFirstSectionIndex = i;
                }
            }

            if (eolFirstSectionIndex >= 0 && ictFirstSectionIndex >= eolFirstSectionIndex)
            {
                ictFirstSectionIndex = -1;
            }

            return sortedInfos;
        }

        internal List<GrpInfo> GetOrderedGrpInfos()
        {
            return GrpFiles?.OrderBy(g => g.Channel)?.ThenBy(g => g.RunOrder)?.ToList();
        }

        public class SectionInfo
        {
            public bool Enabled { get; set; }
            public string SectionTitle { get; set; }
            public string SectionName { get; set; }
            public bool RetryEnabled { get; set; } = true;
            public bool PressDown { get; set; } = false;
            public bool PressUp { get; set; } = false;
            public int RunOrder { get; set; } = 1;
            public bool IsISP { get; set; } = false;
            public bool IsEOL { get; set; } = false;
            public bool IsIspSection => SectionName?.IndexOf("isp", StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
