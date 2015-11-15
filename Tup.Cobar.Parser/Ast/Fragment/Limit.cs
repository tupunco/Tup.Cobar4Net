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
using Tup.Cobar.Parser.Ast.Expression.Primary;
using Tup.Cobar.Parser.Visitor;

namespace Tup.Cobar.Parser.Ast.Fragment
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class Limit : ASTNode
    {
        /// <summary>when it is null, to sql generated must ignore this number</summary>
        private readonly Number offset;

        private readonly Number size;

        private readonly ParamMarker offsetP;

        private readonly ParamMarker sizeP;

        public Limit(Number offset, Number size)
        {
            if (offset == null)
            {
                throw new ArgumentException();
            }
            if (size == null)
            {
                throw new ArgumentException();
            }
            this.offset = offset;
            this.size = size;
            this.offsetP = null;
            this.sizeP = null;
        }

        public Limit(Number offset, ParamMarker sizeP)
        {
            if (offset == null)
            {
                throw new ArgumentException();
            }
            if (sizeP == null)
            {
                throw new ArgumentException();
            }
            this.offset = offset;
            this.size = null;
            this.offsetP = null;
            this.sizeP = sizeP;
        }

        public Limit(ParamMarker offsetP, Number size)
        {
            if (offsetP == null)
            {
                throw new ArgumentException();
            }
            if (size == null)
            {
                throw new ArgumentException();
            }
            this.offset = null;
            this.size = size;
            this.offsetP = offsetP;
            this.sizeP = null;
        }

        public Limit(ParamMarker offsetP, ParamMarker sizeP)
        {
            if (offsetP == null)
            {
                throw new ArgumentException();
            }
            if (sizeP == null)
            {
                throw new ArgumentException();
            }
            this.offset = null;
            this.size = null;
            this.offsetP = offsetP;
            this.sizeP = sizeP;
        }

        /// <returns>
        ///
        /// <see cref="Tup.Cobar.Parser.Ast.Expression.Primary.ParamMarker"/>
        /// or
        /// <see cref="Sharpen.Number"/>
        /// </returns>
        public virtual object GetOffset()
        {
            return offset == null ? (object)offsetP : (object)offset;
        }

        /// <returns>
        ///
        /// <see cref="Tup.Cobar.Parser.Ast.Expression.Primary.ParamMarker"/>
        /// or
        /// <see cref="Sharpen.Number"/>
        /// </returns>
        public virtual object GetSize()
        {
            return size == null ? (object)sizeP : (object)size;
        }

        public virtual void Accept(SQLASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}