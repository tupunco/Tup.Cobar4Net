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
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Tup.Cobar4Net.Parser.Visitor;

namespace Tup.Cobar4Net.Parser.Ast.Expression.Primary
{
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public class Identifier : PrimaryExpression
    {
        /// <summary>trim not happen because parent in given level is not exist</summary>
        public const int ParentAbsent = 0;

        /// <summary>trim happen</summary>
        public const int ParentTrimed = 1;

        /// <summary>trim not happen because parent in given not equals to given name</summary>
        public const int ParentIgnored = 2;

        /// <summary>e.g.</summary>
        /// <remarks>e.g. "id1", "`id1`"</remarks>
        protected readonly string idText;

        protected readonly string idTextUpUnescape;

        /// <summary>null if no parent</summary>
        protected Identifier parent;

        public Identifier(Identifier parent, string idText)
            : this(parent, idText, idText.ToUpper())
        {
        }

        public Identifier(Identifier parent, string idText, string idTextUp)
        {
            this.parent = parent;
            this.idText = idText;
            idTextUpUnescape = UnescapeName(idTextUp);
        }

        public virtual Identifier Parent
        {
            set { parent = value; }
            get { return parent; }
        }

        public virtual string IdText
        {
            get { return idText; }
        }

        public virtual string IdTextUpUnescape
        {
            get { return idTextUpUnescape; }
        }

        public static string UnescapeName(string name)
        {
            return UnescapeName(name, false);
        }

        public static string UnescapeName(string name, bool toUppercase)
        {
            if (name == null || name.Length <= 0)
            {
                return name;
            }
            if (name[0] != '`')
            {
                return toUppercase ? name.ToUpper() : name;
            }
            if (name[name.Length - 1] != '`')
            {
                throw new ArgumentException("id start with a '`' must end with a '`', id: " + name);
            }
            var sb = new StringBuilder(name.Length - 2);
            var endIndex = name.Length - 1;
            var hold = false;
            for (var i = 1; i < endIndex; ++i)
            {
                var c = name[i];
                if (c == '`' && !hold)
                {
                    hold = true;
                    continue;
                }
                hold = false;
                if (toUppercase && c >= 'a' && c <= 'z')
                {
                    c -= (char)32;
                }
                sb.Append(c);
            }
            return sb.ToString();
        }

        public virtual string GetLevelUnescapeUpName(int level)
        {
            var id = this;
            for (var i = level; i > 1 && id != null; --i)
            {
                id = id.parent;
            }
            if (id != null)
            {
                return id.idTextUpUnescape;
            }
            return null;
        }

        /// <param name="level">
        ///     At most how many levels left after trim, must be a positive
        ///     integer. e.g. level = 2 for "schema1.tb1.c1", "tb1.c1" is left
        /// </param>
        /// <param name="trimSchema">
        ///     upper-case. Assumed that top trimmed parent is schema,
        ///     if that equals given schema, do not trim
        /// </param>
        /// <returns>
        ///     <see cref="ParentAbsent" />
        ///     or
        ///     <see cref="ParentTrimed" />
        ///     or
        ///     <see cref="ParentIgnored" />
        /// </returns>
        public virtual int TrimParent(int level, string trimSchema)
        {
            var id = this;
            for (var i = 1; i < level; ++i)
            {
                if (id.parent == null)
                {
                    return ParentAbsent;
                }
                id = id.parent;
            }
            if (id.parent == null)
            {
                return ParentAbsent;
            }
            if (trimSchema != null && !trimSchema.Equals(id.parent.idTextUpUnescape))
            {
                return ParentIgnored;
            }
            id.parent = null;
            return ParentTrimed;
        }

        public override string ToString()
        {
            var sb = new StringBuilder("ID:");
            if (parent != null)
            {
                sb.Append(parent).Append('.');
            }
            return sb.Append(idText).ToString();
        }

        [SuppressMessage("Potential Code Quality Issues", "RECS0025", Justification = "<¹ÒÆð>")]
        public override int GetHashCode()
        {
            var constant = 37;
            var hash = 17;
            if (parent == null)
            {
                hash += constant;
            }
            else
            {
                hash = hash*constant + parent.GetHashCode();
            }
            if (idText == null)
            {
                hash += constant;
            }
            else
            {
                hash = hash*constant + idText.GetHashCode();
            }
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }
            if (obj is Identifier)
            {
                var that = (Identifier)obj;
                return ObjEquals(parent, that.parent) && ObjEquals(idText, that.idText);
            }
            return false;
        }

        private static bool ObjEquals(object obj, object obj2)
        {
            if (obj == obj2)
            {
                return true;
            }
            if (obj == null)
            {
                return obj2 == null;
            }
            return obj.Equals(obj2);
        }

        public override void Accept(ISqlAstVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}