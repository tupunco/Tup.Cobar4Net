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
using Tup.Cobar.Parser.Ast.Expression.Primary;
using Tup.Cobar.Parser.Ast.Fragment.Ddl;
using Tup.Cobar.Parser.Ast.Fragment.Ddl.Index;
using Tup.Cobar.Parser.Util;
using Tup.Cobar.Parser.Visitor;

namespace Tup.Cobar.Parser.Ast.Stmt.Ddl
{
    /// <summary>NOT FULL AST: partition options, foreign key, ORDER BY not supported</summary>
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class DDLAlterTableStatement : DDLStatement
    {
        public interface AlterSpecification : ASTNode
        {
        }

        public class AddColumn : AlterSpecification
        {
            private readonly Identifier columnName;

            private readonly ColumnDefinition columnDefine;

            private readonly bool first;

            private readonly Identifier afterColumn;

            /// <param name="columnName"/>
            /// <param name="columnDefine"/>
            /// <param name="afterColumn">null means fisrt</param>
            public AddColumn(Identifier columnName,
                ColumnDefinition columnDefine,
                Identifier afterColumn)
            {
                // | ADD [COLUMN] col_name column_definition [FIRST | AFTER col_name ]
                this.columnName = columnName;
                this.columnDefine = columnDefine;
                this.afterColumn = afterColumn;
                this.first = afterColumn == null;
            }

            /// <param name="columnName"/>
            /// <param name="columnDefine"/>
            /// <param name="afterColumn">null means fisrt</param>
            public AddColumn(Identifier columnName,
                ColumnDefinition columnDefine)
            {
                this.columnName = columnName;
                this.columnDefine = columnDefine;
                this.afterColumn = null;
                this.first = false;
            }

            public virtual Identifier GetColumnName()
            {
                return columnName;
            }

            public virtual ColumnDefinition GetColumnDefine()
            {
                return columnDefine;
            }

            public virtual bool IsFirst()
            {
                return first;
            }

            public virtual Identifier GetAfterColumn()
            {
                return afterColumn;
            }

            public virtual void Accept(SQLASTVisitor visitor)
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
                this.columns = new List<Pair<Identifier, ColumnDefinition>>(2);
            }

            public virtual DDLAlterTableStatement.AddColumns AddColumn(Identifier name, ColumnDefinition
                 colDef)
            {
                this.columns.Add(new Pair<Identifier, ColumnDefinition>(name, colDef));
                return this;
            }

            public virtual IList<Pair<Identifier, ColumnDefinition>> GetColumns()
            {
                return columns;
            }

            public virtual void Accept(SQLASTVisitor visitor)
            {
                visitor.Visit(this);
            }
        }

        public class AddIndex : AlterSpecification
        {
            private readonly Identifier indexName;

            private readonly IndexDefinition indexDef;

            /// <param name="indexName"/>
            /// <param name="indexType"/>
            public AddIndex(Identifier indexName,
                IndexDefinition indexDef)
            {
                // | ADD {INDEX|KEY} [index_name] [index_type] (index_col_name,...)
                // [index_option] ...
                this.indexName = indexName;
                this.indexDef = indexDef;
            }

            public virtual Identifier GetIndexName()
            {
                return indexName;
            }

            public virtual IndexDefinition GetIndexDef()
            {
                return indexDef;
            }

            public virtual void Accept(SQLASTVisitor visitor)
            {
                visitor.Visit(this);
            }
        }

        public class AddPrimaryKey : AlterSpecification
        {
            private readonly IndexDefinition indexDef;

            public AddPrimaryKey(IndexDefinition indexDef)
            {
                // | ADD [CONSTRAINT [symbol]] PRIMARY KEY [index_type] (index_col_name,...)
                // [index_option] ...
                this.indexDef = indexDef;
            }

            public virtual IndexDefinition GetIndexDef()
            {
                return indexDef;
            }

            public virtual void Accept(SQLASTVisitor visitor)
            {
                visitor.Visit(this);
            }
        }

        public class AddUniqueKey : AlterSpecification
        {
            private readonly Identifier indexName;

            private readonly IndexDefinition indexDef;

            public AddUniqueKey(Identifier indexName,
                IndexDefinition indexDef)
            {
                // | ADD [CONSTRAINT [symbol]] UNIQUE [INDEX|KEY] [index_name] [index_type]
                // (index_col_name,...) [index_option] ...
                this.indexDef = indexDef;
                this.indexName = indexName;
            }

            public virtual Identifier GetIndexName()
            {
                return indexName;
            }

            public virtual IndexDefinition GetIndexDef()
            {
                return indexDef;
            }

            public virtual void Accept(SQLASTVisitor visitor)
            {
                visitor.Visit(this);
            }
        }

        public class AddFullTextIndex : AlterSpecification
        {
            private readonly Identifier indexName;

            private readonly IndexDefinition indexDef;

            public AddFullTextIndex(Identifier indexName,
                IndexDefinition indexDef)
            {
                // | ADD FULLTEXT [INDEX|KEY] [index_name] (index_col_name,...)
                // [index_option] ...
                this.indexDef = indexDef;
                this.indexName = indexName;
            }

            public virtual Identifier GetIndexName()
            {
                return indexName;
            }

            public virtual IndexDefinition GetIndexDef()
            {
                return indexDef;
            }

            public virtual void Accept(SQLASTVisitor visitor)
            {
                visitor.Visit(this);
            }
        }

        public class AddSpatialIndex : AlterSpecification
        {
            private readonly Identifier indexName;

            private readonly IndexDefinition indexDef;

            public AddSpatialIndex(Identifier indexName,
                IndexDefinition indexDef)
            {
                // | ADD SPATIAL [INDEX|KEY] [index_name] (index_col_name,...)
                // [index_option] ...
                this.indexDef = indexDef;
                this.indexName = indexName;
            }

            public virtual Identifier GetIndexName()
            {
                return indexName;
            }

            public virtual IndexDefinition GetIndexDef()
            {
                return indexDef;
            }

            public virtual void Accept(SQLASTVisitor visitor)
            {
                visitor.Visit(this);
            }
        }

        public class AlterColumnDefaultVal : AlterSpecification
        {
            private readonly Identifier columnName;

            private readonly Tup.Cobar.Parser.Ast.Expression.Expression defaultValue;

            private readonly bool dropDefault;

            /// <param name="columnName"/>
            /// <param name="defaultValue"/>
            public AlterColumnDefaultVal(Identifier columnName,
                Tup.Cobar.Parser.Ast.Expression.Expression defaultValue)
            {
                // | ALTER [COLUMN] col_name {SET DEFAULT literal | DROP DEFAULT}
                this.columnName = columnName;
                this.defaultValue = defaultValue;
                this.dropDefault = false;
            }

            /// <summary>DROP DEFAULT</summary>
            /// <param name="columnName"/>
            public AlterColumnDefaultVal(Identifier columnName)
            {
                this.columnName = columnName;
                this.defaultValue = null;
                this.dropDefault = true;
            }

            public virtual Identifier GetColumnName()
            {
                return columnName;
            }

            public virtual Tup.Cobar.Parser.Ast.Expression.Expression GetDefaultValue()
            {
                return defaultValue;
            }

            public virtual bool IsDropDefault()
            {
                return dropDefault;
            }

            public virtual void Accept(SQLASTVisitor visitor)
            {
                visitor.Visit(this);
            }
        }

        public class ChangeColumn : AlterSpecification
        {
            private readonly Identifier oldName;

            private readonly Identifier newName;

            private readonly ColumnDefinition colDef;

            private readonly bool first;

            private readonly Identifier afterColumn;

            public ChangeColumn(Identifier oldName,
                Identifier newName,
                ColumnDefinition colDef,
                Identifier afterColumn)
            {
                // | CHANGE [COLUMN] old_col_name new_col_name column_definition
                // [FIRST|AFTER col_name]
                this.oldName = oldName;
                this.newName = newName;
                this.colDef = colDef;
                this.first = afterColumn == null;
                this.afterColumn = afterColumn;
            }

            /// <summary>without column position specification</summary>
            public ChangeColumn(Identifier oldName,
                Identifier newName,
                ColumnDefinition colDef)
            {
                this.oldName = oldName;
                this.newName = newName;
                this.colDef = colDef;
                this.first = false;
                this.afterColumn = null;
            }

            public virtual Identifier GetOldName()
            {
                return oldName;
            }

            public virtual Identifier GetNewName()
            {
                return newName;
            }

            public virtual ColumnDefinition GetColDef()
            {
                return colDef;
            }

            public virtual bool IsFirst()
            {
                return first;
            }

            public virtual Identifier GetAfterColumn()
            {
                return afterColumn;
            }

            public virtual void Accept(SQLASTVisitor visitor)
            {
                visitor.Visit(this);
            }
        }

        public class ModifyColumn : AlterSpecification
        {
            private readonly Identifier colName;

            private readonly ColumnDefinition colDef;

            private readonly bool first;

            private readonly Identifier afterColumn;

            public ModifyColumn(Identifier colName,
                ColumnDefinition colDef,
                Identifier afterColumn)
            {
                // | MODIFY [COLUMN] col_name column_definition [FIRST | AFTER col_name]
                this.colName = colName;
                this.colDef = colDef;
                this.first = afterColumn == null;
                this.afterColumn = afterColumn;
            }

            /// <summary>without column position specification</summary>
            public ModifyColumn(Identifier colName,
                ColumnDefinition colDef)
            {
                this.colName = colName;
                this.colDef = colDef;
                this.first = false;
                this.afterColumn = null;
            }

            public virtual Identifier GetColName()
            {
                return colName;
            }

            public virtual ColumnDefinition GetColDef()
            {
                return colDef;
            }

            public virtual bool IsFirst()
            {
                return first;
            }

            public virtual Identifier GetAfterColumn()
            {
                return afterColumn;
            }

            public virtual void Accept(SQLASTVisitor visitor)
            {
                visitor.Visit(this);
            }
        }

        public class DropColumn : AlterSpecification
        {
            private readonly Identifier colName;

            public DropColumn(Identifier colName)
            {
                // | DROP [COLUMN] col_name
                this.colName = colName;
            }

            public virtual Identifier GetColName()
            {
                return colName;
            }

            public virtual void Accept(SQLASTVisitor visitor)
            {
                visitor.Visit(this);
            }
        }

        public class DropPrimaryKey : AlterSpecification
        {
            // | DROP PRIMARY KEY
            public virtual void Accept(SQLASTVisitor visitor)
            {
                visitor.Visit(this);
            }
        }

        public class DropIndex : AlterSpecification
        {
            private readonly Identifier indexName;

            public DropIndex(Identifier indexName)
            {
                // | DROP {INDEX|KEY} index_name
                this.indexName = indexName;
            }

            public virtual Identifier GetIndexName()
            {
                return indexName;
            }

            public virtual void Accept(SQLASTVisitor visitor)
            {
                visitor.Visit(this);
            }
        }

        private readonly bool ignore;

        private readonly Identifier table;

        private TableOptions tableOptions;

        private readonly IList<AlterSpecification> alters;

        private bool disableKeys;

        private bool enableKeys;

        private bool discardTableSpace;

        private bool importTableSpace;

        private Identifier renameTo;

        /// <summary>charsetName -&gt; collate</summary>
        private Pair<Identifier, Identifier> convertCharset;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Potential Code Quality Issues", "RECS0021:Warns about calls to virtual member functions occuring in the constructor", Justification = "<¹ÒÆð>")]
        public DDLAlterTableStatement(bool ignore, Identifier table)
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
            this.ignore = ignore;
            this.table = table;
            this.alters = new List<AlterSpecification>(1);
        }

        public virtual DDLAlterTableStatement AddAlterSpecification(AlterSpecification alter)
        {
            alters.Add(alter);
            return this;
        }

        public virtual bool IsDisableKeys()
        {
            return disableKeys;
        }

        public virtual void SetDisableKeys(bool disableKeys)
        {
            this.disableKeys = disableKeys;
        }

        public virtual bool IsEnableKeys()
        {
            return enableKeys;
        }

        public virtual void SetEnableKeys(bool enableKeys)
        {
            this.enableKeys = enableKeys;
        }

        public virtual bool IsDiscardTableSpace()
        {
            return discardTableSpace;
        }

        public virtual void SetDiscardTableSpace(bool discardTableSpace)
        {
            this.discardTableSpace = discardTableSpace;
        }

        public virtual bool IsImportTableSpace()
        {
            return importTableSpace;
        }

        public virtual void SetImportTableSpace(bool importTableSpace)
        {
            this.importTableSpace = importTableSpace;
        }

        public virtual Identifier GetRenameTo()
        {
            return renameTo;
        }

        public virtual void SetRenameTo(Identifier renameTo)
        {
            this.renameTo = renameTo;
        }

        public virtual Pair<Identifier, Identifier> GetConvertCharset()
        {
            return convertCharset;
        }

        public virtual void SetConvertCharset(Pair<Identifier, Identifier> convertCharset)
        {
            this.convertCharset = convertCharset;
        }

        public virtual IList<AlterSpecification> GetAlters()
        {
            return alters;
        }

        public virtual void SetTableOptions(TableOptions tableOptions)
        {
            this.tableOptions = tableOptions;
        }

        public virtual TableOptions GetTableOptions()
        {
            return tableOptions;
        }

        public virtual bool IsIgnore()
        {
            return ignore;
        }

        public virtual Identifier GetTable()
        {
            return table;
        }

        public virtual void Accept(SQLASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}