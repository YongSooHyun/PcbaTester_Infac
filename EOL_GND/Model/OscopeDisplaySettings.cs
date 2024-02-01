using EOL_GND.Common;
using EOL_GND.Device;
using EOL_GND.Model.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Design;
using System.Xml.Serialization;

namespace EOL_GND.Model
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class OscopeDisplaySettings : INotifyPropertyChanged, ICloneable
    {
        [DefaultValue(""),
            Description(OscopeDevice.KeysightInfiniiVision3000T_X_Series + "\r\n" +
            "주석 문자열을 지정합니다. 주석 문자열에는 오실로스코프 화면의 주석 편집 상자에 들어갈 수 있는 최대 254개의 문자가 포함될 수 있습니다. " +
            "\"\\n\" 문자를 사용하여 주석 문자열에 캐리지 리턴을 포함할 수 있습니다. 이 문자는 줄 바꿈 문자가 아니라 문자열의 실제 \"\\\" (백슬래시) 및 \"n\" 문자입니다. " +
            "캐리지 리턴을 사용하면 주석 문자열에 사용할 수 있는 문자 수가 줄어듭니다.")]
        public string Annotation
        {
            get => _annotation;
            set
            {
                if (_annotation != value)
                {
                    _annotation = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string _annotation = null;

        [DefaultValue(false), DisplayName(nameof(Label) + " [Default = False]"),
            Description(OscopeDevice.KeysightInfiniiVision3000T_X_Series + "\r\n" +
            "아날로그 및 디지털 채널 라벨을 켜거나 끕니다.")]
        public bool? Label
        {
            get => _label;
            set
            {
                if (_label != value)
                {
                    _label = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool? _label = null;

        public enum ImageFormat
        {
            BMP,
            BMP8bit,
            PNG,
        }

        [DefaultValue(ImageFormat.PNG), Browsable(true), 
            Description(OscopeDevice.KeysightInfiniiVision3000T_X_Series + "\r\n" +
            "다운로드할 이미지 형식을 24-bit BMP, 8-bit BMP8bit, 또는 24-bit PNG 중 하나로 설정합니다.")]
        public ImageFormat DownloadImageFormat
        {
            get => _downloadImageFormat;
            set
            {
                if (_downloadImageFormat != value)
                {
                    _downloadImageFormat = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private ImageFormat _downloadImageFormat = ImageFormat.PNG;

        public enum PaletteType
        {
            Color,
            Grayscale,
        }

        [DefaultValue(PaletteType.Color), Browsable(true),
            Description(OscopeDevice.KeysightInfiniiVision3000T_X_Series + "\r\n" +
            "")]
        public PaletteType DownloadImagePalette
        {
            get => _downloadImagePalette;
            set
            {
                if (_downloadImagePalette != value)
                {
                    _downloadImagePalette = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private PaletteType _downloadImagePalette = PaletteType.Color;

        [DefaultValue(false), Browsable(true),
            Description("ASYNC 방식으로 다운로드할지 여부를 나타냅니다. ASYNC 방식 다운로드는 다운로드가 완료될 때까지 기다리지 않고 다음 스텝을 실행합니다.")]
        public bool DownloadAsync
        {
            get => _downloadAsync;
            set
            {
                if (_downloadAsync != value)
                {
                    _downloadAsync = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _downloadAsync = false;

        [Flags]
        public enum ImageOperations
        {
            None = 0,
            SaveToFile = 1,
            DisplayAsWindow = 2,
            All = SaveToFile | DisplayAsWindow,
        }

        [DefaultValue(ImageOperations.DisplayAsWindow), Browsable(true),
            Description("스크린 이미지를 다운로드한 다음 진행할 동작들을 설정합니다.\r\n" +
            " • " + nameof(ImageOperations.None) + " : 이미지 다운로드만 합니다.\r\n" +
            " • " + nameof(ImageOperations.SaveToFile) + " : 다운로드한 이미지를 파일로 저장합니다.\r\n" +
            " • " + nameof(ImageOperations.DisplayAsWindow) + " : 다운로드한 이미지를 윈도우로 보여줍니다.")]
        public ImageOperations DownloadCompleteActions
        {
            get => _downloadCompleteActions;
            set
            {
                if (_downloadCompleteActions != value)
                {
                    _downloadCompleteActions = value;
                    UpdateBrowsableAttributes();
                    NotifyPropertyChanged();
                }
            }
        }
        private ImageOperations _downloadCompleteActions = ImageOperations.DisplayAsWindow;

        [Editor(typeof(ImageFileNameEditor), typeof(UITypeEditor)), Browsable(false),
            Description("다운로드한 이미지를 저장할 파일 위치를 지정합니다. " +
            "이 값을 비워두면 'D:\\ElozPlugin\\OscilloscopeScreen\\{년}-{월}-{일}_{시분초}.png' 형식으로 저장됩니다.")]
        public string DownloadFile
        {
            get => _downloadFile;
            set
            {
                if (_downloadFile != value)
                {
                    _downloadFile = value;
                    UpdateBrowsableAttributes();
                    NotifyPropertyChanged();
                }
            }
        }
        private string _downloadFile = null;

        [DefaultValue(false), Browsable(false), 
            Description("다운로드 파일 이름의 뒤에 현재 날짜와 시간을 덧붙여 이름이 중복되지 않도록 합니다.")]
        public bool DownloadFileAppendTime
        {
            get => _downloadFileAppendTime;
            set
            {
                if (_downloadFileAppendTime != value)
                {
                    _downloadFileAppendTime = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _downloadFileAppendTime = false;

        /// <summary>
        /// 다운로드한 이미지를 별도의 윈도우에 보여주는 경우, 그 윈도우 크기를 설정하는 방식을 정의합니다.
        /// </summary>
        public enum DisplayWindowSize
        {
            /// <summary>
            /// 마지막 윈도우 크기 복원.
            /// </summary>
            RestoreLastState,

            /// <summary>
            /// 이미지 크기와 같은 크기로 보여줌.
            /// </summary>
            SameAsImageSize,

            /// <summary>
            /// 사용자가 지정한 위치와 크기로 보여줌.
            /// </summary>
            SpecifiedLocationAndSize,
        }

        [DefaultValue(DisplayWindowSize.RestoreLastState), Browsable(true), 
            Description("다운로드한 이미지를 보여줄 때 그 윈도우 위치와 크기를 어떻게 보여줄지 설정합니다.\r\n" +
            " • " + nameof(DisplayWindowSize.RestoreLastState) + " : 마지막 윈도우 상태를 복원합니다.\r\n" +
            " • " + nameof(DisplayWindowSize.SameAsImageSize) + " : 이미지 크기와 같은 크기로 보여줍니다.\r\n" +
            " • " + nameof(DisplayWindowSize.SpecifiedLocationAndSize) + " : 사용자가 설정한 위치와 크기로 보여줍니다.")]
        public DisplayWindowSize ImageDisplayMode
        {
            get => _imageDisplayMode;
            set
            {
                if (_imageDisplayMode != value)
                {
                    _imageDisplayMode = value;
                    UpdateBrowsableAttributes();
                    NotifyPropertyChanged();
                }
            }
        }
        private DisplayWindowSize _imageDisplayMode = DisplayWindowSize.RestoreLastState;

        [Browsable(false),
            Description(nameof(ImageDisplayMode) + "가 " + nameof(DisplayWindowSize.SpecifiedLocationAndSize) + "인 경우 보여줄 윈도우 위치를 설정합니다.")]
        public Point ImageDisplayLocation
        {
            get => _imageDisplayLocation;
            set
            {
                if (_imageDisplayLocation != value)
                {
                    _imageDisplayLocation = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private Point _imageDisplayLocation;

        [Browsable(false), 
            Description(nameof(ImageDisplayMode) + "가 " + nameof(DisplayWindowSize.SpecifiedLocationAndSize) + "인 경우 보여줄 윈도우 크기를 설정합니다.")]
        public Size ImageDisplaySize
        {
            get => _imageDisplaySize;
            set
            {
                if (_imageDisplaySize != value)
                {
                    _imageDisplaySize = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private Size _imageDisplaySize;

        [DefaultValue(1000), Browsable(false), 
            Description("다운로드한 이미지를 윈도우에 보여줄 때 윈도우가 자동으로 사라질 때까지의 시간을 ms 단위로 설정합니다. " +
            "이 값이 0 이하이면 윈도우가 자동으로 사라지지 않습니다.")]
        public int ImageDisplayCloseDelay
        {
            get => _imageDisplayCloseDelay;
            set
            {
                if (_imageDisplayCloseDelay != value)
                {
                    _imageDisplayCloseDelay = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private int _imageDisplayCloseDelay = 1000;

        public void UpdateBrowsableAttributes()
        {
            bool saveToFile = DownloadCompleteActions.HasFlag(ImageOperations.SaveToFile);
            Utils.SetBrowsableAttribute(this, nameof(DownloadFile), saveToFile);
            Utils.SetBrowsableAttribute(this, nameof(DownloadFileAppendTime), saveToFile && !string.IsNullOrWhiteSpace(DownloadFile));
            bool showAsWindow = DownloadCompleteActions.HasFlag(ImageOperations.DisplayAsWindow);
            Utils.SetBrowsableAttribute(this, nameof(ImageDisplayMode), showAsWindow);
            Utils.SetBrowsableAttribute(this, nameof(ImageDisplayLocation), showAsWindow && ImageDisplayMode == DisplayWindowSize.SpecifiedLocationAndSize);
            Utils.SetBrowsableAttribute(this, nameof(ImageDisplaySize), showAsWindow && ImageDisplayMode == DisplayWindowSize.SpecifiedLocationAndSize);
            Utils.SetBrowsableAttribute(this, nameof(ImageDisplayCloseDelay), showAsWindow);
        }

        public override string ToString()
        {
            return "";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property.  
        // The CallerMemberName attribute that is applied to the optional propertyName  
        //   parameter causes the property name of the caller to be substituted as an argument.
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public object Clone()
        {
            var obj = new OscopeDisplaySettings();
            obj.Annotation = Annotation;
            obj.Label = Label;
            obj.DownloadImageFormat = DownloadImageFormat;
            obj.DownloadImagePalette = DownloadImagePalette;
            obj.DownloadAsync = DownloadAsync;
            obj.DownloadCompleteActions = DownloadCompleteActions;
            obj.DownloadFile = DownloadFile;
            obj.DownloadFileAppendTime = DownloadFileAppendTime;
            obj.ImageDisplayMode = ImageDisplayMode;
            obj.ImageDisplayLocation = ImageDisplayLocation;
            obj.ImageDisplaySize = ImageDisplaySize;
            obj.ImageDisplayCloseDelay = ImageDisplayCloseDelay;
            obj.UpdateBrowsableAttributes();
            return obj;
        }
    }
}
