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

namespace Tup.Cobar.Parser.Util
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public sealed class ListUtil
	{
		public static IList<TItem> CreateList<TItem>(params TItem[] objs)
		{
			return CreateList(new List<TItem>(), objs);
		}

		public static IList<TItem> CreateList<TItem>(IList<TItem> list, params TItem[] objs)
		{
			if (objs != null)
			{
				foreach (var obj in objs)
				{
					list.Add(obj);
				}
			}
			return list;
		}

		public static bool IsEquals<TItem>(IList<TItem> l1, IList<TItem> l2)
		{
			if (l1 == l2)
			{
				return true;
			}
			if (l1 == null)
			{
				return l2 == null;
			}
			if (l2 == null)
			{
				return false;
			}
			if (l1.Count != l2.Count)
			{
				return false;
			}
			var iter1 = l1.GetEnumerator();
			var iter2 = l2.GetEnumerator();
			while (iter1.MoveNext())
			{
				object o1 = iter1.Current;
				object o2 = iter2.Current;
				if (o1 == o2)
				{
					continue;
				}
				if (o1 == null && o2 != null)
				{
					return false;
				}
				if (!o1.Equals(o2))
				{
					return false;
				}
			}
			return true;
		}
	}
}
