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

using System.Text;
using NUnit.Framework;
using Tup.Cobar4Net.Parser.Ast;
using Tup.Cobar4Net.Parser.Visitor;

namespace Tup.Cobar4Net.Parser.Recognizer.Mysql.Syntax
{
    [TestFixture]
    public abstract class AbstractSyntaxTest
    {
        private const bool debug = false;

        protected internal virtual string Output2MySql(IAstNode node, string sql)
        {
            var sb = new StringBuilder(sql.Length);
            node.Accept(new MySqlOutputAstVisitor(sb));
            return sb.ToString();
        }
    }
}