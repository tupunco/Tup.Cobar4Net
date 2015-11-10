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

using Tup.Cobar.Parser.Ast.Fragment;
using Tup.Cobar.Parser.Visitor;

namespace Tup.Cobar.Parser.Ast.Stmt.Dal
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class ShowBinLogEvent : DALShowStatement
    {
        private readonly string logName;

        private readonly Tup.Cobar.Parser.Ast.Expression.Expression pos;

        private readonly Limit limit;

        public ShowBinLogEvent(string logName, Tup.Cobar.Parser.Ast.Expression.Expression
             pos, Limit limit)
        {
            this.logName = logName;
            this.pos = pos;
            this.limit = limit;
        }

        public virtual string GetLogName()
        {
            return logName;
        }

        public virtual Tup.Cobar.Parser.Ast.Expression.Expression GetPos()
        {
            return pos;
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