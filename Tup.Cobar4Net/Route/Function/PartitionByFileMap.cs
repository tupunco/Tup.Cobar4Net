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
using System.IO;
using System.Linq;
using Sharpen;
using Tup.Cobar4Net.Config.Model.Rule;
using Tup.Cobar4Net.Parser.Ast.Expression;
using Tup.Cobar4Net.Parser.Ast.Expression.Primary.Function;

namespace Tup.Cobar4Net.Route.Function
{
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public class PartitionByFileMap : FunctionExpression, IRuleAlgorithm
    {
        private readonly IDictionary<string, int> _app2Partition = null;

        public PartitionByFileMap(string functionName)
            : this(functionName, null)
        {
        }

        public PartitionByFileMap(string functionName, IList<IExpression> arguments)
            : base(functionName, arguments)
        {
        }

        public int DefaultNode { get; set; }

        public string FileMapPath { get; set; }

        public Number[] Calculate(IDictionary<object, object> parameters)
        {
            var rst = new int[1];
            var arg = arguments[0].Evaluation(parameters);
            if (arg == null)
            {
                throw new ArgumentException("partition key is null ");
            }
            if (arg == ExpressionConstants.Unevaluatable)
            {
                throw new ArgumentException("argument is UNEVALUATABLE");
            }

            var pid = _app2Partition.GetValue(arg.ToString(), int.MinValue);
            if (pid == int.MinValue)
            {
                rst[0] = DefaultNode;
            }
            else
            {
                rst[0] = pid;
            }
            return Number.ValueOf(rst);
        }

        public virtual IRuleAlgorithm ConstructMe(params object[] objects)
        {
            var args = objects.Select(x => (IExpression)x).ToList();
            var rst = new PartitionByFileMap(functionName, args)
            {
                FileMapPath = FileMapPath,
                DefaultNode = DefaultNode
            };
            return rst;
        }

        public virtual void Initialize()
        {
            try
            {
                using (var fin = new FileStream(FileMapPath, FileMode.Open))
                {
                    using (var @in = new StreamReader(fin))
                    {
                        for (string line = null; (line = @in.ReadLine()) != null;)
                        {
                            line = line.Trim();
                            if (line.StartsWith("#", StringComparison.Ordinal)
                                || line.StartsWith("//", StringComparison.Ordinal))
                            {
                                continue;
                            }

                            var ind = line.IndexOf('=');
                            if (ind < 0)
                            {
                                continue;
                            }

                            try
                            {
                                var key = Runtime.Substring(line, 0, ind).Trim();
                                var pid = Convert.ToInt32(Runtime.Substring(line, ind + 1).Trim());
                                _app2Partition[key] = pid;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override void Init()
        {
            Initialize();
        }

        protected override object EvaluationInternal(IDictionary<object, object> parameters)
        {
            return Calculate(parameters)[0];
        }

        public override FunctionExpression ConstructFunction(IList<IExpression> arguments)
        {
            if (arguments == null || arguments.Count != 1)
            {
                throw new ArgumentException(string.Format("function {0} must have 1 arguments but is {1}", FunctionName,
                    arguments));
            }
            var args = new object[arguments.Count];
            var i = -1;
            foreach (var arg in arguments)
            {
                args[++i] = arg;
            }
            return (FunctionExpression)ConstructMe(args);
        }
    }
}