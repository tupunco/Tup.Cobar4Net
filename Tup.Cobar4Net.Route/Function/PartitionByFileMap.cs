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
using Sharpen;
using Tup.Cobar4Net.Config.Model.Rule;
using Tup.Cobar4Net.Parser.Ast.Expression.Primary.Function;

namespace Tup.Cobar4Net.Route.Function
{
	/// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
	public class PartitionByFileMap : FunctionExpression, RuleAlgorithm
	{
		public PartitionByFileMap(string functionName)
			: this(functionName, null)
		{
		}

		public PartitionByFileMap(string functionName, IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression
			> arguments)
			: base(functionName, arguments)
		{
		}

		private int defaultNode;

		private string fileMapPath;

		public virtual void SetDefaultNode(int defaultNode)
		{
			this.defaultNode = defaultNode;
		}

		public virtual void SetFileMapPath(string fileMapPath)
		{
			this.fileMapPath = fileMapPath;
		}

		private IDictionary<string, int> app2Partition;

		public override void Init()
		{
			Initialize();
		}

		protected internal override object EvaluationInternal<_T0>(IDictionary<_T0> parameters
			)
		{
			return Calculate(parameters)[0];
		}

		public virtual int[] Calculate<_T0>(IDictionary<_T0> parameters)
		{
			int[] rst = new int[1];
			object arg = arguments[0].Evaluation(parameters);
			if (arg == null)
			{
				throw new ArgumentException("partition key is null ");
			}
			else
			{
				if (arg == Unevaluatable)
				{
					throw new ArgumentException("argument is UNEVALUATABLE");
				}
			}
			int pid = app2Partition[arg];
			if (pid == null)
			{
				rst[0] = defaultNode;
			}
			else
			{
				rst[0] = pid;
			}
			return rst;
		}

		public override FunctionExpression ConstructFunction(IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression
			> arguments)
		{
			if (arguments == null || arguments.Count != 1)
			{
				throw new ArgumentException("function " + GetFunctionName() + " must have 1 arguments but is "
					 + arguments);
			}
			object[] args = new object[arguments.Count];
			int i = -1;
			foreach (Tup.Cobar4Net.Parser.Ast.Expression.Expression arg in arguments)
			{
				args[++i] = arg;
			}
			return (FunctionExpression)ConstructMe(args);
		}

		public virtual RuleAlgorithm ConstructMe(params object[] objects)
		{
			IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression> args = new List<Tup.Cobar4Net.Parser.Ast.Expression.Expression
				>(objects.Length);
			foreach (object obj in objects)
			{
				args.Add((Tup.Cobar4Net.Parser.Ast.Expression.Expression)obj);
			}
			Tup.Cobar4Net.Route.Function.PartitionByFileMap rst = new Tup.Cobar4Net.Route.Function.PartitionByFileMap
				(functionName, args);
			rst.fileMapPath = fileMapPath;
			rst.defaultNode = defaultNode;
			return rst;
		}

		public virtual void Initialize()
		{
			InputStream fin = null;
			try
			{
				fin = new FileInputStream(new FilePath(fileMapPath));
				BufferedReader @in = new BufferedReader(new InputStreamReader(fin));
				app2Partition = new Dictionary<string, int>();
				for (string line = null; (line = @in.ReadLine()) != null; )
				{
					line = line.Trim();
					if (line.StartsWith("#") || line.StartsWith("//"))
					{
						continue;
					}
					int ind = line.IndexOf('=');
					if (ind < 0)
					{
						continue;
					}
					try
					{
						string key = Sharpen.Runtime.Substring(line, 0, ind).Trim();
						int pid = System.Convert.ToInt32(Sharpen.Runtime.Substring(line, ind + 1).Trim());
						app2Partition[key] = pid;
					}
					catch (Exception)
					{
					}
				}
			}
			catch (Exception e)
			{
				throw new RuntimeException(e);
			}
			finally
			{
				try
				{
					fin.Close();
				}
				catch (Exception)
				{
				}
			}
		}
	}
}
