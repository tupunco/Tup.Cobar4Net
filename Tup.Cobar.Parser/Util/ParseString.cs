/*
* Copyright 1999-2012 Alibaba Group.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/
using System;
using Sharpen;

namespace Tup.Cobar.Parser.Util
{
    /// <author>xianmao.hexm</author>
    public sealed class ParseString
    {
        private static readonly byte[] EmptyByteArray = new byte[0];

        public static byte[] HexString2Bytes(char[] hexString, int offset, int length)
        {
            if (hexString == null)
            {
                return null;
            }
            if (length == 0)
            {
                return EmptyByteArray;
            }
            bool odd = length << 31 == int.MinValue;
            byte[] bs = new byte[odd ? (length + 1) >> 1 : length >> 1];
            for (int i = offset, limit = offset + length; i < limit; ++i)
            {
                char high;
                char low;
                if (i == offset && odd)
                {
                    high = '0';
                    low = hexString[i];
                }
                else
                {
                    high = hexString[i];
                    low = hexString[++i];
                }
                int b;
                switch (high)
                {
                    case '0':
                        {
                            b = 0;
                            break;
                        }

                    case '1':
                        {
                            b = unchecked((int)(0x10));
                            break;
                        }

                    case '2':
                        {
                            b = unchecked((int)(0x20));
                            break;
                        }

                    case '3':
                        {
                            b = unchecked((int)(0x30));
                            break;
                        }

                    case '4':
                        {
                            b = unchecked((int)(0x40));
                            break;
                        }

                    case '5':
                        {
                            b = unchecked((int)(0x50));
                            break;
                        }

                    case '6':
                        {
                            b = unchecked((int)(0x60));
                            break;
                        }

                    case '7':
                        {
                            b = unchecked((int)(0x70));
                            break;
                        }

                    case '8':
                        {
                            b = unchecked((int)(0x80));
                            break;
                        }

                    case '9':
                        {
                            b = unchecked((int)(0x90));
                            break;
                        }

                    case 'a':
                    case 'A':
                        {
                            b = unchecked((int)(0xa0));
                            break;
                        }

                    case 'b':
                    case 'B':
                        {
                            b = unchecked((int)(0xb0));
                            break;
                        }

                    case 'c':
                    case 'C':
                        {
                            b = unchecked((int)(0xc0));
                            break;
                        }

                    case 'd':
                    case 'D':
                        {
                            b = unchecked((int)(0xd0));
                            break;
                        }

                    case 'e':
                    case 'E':
                        {
                            b = unchecked((int)(0xe0));
                            break;
                        }

                    case 'f':
                    case 'F':
                        {
                            b = unchecked((int)(0xf0));
                            break;
                        }

                    default:
                        {
                            throw new ArgumentException("illegal hex-string: " + new string(hexString, offset
                                , length));
                        }
                }
                switch (low)
                {
                    case '0':
                        {
                            break;
                        }

                    case '1':
                        {
                            b += 1;
                            break;
                        }

                    case '2':
                        {
                            b += 2;
                            break;
                        }

                    case '3':
                        {
                            b += 3;
                            break;
                        }

                    case '4':
                        {
                            b += 4;
                            break;
                        }

                    case '5':
                        {
                            b += 5;
                            break;
                        }

                    case '6':
                        {
                            b += 6;
                            break;
                        }

                    case '7':
                        {
                            b += 7;
                            break;
                        }

                    case '8':
                        {
                            b += 8;
                            break;
                        }

                    case '9':
                        {
                            b += 9;
                            break;
                        }

                    case 'a':
                    case 'A':
                        {
                            b += 10;
                            break;
                        }

                    case 'b':
                    case 'B':
                        {
                            b += 11;
                            break;
                        }

                    case 'c':
                    case 'C':
                        {
                            b += 12;
                            break;
                        }

                    case 'd':
                    case 'D':
                        {
                            b += 13;
                            break;
                        }

                    case 'e':
                    case 'E':
                        {
                            b += 14;
                            break;
                        }

                    case 'f':
                    case 'F':
                        {
                            b += 15;
                            break;
                        }

                    default:
                        {
                            throw new ArgumentException("illegal hex-string: " + new string(hexString, offset
                                , length));
                        }
                }
                bs[(i - offset) >> 1] = unchecked((byte)b);
            }
            return bs;
        }
    }
}
