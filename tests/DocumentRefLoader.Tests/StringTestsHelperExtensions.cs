using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentRefLoader.Tests
{
    public static class StringTestsHelperExtensions
    {
        /// <summary>
        /// replace windows line breaks by unix line breaks
        /// </summary>
        public static string InvariantNewline(this string s)
        {
            return s.Replace("\r\n", "\n");
        }
    }
}
