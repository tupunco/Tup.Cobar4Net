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
using Tup.Cobar.Parser.Ast.Stmt.Dml;
using Tup.Cobar.Parser.Util;
using Tup.Cobar.Parser.Visitor;

namespace Tup.Cobar.Parser.Ast.Stmt.Ddl
{
    /// <summary>NOT FULL AST: foreign key, ...</summary>
    /// <remarks>NOT FULL AST: foreign key, ... not supported</remarks>
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class DDLCreateTableStatement : DDLStatement
    {
        public enum SelectOption
        {
            Ignored,
            Replace
        }

        private readonly bool temporary;

        private readonly bool ifNotExists;

        private readonly Identifier table;

        private readonly IList<Pair<Identifier, ColumnDefinition>> colDefs;

        private IndexDefinition primaryKey;

        private readonly IList<Pair<Identifier, IndexDefinition>> uniqueKeys;

        private readonly IList<Pair<Identifier, IndexDefinition>> keys;

        private readonly IList<Pair<Identifier, IndexDefinition>> fullTextKeys;

        private readonly IList<Pair<Identifier, IndexDefinition>> spatialKeys;

        private readonly IList<Tup.Cobar.Parser.Ast.Expression.Expression> checks;

        private TableOptions tableOptions;

        private Pair<DDLCreateTableStatement.SelectOption, DMLSelectStatement> select;

        public DDLCreateTableStatement(bool temporary,
            bool ifNotExists,
            Identifier table)
        {
            this.table = table;
            this.temporary = temporary;
            this.ifNotExists = ifNotExists;
            this.colDefs = new List<Pair<Identifier, ColumnDefinition>>(4);
            this.uniqueKeys = new List<Pair<Identifier, IndexDefinition>>(1);
            this.keys = new List<Pair<Identifier, IndexDefinition>>(2);
            this.fullTextKeys = new List<Pair<Identifier, IndexDefinition>>(1);
            this.spatialKeys = new List<Pair<Identifier, IndexDefinition>>(1);
            this.checks = new List<Tup.Cobar.Parser.Ast.Expression.Expression>(1);
        }

        public virtual Tup.Cobar.Parser.Ast.Stmt.Ddl.DDLCreateTableStatement SetTableOptions
            (TableOptions tableOptions)
        {
            this.tableOptions = tableOptions;
            return this;
        }

        public virtual Tup.Cobar.Parser.Ast.Stmt.Ddl.DDLCreateTableStatement AddColumnDefinition
            (Identifier colname, ColumnDefinition def)
        {
            colDefs.Add(new Pair<Identifier, ColumnDefinition>(colname, def));
            return this;
        }

        public virtual Tup.Cobar.Parser.Ast.Stmt.Ddl.DDLCreateTableStatement SetPrimaryKey
            (IndexDefinition def)
        {
            primaryKey = def;
            return this;
        }

        public virtual Tup.Cobar.Parser.Ast.Stmt.Ddl.DDLCreateTableStatement AddUniqueIndex
            (Identifier colname, IndexDefinition def)
        {
            uniqueKeys.Add(new Pair<Identifier, IndexDefinition>(colname, def));
            return this;
        }

        public virtual Tup.Cobar.Parser.Ast.Stmt.Ddl.DDLCreateTableStatement AddIndex(Identifier
             colname, IndexDefinition def)
        {
            keys.Add(new Pair<Identifier, IndexDefinition>(colname, def));
            return this;
        }

        public virtual Tup.Cobar.Parser.Ast.Stmt.Ddl.DDLCreateTableStatement AddFullTextIndex
            (Identifier colname, IndexDefinition def)
        {
            fullTextKeys.Add(new Pair<Identifier, IndexDefinition>(colname, def));
            return this;
        }

        public virtual Tup.Cobar.Parser.Ast.Stmt.Ddl.DDLCreateTableStatement AddSpatialIndex
            (Identifier colname, IndexDefinition def)
        {
            spatialKeys.Add(new Pair<Identifier, IndexDefinition>(colname, def));
            return this;
        }

        public virtual Tup.Cobar.Parser.Ast.Stmt.Ddl.DDLCreateTableStatement AddCheck(
            Tup.Cobar.Parser.Ast.Expression.Expression check)
        {
            checks.Add(check);
            return this;
        }

        public virtual TableOptions GetTableOptions()
        {
            return tableOptions;
        }

        public virtual Pair<DDLCreateTableStatement.SelectOption, DMLSelectStatement> GetSelect
            ()
        {
            return select;
        }

        public virtual void SetSelect(DDLCreateTableStatement.SelectOption option,
            DMLSelectStatement select)
        {
            this.select = new Pair<DDLCreateTableStatement.SelectOption, DMLSelectStatement>(option, select);
        }

        public virtual bool IsTemporary()
        {
            return temporary;
        }

        public virtual bool IsIfNotExists()
        {
            return ifNotExists;
        }

        public virtual Identifier GetTable()
        {
            return table;
        }

        /// <returns>key := columnName</returns>
        public virtual IList<Pair<Identifier, ColumnDefinition>> GetColDefs()
        {
            return colDefs;
        }

        public virtual IndexDefinition GetPrimaryKey()
        {
            return primaryKey;
        }

        public virtual IList<Pair<Identifier, IndexDefinition>> GetUniqueKeys()
        {
            return uniqueKeys;
        }

        public virtual IList<Pair<Identifier, IndexDefinition>> GetKeys()
        {
            return keys;
        }

        public virtual IList<Pair<Identifier, IndexDefinition>> GetFullTextKeys()
        {
            return fullTextKeys;
        }

        public virtual IList<Pair<Identifier, IndexDefinition>> GetSpatialKeys()
        {
            return spatialKeys;
        }

        public virtual IList<Tup.Cobar.Parser.Ast.Expression.Expression> GetChecks()
        {
            return checks;
        }

        public virtual void Accept(SQLASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}