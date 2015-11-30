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
using Tup.Cobar4Net.Parser.Ast.Fragment.Tableref;
using Tup.Cobar4Net.Parser.Ast.Stmt.Dal;
using Tup.Cobar4Net.Parser.Ast.Stmt.Ddl;
using Tup.Cobar4Net.Parser.Ast.Stmt.Dml;
using Tup.Cobar4Net.Parser.Ast.Stmt.Extension;
using Tup.Cobar4Net.Parser.Ast.Stmt.Mts;
using Char = Tup.Cobar4Net.Parser.Ast.Expression.Primary.Function.String.Char;
using Convert = Tup.Cobar4Net.Parser.Ast.Expression.Primary.Function.Cast.Convert;

namespace Tup.Cobar4Net.Parser.Visitor
{
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public sealed class MySqlOutputAstVisitor : ISqlAstVisitor
    {
        private static readonly object[] EmptyObjArray = new object[0];

        private static readonly int[] EmptyIntArray = new int[0];

        private readonly StringBuilder _appendable;

        private readonly object[] _args;

        private int[] _argsIndex;

        private int _index = -1;

        private IDictionary<PlaceHolder, object> _placeHolderToString;

        public MySqlOutputAstVisitor(StringBuilder appendable)
            : this(appendable, null)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="appendable"></param>
        /// <param name="args"></param>
        public MySqlOutputAstVisitor(StringBuilder appendable, object[] args)
        {
            this._appendable = appendable;
            this._args = args ?? EmptyObjArray;
            _argsIndex = args == null ? EmptyIntArray : new int[args.Length];
        }

        public void Visit(BetweenAndExpression node)
        {
            var comparee = node.First;
            var paren = comparee.Precedence <= node.Precedence;
            if (paren)
                _appendable.Append('(');

            comparee.Accept(this);
            if (paren)
                _appendable.Append(')');

            if (node.IsNot)
                _appendable.Append(" NOT BETWEEN ");
            else
                _appendable.Append(" BETWEEN ");

            var start = node.Second;
            paren = start.Precedence < node.Precedence;
            if (paren)
                _appendable.Append('(');

            start.Accept(this);
            if (paren)
                _appendable.Append(')');

            _appendable.Append(" AND ");
            var end = node.Third;
            paren = end.Precedence < node.Precedence;
            if (paren)
                _appendable.Append('(');

            end.Accept(this);
            if (paren)
                _appendable.Append(')');
        }

        public void Visit(ComparisionIsExpression node)
        {
            var comparee = node.Operand;
            var paren = comparee.Precedence < node.Precedence;
            if (paren)
                _appendable.Append('(');

            comparee.Accept(this);
            if (paren)
                _appendable.Append(')');

            switch (node.Mode)
            {
                case ComparisionIsExpression.IsNull:
                {
                    _appendable.Append(" IS NULL");
                    break;
                }

                case ComparisionIsExpression.IsTrue:
                {
                    _appendable.Append(" IS TRUE");
                    break;
                }

                case ComparisionIsExpression.IsFalse:
                {
                    _appendable.Append(" IS FALSE");
                    break;
                }

                case ComparisionIsExpression.IsUnknown:
                {
                    _appendable.Append(" IS UNKNOWN");
                    break;
                }

                case ComparisionIsExpression.IsNotNull:
                {
                    _appendable.Append(" IS NOT NULL");
                    break;
                }

                case ComparisionIsExpression.IsNotTrue:
                {
                    _appendable.Append(" IS NOT TRUE");
                    break;
                }

                case ComparisionIsExpression.IsNotFalse:
                {
                    _appendable.Append(" IS NOT FALSE");
                    break;
                }

                case ComparisionIsExpression.IsNotUnknown:
                {
                    _appendable.Append(" IS NOT UNKNOWN");
                    break;
                }

                default:
                {
                    throw new ArgumentException("unknown mode for IS expression: " + node.Mode);
                }
            }
        }

        public void Visit(InExpressionList node)
        {
            _appendable.Append('(');
            PrintList(node.ExprList);
            _appendable.Append(')');
        }

        public void Visit(LikeExpression node)
        {
            var comparee = node.First;
            var paren = comparee.Precedence < node.Precedence;
            if (paren)
                _appendable.Append('(');

            comparee.Accept(this);
            if (paren)
                _appendable.Append(')');

            if (node.IsNot)
                _appendable.Append(" NOT LIKE ");
            else
                _appendable.Append(" LIKE ");

            var pattern = node.Second;
            paren = pattern.Precedence <= node.Precedence;
            if (paren)
                _appendable.Append('(');

            pattern.Accept(this);
            if (paren)
                _appendable.Append(')');

            var escape = node.Third;
            if (escape != null)
            {
                _appendable.Append(" ESCAPE ");
                paren = escape.Precedence <= node.Precedence;
                if (paren)
                    _appendable.Append('(');
                escape.Accept(this);

                if (paren)
                    _appendable.Append(')');
            }
        }

        public void Visit(CollateExpression node)
        {
            var @string = node.StringValue;
            var paren = @string.Precedence < node.Precedence;
            if (paren)
            {
                _appendable.Append('(');
            }
            @string.Accept(this);
            if (paren)
            {
                _appendable.Append(')');
            }
            _appendable.Append(" COLLATE ").Append(node.CollateName);
        }

        public void Visit(UserExpression node)
        {
            _appendable.Append(node.UserAtHost);
        }

        public void Visit(UnaryOperatorExpression node)
        {
            _appendable.Append(node.Operator).Append(' ');
            var paren = node.Operand.Precedence < node.Precedence;
            if (paren)
            {
                _appendable.Append('(');
            }
            node.Operand.Accept(this);
            if (paren)
            {
                _appendable.Append(')');
            }
        }

        public void Visit(BinaryOperatorExpression node)
        {
            var left = node.LeftOprand;
            var paren = node.IsLeftCombine
                ? left.Precedence < node.Precedence
                : left.Precedence <= node.Precedence;
            if (paren)
            {
                _appendable.Append('(');
            }
            left.Accept(this);
            if (paren)
            {
                _appendable.Append(')');
            }
            _appendable.Append(' ').Append(node.Operator).Append(' ');
            var right = node.RightOprand;
            paren = node.IsLeftCombine
                ? right.Precedence <= node.Precedence
                : right.Precedence < node.Precedence;
            if (paren)
            {
                _appendable.Append('(');
            }
            right.Accept(this);
            if (paren)
            {
                _appendable.Append(')');
            }
        }

        public void Visit(PolyadicOperatorExpression node)
        {
            for (int i = 0, len = node.Arity; i < len; ++i)
            {
                if (i > 0)
                {
                    _appendable.Append(' ').Append(node.Operator).Append(' ');
                }
                var operand = node.GetOperand(i);
                var paren = operand.Precedence < node.Precedence;
                if (paren)
                {
                    _appendable.Append('(');
                }
                operand.Accept(this);
                if (paren)
                {
                    _appendable.Append(')');
                }
            }
        }

        public void Visit(LogicalAndExpression node)
        {
            Visit((PolyadicOperatorExpression) node);
        }

        public void Visit(LogicalOrExpression node)
        {
            Visit((PolyadicOperatorExpression) node);
        }

        public void Visit(ComparisionEqualsExpression node)
        {
            Visit((BinaryOperatorExpression) node);
        }

        public void Visit(ComparisionNullSafeEqualsExpression node)
        {
            Visit((BinaryOperatorExpression) node);
        }

        public void Visit(InExpression node)
        {
            Visit((BinaryOperatorExpression) node);
        }

        public void Visit(FunctionExpression node)
        {
            var functionName = node.FunctionName;
            _appendable.Append(functionName).Append('(');
            PrintList(node.Arguments);
            _appendable.Append(')');
        }

        public void Visit(Char node)
        {
            var functionName = node.FunctionName;
            _appendable.Append(functionName).Append('(');
            PrintList(node.Arguments);
            var charset = node.Charset;
            if (charset != null)
            {
                _appendable.Append(" USING ").Append(charset);
            }
            _appendable.Append(')');
        }

        public void Visit(Convert node)
        {
            var functionName = node.FunctionName;
            _appendable.Append(functionName).Append('(');
            PrintList(node.Arguments);
            var transcodeName = node.TranscodeName;
            _appendable.Append(" USING ").Append(transcodeName);
            _appendable.Append(')');
        }

        public void Visit(Trim node)
        {
            var functionName = node.FunctionName;
            _appendable.Append(functionName).Append('(');
            var remStr = node.RemainString;
            switch (node.Direction)
            {
                case TrimDirection.Default:
                {
                    if (remStr != null)
                    {
                        remStr.Accept(this);
                        _appendable.Append(" FROM ");
                    }
                    break;
                }

                case TrimDirection.Both:
                {
                    _appendable.Append("BOTH ");
                    if (remStr != null)
                    {
                        remStr.Accept(this);
                    }
                    _appendable.Append(" FROM ");
                    break;
                }

                case TrimDirection.Leading:
                {
                    _appendable.Append("LEADING ");
                    if (remStr != null)
                    {
                        remStr.Accept(this);
                    }
                    _appendable.Append(" FROM ");
                    break;
                }

                case TrimDirection.Trailing:
                {
                    _appendable.Append("TRAILING ");
                    if (remStr != null)
                    {
                        remStr.Accept(this);
                    }
                    _appendable.Append(" FROM ");
                    break;
                }

                default:
                {
                    throw new ArgumentException("unknown trim direction: " + node.Direction);
                }
            }
            var str = node.StringValue;
            str.Accept(this);
            _appendable.Append(')');
        }

        public void Visit(Cast node)
        {
            var functionName = node.FunctionName;
            _appendable.Append(functionName).Append('(');
            node.Expr.Accept(this);
            _appendable.Append(" AS ");
            var typeName = node.TypeName;
            _appendable.Append(typeName);
            var info1 = node.TypeInfo1;
            if (info1 != null)
            {
                _appendable.Append('(');
                info1.Accept(this);
                var info2 = node.TypeInfo2;
                if (info2 != null)
                {
                    _appendable.Append(", ");
                    info2.Accept(this);
                }
                _appendable.Append(')');
            }
            _appendable.Append(')');
        }

        public void Visit(Avg node)
        {
            var functionName = node.FunctionName;
            _appendable.Append(functionName).Append('(');
            if (node.IsDistinct)
            {
                _appendable.Append("DISTINCT ");
            }
            PrintList(node.Arguments);
            _appendable.Append(')');
        }

        public void Visit(Max node)
        {
            var functionName = node.FunctionName;
            _appendable.Append(functionName).Append('(');
            if (node.IsDistinct)
            {
                _appendable.Append("DISTINCT ");
            }
            PrintList(node.Arguments);
            _appendable.Append(')');
        }

        public void Visit(Min node)
        {
            var functionName = node.FunctionName;
            _appendable.Append(functionName).Append('(');
            if (node.IsDistinct)
            {
                _appendable.Append("DISTINCT ");
            }
            PrintList(node.Arguments);
            _appendable.Append(')');
        }

        public void Visit(Sum node)
        {
            var functionName = node.FunctionName;
            _appendable.Append(functionName).Append('(');
            if (node.IsDistinct)
            {
                _appendable.Append("DISTINCT ");
            }
            PrintList(node.Arguments);
            _appendable.Append(')');
        }

        public void Visit(Count node)
        {
            var functionName = node.FunctionName;
            _appendable.Append(functionName).Append('(');
            if (node.IsDistinct)
            {
                _appendable.Append("DISTINCT ");
            }
            PrintList(node.Arguments);
            _appendable.Append(')');
        }

        public void Visit(GroupConcat node)
        {
            var functionName = node.FunctionName;
            _appendable.Append(functionName).Append('(');
            if (node.IsDistinct)
            {
                _appendable.Append("DISTINCT ");
            }
            PrintList(node.Arguments);
            var orderBy = node.OrderBy;
            if (orderBy != null)
            {
                _appendable.Append(" ORDER BY ");
                orderBy.Accept(this);
                if (node.IsDesc)
                {
                    _appendable.Append(" DESC");
                }
                else
                {
                    _appendable.Append(" ASC");
                }
                var list = node.AppendedColumnNames;
                if (list != null && !list.IsEmpty())
                {
                    _appendable.Append(", ");
                    PrintList(list);
                }
            }
            var sep = node.Separator;
            if (sep != null)
            {
                _appendable.Append(" SEPARATOR ").Append(sep);
            }
            _appendable.Append(')');
        }

        public void Visit(Extract node)
        {
            _appendable.Append("EXTRACT(")
                .Append(node.Unit.GetEnumName())
                .Append(" FROM ");
            PrintList(node.Arguments);
            _appendable.Append(')');
        }

        public void Visit(Timestampdiff node)
        {
            _appendable.Append("TIMESTAMPDIFF(")
                .Append(node.Unit.GetEnumName())
                .Append(", ");
            PrintList(node.Arguments);
            _appendable.Append(')');
        }

        public void Visit(Timestampadd node)
        {
            _appendable.Append("TIMESTAMPADD(")
                .Append(node.Unit.GetEnumName())
                .Append(", ");
            PrintList(node.Arguments);
            _appendable.Append(')');
        }

        public void Visit(GetFormat node)
        {
            _appendable.Append("GET_FORMAT(");
            var type = node.GetFormatType();
            _appendable.Append(type)
                .Append(", ");
            PrintList(node.Arguments);
            _appendable.Append(')');
        }

        public void Visit(PlaceHolder node)
        {
            if (_placeHolderToString == null)
            {
                _appendable.Append("${")
                    .Append(node.Name)
                    .Append('}');
                return;
            }
            var toStringer = _placeHolderToString.GetValue(node);
            if (toStringer == null)
            {
                _appendable.Append("${")
                    .Append(node.Name)
                    .Append('}');
            }
            else
            {
                _appendable.Append(toStringer);
            }
        }

        public void Visit(IntervalPrimary node)
        {
            _appendable.Append("INTERVAL ");
            var quantity = node.Quantity;
            var paren = quantity.Precedence < node.Precedence;
            if (paren)
            {
                _appendable.Append('(');
            }
            quantity.Accept(this);
            if (paren)
            {
                _appendable.Append(')');
            }
            var unit = node.Unit;
            _appendable.Append(' ').Append(unit.GetEnumName());
        }

        public void Visit(LiteralBitField node)
        {
            var introducer = node.Introducer;
            if (introducer != null)
            {
                _appendable.Append(introducer)
                    .Append(' ');
            }
            _appendable.Append("b'")
                .Append(node.Text)
                .Append('\'');
        }

        public void Visit(LiteralBoolean node)
        {
            if (node.IsTrue)
            {
                _appendable.Append("TRUE");
            }
            else
            {
                _appendable.Append("FALSE");
            }
        }

        public void Visit(LiteralHexadecimal node)
        {
            var introducer = node.Introducer;
            if (introducer != null)
            {
                _appendable.Append(introducer).Append(' ');
            }
            _appendable.Append("x'");
            node.AppendTo(_appendable);
            _appendable.Append('\'');
        }

        public void Visit(LiteralNull node)
        {
            _appendable.Append("NULL");
        }

        public void Visit(LiteralNumber node)
        {
            _appendable.Append(node.NumberValue);
        }

        public void Visit(LiteralString node)
        {
            var introducer = node.Introducer;
            if (introducer != null)
            {
                _appendable.Append(introducer);
            }
            else
            {
                if (node.IsNChars)
                {
                    _appendable.Append('N');
                }
            }
            _appendable.Append('\'').Append(node.StringValue).Append('\'');
        }

        public void Visit(CaseWhenOperatorExpression node)
        {
            _appendable.Append("CASE");
            var comparee = node.Comparee;
            if (comparee != null)
            {
                _appendable.Append(' ');
                comparee.Accept(this);
            }
            var whenList = node.WhenList;
            foreach (var whenthen in whenList)
            {
                _appendable.Append(" WHEN ");
                var when = whenthen.Key;
                when.Accept(this);
                _appendable.Append(" THEN ");
                var then = whenthen.Value;
                then.Accept(this);
            }
            var elseRst = node.ElseResult;
            if (elseRst != null)
            {
                _appendable.Append(" ELSE ");
                elseRst.Accept(this);
            }
            _appendable.Append(" END");
        }

        public void Visit(DefaultValue node)
        {
            _appendable.Append("DEFAULT");
        }

        public void Visit(ExistsPrimary node)
        {
            _appendable.Append("EXISTS (");
            node.Subquery.Accept(this);
            _appendable.Append(')');
        }

        public void Visit(Identifier node)
        {
            IExpression parent = node.Parent;
            if (parent != null)
            {
                parent.Accept(this);
                _appendable.Append('.');
            }
            _appendable.Append(node.IdText);
        }

        public void Visit(MatchExpression node)
        {
            _appendable.Append("MATCH (");
            PrintList(node.Columns);
            _appendable.Append(") AGAINST (");
            var pattern = node.Pattern;
            var inparen = ContainsCompIn(pattern);
            if (inparen)
            {
                _appendable.Append('(');
            }
            pattern.Accept(this);
            if (inparen)
            {
                _appendable.Append(')');
            }
            switch (node.Modifier)
            {
                case MatchModifier.InBooleanMode:
                {
                    _appendable.Append(" IN BOOLEAN MODE");
                    break;
                }

                case MatchModifier.InNaturalLanguageMode:
                {
                    _appendable.Append(" IN NATURAL LANGUAGE MODE");
                    break;
                }

                case MatchModifier.InNaturalLanguageModeWithQueryExpansion:
                {
                    _appendable.Append(" IN NATURAL LANGUAGE MODE WITH QUERY EXPANSION");
                    break;
                }

                case MatchModifier.WithQueryExpansion:
                {
                    _appendable.Append(" WITH QUERY EXPANSION");
                    break;
                }

                case MatchModifier.Default:
                {
                    break;
                }

                default:
                {
                    throw new ArgumentException("unkown modifier for match expression: " + node.Modifier);
                }
            }
            _appendable.Append(')');
        }

        public void Visit(ParamMarker node)
        {
            _appendable.Append('?');
            AppendArgsIndex(node.ParamIndex - 1);
        }

        public void Visit(RowExpression node)
        {
            _appendable.Append("ROW(");
            PrintList(node.RowExprList);
            _appendable.Append(')');
        }

        public void Visit(SysVarPrimary node)
        {
            var scope = node.Scope;
            switch (scope)
            {
                case VariableScope.Global:
                {
                    _appendable.Append("@@global.");
                    break;
                }

                case VariableScope.Session:
                {
                    _appendable.Append("@@");
                    break;
                }

                default:
                {
                    throw new ArgumentException("unkown _hintScope for sysVar primary: " + scope);
                }
            }
            _appendable.Append(node.VarText);
        }

        public void Visit(UsrDefVarPrimary node)
        {
            _appendable.Append(node.VarText);
        }

        public void Visit(IndexHint node)
        {
            var _hintAction = node.HintAction;
            switch (_hintAction)
            {
                case IndexHintAction.Force:
                {
                    _appendable.Append("FORCE ");
                    break;
                }

                case IndexHintAction.Ignore:
                {
                    _appendable.Append("IGNORE ");
                    break;
                }

                case IndexHintAction.Use:
                {
                    _appendable.Append("USE ");
                    break;
                }

                default:
                {
                    throw new ArgumentException("unkown _index _hintAction for _index hint: " + _hintAction);
                }
            }
            var _hintType = node.IndexType;
            switch (_hintType)
            {
                case IndexHintType.Index:
                {
                    _appendable.Append("INDEX ");
                    break;
                }

                case IndexHintType.Key:
                {
                    _appendable.Append("KEY ");
                    break;
                }

                default:
                {
                    throw new ArgumentException("unkown _index _hintType for _index hint: " + _hintType);
                }
            }
            var _hintScope = node.HintScope;
            switch (_hintScope)
            {
                case IndexHintScope.GroupBy:
                {
                    _appendable.Append("FOR GROUP BY ");
                    break;
                }

                case IndexHintScope.OrderBy:
                {
                    _appendable.Append("FOR ORDER BY ");
                    break;
                }

                case IndexHintScope.Join:
                {
                    _appendable.Append("FOR JOIN ");
                    break;
                }

                case IndexHintScope.All:
                {
                    break;
                }

                default:
                {
                    throw new ArgumentException("unkown _index _hintScope for _index hint: " + _hintScope);
                }
            }
            _appendable.Append('(');
            var indexList = node.IndexList;
            var isFst = true;
            foreach (var indexName in indexList)
            {
                if (isFst)
                {
                    isFst = false;
                }
                else
                {
                    _appendable.Append(", ");
                }
                _appendable.Append(indexName);
            }
            _appendable.Append(')');
        }

        public void Visit(TableReferences node)
        {
            PrintList(node.TableReferenceList);
        }

        public void Visit(InnerJoin node)
        {
            var left = node.LeftTableRef;
            var paren = left.Precedence < node.Precedence;
            if (paren)
            {
                _appendable.Append('(');
            }
            left.Accept(this);
            if (paren)
            {
                _appendable.Append(')');
            }
            _appendable.Append(" INNER JOIN ");
            var right = node.RightTableRef;
            paren = right.Precedence <= node.Precedence;
            if (paren)
            {
                _appendable.Append('(');
            }
            right.Accept(this);
            if (paren)
            {
                _appendable.Append(')');
            }
            var on = node.OnCond;
            var @using = node.Using;
            if (on != null)
            {
                _appendable.Append(" ON ");
                on.Accept(this);
            }
            else
            {
                if (@using != null)
                {
                    _appendable.Append(" USING (");
                    var isFst = true;
                    foreach (var col in @using)
                    {
                        if (isFst)
                        {
                            isFst = false;
                        }
                        else
                        {
                            _appendable.Append(", ");
                        }
                        _appendable.Append(col);
                    }
                    _appendable.Append(")");
                }
            }
        }

        public void Visit(NaturalJoin node)
        {
            var left = node.LeftTableRef;
            var paren = left.Precedence < node.Precedence;
            if (paren)
            {
                _appendable.Append('(');
            }
            left.Accept(this);
            if (paren)
            {
                _appendable.Append(')');
            }
            _appendable.Append(" NATURAL ");
            if (node.IsOuter)
            {
                if (node.IsLeft)
                {
                    _appendable.Append("LEFT ");
                }
                else
                {
                    _appendable.Append("RIGHT ");
                }
            }
            _appendable.Append("JOIN ");
            var right = node.RightTableRef;
            paren = right.Precedence <= node.Precedence;
            if (paren)
            {
                _appendable.Append('(');
            }
            right.Accept(this);
            if (paren)
            {
                _appendable.Append(')');
            }
        }

        public void Visit(StraightJoin node)
        {
            var left = node.LeftTableRef;
            var paren = left.Precedence < node.Precedence;
            if (paren)
            {
                _appendable.Append('(');
            }
            left.Accept(this);
            if (paren)
            {
                _appendable.Append(')');
            }
            _appendable.Append(" STRAIGHT_JOIN ");
            var right = node.RightTableRef;
            paren = right.Precedence <= node.Precedence;
            if (paren)
            {
                _appendable.Append('(');
            }
            right.Accept(this);
            if (paren)
            {
                _appendable.Append(')');
            }
            var on = node.OnCond;
            if (on != null)
            {
                _appendable.Append(" ON ");
                on.Accept(this);
            }
        }

        public void Visit(OuterJoin node)
        {
            var left = node.LeftTableRef;
            var paren = left.Precedence < node.Precedence;
            if (paren)
            {
                _appendable.Append('(');
            }
            left.Accept(this);
            if (paren)
            {
                _appendable.Append(')');
            }
            if (node.IsLeftJoin)
            {
                _appendable.Append(" LEFT JOIN ");
            }
            else
            {
                _appendable.Append(" RIGHT JOIN ");
            }
            var right = node.RightTableRef;
            paren = right.Precedence <= node.Precedence;
            if (paren)
            {
                _appendable.Append('(');
            }
            right.Accept(this);
            if (paren)
            {
                _appendable.Append(')');
            }
            var on = node.OnCond;
            var @using = node.Using;
            if (on != null)
            {
                _appendable.Append(" ON ");
                on.Accept(this);
            }
            else
            {
                if (@using != null)
                {
                    _appendable.Append(" USING (");
                    var isFst = true;
                    foreach (var col in @using)
                    {
                        if (isFst)
                        {
                            isFst = false;
                        }
                        else
                        {
                            _appendable.Append(", ");
                        }
                        _appendable.Append(col);
                    }
                    _appendable.Append(")");
                }
                else
                {
                    throw new ArgumentException("either ON or USING must be included for OUTER JOIN");
                }
            }
        }

        public void Visit(SubqueryFactor node)
        {
            _appendable.Append('(');
            var query = node.Subquery;
            query.Accept(this);
            _appendable.Append(") AS ").Append(node.Alias);
        }

        public void Visit(TableRefFactor node)
        {
            var table = node.Table;
            table.Accept(this);
            var alias = node.Alias;
            if (alias != null)
            {
                _appendable.Append(" AS ").Append(alias);
            }
            var list = node.HintList;
            if (list != null && !list.IsEmpty())
            {
                _appendable.Append(' ');
                PrintList(list, " ");
            }
        }

        public void Visit(Dual dual)
        {
            _appendable.Append("DUAL");
        }

        public void Visit(GroupBy node)
        {
            _appendable.Append("GROUP BY ");
            var isFst = true;
            foreach (var p in node.OrderByList)
            {
                if (isFst)
                {
                    isFst = false;
                }
                else
                {
                    _appendable.Append(", ");
                }
                var col = p.Key;
                col.Accept(this);
                switch (p.Value)
                {
                    case SortOrder.Desc:
                    {
                        _appendable.Append(" DESC");
                        break;
                    }
                }
            }
            if (node.IsWithRollup)
            {
                _appendable.Append(" WITH ROLLUP");
            }
        }

        public void Visit(OrderBy node)
        {
            _appendable.Append("ORDER BY ");
            var isFst = true;
            foreach (var p in node.OrderByList)
            {
                if (isFst)
                {
                    isFst = false;
                }
                else
                {
                    _appendable.Append(", ");
                }
                var col = p.Key;
                col.Accept(this);
                switch (p.Value)
                {
                    case SortOrder.Desc:
                    {
                        _appendable.Append(" DESC");
                        break;
                    }
                }
            }
        }

        public void Visit(Limit node)
        {
            _appendable.Append("LIMIT ");
            var offset = node.Offset;
            if (offset is ParamMarker)
            {
                ((ParamMarker) offset).Accept(this);
            }
            else
            {
                _appendable.Append(offset);
            }
            _appendable.Append(", ");
            var size = node.Size;
            if (size is ParamMarker)
            {
                ((ParamMarker) size).Accept(this);
            }
            else
            {
                _appendable.Append(size);
            }
        }

        public void Visit(ColumnDefinition node)
        {
            throw new NotSupportedException("col_def in CREATE TABLE is partially parsed");
        }

        public void Visit(IndexOption node)
        {
            if (node.KeyBlockSize != null)
            {
                _appendable.Append("KEY_BLOCK_SIZE = ");
                node.KeyBlockSize.Accept(this);
            }
            else
            {
                if (node.IndexType != IndexType.None)
                {
                    _appendable.Append("USING ");
                    switch (node.IndexType)
                    {
                        case IndexType.Btree:
                        {
                            // USING {BTREE | HASH}
                            _appendable.Append("BTREE");
                            break;
                        }

                        case IndexType.Hash:
                        {
                            _appendable.Append("HASH");
                            break;
                        }
                    }
                }
                else
                {
                    if (node.ParserName != null)
                    {
                        _appendable.Append("WITH PARSER ");
                        node.ParserName.Accept(this);
                    }
                    else
                    {
                        if (node.Comment != null)
                        {
                            _appendable.Append("COMMENT ");
                            node.Comment.Accept(this);
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
        public void Visit(DdlAlterTableStatement.AlterSpecification node)
        {
            throw new NotSupportedException("subclass have not implement visit");
        }

        public void Visit(DataType node)
        {
            throw new NotSupportedException("subclass have not implement visit");
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
            _appendable.Append("SHOW BINLOG EVENTS");
            var logName = node.LogName;
            if (logName != null)
            {
                _appendable.Append(" IN ").Append(logName);
            }
            var pos = node.Pos;
            if (pos != null)
            {
                _appendable.Append(" FROM ");
                pos.Accept(this);
            }
            var limit = node.Limit;
            if (limit != null)
            {
                _appendable.Append(' ');
                limit.Accept(this);
            }
        }

        public void Visit(ShowCharaterSet node)
        {
            _appendable.Append("SHOW CHARACTER SET");
            PrintLikeOrWhere(node.Pattern, node.Where);
        }

        public void Visit(ShowCollation node)
        {
            _appendable.Append("SHOW COLLATION");
            PrintLikeOrWhere(node.Pattern, node.Where);
        }

        public void Visit(ShowColumns node)
        {
            _appendable.Append("SHOW ");
            if (node.IsFull)
            {
                _appendable.Append("FULL ");
            }
            _appendable.Append("COLUMNS FROM ");
            node.Table.Accept(this);
            PrintLikeOrWhere(node.Pattern, node.Where);
        }

        public void Visit(ShowContributors node)
        {
            PrintSimpleShowStmt("CONTRIBUTORS");
        }

        public void Visit(ShowCreate node)
        {
            _appendable.Append("SHOW CREATE ").Append(node.CreateType.GetEnumName()).Append(' ');
            node.Id.Accept(this);
        }

        public void Visit(ShowDatabases node)
        {
            _appendable.Append("SHOW DATABASES");
            PrintLikeOrWhere(node.Pattern, node.Where);
        }

        public void Visit(ShowEngine node)
        {
            _appendable.Append("SHOW ENGINE ");
            switch (node.EngineType)
            {
                case EngineType.InnodbMutex:
                {
                    _appendable.Append("INNODB MUTEX");
                    break;
                }

                case EngineType.InnodbStatus:
                {
                    _appendable.Append("INNODB STATUS");
                    break;
                }

                case EngineType.PerformanceSchemaStatus:
                {
                    _appendable.Append("PERFORMANCE SCHEMA STATUS");
                    break;
                }

                default:
                {
                    throw new ArgumentException("unrecognized type for SHOW ENGINE: " + node.EngineType);
                }
            }
        }

        public void Visit(ShowEngines node)
        {
            PrintSimpleShowStmt("ENGINES");
        }

        public void Visit(ShowErrors node)
        {
            _appendable.Append("SHOW ");
            if (node.IsCount)
            {
                _appendable.Append("COUNT(*) ERRORS");
            }
            else
            {
                _appendable.Append("ERRORS");
                var limit = node.Limit;
                if (node.Limit != null)
                {
                    _appendable.Append(' ');
                    limit.Accept(this);
                }
            }
        }

        public void Visit(ShowEvents node)
        {
            _appendable.Append("SHOW EVENTS");
            var schema = node.Schema;
            if (schema != null)
            {
                _appendable.Append(" FROM ");
                schema.Accept(this);
            }
            PrintLikeOrWhere(node.Pattern, node.Where);
        }

        public void Visit(ShowFunctionCode node)
        {
            _appendable.Append("SHOW FUNCTION CODE ");
            node.FunctionName.Accept(this);
        }

        public void Visit(ShowFunctionStatus node)
        {
            _appendable.Append("SHOW FUNCTION STATUS");
            PrintLikeOrWhere(node.Pattern, node.Where);
        }

        public void Visit(ShowGrants node)
        {
            _appendable.Append("SHOW GRANTS");
            var user = node.User;
            if (user != null)
            {
                _appendable.Append(" FOR ");
                user.Accept(this);
            }
        }

        public void Visit(ShowIndex node)
        {
            _appendable.Append("SHOW ");
            switch (node.IndexType)
            {
                case ShowIndexType.Index:
                {
                    _appendable.Append("INDEX ");
                    break;
                }

                case ShowIndexType.Indexes:
                {
                    _appendable.Append("INDEXES ");
                    break;
                }

                case ShowIndexType.Keys:
                {
                    _appendable.Append("KEYS ");
                    break;
                }

                default:
                {
                    throw new ArgumentException("unrecognized type for SHOW INDEX: " + node.IndexType);
                }
            }
            _appendable.Append("IN ");
            node.Table.Accept(this);
        }

        public void Visit(ShowMasterStatus node)
        {
            PrintSimpleShowStmt("MASTER STATUS");
        }

        public void Visit(ShowOpenTables node)
        {
            _appendable.Append("SHOW OPEN TABLES");
            var db = node.Schema;
            if (db != null)
            {
                _appendable.Append(" FROM ");
                db.Accept(this);
            }
            PrintLikeOrWhere(node.Pattern, node.Where);
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
            _appendable.Append("SHOW PROCEDURE CODE ");
            node.ProcedureName.Accept(this);
        }

        public void Visit(ShowProcedureStatus node)
        {
            _appendable.Append("SHOW PROCEDURE STATUS");
            PrintLikeOrWhere(node.Pattern, node.Where);
        }

        public void Visit(ShowProcesslist node)
        {
            _appendable.Append("SHOW ");
            if (node.IsFull)
            {
                _appendable.Append("FULL ");
            }
            _appendable.Append("PROCESSLIST");
        }

        public void Visit(ShowProfile node)
        {
            _appendable.Append("SHOW PROFILE");
            var types = node.ProfileTypes;
            var isFst = true;
            foreach (var type in types)
            {
                if (isFst)
                {
                    isFst = false;
                    _appendable.Append(' ');
                }
                else
                {
                    _appendable.Append(", ");
                }
                _appendable.Append(type.GetEnumName().Replace('_', ' '));
            }
            var query = node.ForQuery;
            if (query != null)
            {
                _appendable.Append(" FOR QUERY ");
                query.Accept(this);
            }
            var limit = node.Limit;
            if (limit != null)
            {
                _appendable.Append(' ');
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
            _appendable.Append("SHOW ").Append(node.Scope.GetEnumName().Replace('_', ' ')).Append(" STATUS");
            PrintLikeOrWhere(node.Pattern, node.Where);
        }

        public void Visit(ShowTables node)
        {
            _appendable.Append("SHOW");
            if (node.IsFull)
            {
                _appendable.Append(" FULL");
            }
            _appendable.Append(" TABLES");
            var schema = node.Schema;
            if (schema != null)
            {
                _appendable.Append(" FROM ");
                schema.Accept(this);
            }
            PrintLikeOrWhere(node.Pattern, node.Where);
        }

        public void Visit(ShowTableStatus node)
        {
            _appendable.Append("SHOW TABLE STATUS");
            var schema = node.Database;
            if (schema != null)
            {
                _appendable.Append(" FROM ");
                schema.Accept(this);
            }
            PrintLikeOrWhere(node.Pattern, node.Where);
        }

        public void Visit(ShowTriggers node)
        {
            _appendable.Append("SHOW TRIGGERS");
            var schema = node.Schema;
            if (schema != null)
            {
                _appendable.Append(" FROM ");
                schema.Accept(this);
            }
            PrintLikeOrWhere(node.Pattern, node.Where);
        }

        public void Visit(ShowVariables node)
        {
            _appendable.Append("SHOW ").Append(node.Scope.GetEnumName().Replace('_', ' ')).Append(" VARIABLES");
            PrintLikeOrWhere(node.Pattern, node.Where);
        }

        public void Visit(ShowWarnings node)
        {
            _appendable.Append("SHOW ");
            if (node.IsCount)
            {
                _appendable.Append("COUNT(*) WARNINGS");
            }
            else
            {
                _appendable.Append("WARNINGS");
                var limit = node.Limit;
                if (limit != null)
                {
                    _appendable.Append(' ');
                    limit.Accept(this);
                }
            }
        }

        public void Visit(DescTableStatement node)
        {
            _appendable.Append("DESC ");
            node.Table.Accept(this);
        }

        public void Visit(DalSetStatement node)
        {
            _appendable.Append("SET ");
            var isFst = true;
            foreach (var p in
                node.AssignmentList)
            {
                if (isFst)
                {
                    isFst = false;
                }
                else
                {
                    _appendable.Append(", ");
                }
                p.Key.Accept(this);
                _appendable.Append(" = ");
                p.Value.Accept(this);
            }
        }

        public void Visit(DalSetNamesStatement node)
        {
            _appendable.Append("SET NAMES ");
            if (node.IsDefault)
            {
                _appendable.Append("DEFAULT");
            }
            else
            {
                _appendable.Append(node.CharsetName);
                var collate = node.CollationName;
                if (collate != null)
                {
                    _appendable.Append(" COLLATE ");
                    _appendable.Append(collate);
                }
            }
        }

        public void Visit(DalSetCharacterSetStatement node)
        {
            _appendable.Append("SET CHARACTER SET ");
            if (node.IsDefault)
            {
                _appendable.Append("DEFAULT");
            }
            else
            {
                _appendable.Append(node.Charset);
            }
        }

        public void Visit(MTSSetTransactionStatement node)
        {
            _appendable.Append("SET ");
            var scope = node.Scope;
            if (scope != VariableScope.None)
            {
                switch (scope)
                {
                    case VariableScope.Session:
                    {
                        _appendable.Append("SESSION ");
                        break;
                    }

                    case VariableScope.Global:
                    {
                        _appendable.Append("GLOBAL ");
                        break;
                    }

                    default:
                    {
                        throw new ArgumentException("unknown _hintScope for SET TRANSACTION ISOLATION LEVEL: "
                                                    + scope);
                    }
                }
            }
            _appendable.Append("TRANSACTION ISOLATION LEVEL ");
            switch (node.Level)
            {
                case IsolationLevel.ReadCommitted:
                {
                    _appendable.Append("READ COMMITTED");
                    break;
                }

                case IsolationLevel.ReadUncommitted:
                {
                    _appendable.Append("READ UNCOMMITTED");
                    break;
                }

                case IsolationLevel.RepeatableRead:
                {
                    _appendable.Append("REPEATABLE READ");
                    break;
                }

                case IsolationLevel.Serializable:
                {
                    _appendable.Append("SERIALIZABLE");
                    break;
                }

                default:
                {
                    throw new ArgumentException("unknown level for SET TRANSACTION ISOLATION LEVEL: "
                                                + node.Level);
                }
            }
        }

        public void Visit(MTSSavepointStatement node)
        {
            _appendable.Append("SAVEPOINT ");
            node.Savepoint.Accept(this);
        }

        public void Visit(MTSReleaseStatement node)
        {
            _appendable.Append("RELEASE SAVEPOINT ");
            node.Savepoint.Accept(this);
        }

        public void Visit(MTSRollbackStatement node)
        {
            _appendable.Append("ROLLBACK");
            var savepoint = node.Savepoint;
            if (savepoint == null)
            {
                var type = node.CompleteType;
                switch (type)
                {
                    case CompleteType.Chain:
                    {
                        _appendable.Append(" AND CHAIN");
                        break;
                    }

                    case CompleteType.NoChain:
                    {
                        _appendable.Append(" AND NO CHAIN");
                        break;
                    }

                    case CompleteType.NoRelease:
                    {
                        _appendable.Append(" NO RELEASE");
                        break;
                    }

                    case CompleteType.Release:
                    {
                        _appendable.Append(" RELEASE");
                        break;
                    }

                    case CompleteType.UnDef:
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
                _appendable.Append(" TO SAVEPOINT ");
                savepoint.Accept(this);
            }
        }

        public void Visit(DmlCallStatement node)
        {
            _appendable.Append("CALL ");
            node.GetProcedure().Accept(this);
            _appendable.Append('(');
            PrintList(node.GetArguments());
            _appendable.Append(')');
        }

        public void Visit(DmlDeleteStatement node)
        {
            _appendable.Append("DELETE ");
            if (node.IsLowPriority)
            {
                _appendable.Append("LOW_PRIORITY ");
            }
            if (node.IsQuick)
            {
                _appendable.Append("QUICK ");
            }
            if (node.IsIgnore)
            {
                _appendable.Append("IGNORE ");
            }
            var tableRefs = node.TableRefs;
            if (tableRefs == null)
            {
                _appendable.Append("FROM ");
                node.TableNames[0].Accept(this);
            }
            else
            {
                PrintList(node.TableNames);
                _appendable.Append(" FROM ");
                node.TableRefs.Accept(this);
            }
            var where = node.WhereCondition;
            if (where != null)
            {
                _appendable.Append(" WHERE ");
                where.Accept(this);
            }
            var orderBy = node.OrderBy;
            if (orderBy != null)
            {
                _appendable.Append(' ');
                orderBy.Accept(this);
            }
            var limit = node.Limit;
            if (limit != null)
            {
                _appendable.Append(' ');
                limit.Accept(this);
            }
        }

        public void Visit(DmlInsertStatement node)
        {
            _appendable.Append("INSERT ");
            switch (node.Mode)
            {
                case InsertMode.Delay:
                {
                    _appendable.Append("DELAYED ");
                    break;
                }

                case InsertMode.High:
                {
                    _appendable.Append("HIGH_PRIORITY ");
                    break;
                }

                case InsertMode.Low:
                {
                    _appendable.Append("LOW_PRIORITY ");
                    break;
                }

                case InsertMode.Undef:
                {
                    break;
                }

                default:
                {
                    throw new ArgumentException("unknown mode for INSERT: " + node.Mode);
                }
            }
            if (node.IsIgnore)
            {
                _appendable.Append("IGNORE ");
            }
            _appendable.Append("INTO ");
            node.Table.Accept(this);
            _appendable.Append(' ');
            var cols = node.ColumnNameList;
            if (cols != null && !cols.IsEmpty())
            {
                _appendable.Append('(');
                PrintList(cols);
                _appendable.Append(") ");
            }
            var select = node.Select;
            if (select == null)
            {
                _appendable.Append("VALUES ");
                var rows = node.RowList;
                if (rows != null && !rows.IsEmpty())
                {
                    var isFst = true;
                    foreach (var row in rows)
                    {
                        if (row == null || row.RowExprList.IsEmpty())
                        {
                            continue;
                        }
                        if (isFst)
                        {
                            isFst = false;
                        }
                        else
                        {
                            _appendable.Append(", ");
                        }
                        _appendable.Append('(');
                        PrintList(row.RowExprList);
                        _appendable.Append(')');
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
            var dup = node.DuplicateUpdate;
            if (dup != null && !dup.IsEmpty())
            {
                _appendable.Append(" ON DUPLICATE KEY UPDATE ");
                var isFst = true;
                foreach (var p in dup)
                {
                    if (isFst)
                    {
                        isFst = false;
                    }
                    else
                    {
                        _appendable.Append(", ");
                    }
                    p.Key.Accept(this);
                    _appendable.Append(" = ");
                    p.Value.Accept(this);
                }
            }
        }

        public void Visit(DmlReplaceStatement node)
        {
            _appendable.Append("REPLACE ");
            switch (node.Mode)
            {
                case ReplaceMode.Delay:
                {
                    _appendable.Append("DELAYED ");
                    break;
                }

                case ReplaceMode.Low:
                {
                    _appendable.Append("LOW_PRIORITY ");
                    break;
                }

                case ReplaceMode.Undef:
                {
                    break;
                }

                default:
                {
                    throw new ArgumentException("unknown mode for INSERT: " + node.Mode);
                }
            }
            _appendable.Append("INTO ");
            node.Table.Accept(this);
            _appendable.Append(' ');
            var cols = node.ColumnNameList;
            if (cols != null && !cols.IsEmpty())
            {
                _appendable.Append('(');
                PrintList(cols);
                _appendable.Append(") ");
            }
            var select = node.Select;
            if (select == null)
            {
                _appendable.Append("VALUES ");
                var rows = node.RowList;
                if (rows != null && !rows.IsEmpty())
                {
                    var isFst = true;
                    foreach (var row in rows)
                    {
                        if (row == null || row.RowExprList.IsEmpty())
                        {
                            continue;
                        }
                        if (isFst)
                        {
                            isFst = false;
                        }
                        else
                        {
                            _appendable.Append(", ");
                        }
                        _appendable.Append('(');
                        PrintList(row.RowExprList);
                        _appendable.Append(')');
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

        public void Visit(DmlSelectStatement node)
        {
            _appendable.Append("SELECT ");
            var option = node.Option;
            switch (option.resultDup)
            {
                case SelectDuplicationStrategy.All:
                {
                    break;
                }

                case SelectDuplicationStrategy.Distinct:
                {
                    _appendable.Append("DISTINCT ");
                    break;
                }

                case SelectDuplicationStrategy.Distinctrow:
                {
                    _appendable.Append("DISTINCTROW ");
                    break;
                }

                default:
                {
                    throw new ArgumentException("unknown option for SELECT: " + option);
                }
            }
            if (option.highPriority)
            {
                _appendable.Append("HIGH_PRIORITY ");
            }
            if (option.straightJoin)
            {
                _appendable.Append("STRAIGHT_JOIN ");
            }
            switch (option.resultSize)
            {
                case SelectSmallOrBigResult.SqlBigResult:
                {
                    _appendable.Append("SQL_BIG_RESULT ");
                    break;
                }

                case SelectSmallOrBigResult.SqlSmallResult:
                {
                    _appendable.Append("SQL_SMALL_RESULT ");
                    break;
                }

                case SelectSmallOrBigResult.Undef:
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
                _appendable.Append("SQL_BUFFER_RESULT ");
            }
            switch (option.SelectQueryCache)
            {
                case SelectQueryCacheStrategy.SqlCache:
                {
                    _appendable.Append("SQL_CACHE ");
                    break;
                }

                case SelectQueryCacheStrategy.SqlNoCache:
                {
                    _appendable.Append("SQL_NO_CACHE ");
                    break;
                }

                case SelectQueryCacheStrategy.Undef:
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
                _appendable.Append("SQL_CALC_FOUND_ROWS ");
            }
            var isFst = true;
            var exprList = node.SelectExprList;
            foreach (var p in exprList)
            {
                if (isFst)
                {
                    isFst = false;
                }
                else
                {
                    _appendable.Append(", ");
                }
                p.Key.Accept(this);
                var alias = p.Value;
                if (alias != null)
                {
                    _appendable.Append(" AS ").Append(alias);
                }
            }
            var from = node.Tables;
            if (from != null)
            {
                _appendable.Append(" FROM ");
                from.Accept(this);
            }
            var where = node.Where;
            if (where != null)
            {
                _appendable.Append(" WHERE ");
                where.Accept(this);
            }
            var group = node.Group;
            if (group != null)
            {
                _appendable.Append(' ');
                group.Accept(this);
            }
            var having = node.Having;
            if (having != null)
            {
                _appendable.Append(" HAVING ");
                having.Accept(this);
            }
            var order = node.Order;
            if (order != null)
            {
                _appendable.Append(' ');
                order.Accept(this);
            }
            var limit = node.Limit;
            if (limit != null)
            {
                _appendable.Append(' ');
                limit.Accept(this);
            }
            switch (option.lockMode)
            {
                case LockMode.ForUpdate:
                {
                    _appendable.Append(" FOR UPDATE");
                    break;
                }

                case LockMode.LockInShareMode:
                {
                    _appendable.Append(" LOCK IN SHARE MODE");
                    break;
                }

                case LockMode.Undef:
                {
                    break;
                }

                default:
                {
                    throw new ArgumentException("unknown option for SELECT: " + option);
                }
            }
        }

        public void Visit(DmlSelectUnionStatement node)
        {
            var list = node.SelectStmtList;
            if (list == null || list.IsEmpty())
            {
                throw new ArgumentException("SELECT UNION must have at least one SELECT");
            }
            var fstDist = node.FirstDistinctIndex;
            var i = 0;
            foreach (var select in list)
            {
                if (i > 0)
                {
                    _appendable.Append(" UNION ");
                    if (i > fstDist)
                    {
                        _appendable.Append("ALL ");
                    }
                }
                _appendable.Append('(');
                select.Accept(this);
                _appendable.Append(')');
                ++i;
            }
            var order = node.OrderBy;
            if (order != null)
            {
                _appendable.Append(' ');
                order.Accept(this);
            }
            var limit = node.Limit;
            if (limit != null)
            {
                _appendable.Append(' ');
                limit.Accept(this);
            }
        }

        public void Visit(DmlUpdateStatement node)
        {
            _appendable.Append("UPDATE ");
            if (node.IsLowPriority)
            {
                _appendable.Append("LOW_PRIORITY ");
            }
            if (node.IsIgnore)
            {
                _appendable.Append("IGNORE ");
            }
            node.TableRefs.Accept(this);
            _appendable.Append(" SET ");
            var isFst = true;
            foreach (var p in node.Values)
            {
                if (isFst)
                {
                    isFst = false;
                }
                else
                {
                    _appendable.Append(", ");
                }
                p.Key.Accept(this);
                _appendable.Append(" = ");
                p.Value.Accept(this);
            }
            var where = node.Where;
            if (where != null)
            {
                _appendable.Append(" WHERE ");
                where.Accept(this);
            }
            var order = node.OrderBy;
            if (order != null)
            {
                _appendable.Append(' ');
                order.Accept(this);
            }
            var limit = node.Limit;
            if (limit != null)
            {
                _appendable.Append(' ');
                limit.Accept(this);
            }
        }

        public void Visit(DdlTruncateStatement node)
        {
            _appendable.Append("TRUNCATE TABLE ");
            node.Table.Accept(this);
        }

        public void Visit(DdlAlterTableStatement node)
        {
            throw new NotSupportedException("ALTER TABLE is partially parsed");
        }

        public void Visit(DdlCreateIndexStatement node)
        {
            throw new NotSupportedException("CREATE INDEX is partially parsed");
        }

        public void Visit(DdlCreateTableStatement node)
        {
            throw new NotSupportedException("CREATE TABLE is partially parsed");
        }

        public void Visit(DdlRenameTableStatement node)
        {
            _appendable.Append("RENAME TABLE ");
            var isFst = true;
            foreach (var p in node.PairList)
            {
                if (isFst)
                {
                    isFst = false;
                }
                else
                {
                    _appendable.Append(", ");
                }
                p.Key.Accept(this);
                _appendable.Append(" TO ");
                p.Value.Accept(this);
            }
        }

        public void Visit(DdlDropIndexStatement node)
        {
            _appendable.Append("DROP INDEX ");
            node.IndexName.Accept(this);
            _appendable.Append(" ON ");
            node.Table.Accept(this);
        }

        public void Visit(DdlDropTableStatement node)
        {
            _appendable.Append("DROP ");
            if (node.IsTemp)
            {
                _appendable.Append("TEMPORARY ");
            }
            _appendable.Append("TABLE ");
            if (node.IsIfExists)
            {
                _appendable.Append("IF EXISTS ");
            }
            PrintList(node.TableNames);
            switch (node.Mode)
            {
                case DropTableMode.Cascade:
                {
                    _appendable.Append(" CASCADE");
                    break;
                }

                case DropTableMode.Restrict:
                {
                    _appendable.Append(" RESTRICT");
                    break;
                }

                case DropTableMode.Undef:
                {
                    break;
                }

                default:
                {
                    throw new ArgumentException("unsupported mode for DROP TABLE: " + node.Mode);
                }
            }
        }

        public void Visit(ExtDdlCreatePolicy node)
        {
            _appendable.Append("CREATE POLICY ");
            node.Name.Accept(this);
            _appendable.Append(" (");
            var first = true;
            foreach (var p in node.Proportion)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    _appendable.Append(", ");
                }
                _appendable.Append(p.Key).Append(' ');
                p.Value.Accept(this);
            }
            _appendable.Append(')');
        }

        public void Visit(ExtDdlDropPolicy node)
        {
            _appendable.Append("DROP POLICY ");
            node.PolicyName.Accept(this);
        }

        public void SetPlaceHolderToString(IDictionary<PlaceHolder, object> map)
        {
            _placeHolderToString = map;
        }

        public string GetSql()
        {
            return _appendable.ToString();
        }

        /// <returns>
        ///     never null. rst[i]
        ///     <see cref="_args" />
        ///     [
        ///     <see cref="_argsIndex" />
        ///     [i]]
        /// </returns>
        public object[] GetArguments()
        {
            var argsIndexSize = _argsIndex.Length;
            if (argsIndexSize <= 0)
            {
                return EmptyObjArray;
            }

            var noChange = true;
            for (var i = 0; i < argsIndexSize; ++i)
            {
                if (i != _argsIndex[i])
                {
                    noChange = false;
                    break;
                }
            }
            if (noChange)
            {
                return _args;
            }
            var rst = new object[argsIndexSize];
            for (var i_1 = 0; i_1 < argsIndexSize; ++i_1)
            {
                rst[i_1] = _args[_argsIndex[i_1]];
            }
            return rst;
        }

        /// <param name="list">never null</param>
        private void PrintList<TItem>(IList<TItem> list)
            where TItem : IAstNode
        {
            PrintList(list, ", ");
        }

        /// <param name="list">never null</param>
        private void PrintList<TItem>(IList<TItem> list, string sep)
            where TItem : IAstNode
        {
            var isFst = true;
            foreach (IAstNode arg in list)
            {
                if (isFst)
                {
                    isFst = false;
                }
                else
                {
                    _appendable.Append(sep);
                }
                arg.Accept(this);
            }
        }

        private static bool ContainsCompIn(IExpression pat)
        {
            if (pat.Precedence > ExpressionConstants.PrecedenceComparision)
            {
                return false;
            }
            if (pat is BinaryOperatorExpression)
            {
                if (pat is InExpression)
                {
                    return true;
                }
                var bp = (BinaryOperatorExpression) pat;
                if (bp.IsLeftCombine)
                {
                    return ContainsCompIn(bp.LeftOprand);
                }
                return ContainsCompIn(bp.LeftOprand);
            }
            if (pat is ComparisionIsExpression)
            {
                var @is = (ComparisionIsExpression) pat;
                return ContainsCompIn(@is.Operand);
            }
            if (pat is TernaryOperatorExpression)
            {
                var tp = (TernaryOperatorExpression) pat;
                return ContainsCompIn(tp.First) || ContainsCompIn(tp.Second) || ContainsCompIn(tp.Third);
            }
            if (pat is UnaryOperatorExpression)
            {
                var up = (UnaryOperatorExpression) pat;
                return ContainsCompIn(up.Operand);
            }
            return false;
        }

        private void AppendArgsIndex(int value)
        {
            var i = ++_index;
            if (_argsIndex.Length <= i)
            {
                var a = new int[i + 1];
                if (i > 0)
                {
                    Array.Copy(_argsIndex, 0, a, 0, i);
                }
                _argsIndex = a;
            }
            _argsIndex[i] = value;
        }

        private void PrintSimpleShowStmt(string attName)
        {
            _appendable.Append("SHOW ").Append(attName);
        }

        /// <summary>' ' will be prepended</summary>
        private void PrintLikeOrWhere(string like, IExpression where)
        {
            if (like != null)
            {
                _appendable.Append(" LIKE ").Append(like);
            }
            else
            {
                if (where != null)
                {
                    _appendable.Append(" WHERE ");
                    where.Accept(this);
                }
            }
        }
    }
}