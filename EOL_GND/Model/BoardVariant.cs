using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EOL_GND.Model
{
    public class BoardVariant
    {
        /// <summary>
        /// Variant 이름.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 설명.
        /// </summary>
        public string Remarks { get; set; }

        /// <summary>
        /// Variant는 대소문자 구분 없이 텍스트가 같으면 같은 것으로 처리됨.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static bool Equals(BoardVariant first, BoardVariant second)
        {
            return string.Equals(first.Name, second.Name, StringComparison.OrdinalIgnoreCase);
        }
    }
}
