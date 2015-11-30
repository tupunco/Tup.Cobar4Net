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
using Tup.Cobar4Net.Parser.Ast.Expression;
using Tup.Cobar4Net.Parser.Ast.Expression.Primary;
using Tup.Cobar4Net.Parser.Ast.Expression.Primary.Literal;
using Tup.Cobar4Net.Parser.Ast.Fragment.Ddl;
using Tup.Cobar4Net.Parser.Ast.Stmt.Ddl;
using Tup.Cobar4Net.Parser.Ast.Stmt.Extension;
using Tup.Cobar4Net.Parser.Recognizer.Mysql.Lexer;
using Tup.Cobar4Net.Parser.Util;
using DdlColumnDefinition = Tup.Cobar4Net.Parser.Ast.Fragment.Ddl.ColumnDefinition;
using DdlDatatype = Tup.Cobar4Net.Parser.Ast.Fragment.Ddl.DataType;
using DdlTableOptions = Tup.Cobar4Net.Parser.Ast.Fragment.Ddl.TableOptions;

namespace Tup.Cobar4Net.Parser.Recognizer.Mysql.Syntax
{
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public class MySqlDdlParser : MySqlParser
    {
        private static readonly IDictionary<string, SpecialIdentifier> specialIdentifiers =
            new Dictionary<string, SpecialIdentifier>();

        protected MySqlExprParser exprParser;

        static MySqlDdlParser()
        {
            specialIdentifiers["TRUNCATE"] = SpecialIdentifier.Truncate;
            specialIdentifiers["TEMPORARY"] = SpecialIdentifier.Temporary;
            specialIdentifiers["DEFINER"] = SpecialIdentifier.Definer;
            specialIdentifiers["KEY_BLOCK_SIZE"] = SpecialIdentifier.KeyBlockSize;
            specialIdentifiers["COMMENT"] = SpecialIdentifier.Comment;
            specialIdentifiers["DYNAMIC"] = SpecialIdentifier.Dynamic;
            specialIdentifiers["FIXED"] = SpecialIdentifier.Fixed;
            specialIdentifiers["BIT"] = SpecialIdentifier.Bit;
            specialIdentifiers["DATE"] = SpecialIdentifier.Date;
            specialIdentifiers["TIME"] = SpecialIdentifier.Time;
            specialIdentifiers["TIMESTAMP"] = SpecialIdentifier.Timestamp;
            specialIdentifiers["DATETIME"] = SpecialIdentifier.Datetime;
            specialIdentifiers["YEAR"] = SpecialIdentifier.Year;
            specialIdentifiers["TEXT"] = SpecialIdentifier.Text;
            specialIdentifiers["ENUM"] = SpecialIdentifier.Enum;
            specialIdentifiers["ENGINE"] = SpecialIdentifier.Engine;
            specialIdentifiers["AUTO_INCREMENT"] = SpecialIdentifier.AutoIncrement;
            specialIdentifiers["AVG_ROW_LENGTH"] = SpecialIdentifier.AvgRowLength;
            specialIdentifiers["CHECKSUM"] = SpecialIdentifier.Checksum;
            specialIdentifiers["CONNECTION"] = SpecialIdentifier.Connection;
            specialIdentifiers["DATA"] = SpecialIdentifier.Data;
            specialIdentifiers["DELAY_KEY_WRITE"] = SpecialIdentifier.DelayKeyWrite;
            specialIdentifiers["INSERT_METHOD"] = SpecialIdentifier.InsertMethod;
            specialIdentifiers["MAX_ROWS"] = SpecialIdentifier.MaxRows;
            specialIdentifiers["MIN_ROWS"] = SpecialIdentifier.MinRows;
            specialIdentifiers["PACK_KEYS"] = SpecialIdentifier.PackKeys;
            specialIdentifiers["PASSWORD"] = SpecialIdentifier.Password;
            specialIdentifiers["ROW_FORMAT"] = SpecialIdentifier.RowFormat;
            specialIdentifiers["COMPRESSED"] = SpecialIdentifier.Compressed;
            specialIdentifiers["REDUNDANT"] = SpecialIdentifier.Redundant;
            specialIdentifiers["COMPACT"] = SpecialIdentifier.Compact;
            specialIdentifiers["MODIFY"] = SpecialIdentifier.Modify;
            specialIdentifiers["DISABLE"] = SpecialIdentifier.Disable;
            specialIdentifiers["ENABLE"] = SpecialIdentifier.Enable;
            specialIdentifiers["DISCARD"] = SpecialIdentifier.Discard;
            specialIdentifiers["IMPORT"] = SpecialIdentifier.Import;
            specialIdentifiers["CHARSET"] = SpecialIdentifier.Charset;
            specialIdentifiers["POLICY"] = SpecialIdentifier.Policy;
        }

        public MySqlDdlParser(MySqlLexer lexer, MySqlExprParser exprParser)
            : base(lexer)
        {
            this.exprParser = exprParser;
        }

        /// <exception cref="System.SqlSyntaxErrorException" />
        public virtual DdlTruncateStatement Truncate()
        {
            MatchIdentifier("TRUNCATE");
            if (lexer.Token() == MySqlToken.KwTable)
            {
                lexer.NextToken();
            }
            var tb = Identifier();
            return new DdlTruncateStatement(tb);
        }

        /// <summary>nothing has been pre-consumed</summary>
        /// <exception cref="System.SqlSyntaxErrorException" />
        public virtual IDdlStatement DdlStmt()
        {
            Identifier idTemp1;
            Identifier idTemp2;
            SpecialIdentifier siTemp;
            switch (lexer.Token())
            {
                case MySqlToken.KwAlter:
                {
                    var ignore = false;
                    if (lexer.NextToken() == MySqlToken.KwIgnore)
                    {
                        ignore = true;
                        lexer.NextToken();
                    }
                    switch (lexer.Token())
                    {
                        case MySqlToken.KwTable:
                        {
                            lexer.NextToken();
                            idTemp1 = Identifier();
                            var alterTableStatement = new DdlAlterTableStatement(ignore, idTemp1);
                            return AlterTable(alterTableStatement);
                        }

                        default:
                        {
                            throw Err("Only ALTER TABLE is supported");
                        }
                    }
                    //goto case MySqlToken.KwCreate;
                }

                case MySqlToken.KwCreate:
                {
                    switch (lexer.NextToken())
                    {
                        case MySqlToken.KwUnique:
                        case MySqlToken.KwFulltext:
                        case MySqlToken.KwSpatial:
                        {
                            lexer.NextToken();
                            goto case MySqlToken.KwIndex;
                        }

                        case MySqlToken.KwIndex:
                        {
                            lexer.NextToken();
                            idTemp1 = Identifier();
                            for (; lexer.Token() != MySqlToken.KwOn; lexer.NextToken())
                            {
                            }
                            lexer.NextToken();
                            idTemp2 = Identifier();
                            return new DdlCreateIndexStatement(idTemp1, idTemp2);
                        }

                        case MySqlToken.KwTable:
                        {
                            lexer.NextToken();
                            return CreateTable(false);
                        }

                        case MySqlToken.Identifier:
                        {
                            siTemp = specialIdentifiers.GetValue(lexer.GetStringValueUppercase());
                            if (siTemp != SpecialIdentifier.None)
                            {
                                switch (siTemp)
                                {
                                    case SpecialIdentifier.Temporary:
                                    {
                                        lexer.NextToken();
                                        Match(MySqlToken.KwTable);
                                        return CreateTable(true);
                                    }

                                    case SpecialIdentifier.Policy:
                                    {
                                        lexer.NextToken();
                                        var policyName = Identifier();
                                        Match(MySqlToken.PuncLeftParen);
                                        var policy = new ExtDdlCreatePolicy(policyName);
                                        for (var j = 0; lexer.Token() != MySqlToken.PuncRightParen; ++j)
                                        {
                                            if (j > 0)
                                            {
                                                Match(MySqlToken.PuncComma);
                                            }
                                            var id = (int) lexer.GetIntegerValue();
                                            Match(MySqlToken.LiteralNumPureDigit);
                                            var val = exprParser.Expression();
                                            policy.AddProportion(id, val);
                                        }
                                        Match(MySqlToken.PuncRightParen);
                                        return policy;
                                    }
                                }
                            }
                            goto default;
                        }

                        default:
                        {
                            throw Err("unsupported Ddl for CREATE");
                        }
                    }
                    //goto case MySqlToken.KwDrop;
                }

                case MySqlToken.KwDrop:
                {
                    switch (lexer.NextToken())
                    {
                        case MySqlToken.KwIndex:
                        {
                            lexer.NextToken();
                            idTemp1 = Identifier();
                            Match(MySqlToken.KwOn);
                            idTemp2 = Identifier();
                            return new DdlDropIndexStatement(idTemp1, idTemp2);
                        }

                        case MySqlToken.KwTable:
                        {
                            lexer.NextToken();
                            return DropTable(false);
                        }

                        case MySqlToken.Identifier:
                        {
                            siTemp = specialIdentifiers.GetValue(lexer.GetStringValueUppercase());
                            if (siTemp != SpecialIdentifier.None)
                            {
                                switch (siTemp)
                                {
                                    case SpecialIdentifier.Temporary:
                                    {
                                        lexer.NextToken();
                                        Match(MySqlToken.KwTable);
                                        return DropTable(true);
                                    }

                                    case SpecialIdentifier.Policy:
                                    {
                                        lexer.NextToken();
                                        var policyName = Identifier();
                                        return new ExtDdlDropPolicy(policyName);
                                    }
                                }
                            }
                            goto default;
                        }

                        default:
                        {
                            throw Err("unsupported Ddl for DROP");
                        }
                    }
                    //goto case MySqlToken.KwRename;
                }

                case MySqlToken.KwRename:
                {
                    lexer.NextToken();
                    Match(MySqlToken.KwTable);
                    idTemp1 = Identifier();
                    Match(MySqlToken.KwTo);
                    idTemp2 = Identifier();
                    IList<Pair<Identifier, Identifier>> list;
                    if (lexer.Token() != MySqlToken.PuncComma)
                    {
                        list = new List<Pair<Identifier, Identifier>>(1);
                        list.Add(new Pair<Identifier, Identifier>(idTemp1, idTemp2));
                        return new DdlRenameTableStatement(list);
                    }
                    list = new List<Pair<Identifier, Identifier>>();
                    list.Add(new Pair<Identifier, Identifier>(idTemp1, idTemp2));
                    for (; lexer.Token() == MySqlToken.PuncComma;)
                    {
                        lexer.NextToken();
                        idTemp1 = Identifier();
                        Match(MySqlToken.KwTo);
                        idTemp2 = Identifier();
                        list.Add(new Pair<Identifier, Identifier>(idTemp1, idTemp2));
                    }
                    return new DdlRenameTableStatement(list);
                }

                case MySqlToken.Identifier:
                {
                    var si = specialIdentifiers.GetValue(lexer.GetStringValueUppercase());
                    if (si != SpecialIdentifier.None)
                    {
                        switch (si)
                        {
                            case SpecialIdentifier.Truncate:
                            {
                                return Truncate();
                            }
                        }
                    }
                    goto default;
                }

                default:
                {
                    throw Err("unsupported Ddl");
                }
            }
        }

        /// <summary><code>TABLE</code> has been consumed</summary>
        /// <exception cref="System.SqlSyntaxErrorException" />
        private DdlDropTableStatement DropTable(bool temp)
        {
            var ifExists = false;
            if (lexer.Token() == MySqlToken.KwIf)
            {
                lexer.NextToken();
                Match(MySqlToken.KwExists);
                ifExists = true;
            }
            var tb = Identifier();
            IList<Identifier> list;
            if (lexer.Token() != MySqlToken.PuncComma)
            {
                list = new List<Identifier>(1);
                list.Add(tb);
            }
            else
            {
                list = new List<Identifier>();
                list.Add(tb);
                for (; lexer.Token() == MySqlToken.PuncComma;)
                {
                    lexer.NextToken();
                    tb = Identifier();
                    list.Add(tb);
                }
            }
            var mode = DropTableMode.Undef;
            switch (lexer.Token())
            {
                case MySqlToken.KwRestrict:
                {
                    lexer.NextToken();
                    mode = DropTableMode.Restrict;
                    break;
                }

                case MySqlToken.KwCascade:
                {
                    lexer.NextToken();
                    mode = DropTableMode.Cascade;
                    break;
                }
            }
            return new DdlDropTableStatement(list, temp, ifExists, mode);
        }

        /// <summary>token of table name has been consumed</summary>
        /// <exception cref="System.SqlSyntaxErrorException" />
        private DdlAlterTableStatement AlterTable(DdlAlterTableStatement stmt)
        {
            var options = new DdlTableOptions();
            stmt.TableOptions = options;
            Identifier id = null;
            Identifier id2 = null;
            Identifier id3 = null;
            DdlColumnDefinition colDef = null;
            IndexDefinition indexDef = null;
            IExpression expr = null;
            for (var i = 0; lexer.Token() != MySqlToken.Eof; ++i)
            {
                if (i > 0)
                {
                    Match(MySqlToken.PuncComma);
                }
                if (TableOptions(options))
                {
                    continue;
                }
                switch (lexer.Token())
                {
                    case MySqlToken.KwConvert:
                    {
                        // | CONVERT TO CHARACTER SET charset_name [COLLATE
                        // collation_name]
                        lexer.NextToken();
                        Match(MySqlToken.KwTo);
                        Match(MySqlToken.KwCharacter);
                        Match(MySqlToken.KwSet);
                        id = Identifier();
                        id2 = null;
                        if (lexer.Token() == MySqlToken.KwCollate)
                        {
                            lexer.NextToken();
                            id2 = Identifier();
                        }
                        stmt.ConvertCharset = new Pair<Identifier, Identifier>(id, id2);
                        goto main_switch_break;
                    }

                    case MySqlToken.KwRename:
                    {
                        // | RENAME [TO] new_tbl_name
                        if (lexer.NextToken() == MySqlToken.KwTo)
                        {
                            lexer.NextToken();
                        }
                        id = Identifier();
                        stmt.RenameTo = id;
                        goto main_switch_break;
                    }

                    case MySqlToken.KwDrop:
                    {
                        switch (lexer.NextToken())
                        {
                            case MySqlToken.KwIndex:
                            case MySqlToken.KwKey:
                            {
                                // | DROP {INDEX|KEY} index_name
                                lexer.NextToken();
                                id = Identifier();
                                stmt.AddAlterSpecification(new DdlAlterTableStatement.DropIndex(id));
                                goto drop_switch_break;
                            }

                            case MySqlToken.KwPrimary:
                            {
                                // | DROP PRIMARY KEY
                                lexer.NextToken();
                                Match(MySqlToken.KwKey);
                                stmt.AddAlterSpecification(new DdlAlterTableStatement.DropPrimaryKey());
                                goto drop_switch_break;
                            }

                            case MySqlToken.Identifier:
                            {
                                // | DROP [COLUMN] col_name
                                id = Identifier();
                                stmt.AddAlterSpecification(new DdlAlterTableStatement.DropColumn(id));
                                goto drop_switch_break;
                            }

                            case MySqlToken.KwColumn:
                            {
                                // | DROP [COLUMN] col_name
                                lexer.NextToken();
                                id = Identifier();
                                stmt.AddAlterSpecification(new DdlAlterTableStatement.DropColumn(id));
                                goto drop_switch_break;
                            }

                            default:
                            {
                                throw new SqlSyntaxErrorException("ALTER TABLE error for DROP");
                            }
                        }
                        drop_switch_break:
                        ;
                        goto main_switch_break;
                    }

                    case MySqlToken.KwChange:
                    {
                        // | CHANGE [COLUMN] old_col_name new_col_name column_definition
                        // [FIRST|AFTER col_name]
                        if (lexer.NextToken() == MySqlToken.KwColumn)
                        {
                            lexer.NextToken();
                        }
                        id = Identifier();
                        id2 = Identifier();
                        colDef = ColumnDefinition();
                        if (lexer.Token() == MySqlToken.Identifier)
                        {
                            if ("FIRST".Equals(lexer.GetStringValueUppercase()))
                            {
                                lexer.NextToken();
                                stmt.AddAlterSpecification(new DdlAlterTableStatement.ChangeColumn(id, id2, colDef
                                    , null));
                            }
                            else
                            {
                                if ("AFTER".Equals(lexer.GetStringValueUppercase()))
                                {
                                    lexer.NextToken();
                                    id3 = Identifier();
                                    stmt.AddAlterSpecification(new DdlAlterTableStatement.ChangeColumn(id, id2, colDef
                                        , id3));
                                }
                                else
                                {
                                    stmt.AddAlterSpecification(new DdlAlterTableStatement.ChangeColumn(id, id2, colDef
                                        ));
                                }
                            }
                        }
                        else
                        {
                            stmt.AddAlterSpecification(new DdlAlterTableStatement.ChangeColumn(id, id2, colDef
                                ));
                        }
                        goto main_switch_break;
                    }

                    case MySqlToken.KwAlter:
                    {
                        // | ALTER [COLUMN] col_name {SET DEFAULT literal | DROP
                        // DEFAULT}
                        if (lexer.NextToken() == MySqlToken.KwColumn)
                        {
                            lexer.NextToken();
                        }
                        id = Identifier();
                        switch (lexer.Token())
                        {
                            case MySqlToken.KwSet:
                            {
                                lexer.NextToken();
                                Match(MySqlToken.KwDefault);
                                expr = exprParser.Expression();
                                stmt.AddAlterSpecification(new DdlAlterTableStatement.AlterColumnDefaultVal(id, expr
                                    ));
                                break;
                            }

                            case MySqlToken.KwDrop:
                            {
                                lexer.NextToken();
                                Match(MySqlToken.KwDefault);
                                stmt.AddAlterSpecification(new DdlAlterTableStatement.AlterColumnDefaultVal(id));
                                break;
                            }

                            default:
                            {
                                throw new SqlSyntaxErrorException("ALTER TABLE error for ALTER");
                            }
                        }
                        goto main_switch_break;
                    }

                    case MySqlToken.KwAdd:
                    {
                        switch (lexer.NextToken())
                        {
                            case MySqlToken.Identifier:
                            {
                                // | ADD [COLUMN] col_name column_definition [FIRST | AFTER
                                // col_name ]
                                id = Identifier();
                                colDef = ColumnDefinition();
                                if (lexer.Token() == MySqlToken.Identifier)
                                {
                                    if ("FIRST".Equals(lexer.GetStringValueUppercase()))
                                    {
                                        lexer.NextToken();
                                        stmt.AddAlterSpecification(new DdlAlterTableStatement.AddColumn(id, colDef, null));
                                    }
                                    else
                                    {
                                        if ("AFTER".Equals(lexer.GetStringValueUppercase()))
                                        {
                                            lexer.NextToken();
                                            id2 = Identifier();
                                            stmt.AddAlterSpecification(new DdlAlterTableStatement.AddColumn(id, colDef,
                                                id2));
                                        }
                                        else
                                        {
                                            stmt.AddAlterSpecification(new DdlAlterTableStatement.AddColumn(id, colDef));
                                        }
                                    }
                                }
                                else
                                {
                                    stmt.AddAlterSpecification(new DdlAlterTableStatement.AddColumn(id, colDef));
                                }
                                goto add_switch_break;
                            }

                            case MySqlToken.PuncLeftParen:
                            {
                                // | ADD [COLUMN] (col_name column_definition,...)
                                lexer.NextToken();
                                for (var j = 0; lexer.Token() != MySqlToken.PuncRightParen; ++j)
                                {
                                    var addColumns = new DdlAlterTableStatement.AddColumns();
                                    stmt.AddAlterSpecification(addColumns);
                                    if (j > 0)
                                    {
                                        Match(MySqlToken.PuncComma);
                                    }
                                    id = Identifier();
                                    colDef = ColumnDefinition();
                                    addColumns.AddColumn(id, colDef);
                                }
                                Match(MySqlToken.PuncRightParen);
                                goto add_switch_break;
                            }

                            case MySqlToken.KwColumn:
                            {
                                if (lexer.NextToken() == MySqlToken.PuncLeftParen)
                                {
                                    // | ADD [COLUMN] (col_name column_definition,...)
                                    lexer.NextToken();
                                    for (var j_1 = 0; lexer.Token() != MySqlToken.PuncRightParen; ++j_1)
                                    {
                                        var addColumns = new DdlAlterTableStatement.AddColumns();
                                        stmt.AddAlterSpecification(addColumns);
                                        if (j_1 > 0)
                                        {
                                            Match(MySqlToken.PuncComma);
                                        }
                                        id = Identifier();
                                        colDef = ColumnDefinition();
                                        addColumns.AddColumn(id, colDef);
                                    }
                                    Match(MySqlToken.PuncRightParen);
                                }
                                else
                                {
                                    // | ADD [COLUMN] col_name column_definition [FIRST |
                                    // AFTER col_name ]
                                    id = Identifier();
                                    colDef = ColumnDefinition();
                                    if (lexer.Token() == MySqlToken.Identifier)
                                    {
                                        if ("FIRST".Equals(lexer.GetStringValueUppercase()))
                                        {
                                            lexer.NextToken();
                                            stmt.AddAlterSpecification(new DdlAlterTableStatement.AddColumn(id, colDef,
                                                null));
                                        }
                                        else
                                        {
                                            if ("AFTER".Equals(lexer.GetStringValueUppercase()))
                                            {
                                                lexer.NextToken();
                                                id2 = Identifier();
                                                stmt.AddAlterSpecification(new DdlAlterTableStatement.AddColumn(id,
                                                    colDef, id2));
                                            }
                                            else
                                            {
                                                stmt.AddAlterSpecification(new DdlAlterTableStatement.AddColumn(id,
                                                    colDef));
                                            }
                                        }
                                    }
                                    else
                                    {
                                        stmt.AddAlterSpecification(new DdlAlterTableStatement.AddColumn(id, colDef));
                                    }
                                }
                                goto add_switch_break;
                            }

                            case MySqlToken.KwIndex:
                            case MySqlToken.KwKey:
                            {
                                // | ADD {INDEX|KEY} [index_name] [index_type]
                                // (index_col_name,...) [index_option] ...
                                id = null;
                                if (lexer.NextToken() == MySqlToken.Identifier)
                                {
                                    id = Identifier();
                                }
                                indexDef = IndexDefinition();
                                stmt.AddAlterSpecification(new DdlAlterTableStatement.AddIndex(id, indexDef));
                                goto add_switch_break;
                            }

                            case MySqlToken.KwPrimary:
                            {
                                // | ADD PRIMARY KEY [index_type] (index_col_name,...)
                                // [index_option] ...
                                lexer.NextToken();
                                Match(MySqlToken.KwKey);
                                indexDef = IndexDefinition();
                                stmt.AddAlterSpecification(new DdlAlterTableStatement.AddPrimaryKey(indexDef));
                                goto add_switch_break;
                            }

                            case MySqlToken.KwUnique:
                            {
                                switch (lexer.NextToken())
                                {
                                    case MySqlToken.KwIndex:
                                    case MySqlToken.KwKey:
                                    {
                                        // | ADD UNIQUE [INDEX|KEY] [index_name] [index_type]
                                        // (index_col_name,...) [index_option] ...
                                        lexer.NextToken();
                                        break;
                                    }
                                }
                                id = null;
                                if (lexer.Token() == MySqlToken.Identifier)
                                {
                                    id = Identifier();
                                }
                                indexDef = IndexDefinition();
                                stmt.AddAlterSpecification(new DdlAlterTableStatement.AddUniqueKey(id, indexDef));
                                goto add_switch_break;
                            }

                            case MySqlToken.KwFulltext:
                            {
                                switch (lexer.NextToken())
                                {
                                    case MySqlToken.KwIndex:
                                    case MySqlToken.KwKey:
                                    {
                                        // | ADD FULLTEXT [INDEX|KEY] [index_name]
                                        // (index_col_name,...) [index_option] ...
                                        lexer.NextToken();
                                        break;
                                    }
                                }
                                id = null;
                                if (lexer.Token() == MySqlToken.Identifier)
                                {
                                    id = Identifier();
                                }
                                indexDef = IndexDefinition();
                                stmt.AddAlterSpecification(new DdlAlterTableStatement.AddFullTextIndex(id, indexDef
                                    ));
                                goto add_switch_break;
                            }

                            case MySqlToken.KwSpatial:
                            {
                                switch (lexer.NextToken())
                                {
                                    case MySqlToken.KwIndex:
                                    case MySqlToken.KwKey:
                                    {
                                        // | ADD SPATIAL [INDEX|KEY] [index_name]
                                        // (index_col_name,...) [index_option] ...
                                        lexer.NextToken();
                                        break;
                                    }
                                }
                                id = null;
                                if (lexer.Token() == MySqlToken.Identifier)
                                {
                                    id = Identifier();
                                }
                                indexDef = IndexDefinition();
                                stmt.AddAlterSpecification(new DdlAlterTableStatement.AddSpatialIndex(id, indexDef
                                    ));
                                goto add_switch_break;
                            }

                            default:
                            {
                                throw new SqlSyntaxErrorException("ALTER TABLE error for ADD");
                            }
                        }
                        add_switch_break:
                        ;
                        goto main_switch_break;
                    }

                    case MySqlToken.Identifier:
                    {
                        var si = specialIdentifiers.GetValue(lexer.GetStringValueUppercase());
                        if (si != SpecialIdentifier.None)
                        {
                            switch (si)
                            {
                                case SpecialIdentifier.Import:
                                {
                                    // | IMPORT TABLESPACE
                                    lexer.NextToken();
                                    MatchIdentifier("TABLESPACE");
                                    stmt.IsImportTableSpace = true;
                                    goto main_switch_break;
                                }

                                case SpecialIdentifier.Discard:
                                {
                                    // | DISCARD TABLESPACE
                                    lexer.NextToken();
                                    MatchIdentifier("TABLESPACE");
                                    stmt.DiscardTableSpace = true;
                                    goto main_switch_break;
                                }

                                case SpecialIdentifier.Enable:
                                {
                                    // | ENABLE KEYS
                                    lexer.NextToken();
                                    Match(MySqlToken.KwKeys);
                                    stmt.EnableKeys = true;
                                    goto main_switch_break;
                                }

                                case SpecialIdentifier.Disable:
                                {
                                    // | DISABLE KEYS
                                    lexer.NextToken();
                                    Match(MySqlToken.KwKeys);
                                    stmt.DisableKeys = true;
                                    goto main_switch_break;
                                }

                                case SpecialIdentifier.Modify:
                                {
                                    // | MODIFY [COLUMN] col_name column_definition [FIRST |
                                    // AFTER col_name]
                                    if (lexer.NextToken() == MySqlToken.KwColumn)
                                    {
                                        lexer.NextToken();
                                    }
                                    id = Identifier();
                                    colDef = ColumnDefinition();
                                    if (lexer.Token() == MySqlToken.Identifier)
                                    {
                                        if ("FIRST".Equals(lexer.GetStringValueUppercase()))
                                        {
                                            lexer.NextToken();
                                            stmt.AddAlterSpecification(new DdlAlterTableStatement.ModifyColumn(id,
                                                colDef, null
                                                ));
                                        }
                                        else
                                        {
                                            if ("AFTER".Equals(lexer.GetStringValueUppercase()))
                                            {
                                                lexer.NextToken();
                                                id2 = Identifier();
                                                stmt.AddAlterSpecification(new DdlAlterTableStatement.ModifyColumn(id,
                                                    colDef, id2
                                                    ));
                                            }
                                            else
                                            {
                                                stmt.AddAlterSpecification(new DdlAlterTableStatement.ModifyColumn(id,
                                                    colDef));
                                            }
                                        }
                                    }
                                    else
                                    {
                                        stmt.AddAlterSpecification(new DdlAlterTableStatement.ModifyColumn(id, colDef));
                                    }
                                    goto main_switch_break;
                                }
                            }
                        }
                        goto default;
                    }

                    default:
                    {
                        throw new SqlSyntaxErrorException("unknown ALTER specification");
                    }
                }
                main_switch_break:
                ;
            }
            return stmt;
        }

        /// <summary><code>TABLE</code> has been consumed</summary>
        /// <exception cref="System.SqlSyntaxErrorException" />
        private DdlCreateTableStatement CreateTable(bool temp)
        {
            var ifNotExists = false;
            if (lexer.Token() == MySqlToken.KwIf)
            {
                lexer.NextToken();
                Match(MySqlToken.KwNot);
                Match(MySqlToken.KwExists);
                ifNotExists = true;
            }
            var table = Identifier();
            var stmt = new DdlCreateTableStatement(temp, ifNotExists, table);
            CreateTableDefs(stmt);
            var options = new DdlTableOptions();
            stmt.SetTableOptions(options);
            TableOptions(options);
            var selectOpt = CreateTableSelectOption.None;
            switch (lexer.Token())
            {
                case MySqlToken.KwIgnore:
                {
                    selectOpt = CreateTableSelectOption.Ignored;
                    if (lexer.NextToken() == MySqlToken.KwAs)
                    {
                        lexer.NextToken();
                    }
                    break;
                }

                case MySqlToken.KwReplace:
                {
                    selectOpt = CreateTableSelectOption.Replace;
                    if (lexer.NextToken() == MySqlToken.KwAs)
                    {
                        lexer.NextToken();
                    }
                    break;
                }

                case MySqlToken.KwAs:
                {
                    lexer.NextToken();
                    goto case MySqlToken.KwSelect;
                }

                case MySqlToken.KwSelect:
                {
                    break;
                }

                case MySqlToken.Eof:
                {
                    return stmt;
                }

                default:
                {
                    throw new SqlSyntaxErrorException("Ddl CREATE TABLE statement not end properly");
                }
            }
            var select = new MySqlDmlSelectParser(lexer, exprParser).Select();
            stmt.SetSelect(selectOpt, select);
            Match(MySqlToken.Eof);
            return stmt;
        }

        /// <exception cref="System.SqlSyntaxErrorException" />
        private void CreateTableDefs(DdlCreateTableStatement stmt)
        {
            if (lexer.Token() != MySqlToken.PuncLeftParen)
            {
                return;
            }
            Match(MySqlToken.PuncLeftParen);
            IndexDefinition indexDef;
            Identifier id;
            for (var i = 0; lexer.Token() != MySqlToken.PuncRightParen; ++i)
            {
                if (i > 0)
                {
                    Match(MySqlToken.PuncComma);
                }
                switch (lexer.Token())
                {
                    case MySqlToken.KwPrimary:
                    {
                        lexer.NextToken();
                        Match(MySqlToken.KwKey);
                        indexDef = IndexDefinition();
                        stmt.SetPrimaryKey(indexDef);
                        break;
                    }

                    case MySqlToken.KwIndex:
                    case MySqlToken.KwKey:
                    {
                        lexer.NextToken();
                        if (lexer.Token() == MySqlToken.Identifier)
                        {
                            id = Identifier();
                        }
                        else
                        {
                            id = null;
                        }
                        indexDef = IndexDefinition();
                        stmt.AddIndex(id, indexDef);
                        break;
                    }

                    case MySqlToken.KwUnique:
                    {
                        switch (lexer.NextToken())
                        {
                            case MySqlToken.KwIndex:
                            case MySqlToken.KwKey:
                            {
                                lexer.NextToken();
                                break;
                            }
                        }
                        if (lexer.Token() == MySqlToken.Identifier)
                        {
                            id = Identifier();
                        }
                        else
                        {
                            id = null;
                        }
                        indexDef = IndexDefinition();
                        stmt.AddUniqueIndex(id, indexDef);
                        break;
                    }

                    case MySqlToken.KwFulltext:
                    {
                        switch (lexer.NextToken())
                        {
                            case MySqlToken.KwIndex:
                            case MySqlToken.KwKey:
                            {
                                lexer.NextToken();
                                break;
                            }
                        }
                        if (lexer.Token() == MySqlToken.Identifier)
                        {
                            id = Identifier();
                        }
                        else
                        {
                            id = null;
                        }
                        indexDef = IndexDefinition();
                        if (indexDef.IndexType != IndexType.None)
                        {
                            throw new SqlSyntaxErrorException("FULLTEXT INDEX can specify no index_type");
                        }
                        stmt.AddFullTextIndex(id, indexDef);
                        break;
                    }

                    case MySqlToken.KwSpatial:
                    {
                        switch (lexer.NextToken())
                        {
                            case MySqlToken.KwIndex:
                            case MySqlToken.KwKey:
                            {
                                lexer.NextToken();
                                break;
                            }
                        }
                        if (lexer.Token() == MySqlToken.Identifier)
                        {
                            id = Identifier();
                        }
                        else
                        {
                            id = null;
                        }
                        indexDef = IndexDefinition();
                        if (indexDef.IndexType != IndexType.None)
                        {
                            throw new SqlSyntaxErrorException("SPATIAL INDEX can specify no index_type");
                        }
                        stmt.AddSpatialIndex(id, indexDef);
                        break;
                    }

                    case MySqlToken.KwCheck:
                    {
                        lexer.NextToken();
                        Match(MySqlToken.PuncLeftParen);
                        var expr = exprParser.Expression();
                        Match(MySqlToken.PuncRightParen);
                        stmt.AddCheck(expr);
                        break;
                    }

                    case MySqlToken.Identifier:
                    {
                        var columnName = Identifier();
                        var columnDef = ColumnDefinition();
                        stmt.AddColumnDefinition(columnName, columnDef);
                        break;
                    }

                    default:
                    {
                        throw new SqlSyntaxErrorException("unsupportted column definition");
                    }
                }
            }
            Match(MySqlToken.PuncRightParen);
        }

        // col_name column_definition
        // | [CONSTRAINT [symbol]] PRIMARY KEY [index_type] (index_col_name,...)
        // [index_option] ...
        // | {INDEX|KEY} [index_name] [index_type] (index_col_name,...)
        // [index_option] ...
        // | [CONSTRAINT [symbol]] UNIQUE [INDEX|KEY] [index_name] [index_type]
        // (index_col_name,...) [index_option] ...
        // | {FULLTEXT|SPATIAL} [INDEX|KEY] [index_name] (index_col_name,...)
        // [index_option] ...
        // | [CONSTRAINT [symbol]] FOREIGN KEY [index_name] (index_col_name,...)
        // reference_definition
        // | CHECK (expr)
        /// <exception cref="System.SqlSyntaxErrorException" />
        private IndexDefinition IndexDefinition()
        {
            var indexType = IndexType.None;
            IList<IndexColumnName> columns = new List<IndexColumnName>(1);
            if (lexer.Token() == MySqlToken.KwUsing)
            {
                lexer.NextToken();
                var tp = MatchIdentifier("BTREE", "HASH");
                indexType = tp == 0 ? IndexType.Btree : IndexType.Hash;
            }
            Match(MySqlToken.PuncLeftParen);
            for (var i = 0; lexer.Token() != MySqlToken.PuncRightParen; ++i)
            {
                if (i > 0)
                {
                    Match(MySqlToken.PuncComma);
                }
                var indexColumnName = IndexColumnName();
                columns.Add(indexColumnName);
            }
            Match(MySqlToken.PuncRightParen);
            var options = IndexOptions();
            return new IndexDefinition(indexType, columns, options);
        }

        /// <exception cref="System.SqlSyntaxErrorException" />
        private IList<IndexOption> IndexOptions()
        {
            IList<IndexOption> list = null;
            for (;;)
            {
                switch (lexer.Token())
                {
                    case MySqlToken.KwUsing:
                    {
                        lexer.NextToken();
                        var indexType = MatchIdentifier("BTREE", "HASH") == 0 ? IndexType.Btree : IndexType.Hash;
                        if (list == null)
                        {
                            list = new List<IndexOption>(1);
                        }
                        list.Add(new IndexOption(indexType));
                        goto main_switch_break;
                    }

                    case MySqlToken.KwWith:
                    {
                        lexer.NextToken();
                        MatchIdentifier("PARSER");
                        var id = Identifier();
                        if (list == null)
                        {
                            list = new List<IndexOption>(1);
                        }
                        list.Add(new IndexOption(id));
                        goto main_switch_break;
                    }

                    case MySqlToken.Identifier:
                    {
                        var si = specialIdentifiers.GetValue(lexer.GetStringValueUppercase());
                        if (si != SpecialIdentifier.None)
                        {
                            switch (si)
                            {
                                case SpecialIdentifier.KeyBlockSize:
                                {
                                    lexer.NextToken();
                                    if (lexer.Token() == MySqlToken.OpEquals)
                                    {
                                        lexer.NextToken();
                                    }
                                    var val = exprParser.Expression();
                                    if (list == null)
                                    {
                                        list = new List<IndexOption>(1);
                                    }
                                    list.Add(new IndexOption(val));
                                    goto main_switch_break;
                                }

                                case SpecialIdentifier.Comment:
                                {
                                    lexer.NextToken();
                                    var @string = (LiteralString) exprParser.Expression();
                                    if (list == null)
                                    {
                                        list = new List<IndexOption>(1);
                                    }
                                    list.Add(new IndexOption(@string));
                                    goto main_switch_break;
                                }
                            }
                        }
                        goto default;
                    }

                    default:
                    {
                        return list;
                    }
                }
                main_switch_break:
                ;
            }
        }

        /// <exception cref="System.SqlSyntaxErrorException" />
        private IndexColumnName IndexColumnName()
        {
            // col_name [(length)] [ASC | DESC]
            var colName = Identifier();
            IExpression len = null;
            if (lexer.Token() == MySqlToken.PuncLeftParen)
            {
                lexer.NextToken();
                len = exprParser.Expression();
                Match(MySqlToken.PuncRightParen);
            }
            switch (lexer.Token())
            {
                case MySqlToken.KwAsc:
                {
                    lexer.NextToken();
                    return new IndexColumnName(colName, len,
                        true);
                }

                case MySqlToken.KwDesc:
                {
                    lexer.NextToken();
                    return new IndexColumnName(colName, len,
                        false);
                }

                default:
                {
                    return new IndexColumnName(colName, len,
                        true);
                }
            }
        }

        // data_type:
        // | DATE
        // | TIME
        // | TIMESTAMP
        // | DATETIME
        // | YEAR
        // | spatial_type
        /// <exception cref="System.SqlSyntaxErrorException" />
        private DdlDatatype DataType()
        {
            var typeName = DataTypeName.None;
            var unsigned = false;
            var zerofill = false;
            var binary = false;
            IExpression length = null;
            IExpression decimals = null;
            Identifier charSet = null;
            Identifier collation = null;
            IList<IExpression> collectionVals = null;
            switch (lexer.Token())
            {
                case MySqlToken.KwTinyint:
                {
                    // | TINYINT[(length)] [UNSIGNED] [ZEROFILL]
                    typeName = DataTypeName.Tinyint;
                    if (lexer.NextToken() == MySqlToken.PuncLeftParen)
                    {
                        lexer.NextToken();
                        length = exprParser.Expression();
                        Match(MySqlToken.PuncRightParen);
                    }
                    if (lexer.Token() == MySqlToken.KwUnsigned)
                    {
                        unsigned = true;
                        lexer.NextToken();
                    }
                    if (lexer.Token() == MySqlToken.KwZerofill)
                    {
                        zerofill = true;
                        lexer.NextToken();
                    }
                    goto typeName_break;
                }

                case MySqlToken.KwSmallint:
                {
                    // | SMALLINT[(length)] [UNSIGNED] [ZEROFILL]
                    typeName = DataTypeName.Smallint;
                    if (lexer.NextToken() == MySqlToken.PuncLeftParen)
                    {
                        lexer.NextToken();
                        length = exprParser.Expression();
                        Match(MySqlToken.PuncRightParen);
                    }
                    if (lexer.Token() == MySqlToken.KwUnsigned)
                    {
                        unsigned = true;
                        lexer.NextToken();
                    }
                    if (lexer.Token() == MySqlToken.KwZerofill)
                    {
                        zerofill = true;
                        lexer.NextToken();
                    }
                    goto typeName_break;
                }

                case MySqlToken.KwMediumint:
                {
                    // | MEDIUMINT[(length)] [UNSIGNED] [ZEROFILL]
                    typeName = DataTypeName.Mediumint;
                    if (lexer.NextToken() == MySqlToken.PuncLeftParen)
                    {
                        lexer.NextToken();
                        length = exprParser.Expression();
                        Match(MySqlToken.PuncRightParen);
                    }
                    if (lexer.Token() == MySqlToken.KwUnsigned)
                    {
                        unsigned = true;
                        lexer.NextToken();
                    }
                    if (lexer.Token() == MySqlToken.KwZerofill)
                    {
                        zerofill = true;
                        lexer.NextToken();
                    }
                    goto typeName_break;
                }

                case MySqlToken.KwInteger:
                case MySqlToken.KwInt:
                {
                    // | INT[(length)] [UNSIGNED] [ZEROFILL]
                    // | INTEGER[(length)] [UNSIGNED] [ZEROFILL]
                    typeName = DataTypeName.Int;
                    if (lexer.NextToken() == MySqlToken.PuncLeftParen)
                    {
                        lexer.NextToken();
                        length = exprParser.Expression();
                        Match(MySqlToken.PuncRightParen);
                    }
                    if (lexer.Token() == MySqlToken.KwUnsigned)
                    {
                        unsigned = true;
                        lexer.NextToken();
                    }
                    if (lexer.Token() == MySqlToken.KwZerofill)
                    {
                        zerofill = true;
                        lexer.NextToken();
                    }
                    goto typeName_break;
                }

                case MySqlToken.KwBigint:
                {
                    // | BIGINT[(length)] [UNSIGNED] [ZEROFILL]
                    typeName = DataTypeName.Bigint;
                    if (lexer.NextToken() == MySqlToken.PuncLeftParen)
                    {
                        lexer.NextToken();
                        length = exprParser.Expression();
                        Match(MySqlToken.PuncRightParen);
                    }
                    if (lexer.Token() == MySqlToken.KwUnsigned)
                    {
                        unsigned = true;
                        lexer.NextToken();
                    }
                    if (lexer.Token() == MySqlToken.KwZerofill)
                    {
                        zerofill = true;
                        lexer.NextToken();
                    }
                    goto typeName_break;
                }

                case MySqlToken.KwReal:
                {
                    // | REAL[(length,decimals)] [UNSIGNED] [ZEROFILL]
                    typeName = DataTypeName.Real;
                    if (lexer.NextToken() == MySqlToken.PuncLeftParen)
                    {
                        lexer.NextToken();
                        length = exprParser.Expression();
                        Match(MySqlToken.PuncComma);
                        decimals = exprParser.Expression();
                        Match(MySqlToken.PuncRightParen);
                    }
                    if (lexer.Token() == MySqlToken.KwUnsigned)
                    {
                        unsigned = true;
                        lexer.NextToken();
                    }
                    if (lexer.Token() == MySqlToken.KwZerofill)
                    {
                        zerofill = true;
                        lexer.NextToken();
                    }
                    goto typeName_break;
                }

                case MySqlToken.KwDouble:
                {
                    // | DOUBLE[(length,decimals)] [UNSIGNED] [ZEROFILL]
                    typeName = DataTypeName.Double;
                    if (lexer.NextToken() == MySqlToken.PuncLeftParen)
                    {
                        lexer.NextToken();
                        length = exprParser.Expression();
                        Match(MySqlToken.PuncComma);
                        decimals = exprParser.Expression();
                        Match(MySqlToken.PuncRightParen);
                    }
                    if (lexer.Token() == MySqlToken.KwUnsigned)
                    {
                        unsigned = true;
                        lexer.NextToken();
                    }
                    if (lexer.Token() == MySqlToken.KwZerofill)
                    {
                        zerofill = true;
                        lexer.NextToken();
                    }
                    goto typeName_break;
                }

                case MySqlToken.KwFloat:
                {
                    // | FLOAT[(length,decimals)] [UNSIGNED] [ZEROFILL]
                    typeName = DataTypeName.Float;
                    if (lexer.NextToken() == MySqlToken.PuncLeftParen)
                    {
                        lexer.NextToken();
                        length = exprParser.Expression();
                        Match(MySqlToken.PuncComma);
                        decimals = exprParser.Expression();
                        Match(MySqlToken.PuncRightParen);
                    }
                    if (lexer.Token() == MySqlToken.KwUnsigned)
                    {
                        unsigned = true;
                        lexer.NextToken();
                    }
                    if (lexer.Token() == MySqlToken.KwZerofill)
                    {
                        zerofill = true;
                        lexer.NextToken();
                    }
                    goto typeName_break;
                }

                case MySqlToken.KwNumeric:
                case MySqlToken.KwDecimal:
                case MySqlToken.KwDec:
                {
                    // | DECIMAL[(length[,decimals])] [UNSIGNED] [ZEROFILL]
                    // | NUMERIC[(length[,decimals])] [UNSIGNED] [ZEROFILL]
                    typeName = DataTypeName.Decimal;
                    if (lexer.NextToken() == MySqlToken.PuncLeftParen)
                    {
                        lexer.NextToken();
                        length = exprParser.Expression();
                        if (lexer.Token() == MySqlToken.PuncComma)
                        {
                            Match(MySqlToken.PuncComma);
                            decimals = exprParser.Expression();
                        }
                        Match(MySqlToken.PuncRightParen);
                    }
                    if (lexer.Token() == MySqlToken.KwUnsigned)
                    {
                        unsigned = true;
                        lexer.NextToken();
                    }
                    if (lexer.Token() == MySqlToken.KwZerofill)
                    {
                        zerofill = true;
                        lexer.NextToken();
                    }
                    goto typeName_break;
                }

                case MySqlToken.KwChar:
                {
                    // | CHAR[(length)] [CHARACTER SET charset_name] [COLLATE
                    // collation_name]
                    typeName = DataTypeName.Char;
                    if (lexer.NextToken() == MySqlToken.PuncLeftParen)
                    {
                        lexer.NextToken();
                        length = exprParser.Expression();
                        Match(MySqlToken.PuncRightParen);
                    }
                    if (lexer.Token() == MySqlToken.KwCharacter)
                    {
                        lexer.NextToken();
                        Match(MySqlToken.KwSet);
                        charSet = Identifier();
                    }
                    if (lexer.Token() == MySqlToken.KwCollate)
                    {
                        lexer.NextToken();
                        collation = Identifier();
                    }
                    goto typeName_break;
                }

                case MySqlToken.KwVarchar:
                {
                    // | VARCHAR(length) [CHARACTER SET charset_name] [COLLATE
                    // collation_name]
                    typeName = DataTypeName.Varchar;
                    lexer.NextToken();
                    Match(MySqlToken.PuncLeftParen);
                    length = exprParser.Expression();
                    Match(MySqlToken.PuncRightParen);
                    if (lexer.Token() == MySqlToken.KwCharacter)
                    {
                        lexer.NextToken();
                        Match(MySqlToken.KwSet);
                        charSet = Identifier();
                    }
                    if (lexer.Token() == MySqlToken.KwCollate)
                    {
                        lexer.NextToken();
                        collation = Identifier();
                    }
                    goto typeName_break;
                }

                case MySqlToken.KwBinary:
                {
                    // | BINARY[(length)]
                    typeName = DataTypeName.Binary;
                    if (lexer.NextToken() == MySqlToken.PuncLeftParen)
                    {
                        lexer.NextToken();
                        length = exprParser.Expression();
                        Match(MySqlToken.PuncRightParen);
                    }
                    goto typeName_break;
                }

                case MySqlToken.KwVarbinary:
                {
                    // | VARBINARY(length)
                    typeName = DataTypeName.Varbinary;
                    lexer.NextToken();
                    Match(MySqlToken.PuncLeftParen);
                    length = exprParser.Expression();
                    Match(MySqlToken.PuncRightParen);
                    goto typeName_break;
                }

                case MySqlToken.KwTinyblob:
                {
                    typeName = DataTypeName.Tinyblob;
                    lexer.NextToken();
                    goto typeName_break;
                }

                case MySqlToken.KwBlob:
                {
                    typeName = DataTypeName.Blob;
                    lexer.NextToken();
                    goto typeName_break;
                }

                case MySqlToken.KwMediumblob:
                {
                    typeName = DataTypeName.Mediumblob;
                    lexer.NextToken();
                    goto typeName_break;
                }

                case MySqlToken.KwLongblob:
                {
                    typeName = DataTypeName.Longblob;
                    lexer.NextToken();
                    goto typeName_break;
                }

                case MySqlToken.KwTinytext:
                {
                    // | TINYTEXT [BINARY] [CHARACTER SET charset_name] [COLLATE
                    // collation_name]
                    typeName = DataTypeName.Tinytext;
                    if (lexer.NextToken() == MySqlToken.KwBinary)
                    {
                        lexer.NextToken();
                        binary = true;
                    }
                    if (lexer.Token() == MySqlToken.KwCharacter)
                    {
                        lexer.NextToken();
                        Match(MySqlToken.KwSet);
                        charSet = Identifier();
                    }
                    if (lexer.Token() == MySqlToken.KwCollate)
                    {
                        lexer.NextToken();
                        collation = Identifier();
                    }
                    goto typeName_break;
                }

                case MySqlToken.KwMediumtext:
                {
                    // | MEDIUMTEXT [BINARY] [CHARACTER SET charset_name] [COLLATE
                    // collation_name]
                    typeName = DataTypeName.Mediumtext;
                    if (lexer.NextToken() == MySqlToken.KwBinary)
                    {
                        lexer.NextToken();
                        binary = true;
                    }
                    if (lexer.Token() == MySqlToken.KwCharacter)
                    {
                        lexer.NextToken();
                        Match(MySqlToken.KwSet);
                        charSet = Identifier();
                    }
                    if (lexer.Token() == MySqlToken.KwCollate)
                    {
                        lexer.NextToken();
                        collation = Identifier();
                    }
                    goto typeName_break;
                }

                case MySqlToken.KwLongtext:
                {
                    // | LONGTEXT [BINARY] [CHARACTER SET charset_name] [COLLATE
                    // collation_name]
                    typeName = DataTypeName.Longtext;
                    if (lexer.NextToken() == MySqlToken.KwBinary)
                    {
                        lexer.NextToken();
                        binary = true;
                    }
                    if (lexer.Token() == MySqlToken.KwCharacter)
                    {
                        lexer.NextToken();
                        Match(MySqlToken.KwSet);
                        charSet = Identifier();
                    }
                    if (lexer.Token() == MySqlToken.KwCollate)
                    {
                        lexer.NextToken();
                        collation = Identifier();
                    }
                    goto typeName_break;
                }

                case MySqlToken.KwSet:
                {
                    // | SET(value1,value2,value3,...) [CHARACTER SET charset_name]
                    // [COLLATE collation_name]
                    typeName = DataTypeName.Set;
                    lexer.NextToken();
                    Match(MySqlToken.PuncLeftParen);
                    for (var i = 0; lexer.Token() != MySqlToken.PuncRightParen; ++i)
                    {
                        if (i > 0)
                        {
                            Match(MySqlToken.PuncComma);
                        }
                        else
                        {
                            collectionVals = new List<IExpression>(2);
                        }
                        collectionVals.Add(exprParser.Expression());
                    }
                    Match(MySqlToken.PuncRightParen);
                    if (lexer.Token() == MySqlToken.KwCharacter)
                    {
                        lexer.NextToken();
                        Match(MySqlToken.KwSet);
                        charSet = Identifier();
                    }
                    if (lexer.Token() == MySqlToken.KwCollate)
                    {
                        lexer.NextToken();
                        collation = Identifier();
                    }
                    goto typeName_break;
                }

                case MySqlToken.Identifier:
                {
                    var si = specialIdentifiers.GetValue(lexer.GetStringValueUppercase());
                    if (si != SpecialIdentifier.None)
                    {
                        switch (si)
                        {
                            case SpecialIdentifier.Bit:
                            {
                                // BIT[(length)]
                                typeName = DataTypeName.Bit;
                                if (lexer.NextToken() == MySqlToken.PuncLeftParen)
                                {
                                    lexer.NextToken();
                                    length = exprParser.Expression();
                                    Match(MySqlToken.PuncRightParen);
                                }
                                goto typeName_break;
                            }

                            case SpecialIdentifier.Date:
                            {
                                typeName = DataTypeName.Date;
                                lexer.NextToken();
                                goto typeName_break;
                            }

                            case SpecialIdentifier.Time:
                            {
                                typeName = DataTypeName.Time;
                                lexer.NextToken();
                                goto typeName_break;
                            }

                            case SpecialIdentifier.Timestamp:
                            {
                                typeName = DataTypeName.Timestamp;
                                lexer.NextToken();
                                goto typeName_break;
                            }

                            case SpecialIdentifier.Datetime:
                            {
                                typeName = DataTypeName.Datetime;
                                lexer.NextToken();
                                goto typeName_break;
                            }

                            case SpecialIdentifier.Year:
                            {
                                typeName = DataTypeName.Year;
                                lexer.NextToken();
                                goto typeName_break;
                            }

                            case SpecialIdentifier.Text:
                            {
                                // | TEXT [BINARY] [CHARACTER SET charset_name] [COLLATE
                                // collation_name]
                                typeName = DataTypeName.Text;
                                if (lexer.NextToken() == MySqlToken.KwBinary)
                                {
                                    lexer.NextToken();
                                    binary = true;
                                }
                                if (lexer.Token() == MySqlToken.KwCharacter)
                                {
                                    lexer.NextToken();
                                    Match(MySqlToken.KwSet);
                                    charSet = Identifier();
                                }
                                if (lexer.Token() == MySqlToken.KwCollate)
                                {
                                    lexer.NextToken();
                                    collation = Identifier();
                                }
                                goto typeName_break;
                            }

                            case SpecialIdentifier.Enum:
                            {
                                // | ENUM(value1,value2,value3,...) [CHARACTER SET
                                // charset_name] [COLLATE collation_name]
                                typeName = DataTypeName.Enum;
                                lexer.NextToken();
                                Match(MySqlToken.PuncLeftParen);
                                for (var i_1 = 0; lexer.Token() != MySqlToken.PuncRightParen; ++i_1)
                                {
                                    if (i_1 > 0)
                                    {
                                        Match(MySqlToken.PuncComma);
                                    }
                                    else
                                    {
                                        collectionVals = new List<IExpression>(2);
                                    }
                                    collectionVals.Add(exprParser.Expression());
                                }
                                Match(MySqlToken.PuncRightParen);
                                if (lexer.Token() == MySqlToken.KwCharacter)
                                {
                                    lexer.NextToken();
                                    Match(MySqlToken.KwSet);
                                    charSet = Identifier();
                                }
                                if (lexer.Token() == MySqlToken.KwCollate)
                                {
                                    lexer.NextToken();
                                    collation = Identifier();
                                }
                                goto typeName_break;
                            }
                        }
                    }
                    goto default;
                }

                default:
                {
                    return null;
                }
            }
            typeName_break:
            ;
            return new DdlDatatype(typeName, unsigned
                , zerofill, binary, length, decimals, charSet, collation, collectionVals);
        }

        // column_definition:
        // data_type [NOT NULL | NULL] [DEFAULT default_value]
        // [AUTO_INCREMENT] [UNIQUE [KEY] | [PRIMARY] KEY]
        // [COMMENT 'string']
        // [COLUMN_FORMAT {FIXED|DYNAMIC|DEFAULT}]
        // [reference_definition]
        /// <exception cref="System.SqlSyntaxErrorException" />
        private DdlColumnDefinition ColumnDefinition()
        {
            var dataType = DataType();
            var notNull = false;
            IExpression defaultVal = null;
            var autoIncrement = false;
            var sindex = SpecialIndex.None;
            var format = ColumnFormat.None;
            LiteralString comment = null;
            if (lexer.Token() == MySqlToken.KwNot)
            {
                lexer.NextToken();
                Match(MySqlToken.LiteralNull);
                notNull = true;
            }
            else
            {
                if (lexer.Token() == MySqlToken.LiteralNull)
                {
                    lexer.NextToken();
                }
            }
            if (lexer.Token() == MySqlToken.KwDefault)
            {
                lexer.NextToken();
                defaultVal = exprParser.Expression();
                if (!(defaultVal is Literal))
                {
                    throw new SqlSyntaxErrorException("default value of column must be a literal: " +
                                                      defaultVal);
                }
            }
            if (lexer.Token() == MySqlToken.Identifier && "AUTO_INCREMENT".Equals(lexer.GetStringValueUppercase()))
            {
                lexer.NextToken();
                autoIncrement = true;
            }
            switch (lexer.Token())
            {
                case MySqlToken.KwUnique:
                {
                    if (lexer.NextToken() == MySqlToken.KwKey)
                    {
                        lexer.NextToken();
                    }
                    sindex = SpecialIndex.Unique;
                    break;
                }

                case MySqlToken.KwPrimary:
                {
                    lexer.NextToken();
                    goto case MySqlToken.KwKey;
                }

                case MySqlToken.KwKey:
                {
                    Match(MySqlToken.KwKey);
                    sindex = SpecialIndex.Primary;
                    break;
                }
            }
            if (lexer.Token() == MySqlToken.Identifier && "COMMENT".Equals(lexer.GetStringValueUppercase()))
            {
                lexer.NextToken();
                comment = (LiteralString) exprParser.Expression();
            }
            if (lexer.Token() == MySqlToken.Identifier && "COLUMN_FORMAT".Equals(lexer.GetStringValueUppercase()))
            {
                switch (lexer.NextToken())
                {
                    case MySqlToken.KwDefault:
                    {
                        lexer.NextToken();
                        format = ColumnFormat.Default;
                        break;
                    }

                    case MySqlToken.Identifier:
                    {
                        var si = specialIdentifiers.GetValue(lexer.GetStringValueUppercase());
                        if (si != SpecialIdentifier.None)
                        {
                            switch (si)
                            {
                                case SpecialIdentifier.Fixed:
                                {
                                    lexer.NextToken();
                                    format = ColumnFormat.Fixed;
                                    break;
                                }

                                case SpecialIdentifier.Dynamic:
                                {
                                    lexer.NextToken();
                                    format = ColumnFormat.Dynamic;
                                    break;
                                }
                            }
                        }
                        break;
                    }
                }
            }
            return new DdlColumnDefinition(dataType, notNull,
                defaultVal, autoIncrement, sindex, comment, format);
        }

        /// <exception cref="System.SqlSyntaxErrorException" />
        private bool TableOptions(DdlTableOptions options)
        {
            var matched = false;
            for (var i = 0;; ++i)
            {
                var comma = false;
                if (i > 0 && lexer.Token() == MySqlToken.PuncComma)
                {
                    lexer.NextToken();
                    comma = true;
                }
                if (!TableOption(options))
                {
                    if (comma)
                    {
                        lexer.AddCacheToke(MySqlToken.PuncComma);
                    }
                    break;
                }
                matched = true;
            }
            return matched;
        }

        /// <exception cref="System.SqlSyntaxErrorException" />
        private bool TableOption(DdlTableOptions options)
        {
            Identifier id = null;
            IExpression expr = null;
            switch (lexer.Token())
            {
                case MySqlToken.KwCharacter:
                {
                    lexer.NextToken();
                    Match(MySqlToken.KwSet);
                    if (lexer.Token() == MySqlToken.OpEquals)
                    {
                        lexer.NextToken();
                    }
                    id = Identifier();
                    options.CharSet = id;
                    break;
                }

                case MySqlToken.KwCollate:
                {
                    lexer.NextToken();
                    if (lexer.Token() == MySqlToken.OpEquals)
                    {
                        lexer.NextToken();
                    }
                    id = Identifier();
                    options.Collation = id;
                    break;
                }

                case MySqlToken.KwDefault:
                {
                    switch (lexer.NextToken())
                    {
                        case MySqlToken.KwCharacter:
                        {
                            // | [DEFAULT] CHARSET [=] charset_name { MySql 5.1 legacy}
                            // | [DEFAULT] CHARACTER SET [=] charset_name
                            // | [DEFAULT] COLLATE [=] collation_name
                            lexer.NextToken();
                            Match(MySqlToken.KwSet);
                            if (lexer.Token() == MySqlToken.OpEquals)
                            {
                                lexer.NextToken();
                            }
                            id = Identifier();
                            options.CharSet = id;
                            goto os_break;
                        }

                        case MySqlToken.KwCollate:
                        {
                            lexer.NextToken();
                            if (lexer.Token() == MySqlToken.OpEquals)
                            {
                                lexer.NextToken();
                            }
                            id = Identifier();
                            options.Collation = id;
                            goto os_break;
                        }

                        case MySqlToken.Identifier:
                        {
                            var si = specialIdentifiers.GetValue(lexer.GetStringValueUppercase());
                            if (si != SpecialIdentifier.None)
                            {
                                switch (si)
                                {
                                    case SpecialIdentifier.Charset:
                                    {
                                        lexer.NextToken();
                                        if (lexer.Token() == MySqlToken.OpEquals)
                                        {
                                            lexer.NextToken();
                                        }
                                        id = Identifier();
                                        options.CharSet = id;
                                        goto os_break;
                                    }
                                }
                            }
                            goto default;
                        }

                        default:
                        {
                            lexer.AddCacheToke(MySqlToken.KwDefault);
                            return false;
                        }
                    }
                    //goto case MySqlToken.KwIndex;
                }

                case MySqlToken.KwIndex:
                {
                    // | INDEX DIRECTORY [=] 'absolute path to directory'
                    lexer.NextToken();
                    if (lexer.Token() == MySqlToken.Identifier && "DIRECTORY".Equals(lexer.GetStringValueUppercase()))
                    {
                        if (lexer.NextToken() == MySqlToken.OpEquals)
                        {
                            lexer.NextToken();
                        }
                        options.IndexDir = (LiteralString) exprParser.Expression();
                        break;
                    }
                    lexer.AddCacheToke(MySqlToken.KwIndex);
                    return true;
                }

                case MySqlToken.KwUnion:
                {
                    // | UNION [=] (tbl_name[,tbl_name]...)
                    if (lexer.NextToken() == MySqlToken.OpEquals)
                    {
                        lexer.NextToken();
                    }
                    Match(MySqlToken.PuncLeftParen);
                    IList<Identifier> union = new List<Identifier>(2);
                    for (var j = 0; lexer.Token() != MySqlToken.PuncRightParen; ++j)
                    {
                        if (j > 0)
                        {
                            Match(MySqlToken.PuncComma);
                        }
                        id = Identifier();
                        union.Add(id);
                    }
                    Match(MySqlToken.PuncRightParen);
                    options.Union = union;
                    goto os_break;
                }

                case MySqlToken.Identifier:
                {
                    var si_1 = specialIdentifiers.GetValue(lexer.GetStringValueUppercase());
                    if (si_1 != SpecialIdentifier.None)
                    {
                        switch (si_1)
                        {
                            case SpecialIdentifier.Charset:
                            {
                                // CHARSET [=] charset_name
                                lexer.NextToken();
                                if (lexer.Token() == MySqlToken.OpEquals)
                                {
                                    lexer.NextToken();
                                }
                                id = Identifier();
                                options.CharSet = id;
                                goto os_break;
                            }

                            case SpecialIdentifier.Engine:
                            {
                                // ENGINE [=] engine_name
                                if (lexer.NextToken() == MySqlToken.OpEquals)
                                {
                                    lexer.NextToken();
                                }
                                id = Identifier();
                                options.Engine = id;
                                goto os_break;
                            }

                            case SpecialIdentifier.AutoIncrement:
                            {
                                // | AUTO_INCREMENT [=] value
                                if (lexer.NextToken() == MySqlToken.OpEquals)
                                {
                                    lexer.NextToken();
                                }
                                expr = exprParser.Expression();
                                options.AutoIncrement = expr;
                                goto os_break;
                            }

                            case SpecialIdentifier.AvgRowLength:
                            {
                                // | AVG_ROW_LENGTH [=] value
                                if (lexer.NextToken() == MySqlToken.OpEquals)
                                {
                                    lexer.NextToken();
                                }
                                expr = exprParser.Expression();
                                options.AvgRowLength = expr;
                                goto os_break;
                            }

                            case SpecialIdentifier.Checksum:
                            {
                                // | CHECKSUM [=] {0 | 1}
                                if (lexer.NextToken() == MySqlToken.OpEquals)
                                {
                                    lexer.NextToken();
                                }
                                switch (lexer.Token())
                                {
                                    case MySqlToken.LiteralBoolFalse:
                                    {
                                        lexer.NextToken();
                                        options.CheckSum = false;
                                        goto case MySqlToken.LiteralBoolTrue;
                                    }

                                    case MySqlToken.LiteralBoolTrue:
                                    {
                                        lexer.NextToken();
                                        options.CheckSum = true;
                                        break;
                                    }

                                    case MySqlToken.LiteralNumPureDigit:
                                    {
                                        var intVal = (int) lexer.GetIntegerValue();
                                        lexer.NextToken();
                                        if (intVal == 0)
                                        {
                                            options.CheckSum = false;
                                        }
                                        else
                                        {
                                            options.CheckSum = true;
                                        }
                                        break;
                                    }

                                    default:
                                    {
                                        throw new SqlSyntaxErrorException("table option of CHECKSUM error");
                                    }
                                }
                                goto os_break;
                            }

                            case SpecialIdentifier.DelayKeyWrite:
                            {
                                // | DELAY_KEY_WRITE [=] {0 | 1}
                                if (lexer.NextToken() == MySqlToken.OpEquals)
                                {
                                    lexer.NextToken();
                                }
                                switch (lexer.Token())
                                {
                                    case MySqlToken.LiteralBoolFalse:
                                    {
                                        lexer.NextToken();
                                        options.DelayKeyWrite = false;
                                        goto case MySqlToken.LiteralBoolTrue;
                                    }

                                    case MySqlToken.LiteralBoolTrue:
                                    {
                                        lexer.NextToken();
                                        options.DelayKeyWrite = true;
                                        break;
                                    }

                                    case MySqlToken.LiteralNumPureDigit:
                                    {
                                        var intVal_1 = (int) lexer.GetIntegerValue();
                                        lexer.NextToken();
                                        if (intVal_1 == 0)
                                        {
                                            options.DelayKeyWrite = false;
                                        }
                                        else
                                        {
                                            options.DelayKeyWrite = true;
                                        }
                                        break;
                                    }

                                    default:
                                    {
                                        throw new SqlSyntaxErrorException("table option of DELAY_KEY_WRITE error");
                                    }
                                }
                                goto os_break;
                            }

                            case SpecialIdentifier.Comment:
                            {
                                // | COMMENT [=] 'string'
                                if (lexer.NextToken() == MySqlToken.OpEquals)
                                {
                                    lexer.NextToken();
                                }
                                options.Comment = (LiteralString) exprParser.Expression();
                                goto os_break;
                            }

                            case SpecialIdentifier.Connection:
                            {
                                // | CONNECTION [=] 'connect_string'
                                if (lexer.NextToken() == MySqlToken.OpEquals)
                                {
                                    lexer.NextToken();
                                }
                                options.Connection = (LiteralString) exprParser.Expression();
                                goto os_break;
                            }

                            case SpecialIdentifier.Data:
                            {
                                // | DATA DIRECTORY [=] 'absolute path to directory'
                                lexer.NextToken();
                                MatchIdentifier("DIRECTORY");
                                if (lexer.Token() == MySqlToken.OpEquals)
                                {
                                    lexer.NextToken();
                                }
                                options.DataDir = (LiteralString) exprParser.Expression();
                                goto os_break;
                            }

                            case SpecialIdentifier.InsertMethod:
                            {
                                // | INSERT_METHOD [=] { NO | FIRST | LAST }
                                if (lexer.NextToken() == MySqlToken.OpEquals)
                                {
                                    lexer.NextToken();
                                }
                                switch (MatchIdentifier("NO", "FIRST", "LAST"))
                                {
                                    case 0:
                                    {
                                        options.InsertMethod = InsertMethod.No;
                                        break;
                                    }

                                    case 1:
                                    {
                                        options.InsertMethod = InsertMethod.First;
                                        break;
                                    }

                                    case 2:
                                    {
                                        options.InsertMethod = InsertMethod.Last;
                                        break;
                                    }
                                }
                                goto os_break;
                            }

                            case SpecialIdentifier.KeyBlockSize:
                            {
                                // | KEY_BLOCK_SIZE [=] value
                                if (lexer.NextToken() == MySqlToken.OpEquals)
                                {
                                    lexer.NextToken();
                                }
                                options.KeyBlockSize = exprParser.Expression();
                                goto os_break;
                            }

                            case SpecialIdentifier.MaxRows:
                            {
                                // | MAX_ROWS [=] value
                                if (lexer.NextToken() == MySqlToken.OpEquals)
                                {
                                    lexer.NextToken();
                                }
                                options.MaxRows = exprParser.Expression();
                                goto os_break;
                            }

                            case SpecialIdentifier.MinRows:
                            {
                                // | MIN_ROWS [=] value
                                if (lexer.NextToken() == MySqlToken.OpEquals)
                                {
                                    lexer.NextToken();
                                }
                                options.MinRows = exprParser.Expression();
                                goto os_break;
                            }

                            case SpecialIdentifier.PackKeys:
                            {
                                // | PACK_KEYS [=] {0 | 1 | DEFAULT}
                                if (lexer.NextToken() == MySqlToken.OpEquals)
                                {
                                    lexer.NextToken();
                                }
                                switch (lexer.Token())
                                {
                                    case MySqlToken.LiteralBoolFalse:
                                    {
                                        lexer.NextToken();
                                        options.PackKeys = PackKeys.False;
                                        break;
                                    }

                                    case MySqlToken.LiteralBoolTrue:
                                    {
                                        lexer.NextToken();
                                        options.PackKeys = PackKeys.True;
                                        break;
                                    }

                                    case MySqlToken.LiteralNumPureDigit:
                                    {
                                        var intVal_2 = (int) lexer.GetIntegerValue();
                                        lexer.NextToken();
                                        if (intVal_2 == 0)
                                        {
                                            options.PackKeys = PackKeys.False;
                                        }
                                        else
                                        {
                                            options.PackKeys = PackKeys.True;
                                        }
                                        break;
                                    }

                                    case MySqlToken.KwDefault:
                                    {
                                        lexer.NextToken();
                                        options.PackKeys = PackKeys.Default;
                                        break;
                                    }

                                    default:
                                    {
                                        throw new SqlSyntaxErrorException("table option of PACK_KEYS error");
                                    }
                                }
                                goto os_break;
                            }

                            case SpecialIdentifier.Password:
                            {
                                // | PASSWORD [=] 'string'
                                if (lexer.NextToken() == MySqlToken.OpEquals)
                                {
                                    lexer.NextToken();
                                }
                                options.Password = (LiteralString) exprParser.Expression();
                                goto os_break;
                            }

                            case SpecialIdentifier.RowFormat:
                            {
                                // | ROW_FORMAT [=]
                                // {DEFAULT|DYNAMIC|FIXED|COMPRESSED|REDUNDANT|COMPACT}
                                if (lexer.NextToken() == MySqlToken.OpEquals)
                                {
                                    lexer.NextToken();
                                }
                                switch (lexer.Token())
                                {
                                    case MySqlToken.KwDefault:
                                    {
                                        lexer.NextToken();
                                        options.RowFormat = RowFormat.Default;
                                        goto os_break;
                                    }

                                    case MySqlToken.Identifier:
                                    {
                                        var sid = specialIdentifiers.GetValue(lexer.GetStringValueUppercase());
                                        if (sid != SpecialIdentifier.None)
                                        {
                                            switch (sid)
                                            {
                                                case SpecialIdentifier.Dynamic:
                                                {
                                                    lexer.NextToken();
                                                    options.RowFormat = RowFormat.Dynamic;
                                                    goto os_break;
                                                }

                                                case SpecialIdentifier.Fixed:
                                                {
                                                    lexer.NextToken();
                                                    options.RowFormat = RowFormat.Fixed;
                                                    goto os_break;
                                                }

                                                case SpecialIdentifier.Compressed:
                                                {
                                                    lexer.NextToken();
                                                    options.RowFormat = RowFormat.Compressed;
                                                    goto os_break;
                                                }

                                                case SpecialIdentifier.Redundant:
                                                {
                                                    lexer.NextToken();
                                                    options.RowFormat = RowFormat.Redundant;
                                                    goto os_break;
                                                }

                                                case SpecialIdentifier.Compact:
                                                {
                                                    lexer.NextToken();
                                                    options.RowFormat = RowFormat.Compact;
                                                    goto os_break;
                                                }
                                            }
                                        }
                                        goto default;
                                    }

                                    default:
                                    {
                                        throw new SqlSyntaxErrorException("table option of ROW_FORMAT error");
                                    }
                                }
                                //break;
                            }
                        }
                    }
                    goto default;
                }

                default:
                {
                    return false;
                }
            }
            os_break:
            ;
            return true;
        }

        /// <summary>
        ///     MySqlDdlParser SpecialIdentifier
        /// </summary>
        private enum SpecialIdentifier
        {
            None = 0,

            Truncate,
            Temporary,
            Definer,
            KeyBlockSize,
            Comment,
            Dynamic,
            Fixed,
            Bit,
            Date,
            Time,
            Timestamp,
            Datetime,
            Year,
            Text,
            Enum,
            Engine,
            AutoIncrement,
            AvgRowLength,
            Checksum,
            Connection,
            Data,
            DelayKeyWrite,
            InsertMethod,
            MaxRows,
            MinRows,
            PackKeys,
            Password,
            RowFormat,
            Compressed,
            Redundant,
            Compact,
            Modify,
            Disable,
            Enable,
            Discard,
            Import,
            Charset,
            Policy
        }
    }
}