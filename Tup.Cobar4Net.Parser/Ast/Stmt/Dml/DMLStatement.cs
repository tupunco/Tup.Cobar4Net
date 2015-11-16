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
using System.Text;
using Tup.Cobar4Net.Parser.Visitor;
using Expr = Tup.Cobar4Net.Parser.Ast.Expression.Expression;

namespace Tup.Cobar4Net.Parser.Ast.Stmt.Dml
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public abstract class DMLStatement : SQLStatement
    {
        protected static IList<TItem> EnsureListType<TItem>(IList<TItem> list)
        {
            if (list == null || list.Count <= 0)
            {
                return null;
            }
            if (list is List<TItem>)
            {
                return list;
            }
            return new List<TItem>(list);
        }

        protected static IList<IList<Expr>> CheckAndConvertValuesList(IList<IList<Expr>> valuesList)
        {
            if (valuesList == null || valuesList.IsEmpty())
            {
                throw new ArgumentException("argument 'valuesList' is empty");
            }
            var rst = (valuesList is List<IList<Expr>>) ? valuesList : new List<IList<Expr>>(valuesList.Count);
            bool copy = rst != valuesList;
            int size = -1;
            if (copy)
            {
                foreach (IList<Expr> values in valuesList)
                {
                    if (values == null || values.Count <= 0)
                    {
                        throw new ArgumentException("argument 'valuesList' contains empty element");
                    }
                    if (size < 0)
                    {
                        size = values.Count;
                    }
                    else
                    {
                        if (size != values.Count)
                        {
                            throw new ArgumentException("argument 'valuesList' contains empty elements with different size: "
                                 + size + " != " + values.Count);
                        }
                    }
                    rst.Add(EnsureListType(values));
                }
                return rst;
            }
            for (int i = 0; i < valuesList.Count; ++i)
            {
                var values = valuesList[i];
                if (values == null || values.Count <= 0)
                {
                    throw new ArgumentException("argument 'valuesList' contains empty element");
                }
                if (size < 0)
                {
                    size = values.Count;
                }
                else
                {
                    if (size != values.Count)
                    {
                        throw new ArgumentException("argument 'valuesList' contains empty elements with different size: "
                             + size + " != " + values.Count);
                    }
                }
                if (!(values is List<Expr>))
                {
                    valuesList[i] = new List<Expr>(values);
                }
            }
            return rst;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            Accept(new MySQLOutputASTVisitor(sb));
            return sb.ToString();
        }

        public abstract void Accept(SQLASTVisitor visitor);
    }
}