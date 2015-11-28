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
using Tup.Cobar4Net.Parser.Ast.Expression.Primary;
using Tup.Cobar4Net.Parser.Ast.Expression.Primary.Literal;
using Tup.Cobar4Net.Parser.Ast.Fragment;
using Tup.Cobar4Net.Parser.Ast.Stmt;
using Tup.Cobar4Net.Parser.Ast.Stmt.Dal;
using Tup.Cobar4Net.Parser.Ast.Stmt.Ddl;
using Tup.Cobar4Net.Parser.Ast.Stmt.Mts;
using Tup.Cobar4Net.Parser.Recognizer.Mysql.Lexer;
using Tup.Cobar4Net.Parser.Util;
using DalShowColumns = Tup.Cobar4Net.Parser.Ast.Stmt.Dal.ShowColumns;
using DalShowIndex = Tup.Cobar4Net.Parser.Ast.Stmt.Dal.ShowIndex;
using DalShowProfile = Tup.Cobar4Net.Parser.Ast.Stmt.Dal.ShowProfile;
using Expr = Tup.Cobar4Net.Parser.Ast.Expression.Expression;

namespace Tup.Cobar4Net.Parser.Recognizer.Mysql.Syntax
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class MySQLDALParser : MySQLParser
    {
        protected MySQLExprParser exprParser;

        public MySQLDALParser(MySQLLexer lexer, MySQLExprParser exprParser)
            : base(lexer)
        {
            this.exprParser = exprParser;
        }

        private enum SpecialIdentifier
        {
            None = 0,

            Authors,
            Binlog,
            Block,
            Code,
            Collation,
            Columns,
            Committed,
            Context,
            Contributors,
            Count,
            Cpu,
            Engine,
            Engines,
            Errors,
            Event,
            Events,
            Full,
            Function,
            Global,
            Grants,
            Hosts,
            Indexes,
            Innodb,
            Ipc,
            Local,
            Master,
            Memory,
            Mutex,
            Names,
            Open,
            Page,
            PerformanceSchema,
            Plugins,
            Privileges,
            Processlist,
            Profile,
            Profiles,
            Repeatable,
            Serializable,
            Session,
            Slave,
            Source,
            Status,
            Storage,
            Swaps,
            Tables,
            Transaction,
            Triggers,
            Uncommitted,
            Variables,
            View,
            Warnings
        }

        private static readonly IDictionary<string, SpecialIdentifier> specialIdentifiers
             = new Dictionary<string, SpecialIdentifier>();

        static MySQLDALParser()
        {
            specialIdentifiers["AUTHORS"] = SpecialIdentifier.Authors;
            specialIdentifiers["BINLOG"] = SpecialIdentifier.Binlog;
            specialIdentifiers["COLLATION"] = SpecialIdentifier.Collation;
            specialIdentifiers["COLUMNS"] = SpecialIdentifier.Columns;
            specialIdentifiers["CONTRIBUTORS"] = SpecialIdentifier.Contributors;
            specialIdentifiers["EVENT"] = SpecialIdentifier.Event;
            specialIdentifiers["FUNCTION"] = SpecialIdentifier.Function;
            specialIdentifiers["VIEW"] = SpecialIdentifier.View;
            specialIdentifiers["ENGINE"] = SpecialIdentifier.Engine;
            specialIdentifiers["ENGINES"] = SpecialIdentifier.Engines;
            specialIdentifiers["ERRORS"] = SpecialIdentifier.Errors;
            specialIdentifiers["EVENTS"] = SpecialIdentifier.Events;
            specialIdentifiers["FULL"] = SpecialIdentifier.Full;
            specialIdentifiers["GLOBAL"] = SpecialIdentifier.Global;
            specialIdentifiers["GRANTS"] = SpecialIdentifier.Grants;
            specialIdentifiers["MASTER"] = SpecialIdentifier.Master;
            specialIdentifiers["OPEN"] = SpecialIdentifier.Open;
            specialIdentifiers["PLUGINS"] = SpecialIdentifier.Plugins;
            specialIdentifiers["CODE"] = SpecialIdentifier.Code;
            specialIdentifiers["STATUS"] = SpecialIdentifier.Status;
            specialIdentifiers["PRIVILEGES"] = SpecialIdentifier.Privileges;
            specialIdentifiers["PROCESSLIST"] = SpecialIdentifier.Processlist;
            specialIdentifiers["PROFILE"] = SpecialIdentifier.Profile;
            specialIdentifiers["PROFILES"] = SpecialIdentifier.Profiles;
            specialIdentifiers["SESSION"] = SpecialIdentifier.Session;
            specialIdentifiers["SLAVE"] = SpecialIdentifier.Slave;
            specialIdentifiers["STORAGE"] = SpecialIdentifier.Storage;
            specialIdentifiers["TABLES"] = SpecialIdentifier.Tables;
            specialIdentifiers["TRIGGERS"] = SpecialIdentifier.Triggers;
            specialIdentifiers["VARIABLES"] = SpecialIdentifier.Variables;
            specialIdentifiers["WARNINGS"] = SpecialIdentifier.Warnings;
            specialIdentifiers["INNODB"] = SpecialIdentifier.Innodb;
            specialIdentifiers["PERFORMANCE_SCHEMA"] = SpecialIdentifier.PerformanceSchema;
            specialIdentifiers["MUTEX"] = SpecialIdentifier.Mutex;
            specialIdentifiers["COUNT"] = SpecialIdentifier.Count;
            specialIdentifiers["BLOCK"] = SpecialIdentifier.Block;
            specialIdentifiers["CONTEXT"] = SpecialIdentifier.Context;
            specialIdentifiers["CPU"] = SpecialIdentifier.Cpu;
            specialIdentifiers["MEMORY"] = SpecialIdentifier.Memory;
            specialIdentifiers["PAGE"] = SpecialIdentifier.Page;
            specialIdentifiers["SOURCE"] = SpecialIdentifier.Source;
            specialIdentifiers["SWAPS"] = SpecialIdentifier.Swaps;
            specialIdentifiers["IPC"] = SpecialIdentifier.Ipc;
            specialIdentifiers["LOCAL"] = SpecialIdentifier.Local;
            specialIdentifiers["HOSTS"] = SpecialIdentifier.Hosts;
            specialIdentifiers["INDEXES"] = SpecialIdentifier.Indexes;
            specialIdentifiers["TRANSACTION"] = SpecialIdentifier.Transaction;
            specialIdentifiers["UNCOMMITTED"] = SpecialIdentifier.Uncommitted;
            specialIdentifiers["COMMITTED"] = SpecialIdentifier.Committed;
            specialIdentifiers["REPEATABLE"] = SpecialIdentifier.Repeatable;
            specialIdentifiers["SERIALIZABLE"] = SpecialIdentifier.Serializable;
            specialIdentifiers["NAMES"] = SpecialIdentifier.Names;
        }

        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        public virtual DescTableStatement Desc()
        {
            Match(MySQLToken.KwDesc, MySQLToken.KwDescribe);
            Identifier table = Identifier();
            return new DescTableStatement(table);
        }

        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        public virtual DALShowStatement Show()
        {
            Match(MySQLToken.KwShow);
            string tempStr;
            string tempStrUp;
            Expr tempExpr;
            Identifier tempId;
            SpecialIdentifier tempSi;
            Limit tempLimit;
            switch (lexer.Token())
            {
                case MySQLToken.KwBinary:
                    {
                        lexer.NextToken();
                        MatchIdentifier("LOGS");
                        return new ShowBinaryLog();
                    }

                case MySQLToken.KwCharacter:
                    {
                        lexer.NextToken();
                        Match(MySQLToken.KwSet);
                        switch (lexer.Token())
                        {
                            case MySQLToken.KwLike:
                                {
                                    tempStr = Like();
                                    return new ShowCharaterSet(tempStr);
                                }

                            case MySQLToken.KwWhere:
                                {
                                    tempExpr = Where();
                                    return new ShowCharaterSet(tempExpr);
                                }

                            default:
                                {
                                    return new ShowCharaterSet();
                                }
                        }
                        //goto case MySQLToken.KwCreate;
                    }

                case MySQLToken.KwCreate:
                    {
                        ShowCreate.CreateType showCreateType;
                        switch (lexer.NextToken())
                        {
                            case MySQLToken.KwDatabase:
                                {
                                    showCreateType = ShowCreate.CreateType.Database;
                                    break;
                                }

                            case MySQLToken.KwProcedure:
                                {
                                    showCreateType = ShowCreate.CreateType.Procedure;
                                    break;
                                }

                            case MySQLToken.KwTable:
                                {
                                    showCreateType = ShowCreate.CreateType.Table;
                                    break;
                                }

                            case MySQLToken.KwTrigger:
                                {
                                    showCreateType = ShowCreate.CreateType.Trigger;
                                    break;
                                }

                            case MySQLToken.Identifier:
                                {
                                    tempSi = specialIdentifiers.GetValue(lexer.StringValueUppercase());
                                    if (tempSi != SpecialIdentifier.None)
                                    {
                                        switch (tempSi)
                                        {
                                            case SpecialIdentifier.Event:
                                                {
                                                    showCreateType = ShowCreate.CreateType.Event;
                                                    goto switch1_break;
                                                }

                                            case SpecialIdentifier.Function:
                                                {
                                                    showCreateType = ShowCreate.CreateType.Function;
                                                    goto switch1_break;
                                                }

                                            case SpecialIdentifier.View:
                                                {
                                                    showCreateType = ShowCreate.CreateType.View;
                                                    goto switch1_break;
                                                }
                                        }
                                    }
                                    goto default;
                                }

                            default:
                                {
                                    throw Err("unexpect token for SHOW CREATE");
                                }
                        }
                    switch1_break:;
                        lexer.NextToken();
                        tempId = Identifier();
                        return new ShowCreate(showCreateType, tempId);
                    }

                case MySQLToken.KwSchemas:
                case MySQLToken.KwDatabases:
                    {
                        lexer.NextToken();
                        switch (lexer.Token())
                        {
                            case MySQLToken.KwLike:
                                {
                                    tempStr = Like();
                                    return new ShowDatabases(tempStr);
                                }

                            case MySQLToken.KwWhere:
                                {
                                    tempExpr = Where();
                                    return new ShowDatabases(tempExpr);
                                }
                        }
                        return new ShowDatabases();
                    }

                case MySQLToken.KwKeys:
                    {
                        return ShowIndex(DalShowIndex.Type.Keys);
                    }

                case MySQLToken.KwIndex:
                    {
                        return ShowIndex(DalShowIndex.Type.Index);
                    }

                case MySQLToken.KwProcedure:
                    {
                        lexer.NextToken();
                        tempStrUp = lexer.StringValueUppercase();
                        tempSi = specialIdentifiers.GetValue(tempStrUp);
                        if (tempSi != SpecialIdentifier.None)
                        {
                            switch (tempSi)
                            {
                                case SpecialIdentifier.Code:
                                    {
                                        lexer.NextToken();
                                        tempId = Identifier();
                                        return new ShowProcedureCode(tempId);
                                    }

                                case SpecialIdentifier.Status:
                                    {
                                        switch (lexer.NextToken())
                                        {
                                            case MySQLToken.KwLike:
                                                {
                                                    tempStr = Like();
                                                    return new ShowProcedureStatus(tempStr);
                                                }

                                            case MySQLToken.KwWhere:
                                                {
                                                    tempExpr = Where();
                                                    return new ShowProcedureStatus(tempExpr);
                                                }

                                            default:
                                                {
                                                    return new ShowProcedureStatus();
                                                }
                                        }
                                        //break;
                                    }
                            }
                        }
                        throw Err("unexpect token for SHOW PROCEDURE");
                    }

                case MySQLToken.KwTable:
                    {
                        lexer.NextToken();
                        MatchIdentifier("STATUS");
                        tempId = null;
                        if (lexer.Token() == MySQLToken.KwFrom || lexer.Token() == MySQLToken.KwIn)
                        {
                            lexer.NextToken();
                            tempId = Identifier();
                        }
                        switch (lexer.Token())
                        {
                            case MySQLToken.KwLike:
                                {
                                    tempStr = Like();
                                    return new ShowTableStatus(tempId, tempStr);
                                }

                            case MySQLToken.KwWhere:
                                {
                                    tempExpr = Where();
                                    return new ShowTableStatus(tempId, tempExpr);
                                }
                        }
                        return new ShowTableStatus(tempId);
                    }

                case MySQLToken.Identifier:
                    {
                        tempStrUp = lexer.StringValueUppercase();
                        tempSi = specialIdentifiers.GetValue(tempStrUp);
                        if (tempSi == SpecialIdentifier.None)
                        {
                            break;
                        }
                        switch (tempSi)
                        {
                            case SpecialIdentifier.Indexes:
                                {
                                    return ShowIndex(DalShowIndex.Type.Indexes);
                                }

                            case SpecialIdentifier.Grants:
                                {
                                    if (lexer.NextToken() == MySQLToken.KwFor)
                                    {
                                        lexer.NextToken();
                                        tempExpr = exprParser.Expression();
                                        return new ShowGrants(tempExpr);
                                    }
                                    return new ShowGrants();
                                }

                            case SpecialIdentifier.Authors:
                                {
                                    lexer.NextToken();
                                    return new ShowAuthors();
                                }

                            case SpecialIdentifier.Binlog:
                                {
                                    lexer.NextToken();
                                    MatchIdentifier("EVENTS");
                                    tempStr = null;
                                    tempExpr = null;
                                    tempLimit = null;
                                    if (lexer.Token() == MySQLToken.KwIn)
                                    {
                                        lexer.NextToken();
                                        tempStr = lexer.StringValue();
                                        lexer.NextToken();
                                    }
                                    if (lexer.Token() == MySQLToken.KwFrom)
                                    {
                                        lexer.NextToken();
                                        tempExpr = exprParser.Expression();
                                    }
                                    if (lexer.Token() == MySQLToken.KwLimit)
                                    {
                                        tempLimit = Limit();
                                    }
                                    return new ShowBinLogEvent(tempStr, tempExpr, tempLimit);
                                }

                            case SpecialIdentifier.Collation:
                                {
                                    switch (lexer.NextToken())
                                    {
                                        case MySQLToken.KwLike:
                                            {
                                                tempStr = Like();
                                                return new ShowCollation(tempStr);
                                            }

                                        case MySQLToken.KwWhere:
                                            {
                                                tempExpr = Where();
                                                return new ShowCollation(tempExpr);
                                            }
                                    }
                                    return new ShowCollation();
                                }

                            case SpecialIdentifier.Columns:
                                {
                                    return ShowColumns(false);
                                }

                            case SpecialIdentifier.Contributors:
                                {
                                    lexer.NextToken();
                                    return new ShowContributors();
                                }

                            case SpecialIdentifier.Engine:
                                {
                                    switch (lexer.NextToken())
                                    {
                                        case MySQLToken.Identifier:
                                            {
                                                tempStrUp = lexer.StringValueUppercase();
                                                tempSi = specialIdentifiers.GetValue(tempStrUp);
                                                if (tempSi != SpecialIdentifier.None)
                                                {
                                                    switch (tempSi)
                                                    {
                                                        case SpecialIdentifier.Innodb:
                                                            {
                                                                lexer.NextToken();
                                                                tempStrUp = lexer.StringValueUppercase();
                                                                tempSi = specialIdentifiers.GetValue(tempStrUp);
                                                                if (tempSi != SpecialIdentifier.None)
                                                                {
                                                                    switch (tempSi)
                                                                    {
                                                                        case SpecialIdentifier.Status:
                                                                            {
                                                                                lexer.NextToken();
                                                                                return new ShowEngine(ShowEngine.EngineType.InnodbStatus);
                                                                            }

                                                                        case SpecialIdentifier.Mutex:
                                                                            {
                                                                                lexer.NextToken();
                                                                                return new ShowEngine(ShowEngine.EngineType.InnodbMutex);
                                                                            }
                                                                    }
                                                                }
                                                                goto case SpecialIdentifier.PerformanceSchema;
                                                            }

                                                        case SpecialIdentifier.PerformanceSchema:
                                                            {
                                                                lexer.NextToken();
                                                                MatchIdentifier("STATUS");
                                                                return new ShowEngine(ShowEngine.EngineType.PerformanceSchemaStatus);
                                                            }
                                                    }
                                                }
                                                goto default;
                                            }

                                        default:
                                            {
                                                throw Err("unexpect token for SHOW ENGINE");
                                            }
                                    }
                                    //goto case SpecialIdentifier.Engines;
                                }

                            case SpecialIdentifier.Engines:
                                {
                                    lexer.NextToken();
                                    return new ShowEngines();
                                }

                            case SpecialIdentifier.Errors:
                                {
                                    lexer.NextToken();
                                    tempLimit = Limit();
                                    return new ShowErrors(false, tempLimit);
                                }

                            case SpecialIdentifier.Count:
                                {
                                    lexer.NextToken();
                                    Match(MySQLToken.PuncLeftParen);
                                    Match(MySQLToken.OpAsterisk);
                                    Match(MySQLToken.PuncRightParen);
                                    switch (MatchIdentifier("ERRORS", "WARNINGS"))
                                    {
                                        case 0:
                                            {
                                                return new ShowErrors(true, null);
                                            }

                                        case 1:
                                            {
                                                return new ShowWarnings(true, null);
                                            }
                                    }
                                    goto case SpecialIdentifier.Events;
                                }

                            case SpecialIdentifier.Events:
                                {
                                    tempId = null;
                                    switch (lexer.NextToken())
                                    {
                                        case MySQLToken.KwIn:
                                        case MySQLToken.KwFrom:
                                            {
                                                lexer.NextToken();
                                                tempId = Identifier();
                                                break;
                                            }
                                    }
                                    switch (lexer.Token())
                                    {
                                        case MySQLToken.KwLike:
                                            {
                                                tempStr = Like();
                                                return new ShowEvents(tempId, tempStr);
                                            }

                                        case MySQLToken.KwWhere:
                                            {
                                                tempExpr = Where();
                                                return new ShowEvents(tempId, tempExpr);
                                            }

                                        default:
                                            {
                                                return new ShowEvents(tempId);
                                            }
                                    }
                                    //goto case SpecialIdentifier.Full;
                                }

                            case SpecialIdentifier.Full:
                                {
                                    lexer.NextToken();
                                    tempStrUp = lexer.StringValueUppercase();
                                    tempSi = specialIdentifiers.GetValue(tempStrUp);
                                    if (tempSi != SpecialIdentifier.None)
                                    {
                                        switch (tempSi)
                                        {
                                            case SpecialIdentifier.Columns:
                                                {
                                                    return ShowColumns(true);
                                                }

                                            case SpecialIdentifier.Processlist:
                                                {
                                                    lexer.NextToken();
                                                    return new ShowProcesslist(true);
                                                }

                                            case SpecialIdentifier.Tables:
                                                {
                                                    tempId = null;
                                                    switch (lexer.NextToken())
                                                    {
                                                        case MySQLToken.KwIn:
                                                        case MySQLToken.KwFrom:
                                                            {
                                                                lexer.NextToken();
                                                                tempId = Identifier();
                                                                break;
                                                            }
                                                    }
                                                    switch (lexer.Token())
                                                    {
                                                        case MySQLToken.KwLike:
                                                            {
                                                                tempStr = Like();
                                                                return new ShowTables(true, tempId, tempStr);
                                                            }

                                                        case MySQLToken.KwWhere:
                                                            {
                                                                tempExpr = Where();
                                                                return new ShowTables(true, tempId, tempExpr);
                                                            }

                                                        default:
                                                            {
                                                                return new ShowTables(true, tempId);
                                                            }
                                                    }
                                                    //break;
                                                }
                                        }
                                    }
                                    throw Err("unexpected token for SHOW FULL");
                                }

                            case SpecialIdentifier.Function:
                                {
                                    lexer.NextToken();
                                    tempStrUp = lexer.StringValueUppercase();
                                    tempSi = specialIdentifiers.GetValue(tempStrUp);
                                    if (tempSi != SpecialIdentifier.None)
                                    {
                                        switch (tempSi)
                                        {
                                            case SpecialIdentifier.Code:
                                                {
                                                    lexer.NextToken();
                                                    tempId = Identifier();
                                                    return new ShowFunctionCode(tempId);
                                                }

                                            case SpecialIdentifier.Status:
                                                {
                                                    switch (lexer.NextToken())
                                                    {
                                                        case MySQLToken.KwLike:
                                                            {
                                                                tempStr = Like();
                                                                return new ShowFunctionStatus(tempStr);
                                                            }

                                                        case MySQLToken.KwWhere:
                                                            {
                                                                tempExpr = Where();
                                                                return new ShowFunctionStatus(tempExpr);
                                                            }

                                                        default:
                                                            {
                                                                return new ShowFunctionStatus();
                                                            }
                                                    }
                                                    //break;
                                                }
                                        }
                                    }
                                    throw Err("unexpected token for SHOW FUNCTION");
                                }

                            case SpecialIdentifier.Global:
                                {
                                    lexer.NextToken();
                                    tempStrUp = lexer.StringValueUppercase();
                                    tempSi = specialIdentifiers.GetValue(tempStrUp);
                                    if (tempSi != SpecialIdentifier.None)
                                    {
                                        switch (tempSi)
                                        {
                                            case SpecialIdentifier.Status:
                                                {
                                                    switch (lexer.NextToken())
                                                    {
                                                        case MySQLToken.KwLike:
                                                            {
                                                                tempStr = Like();
                                                                return new ShowStatus(VariableScope.Global, tempStr);
                                                            }

                                                        case MySQLToken.KwWhere:
                                                            {
                                                                tempExpr = Where();
                                                                return new ShowStatus(VariableScope.Global, tempExpr);
                                                            }

                                                        default:
                                                            {
                                                                return new ShowStatus(VariableScope.Global);
                                                            }
                                                    }
                                                    //goto case SpecialIdentifier.Variables;
                                                }

                                            case SpecialIdentifier.Variables:
                                                {
                                                    switch (lexer.NextToken())
                                                    {
                                                        case MySQLToken.KwLike:
                                                            {
                                                                tempStr = Like();
                                                                return new ShowVariables(VariableScope.Global, tempStr);
                                                            }

                                                        case MySQLToken.KwWhere:
                                                            {
                                                                tempExpr = Where();
                                                                return new ShowVariables(VariableScope.Global, tempExpr);
                                                            }

                                                        default:
                                                            {
                                                                return new ShowVariables(VariableScope.Global);
                                                            }
                                                    }
                                                    //break;
                                                }
                                        }
                                    }
                                    throw Err("unexpected token for SHOW GLOBAL");
                                }

                            case SpecialIdentifier.Master:
                                {
                                    lexer.NextToken();
                                    tempStrUp = lexer.StringValueUppercase();
                                    tempSi = specialIdentifiers.GetValue(tempStrUp);
                                    if (tempSi != SpecialIdentifier.None && tempSi == SpecialIdentifier.Status)
                                    {
                                        lexer.NextToken();
                                        return new ShowMasterStatus();
                                    }
                                    MatchIdentifier("LOGS");
                                    return new ShowBinaryLog();
                                }

                            case SpecialIdentifier.Open:
                                {
                                    lexer.NextToken();
                                    MatchIdentifier("TABLES");
                                    tempId = null;
                                    switch (lexer.Token())
                                    {
                                        case MySQLToken.KwIn:
                                        case MySQLToken.KwFrom:
                                            {
                                                lexer.NextToken();
                                                tempId = Identifier();
                                                break;
                                            }
                                    }
                                    switch (lexer.Token())
                                    {
                                        case MySQLToken.KwLike:
                                            {
                                                tempStr = Like();
                                                return new ShowOpenTables(tempId, tempStr);
                                            }

                                        case MySQLToken.KwWhere:
                                            {
                                                tempExpr = Where();
                                                return new ShowOpenTables(tempId, tempExpr);
                                            }

                                        default:
                                            {
                                                return new ShowOpenTables(tempId);
                                            }
                                    }
                                    //goto case SpecialIdentifier.Plugins;
                                }

                            case SpecialIdentifier.Plugins:
                                {
                                    lexer.NextToken();
                                    return new ShowPlugins();
                                }

                            case SpecialIdentifier.Privileges:
                                {
                                    lexer.NextToken();
                                    return new ShowPrivileges();
                                }

                            case SpecialIdentifier.Processlist:
                                {
                                    lexer.NextToken();
                                    return new ShowProcesslist(false);
                                }

                            case SpecialIdentifier.Profile:
                                {
                                    return ShowProfile();
                                }

                            case SpecialIdentifier.Profiles:
                                {
                                    lexer.NextToken();
                                    return new ShowProfiles();
                                }

                            case SpecialIdentifier.Local:
                            case SpecialIdentifier.Session:
                                {
                                    lexer.NextToken();
                                    tempStrUp = lexer.StringValueUppercase();
                                    tempSi = specialIdentifiers.GetValue(tempStrUp);
                                    if (tempSi != SpecialIdentifier.None)
                                    {
                                        switch (tempSi)
                                        {
                                            case SpecialIdentifier.Status:
                                                {
                                                    switch (lexer.NextToken())
                                                    {
                                                        case MySQLToken.KwLike:
                                                            {
                                                                tempStr = Like();
                                                                return new ShowStatus(VariableScope.Session, tempStr);
                                                            }

                                                        case MySQLToken.KwWhere:
                                                            {
                                                                tempExpr = Where();
                                                                return new ShowStatus(VariableScope.Session, tempExpr);
                                                            }

                                                        default:
                                                            {
                                                                return new ShowStatus(VariableScope.Session);
                                                            }
                                                    }
                                                    //goto case SpecialIdentifier.Variables;
                                                }

                                            case SpecialIdentifier.Variables:
                                                {
                                                    switch (lexer.NextToken())
                                                    {
                                                        case MySQLToken.KwLike:
                                                            {
                                                                tempStr = Like();
                                                                return new ShowVariables(VariableScope.Session, tempStr);
                                                            }

                                                        case MySQLToken.KwWhere:
                                                            {
                                                                tempExpr = Where();
                                                                return new ShowVariables(VariableScope.Session, tempExpr);
                                                            }

                                                        default:
                                                            {
                                                                return new ShowVariables(VariableScope.Session);
                                                            }
                                                    }
                                                    // break;
                                                }
                                        }
                                    }
                                    throw Err("unexpected token for SHOW SESSION");
                                }

                            case SpecialIdentifier.Slave:
                                {
                                    lexer.NextToken();
                                    tempStrUp = lexer.StringValueUppercase();
                                    tempSi = specialIdentifiers.GetValue(tempStrUp);
                                    if (tempSi != SpecialIdentifier.None)
                                    {
                                        switch (tempSi)
                                        {
                                            case SpecialIdentifier.Hosts:
                                                {
                                                    lexer.NextToken();
                                                    return new ShowSlaveHosts();
                                                }

                                            case SpecialIdentifier.Status:
                                                {
                                                    lexer.NextToken();
                                                    return new ShowSlaveStatus();
                                                }
                                        }
                                    }
                                    throw Err("unexpected token for SHOW SLAVE");
                                }

                            case SpecialIdentifier.Status:
                                {
                                    switch (lexer.NextToken())
                                    {
                                        case MySQLToken.KwLike:
                                            {
                                                tempStr = Like();
                                                return new ShowStatus(VariableScope.Session, tempStr);
                                            }

                                        case MySQLToken.KwWhere:
                                            {
                                                tempExpr = Where();
                                                return new ShowStatus(VariableScope.Session, tempExpr);
                                            }

                                        default:
                                            {
                                                return new ShowStatus(VariableScope.Session);
                                            }
                                    }
                                    //goto case SpecialIdentifier.Storage;
                                }

                            case SpecialIdentifier.Storage:
                                {
                                    lexer.NextToken();
                                    MatchIdentifier("ENGINES");
                                    return new ShowEngines();
                                }

                            case SpecialIdentifier.Tables:
                                {
                                    tempId = null;
                                    switch (lexer.NextToken())
                                    {
                                        case MySQLToken.KwIn:
                                        case MySQLToken.KwFrom:
                                            {
                                                lexer.NextToken();
                                                tempId = Identifier();
                                                break;
                                            }
                                    }
                                    switch (lexer.Token())
                                    {
                                        case MySQLToken.KwLike:
                                            {
                                                tempStr = Like();
                                                return new ShowTables(false, tempId, tempStr);
                                            }

                                        case MySQLToken.KwWhere:
                                            {
                                                tempExpr = Where();
                                                return new ShowTables(false, tempId, tempExpr);
                                            }

                                        default:
                                            {
                                                return new ShowTables(false, tempId);
                                            }
                                    }
                                    //goto case SpecialIdentifier.Triggers;
                                }

                            case SpecialIdentifier.Triggers:
                                {
                                    tempId = null;
                                    switch (lexer.NextToken())
                                    {
                                        case MySQLToken.KwIn:
                                        case MySQLToken.KwFrom:
                                            {
                                                lexer.NextToken();
                                                tempId = Identifier();
                                                break;
                                            }
                                    }
                                    switch (lexer.Token())
                                    {
                                        case MySQLToken.KwLike:
                                            {
                                                tempStr = Like();
                                                return new ShowTriggers(tempId, tempStr);
                                            }

                                        case MySQLToken.KwWhere:
                                            {
                                                tempExpr = Where();
                                                return new ShowTriggers(tempId, tempExpr);
                                            }

                                        default:
                                            {
                                                return new ShowTriggers(tempId);
                                            }
                                    }
                                    //goto case SpecialIdentifier.Variables;
                                }

                            case SpecialIdentifier.Variables:
                                {
                                    switch (lexer.NextToken())
                                    {
                                        case MySQLToken.KwLike:
                                            {
                                                tempStr = Like();
                                                return new ShowVariables(VariableScope.Session, tempStr);
                                            }

                                        case MySQLToken.KwWhere:
                                            {
                                                tempExpr = Where();
                                                return new ShowVariables(VariableScope.Session, tempExpr);
                                            }

                                        default:
                                            {
                                                return new ShowVariables(VariableScope.Session);
                                            }
                                    }
                                    //goto case SpecialIdentifier.Warnings;
                                }

                            case SpecialIdentifier.Warnings:
                                {
                                    lexer.NextToken();
                                    tempLimit = Limit();
                                    return new ShowWarnings(false, tempLimit);
                                }
                        }
                        break;
                    }
            }
            throw Err("unexpect token for SHOW");
        }

        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        private DalShowIndex ShowIndex(DalShowIndex.Type type)
        {
            lexer.NextToken();
            Match(MySQLToken.KwFrom, MySQLToken.KwIn);
            Identifier tempId = Identifier();
            if (lexer.Token() == MySQLToken.KwFrom || lexer.Token() == MySQLToken.KwIn)
            {
                lexer.NextToken();
                Identifier tempId2 = Identifier();
                return new DalShowIndex(type, tempId, tempId2);
            }
            return new DalShowIndex(type, tempId);
        }

        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        private DalShowProfile ShowProfile()
        {
            lexer.NextToken();
            IList<DalShowProfile.Type> types = new List<DalShowProfile.Type>();
            DalShowProfile.Type type = ShowPrifileType();
            if (type == DalShowProfile.Type.None)
            {
                types = new List<DalShowProfile.Type>(0);
            }
            else
            {
                if (lexer.Token() == MySQLToken.PuncComma)
                {
                    types = new List<DalShowProfile.Type>();
                    types.Add(type);
                    for (; lexer.Token() == MySQLToken.PuncComma;)
                    {
                        lexer.NextToken();
                        type = ShowPrifileType();
                        types.Add(type);
                    }
                }
                else
                {
                    types = new List<DalShowProfile.Type>();
                    types.Add(type);
                }
            }
            Expr forQuery = null;
            if (lexer.Token() == MySQLToken.KwFor)
            {
                lexer.NextToken();
                MatchIdentifier("QUERY");
                forQuery = exprParser.Expression();
            }
            Limit limit = Limit();
            return new DalShowProfile(types, forQuery, limit);
        }

        /// <returns>null if not a type</returns>
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        private DalShowProfile.Type ShowPrifileType()
        {
            switch (lexer.Token())
            {
                case MySQLToken.KwAll:
                    {
                        lexer.NextToken();
                        return DalShowProfile.Type.All;
                    }

                case MySQLToken.Identifier:
                    {
                        string strUp = lexer.StringValueUppercase();
                        SpecialIdentifier si = specialIdentifiers.GetValue(strUp);
                        if (si != SpecialIdentifier.None)
                        {
                            switch (si)
                            {
                                case SpecialIdentifier.Block:
                                    {
                                        lexer.NextToken();
                                        MatchIdentifier("IO");
                                        return DalShowProfile.Type.BlockIo;
                                    }

                                case SpecialIdentifier.Context:
                                    {
                                        lexer.NextToken();
                                        MatchIdentifier("SWITCHES");
                                        return DalShowProfile.Type.ContextSwitches;
                                    }

                                case SpecialIdentifier.Cpu:
                                    {
                                        lexer.NextToken();
                                        return DalShowProfile.Type.Cpu;
                                    }

                                case SpecialIdentifier.Ipc:
                                    {
                                        lexer.NextToken();
                                        return DalShowProfile.Type.Ipc;
                                    }

                                case SpecialIdentifier.Memory:
                                    {
                                        lexer.NextToken();
                                        return DalShowProfile.Type.Memory;
                                    }

                                case SpecialIdentifier.Page:
                                    {
                                        lexer.NextToken();
                                        MatchIdentifier("FAULTS");
                                        return DalShowProfile.Type.PageFaults;
                                    }

                                case SpecialIdentifier.Source:
                                    {
                                        lexer.NextToken();
                                        return DalShowProfile.Type.Source;
                                    }

                                case SpecialIdentifier.Swaps:
                                    {
                                        lexer.NextToken();
                                        return DalShowProfile.Type.Swaps;
                                    }
                            }
                        }
                        goto default;
                    }

                default:
                    {
                        return DalShowProfile.Type.None;
                    }
            }
        }

        /// <summary>
        /// First token is
        /// <see cref="SpecialIdentifier.Columns"/>
        /// <pre>
        /// SHOW [FULL] <code>COLUMNS {FROM | IN} tbl_name [{FROM | IN} db_name] [LIKE 'pattern' | WHERE expr] </code>
        /// </pre>
        /// </summary>
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        private DalShowColumns ShowColumns(bool full)
        {
            lexer.NextToken();
            Match(MySQLToken.KwFrom, MySQLToken.KwIn);
            Identifier table = Identifier();
            Identifier database = null;
            switch (lexer.Token())
            {
                case MySQLToken.KwFrom:
                case MySQLToken.KwIn:
                    {
                        lexer.NextToken();
                        database = Identifier();
                        break;
                    }
            }
            switch (lexer.Token())
            {
                case MySQLToken.KwLike:
                    {
                        string like = Like();
                        return new DalShowColumns(full, table, database, like);
                    }

                case MySQLToken.KwWhere:
                    {
                        Expr where = Where();
                        return new DalShowColumns(full, table, database, where);
                    }
            }
            return new DalShowColumns(full, table, database);
        }

        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        private string Like()
        {
            Match(MySQLToken.KwLike);
            string pattern = lexer.StringValue();
            lexer.NextToken();
            return pattern;
        }

        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        private Expr Where()
        {
            Match(MySQLToken.KwWhere);
            Expr where = exprParser.Expression();
            return where;
        }

        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        private string GetStringValue()
        {
            string name;
            switch (lexer.Token())
            {
                case MySQLToken.Identifier:
                    {
                        name = Tup.Cobar4Net.Parser.Ast.Expression.Primary.Identifier.UnescapeName(lexer.StringValue());
                        lexer.NextToken();
                        return name;
                    }

                case MySQLToken.LiteralChars:
                    {
                        name = lexer.StringValue();
                        name = LiteralString.GetUnescapedString(Sharpen.Runtime.Substring(name, 1, name.Length - 1));
                        lexer.NextToken();
                        return name;
                    }

                default:
                    {
                        throw Err("unexpected token: " + lexer.Token());
                    }
            }
        }

        /// <returns>
        ///
        /// <see cref="Tup.Cobar4Net.Parser.Ast.Stmt.Dal.DALSetStatement"/>
        /// or
        /// <see cref="Tup.Cobar4Net.Parser.Ast.Stmt.Mts.MTSSetTransactionStatement"/>
        /// </returns>
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        public virtual SQLStatement Set()
        {
            Match(MySQLToken.KwSet);
            if (lexer.Token() == MySQLToken.KwOption)
            {
                lexer.NextToken();
            }
            if (lexer.Token() == MySQLToken.Identifier
                && SpecialIdentifier.Names == specialIdentifiers.GetValue(lexer.StringValueUppercase()))
            {
                if (lexer.NextToken() == MySQLToken.KwDefault)
                {
                    lexer.NextToken();
                    return new DALSetNamesStatement();
                }
                string charsetName = GetStringValue();
                string collationName = null;
                if (lexer.Token() == MySQLToken.KwCollate)
                {
                    lexer.NextToken();
                    collationName = GetStringValue();
                }
                return new DALSetNamesStatement(charsetName, collationName);
            }
            else
            {
                if (lexer.Token() == MySQLToken.KwCharacter)
                {
                    lexer.NextToken();
                    Match(MySQLToken.KwSet);
                    if (lexer.Token() == MySQLToken.KwDefault)
                    {
                        lexer.NextToken();
                        return new DALSetCharacterSetStatement();
                    }
                    string charsetName = GetStringValue();
                    return new DALSetCharacterSetStatement(charsetName);
                }
            }
            IList<Pair<VariableExpression, Expr>> assignmentList;
            object obj = VarAssign();
            if (obj is MTSSetTransactionStatement)
            {
                return (MTSSetTransactionStatement)obj;
            }
            Pair<VariableExpression, Expr> pair = (Pair<VariableExpression, Expr>)obj;
            if (lexer.Token() != MySQLToken.PuncComma)
            {
                assignmentList = new List<Pair<VariableExpression, Expr>>(1);
                assignmentList.Add(pair);
                return new DALSetStatement(assignmentList);
            }
            assignmentList = new List<Pair<VariableExpression, Expr>>();
            assignmentList.Add(pair);
            for (; lexer.Token() == MySQLToken.PuncComma;)
            {
                lexer.NextToken();
                pair = (Pair<VariableExpression, Expr>)VarAssign();
                assignmentList.Add(pair);
            }
            return new DALSetStatement(assignmentList);
        }

        /// <summary>first token is <code>TRANSACTION</code></summary>
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        private MTSSetTransactionStatement SetMTSSetTransactionStatement(VariableScope scope)
        {
            lexer.NextToken();
            MatchIdentifier("ISOLATION");
            MatchIdentifier("LEVEL");
            SpecialIdentifier si;
            switch (lexer.Token())
            {
                case MySQLToken.KwRead:
                    {
                        lexer.NextToken();
                        si = specialIdentifiers.GetValue(lexer.StringValueUppercase());
                        if (si != SpecialIdentifier.None)
                        {
                            switch (si)
                            {
                                case SpecialIdentifier.Committed:
                                    {
                                        lexer.NextToken();
                                        return new MTSSetTransactionStatement(scope, MTSSetTransactionStatement.IsolationLevel.ReadCommitted);
                                    }

                                case SpecialIdentifier.Uncommitted:
                                    {
                                        lexer.NextToken();
                                        return new MTSSetTransactionStatement(scope, MTSSetTransactionStatement.IsolationLevel.ReadUncommitted);
                                    }
                            }
                        }
                        throw Err("unknown isolation read level: " + lexer.StringValue());
                    }

                case MySQLToken.Identifier:
                    {
                        si = specialIdentifiers.GetValue(lexer.StringValueUppercase());
                        if (si != SpecialIdentifier.None)
                        {
                            switch (si)
                            {
                                case SpecialIdentifier.Repeatable:
                                    {
                                        lexer.NextToken();
                                        Match(MySQLToken.KwRead);
                                        return new MTSSetTransactionStatement(scope, MTSSetTransactionStatement.IsolationLevel.RepeatableRead);
                                    }

                                case SpecialIdentifier.Serializable:
                                    {
                                        lexer.NextToken();
                                        return new MTSSetTransactionStatement(scope, MTSSetTransactionStatement.IsolationLevel.Serializable);
                                    }
                            }
                        }
                        break;
                    }
            }
            throw Err("unknown isolation level: " + lexer.StringValue());
        }

        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        private object VarAssign()
        {
            VariableExpression var;
            Expr expr;
            VariableScope scope = VariableScope.Session;
            switch (lexer.Token())
            {
                case MySQLToken.Identifier:
                    {
                        bool explictScope = false;
                        var si = specialIdentifiers.GetValue(lexer.StringValueUppercase());
                        if (si != SpecialIdentifier.None)
                        {
                            switch (si)
                            {
                                case SpecialIdentifier.Transaction:
                                    {
                                        return SetMTSSetTransactionStatement(VariableScope.None);
                                    }

                                case SpecialIdentifier.Global:
                                    {
                                        scope = VariableScope.Global;
                                        goto case SpecialIdentifier.Session;
                                    }

                                case SpecialIdentifier.Session:
                                case SpecialIdentifier.Local:
                                    {
                                        explictScope = true;
                                        lexer.NextToken();
                                        goto default;
                                    }

                                default:
                                    {
                                        break;
                                    }
                            }
                        }
                        if (explictScope
                            && specialIdentifiers.GetValue(lexer.StringValueUppercase()) == SpecialIdentifier.Transaction)
                        {
                            return SetMTSSetTransactionStatement(scope);
                        }
                        var = new SysVarPrimary(scope, lexer.StringValue(), lexer.StringValueUppercase());
                        Match(MySQLToken.Identifier);
                        break;
                    }

                case MySQLToken.SysVar:
                    {
                        var = SystemVariale();
                        break;
                    }

                case MySQLToken.UsrVar:
                    {
                        var = new UsrDefVarPrimary(lexer.StringValue());
                        lexer.NextToken();
                        break;
                    }

                default:
                    {
                        throw Err("unexpected token for SET statement");
                    }
            }
            Match(MySQLToken.OpEquals, MySQLToken.OpAssign);
            expr = exprParser.Expression();
            return new Pair<VariableExpression, Expr>(var, expr);
        }
    }
}