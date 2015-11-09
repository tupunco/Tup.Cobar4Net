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
using System.Text;
using Tup.Cobar.Parser.Visitor;

namespace Tup.Cobar.Parser.Ast.Expression.Primary
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class Identifier : PrimaryExpression
    {
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
                throw new ArgumentException("id start with a '`' must end with a '`', id: " + name
                    );
            }
            StringBuilder sb = new StringBuilder(name.Length - 2);
            int endIndex = name.Length - 1;
            bool hold = false;
            for (int i = 1; i < endIndex; ++i)
            {
                char c = name[i];
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

        /// <summary>null if no parent</summary>
        protected internal Identifier parent;

        /// <summary>e.g.</summary>
        /// <remarks>e.g. "id1", "`id1`"</remarks>
        protected internal readonly string idText;

        protected internal readonly string idTextUpUnescape;

        public Identifier(Identifier parent, string idText)
            : this(parent, idText, idText.ToUpper())
        {
        }

        public Identifier(Identifier parent, string idText, string idTextUp)
        {
            this.parent = parent;
            this.idText = idText;
            this.idTextUpUnescape = UnescapeName(idTextUp);
        }

        public virtual string GetLevelUnescapeUpName(int level)
        {
            Identifier id = this;
            for (int i = level; i > 1 && id != null; --i)
            {
                id = id.parent;
            }
            if (id != null)
            {
                return id.idTextUpUnescape;
            }
            return null;
        }

        /// <summary>trim not happen because parent in given level is not exist</summary>
        public const int ParentAbsent = 0;

        /// <summary>trim happen</summary>
        public const int ParentTrimed = 1;

        /// <summary>trim not happen because parent in given not equals to given name</summary>
        public const int ParentIgnored = 2;

        /// <param name="level">
        /// At most how many levels left after trim, must be a positive
        /// integer. e.g. level = 2 for "schema1.tb1.c1", "tb1.c1" is left
        /// </param>
        /// <param name="trimSchema">
        /// upper-case. Assumed that top trimmed parent is schema,
        /// if that equals given schema, do not trim
        /// </param>
        /// <returns>
        ///
        /// <see cref="ParentAbsent"/>
        /// or
        /// <see cref="ParentTrimed"/>
        /// or
        /// <see cref="ParentIgnored"/>
        /// </returns>
        public virtual int TrimParent(int level, string trimSchema)
        {
            Identifier id = this;
            for (int i = 1; i < level; ++i)
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
            else
            {
                id.parent = null;
                return ParentTrimed;
            }
        }

        public virtual void SetParent(Identifier
            parent)
        {
            this.parent = parent;
        }

        public virtual Identifier GetParent()
        {
            return parent;
        }

        public virtual string GetIdText()
        {
            return idText;
        }

        public virtual string GetIdTextUpUnescape()
        {
            return idTextUpUnescape;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("ID:");
            if (parent != null)
            {
                sb.Append(parent).Append('.');
            }
            return sb.Append(idText).ToString();
        }

        public override int GetHashCode()
        {
            int constant = 37;
            int hash = 17;
            if (parent == null)
            {
                hash += constant;
            }
            else
            {
                hash = hash * constant + parent.GetHashCode();
            }
            if (idText == null)
            {
                hash += constant;
            }
            else
            {
                hash = hash * constant + idText.GetHashCode();
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
                Identifier that = (Identifier
                    )obj;
                return ObjEquals(this.parent, that.parent) && ObjEquals(this.idText, that.idText);
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

        public override void Accept(SQLASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}