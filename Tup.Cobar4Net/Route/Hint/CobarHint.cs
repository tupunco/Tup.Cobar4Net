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
using Tup.Cobar4Net.Parser.Util;

namespace Tup.Cobar4Net.Route.Hint
{
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public sealed class CobarHint
    {
        public const string CobarHintPrefix = "/*!cobar:";

        private static readonly IDictionary<string, HintParser>
#pragma warning disable RECS0104 // When object creation uses object or collection initializer, empty argument list is redundant
            HintParsers = new Dictionary<string, HintParser>
                          {
                              {"table", new SimpleHintParser()},
                              {"replica", new SimpleHintParser()},
                              {"dataNodeId", new DataNodeHintParser()},
                              {"partitionOperand", new PartitionOperandHintParser()}
                          };

#pragma warning restore RECS0104 // When object creation uses object or collection initializer, empty argument list is redundant

        private Pair<string[], object[][]> _partitionOperand;

        private string _table;

        /// <value>String[] in upper-case</value>
        public Pair<string[], object[][]> PartitionOperand
        {
            get { return _partitionOperand; }
            set
            {
                var columns = value.Key;
                if (columns == null)
                {
                    _partitionOperand = value;
                }
                else
                {
                    var colUp = new string[columns.Length];
                    for (var i = 0; i < columns.Length; ++i)
                    {
                        colUp[i] = columns[i].ToUpper();
                    }
                    _partitionOperand = new Pair<string[], object[][]>(colUp, value.Value);
                }
            }
        }

        public IList<Pair<int, int>> DataNodes { get; private set; }

        /// <value>upper case</value>
        public string Table
        {
            get { return _table; }
            set { _table = value.ToUpper(); }
        }

        public int Replica { get; set; } = RouteResultsetNode.DefaultReplicaIndex;

        public int CurrentIndex { set; get; }

        public string OutputSql { get; private set; }

        // index start from 1
        // /*!cobar: $dataNodeId=0.0, $_table='offer'*/
        // /*!cobar: $dataNodeId=[0,1,5.2], $_table='offer'*/
        // /*!cobar: $_partitionOperand=('member_id'='m1'), $_table='offer'*/
        // /*!cobar: $_partitionOperand=('member_id'=['m1','m2']), $_table='offer',
        // $_replica=2*/
        // /*!cobar: $_partitionOperand=(['offer_id','group_id']=[123,'3c']),
        // $_table='offer'*/
        // /*!cobar:
        // $_partitionOperand=(['offer_id','group_id']=[[123,'3c'],[234,'food']]),
        // $_table='offer'*/
        /// <param name="offset">
        ///     index of first char of
        ///     <see cref="CobarHintPrefix" />
        /// </param>
        /// <exception cref="System.SqlSyntaxErrorException" />
        public static CobarHint ParserCobarHint(string sql, int offset)
        {
            var hint = new CobarHint
                       {
                           CurrentIndex = offset + CobarHintPrefix.Length
                       };
            hint.Parse(sql);
            return hint;
        }

        public void AddDataNode(int dataNodeIndex, int replica)
        {
            if (dataNodeIndex < 0)
                throw new ArgumentException("data node index is null");

            if (replica == RouteResultsetNode.DefaultReplicaIndex || replica < 0)
                replica = -1;

            if (DataNodes == null)
                DataNodes = new List<Pair<int, int>>();

            DataNodes.Add(new Pair<int, int>(dataNodeIndex, replica));
        }

        /// <exception cref="System.SqlSyntaxErrorException" />
        private void Parse(string sql)
        {
            for (;;)
            {
                for (;;)
                {
                    switch (sql[CurrentIndex])
                    {
                        case '$':
                        {
                            goto skip_break;
                        }

                        case '*':
                        {
                            CurrentIndex += 2;
                            goto cobarHint_break;
                        }

                        default:
                        {
                            ++CurrentIndex;
                            break;
                        }
                    }
                }
                skip_break:
                ;
                var hintNameEnd = sql.IndexOf('=', CurrentIndex);
                var hintName = Runtime.Substring(sql, CurrentIndex + 1, hintNameEnd).Trim();
                var hintParser = HintParsers.GetValue(hintName);
                if (hintParser != null)
                {
                    CurrentIndex = 1 + sql.IndexOf('=', hintNameEnd);
                    hintParser.Process(this, hintName, sql);
                }
                else
                {
                    throw new SqlSyntaxErrorException("unrecognized hint name: ${" + hintName + "}");
                }
            }
            cobarHint_break:
            ;
            OutputSql = Runtime.Substring(sql, CurrentIndex);
        }

        public CobarHint IncreaseCurrentIndex()
        {
            ++CurrentIndex;
            return this;
        }
    }
}