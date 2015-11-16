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

using Tup.Cobar4Net.Parser.Ast.Expression.Primary;
using Tup.Cobar4Net.Parser.Ast.Fragment;
using Tup.Cobar4Net.Parser.Ast.Fragment.Tableref;
using Tup.Cobar4Net.Parser.Util;
using Tup.Cobar4Net.Parser.Visitor;
using Expr = Tup.Cobar4Net.Parser.Ast.Expression.Expression;

namespace Tup.Cobar4Net.Parser.Ast.Stmt.Dml
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class DMLUpdateStatement : DMLStatement
    {
        private readonly bool lowPriority;

        private readonly bool ignore;

        private readonly TableReferences tableRefs;

        private readonly IList<Pair<Identifier, Expr>> values;

        private readonly Expr where;

        private readonly OrderBy orderBy;

        private readonly Limit limit;

        public DMLUpdateStatement(bool lowPriority,
            bool ignore,
            TableReferences tableRefs,
            IList<Pair<Identifier, Expr>> values,
            Expr where, OrderBy orderBy,
            Limit limit)
        {
            this.lowPriority = lowPriority;
            this.ignore = ignore;
            if (tableRefs == null)
            {
                throw new ArgumentException("argument tableRefs is null for update stmt");
            }

            this.tableRefs = tableRefs;
            if (values == null || values.Count <= 0)
            {
                this.values = new List<Pair<Identifier, Expr>>(0);
            }
            else
            {
                if (!(values is List<Pair<Identifier, Expr>>))
                {
                    this.values = new List<Pair<Identifier, Expr>>(values);
                }
                else
                {
                    this.values = values;
                }
            }
            this.where = where;
            this.orderBy = orderBy;
            this.limit = limit;
        }

        public virtual bool IsLowPriority()
        {
            return lowPriority;
        }

        public virtual bool IsIgnore()
        {
            return ignore;
        }

        public virtual TableReferences GetTableRefs()
        {
            return tableRefs;
        }

        public virtual IList<Pair<Identifier, Expr>> GetValues()
        {
            return values;
        }

        public virtual Expr GetWhere()
        {
            return where;
        }

        public virtual OrderBy GetOrderBy()
        {
            return orderBy;
        }

        public virtual Limit GetLimit()
        {
            return limit;
        }

        public override void Accept(SQLASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}