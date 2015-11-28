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

using System.Collections.Generic;

using Tup.Cobar4Net.Config.Loader;
using Tup.Cobar4Net.Config.Model.Rule;
using Tup.Cobar4Net.Config.Util;
using Tup.Cobar4Net.Parser.Ast.Expression.Primary.Function;
using Tup.Cobar4Net.Parser.Recognizer.Mysql;
using Tup.Cobar4Net.Parser.Recognizer.Mysql.Lexer;
using Tup.Cobar4Net.Parser.Recognizer.Mysql.Syntax;
using Tup.Cobar4Net.Route.Function;
using Expr = Tup.Cobar4Net.Parser.Ast.Expression.Expression;

namespace Tup.Cobar4Net.Route.Config
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class RouteRuleInitializer
    {
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        public static void InitRouteRule(SchemaLoader loader)
        {
            var functions = loader.GetFunctions();
            var functionManager = new MySQLFunctionManager(true);
            BuildFuncManager(functionManager, functions);
            foreach (var conf in loader.ListRuleConfig())
            {
                string algorithmString = conf.GetAlgorithm();
                var lexer = new MySQLLexer(algorithmString);
                var parser = new MySQLExprParser(lexer, functionManager, false, MySQLParser.DefaultCharset);
                Expr expression = parser.Expression();
                if (lexer.Token() != MySQLToken.Eof)
                {
                    throw new ConfigException("route algorithm not end with EOF: " + algorithmString);
                }
                RuleAlgorithm algorithm;
                if (expression is RuleAlgorithm)
                {
                    algorithm = (RuleAlgorithm)expression;
                }
                else
                {
                    algorithm = new ExpressionAdapter(expression);
                }
                conf.SetRuleAlgorithm(algorithm);
            }
        }

        private static void BuildFuncManager(MySQLFunctionManager functionManager, IDictionary<string, RuleAlgorithm> functions)
        {
            var extFuncPrototypeMap = new Dictionary<string, FunctionExpression>(functions.Count);
            foreach (var en in functions)
            {
                extFuncPrototypeMap[en.Key] = (FunctionExpression)en.Value;
            }
            functionManager.AddExtendFunction(extFuncPrototypeMap);
        }
    }
}