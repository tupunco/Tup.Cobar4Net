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
using Tup.Cobar4Net.Parser.Ast.Stmt.Mts;
using Tup.Cobar4Net.Parser.Recognizer.Mysql.Lexer;

namespace Tup.Cobar4Net.Parser.Recognizer.Mysql.Syntax
{
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public class MySqlMtsParser : MySqlParser
    {
        private static readonly IDictionary<string, SpecialIdentifier> specialIdentifiers =
            new Dictionary<string, SpecialIdentifier>();

        static MySqlMtsParser()
        {
            specialIdentifiers["SAVEPOINT"] = SpecialIdentifier.Savepoint;
            specialIdentifiers["WORK"] = SpecialIdentifier.Work;
            specialIdentifiers["CHAIN"] = SpecialIdentifier.Chain;
            specialIdentifiers["RELEASE"] = SpecialIdentifier.Release;
            specialIdentifiers["NO"] = SpecialIdentifier.No;
        }

        public MySqlMtsParser(MySqlLexer lexer)
            : base(lexer)
        {
        }

        /// <summary>first token <code>SAVEPOINT</code> is scanned but not yet consumed</summary>
        /// <exception cref="System.SqlSyntaxErrorException" />
        public virtual MTSSavepointStatement Savepoint()
        {
            // matchIdentifier("SAVEPOINT"); // for performance issue, change to
            // follow:
            lexer.NextToken();
            var id = Identifier();
            Match(MySqlToken.Eof);
            return new MTSSavepointStatement(id);
        }

        /// <summary>first token <code>RELEASE</code> is scanned but not yet consumed</summary>
        /// <exception cref="System.SqlSyntaxErrorException" />
        public virtual MTSReleaseStatement Release()
        {
            Match(MySqlToken.KwRelease);
            MatchIdentifier("SAVEPOINT");
            var id = Identifier();
            Match(MySqlToken.Eof);
            return new MTSReleaseStatement(id);
        }

        /// <summary>
        ///     first token <code>ROLLBACK</code> is scanned but not yet consumed
        ///     <pre>
        ///         ROLLBACK [WORK] TO [SAVEPOINT] identifier
        ///         ROLLBACK [WORK] [AND [NO] CHAIN | [NO] RELEASE]
        ///     </pre>
        /// </summary>
        /// <exception cref="System.SqlSyntaxErrorException" />
        public virtual MTSRollbackStatement Rollback()
        {
            // matchIdentifier("ROLLBACK"); // for performance issue, change to
            // follow:
            lexer.NextToken();
            var siTemp = specialIdentifiers.GetValue(lexer.GetStringValueUppercase());
            if (siTemp == SpecialIdentifier.Work)
            {
                lexer.NextToken();
            }
            switch (lexer.Token())
            {
                case MySqlToken.Eof:
                {
                    return new MTSRollbackStatement(CompleteType.UnDef);
                }

                case MySqlToken.KwTo:
                {
                    lexer.NextToken();
                    if (specialIdentifiers.GetValue(lexer.GetStringValueUppercase()) == SpecialIdentifier.Savepoint)
                    {
                        lexer.NextToken();
                    }
                    var savepoint = Identifier();
                    Match(MySqlToken.Eof);
                    return new MTSRollbackStatement(savepoint);
                }

                case MySqlToken.KwAnd:
                {
                    lexer.NextToken();
                    siTemp = specialIdentifiers.GetValue(lexer.GetStringValueUppercase());
                    if (siTemp == SpecialIdentifier.No)
                    {
                        lexer.NextToken();
                        MatchIdentifier("CHAIN");
                        Match(MySqlToken.Eof);
                        return new MTSRollbackStatement(CompleteType.NoChain);
                    }
                    MatchIdentifier("CHAIN");
                    Match(MySqlToken.Eof);
                    return new MTSRollbackStatement(CompleteType.Chain);
                }

                case MySqlToken.KwRelease:
                {
                    lexer.NextToken();
                    Match(MySqlToken.Eof);
                    return new MTSRollbackStatement(CompleteType.Release);
                }

                case MySqlToken.Identifier:
                {
                    siTemp = specialIdentifiers.GetValue(lexer.GetStringValueUppercase());
                    if (siTemp == SpecialIdentifier.No)
                    {
                        lexer.NextToken();
                        Match(MySqlToken.KwRelease);
                        Match(MySqlToken.Eof);
                        return new MTSRollbackStatement(CompleteType.NoRelease);
                    }
                    goto default;
                }

                default:
                {
                    throw Err("unrecognized complete type: " + lexer.Token());
                }
            }
        }

        /// <summary>
        ///     MySqlMtsParser SpecialIdentifier
        /// </summary>
        private enum SpecialIdentifier
        {
            None = 0,

            Chain,
            No,
            Release,
            Savepoint,
            Work
        }
    }
}