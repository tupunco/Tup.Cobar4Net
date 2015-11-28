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
using Tup.Cobar4Net.Parser.Visitor;
using Expr = Tup.Cobar4Net.Parser.Ast.Expression.Expression;

namespace Tup.Cobar4Net.Parser.Ast.Stmt.Dml
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class DMLDeleteStatement : DMLStatement
    {
        private readonly bool lowPriority;

        private readonly bool quick;

        private readonly bool ignore;

        /// <summary>tableName[.*]</summary>
        private readonly IList<Identifier> tableNames;

        private readonly TableReferences tableRefs;

        private readonly Expr whereCondition;

        private readonly OrderBy orderBy;

        private readonly Limit limit;

        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        public DMLDeleteStatement(bool lowPriority,
            bool quick,
            bool ignore,
            Identifier tableName)
            : this(lowPriority, quick, ignore, tableName, null, null, null)
        {
        }

        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        public DMLDeleteStatement(bool lowPriority,
            bool quick,
            bool ignore,
            Identifier tableName,
            Expr where)
            : this(lowPriority, quick, ignore, tableName, where, null, null)
        {
        }

        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        public DMLDeleteStatement(bool lowPriority,
            bool quick,
            bool ignore,
            Identifier tableName,
            Expr where,
            OrderBy orderBy,
            Limit limit)
        {
            // ------- single-row delete------------
            this.lowPriority = lowPriority;
            this.quick = quick;
            this.ignore = ignore;
            this.tableNames = new List<Identifier>(1);
            this.tableNames.Add(tableName);
            this.tableRefs = null;
            this.whereCondition = where;
            this.orderBy = orderBy;
            this.limit = limit;
        }

        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        public DMLDeleteStatement(bool lowPriority,
            bool quick,
            bool ignore,
            IList<Identifier> tableNameList,
            TableReferences tableRefs)
            : this(lowPriority, quick, ignore, tableNameList, tableRefs, null)
        {
        }

        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        public DMLDeleteStatement(bool lowPriority,
            bool quick,
            bool ignore,
            IList<Identifier> tableNameList,
            TableReferences tableRefs, Expr whereCondition)
        {
            // ------- multi-row delete------------
            this.lowPriority = lowPriority;
            this.quick = quick;
            this.ignore = ignore;
            if (tableNameList == null || tableNameList.IsEmpty())
            {
                throw new ArgumentException("argument 'tableNameList' is empty");
            }
            else
            {
                if (tableNameList is List<Identifier>)
                {
                    this.tableNames = tableNameList;
                }
                else
                {
                    this.tableNames = new List<Identifier>(tableNameList);
                }
            }
            if (tableRefs == null)
            {
                throw new ArgumentException("argument 'tableRefs' is null");
            }
            this.tableRefs = tableRefs;
            this.whereCondition = whereCondition;
            this.orderBy = null;
            this.limit = null;
        }

        public virtual IList<Identifier> GetTableNames()
        {
            return tableNames;
        }

        public virtual TableReferences GetTableRefs()
        {
            return tableRefs;
        }

        public virtual Expr GetWhereCondition()
        {
            return whereCondition;
        }

        public virtual OrderBy GetOrderBy()
        {
            return orderBy;
        }

        public virtual Limit GetLimit()
        {
            return limit;
        }

        public virtual bool IsLowPriority()
        {
            return lowPriority;
        }

        public virtual bool IsQuick()
        {
            return quick;
        }

        public virtual bool IsIgnore()
        {
            return ignore;
        }

        public override void Accept(SQLASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}