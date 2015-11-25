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

using Sharpen;
using System;
using System.Collections.Generic;
using Tup.Cobar4Net.Parser.Util;

namespace Tup.Cobar4Net.Route.Hint
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public sealed class CobarHint
    {
        public const string CobarHintPrefix = "/*!cobar:";

        private static readonly IDictionary<string, HintParser>

#pragma warning disable RECS0104 // When object creation uses object or collection initializer, empty argument list is redundant
            HintParsers = new Dictionary<string, HintParser>()
            {
                {"table", new SimpleHintParser()},
                {"replica", new SimpleHintParser()},
                {"dataNodeId", new DataNodeHintParser()},
                {"partitionOperand", new PartitionOperandHintParser()}
            };

#pragma warning restore RECS0104 // When object creation uses object or collection initializer, empty argument list is redundant

        private int replica = RouteResultsetNode.DefaultReplicaIndex;

        private string table;

        private IList<Pair<int, int>> dataNodes;

        private Pair<string[], object[][]> partitionOperand;

        // index start from 1
        // /*!cobar: $dataNodeId=0.0, $table='offer'*/
        // /*!cobar: $dataNodeId=[0,1,5.2], $table='offer'*/
        // /*!cobar: $partitionOperand=('member_id'='m1'), $table='offer'*/
        // /*!cobar: $partitionOperand=('member_id'=['m1','m2']), $table='offer',
        // $replica=2*/
        // /*!cobar: $partitionOperand=(['offer_id','group_id']=[123,'3c']),
        // $table='offer'*/
        // /*!cobar:
        // $partitionOperand=(['offer_id','group_id']=[[123,'3c'],[234,'food']]),
        // $table='offer'*/
        /// <param name="offset">
        /// index of first char of
        /// <see cref="CobarHintPrefix"/>
        /// </param>
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        public static CobarHint ParserCobarHint(string sql, int offset)
        {
            CobarHint hint = new CobarHint();
            hint.currentIndex = offset + CobarHintPrefix.Length;
            hint.Parse(sql);
            return hint;
        }

        /// <returns>String[] in upper-case</returns>
        public Pair<string[], object[][]> GetPartitionOperand()
        {
            return partitionOperand;
        }

        public void SetPartitionOperand(Pair<string[], object[][]> partitionOperand)
        {
            string[] columns = partitionOperand.GetKey();
            if (columns == null)
            {
                this.partitionOperand = partitionOperand;
            }
            else
            {
                string[] colUp = new string[columns.Length];
                for (int i = 0; i < columns.Length; ++i)
                {
                    colUp[i] = columns[i].ToUpper();
                }
                this.partitionOperand = new Pair<string[], object[][]>(colUp, partitionOperand.GetValue());
            }
        }

        public IList<Pair<int, int>> GetDataNodes()
        {
            return dataNodes;
        }

        public void AddDataNode(int dataNodeIndex, int replica)
        {
            if (dataNodeIndex < 0)
            {
                throw new ArgumentException("data node index is null");
            }
            if (replica == RouteResultsetNode.DefaultReplicaIndex || replica < 0)
            {
                replica = -1;
            }
            if (dataNodes == null)
            {
                dataNodes = new List<Pair<int, int>>();
            }
            dataNodes.Add(new Pair<int, int>(dataNodeIndex, replica));
        }

        /// <returns>upper case</returns>
        public string GetTable()
        {
            return table;
        }

        public void SetTable(string table)
        {
            this.table = table.ToUpper();
        }

        public string Table
        {
            get { return table; }
            set { table = value.ToUpper(); }
        }

        public int Replica
        {
            get { return replica; }
            set { replica = value; }
        }

        public int GetReplica()
        {
            return replica;
        }

        public void SetReplica(int replica)
        {
            this.replica = replica;
        }

        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        private void Parse(string sql)
        {
            for (;;)
            {
                for (;;)
                {
                    switch (sql[currentIndex])
                    {
                        case '$':
                            {
                                goto skip_break;
                            }

                        case '*':
                            {
                                currentIndex += 2;
                                goto cobarHint_break;
                            }

                        default:
                            {
                                ++currentIndex;
                                break;
                            }
                    }
                }
            skip_break:;
                int hintNameEnd = sql.IndexOf('=', currentIndex);
                string hintName = Runtime.Substring(sql, currentIndex + 1, hintNameEnd).Trim();
                var hintParser = HintParsers.GetValue(hintName);
                if (hintParser != null)
                {
                    currentIndex = 1 + sql.IndexOf('=', hintNameEnd);
                    hintParser.Process(this, hintName, sql);
                }
                else
                {
                    throw new SQLSyntaxErrorException("unrecognized hint name: ${" + hintName + "}");
                }
            }
        cobarHint_break:;
            outputSql = Runtime.Substring(sql, currentIndex);
        }

        private string outputSql;

        private int currentIndex;

        public int GetCurrentIndex()
        {
            return currentIndex;
        }

        public CobarHint IncreaseCurrentIndex()
        {
            ++currentIndex;
            return this;
        }

        public void SetCurrentIndex(int ci)
        {
            currentIndex = ci;
        }

        public string GetOutputSql()
        {
            return outputSql;
        }
    }
}