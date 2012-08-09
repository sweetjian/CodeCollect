using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
 
namespace Lenic.Core
{
    /// <summary>
    /// 数字转大写汉字字符类
    /// </summary>
    [DebuggerStepThrough]
    public class NumericConvert
    {
        #region Private Fields
        /// <summary>
        /// 段的分隔字符，从低到高依次增加：空、万、亿、万亿，最高不超过万亿的数字都可以
        /// </summary>
        public static readonly string[] DefaultRangeNumeric = new string[] { string.Empty, "万", "亿", "兆" };
        /// <summary>
        /// 位的分隔字符，从低到高依次是：仟、佰、拾、空
        /// </summary>
        public static readonly char[] DefaultUnitNumeric = new char[] { '仟', '佰', '拾', char.MinValue };
        /// <summary>
        /// 数字替换的字符，从低到高依次是：零、壹、贰、叁、肆、伍、陆、柒、捌、玖
        /// </summary>
        public static readonly char[] DefaultCharNumeric = new char[] { '零', '壹', '贰', '叁', '肆', '伍', '陆', '柒', '捌', '玖' };
 
        private char[] charNumeric = DefaultCharNumeric;
        private string[] rangeNumeric = DefaultRangeNumeric;
        private char zeroNumeric = DefaultCharNumeric[0];
        private char[] unitNumeric = DefaultUnitNumeric;
        #endregion
 
        #region Business Methods
        /// <summary>
        /// 重置数字替换的字符，必须从小到大是 10 个汉字字符。
        /// </summary>
        /// <param name="data">目标字符数组</param>
        /// <returns>成功替换则返回 <c>true</c> 。</returns>
        public bool ResetCharNumeric(char[] data)
        {
            if (data == null || data.Length != 10)
                return false;
 
            charNumeric = data;
            zeroNumeric = data[0];
            return true;
        }
 
        /// <summary>
        /// 重置位的分隔字符，必须从小到大是 4 个汉字字符。
        /// </summary>
        /// <param name="data">目标字符数组</param>
        /// <returns>成功替换则返回 <c>true</c> 。</returns>
        public bool ResetUnitNumeric(char[] data)
        {
            if (data == null || data.Length != 4)
                return false;
 
            unitNumeric = data;
            return true;
        }
 
        /// <summary>
        /// 重置段的分隔字符。
        /// </summary>
        /// <param name="data">目标字符数组</param>
        public void ResetRangeNumeric(string[] data)
        {
            rangeNumeric = data ?? DefaultRangeNumeric;
        }
 
        /// <summary>
        /// 执行数字转大写汉字字符的操作。
        /// </summary>
        /// <param name="obj">待转换的数字</param>
        /// <returns>转换完成的大写汉字字符串。</returns>
        public string Convert(decimal obj)
        {
            if (obj > 9999999999999999.99M)
                throw new ApplicationException("The numeric too big!");
 
            var data = obj.ToString("#.##");
 
            var list = data.Split('.');
 
            var result = MultiConvert(list[0]);
            if (list.Length > 1)
                result += DecimalConvert(list[1]);
 
            return result;
        }
        #endregion
 
        #region Private Methods
        private string MultiConvert(string data)
        {
            var list = Split(data).ToArray();
 
            var results = new List<string>();
            foreach (var item in list)
                results.Add(SingleConvert(item));
 
            var sbResult = new StringBuilder();
            var len = results.Count;
            var index = len - 1;
 
            for (int i = 0; i < len; i++)
            {
                var item = results[i];
                if ((i + 2 < len) && item == zeroNumeric.ToString() && results[i + 1].StartsWith(zeroNumeric.ToString()))
                    continue;
 
                if (!(i == (len - 1) && item == zeroNumeric.ToString()))
                    sbResult.Append(item);
 
                var unit = rangeNumeric[index - i];
                if (unit != string.Empty && item != zeroNumeric.ToString())
                    sbResult.Append(unit);
            }
 
            if (sbResult[sbResult.Length - 1] == zeroNumeric)
                sbResult.Remove(sbResult.Length - 1, 1);
            sbResult.Append("元");
 
            return sbResult.ToString();
        }
 
        private string SingleConvert(string data)
        {
            var len = data.Length;
            var result = new List<char>();
            var previousChar = char.MinValue;
            var unitIndex = len == 4 ? 0 : (4 - len);
 
            for (int i = 0; i < len; i++)
            {
                var item = CharToInt(data[i]);
                var currentChineseChar = charNumeric[item];
                if (currentChineseChar == previousChar && previousChar == zeroNumeric && currentChineseChar == zeroNumeric)
                    continue;
                else
                {
                    result.Add(previousChar = currentChineseChar);
 
                    var currentUnit = unitNumeric[unitIndex + i];
                    if (currentChineseChar != zeroNumeric && currentUnit != char.MinValue)
                        result.Add(currentUnit);
                }
            }
 
            if (result.Count != 1 && result.Last() == zeroNumeric)
                result.RemoveAt(result.Count - 1);
 
            return new string(result.ToArray());
        }
 
        private string DecimalConvert(string data)
        {
            StringBuilder sbResult = new StringBuilder();
            if (data[0] != '0')
            {
                sbResult.Append(charNumeric[CharToInt(data[0])]);
                sbResult.Append("角");
            }
            if (data[1] != '0')
            {
                sbResult.Append(charNumeric[CharToInt(data[1])]);
                sbResult.Append("分");
            }
            return sbResult.ToString();
        }
 
        private IEnumerable<string> Split(string data)
        {
            var len = data.Length / 4;
            var mod = data.Length % 4;
            if (mod != 0)
                len += 1;
 
            var startIndex = 0;
            var blockLength = mod != 0 ? mod : 4;
 
            for (int i = 0; i < len; i++)
            {
                yield return data.Substring(startIndex, blockLength);
                startIndex += blockLength;
                blockLength = 4;
            }
        }
 
        private int CharToInt(char obj)
        {
            return ((int)obj) - 48;
        }
        #endregion
    }
}