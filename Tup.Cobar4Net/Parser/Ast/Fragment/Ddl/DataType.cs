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
using System.Collections.Generic;
using Tup.Cobar4Net.Parser.Ast.Expression;
using Tup.Cobar4Net.Parser.Ast.Expression.Primary;
using Tup.Cobar4Net.Parser.Visitor;

namespace Tup.Cobar4Net.Parser.Ast.Fragment.Ddl
{
    public enum DataTypeName
    {
        None = 0,

        Bit,
        Tinyint,
        Smallint,
        Mediumint,
        Int,
        Bigint,
        Real,
        Double,
        Float,
        Decimal,
        Date,
        Time,
        Timestamp,
        Datetime,
        Year,
        Char,
        Varchar,
        Binary,
        Varbinary,
        Tinyblob,
        Blob,
        Mediumblob,
        Longblob,
        Tinytext,
        Text,
        Mediumtext,
        Longtext,
        Enum,
        Set
    }

    /// <summary><code>spatial data type</code> for MyISAM is not supported</summary>
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public class DataType : IAstNode
    {
        public DataType(DataTypeName typeName,
                        bool unsigned,
                        bool zerofill,
                        bool binary,
                        IExpression length,
                        IExpression decimals,
                        Identifier charSet,
                        Identifier collation,
                        IList<IExpression> collectionVals)
        {
            // BIT[(length)]
            // | TINYINT[(length)] [UNSIGNED] [ZEROFILL]
            // | SMALLINT[(length)] [UNSIGNED] [ZEROFILL]
            // | MEDIUMINT[(length)] [UNSIGNED] [ZEROFILL]
            // | INT[(length)] [UNSIGNED] [ZEROFILL]
            // | INTEGER[(length)] [UNSIGNED] [ZEROFILL]
            // | BIGINT[(length)] [UNSIGNED] [ZEROFILL]
            // | DOUBLE[(length,decimals)] [UNSIGNED] [ZEROFILL]
            // | REAL[(length,decimals)] [UNSIGNED] [ZEROFILL]
            // | FLOAT[(length,decimals)] [UNSIGNED] [ZEROFILL]
            // | DECIMAL[(length[,decimals])] [UNSIGNED] [ZEROFILL]
            // | NUMERIC[(length[,decimals])] [UNSIGNED] [ZEROFILL] 同上
            // | DATE
            // | TIME
            // | TIMESTAMP
            // | DATETIME
            // | YEAR
            // | CHAR[(length)][CHARACTER SET charset_name] [COLLATE collation_name]
            // | VARCHAR(length)[CHARACTER SET charset_name] [COLLATE collation_name]
            // | BINARY[(length)]
            // | VARBINARY(length)
            // | TINYBLOB
            // | BLOB
            // | MEDIUMBLOB
            // | LONGBLOB
            // | TINYTEXT [BINARY][CHARACTER SET charset_name] [COLLATE collation_name]
            // | TEXT [BINARY][CHARACTER SET charset_name] [COLLATE collation_name]
            // | MEDIUMTEXT [BINARY][CHARACTER SET charset_name] [COLLATE
            // collation_name]
            // | LONGTEXT [BINARY][CHARACTER SET charset_name] [COLLATE collation_name]
            // | ENUM(value1,value2,value3,...)[CHARACTER SET charset_name] [COLLATE
            // collation_name]
            // | SET(value1,value2,value3,...)[CHARACTER SET charset_name] [COLLATE
            // collation_name]
            // | spatial_type 不支�?
            if (typeName == DataTypeName.None)
            {
                throw new ArgumentException("typeName is null");
            }

            TypeName = typeName;
            IsUnsigned = unsigned;
            IsZerofill = zerofill;
            IsBinary = binary;
            Length = length;
            Decimals = decimals;
            CharSet = charSet;
            Collation = collation;
            CollectionVals = collectionVals;
        }

        public virtual DataTypeName TypeName { get; }

        public virtual bool IsUnsigned { get; }

        public virtual bool IsZerofill { get; }

        public virtual bool IsBinary { get; }

        public virtual IExpression Length { get; }

        public virtual IExpression Decimals { get; }

        public virtual Identifier CharSet { get; }

        public virtual Identifier Collation { get; }

        public virtual IList<IExpression> CollectionVals { get; }

        public virtual void Accept(ISqlAstVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}