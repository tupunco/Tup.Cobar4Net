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
using Sharpen;
using Tup.Cobar4Net.Parser.Ast.Expression;
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


namespace Tup.Cobar4Net.Parser.Recognizer.Mysql.Syntax
{
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public class MySqlDalParser : MySqlParser
    {
        private static readonly IDictionary<string, SpecialIdentifier> specialIdentifiers
            = new Dictionary<string, SpecialIdentifier>();

        protected MySqlExprParser exprParser;

        static MySqlDalParser()
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

        public MySqlDalParser(MySqlLexer lexer, MySqlExprParser exprParser)
            : base(lexer)
        {
            this.exprParser = exprParser;
        }

        /// <exception cref="System.SqlSyntaxErrorException" />
        public virtual DescTableStatement Desc()
        {
            Match(MySqlToken.KwDesc, MySqlToken.KwDescribe);
            var table = Identifier();
            return new DescTableStatement(table);
        }

        /// <exception cref="System.SqlSyntaxErrorException" />
        public virtual DalShowStatement Show()
        {
            Match(MySqlToken.KwShow);
            string tempStr;
            string tempStrUp;
            IExpression tempExpr;
            Identifier tempId;
            SpecialIdentifier tempSi;
            Limit tempLimit;
            switch (lexer.Token())
            {
                case MySqlToken.KwBinary:
                {
                    lexer.NextToken();
                    MatchIdentifier("LOGS");
                    return new ShowBinaryLog();
                }

                case MySqlToken.KwCharacter:
                {
                    lexer.NextToken();
                    Match(MySqlToken.KwSet);
                    switch (lexer.Token())
                    {
                        case MySqlToken.KwLike:
                        {
                            tempStr = Like();
                            return new ShowCharaterSet(tempStr);
                        }

                        case MySqlToken.KwWhere:
                        {
                            tempExpr = Where();
                            return new ShowCharaterSet(tempExpr);
                        }

                        default:
                        {
                            return new ShowCharaterSet();
                        }
                    }
                    //goto case MySqlToken.KwCreate;
                }

                case MySqlToken.KwCreate:
                {
                    CreateType createType;
                    switch (lexer.NextToken())
                    {
                        case MySqlToken.KwDatabase:
                        {
                            createType = CreateType.Database;
                            break;
                        }

                        case MySqlToken.KwProcedure:
                        {
                            createType = CreateType.Procedure;
                            break;
                        }

                        case MySqlToken.KwTable:
                        {
                            createType = CreateType.Table;
                            break;
                        }

                        case MySqlToken.KwTrigger:
                        {
                            createType = CreateType.Trigger;
                            break;
                        }

                        case MySqlToken.Identifier:
                        {
                            tempSi = specialIdentifiers.GetValue(lexer.GetStringValueUppercase());
                            if (tempSi != SpecialIdentifier.None)
                            {
                                switch (tempSi)
                                {
                                    case SpecialIdentifier.Event:
                                    {
                                        createType = CreateType.Event;
                                        goto switch1_break;
                                    }

                                    case SpecialIdentifier.Function:
                                    {
                                        createType = CreateType.Function;
                                        goto switch1_break;
                                    }

                                    case SpecialIdentifier.View:
                                    {
                                        createType = CreateType.View;
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
                    switch1_break:
                    ;
                    lexer.NextToken();
                    tempId = Identifier();
                    return new ShowCreate(createType, tempId);
                }

                case MySqlToken.KwSchemas:
                case MySqlToken.KwDatabases:
                {
                    lexer.NextToken();
                    switch (lexer.Token())
                    {
                        case MySqlToken.KwLike:
                        {
                            tempStr = Like();
                            return new ShowDatabases(tempStr);
                        }

                        case MySqlToken.KwWhere:
                        {
                            tempExpr = Where();
                            return new ShowDatabases(tempExpr);
                        }
                    }
                    return new ShowDatabases();
                }

                case MySqlToken.KwKeys:
                {
                    return ShowIndex(ShowIndexType.Keys);
                }

                case MySqlToken.KwIndex:
                {
                    return ShowIndex(ShowIndexType.Index);
                }

                case MySqlToken.KwProcedure:
                {
                    lexer.NextToken();
                    tempStrUp = lexer.GetStringValueUppercase();
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
                                    case MySqlToken.KwLike:
                                    {
                                        tempStr = Like();
                                        return new ShowProcedureStatus(tempStr);
                                    }

                                    case MySqlToken.KwWhere:
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

                case MySqlToken.KwTable:
                {
                    lexer.NextToken();
                    MatchIdentifier("STATUS");
                    tempId = null;
                    if (lexer.Token() == MySqlToken.KwFrom || lexer.Token() == MySqlToken.KwIn)
                    {
                        lexer.NextToken();
                        tempId = Identifier();
                    }
                    switch (lexer.Token())
                    {
                        case MySqlToken.KwLike:
                        {
                            tempStr = Like();
                            return new ShowTableStatus(tempId, tempStr);
                        }

                        case MySqlToken.KwWhere:
                        {
                            tempExpr = Where();
                            return new ShowTableStatus(tempId, tempExpr);
                        }
                    }
                    return new ShowTableStatus(tempId);
                }

                case MySqlToken.Identifier:
                {
                    tempStrUp = lexer.GetStringValueUppercase();
                    tempSi = specialIdentifiers.GetValue(tempStrUp);
                    if (tempSi == SpecialIdentifier.None)
                    {
                        break;
                    }
                    switch (tempSi)
                    {
                        case SpecialIdentifier.Indexes:
                        {
                            return ShowIndex(ShowIndexType.Indexes);
                        }

                        case SpecialIdentifier.Grants:
                        {
                            if (lexer.NextToken() == MySqlToken.KwFor)
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
                            if (lexer.Token() == MySqlToken.KwIn)
                            {
                                lexer.NextToken();
                                tempStr = lexer.GetStringValue();
                                lexer.NextToken();
                            }
                            if (lexer.Token() == MySqlToken.KwFrom)
                            {
                                lexer.NextToken();
                                tempExpr = exprParser.Expression();
                            }
                            if (lexer.Token() == MySqlToken.KwLimit)
                            {
                                tempLimit = Limit();
                            }
                            return new ShowBinLogEvent(tempStr, tempExpr, tempLimit);
                        }

                        case SpecialIdentifier.Collation:
                        {
                            switch (lexer.NextToken())
                            {
                                case MySqlToken.KwLike:
                                {
                                    tempStr = Like();
                                    return new ShowCollation(tempStr);
                                }

                                case MySqlToken.KwWhere:
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
                                case MySqlToken.Identifier:
                                {
                                    tempStrUp = lexer.GetStringValueUppercase();
                                    tempSi = specialIdentifiers.GetValue(tempStrUp);
                                    if (tempSi != SpecialIdentifier.None)
                                    {
                                        switch (tempSi)
                                        {
                                            case SpecialIdentifier.Innodb:
                                            {
                                                lexer.NextToken();
                                                tempStrUp = lexer.GetStringValueUppercase();
                                                tempSi = specialIdentifiers.GetValue(tempStrUp);
                                                if (tempSi != SpecialIdentifier.None)
                                                {
                                                    switch (tempSi)
                                                    {
                                                        case SpecialIdentifier.Status:
                                                        {
                                                            lexer.NextToken();
                                                            return new ShowEngine(EngineType.InnodbStatus);
                                                        }

                                                        case SpecialIdentifier.Mutex:
                                                        {
                                                            lexer.NextToken();
                                                            return new ShowEngine(EngineType.InnodbMutex);
                                                        }
                                                    }
                                                }
                                                goto case SpecialIdentifier.PerformanceSchema;
                                            }

                                            case SpecialIdentifier.PerformanceSchema:
                                            {
                                                lexer.NextToken();
                                                MatchIdentifier("STATUS");
                                                return new ShowEngine(EngineType.PerformanceSchemaStatus);
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
                            Match(MySqlToken.PuncLeftParen);
                            Match(MySqlToken.OpAsterisk);
                            Match(MySqlToken.PuncRightParen);
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
                                case MySqlToken.KwIn:
                                case MySqlToken.KwFrom:
                                {
                                    lexer.NextToken();
                                    tempId = Identifier();
                                    break;
                                }
                            }
                            switch (lexer.Token())
                            {
                                case MySqlToken.KwLike:
                                {
                                    tempStr = Like();
                                    return new ShowEvents(tempId, tempStr);
                                }

                                case MySqlToken.KwWhere:
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
                            tempStrUp = lexer.GetStringValueUppercase();
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
                                            case MySqlToken.KwIn:
                                            case MySqlToken.KwFrom:
                                            {
                                                lexer.NextToken();
                                                tempId = Identifier();
                                                break;
                                            }
                                        }
                                        switch (lexer.Token())
                                        {
                                            case MySqlToken.KwLike:
                                            {
                                                tempStr = Like();
                                                return new ShowTables(true, tempId, tempStr);
                                            }

                                            case MySqlToken.KwWhere:
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
                            tempStrUp = lexer.GetStringValueUppercase();
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
                                            case MySqlToken.KwLike:
                                            {
                                                tempStr = Like();
                                                return new ShowFunctionStatus(tempStr);
                                            }

                                            case MySqlToken.KwWhere:
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
                            tempStrUp = lexer.GetStringValueUppercase();
                            tempSi = specialIdentifiers.GetValue(tempStrUp);
                            if (tempSi != SpecialIdentifier.None)
                            {
                                switch (tempSi)
                                {
                                    case SpecialIdentifier.Status:
                                    {
                                        switch (lexer.NextToken())
                                        {
                                            case MySqlToken.KwLike:
                                            {
                                                tempStr = Like();
                                                return new ShowStatus(VariableScope.Global, tempStr);
                                            }

                                            case MySqlToken.KwWhere:
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
                                            case MySqlToken.KwLike:
                                            {
                                                tempStr = Like();
                                                return new ShowVariables(VariableScope.Global, tempStr);
                                            }

                                            case MySqlToken.KwWhere:
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
                            tempStrUp = lexer.GetStringValueUppercase();
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
                                case MySqlToken.KwIn:
                                case MySqlToken.KwFrom:
                                {
                                    lexer.NextToken();
                                    tempId = Identifier();
                                    break;
                                }
                            }
                            switch (lexer.Token())
                            {
                                case MySqlToken.KwLike:
                                {
                                    tempStr = Like();
                                    return new ShowOpenTables(tempId, tempStr);
                                }

                                case MySqlToken.KwWhere:
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
                            tempStrUp = lexer.GetStringValueUppercase();
                            tempSi = specialIdentifiers.GetValue(tempStrUp);
                            if (tempSi != SpecialIdentifier.None)
                            {
                                switch (tempSi)
                                {
                                    case SpecialIdentifier.Status:
                                    {
                                        switch (lexer.NextToken())
                                        {
                                            case MySqlToken.KwLike:
                                            {
                                                tempStr = Like();
                                                return new ShowStatus(VariableScope.Session, tempStr);
                                            }

                                            case MySqlToken.KwWhere:
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
                                            case MySqlToken.KwLike:
                                            {
                                                tempStr = Like();
                                                return new ShowVariables(VariableScope.Session, tempStr);
                                            }

                                            case MySqlToken.KwWhere:
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
                            tempStrUp = lexer.GetStringValueUppercase();
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
                                case MySqlToken.KwLike:
                                {
                                    tempStr = Like();
                                    return new ShowStatus(VariableScope.Session, tempStr);
                                }

                                case MySqlToken.KwWhere:
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
                                case MySqlToken.KwIn:
                                case MySqlToken.KwFrom:
                                {
                                    lexer.NextToken();
                                    tempId = Identifier();
                                    break;
                                }
                            }
                            switch (lexer.Token())
                            {
                                case MySqlToken.KwLike:
                                {
                                    tempStr = Like();
                                    return new ShowTables(false, tempId, tempStr);
                                }

                                case MySqlToken.KwWhere:
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
                                case MySqlToken.KwIn:
                                case MySqlToken.KwFrom:
                                {
                                    lexer.NextToken();
                                    tempId = Identifier();
                                    break;
                                }
                            }
                            switch (lexer.Token())
                            {
                                case MySqlToken.KwLike:
                                {
                                    tempStr = Like();
                                    return new ShowTriggers(tempId, tempStr);
                                }

                                case MySqlToken.KwWhere:
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
                                case MySqlToken.KwLike:
                                {
                                    tempStr = Like();
                                    return new ShowVariables(VariableScope.Session, tempStr);
                                }

                                case MySqlToken.KwWhere:
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

        /// <exception cref="System.SqlSyntaxErrorException" />
        private DalShowIndex ShowIndex(ShowIndexType _showIndexType)
        {
            lexer.NextToken();
            Match(MySqlToken.KwFrom, MySqlToken.KwIn);
            var tempId = Identifier();
            if (lexer.Token() == MySqlToken.KwFrom || lexer.Token() == MySqlToken.KwIn)
            {
                lexer.NextToken();
                var tempId2 = Identifier();
                return new DalShowIndex(_showIndexType, tempId, tempId2);
            }
            return new DalShowIndex(_showIndexType, tempId);
        }

        /// <exception cref="System.SqlSyntaxErrorException" />
        private DalShowProfile ShowProfile()
        {
            lexer.NextToken();
            IList<ProfileType> types = new List<ProfileType>();
            var type = ShowPrifileType();
            if (type == ProfileType.None)
            {
                types = new List<ProfileType>(0);
            }
            else
            {
                if (lexer.Token() == MySqlToken.PuncComma)
                {
                    types = new List<ProfileType>();
                    types.Add(type);
                    for (; lexer.Token() == MySqlToken.PuncComma;)
                    {
                        lexer.NextToken();
                        type = ShowPrifileType();
                        types.Add(type);
                    }
                }
                else
                {
                    types = new List<ProfileType>();
                    types.Add(type);
                }
            }
            IExpression forQuery = null;
            if (lexer.Token() == MySqlToken.KwFor)
            {
                lexer.NextToken();
                MatchIdentifier("QUERY");
                forQuery = exprParser.Expression();
            }
            var limit = Limit();
            return new DalShowProfile(types, forQuery, limit);
        }

        /// <returns>null if not a type</returns>
        /// <exception cref="System.SqlSyntaxErrorException" />
        private ProfileType ShowPrifileType()
        {
            switch (lexer.Token())
            {
                case MySqlToken.KwAll:
                {
                    lexer.NextToken();
                    return ProfileType.All;
                }

                case MySqlToken.Identifier:
                {
                    var strUp = lexer.GetStringValueUppercase();
                    var si = specialIdentifiers.GetValue(strUp);
                    if (si != SpecialIdentifier.None)
                    {
                        switch (si)
                        {
                            case SpecialIdentifier.Block:
                            {
                                lexer.NextToken();
                                MatchIdentifier("IO");
                                return ProfileType.BlockIo;
                            }

                            case SpecialIdentifier.Context:
                            {
                                lexer.NextToken();
                                MatchIdentifier("SWITCHES");
                                return ProfileType.ContextSwitches;
                            }

                            case SpecialIdentifier.Cpu:
                            {
                                lexer.NextToken();
                                return ProfileType.Cpu;
                            }

                            case SpecialIdentifier.Ipc:
                            {
                                lexer.NextToken();
                                return ProfileType.Ipc;
                            }

                            case SpecialIdentifier.Memory:
                            {
                                lexer.NextToken();
                                return ProfileType.Memory;
                            }

                            case SpecialIdentifier.Page:
                            {
                                lexer.NextToken();
                                MatchIdentifier("FAULTS");
                                return ProfileType.PageFaults;
                            }

                            case SpecialIdentifier.Source:
                            {
                                lexer.NextToken();
                                return ProfileType.Source;
                            }

                            case SpecialIdentifier.Swaps:
                            {
                                lexer.NextToken();
                                return ProfileType.Swaps;
                            }
                        }
                    }
                    goto default;
                }

                default:
                {
                    return ProfileType.None;
                }
            }
        }

        /// <summary>
        ///     First token is
        ///     <see cref="SpecialIdentifier.Columns" />
        ///     <pre>
        ///         SHOW [FULL] <code>COLUMNS {FROM | IN} tbl_name [{FROM | IN} db_name] [LIKE 'pattern' | WHERE expr] </code>
        ///     </pre>
        /// </summary>
        /// <exception cref="System.SqlSyntaxErrorException" />
        private DalShowColumns ShowColumns(bool full)
        {
            lexer.NextToken();
            Match(MySqlToken.KwFrom, MySqlToken.KwIn);
            var table = Identifier();
            Identifier database = null;
            switch (lexer.Token())
            {
                case MySqlToken.KwFrom:
                case MySqlToken.KwIn:
                {
                    lexer.NextToken();
                    database = Identifier();
                    break;
                }
            }
            switch (lexer.Token())
            {
                case MySqlToken.KwLike:
                {
                    var like = Like();
                    return new DalShowColumns(full, table, database, like);
                }

                case MySqlToken.KwWhere:
                {
                    var where = Where();
                    return new DalShowColumns(full, table, database, where);
                }
            }
            return new DalShowColumns(full, table, database);
        }

        /// <exception cref="System.SqlSyntaxErrorException" />
        private string Like()
        {
            Match(MySqlToken.KwLike);
            var pattern = lexer.GetStringValue();
            lexer.NextToken();
            return pattern;
        }

        /// <exception cref="System.SqlSyntaxErrorException" />
        private IExpression Where()
        {
            Match(MySqlToken.KwWhere);
            var where = exprParser.Expression();
            return where;
        }

        /// <exception cref="System.SqlSyntaxErrorException" />
        private string GetStringValue()
        {
            string name;
            switch (lexer.Token())
            {
                case MySqlToken.Identifier:
                {
                    name = Ast.Expression.Primary.Identifier.UnescapeName(lexer.GetStringValue());
                    lexer.NextToken();
                    return name;
                }

                case MySqlToken.LiteralChars:
                {
                    name = lexer.GetStringValue();
                    name = LiteralString.GetUnescapedString(Runtime.Substring(name, 1, name.Length - 1));
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
        ///     <see cref="Tup.Cobar4Net.Parser.Ast.Stmt.Dal.DalSetStatement" />
        ///     or
        ///     <see cref="Tup.Cobar4Net.Parser.Ast.Stmt.Mts.MTSSetTransactionStatement" />
        /// </returns>
        /// <exception cref="System.SqlSyntaxErrorException" />
        public virtual ISqlStatement Set()
        {
            Match(MySqlToken.KwSet);
            if (lexer.Token() == MySqlToken.KwOption)
            {
                lexer.NextToken();
            }
            if (lexer.Token() == MySqlToken.Identifier
                && SpecialIdentifier.Names == specialIdentifiers.GetValue(lexer.GetStringValueUppercase()))
            {
                if (lexer.NextToken() == MySqlToken.KwDefault)
                {
                    lexer.NextToken();
                    return new DalSetNamesStatement();
                }
                var charsetName = GetStringValue();
                string collationName = null;
                if (lexer.Token() == MySqlToken.KwCollate)
                {
                    lexer.NextToken();
                    collationName = GetStringValue();
                }
                return new DalSetNamesStatement(charsetName, collationName);
            }
            if (lexer.Token() == MySqlToken.KwCharacter)
            {
                lexer.NextToken();
                Match(MySqlToken.KwSet);
                if (lexer.Token() == MySqlToken.KwDefault)
                {
                    lexer.NextToken();
                    return new DalSetCharacterSetStatement();
                }
                var charsetName = GetStringValue();
                return new DalSetCharacterSetStatement(charsetName);
            }
            IList<Pair<VariableExpression, IExpression>> assignmentList;
            var obj = VarAssign();
            if (obj is MTSSetTransactionStatement)
            {
                return (MTSSetTransactionStatement) obj;
            }
            var pair = (Pair<VariableExpression, IExpression>) obj;
            if (lexer.Token() != MySqlToken.PuncComma)
            {
                assignmentList = new List<Pair<VariableExpression, IExpression>>(1);
                assignmentList.Add(pair);
                return new DalSetStatement(assignmentList);
            }
            assignmentList = new List<Pair<VariableExpression, IExpression>>();
            assignmentList.Add(pair);
            for (; lexer.Token() == MySqlToken.PuncComma;)
            {
                lexer.NextToken();
                pair = (Pair<VariableExpression, IExpression>) VarAssign();
                assignmentList.Add(pair);
            }
            return new DalSetStatement(assignmentList);
        }

        /// <summary>first token is <code>TRANSACTION</code></summary>
        /// <exception cref="System.SqlSyntaxErrorException" />
        private MTSSetTransactionStatement SetMTSSetTransactionStatement(VariableScope scope)
        {
            lexer.NextToken();
            MatchIdentifier("ISOLATION");
            MatchIdentifier("LEVEL");
            SpecialIdentifier si;
            switch (lexer.Token())
            {
                case MySqlToken.KwRead:
                {
                    lexer.NextToken();
                    si = specialIdentifiers.GetValue(lexer.GetStringValueUppercase());
                    if (si != SpecialIdentifier.None)
                    {
                        switch (si)
                        {
                            case SpecialIdentifier.Committed:
                            {
                                lexer.NextToken();
                                return new MTSSetTransactionStatement(scope,
                                    IsolationLevel.ReadCommitted);
                            }

                            case SpecialIdentifier.Uncommitted:
                            {
                                lexer.NextToken();
                                return new MTSSetTransactionStatement(scope,
                                    IsolationLevel.ReadUncommitted);
                            }
                        }
                    }
                    throw Err("unknown isolation read level: " + lexer.GetStringValue());
                }

                case MySqlToken.Identifier:
                {
                    si = specialIdentifiers.GetValue(lexer.GetStringValueUppercase());
                    if (si != SpecialIdentifier.None)
                    {
                        switch (si)
                        {
                            case SpecialIdentifier.Repeatable:
                            {
                                lexer.NextToken();
                                Match(MySqlToken.KwRead);
                                return new MTSSetTransactionStatement(scope,
                                    IsolationLevel.RepeatableRead);
                            }

                            case SpecialIdentifier.Serializable:
                            {
                                lexer.NextToken();
                                return new MTSSetTransactionStatement(scope,
                                    IsolationLevel.Serializable);
                            }
                        }
                    }
                    break;
                }
            }
            throw Err("unknown isolation level: " + lexer.GetStringValue());
        }

        /// <exception cref="System.SqlSyntaxErrorException" />
        private object VarAssign()
        {
            VariableExpression var;
            IExpression expr;
            var scope = VariableScope.Session;
            switch (lexer.Token())
            {
                case MySqlToken.Identifier:
                {
                    var explictScope = false;
                    var si = specialIdentifiers.GetValue(lexer.GetStringValueUppercase());
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
                        && specialIdentifiers.GetValue(lexer.GetStringValueUppercase()) == SpecialIdentifier.Transaction)
                    {
                        return SetMTSSetTransactionStatement(scope);
                    }
                    var = new SysVarPrimary(scope, lexer.GetStringValue(), lexer.GetStringValueUppercase());
                    Match(MySqlToken.Identifier);
                    break;
                }

                case MySqlToken.SysVar:
                {
                    var = SystemVariale();
                    break;
                }

                case MySqlToken.UsrVar:
                {
                    var = new UsrDefVarPrimary(lexer.GetStringValue());
                    lexer.NextToken();
                    break;
                }

                default:
                {
                    throw Err("unexpected token for SET statement");
                }
            }
            Match(MySqlToken.OpEquals, MySqlToken.OpAssign);
            expr = exprParser.Expression();
            return new Pair<VariableExpression, IExpression>(var, expr);
        }

        /// <summary>
        ///     MySqlDalParser SpecialIdentifier
        /// </summary>
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
    }
}