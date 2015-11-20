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
using Tup.Cobar4Net.Parser.Util;
using Tup.Cobar4Net.Route.Util;
using Tup.Cobar4Net.Util;

namespace Tup.Cobar4Net.Route.Function
{
	/// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
	public sealed class Dimension2PartitionFunction : FunctionExpression, RuleAlgorithm
	{
		public Dimension2PartitionFunction(string functionName)
			: this(functionName, null)
		{
		}

		public Dimension2PartitionFunction(string functionName, IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression
			> arguments)
			: base(functionName, arguments)
		{
		}

		private const int PartitionKeyTypeLong = 1;

		private const int PartitionKeyTypeString = 2;

		private int[] countX;

		private int[] lengthX;

		private int[] countY;

		private int[] lengthY;

		private static int ConvertType(string keyType)
		{
			if (Sharpen.Runtime.EqualsIgnoreCase("long", keyType))
			{
				return PartitionKeyTypeLong;
			}
			if (Sharpen.Runtime.EqualsIgnoreCase("string", keyType))
			{
				return PartitionKeyTypeString;
			}
			throw new ArgumentException("unknown partition key type: " + keyType);
		}

		public void SetKeyTypeX(string keyTypeX)
		{
			this.keyTypeX = ConvertType(keyTypeX);
		}

		public void SetKeyTypeY(string keyTypeY)
		{
			this.keyTypeY = ConvertType(keyTypeY);
		}

		public void SetPartitionCountX(string partitionCount)
		{
			this.countX = ToIntArray(partitionCount);
			this.xSize = 0;
			foreach (int c in countX)
			{
				this.xSize += c;
			}
		}

		public void SetPartitionLengthX(string partitionLength)
		{
			this.lengthX = ToIntArray(partitionLength);
		}

		public void SetHashLengthX(int hashLengthX)
		{
			SetHashSliceX(hashLengthX.ToString());
		}

		public void SetHashSliceX(string hashSlice)
		{
			Pair<int, int> p = PairUtil.SequenceSlicing(hashSlice);
			hashSliceStartX = p.GetKey();
			hashSliceEndX = p.GetValue();
		}

		public void SetPartitionCountY(string partitionCount)
		{
			this.countY = ToIntArray(partitionCount);
			this.ySize = 0;
			foreach (int c in countY)
			{
				this.ySize += c;
			}
		}

		public void SetPartitionLengthY(string partitionLength)
		{
			this.lengthY = ToIntArray(partitionLength);
		}

		public void SetHashLengthY(int hashLengthY)
		{
			SetHashSliceY(hashLengthY.ToString());
		}

		public void SetHashSliceY(string hashSlice)
		{
			Pair<int, int> p = PairUtil.SequenceSlicing(hashSlice);
			hashSliceStartY = p.GetKey();
			hashSliceEndY = p.GetValue();
		}

		private static int[] ToIntArray(string @string)
		{
			string[] strs = SplitUtil.Split(@string, ',', true);
			int[] ints = new int[strs.Length];
			for (int i = 0; i < strs.Length; ++i)
			{
				ints[i] = System.Convert.ToInt32(strs[i]);
			}
			return ints;
		}

		private int xSize;

		private int keyTypeX = PartitionKeyTypeLong;

		private int hashSliceStartX = 0;

		private int hashSliceEndX = 8;

		private PartitionUtil partitionUtilX;

		private int ySize;

		private int keyTypeY = PartitionKeyTypeLong;

		private int hashSliceStartY = 0;

		private int hashSliceEndY = 8;

		private PartitionUtil partitionUtilY;

		private int[][] byX;

		private int[][] byY;

		private int[] all;

		private void BuildByX()
		{
			//byX = new int[xSize][ySize];
			byX = new int[xSize][];
			for (int x = 0; x < xSize; ++x)
			{
				for (int y = 0; y < ySize; ++y)
				{
					byX[x][y] = GetByXY(x, y);
				}
			}
		}

		private void BuildByY()
		{
			//byY = new int[ySize][xSize];
			byY = new int[ySize][];
			for (int y = 0; y < ySize; ++y)
			{
				for (int x = 0; x < xSize; ++x)
				{
					byY[y][x] = GetByXY(x, y);
				}
			}
		}

		private void BuildAll()
		{
			int size = xSize * ySize;
			all = new int[size];
			for (int i = 0; i < size; ++i)
			{
				all[i] = i;
			}
		}

		private int[] GetAll()
		{
			return all;
		}

		private int[] GetByX(int x)
		{
			return byX[x];
		}

		private int[] GetByY(int y)
		{
			return byY[y];
		}

		private int GetByXY(int x, int y)
		{
			if (x >= xSize || y >= ySize)
			{
				throw new ArgumentException("x, y out of bound: x=" + x + ", y=" + y);
			}
			return x + xSize * y;
		}

		/// <returns>null if eval invalid type</returns>
		private static int Calculate(object eval, PartitionUtil partitionUtil, int keyType
			, int hashSliceStart, int hashSliceEnd)
		{
			if (eval == Unevaluatable || eval == null)
			{
				return null;
			}
			switch (keyType)
			{
				case PartitionKeyTypeLong:
				{
					long longVal;
					if (eval is Number)
					{
						longVal = ((Number)eval);
					}
					else
					{
						if (eval is string)
						{
							longVal = long.Parse((string)eval);
						}
						else
						{
							throw new ArgumentException("unsupported data type for partition key: " + eval.GetType
								());
						}
					}
					return partitionUtil.Partition(longVal);
				}

				case PartitionKeyTypeString:
				{
					string key = eval.ToString();
					int start = hashSliceStart >= 0 ? hashSliceStart : key.Length + hashSliceStart;
					int end = hashSliceEnd > 0 ? hashSliceEnd : key.Length + hashSliceEnd;
					long hash = StringUtil.Hash(key, start, end);
					return partitionUtil.Partition(hash);
				}

				default:
				{
					throw new ArgumentException("unsupported partition key type: " + keyType);
				}
			}
		}

		protected internal override object EvaluationInternal<_T0>(IDictionary<_T0> parameters
			)
		{
			return Calculate(parameters);
		}

		private int[] Eval(object xInput, object yInput)
		{
			int x = Calculate(xInput, partitionUtilX, keyTypeX, hashSliceStartX, hashSliceEndX
				);
			int y = Calculate(yInput, partitionUtilY, keyTypeY, hashSliceStartY, hashSliceEndY
				);
			if (x != null && y != null)
			{
				return new int[] { GetByXY(x, y) };
			}
			else
			{
				if (x == null && y != null)
				{
					return GetByY(y);
				}
				else
				{
					if (x != null && y == null)
					{
						return GetByX(x);
					}
					else
					{
						return GetAll();
					}
				}
			}
		}

		public override void Init()
		{
			Initialize();
		}

		public void Initialize()
		{
			partitionUtilX = new PartitionUtil(countX, lengthX);
			partitionUtilY = new PartitionUtil(countY, lengthY);
			BuildAll();
			BuildByX();
			BuildByY();
		}

		public override FunctionExpression ConstructFunction(IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression
			> arguments)
		{
			if (arguments == null || arguments.Count != 2)
			{
				throw new ArgumentException("function " + GetFunctionName() + " must have 2 arguments but is "
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

		public RuleAlgorithm ConstructMe(params object[] objects)
		{
			IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression> args = new List<Tup.Cobar4Net.Parser.Ast.Expression.Expression
				>(objects.Length);
			foreach (object obj in objects)
			{
				args.Add((Tup.Cobar4Net.Parser.Ast.Expression.Expression)obj);
			}
			Tup.Cobar4Net.Route.Function.Dimension2PartitionFunction rst = new Tup.Cobar4Net.Route.Function.Dimension2PartitionFunction
				(functionName, args);
			rst.countX = countX;
			rst.xSize = xSize;
			rst.lengthX = lengthX;
			rst.keyTypeX = keyTypeX;
			rst.hashSliceStartX = hashSliceStartX;
			rst.hashSliceEndX = hashSliceEndX;
			rst.countY = countY;
			rst.ySize = ySize;
			rst.lengthY = lengthY;
			rst.keyTypeY = keyTypeY;
			rst.hashSliceStartY = hashSliceStartY;
			rst.hashSliceEndY = hashSliceEndY;
			return rst;
		}

		public int[] Calculate<_T0>(IDictionary<_T0> parameters)
		{
			if (arguments == null || arguments.Count < 2)
			{
				throw new ArgumentException("arguments.size < 2 for function of " + GetFunctionName
					());
			}
			object xInput = arguments[0].Evaluation(parameters);
			object yInput = arguments[1].Evaluation(parameters);
			// return (Integer[])eval(xInput, yInput);
			return null;
		}
		// public static void main(String[] args) throws Exception {
		// Dimension2PartitionFunction func = new
		// Dimension2PartitionFunction("test999", new ArrayList<Expression>(2));
		// func.setKeyTypeX("long");
		// func.setPartitionCountX("1,2");
		// func.setPartitionLengthX("512,256");
		// func.setKeyTypeY("string");
		// func.setPartitionCountY("2");
		// func.setPartitionLengthY("512");
		// func.setHashLengthY(8);
		// func.init();
		//
		// Integer[] ints=func.eval(1023L, "zzzz");
		// for(Integer i:ints){
		// System.out.println(i);
		// }
		// }
	}
}
