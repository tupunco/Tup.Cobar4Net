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

using Tup.Cobar4Net.Parser.Ast;
using Tup.Cobar4Net.Parser.Ast.Expression;
using Tup.Cobar4Net.Parser.Ast.Expression.Comparison;
using Tup.Cobar4Net.Parser.Ast.Expression.Logical;
using Tup.Cobar4Net.Parser.Ast.Expression.Misc;
using Tup.Cobar4Net.Parser.Ast.Expression.Primary;
using Tup.Cobar4Net.Parser.Ast.Expression.Primary.Function;
using Tup.Cobar4Net.Parser.Ast.Expression.Primary.Function.Cast;
using Tup.Cobar4Net.Parser.Ast.Expression.Primary.Function.Datetime;
using Tup.Cobar4Net.Parser.Ast.Expression.Primary.Function.Groupby;
using Tup.Cobar4Net.Parser.Ast.Expression.Primary.Function.String;
using Tup.Cobar4Net.Parser.Ast.Expression.Primary.Literal;
using Tup.Cobar4Net.Parser.Ast.Expression.String;
using Tup.Cobar4Net.Parser.Ast.Expression.Type;
using Tup.Cobar4Net.Parser.Ast.Fragment;
using Tup.Cobar4Net.Parser.Ast.Fragment.Ddl;
using Tup.Cobar4Net.Parser.Ast.Fragment.Ddl.Datatype;
using Tup.Cobar4Net.Parser.Ast.Fragment.Ddl.Index;
using Tup.Cobar4Net.Parser.Ast.Fragment.Tableref;
using Tup.Cobar4Net.Parser.Ast.Stmt.Dal;
using Tup.Cobar4Net.Parser.Ast.Stmt.Ddl;
using Tup.Cobar4Net.Parser.Ast.Stmt.Dml;
using Tup.Cobar4Net.Parser.Ast.Stmt.Extension;
using Tup.Cobar4Net.Parser.Ast.Stmt.Mts;
using Tup.Cobar4Net.Parser.Util;
using Expr = Tup.Cobar4Net.Parser.Ast.Expression.Expression;

namespace Tup.Cobar4Net.Parser.Visitor
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public sealed class MySQLOutputASTVisitor : SQLASTVisitor
    {
        private static readonly object[] EmptyObjArray = new object[0];

        private static readonly int[] EmptyIntArray = new int[0];

        private readonly StringBuilder appendable;

        private readonly object[] args;

        private int[] argsIndex;

        private IDictionary<PlaceHolder, object> placeHolderToString;

        public MySQLOutputASTVisitor(StringBuilder appendable)
            : this(appendable, null)
        {
        }

        /// <param name="args">
        /// parameters for
        /// <see cref="System.Data.Sql.PreparedStatement">preparedStmt</see>
        /// </param>
        public MySQLOutputASTVisitor(StringBuilder appendable, object[] args)
        {
            this.appendable = appendable;
            this.args = args == null ? EmptyObjArray : args;
            this.argsIndex = args == null ? EmptyIntArray : new int[args.Length];
        }

        public void SetPlaceHolderToString(IDictionary<PlaceHolder, object> map)
        {
            this.placeHolderToString = map;
        }

        public string GetSql()
        {
            return appendable.ToString();
        }

        /// <returns>
        /// never null. rst[i] â‰?
        /// <see cref="args"/>
        /// [
        /// <see cref="argsIndex"/>
        /// [i]]
        /// </returns>
        public object[] GetArguments()
        {
            int argsIndexSize = argsIndex.Length;
            if (argsIndexSize <= 0)
            {
                return EmptyObjArray;
            }
            bool noChange = true;
            for (int i = 0; i < argsIndexSize; ++i)
            {
                if (i != argsIndex[i])
                {
                    noChange = false;
                    break;
                }
            }
            if (noChange)
            {
                return args;
            }
            object[] rst = new object[argsIndexSize];
            for (int i_1 = 0; i_1 < argsIndexSize; ++i_1)
            {
                rst[i_1] = args[argsIndex[i_1]];
            }
            return rst;
        }

        /// <param name="list">never null</param>
        private void PrintList<TItem>(IList<TItem> list)
            where TItem : ASTNode
        {
            PrintList(list, ", ");
        }

        /// <param name="list">never null</param>
        private void PrintList<TItem>(IList<TItem> list, string sep)
            where TItem : ASTNode
        {
            bool isFst = true;
            foreach (ASTNode arg in list)
            {
                if (isFst)
                {
                    isFst = false;
                }
                else
                {
                    appendable.Append(sep);
                }
                arg.Accept(this);
            }
        }

        public void Visit(BetweenAndExpression node)
        {
            Expr comparee = node.GetFirst();
            bool paren = comparee.GetPrecedence() <= node.GetPrecedence();
            if (paren)
            {
                appendable.Append('(');
            }
            comparee.Accept(this);
            if (paren)
            {
                appendable.Append(')');
            }
            if (node.IsNot())
            {
                appendable.Append(" NOT BETWEEN ");
            }
            else
            {
                appendable.Append(" BETWEEN ");
            }
            Expr start = node.GetSecond();
            paren = start.GetPrecedence() < node.GetPrecedence();
            if (paren)
            {
                appendable.Append('(');
            }
            start.Accept(this);
            if (paren)
            {
                appendable.Append(')');
            }
            appendable.Append(" AND ");
            Expr end = node.GetThird();
            paren = end.GetPrecedence() < node.GetPrecedence();
            if (paren)
            {
                appendable.Append('(');
            }
            end.Accept(this);
            if (paren)
            {
                appendable.Append(')');
            }
        }

        public void Visit(ComparisionIsExpression node)
        {
            Expr comparee = node.GetOperand();
            bool paren = comparee.GetPrecedence() < node.GetPrecedence();
            if (paren)
            {
                appendable.Append('(');
            }
            comparee.Accept(this);
            if (paren)
            {
                appendable.Append(')');
            }
            switch (node.GetMode())
            {
                case ComparisionIsExpression.IsNull:
                    {
                        appendable.Append(" IS NULL");
                        break;
                    }

                case ComparisionIsExpression.IsTrue:
                    {
                        appendable.Append(" IS TRUE");
                        break;
                    }

                case ComparisionIsExpression.IsFalse:
                    {
                        appendable.Append(" IS FALSE");
                        break;
                    }

                case ComparisionIsExpression.IsUnknown:
                    {
                        appendable.Append(" IS UNKNOWN");
                        break;
                    }

                case ComparisionIsExpression.IsNotNull:
                    {
                        appendable.Append(" IS NOT NULL");
                        break;
                    }

                case ComparisionIsExpression.IsNotTrue:
                    {
                        appendable.Append(" IS NOT TRUE");
                        break;
                    }

                case ComparisionIsExpression.IsNotFalse:
                    {
                        appendable.Append(" IS NOT FALSE");
                        break;
                    }

                case ComparisionIsExpression.IsNotUnknown:
                    {
                        appendable.Append(" IS NOT UNKNOWN");
                        break;
                    }

                default:
                    {
                        throw new ArgumentException("unknown mode for IS expression: " + node.GetMode());
                    }
            }
        }

        public void Visit(InExpressionList node)
        {
            appendable.Append('(');
            PrintList(node.GetList());
            appendable.Append(')');
        }

        public void Visit(LikeExpression node)
        {
            Expr comparee = node.GetFirst();
            bool paren = comparee.GetPrecedence() < node.GetPrecedence();
            if (paren)
            {
                appendable.Append('(');
            }
            comparee.Accept(this);
            if (paren)
            {
                appendable.Append(')');
            }
            if (node.IsNot())
            {
                appendable.Append(" NOT LIKE ");
            }
            else
            {
                appendable.Append(" LIKE ");
            }
            Expr pattern = node.GetSecond();
            paren = pattern.GetPrecedence() <= node.GetPrecedence();
            if (paren)
            {
                appendable.Append('(');
            }
            pattern.Accept(this);
            if (paren)
            {
                appendable.Append(')');
            }
            Expr escape = node.GetThird();
            if (escape != null)
            {
                appendable.Append(" ESCAPE ");
                paren = escape.GetPrecedence() <= node.GetPrecedence();
                if (paren)
                {
                    appendable.Append('(');
                }
                escape.Accept(this);
                if (paren)
                {
                    appendable.Append(')');
                }
            }
        }

        public void Visit(CollateExpression node)
        {
            Expr @string = node.GetString();
            bool paren = @string.GetPrecedence() < node.GetPrecedence();
            if (paren)
            {
                appendable.Append('(');
            }
            @string.Accept(this);
            if (paren)
            {
                appendable.Append(')');
            }
            appendable.Append(" COLLATE ").Append(node.GetCollateName());
        }

        public void Visit(UserExpression node)
        {
            appendable.Append(node.GetUserAtHost());
        }

        public void Visit(UnaryOperatorExpression node)
        {
            appendable.Append(node.GetOperator()).Append(' ');
            bool paren = node.GetOperand().GetPrecedence() < node.GetPrecedence();
            if (paren)
            {
                appendable.Append('(');
            }
            node.GetOperand().Accept(this);
            if (paren)
            {
                appendable.Append(')');
            }
        }

        public void Visit(BinaryOperatorExpression node)
        {
            Expr left = node.GetLeftOprand();
            bool paren = node.IsLeftCombine()
                ? left.GetPrecedence() < node.GetPrecedence()
                : left.GetPrecedence() <= node.GetPrecedence();
            if (paren)
            {
                appendable.Append('(');
            }
            left.Accept(this);
            if (paren)
            {
                appendable.Append(')');
            }
            appendable.Append(' ').Append(node.GetOperator()).Append(' ');
            Expr right = node.GetRightOprand();
            paren = node.IsLeftCombine()
                ? right.GetPrecedence() <= node.GetPrecedence()
                : right.GetPrecedence() < node.GetPrecedence();
            if (paren)
            {
                appendable.Append('(');
            }
            right.Accept(this);
            if (paren)
            {
                appendable.Append(')');
            }
        }

        public void Visit(PolyadicOperatorExpression node)
        {
            for (int i = 0, len = node.GetArity(); i < len; ++i)
            {
                if (i > 0)
                {
                    appendable.Append(' ').Append(node.GetOperator()).Append(' ');
                }
                Expr operand = node.GetOperand(i);
                bool paren = operand.GetPrecedence() < node.GetPrecedence();
                if (paren)
                {
                    appendable.Append('(');
                }
                operand.Accept(this);
                if (paren)
                {
                    appendable.Append(')');
                }
            }
        }

        public void Visit(LogicalAndExpression node)
        {
            Visit((PolyadicOperatorExpression)node);
        }

        public void Visit(LogicalOrExpression node)
        {
            Visit((PolyadicOperatorExpression)node);
        }

        public void Visit(ComparisionEqualsExpression node)
        {
            Visit((BinaryOperatorExpression)node);
        }

        public void Visit(ComparisionNullSafeEqualsExpression node)
        {
            Visit((BinaryOperatorExpression)node);
        }

        public void Visit(InExpression node)
        {
            Visit((BinaryOperatorExpression)node);
        }

        public void Visit(FunctionExpression node)
        {
            string functionName = node.GetFunctionName();
            appendable.Append(functionName).Append('(');
            PrintList(node.GetArguments());
            appendable.Append(')');
        }

        public void Visit(Ast.Expression.Primary.Function.String.Char node)
        {
            string functionName = node.GetFunctionName();
            appendable.Append(functionName).Append('(');
            PrintList(node.GetArguments());
            string charset = node.GetCharset();
            if (charset != null)
            {
                appendable.Append(" USING ").Append(charset);
            }
            appendable.Append(')');
        }

        public void Visit(Ast.Expression.Primary.Function.Cast.Convert node)
        {
            string functionName = node.GetFunctionName();
            appendable.Append(functionName).Append('(');
            PrintList(node.GetArguments());
            string transcodeName = node.GetTranscodeName();
            appendable.Append(" USING ").Append(transcodeName);
            appendable.Append(')');
        }

        public void Visit(Trim node)
        {
            string functionName = node.GetFunctionName();
            appendable.Append(functionName).Append('(');
            Expr remStr = node.GetRemainString();
            switch (node.GetDirection())
            {
                case Trim.Direction.Default:
                    {
                        if (remStr != null)
                        {
                            remStr.Accept(this);
                            appendable.Append(" FROM ");
                        }
                        break;
                    }

                case Trim.Direction.Both:
                    {
                        appendable.Append("BOTH ");
                        if (remStr != null)
                        {
                            remStr.Accept(this);
                        }
                        appendable.Append(" FROM ");
                        break;
                    }

                case Trim.Direction.Leading:
                    {
                        appendable.Append("LEADING ");
                        if (remStr != null)
                        {
                            remStr.Accept(this);
                        }
                        appendable.Append(" FROM ");
                        break;
                    }

                case Trim.Direction.Trailing:
                    {
                        appendable.Append("TRAILING ");
                        if (remStr != null)
                        {
                            remStr.Accept(this);
                        }
                        appendable.Append(" FROM ");
                        break;
                    }

                default:
                    {
                        throw new ArgumentException("unknown trim direction: " + node.GetDirection());
                    }
            }
            Expr str = node.GetString();
            str.Accept(this);
            appendable.Append(')');
        }

        public void Visit(Cast node)
        {
            string functionName = node.GetFunctionName();
            appendable.Append(functionName).Append('(');
            node.GetExpr().Accept(this);
            appendable.Append(" AS ");
            string typeName = node.GetTypeName();
            appendable.Append(typeName);
            Expr info1 = node.GetTypeInfo1();
            if (info1 != null)
            {
                appendable.Append('(');
                info1.Accept(this);
                Expr info2 = node.GetTypeInfo2();
                if (info2 != null)
                {
                    appendable.Append(", ");
                    info2.Accept(this);
                }
                appendable.Append(')');
            }
            appendable.Append(')');
        }

        public void Visit(Avg node)
        {
            string functionName = node.GetFunctionName();
            appendable.Append(functionName).Append('(');
            if (node.IsDistinct())
            {
                appendable.Append("DISTINCT ");
            }
            PrintList(node.GetArguments());
            appendable.Append(')');
        }

        public void Visit(Max node)
        {
            string functionName = node.GetFunctionName();
            appendable.Append(functionName).Append('(');
            if (node.IsDistinct())
            {
                appendable.Append("DISTINCT ");
            }
            PrintList(node.GetArguments());
            appendable.Append(')');
        }

        public void Visit(Min node)
        {
            string functionName = node.GetFunctionName();
            appendable.Append(functionName).Append('(');
            if (node.IsDistinct())
            {
                appendable.Append("DISTINCT ");
            }
            PrintList(node.GetArguments());
            appendable.Append(')');
        }

        public void Visit(Sum node)
        {
            string functionName = node.GetFunctionName();
            appendable.Append(functionName).Append('(');
            if (node.IsDistinct())
            {
                appendable.Append("DISTINCT ");
            }
            PrintList(node.GetArguments());
            appendable.Append(')');
        }

        public void Visit(Count node)
        {
            string functionName = node.GetFunctionName();
            appendable.Append(functionName).Append('(');
            if (node.IsDistinct())
            {
                appendable.Append("DISTINCT ");
            }
            PrintList(node.GetArguments());
            appendable.Append(')');
        }

        public void Visit(GroupConcat node)
        {
            string functionName = node.GetFunctionName();
            appendable.Append(functionName).Append('(');
            if (node.IsDistinct())
            {
                appendable.Append("DISTINCT ");
            }
            PrintList(node.GetArguments());
            Expr orderBy = node.GetOrderBy();
            if (orderBy != null)
            {
                appendable.Append(" ORDER BY ");
                orderBy.Accept(this);
                if (node.IsDesc())
                {
                    appendable.Append(" DESC");
                }
                else
                {
                    appendable.Append(" ASC");
                }
                var list = node.GetAppendedColumnNames();
                if (list != null && !list.IsEmpty())
                {
                    appendable.Append(", ");
                    PrintList(list);
                }
            }
            string sep = node.GetSeparator();
            if (sep != null)
            {
                appendable.Append(" SEPARATOR ").Append(sep);
            }
            appendable.Append(')');
        }

        public void Visit(Extract node)
        {
            appendable.Append("EXTRACT(")
                .Append(node.GetUnit().GetEnumName())
                .Append(" FROM ");
            PrintList(node.GetArguments());
            appendable.Append(')');
        }

        public void Visit(Timestampdiff node)
        {
            appendable.Append("TIMESTAMPDIFF(")
                .Append(node.GetUnit().GetEnumName())
                .Append(", ");
            PrintList(node.GetArguments());
            appendable.Append(')');
        }

        public void Visit(Timestampadd node)
        {
            appendable.Append("TIMESTAMPADD(")
                .Append(node.GetUnit().GetEnumName())
                .Append(", ");
            PrintList(node.GetArguments());
            appendable.Append(')');
        }

        public void Visit(GetFormat node)
        {
            appendable.Append("GET_FORMAT(");
            GetFormat.FormatType type = node.GetFormatType();
            appendable.Append(type.ToString())
                      .Append(", ");
            PrintList(node.GetArguments());
            appendable.Append(')');
        }

        public void Visit(PlaceHolder node)
        {
            if (placeHolderToString == null)
            {
                appendable.Append("${")
                            .Append(node.GetName())
                            .Append('}');
                return;
            }
            object toStringer = placeHolderToString.GetValue(node);
            if (toStringer == null)
            {
                appendable.Append("${")
                            .Append(node.GetName())
                            .Append('}');
            }
            else
            {
                appendable.Append(toStringer.ToString());
            }
        }

        public void Visit(IntervalPrimary node)
        {
            appendable.Append("INTERVAL ");
            Expr quantity = node.GetQuantity();
            bool paren = quantity.GetPrecedence() < node.GetPrecedence();
            if (paren)
            {
                appendable.Append('(');
            }
            quantity.Accept(this);
            if (paren)
            {
                appendable.Append(')');
            }
            IntervalPrimary.Unit unit = node.GetUnit();
            appendable.Append(' ').Append(unit.GetEnumName());
        }

        public void Visit(LiteralBitField node)
        {
            string introducer = node.GetIntroducer();
            if (introducer != null)
            {
                appendable.Append(introducer)
                            .Append(' ');
            }
            appendable.Append("b'")
                        .Append(node.GetText())
                        .Append('\'');
        }

        public void Visit(LiteralBoolean node)
        {
            if (node.IsTrue())
            {
                appendable.Append("TRUE");
            }
            else
            {
                appendable.Append("FALSE");
            }
        }

        public void Visit(LiteralHexadecimal node)
        {
            string introducer = node.GetIntroducer();
            if (introducer != null)
            {
                appendable.Append(introducer).Append(' ');
            }
            appendable.Append("x'");
            node.AppendTo(appendable);
            appendable.Append('\'');
        }

        public void Visit(LiteralNull node)
        {
            appendable.Append("NULL");
        }

        public void Visit(LiteralNumber node)
        {
            appendable.Append(node.GetNumber().ToString());
        }

        public void Visit(LiteralString node)
        {
            string introducer = node.GetIntroducer();
            if (introducer != null)
            {
                appendable.Append(introducer);
            }
            else
            {
                if (node.IsNchars())
                {
                    appendable.Append('N');
                }
            }
            appendable.Append('\'').Append(node.GetString()).Append('\'');
        }

        public void Visit(CaseWhenOperatorExpression node)
        {
            appendable.Append("CASE");
            Expr comparee = node.GetComparee();
            if (comparee != null)
            {
                appendable.Append(' ');
                comparee.Accept(this);
            }
            var whenList = node.GetWhenList();
            foreach (var whenthen in whenList)
            {
                appendable.Append(" WHEN ");
                Expr when = whenthen.GetKey();
                when.Accept(this);
                appendable.Append(" THEN ");
                Expr then = whenthen.GetValue();
                then.Accept(this);
            }
            Expr elseRst = node.GetElseResult();
            if (elseRst != null)
            {
                appendable.Append(" ELSE ");
                elseRst.Accept(this);
            }
            appendable.Append(" END");
        }

        public void Visit(DefaultValue node)
        {
            appendable.Append("DEFAULT");
        }

        public void Visit(ExistsPrimary node)
        {
            appendable.Append("EXISTS (");
            node.GetSubquery().Accept(this);
            appendable.Append(')');
        }

        public void Visit(Identifier node)
        {
            Expr parent = node.GetParent();
            if (parent != null)
            {
                parent.Accept(this);
                appendable.Append('.');
            }
            appendable.Append(node.GetIdText());
        }

        private static bool ContainsCompIn(Expr pat)
        {
            if (pat.GetPrecedence() > ExpressionConstants.PrecedenceComparision)
            {
                return false;
            }
            if (pat is BinaryOperatorExpression)
            {
                if (pat is InExpression)
                {
                    return true;
                }
                BinaryOperatorExpression bp = (BinaryOperatorExpression)pat;
                if (bp.IsLeftCombine())
                {
                    return ContainsCompIn(bp.GetLeftOprand());
                }
                else
                {
                    return ContainsCompIn(bp.GetLeftOprand());
                }
            }
            else
            {
                if (pat is ComparisionIsExpression)
                {
                    ComparisionIsExpression @is = (ComparisionIsExpression)pat;
                    return ContainsCompIn(@is.GetOperand());
                }
                else
                {
                    if (pat is TernaryOperatorExpression)
                    {
                        TernaryOperatorExpression tp = (TernaryOperatorExpression)pat;
                        return ContainsCompIn(tp.GetFirst()) || ContainsCompIn(tp.GetSecond()) || ContainsCompIn
                            (tp.GetThird());
                    }
                    else
                    {
                        if (pat is UnaryOperatorExpression)
                        {
                            UnaryOperatorExpression up = (UnaryOperatorExpression)pat;
                            return ContainsCompIn(up.GetOperand());
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
        }

        public void Visit(MatchExpression node)
        {
            appendable.Append("MATCH (");
            PrintList(node.GetColumns());
            appendable.Append(") AGAINST (");
            Expr pattern = node.GetPattern();
            bool inparen = ContainsCompIn(pattern);
            if (inparen)
            {
                appendable.Append('(');
            }
            pattern.Accept(this);
            if (inparen)
            {
                appendable.Append(')');
            }
            switch (node.GetModifier())
            {
                case MatchExpression.Modifier.InBooleanMode:
                    {
                        appendable.Append(" IN BOOLEAN MODE");
                        break;
                    }

                case MatchExpression.Modifier.InNaturalLanguageMode:
                    {
                        appendable.Append(" IN NATURAL LANGUAGE MODE");
                        break;
                    }

                case MatchExpression.Modifier.InNaturalLanguageModeWithQueryExpansion:
                    {
                        appendable.Append(" IN NATURAL LANGUAGE MODE WITH QUERY EXPANSION");
                        break;
                    }

                case MatchExpression.Modifier.WithQueryExpansion:
                    {
                        appendable.Append(" WITH QUERY EXPANSION");
                        break;
                    }

                case MatchExpression.Modifier.Default:
                    {
                        break;
                    }

                default:
                    {
                        throw new ArgumentException("unkown modifier for match expression: " + node.GetModifier
                            ());
                    }
            }
            appendable.Append(')');
        }

        private int index = -1;

        private void AppendArgsIndex(int value)
        {
            int i = ++index;
            if (argsIndex.Length <= i)
            {
                int[] a = new int[i + 1];
                if (i > 0)
                {
                    System.Array.Copy(argsIndex, 0, a, 0, i);
                }
                argsIndex = a;
            }
            argsIndex[i] = value;
        }

        public void Visit(ParamMarker node)
        {
            appendable.Append('?');
            AppendArgsIndex(node.GetParamIndex() - 1);
        }

        public void Visit(RowExpression node)
        {
            appendable.Append("ROW(");
            PrintList(node.GetRowExprList());
            appendable.Append(')');
        }

        public void Visit(SysVarPrimary node)
        {
            VariableScope scope = node.GetScope();
            switch (scope)
            {
                case VariableScope.Global:
                    {
                        appendable.Append("@@global.");
                        break;
                    }

                case VariableScope.Session:
                    {
                        appendable.Append("@@");
                        break;
                    }

                default:
                    {
                        throw new ArgumentException("unkown scope for sysVar primary: " + scope);
                    }
            }
            appendable.Append(node.GetVarText());
        }

        public void Visit(UsrDefVarPrimary node)
        {
            appendable.Append(node.GetVarText());
        }

        public void Visit(IndexHint node)
        {
            IndexHint.IndexAction action = node.GetAction();
            switch (action)
            {
                case IndexHint.IndexAction.Force:
                    {
                        appendable.Append("FORCE ");
                        break;
                    }

                case IndexHint.IndexAction.Ignore:
                    {
                        appendable.Append("IGNORE ");
                        break;
                    }

                case IndexHint.IndexAction.Use:
                    {
                        appendable.Append("USE ");
                        break;
                    }

                default:
                    {
                        throw new ArgumentException("unkown index action for index hint: " + action);
                    }
            }
            IndexHint.IndexType type = node.GetIndexType();
            switch (type)
            {
                case IndexHint.IndexType.Index:
                    {
                        appendable.Append("INDEX ");
                        break;
                    }

                case IndexHint.IndexType.Key:
                    {
                        appendable.Append("KEY ");
                        break;
                    }

                default:
                    {
                        throw new ArgumentException("unkown index type for index hint: " + type);
                    }
            }
            IndexHint.IndexScope scope = node.GetScope();
            switch (scope)
            {
                case IndexHint.IndexScope.GroupBy:
                    {
                        appendable.Append("FOR GROUP BY ");
                        break;
                    }

                case IndexHint.IndexScope.OrderBy:
                    {
                        appendable.Append("FOR ORDER BY ");
                        break;
                    }

                case IndexHint.IndexScope.Join:
                    {
                        appendable.Append("FOR JOIN ");
                        break;
                    }

                case IndexHint.IndexScope.All:
                    {
                        break;
                    }

                default:
                    {
                        throw new ArgumentException("unkown index scope for index hint: " + scope);
                    }
            }
            appendable.Append('(');
            IList<string> indexList = node.GetIndexList();
            bool isFst = true;
            foreach (string indexName in indexList)
            {
                if (isFst)
                {
                    isFst = false;
                }
                else
                {
                    appendable.Append(", ");
                }
                appendable.Append(indexName);
            }
            appendable.Append(')');
        }

        public void Visit(TableReferences node)
        {
            PrintList(node.GetTableReferenceList());
        }

        public void Visit(InnerJoin node)
        {
            TableReference left = node.GetLeftTableRef();
            bool paren = left.GetPrecedence() < node.GetPrecedence();
            if (paren)
            {
                appendable.Append('(');
            }
            left.Accept(this);
            if (paren)
            {
                appendable.Append(')');
            }
            appendable.Append(" INNER JOIN ");
            TableReference right = node.GetRightTableRef();
            paren = right.GetPrecedence() <= node.GetPrecedence();
            if (paren)
            {
                appendable.Append('(');
            }
            right.Accept(this);
            if (paren)
            {
                appendable.Append(')');
            }
            Expr on = node.GetOnCond();
            IList<string> @using = node.GetUsing();
            if (on != null)
            {
                appendable.Append(" ON ");
                on.Accept(this);
            }
            else
            {
                if (@using != null)
                {
                    appendable.Append(" USING (");
                    bool isFst = true;
                    foreach (string col in @using)
                    {
                        if (isFst)
                        {
                            isFst = false;
                        }
                        else
                        {
                            appendable.Append(", ");
                        }
                        appendable.Append(col);
                    }
                    appendable.Append(")");
                }
            }
        }

        public void Visit(NaturalJoin node)
        {
            TableReference left = node.GetLeftTableRef();
            bool paren = left.GetPrecedence() < node.GetPrecedence();
            if (paren)
            {
                appendable.Append('(');
            }
            left.Accept(this);
            if (paren)
            {
                appendable.Append(')');
            }
            appendable.Append(" NATURAL ");
            if (node.IsOuter())
            {
                if (node.IsLeft())
                {
                    appendable.Append("LEFT ");
                }
                else
                {
                    appendable.Append("RIGHT ");
                }
            }
            appendable.Append("JOIN ");
            TableReference right = node.GetRightTableRef();
            paren = right.GetPrecedence() <= node.GetPrecedence();
            if (paren)
            {
                appendable.Append('(');
            }
            right.Accept(this);
            if (paren)
            {
                appendable.Append(')');
            }
        }

        public void Visit(StraightJoin node)
        {
            TableReference left = node.GetLeftTableRef();
            bool paren = left.GetPrecedence() < node.GetPrecedence();
            if (paren)
            {
                appendable.Append('(');
            }
            left.Accept(this);
            if (paren)
            {
                appendable.Append(')');
            }
            appendable.Append(" STRAIGHT_JOIN ");
            TableReference right = node.GetRightTableRef();
            paren = right.GetPrecedence() <= node.GetPrecedence();
            if (paren)
            {
                appendable.Append('(');
            }
            right.Accept(this);
            if (paren)
            {
                appendable.Append(')');
            }
            Expr on = node.GetOnCond();
            if (on != null)
            {
                appendable.Append(" ON ");
                on.Accept(this);
            }
        }

        public void Visit(OuterJoin node)
        {
            TableReference left = node.GetLeftTableRef();
            bool paren = left.GetPrecedence() < node.GetPrecedence();
            if (paren)
            {
                appendable.Append('(');
            }
            left.Accept(this);
            if (paren)
            {
                appendable.Append(')');
            }
            if (node.IsLeftJoin())
            {
                appendable.Append(" LEFT JOIN ");
            }
            else
            {
                appendable.Append(" RIGHT JOIN ");
            }
            TableReference right = node.GetRightTableRef();
            paren = right.GetPrecedence() <= node.GetPrecedence();
            if (paren)
            {
                appendable.Append('(');
            }
            right.Accept(this);
            if (paren)
            {
                appendable.Append(')');
            }
            Expr on = node.GetOnCond();
            IList<string> @using = node.GetUsing();
            if (on != null)
            {
                appendable.Append(" ON ");
                on.Accept(this);
            }
            else
            {
                if (@using != null)
                {
                    appendable.Append(" USING (");
                    bool isFst = true;
                    foreach (string col in @using)
                    {
                        if (isFst)
                        {
                            isFst = false;
                        }
                        else
                        {
                            appendable.Append(", ");
                        }
                        appendable.Append(col);
                    }
                    appendable.Append(")");
                }
                else
                {
                    throw new ArgumentException("either ON or USING must be included for OUTER JOIN");
                }
            }
        }

        public void Visit(SubqueryFactor node)
        {
            appendable.Append('(');
            QueryExpression query = node.GetSubquery();
            query.Accept(this);
            appendable.Append(") AS ").Append(node.GetAlias());
        }

        public void Visit(TableRefFactor node)
        {
            Identifier table = node.GetTable();
            table.Accept(this);
            string alias = node.GetAlias();
            if (alias != null)
            {
                appendable.Append(" AS ").Append(alias);
            }
            IList<IndexHint> list = node.GetHintList();
            if (list != null && !list.IsEmpty())
            {
                appendable.Append(' ');
                PrintList(list, " ");
            }
        }

        public void Visit(Dual dual)
        {
            appendable.Append("DUAL");
        }

        public void Visit(GroupBy node)
        {
            appendable.Append("GROUP BY ");
            bool isFst = true;
            foreach (var p in node.GetOrderByList())
            {
                if (isFst)
                {
                    isFst = false;
                }
                else
                {
                    appendable.Append(", ");
                }
                Expr col = p.GetKey();
                col.Accept(this);
                switch (p.GetValue())
                {
                    case SortOrder.Desc:
                        {
                            appendable.Append(" DESC");
                            break;
                        }
                }
            }
            if (node.IsWithRollup())
            {
                appendable.Append(" WITH ROLLUP");
            }
        }

        public void Visit(OrderBy node)
        {
            appendable.Append("ORDER BY ");
            bool isFst = true;
            foreach (var p in node.GetOrderByList())
            {
                if (isFst)
                {
                    isFst = false;
                }
                else
                {
                    appendable.Append(", ");
                }
                Expr col = p.GetKey();
                col.Accept(this);
                switch (p.GetValue())
                {
                    case SortOrder.Desc:
                        {
                            appendable.Append(" DESC");
                            break;
                        }
                }
            }
        }

        public void Visit(Limit node)
        {
            appendable.Append("LIMIT ");
            object offset = node.GetOffset();
            if (offset is ParamMarker)
            {
                ((ParamMarker)offset).Accept(this);
            }
            else
            {
                appendable.Append(offset.ToString());
            }
            appendable.Append(", ");
            object size = node.GetSize();
            if (size is ParamMarker)
            {
                ((ParamMarker)size).Accept(this);
            }
            else
            {
                appendable.Append(size.ToString());
            }
        }

        public void Visit(ColumnDefinition node)
        {
            throw new NotSupportedException("col_def in CREATE TABLE is partially parsed");
        }

        public void Visit(IndexOption node)
        {
            if (node.GetKeyBlockSize() != null)
            {
                appendable.Append("KEY_BLOCK_SIZE = ");
                node.GetKeyBlockSize().Accept(this);
            }
            else
            {
                if (node.GetIndexType() != IndexType.None)
                {
                    appendable.Append("USING ");
                    switch (node.GetIndexType())
                    {
                        case IndexType.Btree:
                            {
                                // USING {BTREE | HASH}
                                appendable.Append("BTREE");
                                break;
                            }

                        case IndexType.Hash:
                            {
                                appendable.Append("HASH");
                                break;
                            }
                    }
                }
                else
                {
                    if (node.GetParserName() != null)
                    {
                        appendable.Append("WITH PARSER ");
                        node.GetParserName().Accept(this);
                    }
                    else
                    {
                        if (node.GetComment() != null)
                        {
                            appendable.Append("COMMENT ");
                            node.GetComment().Accept(this);
                        }
                    }
                }
            }
        }

        public void Visit(IndexColumnName node)
        {
        }

        // QS_TODO
        public void Visit(TableOptions node)
        {
        }

        // QS_TODO
        public void Visit(DDLAlterTableStatement.AlterSpecification node)
        {
            throw new NotSupportedException("subclass have not implement visit");
        }

        public void Visit(DataType node)
        {
            throw new NotSupportedException("subclass have not implement visit");
        }

        private void PrintSimpleShowStmt(string attName)
        {
            appendable.Append("SHOW ").Append(attName);
        }

        public void Visit(ShowAuthors node)
        {
            PrintSimpleShowStmt("AUTHORS");
        }

        public void Visit(ShowBinaryLog node)
        {
            PrintSimpleShowStmt("BINARY LOGS");
        }

        public void Visit(ShowBinLogEvent node)
        {
            appendable.Append("SHOW BINLOG EVENTS");
            string logName = node.GetLogName();
            if (logName != null)
            {
                appendable.Append(" IN ").Append(logName);
            }
            Expr pos = node.GetPos();
            if (pos != null)
            {
                appendable.Append(" FROM ");
                pos.Accept(this);
            }
            Limit limit = node.GetLimit();
            if (limit != null)
            {
                appendable.Append(' ');
                limit.Accept(this);
            }
        }

        /// <summary>' ' will be prepended</summary>
        private void PrintLikeOrWhere(string like, Expr
             where)
        {
            if (like != null)
            {
                appendable.Append(" LIKE ").Append(like);
            }
            else
            {
                if (where != null)
                {
                    appendable.Append(" WHERE ");
                    where.Accept(this);
                }
            }
        }

        public void Visit(ShowCharaterSet node)
        {
            appendable.Append("SHOW CHARACTER SET");
            PrintLikeOrWhere(node.GetPattern(), node.GetWhere());
        }

        public void Visit(ShowCollation node)
        {
            appendable.Append("SHOW COLLATION");
            PrintLikeOrWhere(node.GetPattern(), node.GetWhere());
        }

        public void Visit(ShowColumns node)
        {
            appendable.Append("SHOW ");
            if (node.IsFull())
            {
                appendable.Append("FULL ");
            }
            appendable.Append("COLUMNS FROM ");
            node.GetTable().Accept(this);
            PrintLikeOrWhere(node.GetPattern(), node.GetWhere());
        }

        public void Visit(ShowContributors node)
        {
            PrintSimpleShowStmt("CONTRIBUTORS");
        }

        public void Visit(ShowCreate node)
        {
            appendable.Append("SHOW CREATE ").Append(node.GetCreateType().GetEnumName()).Append(' ');
            node.GetId().Accept(this);
        }

        public void Visit(ShowDatabases node)
        {
            appendable.Append("SHOW DATABASES");
            PrintLikeOrWhere(node.GetPattern(), node.GetWhere());
        }

        public void Visit(ShowEngine node)
        {
            appendable.Append("SHOW ENGINE ");
            switch (node.GetEngineType())
            {
                case ShowEngine.Type.InnodbMutex:
                    {
                        appendable.Append("INNODB MUTEX");
                        break;
                    }

                case ShowEngine.Type.InnodbStatus:
                    {
                        appendable.Append("INNODB STATUS");
                        break;
                    }

                case ShowEngine.Type.PerformanceSchemaStatus:
                    {
                        appendable.Append("PERFORMANCE SCHEMA STATUS");
                        break;
                    }

                default:
                    {
                        throw new ArgumentException("unrecognized type for SHOW ENGINE: " + node.GetEngineType(
                            ));
                    }
            }
        }

        public void Visit(ShowEngines node)
        {
            PrintSimpleShowStmt("ENGINES");
        }

        public void Visit(ShowErrors node)
        {
            appendable.Append("SHOW ");
            if (node.IsCount())
            {
                appendable.Append("COUNT(*) ERRORS");
            }
            else
            {
                appendable.Append("ERRORS");
                Limit limit = node.GetLimit();
                if (node.GetLimit() != null)
                {
                    appendable.Append(' ');
                    limit.Accept(this);
                }
            }
        }

        public void Visit(ShowEvents node)
        {
            appendable.Append("SHOW EVENTS");
            Identifier schema = node.GetSchema();
            if (schema != null)
            {
                appendable.Append(" FROM ");
                schema.Accept(this);
            }
            PrintLikeOrWhere(node.GetPattern(), node.GetWhere());
        }

        public void Visit(ShowFunctionCode node)
        {
            appendable.Append("SHOW FUNCTION CODE ");
            node.GetFunctionName().Accept(this);
        }

        public void Visit(ShowFunctionStatus node)
        {
            appendable.Append("SHOW FUNCTION STATUS");
            PrintLikeOrWhere(node.GetPattern(), node.GetWhere());
        }

        public void Visit(ShowGrants node)
        {
            appendable.Append("SHOW GRANTS");
            Expr user = node.GetUser();
            if (user != null)
            {
                appendable.Append(" FOR ");
                user.Accept(this);
            }
        }

        public void Visit(ShowIndex node)
        {
            appendable.Append("SHOW ");
            switch (node.GetIndexType())
            {
                case ShowIndex.Type.Index:
                    {
                        appendable.Append("INDEX ");
                        break;
                    }

                case ShowIndex.Type.Indexes:
                    {
                        appendable.Append("INDEXES ");
                        break;
                    }

                case ShowIndex.Type.Keys:
                    {
                        appendable.Append("KEYS ");
                        break;
                    }

                default:
                    {
                        throw new ArgumentException("unrecognized type for SHOW INDEX: " + node.GetIndexType()
                            );
                    }
            }
            appendable.Append("IN ");
            node.GetTable().Accept(this);
        }

        public void Visit(ShowMasterStatus node)
        {
            PrintSimpleShowStmt("MASTER STATUS");
        }

        public void Visit(ShowOpenTables node)
        {
            appendable.Append("SHOW OPEN TABLES");
            Identifier db = node.GetSchema();
            if (db != null)
            {
                appendable.Append(" FROM ");
                db.Accept(this);
            }
            PrintLikeOrWhere(node.GetPattern(), node.GetWhere());
        }

        public void Visit(ShowPlugins node)
        {
            PrintSimpleShowStmt("PLUGINS");
        }

        public void Visit(ShowPrivileges node)
        {
            PrintSimpleShowStmt("PRIVILEGES");
        }

        public void Visit(ShowProcedureCode node)
        {
            appendable.Append("SHOW PROCEDURE CODE ");
            node.GetProcedureName().Accept(this);
        }

        public void Visit(ShowProcedureStatus node)
        {
            appendable.Append("SHOW PROCEDURE STATUS");
            PrintLikeOrWhere(node.GetPattern(), node.GetWhere());
        }

        public void Visit(ShowProcesslist node)
        {
            appendable.Append("SHOW ");
            if (node.IsFull())
            {
                appendable.Append("FULL ");
            }
            appendable.Append("PROCESSLIST");
        }

        public void Visit(ShowProfile node)
        {
            appendable.Append("SHOW PROFILE");
            IList<ShowProfile.Type> types = node.GetTypes();
            bool isFst = true;
            foreach (ShowProfile.Type type in types)
            {
                if (isFst)
                {
                    isFst = false;
                    appendable.Append(' ');
                }
                else
                {
                    appendable.Append(", ");
                }
                appendable.Append(type.GetEnumName().Replace('_', ' '));
            }
            Expr query = node.GetForQuery();
            if (query != null)
            {
                appendable.Append(" FOR QUERY ");
                query.Accept(this);
            }
            Limit limit = node.GetLimit();
            if (limit != null)
            {
                appendable.Append(' ');
                limit.Accept(this);
            }
        }

        public void Visit(ShowProfiles node)
        {
            PrintSimpleShowStmt("PROFILES");
        }

        public void Visit(ShowSlaveHosts node)
        {
            PrintSimpleShowStmt("SLAVE HOSTS");
        }

        public void Visit(ShowSlaveStatus node)
        {
            PrintSimpleShowStmt("SLAVE STATUS");
        }

        public void Visit(ShowStatus node)
        {
            appendable.Append("SHOW ").Append(node.GetScope().GetEnumName().Replace('_', ' ')).Append
                (" STATUS");
            PrintLikeOrWhere(node.GetPattern(), node.GetWhere());
        }

        public void Visit(ShowTables node)
        {
            appendable.Append("SHOW");
            if (node.IsFull())
            {
                appendable.Append(" FULL");
            }
            appendable.Append(" TABLES");
            Identifier schema = node.GetSchema();
            if (schema != null)
            {
                appendable.Append(" FROM ");
                schema.Accept(this);
            }
            PrintLikeOrWhere(node.GetPattern(), node.GetWhere());
        }

        public void Visit(ShowTableStatus node)
        {
            appendable.Append("SHOW TABLE STATUS");
            Identifier schema = node.GetDatabase();
            if (schema != null)
            {
                appendable.Append(" FROM ");
                schema.Accept(this);
            }
            PrintLikeOrWhere(node.GetPattern(), node.GetWhere());
        }

        public void Visit(ShowTriggers node)
        {
            appendable.Append("SHOW TRIGGERS");
            Identifier schema = node.GetSchema();
            if (schema != null)
            {
                appendable.Append(" FROM ");
                schema.Accept(this);
            }
            PrintLikeOrWhere(node.GetPattern(), node.GetWhere());
        }

        public void Visit(ShowVariables node)
        {
            appendable.Append("SHOW ").Append(node.GetScope().GetEnumName().Replace('_', ' ')).Append
                (" VARIABLES");
            PrintLikeOrWhere(node.GetPattern(), node.GetWhere());
        }

        public void Visit(ShowWarnings node)
        {
            appendable.Append("SHOW ");
            if (node.IsCount())
            {
                appendable.Append("COUNT(*) WARNINGS");
            }
            else
            {
                appendable.Append("WARNINGS");
                Limit limit = node.GetLimit();
                if (limit != null)
                {
                    appendable.Append(' ');
                    limit.Accept(this);
                }
            }
        }

        public void Visit(DescTableStatement node)
        {
            appendable.Append("DESC ");
            node.GetTable().Accept(this);
        }

        public void Visit(DALSetStatement node)
        {
            appendable.Append("SET ");
            bool isFst = true;
            foreach (Pair<VariableExpression, Expr> p in
                node.GetAssignmentList())
            {
                if (isFst)
                {
                    isFst = false;
                }
                else
                {
                    appendable.Append(", ");
                }
                p.GetKey().Accept(this);
                appendable.Append(" = ");
                p.GetValue().Accept(this);
            }
        }

        public void Visit(DALSetNamesStatement node)
        {
            appendable.Append("SET NAMES ");
            if (node.IsDefault())
            {
                appendable.Append("DEFAULT");
            }
            else
            {
                appendable.Append(node.GetCharsetName());
                string collate = node.GetCollationName();
                if (collate != null)
                {
                    appendable.Append(" COLLATE ");
                    appendable.Append(collate);
                }
            }
        }

        public void Visit(DALSetCharacterSetStatement node)
        {
            appendable.Append("SET CHARACTER SET ");
            if (node.IsDefault())
            {
                appendable.Append("DEFAULT");
            }
            else
            {
                appendable.Append(node.GetCharset());
            }
        }

        public void Visit(MTSSetTransactionStatement node)
        {
            appendable.Append("SET ");
            VariableScope scope = node.GetScope();
            if (scope != VariableScope.None)
            {
                switch (scope)
                {
                    case VariableScope.Session:
                        {
                            appendable.Append("SESSION ");
                            break;
                        }

                    case VariableScope.Global:
                        {
                            appendable.Append("GLOBAL ");
                            break;
                        }

                    default:
                        {
                            throw new ArgumentException("unknown scope for SET TRANSACTION ISOLATION LEVEL: "
                                 + scope);
                        }
                }
            }
            appendable.Append("TRANSACTION ISOLATION LEVEL ");
            switch (node.GetLevel())
            {
                case MTSSetTransactionStatement.IsolationLevel.ReadCommitted:
                    {
                        appendable.Append("READ COMMITTED");
                        break;
                    }

                case MTSSetTransactionStatement.IsolationLevel.ReadUncommitted:
                    {
                        appendable.Append("READ UNCOMMITTED");
                        break;
                    }

                case MTSSetTransactionStatement.IsolationLevel.RepeatableRead:
                    {
                        appendable.Append("REPEATABLE READ");
                        break;
                    }

                case MTSSetTransactionStatement.IsolationLevel.Serializable:
                    {
                        appendable.Append("SERIALIZABLE");
                        break;
                    }

                default:
                    {
                        throw new ArgumentException("unknown level for SET TRANSACTION ISOLATION LEVEL: "
                             + node.GetLevel());
                    }
            }
        }

        public void Visit(MTSSavepointStatement node)
        {
            appendable.Append("SAVEPOINT ");
            node.GetSavepoint().Accept(this);
        }

        public void Visit(MTSReleaseStatement node)
        {
            appendable.Append("RELEASE SAVEPOINT ");
            node.GetSavepoint().Accept(this);
        }

        public void Visit(MTSRollbackStatement node)
        {
            appendable.Append("ROLLBACK");
            Identifier savepoint = node.GetSavepoint();
            if (savepoint == null)
            {
                MTSRollbackStatement.CompleteType type = node.GetCompleteType();
                switch (type)
                {
                    case MTSRollbackStatement.CompleteType.Chain:
                        {
                            appendable.Append(" AND CHAIN");
                            break;
                        }

                    case MTSRollbackStatement.CompleteType.NoChain:
                        {
                            appendable.Append(" AND NO CHAIN");
                            break;
                        }

                    case MTSRollbackStatement.CompleteType.NoRelease:
                        {
                            appendable.Append(" NO RELEASE");
                            break;
                        }

                    case MTSRollbackStatement.CompleteType.Release:
                        {
                            appendable.Append(" RELEASE");
                            break;
                        }

                    case MTSRollbackStatement.CompleteType.UnDef:
                        {
                            break;
                        }

                    default:
                        {
                            throw new ArgumentException("unrecgnized complete type: " + type);
                        }
                }
            }
            else
            {
                appendable.Append(" TO SAVEPOINT ");
                savepoint.Accept(this);
            }
        }

        public void Visit(DMLCallStatement node)
        {
            appendable.Append("CALL ");
            node.GetProcedure().Accept(this);
            appendable.Append('(');
            PrintList(node.GetArguments());
            appendable.Append(')');
        }

        public void Visit(DMLDeleteStatement node)
        {
            appendable.Append("DELETE ");
            if (node.IsLowPriority())
            {
                appendable.Append("LOW_PRIORITY ");
            }
            if (node.IsQuick())
            {
                appendable.Append("QUICK ");
            }
            if (node.IsIgnore())
            {
                appendable.Append("IGNORE ");
            }
            TableReferences tableRefs = node.GetTableRefs();
            if (tableRefs == null)
            {
                appendable.Append("FROM ");
                node.GetTableNames()[0].Accept(this);
            }
            else
            {
                PrintList(node.GetTableNames());
                appendable.Append(" FROM ");
                node.GetTableRefs().Accept(this);
            }
            Expr where = node.GetWhereCondition();
            if (where != null)
            {
                appendable.Append(" WHERE ");
                where.Accept(this);
            }
            OrderBy orderBy = node.GetOrderBy();
            if (orderBy != null)
            {
                appendable.Append(' ');
                orderBy.Accept(this);
            }
            Limit limit = node.GetLimit();
            if (limit != null)
            {
                appendable.Append(' ');
                limit.Accept(this);
            }
        }

        public void Visit(DMLInsertStatement node)
        {
            appendable.Append("INSERT ");
            switch (node.GetMode())
            {
                case DMLInsertStatement.InsertMode.Delay:
                    {
                        appendable.Append("DELAYED ");
                        break;
                    }

                case DMLInsertStatement.InsertMode.High:
                    {
                        appendable.Append("HIGH_PRIORITY ");
                        break;
                    }

                case DMLInsertStatement.InsertMode.Low:
                    {
                        appendable.Append("LOW_PRIORITY ");
                        break;
                    }

                case DMLInsertStatement.InsertMode.Undef:
                    {
                        break;
                    }

                default:
                    {
                        throw new ArgumentException("unknown mode for INSERT: " + node.GetMode());
                    }
            }
            if (node.IsIgnore())
            {
                appendable.Append("IGNORE ");
            }
            appendable.Append("INTO ");
            node.GetTable().Accept(this);
            appendable.Append(' ');
            IList<Identifier> cols = node.GetColumnNameList();
            if (cols != null && !cols.IsEmpty())
            {
                appendable.Append('(');
                PrintList(cols);
                appendable.Append(") ");
            }
            QueryExpression select = node.GetSelect();
            if (select == null)
            {
                appendable.Append("VALUES ");
                IList<RowExpression> rows = node.GetRowList();
                if (rows != null && !rows.IsEmpty())
                {
                    bool isFst = true;
                    foreach (RowExpression row in rows)
                    {
                        if (row == null || row.GetRowExprList().IsEmpty())
                        {
                            continue;
                        }
                        if (isFst)
                        {
                            isFst = false;
                        }
                        else
                        {
                            appendable.Append(", ");
                        }
                        appendable.Append('(');
                        PrintList(row.GetRowExprList());
                        appendable.Append(')');
                    }
                }
                else
                {
                    throw new ArgumentException("at least one row for INSERT");
                }
            }
            else
            {
                select.Accept(this);
            }
            var dup = node.GetDuplicateUpdate();
            if (dup != null && !dup.IsEmpty())
            {
                appendable.Append(" ON DUPLICATE KEY UPDATE ");
                bool isFst = true;
                foreach (Pair<Identifier, Expr> p in dup)
                {
                    if (isFst)
                    {
                        isFst = false;
                    }
                    else
                    {
                        appendable.Append(", ");
                    }
                    p.GetKey().Accept(this);
                    appendable.Append(" = ");
                    p.GetValue().Accept(this);
                }
            }
        }

        public void Visit(DMLReplaceStatement node)
        {
            appendable.Append("REPLACE ");
            switch (node.GetMode())
            {
                case DMLReplaceStatement.ReplaceMode.Delay:
                    {
                        appendable.Append("DELAYED ");
                        break;
                    }

                case DMLReplaceStatement.ReplaceMode.Low:
                    {
                        appendable.Append("LOW_PRIORITY ");
                        break;
                    }

                case DMLReplaceStatement.ReplaceMode.Undef:
                    {
                        break;
                    }

                default:
                    {
                        throw new ArgumentException("unknown mode for INSERT: " + node.GetMode());
                    }
            }
            appendable.Append("INTO ");
            node.GetTable().Accept(this);
            appendable.Append(' ');
            IList<Identifier> cols = node.GetColumnNameList();
            if (cols != null && !cols.IsEmpty())
            {
                appendable.Append('(');
                PrintList(cols);
                appendable.Append(") ");
            }
            QueryExpression select = node.GetSelect();
            if (select == null)
            {
                appendable.Append("VALUES ");
                IList<RowExpression> rows = node.GetRowList();
                if (rows != null && !rows.IsEmpty())
                {
                    bool isFst = true;
                    foreach (RowExpression row in rows)
                    {
                        if (row == null || row.GetRowExprList().IsEmpty())
                        {
                            continue;
                        }
                        if (isFst)
                        {
                            isFst = false;
                        }
                        else
                        {
                            appendable.Append(", ");
                        }
                        appendable.Append('(');
                        PrintList(row.GetRowExprList());
                        appendable.Append(')');
                    }
                }
                else
                {
                    throw new ArgumentException("at least one row for REPLACE");
                }
            }
            else
            {
                select.Accept(this);
            }
        }

        public void Visit(DMLSelectStatement node)
        {
            appendable.Append("SELECT ");
            DMLSelectStatement.SelectOption option = node.GetOption();
            switch (option.resultDup)
            {
                case DMLSelectStatement.SelectDuplicationStrategy.All:
                    {
                        break;
                    }

                case DMLSelectStatement.SelectDuplicationStrategy.Distinct:
                    {
                        appendable.Append("DISTINCT ");
                        break;
                    }

                case DMLSelectStatement.SelectDuplicationStrategy.Distinctrow:
                    {
                        appendable.Append("DISTINCTROW ");
                        break;
                    }

                default:
                    {
                        throw new ArgumentException("unknown option for SELECT: " + option);
                    }
            }
            if (option.highPriority)
            {
                appendable.Append("HIGH_PRIORITY ");
            }
            if (option.straightJoin)
            {
                appendable.Append("STRAIGHT_JOIN ");
            }
            switch (option.resultSize)
            {
                case DMLSelectStatement.SmallOrBigResult.SqlBigResult:
                    {
                        appendable.Append("SQL_BIG_RESULT ");
                        break;
                    }

                case DMLSelectStatement.SmallOrBigResult.SqlSmallResult:
                    {
                        appendable.Append("SQL_SMALL_RESULT ");
                        break;
                    }

                case DMLSelectStatement.SmallOrBigResult.Undef:
                    {
                        break;
                    }

                default:
                    {
                        throw new ArgumentException("unknown option for SELECT: " + option);
                    }
            }
            if (option.sqlBufferResult)
            {
                appendable.Append("SQL_BUFFER_RESULT ");
            }
            switch (option.queryCache)
            {
                case DMLSelectStatement.QueryCacheStrategy.SqlCache:
                    {
                        appendable.Append("SQL_CACHE ");
                        break;
                    }

                case DMLSelectStatement.QueryCacheStrategy.SqlNoCache:
                    {
                        appendable.Append("SQL_NO_CACHE ");
                        break;
                    }

                case DMLSelectStatement.QueryCacheStrategy.Undef:
                    {
                        break;
                    }

                default:
                    {
                        throw new ArgumentException("unknown option for SELECT: " + option);
                    }
            }
            if (option.sqlCalcFoundRows)
            {
                appendable.Append("SQL_CALC_FOUND_ROWS ");
            }
            bool isFst = true;
            var exprList = node.GetSelectExprList();
            foreach (Pair<Expr, string> p in exprList)
            {
                if (isFst)
                {
                    isFst = false;
                }
                else
                {
                    appendable.Append(", ");
                }
                p.GetKey().Accept(this);
                string alias = p.GetValue();
                if (alias != null)
                {
                    appendable.Append(" AS ").Append(alias);
                }
            }
            TableReferences from = node.GetTables();
            if (from != null)
            {
                appendable.Append(" FROM ");
                from.Accept(this);
            }
            Expr where = node.GetWhere();
            if (where != null)
            {
                appendable.Append(" WHERE ");
                where.Accept(this);
            }
            GroupBy group = node.GetGroup();
            if (group != null)
            {
                appendable.Append(' ');
                group.Accept(this);
            }
            Expr having = node.GetHaving();
            if (having != null)
            {
                appendable.Append(" HAVING ");
                having.Accept(this);
            }
            OrderBy order = node.GetOrder();
            if (order != null)
            {
                appendable.Append(' ');
                order.Accept(this);
            }
            Limit limit = node.GetLimit();
            if (limit != null)
            {
                appendable.Append(' ');
                limit.Accept(this);
            }
            switch (option.lockMode)
            {
                case DMLSelectStatement.LockMode.ForUpdate:
                    {
                        appendable.Append(" FOR UPDATE");
                        break;
                    }

                case DMLSelectStatement.LockMode.LockInShareMode:
                    {
                        appendable.Append(" LOCK IN SHARE MODE");
                        break;
                    }

                case DMLSelectStatement.LockMode.Undef:
                    {
                        break;
                    }

                default:
                    {
                        throw new ArgumentException("unknown option for SELECT: " + option);
                    }
            }
        }

        public void Visit(DMLSelectUnionStatement node)
        {
            IList<DMLSelectStatement> list = node.GetSelectStmtList();
            if (list == null || list.IsEmpty())
            {
                throw new ArgumentException("SELECT UNION must have at least one SELECT");
            }
            int fstDist = node.GetFirstDistinctIndex();
            int i = 0;
            foreach (DMLSelectStatement select in list)
            {
                if (i > 0)
                {
                    appendable.Append(" UNION ");
                    if (i > fstDist)
                    {
                        appendable.Append("ALL ");
                    }
                }
                appendable.Append('(');
                select.Accept(this);
                appendable.Append(')');
                ++i;
            }
            OrderBy order = node.GetOrderBy();
            if (order != null)
            {
                appendable.Append(' ');
                order.Accept(this);
            }
            Limit limit = node.GetLimit();
            if (limit != null)
            {
                appendable.Append(' ');
                limit.Accept(this);
            }
        }

        public void Visit(DMLUpdateStatement node)
        {
            appendable.Append("UPDATE ");
            if (node.IsLowPriority())
            {
                appendable.Append("LOW_PRIORITY ");
            }
            if (node.IsIgnore())
            {
                appendable.Append("IGNORE ");
            }
            node.GetTableRefs().Accept(this);
            appendable.Append(" SET ");
            bool isFst = true;
            foreach (Pair<Identifier, Expr> p in node.GetValues
                ())
            {
                if (isFst)
                {
                    isFst = false;
                }
                else
                {
                    appendable.Append(", ");
                }
                p.GetKey().Accept(this);
                appendable.Append(" = ");
                p.GetValue().Accept(this);
            }
            Expr where = node.GetWhere();
            if (where != null)
            {
                appendable.Append(" WHERE ");
                where.Accept(this);
            }
            OrderBy order = node.GetOrderBy();
            if (order != null)
            {
                appendable.Append(' ');
                order.Accept(this);
            }
            Limit limit = node.GetLimit();
            if (limit != null)
            {
                appendable.Append(' ');
                limit.Accept(this);
            }
        }

        public void Visit(DDLTruncateStatement node)
        {
            appendable.Append("TRUNCATE TABLE ");
            node.GetTable().Accept(this);
        }

        public void Visit(DDLAlterTableStatement node)
        {
            throw new NotSupportedException("ALTER TABLE is partially parsed");
        }

        public void Visit(DDLCreateIndexStatement node)
        {
            throw new NotSupportedException("CREATE INDEX is partially parsed");
        }

        public void Visit(DDLCreateTableStatement node)
        {
            throw new NotSupportedException("CREATE TABLE is partially parsed");
        }

        public void Visit(DDLRenameTableStatement node)
        {
            appendable.Append("RENAME TABLE ");
            bool isFst = true;
            foreach (Pair<Identifier, Identifier> p in node.GetList())
            {
                if (isFst)
                {
                    isFst = false;
                }
                else
                {
                    appendable.Append(", ");
                }
                p.GetKey().Accept(this);
                appendable.Append(" TO ");
                p.GetValue().Accept(this);
            }
        }

        public void Visit(DDLDropIndexStatement node)
        {
            appendable.Append("DROP INDEX ");
            node.GetIndexName().Accept(this);
            appendable.Append(" ON ");
            node.GetTable().Accept(this);
        }

        public void Visit(DDLDropTableStatement node)
        {
            appendable.Append("DROP ");
            if (node.IsTemp())
            {
                appendable.Append("TEMPORARY ");
            }
            appendable.Append("TABLE ");
            if (node.IsIfExists())
            {
                appendable.Append("IF EXISTS ");
            }
            PrintList(node.GetTableNames());
            switch (node.GetMode())
            {
                case DDLDropTableStatement.Mode.Cascade:
                    {
                        appendable.Append(" CASCADE");
                        break;
                    }

                case DDLDropTableStatement.Mode.Restrict:
                    {
                        appendable.Append(" RESTRICT");
                        break;
                    }

                case DDLDropTableStatement.Mode.Undef:
                    {
                        break;
                    }

                default:
                    {
                        throw new ArgumentException("unsupported mode for DROP TABLE: " + node.GetMode());
                    }
            }
        }

        public void Visit(ExtDDLCreatePolicy node)
        {
            appendable.Append("CREATE POLICY ");
            node.GetName().Accept(this);
            appendable.Append(" (");
            bool first = true;
            foreach (var p in node.GetProportion())
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    appendable.Append(", ");
                }
                appendable.Append(p.GetKey()).Append(' ');
                p.GetValue().Accept(this);
            }
            appendable.Append(')');
        }

        public void Visit(ExtDDLDropPolicy node)
        {
            appendable.Append("DROP POLICY ");
            node.GetPolicyName().Accept(this);
        }
    }
}