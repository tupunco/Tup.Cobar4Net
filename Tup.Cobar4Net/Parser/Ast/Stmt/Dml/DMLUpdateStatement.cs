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
using Tup.Cobar4Net.Parser.Ast.Fragment;
using Tup.Cobar4Net.Parser.Ast.Fragment.Tableref;
using Tup.Cobar4Net.Parser.Util;
using Tup.Cobar4Net.Parser.Visitor;

namespace Tup.Cobar4Net.Parser.Ast.Stmt.Dml
{
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public class DmlUpdateStatement : DmlStatement
    {
        public DmlUpdateStatement(bool lowPriority,
            bool ignore,
            TableReferences tableRefs,
            IList<Pair<Identifier, IExpression>> values,
            IExpression where, OrderBy orderBy,
            Limit limit)
        {
            IsLowPriority = lowPriority;
            IsIgnore = ignore;
            if (tableRefs == null)
            {
                throw new ArgumentException("argument tableRefs is null for update stmt");
            }

            TableRefs = tableRefs;
            if (values == null || values.Count <= 0)
            {
                Values = new List<Pair<Identifier, IExpression>>(0);
            }
            else
            {
                if (!(values is List<Pair<Identifier, IExpression>>))
                {
                    Values = new List<Pair<Identifier, IExpression>>(values);
                }
                else
                {
                    Values = values;
                }
            }
            Where = where;
            OrderBy = orderBy;
            Limit = limit;
        }

        public virtual bool IsLowPriority { get; }

        public virtual bool IsIgnore { get; }

        public virtual TableReferences TableRefs { get; }

        public virtual IList<Pair<Identifier, IExpression>> Values { get; }

        public virtual IExpression Where { get; }

        public virtual OrderBy OrderBy { get; }

        public virtual Limit Limit { get; }

        public override void Accept(ISqlAstVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}