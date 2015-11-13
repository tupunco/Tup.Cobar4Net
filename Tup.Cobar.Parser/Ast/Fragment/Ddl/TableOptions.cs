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
using System.Collections.Generic;
using Sharpen;
using Tup.Cobar.Parser.Ast;
using Tup.Cobar.Parser.Ast.Expression.Primary;
using Tup.Cobar.Parser.Ast.Expression.Primary.Literal;
using Tup.Cobar.Parser.Visitor;
using Expr = Tup.Cobar.Parser.Ast.Expression.Expression;

namespace Tup.Cobar.Parser.Ast.Fragment.Ddl
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class TableOptions : ASTNode
    {
        public enum InsertMethod
        {
            No,
            First,
            Last
        }

        public enum PackKeys
        {
            False,
            True,
            Default
        }

        public enum RowFormat
        {
            Default,
            Dynamic,
            Fixed,
            Compressed,
            Redundant,
            Compact
        }

        private Identifier engine;

        private Expr autoIncrement;

        private Expr avgRowLength;

        private Identifier charSet;

        private Identifier collation;

        private bool checkSum;

        private LiteralString comment;

        private LiteralString connection;

        private LiteralString dataDir;

        private LiteralString indexDir;

        private bool delayKeyWrite;

        private TableOptions.InsertMethod insertMethod;

        private Expr keyBlockSize;

        private Expr maxRows;

        private Expr minRows;

        private TableOptions.PackKeys packKeys;

        private LiteralString password;

        private TableOptions.RowFormat rowFormat;

        private IList<Identifier> union;

        public TableOptions()
        {
        }

        // table_option:
        // ENGINE [=] engine_name
        // | AUTO_INCREMENT [=] value
        // | AVG_ROW_LENGTH [=] value
        // | [DEFAULT] CHARACTER SET [=] charset_name
        // | CHECKSUM [=] {0 | 1}
        // | [DEFAULT] COLLATE [=] collation_name
        // | COMMENT [=] 'string'
        // | CONNECTION [=] 'connect_string'
        // | DATA DIRECTORY [=] 'absolute path to directory'
        // | DELAY_KEY_WRITE [=] {0 | 1}
        // | INDEX DIRECTORY [=] 'absolute path to directory'
        // | INSERT_METHOD [=] { NO | FIRST | LAST }
        // | KEY_BLOCK_SIZE [=] value
        // | MAX_ROWS [=] value
        // | MIN_ROWS [=] value
        // | PACK_KEYS [=] {0 | 1 | DEFAULT}
        // | PASSWORD [=] 'string'
        // | ROW_FORMAT [=] {DEFAULT|DYNAMIC|FIXED|COMPRESSED|REDUNDANT|COMPACT}
        // | UNION [=] (tbl_name[,tbl_name]...)
        public virtual Identifier GetEngine()
        {
            return engine;
        }

        public virtual void SetEngine(Identifier engine)
        {
            this.engine = engine;
        }

        public virtual Expr GetAutoIncrement()
        {
            return autoIncrement;
        }

        public virtual void SetAutoIncrement(Expr autoIncrement)
        {
            this.autoIncrement = autoIncrement;
        }

        public virtual Expr GetAvgRowLength()
        {
            return avgRowLength;
        }

        public virtual void SetAvgRowLength(Expr avgRowLength)
        {
            this.avgRowLength = avgRowLength;
        }

        public virtual Identifier GetCharSet()
        {
            return charSet;
        }

        public virtual void SetCharSet(Identifier charSet)
        {
            this.charSet = charSet;
        }

        public virtual Identifier GetCollation()
        {
            return collation;
        }

        public virtual void SetCollation(Identifier collation)
        {
            this.collation = collation;
        }

        public virtual bool GetCheckSum()
        {
            return checkSum;
        }

        public virtual void SetCheckSum(bool checkSum)
        {
            this.checkSum = checkSum;
        }

        public virtual LiteralString GetComment()
        {
            return comment;
        }

        public virtual void SetComment(LiteralString comment)
        {
            this.comment = comment;
        }

        public virtual LiteralString GetConnection()
        {
            return connection;
        }

        public virtual void SetConnection(LiteralString connection)
        {
            this.connection = connection;
        }

        public virtual LiteralString GetDataDir()
        {
            return dataDir;
        }

        public virtual void SetDataDir(LiteralString dataDir)
        {
            this.dataDir = dataDir;
        }

        public virtual LiteralString GetIndexDir()
        {
            return indexDir;
        }

        public virtual void SetIndexDir(LiteralString indexDir)
        {
            this.indexDir = indexDir;
        }

        public virtual bool GetDelayKeyWrite()
        {
            return delayKeyWrite;
        }

        public virtual void SetDelayKeyWrite(bool delayKeyWrite)
        {
            this.delayKeyWrite = delayKeyWrite;
        }

        public virtual TableOptions.InsertMethod GetInsertMethod()
        {
            return insertMethod;
        }

        public virtual void SetInsertMethod(TableOptions.InsertMethod insertMethod)
        {
            this.insertMethod = insertMethod;
        }

        public virtual Expr GetKeyBlockSize()
        {
            return keyBlockSize;
        }

        public virtual void SetKeyBlockSize(Expr keyBlockSize
            )
        {
            this.keyBlockSize = keyBlockSize;
        }

        public virtual Expr GetMaxRows()
        {
            return maxRows;
        }

        public virtual void SetMaxRows(Expr maxRows)
        {
            this.maxRows = maxRows;
        }

        public virtual Expr GetMinRows()
        {
            return minRows;
        }

        public virtual void SetMinRows(Expr minRows)
        {
            this.minRows = minRows;
        }

        public virtual TableOptions.PackKeys GetPackKeys()
        {
            return packKeys;
        }

        public virtual void SetPackKeys(TableOptions.PackKeys packKeys)
        {
            this.packKeys = packKeys;
        }

        public virtual LiteralString GetPassword()
        {
            return password;
        }

        public virtual void SetPassword(LiteralString password)
        {
            this.password = password;
        }

        public virtual TableOptions.RowFormat GetRowFormat()
        {
            return rowFormat;
        }

        public virtual void SetRowFormat(TableOptions.RowFormat rowFormat)
        {
            this.rowFormat = rowFormat;
        }

        public virtual IList<Identifier> GetUnion()
        {
            return union;
        }

        public virtual void SetUnion(IList<Identifier> union)
        {
            this.union = union;
        }

        public virtual void Accept(SQLASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
