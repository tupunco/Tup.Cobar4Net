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
using Tup.Cobar4Net.Parser.Ast.Expression;
using Tup.Cobar4Net.Parser.Ast.Expression.Primary;
using Tup.Cobar4Net.Parser.Ast.Fragment.Ddl;
using Tup.Cobar4Net.Parser.Ast.Stmt.Dml;
using Tup.Cobar4Net.Parser.Util;
using Tup.Cobar4Net.Parser.Visitor;

namespace Tup.Cobar4Net.Parser.Ast.Stmt.Ddl
{
    /// <summary>
    ///     DdlCreateTableStatement CreateTableSelectOption
    /// </summary>
    public enum CreateTableSelectOption
    {
        None = 0,

        Ignored,
        Replace
    }

    /// <summary>NOT FULL AST: foreign key, ...</summary>
    /// <remarks>NOT FULL AST: foreign key, ... not supported</remarks>
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public class DdlCreateTableStatement : IDdlStatement
    {
        public DdlCreateTableStatement(bool temporary,
            bool ifNotExists,
            Identifier table)
        {
            Table = table;
            IsTemporary = temporary;
            IsIfNotExists = ifNotExists;
            ColDefs = new List<Pair<Identifier, ColumnDefinition>>(4);
            UniqueKeys = new List<Pair<Identifier, IndexDefinition>>(1);
            Keys = new List<Pair<Identifier, IndexDefinition>>(2);
            FullTextKeys = new List<Pair<Identifier, IndexDefinition>>(1);
            SpatialKeys = new List<Pair<Identifier, IndexDefinition>>(1);
            Checks = new List<IExpression>(1);
        }

        public virtual TableOptions TableOptions { get; private set; }

        public virtual Pair<CreateTableSelectOption, DmlSelectStatement> Select { get; private set; }

        public virtual bool IsTemporary { get; }

        public virtual bool IsIfNotExists { get; }

        public virtual Identifier Table { get; }

        /// <value>key := columnName</value>
        public virtual IList<Pair<Identifier, ColumnDefinition>> ColDefs { get; }

        public virtual IndexDefinition PrimaryKey { get; private set; }

        public virtual IList<Pair<Identifier, IndexDefinition>> UniqueKeys { get; }

        public virtual IList<Pair<Identifier, IndexDefinition>> Keys { get; }

        public virtual IList<Pair<Identifier, IndexDefinition>> FullTextKeys { get; }

        public virtual IList<Pair<Identifier, IndexDefinition>> SpatialKeys { get; }

        public virtual IList<IExpression> Checks { get; }

        public virtual void Accept(ISqlAstVisitor visitor)
        {
            visitor.Visit(this);
        }

        public virtual DdlCreateTableStatement SetTableOptions(TableOptions tableOptions)
        {
            this.TableOptions = tableOptions;
            return this;
        }

        public virtual DdlCreateTableStatement AddColumnDefinition(Identifier colname, ColumnDefinition def)
        {
            ColDefs.Add(new Pair<Identifier, ColumnDefinition>(colname, def));
            return this;
        }

        public virtual DdlCreateTableStatement SetPrimaryKey(IndexDefinition def)
        {
            PrimaryKey = def;
            return this;
        }

        public virtual DdlCreateTableStatement AddUniqueIndex(Identifier colname, IndexDefinition def)
        {
            UniqueKeys.Add(new Pair<Identifier, IndexDefinition>(colname, def));
            return this;
        }

        public virtual DdlCreateTableStatement AddIndex(Identifier colname, IndexDefinition def)
        {
            Keys.Add(new Pair<Identifier, IndexDefinition>(colname, def));
            return this;
        }

        public virtual DdlCreateTableStatement AddFullTextIndex(Identifier colname, IndexDefinition def)
        {
            FullTextKeys.Add(new Pair<Identifier, IndexDefinition>(colname, def));
            return this;
        }

        public virtual DdlCreateTableStatement AddSpatialIndex(Identifier colname, IndexDefinition def)
        {
            SpatialKeys.Add(new Pair<Identifier, IndexDefinition>(colname, def));
            return this;
        }

        public virtual DdlCreateTableStatement AddCheck(IExpression check)
        {
            Checks.Add(check);
            return this;
        }

        public virtual void SetSelect(CreateTableSelectOption option, DmlSelectStatement select)
        {
            this.Select = new Pair<CreateTableSelectOption, DmlSelectStatement>(option, select);
        }
    }
}