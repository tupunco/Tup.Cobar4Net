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
using System.Linq;
using Sharpen;
using Tup.Cobar4Net.Config.Model.Rule;
using Tup.Cobar4Net.Parser.Ast.Expression;
using Tup.Cobar4Net.Parser.Ast.Expression.Primary.Function;
using Tup.Cobar4Net.Parser.Util;
using Tup.Cobar4Net.Route.Util;
using Tup.Cobar4Net.Util;

namespace Tup.Cobar4Net.Route.Function
{
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public sealed class Dimension2PartitionFunction : FunctionExpression, IRuleAlgorithm
    {
        private const int PartitionKeyTypeLong = 1;

        private const int PartitionKeyTypeString = 2;

        private int[] _all;

        private int[][] _byX;

        private int[][] _byY;

        private int[] _countX;

        private int[] _countY;

        private int _hashSliceEndX = 8;

        private int _hashSliceEndY = 8;

        private int _hashSliceStartX;

        private int _hashSliceStartY;

        private int _keyTypeX = PartitionKeyTypeLong;

        private int _keyTypeY = PartitionKeyTypeLong;

        private int[] _lengthX;

        private int[] _lengthY;

        private PartitionUtil _partitionUtilX;

        private PartitionUtil _partitionUtilY;

        private int _xSize;

        private int _ySize;

        private int m_DualHashLengthX;

        private int m_DualHashLengthY;

        private string m_DualHashSliceX = string.Empty;

        private string m_DualHashSliceY = string.Empty;

        private string m_DualPartitionCountX = string.Empty;

        private string m_DualPartitionCountY = string.Empty;

        private string m_DualPartitionLengthX = string.Empty;

        private string m_DualPartitionLengthY = string.Empty;

        public Dimension2PartitionFunction(string functionName)
            : this(functionName, null)
        {
        }

        public Dimension2PartitionFunction(string functionName, IList<IExpression> arguments)
            : base(functionName, arguments)
        {
        }

        public string KeyTypeX
        {
            get
            {
                return _keyTypeX == PartitionKeyTypeLong
                    ? "long"
                    : (_keyTypeX == PartitionKeyTypeString ? "string" : "error");
            }
            set { _keyTypeX = ConvertType(value); }
        }

        public string KeyTypeY
        {
            get
            {
                return _keyTypeY == PartitionKeyTypeLong
                    ? "long"
                    : (_keyTypeY == PartitionKeyTypeString ? "string" : "error");
            }
            set { _keyTypeY = ConvertType(value); }
        }

        public string PartitionCountX
        {
            get { return m_DualPartitionCountX; }
            set
            {
                m_DualPartitionCountX = value;

                _countX = ToIntArray(value);
                _xSize = 0;
                foreach (var c in _countX)
                {
                    _xSize += c;
                }
            }
        }

        public string PartitionLengthX
        {
            get { return m_DualPartitionLengthX; }
            set
            {
                m_DualPartitionLengthX = value;
                _lengthX = ToIntArray(value);
            }
        }

        public int HashLengthX
        {
            get { return m_DualHashLengthX; }
            set
            {
                m_DualHashLengthX = value;

                HashSliceX = value.ToString();
            }
        }

        public string HashSliceX
        {
            get { return m_DualHashSliceX; }
            set
            {
                m_DualHashSliceX = value;

                var p = PairUtil.SequenceSlicing(value);
                _hashSliceStartX = p.Key;
                _hashSliceEndX = p.Value;
            }
        }

        public string PartitionCountY
        {
            get { return m_DualPartitionCountY; }
            set
            {
                m_DualPartitionCountY = value;

                _countY = ToIntArray(value);
                _ySize = 0;
                foreach (var c in _countY)
                {
                    _ySize += c;
                }
            }
        }

        public string PartitionLengthY
        {
            get { return m_DualPartitionLengthY; }
            set
            {
                m_DualPartitionLengthY = value;

                _lengthY = ToIntArray(value);
            }
        }

        public int HashLengthY
        {
            get { return m_DualHashLengthY; }
            set
            {
                m_DualHashLengthY = value;

                HashSliceY = value.ToString();
            }
        }

        public string HashSliceY
        {
            get { return m_DualHashSliceY; }
            set
            {
                m_DualHashSliceY = value;

                var p = PairUtil.SequenceSlicing(value);
                _hashSliceStartY = p.Key;
                _hashSliceEndY = p.Value;
            }
        }

        public void Initialize()
        {
            _partitionUtilX = new PartitionUtil(_countX, _lengthX);
            _partitionUtilY = new PartitionUtil(_countY, _lengthY);
            BuildAll();
            BuildByX();
            BuildByY();
        }

        public IRuleAlgorithm ConstructMe(params object[] objects)
        {
            var args = objects.Select(x => (IExpression)x).ToList();

            return new Dimension2PartitionFunction(functionName, args)
            {
                _countX = _countX,
                _xSize = _xSize,
                _lengthX = _lengthX,
                _keyTypeX = _keyTypeX,
                _hashSliceStartX = _hashSliceStartX,
                _hashSliceEndX = _hashSliceEndX,
                _countY = _countY,
                _ySize = _ySize,
                _lengthY = _lengthY,
                _keyTypeY = _keyTypeY,
                _hashSliceStartY = _hashSliceStartY,
                _hashSliceEndY = _hashSliceEndY
            };
        }

        public Number[] Calculate(IDictionary<object, object> parameters)
        {
            if (arguments == null || arguments.Count < 2)
            {
                throw new ArgumentException("arguments.size < 2 for function of " + FunctionName);
            }
            var xInput = arguments[0].Evaluation(parameters);
            var yInput = arguments[1].Evaluation(parameters);

            return Eval(xInput, yInput);
        }

        private static int ConvertType(string keyType)
        {
            if (Runtime.EqualsIgnoreCase("long", keyType))
            {
                return PartitionKeyTypeLong;
            }
            if (Runtime.EqualsIgnoreCase("string", keyType))
            {
                return PartitionKeyTypeString;
            }
            throw new ArgumentException("unknown partition key type: " + keyType);
        }


        private static int[] ToIntArray(string @string)
        {
            var strs = SplitUtil.Split(@string, ',', true);
            var ints = new int[strs.Length];
            for (var i = 0; i < strs.Length; ++i)
            {
                ints[i] = Convert.ToInt32(strs[i]);
            }
            return ints;
        }

        private void BuildByX()
        {
            //_byX = new int[_xSize][_ySize];
            _byX = new int[_xSize][];
            for (var x = 0; x < _xSize; ++x)
            {
                _byX[x] = new int[_ySize];

                for (var y = 0; y < _ySize; ++y)
                {
                    _byX[x][y] = GetByXY(x, y);
                }
            }
        }

        private void BuildByY()
        {
            //_byY = new int[_ySize][_xSize];
            _byY = new int[_ySize][];
            for (var y = 0; y < _ySize; ++y)
            {
                _byY[y] = new int[_xSize];

                for (var x = 0; x < _xSize; ++x)
                {
                    _byY[y][x] = GetByXY(x, y);
                }
            }
        }

        private void BuildAll()
        {
            var size = _xSize*_ySize;
            _all = new int[size];
            for (var i = 0; i < size; ++i)
            {
                _all[i] = i;
            }
        }

        private int[] GetAll()
        {
            return _all;
        }

        private int[] GetByX(int x)
        {
            return _byX[x];
        }

        private int[] GetByY(int y)
        {
            return _byY[y];
        }

        private int GetByXY(int x, int y)
        {
            if (x >= _xSize || y >= _ySize)
            {
                throw new ArgumentException("x, y out of bound: x=" + x + ", y=" + y);
            }
            return x + _xSize*y;
        }

        /// <returns>null if eval invalid type</returns>
        private static int? Calculate(object eval,
            PartitionUtil partitionUtil,
            int keyType,
            int hashSliceStart,
            int hashSliceEnd)
        {
            if (eval == ExpressionConstants.Unevaluatable || eval == null)
                return null;

            switch (keyType)
            {
                case PartitionKeyTypeLong:
                {
                    long longVal;
                    if (eval is Number)
                    {
                        longVal = (long)(Number)eval;
                    }
                    else if (eval is string)
                    {
                        longVal = long.Parse((string)eval);
                    }
                    else
                    {
                        throw new ArgumentException("unsupported data type for partition key: " + eval.GetType());
                    }

                    return partitionUtil.Partition(longVal);
                }

                case PartitionKeyTypeString:
                {
                    var key = eval.ToString();
                    var start = hashSliceStart >= 0 ? hashSliceStart : key.Length + hashSliceStart;
                    var end = hashSliceEnd > 0 ? hashSliceEnd : key.Length + hashSliceEnd;
                    var hash = StringUtil.Hash(key, start, end);
                    return partitionUtil.Partition(hash);
                }

                default:
                {
                    throw new ArgumentException("unsupported partition key type: " + keyType);
                }
            }
        }

        protected override object EvaluationInternal(IDictionary<object, object> parameters)
        {
            return Calculate(parameters);
        }

        private Number[] Eval(object xInput, object yInput)
        {
            var x = Calculate(xInput, _partitionUtilX, _keyTypeX, _hashSliceStartX, _hashSliceEndX);
            var y = Calculate(yInput, _partitionUtilY, _keyTypeY, _hashSliceStartY, _hashSliceEndY);
            if (x != null && y != null)
            {
                return new Number[] {GetByXY(x.Value, y.Value)};
            }
            if (x == null && y != null)
            {
                return Number.ValueOf(GetByY(y.Value));
            }
            if (x != null && y == null)
            {
                return Number.ValueOf(GetByX(x.Value));
            }
            return Number.ValueOf(GetAll());
        }

        public override void Init()
        {
            Initialize();
        }

        public override FunctionExpression ConstructFunction(IList<IExpression> arguments)
        {
            if (arguments == null || arguments.Count != 2)
            {
                throw new ArgumentException("function " + FunctionName + " must have 2 arguments but is " +
                                            arguments);
            }

            var args = new object[arguments.Count];
            var i = -1;
            foreach (var arg in arguments)
            {
                args[++i] = arg;
            }
            return (FunctionExpression)ConstructMe(args);
        }

        // }
        // }
        // System.out.println(i);
        // for(Integer i:ints){
        // Integer[] ints=func.eval(1023L, "zzzz");
        //
        // func.init();
        // func.setHashLengthY(8);
        // func.setPartitionLengthY("512");
        // func.setPartitionCountY("2");
        // func.setKeyTypeY("string");
        // func.setPartitionLengthX("512,256");
        // func.setPartitionCountX("1,2");
        // func.setKeyTypeX("long");
        // Dimension2PartitionFunction("test999", new ArrayList<Expression>(2));
        // Dimension2PartitionFunction func = new

        // public static void main(String[] args) throws Exception {
    }
}