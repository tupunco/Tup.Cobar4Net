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

namespace Tup.Cobar4Net.Parser.Recognizer.Mysql
{
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    [Serializable]
    public enum MySqlToken
    {
        None = 0,

        Eof,
        PlaceHolder,
        Identifier,
        SysVar,
        UsrVar,
        LiteralNumPureDigit,
        LiteralNumMixDigit,
        LiteralHex,
        LiteralBit,
        LiteralChars,
        LiteralNchars,
        LiteralNull,
        LiteralBoolTrue,
        LiteralBoolFalse,
        QuestionMark,
        PuncLeftParen,
        PuncRightParen,
        PuncLeftBrace,
        PuncRightBrace,
        PuncLeftBracket,
        PuncRightBracket,
        PuncSemicolon,
        PuncComma,
        PuncDot,
        PuncColon,
        PuncCStyleCommentEnd,
        OpEquals,
        OpGreaterThan,
        OpLessThan,
        OpExclamation,
        OpTilde,
        OpPlus,
        OpMinus,
        OpAsterisk,
        OpSlash,
        OpAmpersand,
        OpVerticalBar,
        OpCaret,
        OpPercent,
        OpAssign,
        OpLessOrEquals,
        OpLessOrGreater,
        OpGreaterOrEquals,
        OpNotEquals,
        OpLogicalAnd,
        OpLogicalOr,
        OpLeftShift,
        OpRightShift,
        OpNullSafeEquals,
        KwAccessible,
        KwAdd,
        KwAll,
        KwAlter,
        KwAnalyze,
        KwAnd,
        KwAs,
        KwAsc,
        KwAsensitive,
        KwBefore,
        KwBetween,
        KwBigint,
        KwBinary,
        KwBlob,
        KwBoth,
        KwBy,
        KwCall,
        KwCascade,
        KwCase,
        KwChange,
        KwChar,
        KwCharacter,
        KwCheck,
        KwCollate,
        KwColumn,
        KwCondition,
        KwConstraint,
        KwContinue,
        KwConvert,
        KwCreate,
        KwCross,
        KwCurrentDate,
        KwCurrentTime,
        KwCurrentTimestamp,
        KwCurrentUser,
        KwCursor,
        KwDatabase,
        KwDatabases,
        KwDayHour,
        KwDayMicrosecond,
        KwDayMinute,
        KwDaySecond,
        KwDec,
        KwDecimal,
        KwDeclare,
        KwDefault,
        KwDelayed,
        KwDelete,
        KwDesc,
        KwDescribe,
        KwDeterministic,
        KwDistinct,
        KwDistinctrow,
        KwDiv,
        KwDouble,
        KwDrop,
        KwDual,
        KwEach,
        KwElse,
        KwElseif,
        KwEnclosed,
        KwEscaped,
        KwExists,
        KwExit,
        KwExplain,
        KwFetch,
        KwFloat,
        KwFloat4,
        KwFloat8,
        KwFor,
        KwForce,
        KwForeign,
        KwFrom,
        KwFulltext,
        KwGeneral,
        KwGrant,
        KwGroup,
        KwHaving,
        KwHighPriority,
        KwHourMicrosecond,
        KwHourMinute,
        KwHourSecond,
        KwIf,
        KwIgnore,
        KwIgnoreServerIds,
        KwIn,
        KwIndex,
        KwInfile,
        KwInner,
        KwInout,
        KwInsensitive,
        KwInsert,
        KwInt,
        KwInt1,
        KwInt2,
        KwInt3,
        KwInt4,
        KwInt8,
        KwInteger,
        KwInterval,
        KwInto,
        KwIs,
        KwIterate,
        KwJoin,
        KwKey,
        KwKeys,
        KwKill,
        KwLeading,
        KwLeave,
        KwLeft,
        KwLike,
        KwLimit,
        KwLinear,
        KwLines,
        KwLoad,
        KwLocaltime,
        KwLocaltimestamp,
        KwLock,
        KwLong,
        KwLongblob,
        KwLongtext,
        KwLoop,
        KwLowPriority,
        KwMasterHeartbeatPeriod,
        KwMasterSslVerifyServerCert,
        KwMatch,
        KwMaxvalue,
        KwMediumblob,
        KwMediumint,
        KwMediumtext,
        KwMiddleint,
        KwMinuteMicrosecond,
        KwMinuteSecond,
        KwMod,
        KwModifies,
        KwNatural,
        KwNot,
        KwNoWriteToBinlog,
        KwNumeric,
        KwOn,
        KwOptimize,
        KwOption,
        KwOptionally,
        KwOr,
        KwOrder,
        KwOut,
        KwOuter,
        KwOutfile,
        KwPrecision,
        KwPrimary,
        KwProcedure,
        KwPurge,
        KwRange,
        KwRead,
        KwReads,
        KwReadWrite,
        KwReal,
        KwReferences,
        KwRegexp,
        KwRelease,
        KwRename,
        KwRepeat,
        KwReplace,
        KwRequire,
        KwResignal,
        KwRestrict,
        KwReturn,
        KwRevoke,
        KwRight,
        KwRlike,
        KwSchema,
        KwSchemas,
        KwSecondMicrosecond,
        KwSelect,
        KwSensitive,
        KwSeparator,
        KwSet,
        KwShow,
        KwSignal,
        KwSlow,
        KwSmallint,
        KwSpatial,
        KwSpecific,
        KwSql,
        KwSqlexception,
        KwSqlstate,
        KwSqlwarning,
        KwSqlBigResult,
        KwSqlCalcFoundRows,
        KwSqlSmallResult,
        KwSsl,
        KwStarting,
        KwStraightJoin,
        KwTable,
        KwTerminated,
        KwThen,
        KwTinyblob,
        KwTinyint,
        KwTinytext,
        KwTo,
        KwTrailing,
        KwTrigger,
        KwUndo,
        KwUnion,
        KwUnique,
        KwUnlock,
        KwUnsigned,
        KwUpdate,
        KwUsage,
        KwUse,
        KwUsing,
        KwUtcDate,
        KwUtcTime,
        KwUtcTimestamp,
        KwValues,
        KwVarbinary,
        KwVarchar,
        KwVarcharacter,
        KwVarying,
        KwWhen,
        KwWhere,
        KwWhile,
        KwWith,
        KwWrite,
        KwXor,
        KwYearMonth,
        KwZerofill
    }

    internal static class MySqlTokenUtils
    {
        // /** &#64; */
        // OP_AT,
        public static string KeyWordToString(this MySqlToken token)
        {
            switch (token)
            {
                case MySqlToken.KwAccessible:
                {
                    return "ACCESSIBLE";
                }

                case MySqlToken.KwAdd:
                {
                    return "ADD";
                }

                case MySqlToken.KwAll:
                {
                    return "ALL";
                }

                case MySqlToken.KwAlter:
                {
                    return "ALTER";
                }

                case MySqlToken.KwAnalyze:
                {
                    return "ANALYZE";
                }

                case MySqlToken.KwAnd:
                {
                    return "AND";
                }

                case MySqlToken.KwAs:
                {
                    return "AS";
                }

                case MySqlToken.KwAsc:
                {
                    return "ASC";
                }

                case MySqlToken.KwAsensitive:
                {
                    return "ASENSITIVE";
                }

                case MySqlToken.KwBefore:
                {
                    return "BEFORE";
                }

                case MySqlToken.KwBetween:
                {
                    return "BETWEEN";
                }

                case MySqlToken.KwBigint:
                {
                    return "BIGINT";
                }

                case MySqlToken.KwBinary:
                {
                    return "BINARY";
                }

                case MySqlToken.KwBlob:
                {
                    return "BLOB";
                }

                case MySqlToken.KwBoth:
                {
                    return "BOTH";
                }

                case MySqlToken.KwBy:
                {
                    return "BY";
                }

                case MySqlToken.KwCall:
                {
                    return "CALL";
                }

                case MySqlToken.KwCascade:
                {
                    return "CASCADE";
                }

                case MySqlToken.KwCase:
                {
                    return "CASE";
                }

                case MySqlToken.KwChange:
                {
                    return "CHANGE";
                }

                case MySqlToken.KwChar:
                {
                    return "CHAR";
                }

                case MySqlToken.KwCharacter:
                {
                    return "CHARACTER";
                }

                case MySqlToken.KwCheck:
                {
                    return "CHECK";
                }

                case MySqlToken.KwCollate:
                {
                    return "COLLATE";
                }

                case MySqlToken.KwColumn:
                {
                    return "COLUMN";
                }

                case MySqlToken.KwCondition:
                {
                    return "CONDITION";
                }

                case MySqlToken.KwConstraint:
                {
                    return "CONSTRAINT";
                }

                case MySqlToken.KwContinue:
                {
                    return "CONTINUE";
                }

                case MySqlToken.KwConvert:
                {
                    return "CONVERT";
                }

                case MySqlToken.KwCreate:
                {
                    return "CREATE";
                }

                case MySqlToken.KwCross:
                {
                    return "CROSS";
                }

                case MySqlToken.KwCurrentDate:
                {
                    return "CURRENT_DATE";
                }

                case MySqlToken.KwCurrentTime:
                {
                    return "CURRENT_TIME";
                }

                case MySqlToken.KwCurrentTimestamp:
                {
                    return "CURRENT_TIMESTAMP";
                }

                case MySqlToken.KwCurrentUser:
                {
                    return "CURRENT_USER";
                }

                case MySqlToken.KwCursor:
                {
                    return "CURSOR";
                }

                case MySqlToken.KwDatabase:
                {
                    return "DATABASE";
                }

                case MySqlToken.KwDatabases:
                {
                    return "DATABASES";
                }

                case MySqlToken.KwDayHour:
                {
                    return "DAY_HOUR";
                }

                case MySqlToken.KwDayMicrosecond:
                {
                    return "DAY_MICROSECOND";
                }

                case MySqlToken.KwDayMinute:
                {
                    return "DAY_MINUTE";
                }

                case MySqlToken.KwDaySecond:
                {
                    return "DAY_SECOND";
                }

                case MySqlToken.KwDec:
                {
                    return "DEC";
                }

                case MySqlToken.KwDecimal:
                {
                    return "DECIMAL";
                }

                case MySqlToken.KwDeclare:
                {
                    return "DECLARE";
                }

                case MySqlToken.KwDefault:
                {
                    return "DEFAULT";
                }

                case MySqlToken.KwDelayed:
                {
                    return "DELAYED";
                }

                case MySqlToken.KwDelete:
                {
                    return "DELETE";
                }

                case MySqlToken.KwDesc:
                {
                    return "DESC";
                }

                case MySqlToken.KwDescribe:
                {
                    return "DESCRIBE";
                }

                case MySqlToken.KwDeterministic:
                {
                    return "DETERMINISTIC";
                }

                case MySqlToken.KwDistinct:
                {
                    return "DISTINCT";
                }

                case MySqlToken.KwDistinctrow:
                {
                    return "DISTINCTROW";
                }

                case MySqlToken.KwDiv:
                {
                    return "DIV";
                }

                case MySqlToken.KwDouble:
                {
                    return "DOUBLE";
                }

                case MySqlToken.KwDrop:
                {
                    return "DROP";
                }

                case MySqlToken.KwDual:
                {
                    return "DUAL";
                }

                case MySqlToken.KwEach:
                {
                    return "EACH";
                }

                case MySqlToken.KwElse:
                {
                    return "ELSE";
                }

                case MySqlToken.KwElseif:
                {
                    return "ELSEIF";
                }

                case MySqlToken.KwEnclosed:
                {
                    return "ENCLOSED";
                }

                case MySqlToken.KwEscaped:
                {
                    return "ESCAPED";
                }

                case MySqlToken.KwExists:
                {
                    return "EXISTS";
                }

                case MySqlToken.KwExit:
                {
                    return "EXIT";
                }

                case MySqlToken.KwExplain:
                {
                    return "EXPLAIN";
                }

                case MySqlToken.KwFetch:
                {
                    return "FETCH";
                }

                case MySqlToken.KwFloat:
                {
                    return "FLOAT";
                }

                case MySqlToken.KwFloat4:
                {
                    return "FLOAT4";
                }

                case MySqlToken.KwFloat8:
                {
                    return "FLOAT8";
                }

                case MySqlToken.KwFor:
                {
                    return "FOR";
                }

                case MySqlToken.KwForce:
                {
                    return "FORCE";
                }

                case MySqlToken.KwForeign:
                {
                    return "FOREIGN";
                }

                case MySqlToken.KwFrom:
                {
                    return "FROM";
                }

                case MySqlToken.KwFulltext:
                {
                    return "FULLTEXT";
                }

                case MySqlToken.KwGeneral:
                {
                    return "GENERAL";
                }

                case MySqlToken.KwGrant:
                {
                    return "GRANT";
                }

                case MySqlToken.KwGroup:
                {
                    return "GROUP";
                }

                case MySqlToken.KwHaving:
                {
                    return "HAVING";
                }

                case MySqlToken.KwHighPriority:
                {
                    return "HIGH_PRIORITY";
                }

                case MySqlToken.KwHourMicrosecond:
                {
                    return "HOUR_MICROSECOND";
                }

                case MySqlToken.KwHourMinute:
                {
                    return "HOUR_MINUTE";
                }

                case MySqlToken.KwHourSecond:
                {
                    return "HOUR_SECOND";
                }

                case MySqlToken.KwIf:
                {
                    return "IF";
                }

                case MySqlToken.KwIgnore:
                {
                    return "IGNORE";
                }

                case MySqlToken.KwIgnoreServerIds:
                {
                    return "IGNORE_SERVER_IDS";
                }

                case MySqlToken.KwIn:
                {
                    return "IN";
                }

                case MySqlToken.KwIndex:
                {
                    return "INDEX";
                }

                case MySqlToken.KwInfile:
                {
                    return "INFILE";
                }

                case MySqlToken.KwInner:
                {
                    return "INNER";
                }

                case MySqlToken.KwInout:
                {
                    return "INOUT";
                }

                case MySqlToken.KwInsensitive:
                {
                    return "INSENSITIVE";
                }

                case MySqlToken.KwInsert:
                {
                    return "INSERT";
                }

                case MySqlToken.KwInt:
                {
                    return "INT";
                }

                case MySqlToken.KwInt1:
                {
                    return "INT1";
                }

                case MySqlToken.KwInt2:
                {
                    return "INT2";
                }

                case MySqlToken.KwInt3:
                {
                    return "INT3";
                }

                case MySqlToken.KwInt4:
                {
                    return "INT4";
                }

                case MySqlToken.KwInt8:
                {
                    return "INT8";
                }

                case MySqlToken.KwInteger:
                {
                    return "INTEGER";
                }

                case MySqlToken.KwInterval:
                {
                    return "INTERVAL";
                }

                case MySqlToken.KwInto:
                {
                    return "INTO";
                }

                case MySqlToken.KwIs:
                {
                    return "IS";
                }

                case MySqlToken.KwIterate:
                {
                    return "ITERATE";
                }

                case MySqlToken.KwJoin:
                {
                    return "JOIN";
                }

                case MySqlToken.KwKey:
                {
                    return "KEY";
                }

                case MySqlToken.KwKeys:
                {
                    return "KEYS";
                }

                case MySqlToken.KwKill:
                {
                    return "KILL";
                }

                case MySqlToken.KwLeading:
                {
                    return "LEADING";
                }

                case MySqlToken.KwLeave:
                {
                    return "LEAVE";
                }

                case MySqlToken.KwLeft:
                {
                    return "LEFT";
                }

                case MySqlToken.KwLike:
                {
                    return "LIKE";
                }

                case MySqlToken.KwLimit:
                {
                    return "LIMIT";
                }

                case MySqlToken.KwLinear:
                {
                    return "LINEAR";
                }

                case MySqlToken.KwLines:
                {
                    return "LINES";
                }

                case MySqlToken.KwLoad:
                {
                    return "LOAD";
                }

                case MySqlToken.KwLocaltime:
                {
                    return "LOCALTIME";
                }

                case MySqlToken.KwLocaltimestamp:
                {
                    return "LOCALTIMESTAMP";
                }

                case MySqlToken.KwLock:
                {
                    return "LOCK";
                }

                case MySqlToken.KwLong:
                {
                    return "LONG";
                }

                case MySqlToken.KwLongblob:
                {
                    return "LONGBLOB";
                }

                case MySqlToken.KwLongtext:
                {
                    return "LONGTEXT";
                }

                case MySqlToken.KwLoop:
                {
                    return "LOOP";
                }

                case MySqlToken.KwLowPriority:
                {
                    return "LOW_PRIORITY";
                }

                case MySqlToken.KwMasterHeartbeatPeriod:
                {
                    return "MASTER_HEARTBEAT_PERIOD";
                }

                case MySqlToken.KwMasterSslVerifyServerCert:
                {
                    return "MASTER_SSL_VERIFY_SERVER_CERT";
                }

                case MySqlToken.KwMatch:
                {
                    return "MATCH";
                }

                case MySqlToken.KwMaxvalue:
                {
                    return "MAXVALUE";
                }

                case MySqlToken.KwMediumblob:
                {
                    return "MEDIUMBLOB";
                }

                case MySqlToken.KwMediumint:
                {
                    return "MEDIUMINT";
                }

                case MySqlToken.KwMediumtext:
                {
                    return "MEDIUMTEXT";
                }

                case MySqlToken.KwMiddleint:
                {
                    return "MIDdlEINT";
                }

                case MySqlToken.KwMinuteMicrosecond:
                {
                    return "MINUTE_MICROSECOND";
                }

                case MySqlToken.KwMinuteSecond:
                {
                    return "MINUTE_SECOND";
                }

                case MySqlToken.KwMod:
                {
                    return "MOD";
                }

                case MySqlToken.KwModifies:
                {
                    return "MODIFIES";
                }

                case MySqlToken.KwNatural:
                {
                    return "NATURAL";
                }

                case MySqlToken.KwNot:
                {
                    return "NOT";
                }

                case MySqlToken.KwNoWriteToBinlog:
                {
                    return "NO_WRITE_TO_BINLOG";
                }

                case MySqlToken.KwNumeric:
                {
                    return "NUMERIC";
                }

                case MySqlToken.KwOn:
                {
                    return "ON";
                }

                case MySqlToken.KwOptimize:
                {
                    return "OPTIMIZE";
                }

                case MySqlToken.KwOption:
                {
                    return "OPTION";
                }

                case MySqlToken.KwOptionally:
                {
                    return "OPTIONALLY";
                }

                case MySqlToken.KwOr:
                {
                    return "OR";
                }

                case MySqlToken.KwOrder:
                {
                    return "ORDER";
                }

                case MySqlToken.KwOut:
                {
                    return "OUT";
                }

                case MySqlToken.KwOuter:
                {
                    return "OUTER";
                }

                case MySqlToken.KwOutfile:
                {
                    return "OUTFILE";
                }

                case MySqlToken.KwPrecision:
                {
                    return "PRECISION";
                }

                case MySqlToken.KwPrimary:
                {
                    return "PRIMARY";
                }

                case MySqlToken.KwProcedure:
                {
                    return "PROCEDURE";
                }

                case MySqlToken.KwPurge:
                {
                    return "PURGE";
                }

                case MySqlToken.KwRange:
                {
                    return "RANGE";
                }

                case MySqlToken.KwRead:
                {
                    return "READ";
                }

                case MySqlToken.KwReads:
                {
                    return "READS";
                }

                case MySqlToken.KwReadWrite:
                {
                    return "READ_WRITE";
                }

                case MySqlToken.KwReal:
                {
                    return "REAL";
                }

                case MySqlToken.KwReferences:
                {
                    return "REFERENCES";
                }

                case MySqlToken.KwRegexp:
                {
                    return "REGEXP";
                }

                case MySqlToken.KwRelease:
                {
                    return "RELEASE";
                }

                case MySqlToken.KwRename:
                {
                    return "RENAME";
                }

                case MySqlToken.KwRepeat:
                {
                    return "REPEAT";
                }

                case MySqlToken.KwReplace:
                {
                    return "REPLACE";
                }

                case MySqlToken.KwRequire:
                {
                    return "REQUIRE";
                }

                case MySqlToken.KwResignal:
                {
                    return "RESIGNAL";
                }

                case MySqlToken.KwRestrict:
                {
                    return "RESTRICT";
                }

                case MySqlToken.KwReturn:
                {
                    return "RETURN";
                }

                case MySqlToken.KwRevoke:
                {
                    return "REVOKE";
                }

                case MySqlToken.KwRight:
                {
                    return "RIGHT";
                }

                case MySqlToken.KwRlike:
                {
                    return "RLIKE";
                }

                case MySqlToken.KwSchema:
                {
                    return "SCHEMA";
                }

                case MySqlToken.KwSchemas:
                {
                    return "SCHEMAS";
                }

                case MySqlToken.KwSecondMicrosecond:
                {
                    return "SECOND_MICROSECOND";
                }

                case MySqlToken.KwSelect:
                {
                    return "SELECT";
                }

                case MySqlToken.KwSensitive:
                {
                    return "SENSITIVE";
                }

                case MySqlToken.KwSeparator:
                {
                    return "SEPARATOR";
                }

                case MySqlToken.KwSet:
                {
                    return "SET";
                }

                case MySqlToken.KwShow:
                {
                    return "SHOW";
                }

                case MySqlToken.KwSignal:
                {
                    return "SIGNAL";
                }

                case MySqlToken.KwSlow:
                {
                    return "SLOW";
                }

                case MySqlToken.KwSmallint:
                {
                    return "SMALLINT";
                }

                case MySqlToken.KwSpatial:
                {
                    return "SPATIAL";
                }

                case MySqlToken.KwSpecific:
                {
                    return "SPECIFIC";
                }

                case MySqlToken.KwSql:
                {
                    return "Sql";
                }

                case MySqlToken.KwSqlexception:
                {
                    return "SQLEXCEPTION";
                }

                case MySqlToken.KwSqlstate:
                {
                    return "SQLSTATE";
                }

                case MySqlToken.KwSqlwarning:
                {
                    return "SQLWARNING";
                }

                case MySqlToken.KwSqlBigResult:
                {
                    return "SQL_BIG_RESULT";
                }

                case MySqlToken.KwSqlCalcFoundRows:
                {
                    return "SQL_CALC_FOUND_ROWS";
                }

                case MySqlToken.KwSqlSmallResult:
                {
                    return "SQL_SMALL_RESULT";
                }

                case MySqlToken.KwSsl:
                {
                    return "SSL";
                }

                case MySqlToken.KwStarting:
                {
                    return "STARTING";
                }

                case MySqlToken.KwStraightJoin:
                {
                    return "STRAIGHT_JOIN";
                }

                case MySqlToken.KwTable:
                {
                    return "TABLE";
                }

                case MySqlToken.KwTerminated:
                {
                    return "TERMINATED";
                }

                case MySqlToken.KwThen:
                {
                    return "THEN";
                }

                case MySqlToken.KwTinyblob:
                {
                    return "TINYBLOB";
                }

                case MySqlToken.KwTinyint:
                {
                    return "TINYINT";
                }

                case MySqlToken.KwTinytext:
                {
                    return "TINYTEXT";
                }

                case MySqlToken.KwTo:
                {
                    return "TO";
                }

                case MySqlToken.KwTrailing:
                {
                    return "TRAILING";
                }

                case MySqlToken.KwTrigger:
                {
                    return "TRIGGER";
                }

                case MySqlToken.KwUndo:
                {
                    return "UNDO";
                }

                case MySqlToken.KwUnion:
                {
                    return "UNION";
                }

                case MySqlToken.KwUnique:
                {
                    return "UNIQUE";
                }

                case MySqlToken.KwUnlock:
                {
                    return "UNLOCK";
                }

                case MySqlToken.KwUnsigned:
                {
                    return "UNSIGNED";
                }

                case MySqlToken.KwUpdate:
                {
                    return "UPDATE";
                }

                case MySqlToken.KwUsage:
                {
                    return "USAGE";
                }

                case MySqlToken.KwUse:
                {
                    return "USE";
                }

                case MySqlToken.KwUsing:
                {
                    return "USING";
                }

                case MySqlToken.KwUtcDate:
                {
                    return "UTC_DATE";
                }

                case MySqlToken.KwUtcTime:
                {
                    return "UTC_TIME";
                }

                case MySqlToken.KwUtcTimestamp:
                {
                    return "UTC_TIMESTAMP";
                }

                case MySqlToken.KwValues:
                {
                    return "VALUES";
                }

                case MySqlToken.KwVarbinary:
                {
                    return "VARBINARY";
                }

                case MySqlToken.KwVarchar:
                {
                    return "VARCHAR";
                }

                case MySqlToken.KwVarcharacter:
                {
                    return "VARCHARACTER";
                }

                case MySqlToken.KwVarying:
                {
                    return "VARYING";
                }

                case MySqlToken.KwWhen:
                {
                    return "WHEN";
                }

                case MySqlToken.KwWhere:
                {
                    return "WHERE";
                }

                case MySqlToken.KwWhile:
                {
                    return "WHILE";
                }

                case MySqlToken.KwWith:
                {
                    return "WITH";
                }

                case MySqlToken.KwWrite:
                {
                    return "WRITE";
                }

                case MySqlToken.KwXor:
                {
                    return "XOR";
                }

                case MySqlToken.KwYearMonth:
                {
                    return "YEAR_MONTH";
                }

                case MySqlToken.KwZerofill:
                {
                    return "ZEROFILL";
                }

                default:
                {
                    throw new ArgumentException("token is not keyword: " + token);
                }
            }
        }
    }
}