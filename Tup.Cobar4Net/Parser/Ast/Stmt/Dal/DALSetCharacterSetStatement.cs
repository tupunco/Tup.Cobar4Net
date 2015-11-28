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
using Tup.Cobar4Net.Parser.Visitor;

namespace Tup.Cobar4Net.Parser.Ast.Stmt.Dal
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class DALSetCharacterSetStatement : SQLStatement
    {
        private readonly string charset;

        public DALSetCharacterSetStatement()
        {
            this.charset = null;
        }

        public DALSetCharacterSetStatement(string charset)
        {
            if (charset == null)
            {
                throw new ArgumentException("charsetName is null");
            }
            this.charset = charset;
        }

        public virtual bool IsDefault()
        {
            return charset == null;
        }

        public virtual string GetCharset()
        {
            return charset;
        }

        public virtual void Accept(SQLASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}