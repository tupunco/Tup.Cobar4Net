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
using Tup.Cobar4Net.Parser.Visitor;

namespace Tup.Cobar4Net.Parser.Ast.Stmt.Dml
{
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public class DmlDeleteStatement : DmlStatement
    {
        /// <summary>tableName[.*]</summary>
        private readonly IList<Identifier> tableNames;

        /// <exception cref="System.SqlSyntaxErrorException" />
        public DmlDeleteStatement(bool lowPriority,
                                  bool quick,
                                  bool ignore,
                                  Identifier tableName)
            : this(lowPriority, quick, ignore, tableName, null, null, null)
        {
        }

        /// <exception cref="System.SqlSyntaxErrorException" />
        public DmlDeleteStatement(bool lowPriority,
                                  bool quick,
                                  bool ignore,
                                  Identifier tableName,
                                  IExpression where)
            : this(lowPriority, quick, ignore, tableName, where, null, null)
        {
        }

        /// <exception cref="System.SqlSyntaxErrorException" />
        public DmlDeleteStatement(bool lowPriority,
                                  bool quick,
                                  bool ignore,
                                  Identifier tableName,
                                  IExpression where,
                                  OrderBy orderBy,
                                  Limit limit)
        {
            // ------- single-row delete------------
            IsLowPriority = lowPriority;
            IsQuick = quick;
            IsIgnore = ignore;
            tableNames = new List<Identifier>(1);
            tableNames.Add(tableName);
            TableRefs = null;
            WhereCondition = where;
            OrderBy = orderBy;
            Limit = limit;
        }

        /// <exception cref="System.SqlSyntaxErrorException" />
        public DmlDeleteStatement(bool lowPriority,
                                  bool quick,
                                  bool ignore,
                                  IList<Identifier> tableNameList,
                                  TableReferences tableRefs)
            : this(lowPriority, quick, ignore, tableNameList, tableRefs, null)
        {
        }

        /// <exception cref="System.SqlSyntaxErrorException" />
        public DmlDeleteStatement(bool lowPriority,
                                  bool quick,
                                  bool ignore,
                                  IList<Identifier> tableNameList,
                                  TableReferences tableRefs, IExpression whereCondition)
        {
            // ------- multi-row delete------------
            IsLowPriority = lowPriority;
            IsQuick = quick;
            IsIgnore = ignore;
            if (tableNameList == null || tableNameList.IsEmpty())
            {
                throw new ArgumentException("argument 'tableNameList' is empty");
            }
            if (tableNameList is List<Identifier>)
            {
                tableNames = tableNameList;
            }
            else
            {
                tableNames = new List<Identifier>(tableNameList);
            }
            if (tableRefs == null)
            {
                throw new ArgumentException("argument 'tableRefs' is null");
            }
            TableRefs = tableRefs;
            WhereCondition = whereCondition;
            OrderBy = null;
            Limit = null;
        }

        public virtual IList<Identifier> TableNames
        {
            get { return tableNames; }
        }

        public virtual TableReferences TableRefs { get; }

        public virtual IExpression WhereCondition { get; }

        public virtual OrderBy OrderBy { get; }

        public virtual Limit Limit { get; }

        public virtual bool IsLowPriority { get; }

        public virtual bool IsQuick { get; }

        public virtual bool IsIgnore { get; }

        public override void Accept(ISqlAstVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}