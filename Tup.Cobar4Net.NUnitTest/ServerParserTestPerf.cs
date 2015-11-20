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
using Tup.Cobar.Server.Parser;

namespace Tup.Cobar4Net.Parser
{
	/// <author>xianmao.hexm</author>
	public sealed class ServerParserTestPerf
	{
		private static void ParseSetPerf()
		{
			// ServerParse.parse("show databases");
			// ServerParseSet.parse("set autocommit=1");
			// ServerParseSet.parse("set names=1");
			ServerParseSet.Parse("SET character_set_results = NULL", 4);
		}

		// ServerParse.parse("select id,name,value from t");
		// ServerParse.parse("select * from offer where member_id='abc'");
		public static void Main(string[] args)
		{
			ParseSetPerf();
			int count = 10000000;
			Runtime.CurrentTimeMillis();
			long t1 = Runtime.CurrentTimeMillis();
			for (int i = 0; i < count; i++)
			{
				ParseSetPerf();
			}
			long t2 = Runtime.CurrentTimeMillis();
			// print time
			System.Console.Out.WriteLine("take:" + ((t2 - t1) * 1000 * 1000) / count + " ns."
				);
		}
	}
}
