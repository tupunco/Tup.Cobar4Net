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

namespace Tup.Cobar4Net.Route.Config
{
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public class RouteRuleInitializer
    {
        /// <exception cref="System.SqlSyntaxErrorException" />
        public static void InitRouteRule(ISchemaLoader loader)
        {
            var functions = loader.Functions;
            var functionManager = new MySqlFunctionManager(true);
            BuildFuncManager(functionManager, functions);
            foreach (var conf in loader.RuleConfigList)
            {
                var algorithmString = conf.Algorithm;
                var lexer = new MySqlLexer(algorithmString);
                var parser = new MySqlExprParser(lexer, functionManager, false, MySqlParser.DefaultCharset);
                var expression = parser.Expression();
                if (lexer.Token() != MySqlToken.Eof)
                {
                    throw new ConfigException("route algorithm not end with EOF: " + algorithmString);
                }
                IRuleAlgorithm algorithm;
                if (expression is IRuleAlgorithm)
                {
                    algorithm = (IRuleAlgorithm)expression;
                }
                else
                {
                    algorithm = new ExpressionAdapter(expression);
                }
                conf.RuleAlgorithm = algorithm;
            }
        }

        private static void BuildFuncManager(MySqlFunctionManager functionManager,
            IDictionary<string, IRuleAlgorithm> functions)
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