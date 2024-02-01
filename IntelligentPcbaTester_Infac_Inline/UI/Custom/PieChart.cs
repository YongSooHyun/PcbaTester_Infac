using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace IntelligentPcbaTester
{
    public partial class PieChart : UserControl
    {
        /// <summary>
        /// Percentage 를 나타내는 색깔.
        /// </summary>
        [DefaultValue(typeof(Color), "Black")]
        public Color CircleForeColor
        {
            get => _circleForeColor;
            set
            {
                if (_circleForeColor != value)
                {
                    _circleForeColor = value;
                    Invalidate();
                }
            }
        }
        private Color _circleForeColor = Color.Black;

        /// <summary>
        /// 원 배경색.
        /// </summary>
        [DefaultValue(typeof(Color), "White")]
        public Color CircleBackColor
        {
            get => _circleBackColor;
            set
            {
                if (_circleBackColor != value)
                {
                    _circleBackColor = value;
                    Invalidate();
                }
            }
        }
        private Color _circleBackColor = Color.White;

        /// <summary>
        /// <see cref="Control.DoubleBuffered"/> 프로퍼티.
        /// </summary>
        [DefaultValue(false),
            Description("Gets or sets a value indicating whether this control should redraw its surface using a secondary buffer to reduce or prevent flicker.")]
        public bool DoubleBufferedProperty
        {
            get => DoubleBuffered;
            set
            {
                DoubleBuffered = value;
            }
        }

        /// <summary>
        /// 도 단위로 나타낸 시각 각도.
        /// </summary>
        [DefaultValue(0.0F),
            Description("Angle in degrees measured clockwise from the x-axis to the first side of the pie section.")]
        public float StartAngle
        {
            get => _startAngle;
            set
            {
                if (_startAngle != value)
                {
                    _startAngle = value;
                    Invalidate();
                }
            }
        }
        private float _startAngle = 0;

        /// <summary>
        /// 도 단위로 나타낸 파이의 각도 크기.
        /// </summary>
        [DefaultValue(360.0F), 
            Description("Angle in degrees measured clockwise from the startAngle parameter to the second side of the pie section.")]
        private float SweepAngle
        {
            get => _sweepAngle;
            set
            {
                if (_sweepAngle != value)
                {
                    _sweepAngle = value;
                    Invalidate();
                }
            }
        }
        private float _sweepAngle = 360;

        /// <summary>
        /// 퍼센트. 0보다 작은 값을 설정하면 0으로, 100보다 큰 값을 설정하면 100으로 된다.
        /// </summary>
        public float Percent
        {
            get => _percent;
            set
            {
                if (_percent != value)
                {
                    if (value < 0)
                    {
                        _percent = 0;
                    }
                    else if (value > 100)
                    {
                        _percent = 100;
                    }
                    else
                    {
                        _percent = value;
                    }

                    SweepAngle = _percent / 100 * 360;
                }
            }
        }
        private float _percent = 50;

        public PieChart()
        {
            InitializeComponent();

            BackColor = Color.Transparent;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var pieRect = new Rectangle(Point.Empty, Size);
            e.Graphics.FillPie(new SolidBrush(CircleBackColor), pieRect, 0, 360);
            e.Graphics.FillPie(new SolidBrush(CircleForeColor), pieRect, StartAngle, SweepAngle);
        }
    }
}
