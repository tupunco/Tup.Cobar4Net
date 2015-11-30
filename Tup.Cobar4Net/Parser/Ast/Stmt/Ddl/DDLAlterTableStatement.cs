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
using System.Diagnostics.CodeAnalysis;

using Tup.Cobar4Net.Parser.Ast.Expression;
using Tup.Cobar4Net.Parser.Ast.Expression.Primary;
using Tup.Cobar4Net.Parser.Ast.Fragment.Ddl;
using Tup.Cobar4Net.Parser.Util;
using Tup.Cobar4Net.Parser.Visitor;

namespace Tup.Cobar4Net.Parser.Ast.Stmt.Ddl
{
    /// <summary>NOT FULL AST: partition options, foreign key, ORDER BY not supported</summary>
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public class DdlAlterTableStatement : IDdlStatement
    {
        [SuppressMessage("Potential Code Quality Issues", "RECS0021:Warns about calls to virtual member functions occuring in the constructor",
            Justification = "<¹ÒÆð>")]
        public DdlAlterTableStatement(bool ignore, Identifier table)
        {
            // | DISABLE KEYS
            // | ENABLE KEYS
            // | RENAME [TO] new_tbl_name
            // | ORDER BY col_name [, col_name] ...
            // | CONVERT TO CHARACTER SET charset_name [COLLATE collation_name]
            // | DISCARD TABLESPACE
            // | IMPORT TABLESPACE
            // /// | ADD [CONSTRAINT [symbol]] FOREIGN KEY [index_name]
            // (index_col_name,...) reference_definition
            // /// | DROP FOREIGN KEY fk_symbol
            // /// | ADD PARTITION (partition_definition)
            // /// | DROP PARTITION partition_names
            // /// | TRUNCATE PARTITION {partition_names | ALL }
            // /// | COALESCE PARTITION number
            // /// | REORGANIZE PARTITION partition_names INTO (partition_definitions)
            // /// | ANALYZE PARTITION {partition_names | ALL }
            // /// | CHECK PARTITION {partition_names | ALL }
            // /// | OPTIMIZE PARTITION {partition_names | ALL }
            // /// | REBUILD PARTITION {partition_names | ALL }
            // /// | REPAIR PARTITION {partition_names | ALL }
            // /// | REMOVE PARTITIONING
            // ADD, ALTER, DROP, and CHANGE can be multiple
            IsIgnore = ignore;
            Table = table;
            Alters = new List<AlterSpecification>(1);
        }

        public virtual bool DisableKeys { get; set; }

        public virtual bool EnableKeys { get; set; }

        public virtual bool DiscardTableSpace { get; set; }

        public virtual bool IsImportTableSpace { get; set; }

        public virtual Identifier RenameTo { get; set; }

        public virtual Pair<Identifier, Identifier> ConvertCharset { get; set; }

        public virtual IList<AlterSpecification> Alters { get; }

        public virtual TableOptions TableOptions { set; get; }

        public virtual bool IsIgnore { get; }

        public virtual Identifier Table { get; }

        public virtual void Accept(ISqlAstVisitor visitor)
        {
            visitor.Visit(this);
        }

        public virtual DdlAlterTableStatement AddAlterSpecification(AlterSpecification alter)
        {
            Alters.Add(alter);
            return this;
        }

        public interface AlterSpecification : IAstNode
        {
        }

        public class AddColumn : AlterSpecification
        {
            /// <param name="columnName" />
            /// <param name="columnDefine" />
            /// <param name="afterColumn">null means fisrt</param>
            public AddColumn(Identifier columnName,
                ColumnDefinition columnDefine,
                Identifier afterColumn)
            {
                // | ADD [COLUMN] col_name column_definition [FIRST | AFTER col_name ]
                ColumnName = columnName;
                ColumnDefine = columnDefine;
                AfterColumn = afterColumn;
                IsFirst = afterColumn == null;
            }

            /// <param name="columnName" />
            /// <param name="columnDefine" />
            /// <param name="afterColumn">null means fisrt</param>
            public AddColumn(Identifier columnName,
                ColumnDefinition columnDefine)
            {
                ColumnName = columnName;
                ColumnDefine = columnDefine;
                AfterColumn = null;
                IsFirst = false;
            }

            public virtual Identifier ColumnName { get; }

            public virtual ColumnDefinition ColumnDefine { get; }

            public virtual bool IsFirst { get; }

            public virtual Identifier AfterColumn { get; }

            public virtual void Accept(ISqlAstVisitor visitor)
            {
                visitor.Visit(this);
            }
        }

        public class AddColumns : AlterSpecification
        {
            private readonly IList<Pair<Identifier, ColumnDefinition>> columns;

            public AddColumns()
            {
                // | ADD [COLUMN] (col_name column_definition,...)
                columns = new List<Pair<Identifier, ColumnDefinition>>(2);
            }

            public virtual IList<Pair<Identifier, ColumnDefinition>> Columns
            {
                get { return columns; }
            }

            public virtual void Accept(ISqlAstVisitor visitor)
            {
                visitor.Visit(this);
            }

            public virtual AddColumns AddColumn(Identifier name, ColumnDefinition colDef)
            {
                columns.Add(new Pair<Identifier, ColumnDefinition>(name, colDef));
                return this;
            }
        }

        public class AddIndex : AlterSpecification
        {
            /// <param name="indexName" />
            /// <param name="indexType" />
            public AddIndex(Identifier indexName,
                IndexDefinition indexDef)
            {
                // | ADD {INDEX|KEY} [index_name] [index_type] (index_col_name,...)
                // [index_option] ...
                IndexName = indexName;
                IndexDef = indexDef;
            }

            public virtual Identifier IndexName { get; }

            public virtual IndexDefinition IndexDef { get; }

            public virtual void Accept(ISqlAstVisitor visitor)
            {
                visitor.Visit(this);
            }
        }

        public class AddPrimaryKey : AlterSpecification
        {
            public AddPrimaryKey(IndexDefinition indexDef)
            {
                // | ADD [CONSTRAINT [symbol]] PRIMARY KEY [index_type] (index_col_name,...)
                // [index_option] ...
                IndexDef = indexDef;
            }

            public virtual IndexDefinition IndexDef { get; }

            public virtual void Accept(ISqlAstVisitor visitor)
            {
                visitor.Visit(this);
            }
        }

        public class AddUniqueKey : AlterSpecification
        {
            public AddUniqueKey(Identifier indexName,
                IndexDefinition indexDef)
            {
                // | ADD [CONSTRAINT [symbol]] UNIQUE [INDEX|KEY] [index_name] [index_type]
                // (index_col_name,...) [index_option] ...
                IndexDef = indexDef;
                IndexName = indexName;
            }

            public virtual Identifier IndexName { get; }

            public virtual IndexDefinition IndexDef { get; }

            public virtual void Accept(ISqlAstVisitor visitor)
            {
                visitor.Visit(this);
            }
        }

        public class AddFullTextIndex : AlterSpecification
        {
            public AddFullTextIndex(Identifier indexName,
                IndexDefinition indexDef)
            {
                // | ADD FULLTEXT [INDEX|KEY] [index_name] (index_col_name,...)
                // [index_option] ...
                IndexDef = indexDef;
                IndexName = indexName;
            }

            public virtual Identifier IndexName { get; }

            public virtual IndexDefinition IndexDef { get; }

            public virtual void Accept(ISqlAstVisitor visitor)
            {
                visitor.Visit(this);
            }
        }

        public class AddSpatialIndex : AlterSpecification
        {
            public AddSpatialIndex(Identifier indexName,
                IndexDefinition indexDef)
            {
                // | ADD SPATIAL [INDEX|KEY] [index_name] (index_col_name,...)
                // [index_option] ...
                IndexDef = indexDef;
                IndexName = indexName;
            }

            public virtual Identifier IndexName { get; }

            public virtual IndexDefinition IndexDef { get; }

            public virtual void Accept(ISqlAstVisitor visitor)
            {
                visitor.Visit(this);
            }
        }

        public class AlterColumnDefaultVal : AlterSpecification
        {
            /// <param name="columnName" />
            /// <param name="defaultValue" />
            public AlterColumnDefaultVal(Identifier columnName,
                IExpression defaultValue)
            {
                // | ALTER [COLUMN] col_name {SET DEFAULT literal | DROP DEFAULT}
                ColumnName = columnName;
                DefaultValue = defaultValue;
                IsDropDefault = false;
            }

            /// <summary>DROP DEFAULT</summary>
            /// <param name="columnName" />
            public AlterColumnDefaultVal(Identifier columnName)
            {
                ColumnName = columnName;
                DefaultValue = null;
                IsDropDefault = true;
            }

            public virtual Identifier ColumnName { get; }

            public virtual IExpression DefaultValue { get; }

            public virtual bool IsDropDefault { get; }

            public virtual void Accept(ISqlAstVisitor visitor)
            {
                visitor.Visit(this);
            }
        }

        public class ChangeColumn : AlterSpecification
        {
            public ChangeColumn(Identifier oldName,
                Identifier newName,
                ColumnDefinition colDef,
                Identifier afterColumn)
            {
                // | CHANGE [COLUMN] old_col_name new_col_name column_definition
                // [FIRST|AFTER col_name]
                OldName = oldName;
                NewName = newName;
                ColDef = colDef;
                IsFirst = afterColumn == null;
                AfterColumn = afterColumn;
            }

            /// <summary>without column position specification</summary>
            public ChangeColumn(Identifier oldName,
                Identifier newName,
                ColumnDefinition colDef)
            {
                OldName = oldName;
                NewName = newName;
                ColDef = colDef;
                IsFirst = false;
                AfterColumn = null;
            }

            public virtual Identifier OldName { get; }

            public virtual Identifier NewName { get; }

            public virtual ColumnDefinition ColDef { get; }

            public virtual bool IsFirst { get; }

            public virtual Identifier AfterColumn { get; }

            public virtual void Accept(ISqlAstVisitor visitor)
            {
                visitor.Visit(this);
            }
        }

        public class ModifyColumn : AlterSpecification
        {
            public ModifyColumn(Identifier colName,
                ColumnDefinition colDef,
                Identifier afterColumn)
            {
                // | MODIFY [COLUMN] col_name column_definition [FIRST | AFTER col_name]
                ColName = colName;
                ColDef = colDef;
                IsFirst = afterColumn == null;
                AfterColumn = afterColumn;
            }

            /// <summary>without column position specification</summary>
            public ModifyColumn(Identifier colName,
                ColumnDefinition colDef)
            {
                ColName = colName;
                ColDef = colDef;
                IsFirst = false;
                AfterColumn = null;
            }

            public virtual Identifier ColName { get; }

            public virtual ColumnDefinition ColDef { get; }

            public virtual bool IsFirst { get; }

            public virtual Identifier AfterColumn { get; }

            public virtual void Accept(ISqlAstVisitor visitor)
            {
                visitor.Visit(this);
            }
        }

        public class DropColumn : AlterSpecification
        {
            public DropColumn(Identifier colName)
            {
                // | DROP [COLUMN] col_name
                ColName = colName;
            }

            public virtual Identifier ColName { get; }

            public virtual void Accept(ISqlAstVisitor visitor)
            {
                visitor.Visit(this);
            }
        }

        public class DropPrimaryKey : AlterSpecification
        {
            // | DROP PRIMARY KEY
            public virtual void Accept(ISqlAstVisitor visitor)
            {
                visitor.Visit(this);
            }
        }

        public class DropIndex : AlterSpecification
        {
            public DropIndex(Identifier indexName)
            {
                // | DROP {INDEX|KEY} index_name
                IndexName = indexName;
            }

            public virtual Identifier IndexName { get; }

            public virtual void Accept(ISqlAstVisitor visitor)
            {
                visitor.Visit(this);
            }
        }
    }
}