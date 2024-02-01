using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentPcbaTester
{
    /// <summary>
    /// 직렬연결을 위한 메시지 데이터.<br/>
    /// 형식: {ByteHeader(1byte)}{ByteLength(4bytes)}{StringField1}|{StringField2}|...|{StringFieldN}<br/>
    /// </summary>
    internal abstract class SeriesMessage
    {
        /// <summary>
        /// 명령 종류.
        /// </summary>
        internal enum RequestCommand
        {
            /// <summary>
            /// 처리 가능한 PCB 개수 문의.
            /// </summary>
            GetCapacity,
        }

        /// <summary>
        /// 메시지 시작 바이트.
        /// </summary>
        internal const byte Header = 0x96;

        /// <summary>
        /// 메시지 필드들 사이 구분자.
        /// </summary>
        internal const string Separator = "|";

        /// <summary>
        /// 메시지 필드 내에 구분자를 포함할 때 Escape 문자열.
        /// </summary>
        internal const string SeparatorESC = "&sep;";

        /// <summary>
        /// 문자열과 바이트 사이 변환을 위한 인코딩.
        /// </summary>
        internal static Encoding TextEncoding => Encoding.UTF8;

        /// <summary>
        /// 수행 명령.
        /// </summary>
        public RequestCommand Command { get; protected set; } = RequestCommand.GetCapacity;
    }
}
