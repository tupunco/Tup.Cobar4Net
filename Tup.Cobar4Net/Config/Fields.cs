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

namespace Tup.Cobar4Net.Config
{
    /// <summary>字段类型及标识定义</summary>
    /// <author>xianmao.hexm</author>
    public abstract class Fields
    {
        /// <summary>field data type</summary>
        public const int FieldTypeDecimal = 0;

        public const int FieldTypeTiny = 1;

        public const int FieldTypeShort = 2;

        public const int FieldTypeLong = 3;

        public const int FieldTypeFloat = 4;

        public const int FieldTypeDouble = 5;

        public const int FieldTypeNull = 6;

        public const int FieldTypeTimestamp = 7;

        public const int FieldTypeLonglong = 8;

        public const int FieldTypeInt24 = 9;

        public const int FieldTypeDate = 10;

        public const int FieldTypeTime = 11;

        public const int FieldTypeDatetime = 12;

        public const int FieldTypeYear = 13;

        public const int FieldTypeNewdate = 14;

        public const int FieldTypeVarchar = 15;

        public const int FieldTypeBit = 16;

        public const int FieldTypeNewDecimal = 246;

        public const int FieldTypeEnum = 247;

        public const int FieldTypeSet = 248;

        public const int FieldTypeTinyBlob = 249;

        public const int FieldTypeMediumBlob = 250;

        public const int FieldTypeLongBlob = 251;

        public const int FieldTypeBlob = 252;

        public const int FieldTypeVarString = 253;

        public const int FieldTypeString = 254;

        public const int FieldTypeGeometry = 255;

        /// <summary>field flag</summary>
        public const int NotNullFlag = unchecked((int)(0x0001));

        public const int PriKeyFlag = unchecked((int)(0x0002));

        public const int UniqueKeyFlag = unchecked((int)(0x0004));

        public const int MultipleKeyFlag = unchecked((int)(0x0008));

        public const int BlobFlag = unchecked((int)(0x0010));

        public const int UnsignedFlag = unchecked((int)(0x0020));

        public const int ZerofillFlag = unchecked((int)(0x0040));

        public const int BinaryFlag = unchecked((int)(0x0080));

        public const int EnumFlag = unchecked((int)(0x0100));

        public const int AutoIncrementFlag = unchecked((int)(0x0200));

        public const int TimestampFlag = unchecked((int)(0x0400));

        public const int SetFlag = unchecked((int)(0x0800));
    }

    public static class FieldsConstants
    {
    }
}