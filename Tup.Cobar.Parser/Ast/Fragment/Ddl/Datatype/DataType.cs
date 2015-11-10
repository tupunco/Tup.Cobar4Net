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
using Sharpen;
using Tup.Cobar.Parser.Ast;
using Tup.Cobar.Parser.Ast.Expression.Primary;
using Tup.Cobar.Parser.Visitor;

namespace Tup.Cobar.Parser.Ast.Fragment.Ddl.Datatype
{
    /// <summary><code>spatial data type</code> for MyISAM is not supported</summary>
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class DataType : ASTNode
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

        private readonly DataType.DataTypeName typeName;

        private readonly bool unsigned;

        private readonly bool zerofill;

        /// <summary>for text only</summary>
        private readonly bool binary;

        private readonly Tup.Cobar.Parser.Ast.Expression.Expression length;

        private readonly Tup.Cobar.Parser.Ast.Expression.Expression decimals;

        private readonly Identifier charSet;

        private readonly Identifier collation;

        private readonly IList<Tup.Cobar.Parser.Ast.Expression.Expression> collectionVals;

        public DataType(DataType.DataTypeName typeName,
            bool unsigned,
            bool zerofill,
            bool binary,
            Tup.Cobar.Parser.Ast.Expression.Expression length,
            Tup.Cobar.Parser.Ast.Expression.Expression
             decimals,
            Identifier charSet,
            Identifier collation,
            IList<Tup.Cobar.Parser.Ast.Expression.Expression> collectionVals)
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

            this.typeName = typeName;
            this.unsigned = unsigned;
            this.zerofill = zerofill;
            this.binary = binary;
            this.length = length;
            this.decimals = decimals;
            this.charSet = charSet;
            this.collation = collation;
            this.collectionVals = collectionVals;
        }

        public virtual DataType.DataTypeName GetTypeName()
        {
            return typeName;
        }

        public virtual bool IsUnsigned()
        {
            return unsigned;
        }

        public virtual bool IsZerofill()
        {
            return zerofill;
        }

        public virtual bool IsBinary()
        {
            return binary;
        }

        public virtual Tup.Cobar.Parser.Ast.Expression.Expression GetLength()
        {
            return length;
        }

        public virtual Tup.Cobar.Parser.Ast.Expression.Expression GetDecimals()
        {
            return decimals;
        }

        public virtual Identifier GetCharSet()
        {
            return charSet;
        }

        public virtual Identifier GetCollation()
        {
            return collation;
        }

        public virtual IList<Tup.Cobar.Parser.Ast.Expression.Expression> GetCollectionVals()
        {
            return collectionVals;
        }

        public virtual void Accept(SQLASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
