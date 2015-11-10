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
using Sharpen;
using Tup.Cobar.Parser.Ast;
using Tup.Cobar.Parser.Ast.Expression.Primary.Literal;
using Tup.Cobar.Parser.Ast.Fragment.Ddl.Datatype;
using Tup.Cobar.Parser.Visitor;

namespace Tup.Cobar.Parser.Ast.Fragment.Ddl
{
	/// <summary>NOT FULL AST</summary>
	/// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
	public class ColumnDefinition : ASTNode
	{
		public enum SpecialIndex
		{
			Primary,
			Unique
		}

		public enum ColumnFormat
		{
			Fixed,
			Dynamic,
			Default
		}

		private readonly DataType dataType;

		private readonly bool notNull;

		private readonly Tup.Cobar.Parser.Ast.Expression.Expression defaultVal;

		private readonly bool autoIncrement;

		private readonly ColumnDefinition.SpecialIndex specialIndex;

		private readonly LiteralString comment;

		private readonly ColumnDefinition.ColumnFormat columnFormat;

		/// <param name="dataType"/>
		/// <param name="notNull"/>
		/// <param name="defaultVal">might be null</param>
		/// <param name="autoIncrement"/>
		/// <param name="specialIndex">might be null</param>
		/// <param name="comment">might be null</param>
		/// <param name="columnFormat">might be null</param>
		public ColumnDefinition(DataType dataType, bool notNull, Tup.Cobar.Parser.Ast.Expression.Expression
			 defaultVal, bool autoIncrement, ColumnDefinition.SpecialIndex specialIndex, LiteralString
			 comment, ColumnDefinition.ColumnFormat columnFormat)
		{
			if (dataType == null)
			{
				throw new ArgumentException("data type is null");
			}
			this.dataType = dataType;
			this.notNull = notNull;
			this.defaultVal = defaultVal;
			this.autoIncrement = autoIncrement;
			this.specialIndex = specialIndex;
			this.comment = comment;
			this.columnFormat = columnFormat;
		}

		public virtual DataType GetDataType()
		{
			return dataType;
		}

		public virtual bool IsNotNull()
		{
			return notNull;
		}

		public virtual Tup.Cobar.Parser.Ast.Expression.Expression GetDefaultVal()
		{
			return defaultVal;
		}

		public virtual bool IsAutoIncrement()
		{
			return autoIncrement;
		}

		public virtual ColumnDefinition.SpecialIndex GetSpecialIndex()
		{
			return specialIndex;
		}

		public virtual LiteralString GetComment()
		{
			return comment;
		}

		public virtual ColumnDefinition.ColumnFormat GetColumnFormat()
		{
			return columnFormat;
		}

		public virtual void Accept(SQLASTVisitor visitor)
		{
			visitor.Visit(this);
		}
	}
}
