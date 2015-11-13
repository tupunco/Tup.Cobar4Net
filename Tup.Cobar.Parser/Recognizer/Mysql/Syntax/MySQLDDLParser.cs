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

using Tup.Cobar.Parser.Ast.Expression.Primary;
using Tup.Cobar.Parser.Ast.Expression.Primary.Literal;
using Tup.Cobar.Parser.Ast.Fragment.Ddl.Index;
using Tup.Cobar.Parser.Ast.Stmt.Ddl;
using Tup.Cobar.Parser.Ast.Stmt.Dml;
using Tup.Cobar.Parser.Ast.Stmt.Extension;
using Tup.Cobar.Parser.Recognizer.Mysql.Lexer;
using Tup.Cobar.Parser.Util;
using DdlColumnDefinition = Tup.Cobar.Parser.Ast.Fragment.Ddl.ColumnDefinition;
using DdlDatatype = Tup.Cobar.Parser.Ast.Fragment.Ddl.Datatype.DataType;
using DdlTableOptions = Tup.Cobar.Parser.Ast.Fragment.Ddl.TableOptions;

namespace Tup.Cobar.Parser.Recognizer.Mysql.Syntax
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class MySQLDDLParser : MySQLParser
    {
        protected internal MySQLExprParser exprParser;

        public MySQLDDLParser(MySQLLexer lexer, MySQLExprParser exprParser)
            : base(lexer)
        {
            this.exprParser = exprParser;
        }

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

        private static readonly IDictionary<string, SpecialIdentifier> specialIdentifiers
             = new Dictionary<string, SpecialIdentifier>();

        static MySQLDDLParser()
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

        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        public virtual DDLTruncateStatement Truncate()
        {
            MatchIdentifier("TRUNCATE");
            if (lexer.Token() == MySQLToken.KwTable)
            {
                lexer.NextToken();
            }
            Identifier tb = Identifier();
            return new DDLTruncateStatement(tb);
        }

        /// <summary>nothing has been pre-consumed</summary>
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        public virtual DDLStatement DdlStmt()
        {
            Identifier idTemp1;
            Identifier idTemp2;
            SpecialIdentifier siTemp;
            switch (lexer.Token())
            {
                case MySQLToken.KwAlter:
                    {
                        bool ignore = false;
                        if (lexer.NextToken() == MySQLToken.KwIgnore)
                        {
                            ignore = true;
                            lexer.NextToken();
                        }
                        switch (lexer.Token())
                        {
                            case MySQLToken.KwTable:
                                {
                                    lexer.NextToken();
                                    idTemp1 = Identifier();
                                    DDLAlterTableStatement alterTableStatement = new DDLAlterTableStatement(ignore, idTemp1);
                                    return AlterTable(alterTableStatement);
                                }

                            default:
                                {
                                    throw Err("Only ALTER TABLE is supported");
                                }
                        }
                        //goto case MySQLToken.KwCreate;
                    }

                case MySQLToken.KwCreate:
                    {
                        switch (lexer.NextToken())
                        {
                            case MySQLToken.KwUnique:
                            case MySQLToken.KwFulltext:
                            case MySQLToken.KwSpatial:
                                {
                                    lexer.NextToken();
                                    goto case MySQLToken.KwIndex;
                                }

                            case MySQLToken.KwIndex:
                                {
                                    lexer.NextToken();
                                    idTemp1 = Identifier();
                                    for (; lexer.Token() != MySQLToken.KwOn; lexer.NextToken())
                                    {
                                    }
                                    lexer.NextToken();
                                    idTemp2 = Identifier();
                                    return new DDLCreateIndexStatement(idTemp1, idTemp2);
                                }

                            case MySQLToken.KwTable:
                                {
                                    lexer.NextToken();
                                    return CreateTable(false);
                                }

                            case MySQLToken.Identifier:
                                {
                                    siTemp = specialIdentifiers.GetValue(lexer.StringValueUppercase());
                                    if (siTemp != SpecialIdentifier.None)
                                    {
                                        switch (siTemp)
                                        {
                                            case SpecialIdentifier.Temporary:
                                                {
                                                    lexer.NextToken();
                                                    Match(MySQLToken.KwTable);
                                                    return CreateTable(true);
                                                }

                                            case SpecialIdentifier.Policy:
                                                {
                                                    lexer.NextToken();
                                                    Identifier policyName = Identifier();
                                                    Match(MySQLToken.PuncLeftParen);
                                                    ExtDDLCreatePolicy policy = new ExtDDLCreatePolicy(policyName);
                                                    for (int j = 0; lexer.Token() != MySQLToken.PuncRightParen; ++j)
                                                    {
                                                        if (j > 0)
                                                        {
                                                            Match(MySQLToken.PuncComma);
                                                        }
                                                        int id = (int)lexer.IntegerValue();
                                                        Match(MySQLToken.LiteralNumPureDigit);
                                                        Tup.Cobar.Parser.Ast.Expression.Expression val = exprParser.Expression();
                                                        policy.AddProportion(id, val);
                                                    }
                                                    Match(MySQLToken.PuncRightParen);
                                                    return policy;
                                                }
                                        }
                                    }
                                    goto default;
                                }

                            default:
                                {
                                    throw Err("unsupported DDL for CREATE");
                                }
                        }
                        //goto case MySQLToken.KwDrop;
                    }

                case MySQLToken.KwDrop:
                    {
                        switch (lexer.NextToken())
                        {
                            case MySQLToken.KwIndex:
                                {
                                    lexer.NextToken();
                                    idTemp1 = Identifier();
                                    Match(MySQLToken.KwOn);
                                    idTemp2 = Identifier();
                                    return new DDLDropIndexStatement(idTemp1, idTemp2);
                                }

                            case MySQLToken.KwTable:
                                {
                                    lexer.NextToken();
                                    return DropTable(false);
                                }

                            case MySQLToken.Identifier:
                                {
                                    siTemp = specialIdentifiers.GetValue(lexer.StringValueUppercase());
                                    if (siTemp != SpecialIdentifier.None)
                                    {
                                        switch (siTemp)
                                        {
                                            case SpecialIdentifier.Temporary:
                                                {
                                                    lexer.NextToken();
                                                    Match(MySQLToken.KwTable);
                                                    return DropTable(true);
                                                }

                                            case SpecialIdentifier.Policy:
                                                {
                                                    lexer.NextToken();
                                                    Identifier policyName = Identifier();
                                                    return new ExtDDLDropPolicy(policyName);
                                                }
                                        }
                                    }
                                    goto default;
                                }

                            default:
                                {
                                    throw Err("unsupported DDL for DROP");
                                }
                        }
                        //goto case MySQLToken.KwRename;
                    }

                case MySQLToken.KwRename:
                    {
                        lexer.NextToken();
                        Match(MySQLToken.KwTable);
                        idTemp1 = Identifier();
                        Match(MySQLToken.KwTo);
                        idTemp2 = Identifier();
                        IList<Pair<Identifier, Identifier>> list;
                        if (lexer.Token() != MySQLToken.PuncComma)
                        {
                            list = new List<Pair<Identifier, Identifier>>(1);
                            list.Add(new Pair<Identifier, Identifier>(idTemp1, idTemp2));
                            return new DDLRenameTableStatement(list);
                        }
                        list = new List<Pair<Identifier, Identifier>>();
                        list.Add(new Pair<Identifier, Identifier>(idTemp1, idTemp2));
                        for (; lexer.Token() == MySQLToken.PuncComma;)
                        {
                            lexer.NextToken();
                            idTemp1 = Identifier();
                            Match(MySQLToken.KwTo);
                            idTemp2 = Identifier();
                            list.Add(new Pair<Identifier, Identifier>(idTemp1, idTemp2));
                        }
                        return new DDLRenameTableStatement(list);
                    }

                case MySQLToken.Identifier:
                    {
                        SpecialIdentifier si = specialIdentifiers.GetValue(lexer.StringValueUppercase());
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
                        throw Err("unsupported DDL");
                    }
            }
        }

        /// <summary><code>TABLE</code> has been consumed</summary>
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        private DDLDropTableStatement DropTable(bool temp)
        {
            bool ifExists = false;
            if (lexer.Token() == MySQLToken.KwIf)
            {
                lexer.NextToken();
                Match(MySQLToken.KwExists);
                ifExists = true;
            }
            Identifier tb = Identifier();
            IList<Identifier> list;
            if (lexer.Token() != MySQLToken.PuncComma)
            {
                list = new List<Identifier>(1);
                list.Add(tb);
            }
            else
            {
                list = new List<Identifier>();
                list.Add(tb);
                for (; lexer.Token() == MySQLToken.PuncComma;)
                {
                    lexer.NextToken();
                    tb = Identifier();
                    list.Add(tb);
                }
            }
            DDLDropTableStatement.Mode mode = DDLDropTableStatement.Mode.Undef;
            switch (lexer.Token())
            {
                case MySQLToken.KwRestrict:
                    {
                        lexer.NextToken();
                        mode = DDLDropTableStatement.Mode.Restrict;
                        break;
                    }

                case MySQLToken.KwCascade:
                    {
                        lexer.NextToken();
                        mode = DDLDropTableStatement.Mode.Cascade;
                        break;
                    }
            }
            return new DDLDropTableStatement(list, temp, ifExists, mode);
        }

        /// <summary>token of table name has been consumed</summary>
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        private DDLAlterTableStatement AlterTable(DDLAlterTableStatement stmt)
        {
            DdlTableOptions options = new DdlTableOptions
                ();
            stmt.SetTableOptions(options);
            Identifier id = null;
            Identifier id2 = null;
            Identifier id3 = null;
            DdlColumnDefinition colDef = null;
            IndexDefinition indexDef = null;
            Tup.Cobar.Parser.Ast.Expression.Expression expr = null;
            for (int i = 0; lexer.Token() != MySQLToken.Eof; ++i)
            {
                if (i > 0)
                {
                    Match(MySQLToken.PuncComma);
                }
                if (TableOptions(options))
                {
                    continue;
                }
                switch (lexer.Token())
                {
                    case MySQLToken.KwConvert:
                        {
                            // | CONVERT TO CHARACTER SET charset_name [COLLATE
                            // collation_name]
                            lexer.NextToken();
                            Match(MySQLToken.KwTo);
                            Match(MySQLToken.KwCharacter);
                            Match(MySQLToken.KwSet);
                            id = Identifier();
                            id2 = null;
                            if (lexer.Token() == MySQLToken.KwCollate)
                            {
                                lexer.NextToken();
                                id2 = Identifier();
                            }
                            stmt.SetConvertCharset(new Pair<Identifier, Identifier>(id, id2));
                            goto main_switch_break;
                        }

                    case MySQLToken.KwRename:
                        {
                            // | RENAME [TO] new_tbl_name
                            if (lexer.NextToken() == MySQLToken.KwTo)
                            {
                                lexer.NextToken();
                            }
                            id = Identifier();
                            stmt.SetRenameTo(id);
                            goto main_switch_break;
                        }

                    case MySQLToken.KwDrop:
                        {
                            switch (lexer.NextToken())
                            {
                                case MySQLToken.KwIndex:
                                case MySQLToken.KwKey:
                                    {
                                        // | DROP {INDEX|KEY} index_name
                                        lexer.NextToken();
                                        id = Identifier();
                                        stmt.AddAlterSpecification(new DDLAlterTableStatement.DropIndex(id));
                                        goto drop_switch_break;
                                    }

                                case MySQLToken.KwPrimary:
                                    {
                                        // | DROP PRIMARY KEY
                                        lexer.NextToken();
                                        Match(MySQLToken.KwKey);
                                        stmt.AddAlterSpecification(new DDLAlterTableStatement.DropPrimaryKey());
                                        goto drop_switch_break;
                                    }

                                case MySQLToken.Identifier:
                                    {
                                        // | DROP [COLUMN] col_name
                                        id = Identifier();
                                        stmt.AddAlterSpecification(new DDLAlterTableStatement.DropColumn(id));
                                        goto drop_switch_break;
                                    }

                                case MySQLToken.KwColumn:
                                    {
                                        // | DROP [COLUMN] col_name
                                        lexer.NextToken();
                                        id = Identifier();
                                        stmt.AddAlterSpecification(new DDLAlterTableStatement.DropColumn(id));
                                        goto drop_switch_break;
                                    }

                                default:
                                    {
                                        throw new SQLSyntaxErrorException("ALTER TABLE error for DROP");
                                    }
                            }
                        drop_switch_break:;
                            goto main_switch_break;
                        }

                    case MySQLToken.KwChange:
                        {
                            // | CHANGE [COLUMN] old_col_name new_col_name column_definition
                            // [FIRST|AFTER col_name]
                            if (lexer.NextToken() == MySQLToken.KwColumn)
                            {
                                lexer.NextToken();
                            }
                            id = Identifier();
                            id2 = Identifier();
                            colDef = ColumnDefinition();
                            if (lexer.Token() == MySQLToken.Identifier)
                            {
                                if ("FIRST".Equals(lexer.StringValueUppercase()))
                                {
                                    lexer.NextToken();
                                    stmt.AddAlterSpecification(new DDLAlterTableStatement.ChangeColumn(id, id2, colDef
                                        , null));
                                }
                                else
                                {
                                    if ("AFTER".Equals(lexer.StringValueUppercase()))
                                    {
                                        lexer.NextToken();
                                        id3 = Identifier();
                                        stmt.AddAlterSpecification(new DDLAlterTableStatement.ChangeColumn(id, id2, colDef
                                            , id3));
                                    }
                                    else
                                    {
                                        stmt.AddAlterSpecification(new DDLAlterTableStatement.ChangeColumn(id, id2, colDef
                                            ));
                                    }
                                }
                            }
                            else
                            {
                                stmt.AddAlterSpecification(new DDLAlterTableStatement.ChangeColumn(id, id2, colDef
                                    ));
                            }
                            goto main_switch_break;
                        }

                    case MySQLToken.KwAlter:
                        {
                            // | ALTER [COLUMN] col_name {SET DEFAULT literal | DROP
                            // DEFAULT}
                            if (lexer.NextToken() == MySQLToken.KwColumn)
                            {
                                lexer.NextToken();
                            }
                            id = Identifier();
                            switch (lexer.Token())
                            {
                                case MySQLToken.KwSet:
                                    {
                                        lexer.NextToken();
                                        Match(MySQLToken.KwDefault);
                                        expr = exprParser.Expression();
                                        stmt.AddAlterSpecification(new DDLAlterTableStatement.AlterColumnDefaultVal(id, expr
                                            ));
                                        break;
                                    }

                                case MySQLToken.KwDrop:
                                    {
                                        lexer.NextToken();
                                        Match(MySQLToken.KwDefault);
                                        stmt.AddAlterSpecification(new DDLAlterTableStatement.AlterColumnDefaultVal(id));
                                        break;
                                    }

                                default:
                                    {
                                        throw new SQLSyntaxErrorException("ALTER TABLE error for ALTER");
                                    }
                            }
                            goto main_switch_break;
                        }

                    case MySQLToken.KwAdd:
                        {
                            switch (lexer.NextToken())
                            {
                                case MySQLToken.Identifier:
                                    {
                                        // | ADD [COLUMN] col_name column_definition [FIRST | AFTER
                                        // col_name ]
                                        id = Identifier();
                                        colDef = ColumnDefinition();
                                        if (lexer.Token() == MySQLToken.Identifier)
                                        {
                                            if ("FIRST".Equals(lexer.StringValueUppercase()))
                                            {
                                                lexer.NextToken();
                                                stmt.AddAlterSpecification(new DDLAlterTableStatement.AddColumn(id, colDef, null)
                                                    );
                                            }
                                            else
                                            {
                                                if ("AFTER".Equals(lexer.StringValueUppercase()))
                                                {
                                                    lexer.NextToken();
                                                    id2 = Identifier();
                                                    stmt.AddAlterSpecification(new DDLAlterTableStatement.AddColumn(id, colDef, id2));
                                                }
                                                else
                                                {
                                                    stmt.AddAlterSpecification(new DDLAlterTableStatement.AddColumn(id, colDef));
                                                }
                                            }
                                        }
                                        else
                                        {
                                            stmt.AddAlterSpecification(new DDLAlterTableStatement.AddColumn(id, colDef));
                                        }
                                        goto add_switch_break;
                                    }

                                case MySQLToken.PuncLeftParen:
                                    {
                                        // | ADD [COLUMN] (col_name column_definition,...)
                                        lexer.NextToken();
                                        for (int j = 0; lexer.Token() != MySQLToken.PuncRightParen; ++j)
                                        {
                                            DDLAlterTableStatement.AddColumns addColumns = new DDLAlterTableStatement.AddColumns
                                                ();
                                            stmt.AddAlterSpecification(addColumns);
                                            if (j > 0)
                                            {
                                                Match(MySQLToken.PuncComma);
                                            }
                                            id = Identifier();
                                            colDef = ColumnDefinition();
                                            addColumns.AddColumn(id, colDef);
                                        }
                                        Match(MySQLToken.PuncRightParen);
                                        goto add_switch_break;
                                    }

                                case MySQLToken.KwColumn:
                                    {
                                        if (lexer.NextToken() == MySQLToken.PuncLeftParen)
                                        {
                                            // | ADD [COLUMN] (col_name column_definition,...)
                                            lexer.NextToken();
                                            for (int j_1 = 0; lexer.Token() != MySQLToken.PuncRightParen; ++j_1)
                                            {
                                                DDLAlterTableStatement.AddColumns addColumns = new DDLAlterTableStatement.AddColumns
                                                    ();
                                                stmt.AddAlterSpecification(addColumns);
                                                if (j_1 > 0)
                                                {
                                                    Match(MySQLToken.PuncComma);
                                                }
                                                id = Identifier();
                                                colDef = ColumnDefinition();
                                                addColumns.AddColumn(id, colDef);
                                            }
                                            Match(MySQLToken.PuncRightParen);
                                        }
                                        else
                                        {
                                            // | ADD [COLUMN] col_name column_definition [FIRST |
                                            // AFTER col_name ]
                                            id = Identifier();
                                            colDef = ColumnDefinition();
                                            if (lexer.Token() == MySQLToken.Identifier)
                                            {
                                                if ("FIRST".Equals(lexer.StringValueUppercase()))
                                                {
                                                    lexer.NextToken();
                                                    stmt.AddAlterSpecification(new DDLAlterTableStatement.AddColumn(id, colDef, null)
                                                        );
                                                }
                                                else
                                                {
                                                    if ("AFTER".Equals(lexer.StringValueUppercase()))
                                                    {
                                                        lexer.NextToken();
                                                        id2 = Identifier();
                                                        stmt.AddAlterSpecification(new DDLAlterTableStatement.AddColumn(id, colDef, id2));
                                                    }
                                                    else
                                                    {
                                                        stmt.AddAlterSpecification(new DDLAlterTableStatement.AddColumn(id, colDef));
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                stmt.AddAlterSpecification(new DDLAlterTableStatement.AddColumn(id, colDef));
                                            }
                                        }
                                        goto add_switch_break;
                                    }

                                case MySQLToken.KwIndex:
                                case MySQLToken.KwKey:
                                    {
                                        // | ADD {INDEX|KEY} [index_name] [index_type]
                                        // (index_col_name,...) [index_option] ...
                                        id = null;
                                        if (lexer.NextToken() == MySQLToken.Identifier)
                                        {
                                            id = Identifier();
                                        }
                                        indexDef = IndexDefinition();
                                        stmt.AddAlterSpecification(new DDLAlterTableStatement.AddIndex(id, indexDef));
                                        goto add_switch_break;
                                    }

                                case MySQLToken.KwPrimary:
                                    {
                                        // | ADD PRIMARY KEY [index_type] (index_col_name,...)
                                        // [index_option] ...
                                        lexer.NextToken();
                                        Match(MySQLToken.KwKey);
                                        indexDef = IndexDefinition();
                                        stmt.AddAlterSpecification(new DDLAlterTableStatement.AddPrimaryKey(indexDef));
                                        goto add_switch_break;
                                    }

                                case MySQLToken.KwUnique:
                                    {
                                        switch (lexer.NextToken())
                                        {
                                            case MySQLToken.KwIndex:
                                            case MySQLToken.KwKey:
                                                {
                                                    // | ADD UNIQUE [INDEX|KEY] [index_name] [index_type]
                                                    // (index_col_name,...) [index_option] ...
                                                    lexer.NextToken();
                                                    break;
                                                }
                                        }
                                        id = null;
                                        if (lexer.Token() == MySQLToken.Identifier)
                                        {
                                            id = Identifier();
                                        }
                                        indexDef = IndexDefinition();
                                        stmt.AddAlterSpecification(new DDLAlterTableStatement.AddUniqueKey(id, indexDef));
                                        goto add_switch_break;
                                    }

                                case MySQLToken.KwFulltext:
                                    {
                                        switch (lexer.NextToken())
                                        {
                                            case MySQLToken.KwIndex:
                                            case MySQLToken.KwKey:
                                                {
                                                    // | ADD FULLTEXT [INDEX|KEY] [index_name]
                                                    // (index_col_name,...) [index_option] ...
                                                    lexer.NextToken();
                                                    break;
                                                }
                                        }
                                        id = null;
                                        if (lexer.Token() == MySQLToken.Identifier)
                                        {
                                            id = Identifier();
                                        }
                                        indexDef = IndexDefinition();
                                        stmt.AddAlterSpecification(new DDLAlterTableStatement.AddFullTextIndex(id, indexDef
                                            ));
                                        goto add_switch_break;
                                    }

                                case MySQLToken.KwSpatial:
                                    {
                                        switch (lexer.NextToken())
                                        {
                                            case MySQLToken.KwIndex:
                                            case MySQLToken.KwKey:
                                                {
                                                    // | ADD SPATIAL [INDEX|KEY] [index_name]
                                                    // (index_col_name,...) [index_option] ...
                                                    lexer.NextToken();
                                                    break;
                                                }
                                        }
                                        id = null;
                                        if (lexer.Token() == MySQLToken.Identifier)
                                        {
                                            id = Identifier();
                                        }
                                        indexDef = IndexDefinition();
                                        stmt.AddAlterSpecification(new DDLAlterTableStatement.AddSpatialIndex(id, indexDef
                                            ));
                                        goto add_switch_break;
                                    }

                                default:
                                    {
                                        throw new SQLSyntaxErrorException("ALTER TABLE error for ADD");
                                    }
                            }
                        add_switch_break:;
                            goto main_switch_break;
                        }

                    case MySQLToken.Identifier:
                        {
                            var si = specialIdentifiers.GetValue(lexer.StringValueUppercase());
                            if (si != SpecialIdentifier.None)
                            {
                                switch (si)
                                {
                                    case SpecialIdentifier.Import:
                                        {
                                            // | IMPORT TABLESPACE
                                            lexer.NextToken();
                                            MatchIdentifier("TABLESPACE");
                                            stmt.SetImportTableSpace(true);
                                            goto main_switch_break;
                                        }

                                    case SpecialIdentifier.Discard:
                                        {
                                            // | DISCARD TABLESPACE
                                            lexer.NextToken();
                                            MatchIdentifier("TABLESPACE");
                                            stmt.SetDiscardTableSpace(true);
                                            goto main_switch_break;
                                        }

                                    case SpecialIdentifier.Enable:
                                        {
                                            // | ENABLE KEYS
                                            lexer.NextToken();
                                            Match(MySQLToken.KwKeys);
                                            stmt.SetEnableKeys(true);
                                            goto main_switch_break;
                                        }

                                    case SpecialIdentifier.Disable:
                                        {
                                            // | DISABLE KEYS
                                            lexer.NextToken();
                                            Match(MySQLToken.KwKeys);
                                            stmt.SetDisableKeys(true);
                                            goto main_switch_break;
                                        }

                                    case SpecialIdentifier.Modify:
                                        {
                                            // | MODIFY [COLUMN] col_name column_definition [FIRST |
                                            // AFTER col_name]
                                            if (lexer.NextToken() == MySQLToken.KwColumn)
                                            {
                                                lexer.NextToken();
                                            }
                                            id = Identifier();
                                            colDef = ColumnDefinition();
                                            if (lexer.Token() == MySQLToken.Identifier)
                                            {
                                                if ("FIRST".Equals(lexer.StringValueUppercase()))
                                                {
                                                    lexer.NextToken();
                                                    stmt.AddAlterSpecification(new DDLAlterTableStatement.ModifyColumn(id, colDef, null
                                                        ));
                                                }
                                                else
                                                {
                                                    if ("AFTER".Equals(lexer.StringValueUppercase()))
                                                    {
                                                        lexer.NextToken();
                                                        id2 = Identifier();
                                                        stmt.AddAlterSpecification(new DDLAlterTableStatement.ModifyColumn(id, colDef, id2
                                                            ));
                                                    }
                                                    else
                                                    {
                                                        stmt.AddAlterSpecification(new DDLAlterTableStatement.ModifyColumn(id, colDef));
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                stmt.AddAlterSpecification(new DDLAlterTableStatement.ModifyColumn(id, colDef));
                                            }
                                            goto main_switch_break;
                                        }
                                }
                            }
                            goto default;
                        }

                    default:
                        {
                            throw new SQLSyntaxErrorException("unknown ALTER specification");
                        }
                }
            main_switch_break:;
            }
            return stmt;
        }

        /// <summary><code>TABLE</code> has been consumed</summary>
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        private DDLCreateTableStatement CreateTable(bool temp)
        {
            bool ifNotExists = false;
            if (lexer.Token() == MySQLToken.KwIf)
            {
                lexer.NextToken();
                Match(MySQLToken.KwNot);
                Match(MySQLToken.KwExists);
                ifNotExists = true;
            }
            Identifier table = Identifier();
            DDLCreateTableStatement stmt = new DDLCreateTableStatement(temp, ifNotExists, table
                );
            CreateTableDefs(stmt);
            DdlTableOptions options = new DdlTableOptions
                ();
            stmt.SetTableOptions(options);
            TableOptions(options);
            DDLCreateTableStatement.SelectOption selectOpt = DDLCreateTableStatement.SelectOption.None;
            switch (lexer.Token())
            {
                case MySQLToken.KwIgnore:
                    {
                        selectOpt = DDLCreateTableStatement.SelectOption.Ignored;
                        if (lexer.NextToken() == MySQLToken.KwAs)
                        {
                            lexer.NextToken();
                        }
                        break;
                    }

                case MySQLToken.KwReplace:
                    {
                        selectOpt = DDLCreateTableStatement.SelectOption.Replace;
                        if (lexer.NextToken() == MySQLToken.KwAs)
                        {
                            lexer.NextToken();
                        }
                        break;
                    }

                case MySQLToken.KwAs:
                    {
                        lexer.NextToken();
                        goto case MySQLToken.KwSelect;
                    }

                case MySQLToken.KwSelect:
                    {
                        break;
                    }

                case MySQLToken.Eof:
                    {
                        return stmt;
                    }

                default:
                    {
                        throw new SQLSyntaxErrorException("DDL CREATE TABLE statement not end properly");
                    }
            }
            DMLSelectStatement select = new MySQLDMLSelectParser(lexer, exprParser).Select();
            stmt.SetSelect(selectOpt, select);
            Match(MySQLToken.Eof);
            return stmt;
        }

        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        private void CreateTableDefs(DDLCreateTableStatement stmt)
        {
            if (lexer.Token() != MySQLToken.PuncLeftParen)
            {
                return;
            }
            Match(MySQLToken.PuncLeftParen);
            IndexDefinition indexDef;
            Identifier id;
            for (int i = 0; lexer.Token() != MySQLToken.PuncRightParen; ++i)
            {
                if (i > 0)
                {
                    Match(MySQLToken.PuncComma);
                }
                switch (lexer.Token())
                {
                    case MySQLToken.KwPrimary:
                        {
                            lexer.NextToken();
                            Match(MySQLToken.KwKey);
                            indexDef = IndexDefinition();
                            stmt.SetPrimaryKey(indexDef);
                            break;
                        }

                    case MySQLToken.KwIndex:
                    case MySQLToken.KwKey:
                        {
                            lexer.NextToken();
                            if (lexer.Token() == MySQLToken.Identifier)
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

                    case MySQLToken.KwUnique:
                        {
                            switch (lexer.NextToken())
                            {
                                case MySQLToken.KwIndex:
                                case MySQLToken.KwKey:
                                    {
                                        lexer.NextToken();
                                        break;
                                    }
                            }
                            if (lexer.Token() == MySQLToken.Identifier)
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

                    case MySQLToken.KwFulltext:
                        {
                            switch (lexer.NextToken())
                            {
                                case MySQLToken.KwIndex:
                                case MySQLToken.KwKey:
                                    {
                                        lexer.NextToken();
                                        break;
                                    }
                            }
                            if (lexer.Token() == MySQLToken.Identifier)
                            {
                                id = Identifier();
                            }
                            else
                            {
                                id = null;
                            }
                            indexDef = IndexDefinition();
                            if (indexDef.GetIndexType() != IndexType.None)
                            {
                                throw new SQLSyntaxErrorException("FULLTEXT INDEX can specify no index_type");
                            }
                            stmt.AddFullTextIndex(id, indexDef);
                            break;
                        }

                    case MySQLToken.KwSpatial:
                        {
                            switch (lexer.NextToken())
                            {
                                case MySQLToken.KwIndex:
                                case MySQLToken.KwKey:
                                    {
                                        lexer.NextToken();
                                        break;
                                    }
                            }
                            if (lexer.Token() == MySQLToken.Identifier)
                            {
                                id = Identifier();
                            }
                            else
                            {
                                id = null;
                            }
                            indexDef = IndexDefinition();
                            if (indexDef.GetIndexType() != IndexType.None)
                            {
                                throw new SQLSyntaxErrorException("SPATIAL INDEX can specify no index_type");
                            }
                            stmt.AddSpatialIndex(id, indexDef);
                            break;
                        }

                    case MySQLToken.KwCheck:
                        {
                            lexer.NextToken();
                            Match(MySQLToken.PuncLeftParen);
                            Tup.Cobar.Parser.Ast.Expression.Expression expr = exprParser.Expression();
                            Match(MySQLToken.PuncRightParen);
                            stmt.AddCheck(expr);
                            break;
                        }

                    case MySQLToken.Identifier:
                        {
                            Identifier columnName = Identifier();
                            DdlColumnDefinition columnDef = ColumnDefinition();
                            stmt.AddColumnDefinition(columnName, columnDef);
                            break;
                        }

                    default:
                        {
                            throw new SQLSyntaxErrorException("unsupportted column definition");
                        }
                }
            }
            Match(MySQLToken.PuncRightParen);
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
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        private IndexDefinition IndexDefinition()
        {
            IndexType indexType = IndexType.None;
            IList<IndexColumnName> columns = new List<IndexColumnName>(1);
            if (lexer.Token() == MySQLToken.KwUsing)
            {
                lexer.NextToken();
                int tp = MatchIdentifier("BTREE", "HASH");
                indexType = tp == 0 ? IndexType.Btree : IndexType.Hash;
            }
            Match(MySQLToken.PuncLeftParen);
            for (int i = 0; lexer.Token() != MySQLToken.PuncRightParen; ++i)
            {
                if (i > 0)
                {
                    Match(MySQLToken.PuncComma);
                }
                IndexColumnName indexColumnName = IndexColumnName
                    ();
                columns.Add(indexColumnName);
            }
            Match(MySQLToken.PuncRightParen);
            IList<IndexOption> options = IndexOptions();
            return new IndexDefinition(indexType, columns, options);
        }

        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        private IList<IndexOption> IndexOptions()
        {
            IList<IndexOption> list = null;
            for (;;)
            {
                switch (lexer.Token())
                {
                    case MySQLToken.KwUsing:
                        {
                            lexer.NextToken();
                            IndexType indexType = MatchIdentifier("BTREE", "HASH") == 0 ? IndexType.Btree : IndexType.Hash;
                            if (list == null)
                            {
                                list = new List<IndexOption>(1);
                            }
                            list.Add(new IndexOption(indexType));
                            goto main_switch_break;
                        }

                    case MySQLToken.KwWith:
                        {
                            lexer.NextToken();
                            MatchIdentifier("PARSER");
                            Identifier id = Identifier();
                            if (list == null)
                            {
                                list = new List<IndexOption>(1);
                            }
                            list.Add(new IndexOption(id));
                            goto main_switch_break;
                        }

                    case MySQLToken.Identifier:
                        {
                            var si = specialIdentifiers.GetValue(lexer.StringValueUppercase());
                            if (si != SpecialIdentifier.None)
                            {
                                switch (si)
                                {
                                    case SpecialIdentifier.KeyBlockSize:
                                        {
                                            lexer.NextToken();
                                            if (lexer.Token() == MySQLToken.OpEquals)
                                            {
                                                lexer.NextToken();
                                            }
                                            Tup.Cobar.Parser.Ast.Expression.Expression val = exprParser.Expression();
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
                                            LiteralString @string = (LiteralString)exprParser.Expression();
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
            main_switch_break:;
            }
        }

        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        private IndexColumnName IndexColumnName()
        {
            // col_name [(length)] [ASC | DESC]
            Identifier colName = Identifier();
            Tup.Cobar.Parser.Ast.Expression.Expression len = null;
            if (lexer.Token() == MySQLToken.PuncLeftParen)
            {
                lexer.NextToken();
                len = exprParser.Expression();
                Match(MySQLToken.PuncRightParen);
            }
            switch (lexer.Token())
            {
                case MySQLToken.KwAsc:
                    {
                        lexer.NextToken();
                        return new IndexColumnName(colName, len,
                            true);
                    }

                case MySQLToken.KwDesc:
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
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        private DdlDatatype DataType()
        {
            var typeName = DdlDatatype.DataTypeName.None;
            bool unsigned = false;
            bool zerofill = false;
            bool binary = false;
            Tup.Cobar.Parser.Ast.Expression.Expression length = null;
            Tup.Cobar.Parser.Ast.Expression.Expression decimals = null;
            Identifier charSet = null;
            Identifier collation = null;
            IList<Tup.Cobar.Parser.Ast.Expression.Expression> collectionVals = null;
            switch (lexer.Token())
            {
                case MySQLToken.KwTinyint:
                    {
                        // | TINYINT[(length)] [UNSIGNED] [ZEROFILL]
                        typeName = DdlDatatype.DataTypeName.Tinyint;
                        if (lexer.NextToken() == MySQLToken.PuncLeftParen)
                        {
                            lexer.NextToken();
                            length = exprParser.Expression();
                            Match(MySQLToken.PuncRightParen);
                        }
                        if (lexer.Token() == MySQLToken.KwUnsigned)
                        {
                            unsigned = true;
                            lexer.NextToken();
                        }
                        if (lexer.Token() == MySQLToken.KwZerofill)
                        {
                            zerofill = true;
                            lexer.NextToken();
                        }
                        goto typeName_break;
                    }

                case MySQLToken.KwSmallint:
                    {
                        // | SMALLINT[(length)] [UNSIGNED] [ZEROFILL]
                        typeName = DdlDatatype.DataTypeName.Smallint;
                        if (lexer.NextToken() == MySQLToken.PuncLeftParen)
                        {
                            lexer.NextToken();
                            length = exprParser.Expression();
                            Match(MySQLToken.PuncRightParen);
                        }
                        if (lexer.Token() == MySQLToken.KwUnsigned)
                        {
                            unsigned = true;
                            lexer.NextToken();
                        }
                        if (lexer.Token() == MySQLToken.KwZerofill)
                        {
                            zerofill = true;
                            lexer.NextToken();
                        }
                        goto typeName_break;
                    }

                case MySQLToken.KwMediumint:
                    {
                        // | MEDIUMINT[(length)] [UNSIGNED] [ZEROFILL]
                        typeName = DdlDatatype.DataTypeName.Mediumint;
                        if (lexer.NextToken() == MySQLToken.PuncLeftParen)
                        {
                            lexer.NextToken();
                            length = exprParser.Expression();
                            Match(MySQLToken.PuncRightParen);
                        }
                        if (lexer.Token() == MySQLToken.KwUnsigned)
                        {
                            unsigned = true;
                            lexer.NextToken();
                        }
                        if (lexer.Token() == MySQLToken.KwZerofill)
                        {
                            zerofill = true;
                            lexer.NextToken();
                        }
                        goto typeName_break;
                    }

                case MySQLToken.KwInteger:
                case MySQLToken.KwInt:
                    {
                        // | INT[(length)] [UNSIGNED] [ZEROFILL]
                        // | INTEGER[(length)] [UNSIGNED] [ZEROFILL]
                        typeName = DdlDatatype.DataTypeName.Int;
                        if (lexer.NextToken() == MySQLToken.PuncLeftParen)
                        {
                            lexer.NextToken();
                            length = exprParser.Expression();
                            Match(MySQLToken.PuncRightParen);
                        }
                        if (lexer.Token() == MySQLToken.KwUnsigned)
                        {
                            unsigned = true;
                            lexer.NextToken();
                        }
                        if (lexer.Token() == MySQLToken.KwZerofill)
                        {
                            zerofill = true;
                            lexer.NextToken();
                        }
                        goto typeName_break;
                    }

                case MySQLToken.KwBigint:
                    {
                        // | BIGINT[(length)] [UNSIGNED] [ZEROFILL]
                        typeName = DdlDatatype.DataTypeName.Bigint;
                        if (lexer.NextToken() == MySQLToken.PuncLeftParen)
                        {
                            lexer.NextToken();
                            length = exprParser.Expression();
                            Match(MySQLToken.PuncRightParen);
                        }
                        if (lexer.Token() == MySQLToken.KwUnsigned)
                        {
                            unsigned = true;
                            lexer.NextToken();
                        }
                        if (lexer.Token() == MySQLToken.KwZerofill)
                        {
                            zerofill = true;
                            lexer.NextToken();
                        }
                        goto typeName_break;
                    }

                case MySQLToken.KwReal:
                    {
                        // | REAL[(length,decimals)] [UNSIGNED] [ZEROFILL]
                        typeName = DdlDatatype.DataTypeName.Real;
                        if (lexer.NextToken() == MySQLToken.PuncLeftParen)
                        {
                            lexer.NextToken();
                            length = exprParser.Expression();
                            Match(MySQLToken.PuncComma);
                            decimals = exprParser.Expression();
                            Match(MySQLToken.PuncRightParen);
                        }
                        if (lexer.Token() == MySQLToken.KwUnsigned)
                        {
                            unsigned = true;
                            lexer.NextToken();
                        }
                        if (lexer.Token() == MySQLToken.KwZerofill)
                        {
                            zerofill = true;
                            lexer.NextToken();
                        }
                        goto typeName_break;
                    }

                case MySQLToken.KwDouble:
                    {
                        // | DOUBLE[(length,decimals)] [UNSIGNED] [ZEROFILL]
                        typeName = DdlDatatype.DataTypeName.Double;
                        if (lexer.NextToken() == MySQLToken.PuncLeftParen)
                        {
                            lexer.NextToken();
                            length = exprParser.Expression();
                            Match(MySQLToken.PuncComma);
                            decimals = exprParser.Expression();
                            Match(MySQLToken.PuncRightParen);
                        }
                        if (lexer.Token() == MySQLToken.KwUnsigned)
                        {
                            unsigned = true;
                            lexer.NextToken();
                        }
                        if (lexer.Token() == MySQLToken.KwZerofill)
                        {
                            zerofill = true;
                            lexer.NextToken();
                        }
                        goto typeName_break;
                    }

                case MySQLToken.KwFloat:
                    {
                        // | FLOAT[(length,decimals)] [UNSIGNED] [ZEROFILL]
                        typeName = DdlDatatype.DataTypeName.Float;
                        if (lexer.NextToken() == MySQLToken.PuncLeftParen)
                        {
                            lexer.NextToken();
                            length = exprParser.Expression();
                            Match(MySQLToken.PuncComma);
                            decimals = exprParser.Expression();
                            Match(MySQLToken.PuncRightParen);
                        }
                        if (lexer.Token() == MySQLToken.KwUnsigned)
                        {
                            unsigned = true;
                            lexer.NextToken();
                        }
                        if (lexer.Token() == MySQLToken.KwZerofill)
                        {
                            zerofill = true;
                            lexer.NextToken();
                        }
                        goto typeName_break;
                    }

                case MySQLToken.KwNumeric:
                case MySQLToken.KwDecimal:
                case MySQLToken.KwDec:
                    {
                        // | DECIMAL[(length[,decimals])] [UNSIGNED] [ZEROFILL]
                        // | NUMERIC[(length[,decimals])] [UNSIGNED] [ZEROFILL]
                        typeName = DdlDatatype.DataTypeName.Decimal;
                        if (lexer.NextToken() == MySQLToken.PuncLeftParen)
                        {
                            lexer.NextToken();
                            length = exprParser.Expression();
                            if (lexer.Token() == MySQLToken.PuncComma)
                            {
                                Match(MySQLToken.PuncComma);
                                decimals = exprParser.Expression();
                            }
                            Match(MySQLToken.PuncRightParen);
                        }
                        if (lexer.Token() == MySQLToken.KwUnsigned)
                        {
                            unsigned = true;
                            lexer.NextToken();
                        }
                        if (lexer.Token() == MySQLToken.KwZerofill)
                        {
                            zerofill = true;
                            lexer.NextToken();
                        }
                        goto typeName_break;
                    }

                case MySQLToken.KwChar:
                    {
                        // | CHAR[(length)] [CHARACTER SET charset_name] [COLLATE
                        // collation_name]
                        typeName = DdlDatatype.DataTypeName.Char;
                        if (lexer.NextToken() == MySQLToken.PuncLeftParen)
                        {
                            lexer.NextToken();
                            length = exprParser.Expression();
                            Match(MySQLToken.PuncRightParen);
                        }
                        if (lexer.Token() == MySQLToken.KwCharacter)
                        {
                            lexer.NextToken();
                            Match(MySQLToken.KwSet);
                            charSet = Identifier();
                        }
                        if (lexer.Token() == MySQLToken.KwCollate)
                        {
                            lexer.NextToken();
                            collation = Identifier();
                        }
                        goto typeName_break;
                    }

                case MySQLToken.KwVarchar:
                    {
                        // | VARCHAR(length) [CHARACTER SET charset_name] [COLLATE
                        // collation_name]
                        typeName = DdlDatatype.DataTypeName.Varchar;
                        lexer.NextToken();
                        Match(MySQLToken.PuncLeftParen);
                        length = exprParser.Expression();
                        Match(MySQLToken.PuncRightParen);
                        if (lexer.Token() == MySQLToken.KwCharacter)
                        {
                            lexer.NextToken();
                            Match(MySQLToken.KwSet);
                            charSet = Identifier();
                        }
                        if (lexer.Token() == MySQLToken.KwCollate)
                        {
                            lexer.NextToken();
                            collation = Identifier();
                        }
                        goto typeName_break;
                    }

                case MySQLToken.KwBinary:
                    {
                        // | BINARY[(length)]
                        typeName = DdlDatatype.DataTypeName.Binary;
                        if (lexer.NextToken() == MySQLToken.PuncLeftParen)
                        {
                            lexer.NextToken();
                            length = exprParser.Expression();
                            Match(MySQLToken.PuncRightParen);
                        }
                        goto typeName_break;
                    }

                case MySQLToken.KwVarbinary:
                    {
                        // | VARBINARY(length)
                        typeName = DdlDatatype.DataTypeName.Varbinary;
                        lexer.NextToken();
                        Match(MySQLToken.PuncLeftParen);
                        length = exprParser.Expression();
                        Match(MySQLToken.PuncRightParen);
                        goto typeName_break;
                    }

                case MySQLToken.KwTinyblob:
                    {
                        typeName = DdlDatatype.DataTypeName.Tinyblob;
                        lexer.NextToken();
                        goto typeName_break;
                    }

                case MySQLToken.KwBlob:
                    {
                        typeName = DdlDatatype.DataTypeName.Blob;
                        lexer.NextToken();
                        goto typeName_break;
                    }

                case MySQLToken.KwMediumblob:
                    {
                        typeName = DdlDatatype.DataTypeName.Mediumblob;
                        lexer.NextToken();
                        goto typeName_break;
                    }

                case MySQLToken.KwLongblob:
                    {
                        typeName = DdlDatatype.DataTypeName.Longblob;
                        lexer.NextToken();
                        goto typeName_break;
                    }

                case MySQLToken.KwTinytext:
                    {
                        // | TINYTEXT [BINARY] [CHARACTER SET charset_name] [COLLATE
                        // collation_name]
                        typeName = DdlDatatype.DataTypeName.Tinytext;
                        if (lexer.NextToken() == MySQLToken.KwBinary)
                        {
                            lexer.NextToken();
                            binary = true;
                        }
                        if (lexer.Token() == MySQLToken.KwCharacter)
                        {
                            lexer.NextToken();
                            Match(MySQLToken.KwSet);
                            charSet = Identifier();
                        }
                        if (lexer.Token() == MySQLToken.KwCollate)
                        {
                            lexer.NextToken();
                            collation = Identifier();
                        }
                        goto typeName_break;
                    }

                case MySQLToken.KwMediumtext:
                    {
                        // | MEDIUMTEXT [BINARY] [CHARACTER SET charset_name] [COLLATE
                        // collation_name]
                        typeName = DdlDatatype.DataTypeName.Mediumtext;
                        if (lexer.NextToken() == MySQLToken.KwBinary)
                        {
                            lexer.NextToken();
                            binary = true;
                        }
                        if (lexer.Token() == MySQLToken.KwCharacter)
                        {
                            lexer.NextToken();
                            Match(MySQLToken.KwSet);
                            charSet = Identifier();
                        }
                        if (lexer.Token() == MySQLToken.KwCollate)
                        {
                            lexer.NextToken();
                            collation = Identifier();
                        }
                        goto typeName_break;
                    }

                case MySQLToken.KwLongtext:
                    {
                        // | LONGTEXT [BINARY] [CHARACTER SET charset_name] [COLLATE
                        // collation_name]
                        typeName = DdlDatatype.DataTypeName.Longtext;
                        if (lexer.NextToken() == MySQLToken.KwBinary)
                        {
                            lexer.NextToken();
                            binary = true;
                        }
                        if (lexer.Token() == MySQLToken.KwCharacter)
                        {
                            lexer.NextToken();
                            Match(MySQLToken.KwSet);
                            charSet = Identifier();
                        }
                        if (lexer.Token() == MySQLToken.KwCollate)
                        {
                            lexer.NextToken();
                            collation = Identifier();
                        }
                        goto typeName_break;
                    }

                case MySQLToken.KwSet:
                    {
                        // | SET(value1,value2,value3,...) [CHARACTER SET charset_name]
                        // [COLLATE collation_name]
                        typeName = DdlDatatype.DataTypeName.Set;
                        lexer.NextToken();
                        Match(MySQLToken.PuncLeftParen);
                        for (int i = 0; lexer.Token() != MySQLToken.PuncRightParen; ++i)
                        {
                            if (i > 0)
                            {
                                Match(MySQLToken.PuncComma);
                            }
                            else
                            {
                                collectionVals = new List<Tup.Cobar.Parser.Ast.Expression.Expression>(2);
                            }
                            collectionVals.Add(exprParser.Expression());
                        }
                        Match(MySQLToken.PuncRightParen);
                        if (lexer.Token() == MySQLToken.KwCharacter)
                        {
                            lexer.NextToken();
                            Match(MySQLToken.KwSet);
                            charSet = Identifier();
                        }
                        if (lexer.Token() == MySQLToken.KwCollate)
                        {
                            lexer.NextToken();
                            collation = Identifier();
                        }
                        goto typeName_break;
                    }

                case MySQLToken.Identifier:
                    {
                        var si = specialIdentifiers.GetValue(lexer.StringValueUppercase());
                        if (si != SpecialIdentifier.None)
                        {
                            switch (si)
                            {
                                case SpecialIdentifier.Bit:
                                    {
                                        // BIT[(length)]
                                        typeName = DdlDatatype.DataTypeName.Bit;
                                        if (lexer.NextToken() == MySQLToken.PuncLeftParen)
                                        {
                                            lexer.NextToken();
                                            length = exprParser.Expression();
                                            Match(MySQLToken.PuncRightParen);
                                        }
                                        goto typeName_break;
                                    }

                                case SpecialIdentifier.Date:
                                    {
                                        typeName = DdlDatatype.DataTypeName.Date;
                                        lexer.NextToken();
                                        goto typeName_break;
                                    }

                                case SpecialIdentifier.Time:
                                    {
                                        typeName = DdlDatatype.DataTypeName.Time;
                                        lexer.NextToken();
                                        goto typeName_break;
                                    }

                                case SpecialIdentifier.Timestamp:
                                    {
                                        typeName = DdlDatatype.DataTypeName.Timestamp;
                                        lexer.NextToken();
                                        goto typeName_break;
                                    }

                                case SpecialIdentifier.Datetime:
                                    {
                                        typeName = DdlDatatype.DataTypeName.Datetime;
                                        lexer.NextToken();
                                        goto typeName_break;
                                    }

                                case SpecialIdentifier.Year:
                                    {
                                        typeName = DdlDatatype.DataTypeName.Year;
                                        lexer.NextToken();
                                        goto typeName_break;
                                    }

                                case SpecialIdentifier.Text:
                                    {
                                        // | TEXT [BINARY] [CHARACTER SET charset_name] [COLLATE
                                        // collation_name]
                                        typeName = DdlDatatype.DataTypeName.Text;
                                        if (lexer.NextToken() == MySQLToken.KwBinary)
                                        {
                                            lexer.NextToken();
                                            binary = true;
                                        }
                                        if (lexer.Token() == MySQLToken.KwCharacter)
                                        {
                                            lexer.NextToken();
                                            Match(MySQLToken.KwSet);
                                            charSet = Identifier();
                                        }
                                        if (lexer.Token() == MySQLToken.KwCollate)
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
                                        typeName = DdlDatatype.DataTypeName.Enum;
                                        lexer.NextToken();
                                        Match(MySQLToken.PuncLeftParen);
                                        for (int i_1 = 0; lexer.Token() != MySQLToken.PuncRightParen; ++i_1)
                                        {
                                            if (i_1 > 0)
                                            {
                                                Match(MySQLToken.PuncComma);
                                            }
                                            else
                                            {
                                                collectionVals = new List<Tup.Cobar.Parser.Ast.Expression.Expression>(2);
                                            }
                                            collectionVals.Add(exprParser.Expression());
                                        }
                                        Match(MySQLToken.PuncRightParen);
                                        if (lexer.Token() == MySQLToken.KwCharacter)
                                        {
                                            lexer.NextToken();
                                            Match(MySQLToken.KwSet);
                                            charSet = Identifier();
                                        }
                                        if (lexer.Token() == MySQLToken.KwCollate)
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
        typeName_break:;
            return new DdlDatatype(typeName, unsigned
                , zerofill, binary, length, decimals, charSet, collation, collectionVals);
        }

        // column_definition:
        // data_type [NOT NULL | NULL] [DEFAULT default_value]
        // [AUTO_INCREMENT] [UNIQUE [KEY] | [PRIMARY] KEY]
        // [COMMENT 'string']
        // [COLUMN_FORMAT {FIXED|DYNAMIC|DEFAULT}]
        // [reference_definition]
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        private DdlColumnDefinition ColumnDefinition()
        {
            DdlDatatype dataType = DataType();
            bool notNull = false;
            Tup.Cobar.Parser.Ast.Expression.Expression defaultVal = null;
            bool autoIncrement = false;
            DdlColumnDefinition.SpecialIndex sindex = DdlColumnDefinition.SpecialIndex.None;
            DdlColumnDefinition.ColumnFormat format = DdlColumnDefinition.ColumnFormat.None;
            LiteralString comment = null;
            if (lexer.Token() == MySQLToken.KwNot)
            {
                lexer.NextToken();
                Match(MySQLToken.LiteralNull);
                notNull = true;
            }
            else
            {
                if (lexer.Token() == MySQLToken.LiteralNull)
                {
                    lexer.NextToken();
                }
            }
            if (lexer.Token() == MySQLToken.KwDefault)
            {
                lexer.NextToken();
                defaultVal = exprParser.Expression();
                if (!(defaultVal is Tup.Cobar.Parser.Ast.Expression.Primary.Literal.Literal))
                {
                    throw new SQLSyntaxErrorException("default value of column must be a literal: " +
                         defaultVal);
                }
            }
            if (lexer.Token() == MySQLToken.Identifier && "AUTO_INCREMENT".Equals(lexer.StringValueUppercase()))
            {
                lexer.NextToken();
                autoIncrement = true;
            }
            switch (lexer.Token())
            {
                case MySQLToken.KwUnique:
                    {
                        if (lexer.NextToken() == MySQLToken.KwKey)
                        {
                            lexer.NextToken();
                        }
                        sindex = DdlColumnDefinition.SpecialIndex.Unique;
                        break;
                    }

                case MySQLToken.KwPrimary:
                    {
                        lexer.NextToken();
                        goto case MySQLToken.KwKey;
                    }

                case MySQLToken.KwKey:
                    {
                        Match(MySQLToken.KwKey);
                        sindex = DdlColumnDefinition.SpecialIndex.Primary;
                        break;
                    }
            }
            if (lexer.Token() == MySQLToken.Identifier && "COMMENT".Equals(lexer.StringValueUppercase()))
            {
                lexer.NextToken();
                comment = (LiteralString)exprParser.Expression();
            }
            if (lexer.Token() == MySQLToken.Identifier && "COLUMN_FORMAT".Equals(lexer.StringValueUppercase()))
            {
                switch (lexer.NextToken())
                {
                    case MySQLToken.KwDefault:
                        {
                            lexer.NextToken();
                            format = DdlColumnDefinition.ColumnFormat.Default;
                            break;
                        }

                    case MySQLToken.Identifier:
                        {
                            var si = specialIdentifiers.GetValue(lexer.StringValueUppercase());
                            if (si != SpecialIdentifier.None)
                            {
                                switch (si)
                                {
                                    case SpecialIdentifier.Fixed:
                                        {
                                            lexer.NextToken();
                                            format = DdlColumnDefinition.ColumnFormat.Fixed;
                                            break;
                                        }

                                    case SpecialIdentifier.Dynamic:
                                        {
                                            lexer.NextToken();
                                            format = DdlColumnDefinition.ColumnFormat.Dynamic;
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

        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        private bool TableOptions(DdlTableOptions options)
        {
            bool matched = false;
            for (int i = 0; ; ++i)
            {
                bool comma = false;
                if (i > 0 && lexer.Token() == MySQLToken.PuncComma)
                {
                    lexer.NextToken();
                    comma = true;
                }
                if (!TableOption(options))
                {
                    if (comma)
                    {
                        lexer.AddCacheToke(MySQLToken.PuncComma);
                    }
                    break;
                }
                else
                {
                    matched = true;
                }
            }
            return matched;
        }

        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        private bool TableOption(DdlTableOptions options)
        {
            Identifier id = null;
            Tup.Cobar.Parser.Ast.Expression.Expression expr = null;
            switch (lexer.Token())
            {
                case MySQLToken.KwCharacter:
                    {
                        lexer.NextToken();
                        Match(MySQLToken.KwSet);
                        if (lexer.Token() == MySQLToken.OpEquals)
                        {
                            lexer.NextToken();
                        }
                        id = Identifier();
                        options.SetCharSet(id);
                        break;
                    }

                case MySQLToken.KwCollate:
                    {
                        lexer.NextToken();
                        if (lexer.Token() == MySQLToken.OpEquals)
                        {
                            lexer.NextToken();
                        }
                        id = Identifier();
                        options.SetCollation(id);
                        break;
                    }

                case MySQLToken.KwDefault:
                    {
                        switch (lexer.NextToken())
                        {
                            case MySQLToken.KwCharacter:
                                {
                                    // | [DEFAULT] CHARSET [=] charset_name { MySQL 5.1 legacy}
                                    // | [DEFAULT] CHARACTER SET [=] charset_name
                                    // | [DEFAULT] COLLATE [=] collation_name
                                    lexer.NextToken();
                                    Match(MySQLToken.KwSet);
                                    if (lexer.Token() == MySQLToken.OpEquals)
                                    {
                                        lexer.NextToken();
                                    }
                                    id = Identifier();
                                    options.SetCharSet(id);
                                    goto os_break;
                                }

                            case MySQLToken.KwCollate:
                                {
                                    lexer.NextToken();
                                    if (lexer.Token() == MySQLToken.OpEquals)
                                    {
                                        lexer.NextToken();
                                    }
                                    id = Identifier();
                                    options.SetCollation(id);
                                    goto os_break;
                                }

                            case MySQLToken.Identifier:
                                {
                                    var si = specialIdentifiers.GetValue(lexer.StringValueUppercase());
                                    if (si != SpecialIdentifier.None)
                                    {
                                        switch (si)
                                        {
                                            case SpecialIdentifier.Charset:
                                                {
                                                    lexer.NextToken();
                                                    if (lexer.Token() == MySQLToken.OpEquals)
                                                    {
                                                        lexer.NextToken();
                                                    }
                                                    id = Identifier();
                                                    options.SetCharSet(id);
                                                    goto os_break;
                                                }
                                        }
                                    }
                                    goto default;
                                }

                            default:
                                {
                                    lexer.AddCacheToke(MySQLToken.KwDefault);
                                    return false;
                                }
                        }
                        //goto case MySQLToken.KwIndex;
                    }

                case MySQLToken.KwIndex:
                    {
                        // | INDEX DIRECTORY [=] 'absolute path to directory'
                        lexer.NextToken();
                        if (lexer.Token() == MySQLToken.Identifier && "DIRECTORY".Equals(lexer.StringValueUppercase()))
                        {
                            if (lexer.NextToken() == MySQLToken.OpEquals)
                            {
                                lexer.NextToken();
                            }
                            options.SetIndexDir((LiteralString)exprParser.Expression());
                            break;
                        }
                        lexer.AddCacheToke(MySQLToken.KwIndex);
                        return true;
                    }

                case MySQLToken.KwUnion:
                    {
                        // | UNION [=] (tbl_name[,tbl_name]...)
                        if (lexer.NextToken() == MySQLToken.OpEquals)
                        {
                            lexer.NextToken();
                        }
                        Match(MySQLToken.PuncLeftParen);
                        IList<Identifier> union = new List<Identifier>(2);
                        for (int j = 0; lexer.Token() != MySQLToken.PuncRightParen; ++j)
                        {
                            if (j > 0)
                            {
                                Match(MySQLToken.PuncComma);
                            }
                            id = Identifier();
                            union.Add(id);
                        }
                        Match(MySQLToken.PuncRightParen);
                        options.SetUnion(union);
                        goto os_break;
                    }

                case MySQLToken.Identifier:
                    {
                        var si_1 = specialIdentifiers.GetValue(lexer.StringValueUppercase());
                        if (si_1 != SpecialIdentifier.None)
                        {
                            switch (si_1)
                            {
                                case SpecialIdentifier.Charset:
                                    {
                                        // CHARSET [=] charset_name
                                        lexer.NextToken();
                                        if (lexer.Token() == MySQLToken.OpEquals)
                                        {
                                            lexer.NextToken();
                                        }
                                        id = Identifier();
                                        options.SetCharSet(id);
                                        goto os_break;
                                    }

                                case SpecialIdentifier.Engine:
                                    {
                                        // ENGINE [=] engine_name
                                        if (lexer.NextToken() == MySQLToken.OpEquals)
                                        {
                                            lexer.NextToken();
                                        }
                                        id = Identifier();
                                        options.SetEngine(id);
                                        goto os_break;
                                    }

                                case SpecialIdentifier.AutoIncrement:
                                    {
                                        // | AUTO_INCREMENT [=] value
                                        if (lexer.NextToken() == MySQLToken.OpEquals)
                                        {
                                            lexer.NextToken();
                                        }
                                        expr = exprParser.Expression();
                                        options.SetAutoIncrement(expr);
                                        goto os_break;
                                    }

                                case SpecialIdentifier.AvgRowLength:
                                    {
                                        // | AVG_ROW_LENGTH [=] value
                                        if (lexer.NextToken() == MySQLToken.OpEquals)
                                        {
                                            lexer.NextToken();
                                        }
                                        expr = exprParser.Expression();
                                        options.SetAvgRowLength(expr);
                                        goto os_break;
                                    }

                                case SpecialIdentifier.Checksum:
                                    {
                                        // | CHECKSUM [=] {0 | 1}
                                        if (lexer.NextToken() == MySQLToken.OpEquals)
                                        {
                                            lexer.NextToken();
                                        }
                                        switch (lexer.Token())
                                        {
                                            case MySQLToken.LiteralBoolFalse:
                                                {
                                                    lexer.NextToken();
                                                    options.SetCheckSum(false);
                                                    goto case MySQLToken.LiteralBoolTrue;
                                                }

                                            case MySQLToken.LiteralBoolTrue:
                                                {
                                                    lexer.NextToken();
                                                    options.SetCheckSum(true);
                                                    break;
                                                }

                                            case MySQLToken.LiteralNumPureDigit:
                                                {
                                                    int intVal = (int)lexer.IntegerValue();
                                                    lexer.NextToken();
                                                    if (intVal == 0)
                                                    {
                                                        options.SetCheckSum(false);
                                                    }
                                                    else
                                                    {
                                                        options.SetCheckSum(true);
                                                    }
                                                    break;
                                                }

                                            default:
                                                {
                                                    throw new SQLSyntaxErrorException("table option of CHECKSUM error");
                                                }
                                        }
                                        goto os_break;
                                    }

                                case SpecialIdentifier.DelayKeyWrite:
                                    {
                                        // | DELAY_KEY_WRITE [=] {0 | 1}
                                        if (lexer.NextToken() == MySQLToken.OpEquals)
                                        {
                                            lexer.NextToken();
                                        }
                                        switch (lexer.Token())
                                        {
                                            case MySQLToken.LiteralBoolFalse:
                                                {
                                                    lexer.NextToken();
                                                    options.SetDelayKeyWrite(false);
                                                    goto case MySQLToken.LiteralBoolTrue;
                                                }

                                            case MySQLToken.LiteralBoolTrue:
                                                {
                                                    lexer.NextToken();
                                                    options.SetDelayKeyWrite(true);
                                                    break;
                                                }

                                            case MySQLToken.LiteralNumPureDigit:
                                                {
                                                    int intVal_1 = (int)lexer.IntegerValue();
                                                    lexer.NextToken();
                                                    if (intVal_1 == 0)
                                                    {
                                                        options.SetDelayKeyWrite(false);
                                                    }
                                                    else
                                                    {
                                                        options.SetDelayKeyWrite(true);
                                                    }
                                                    break;
                                                }

                                            default:
                                                {
                                                    throw new SQLSyntaxErrorException("table option of DELAY_KEY_WRITE error");
                                                }
                                        }
                                        goto os_break;
                                    }

                                case SpecialIdentifier.Comment:
                                    {
                                        // | COMMENT [=] 'string'
                                        if (lexer.NextToken() == MySQLToken.OpEquals)
                                        {
                                            lexer.NextToken();
                                        }
                                        options.SetComment((LiteralString)exprParser.Expression());
                                        goto os_break;
                                    }

                                case SpecialIdentifier.Connection:
                                    {
                                        // | CONNECTION [=] 'connect_string'
                                        if (lexer.NextToken() == MySQLToken.OpEquals)
                                        {
                                            lexer.NextToken();
                                        }
                                        options.SetConnection((LiteralString)exprParser.Expression());
                                        goto os_break;
                                    }

                                case SpecialIdentifier.Data:
                                    {
                                        // | DATA DIRECTORY [=] 'absolute path to directory'
                                        lexer.NextToken();
                                        MatchIdentifier("DIRECTORY");
                                        if (lexer.Token() == MySQLToken.OpEquals)
                                        {
                                            lexer.NextToken();
                                        }
                                        options.SetDataDir((LiteralString)exprParser.Expression());
                                        goto os_break;
                                    }

                                case SpecialIdentifier.InsertMethod:
                                    {
                                        // | INSERT_METHOD [=] { NO | FIRST | LAST }
                                        if (lexer.NextToken() == MySQLToken.OpEquals)
                                        {
                                            lexer.NextToken();
                                        }
                                        switch (MatchIdentifier("NO", "FIRST", "LAST"))
                                        {
                                            case 0:
                                                {
                                                    options.SetInsertMethod(DdlTableOptions.InsertMethod.No);
                                                    break;
                                                }

                                            case 1:
                                                {
                                                    options.SetInsertMethod(DdlTableOptions.InsertMethod.First);
                                                    break;
                                                }

                                            case 2:
                                                {
                                                    options.SetInsertMethod(DdlTableOptions.InsertMethod.Last);
                                                    break;
                                                }
                                        }
                                        goto os_break;
                                    }

                                case SpecialIdentifier.KeyBlockSize:
                                    {
                                        // | KEY_BLOCK_SIZE [=] value
                                        if (lexer.NextToken() == MySQLToken.OpEquals)
                                        {
                                            lexer.NextToken();
                                        }
                                        options.SetKeyBlockSize(exprParser.Expression());
                                        goto os_break;
                                    }

                                case SpecialIdentifier.MaxRows:
                                    {
                                        // | MAX_ROWS [=] value
                                        if (lexer.NextToken() == MySQLToken.OpEquals)
                                        {
                                            lexer.NextToken();
                                        }
                                        options.SetMaxRows(exprParser.Expression());
                                        goto os_break;
                                    }

                                case SpecialIdentifier.MinRows:
                                    {
                                        // | MIN_ROWS [=] value
                                        if (lexer.NextToken() == MySQLToken.OpEquals)
                                        {
                                            lexer.NextToken();
                                        }
                                        options.SetMinRows(exprParser.Expression());
                                        goto os_break;
                                    }

                                case SpecialIdentifier.PackKeys:
                                    {
                                        // | PACK_KEYS [=] {0 | 1 | DEFAULT}
                                        if (lexer.NextToken() == MySQLToken.OpEquals)
                                        {
                                            lexer.NextToken();
                                        }
                                        switch (lexer.Token())
                                        {
                                            case MySQLToken.LiteralBoolFalse:
                                                {
                                                    lexer.NextToken();
                                                    options.SetPackKeys(DdlTableOptions.PackKeys.False);
                                                    break;
                                                }

                                            case MySQLToken.LiteralBoolTrue:
                                                {
                                                    lexer.NextToken();
                                                    options.SetPackKeys(DdlTableOptions.PackKeys.True);
                                                    break;
                                                }

                                            case MySQLToken.LiteralNumPureDigit:
                                                {
                                                    int intVal_2 = (int)lexer.IntegerValue();
                                                    lexer.NextToken();
                                                    if (intVal_2 == 0)
                                                    {
                                                        options.SetPackKeys(DdlTableOptions.PackKeys.False);
                                                    }
                                                    else
                                                    {
                                                        options.SetPackKeys(DdlTableOptions.PackKeys.True);
                                                    }
                                                    break;
                                                }

                                            case MySQLToken.KwDefault:
                                                {
                                                    lexer.NextToken();
                                                    options.SetPackKeys(DdlTableOptions.PackKeys.Default);
                                                    break;
                                                }

                                            default:
                                                {
                                                    throw new SQLSyntaxErrorException("table option of PACK_KEYS error");
                                                }
                                        }
                                        goto os_break;
                                    }

                                case SpecialIdentifier.Password:
                                    {
                                        // | PASSWORD [=] 'string'
                                        if (lexer.NextToken() == MySQLToken.OpEquals)
                                        {
                                            lexer.NextToken();
                                        }
                                        options.SetPassword((LiteralString)exprParser.Expression());
                                        goto os_break;
                                    }

                                case SpecialIdentifier.RowFormat:
                                    {
                                        // | ROW_FORMAT [=]
                                        // {DEFAULT|DYNAMIC|FIXED|COMPRESSED|REDUNDANT|COMPACT}
                                        if (lexer.NextToken() == MySQLToken.OpEquals)
                                        {
                                            lexer.NextToken();
                                        }
                                        switch (lexer.Token())
                                        {
                                            case MySQLToken.KwDefault:
                                                {
                                                    lexer.NextToken();
                                                    options.SetRowFormat(DdlTableOptions.RowFormat.Default);
                                                    goto os_break;
                                                }

                                            case MySQLToken.Identifier:
                                                {
                                                    var sid = specialIdentifiers.GetValue(lexer.StringValueUppercase());
                                                    if (sid != SpecialIdentifier.None)
                                                    {
                                                        switch (sid)
                                                        {
                                                            case SpecialIdentifier.Dynamic:
                                                                {
                                                                    lexer.NextToken();
                                                                    options.SetRowFormat(DdlTableOptions.RowFormat.Dynamic);
                                                                    goto os_break;
                                                                }

                                                            case SpecialIdentifier.Fixed:
                                                                {
                                                                    lexer.NextToken();
                                                                    options.SetRowFormat(DdlTableOptions.RowFormat.Fixed);
                                                                    goto os_break;
                                                                }

                                                            case SpecialIdentifier.Compressed:
                                                                {
                                                                    lexer.NextToken();
                                                                    options.SetRowFormat(DdlTableOptions.RowFormat.Compressed);
                                                                    goto os_break;
                                                                }

                                                            case SpecialIdentifier.Redundant:
                                                                {
                                                                    lexer.NextToken();
                                                                    options.SetRowFormat(DdlTableOptions.RowFormat.Redundant);
                                                                    goto os_break;
                                                                }

                                                            case SpecialIdentifier.Compact:
                                                                {
                                                                    lexer.NextToken();
                                                                    options.SetRowFormat(DdlTableOptions.RowFormat.Compact);
                                                                    goto os_break;
                                                                }
                                                        }
                                                    }
                                                    goto default;
                                                }

                                            default:
                                                {
                                                    throw new SQLSyntaxErrorException("table option of ROW_FORMAT error");
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
        os_break:;
            return true;
        }
    }
}