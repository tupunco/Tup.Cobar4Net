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

namespace Tup.Cobar.Parser.Recognizer.Mysql
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    [System.Serializable]
    public enum MySQLToken
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

    internal static class MySQLTokenUtils
    {
        // /** &#64; */
        // OP_AT,
        public static string KeyWordToString(this MySQLToken token)
        {
            switch (token)
            {
                case MySQLToken.KwAccessible:
                    {
                        return "ACCESSIBLE";
                    }

                case MySQLToken.KwAdd:
                    {
                        return "ADD";
                    }

                case MySQLToken.KwAll:
                    {
                        return "ALL";
                    }

                case MySQLToken.KwAlter:
                    {
                        return "ALTER";
                    }

                case MySQLToken.KwAnalyze:
                    {
                        return "ANALYZE";
                    }

                case MySQLToken.KwAnd:
                    {
                        return "AND";
                    }

                case MySQLToken.KwAs:
                    {
                        return "AS";
                    }

                case MySQLToken.KwAsc:
                    {
                        return "ASC";
                    }

                case MySQLToken.KwAsensitive:
                    {
                        return "ASENSITIVE";
                    }

                case MySQLToken.KwBefore:
                    {
                        return "BEFORE";
                    }

                case MySQLToken.KwBetween:
                    {
                        return "BETWEEN";
                    }

                case MySQLToken.KwBigint:
                    {
                        return "BIGINT";
                    }

                case MySQLToken.KwBinary:
                    {
                        return "BINARY";
                    }

                case MySQLToken.KwBlob:
                    {
                        return "BLOB";
                    }

                case MySQLToken.KwBoth:
                    {
                        return "BOTH";
                    }

                case MySQLToken.KwBy:
                    {
                        return "BY";
                    }

                case MySQLToken.KwCall:
                    {
                        return "CALL";
                    }

                case MySQLToken.KwCascade:
                    {
                        return "CASCADE";
                    }

                case MySQLToken.KwCase:
                    {
                        return "CASE";
                    }

                case MySQLToken.KwChange:
                    {
                        return "CHANGE";
                    }

                case MySQLToken.KwChar:
                    {
                        return "CHAR";
                    }

                case MySQLToken.KwCharacter:
                    {
                        return "CHARACTER";
                    }

                case MySQLToken.KwCheck:
                    {
                        return "CHECK";
                    }

                case MySQLToken.KwCollate:
                    {
                        return "COLLATE";
                    }

                case MySQLToken.KwColumn:
                    {
                        return "COLUMN";
                    }

                case MySQLToken.KwCondition:
                    {
                        return "CONDITION";
                    }

                case MySQLToken.KwConstraint:
                    {
                        return "CONSTRAINT";
                    }

                case MySQLToken.KwContinue:
                    {
                        return "CONTINUE";
                    }

                case MySQLToken.KwConvert:
                    {
                        return "CONVERT";
                    }

                case MySQLToken.KwCreate:
                    {
                        return "CREATE";
                    }

                case MySQLToken.KwCross:
                    {
                        return "CROSS";
                    }

                case MySQLToken.KwCurrentDate:
                    {
                        return "CURRENT_DATE";
                    }

                case MySQLToken.KwCurrentTime:
                    {
                        return "CURRENT_TIME";
                    }

                case MySQLToken.KwCurrentTimestamp:
                    {
                        return "CURRENT_TIMESTAMP";
                    }

                case MySQLToken.KwCurrentUser:
                    {
                        return "CURRENT_USER";
                    }

                case MySQLToken.KwCursor:
                    {
                        return "CURSOR";
                    }

                case MySQLToken.KwDatabase:
                    {
                        return "DATABASE";
                    }

                case MySQLToken.KwDatabases:
                    {
                        return "DATABASES";
                    }

                case MySQLToken.KwDayHour:
                    {
                        return "DAY_HOUR";
                    }

                case MySQLToken.KwDayMicrosecond:
                    {
                        return "DAY_MICROSECOND";
                    }

                case MySQLToken.KwDayMinute:
                    {
                        return "DAY_MINUTE";
                    }

                case MySQLToken.KwDaySecond:
                    {
                        return "DAY_SECOND";
                    }

                case MySQLToken.KwDec:
                    {
                        return "DEC";
                    }

                case MySQLToken.KwDecimal:
                    {
                        return "DECIMAL";
                    }

                case MySQLToken.KwDeclare:
                    {
                        return "DECLARE";
                    }

                case MySQLToken.KwDefault:
                    {
                        return "DEFAULT";
                    }

                case MySQLToken.KwDelayed:
                    {
                        return "DELAYED";
                    }

                case MySQLToken.KwDelete:
                    {
                        return "DELETE";
                    }

                case MySQLToken.KwDesc:
                    {
                        return "DESC";
                    }

                case MySQLToken.KwDescribe:
                    {
                        return "DESCRIBE";
                    }

                case MySQLToken.KwDeterministic:
                    {
                        return "DETERMINISTIC";
                    }

                case MySQLToken.KwDistinct:
                    {
                        return "DISTINCT";
                    }

                case MySQLToken.KwDistinctrow:
                    {
                        return "DISTINCTROW";
                    }

                case MySQLToken.KwDiv:
                    {
                        return "DIV";
                    }

                case MySQLToken.KwDouble:
                    {
                        return "DOUBLE";
                    }

                case MySQLToken.KwDrop:
                    {
                        return "DROP";
                    }

                case MySQLToken.KwDual:
                    {
                        return "DUAL";
                    }

                case MySQLToken.KwEach:
                    {
                        return "EACH";
                    }

                case MySQLToken.KwElse:
                    {
                        return "ELSE";
                    }

                case MySQLToken.KwElseif:
                    {
                        return "ELSEIF";
                    }

                case MySQLToken.KwEnclosed:
                    {
                        return "ENCLOSED";
                    }

                case MySQLToken.KwEscaped:
                    {
                        return "ESCAPED";
                    }

                case MySQLToken.KwExists:
                    {
                        return "EXISTS";
                    }

                case MySQLToken.KwExit:
                    {
                        return "EXIT";
                    }

                case MySQLToken.KwExplain:
                    {
                        return "EXPLAIN";
                    }

                case MySQLToken.KwFetch:
                    {
                        return "FETCH";
                    }

                case MySQLToken.KwFloat:
                    {
                        return "FLOAT";
                    }

                case MySQLToken.KwFloat4:
                    {
                        return "FLOAT4";
                    }

                case MySQLToken.KwFloat8:
                    {
                        return "FLOAT8";
                    }

                case MySQLToken.KwFor:
                    {
                        return "FOR";
                    }

                case MySQLToken.KwForce:
                    {
                        return "FORCE";
                    }

                case MySQLToken.KwForeign:
                    {
                        return "FOREIGN";
                    }

                case MySQLToken.KwFrom:
                    {
                        return "FROM";
                    }

                case MySQLToken.KwFulltext:
                    {
                        return "FULLTEXT";
                    }

                case MySQLToken.KwGeneral:
                    {
                        return "GENERAL";
                    }

                case MySQLToken.KwGrant:
                    {
                        return "GRANT";
                    }

                case MySQLToken.KwGroup:
                    {
                        return "GROUP";
                    }

                case MySQLToken.KwHaving:
                    {
                        return "HAVING";
                    }

                case MySQLToken.KwHighPriority:
                    {
                        return "HIGH_PRIORITY";
                    }

                case MySQLToken.KwHourMicrosecond:
                    {
                        return "HOUR_MICROSECOND";
                    }

                case MySQLToken.KwHourMinute:
                    {
                        return "HOUR_MINUTE";
                    }

                case MySQLToken.KwHourSecond:
                    {
                        return "HOUR_SECOND";
                    }

                case MySQLToken.KwIf:
                    {
                        return "IF";
                    }

                case MySQLToken.KwIgnore:
                    {
                        return "IGNORE";
                    }

                case MySQLToken.KwIgnoreServerIds:
                    {
                        return "IGNORE_SERVER_IDS";
                    }

                case MySQLToken.KwIn:
                    {
                        return "IN";
                    }

                case MySQLToken.KwIndex:
                    {
                        return "INDEX";
                    }

                case MySQLToken.KwInfile:
                    {
                        return "INFILE";
                    }

                case MySQLToken.KwInner:
                    {
                        return "INNER";
                    }

                case MySQLToken.KwInout:
                    {
                        return "INOUT";
                    }

                case MySQLToken.KwInsensitive:
                    {
                        return "INSENSITIVE";
                    }

                case MySQLToken.KwInsert:
                    {
                        return "INSERT";
                    }

                case MySQLToken.KwInt:
                    {
                        return "INT";
                    }

                case MySQLToken.KwInt1:
                    {
                        return "INT1";
                    }

                case MySQLToken.KwInt2:
                    {
                        return "INT2";
                    }

                case MySQLToken.KwInt3:
                    {
                        return "INT3";
                    }

                case MySQLToken.KwInt4:
                    {
                        return "INT4";
                    }

                case MySQLToken.KwInt8:
                    {
                        return "INT8";
                    }

                case MySQLToken.KwInteger:
                    {
                        return "INTEGER";
                    }

                case MySQLToken.KwInterval:
                    {
                        return "INTERVAL";
                    }

                case MySQLToken.KwInto:
                    {
                        return "INTO";
                    }

                case MySQLToken.KwIs:
                    {
                        return "IS";
                    }

                case MySQLToken.KwIterate:
                    {
                        return "ITERATE";
                    }

                case MySQLToken.KwJoin:
                    {
                        return "JOIN";
                    }

                case MySQLToken.KwKey:
                    {
                        return "KEY";
                    }

                case MySQLToken.KwKeys:
                    {
                        return "KEYS";
                    }

                case MySQLToken.KwKill:
                    {
                        return "KILL";
                    }

                case MySQLToken.KwLeading:
                    {
                        return "LEADING";
                    }

                case MySQLToken.KwLeave:
                    {
                        return "LEAVE";
                    }

                case MySQLToken.KwLeft:
                    {
                        return "LEFT";
                    }

                case MySQLToken.KwLike:
                    {
                        return "LIKE";
                    }

                case MySQLToken.KwLimit:
                    {
                        return "LIMIT";
                    }

                case MySQLToken.KwLinear:
                    {
                        return "LINEAR";
                    }

                case MySQLToken.KwLines:
                    {
                        return "LINES";
                    }

                case MySQLToken.KwLoad:
                    {
                        return "LOAD";
                    }

                case MySQLToken.KwLocaltime:
                    {
                        return "LOCALTIME";
                    }

                case MySQLToken.KwLocaltimestamp:
                    {
                        return "LOCALTIMESTAMP";
                    }

                case MySQLToken.KwLock:
                    {
                        return "LOCK";
                    }

                case MySQLToken.KwLong:
                    {
                        return "LONG";
                    }

                case MySQLToken.KwLongblob:
                    {
                        return "LONGBLOB";
                    }

                case MySQLToken.KwLongtext:
                    {
                        return "LONGTEXT";
                    }

                case MySQLToken.KwLoop:
                    {
                        return "LOOP";
                    }

                case MySQLToken.KwLowPriority:
                    {
                        return "LOW_PRIORITY";
                    }

                case MySQLToken.KwMasterHeartbeatPeriod:
                    {
                        return "MASTER_HEARTBEAT_PERIOD";
                    }

                case MySQLToken.KwMasterSslVerifyServerCert:
                    {
                        return "MASTER_SSL_VERIFY_SERVER_CERT";
                    }

                case MySQLToken.KwMatch:
                    {
                        return "MATCH";
                    }

                case MySQLToken.KwMaxvalue:
                    {
                        return "MAXVALUE";
                    }

                case MySQLToken.KwMediumblob:
                    {
                        return "MEDIUMBLOB";
                    }

                case MySQLToken.KwMediumint:
                    {
                        return "MEDIUMINT";
                    }

                case MySQLToken.KwMediumtext:
                    {
                        return "MEDIUMTEXT";
                    }

                case MySQLToken.KwMiddleint:
                    {
                        return "MIDDLEINT";
                    }

                case MySQLToken.KwMinuteMicrosecond:
                    {
                        return "MINUTE_MICROSECOND";
                    }

                case MySQLToken.KwMinuteSecond:
                    {
                        return "MINUTE_SECOND";
                    }

                case MySQLToken.KwMod:
                    {
                        return "MOD";
                    }

                case MySQLToken.KwModifies:
                    {
                        return "MODIFIES";
                    }

                case MySQLToken.KwNatural:
                    {
                        return "NATURAL";
                    }

                case MySQLToken.KwNot:
                    {
                        return "NOT";
                    }

                case MySQLToken.KwNoWriteToBinlog:
                    {
                        return "NO_WRITE_TO_BINLOG";
                    }

                case MySQLToken.KwNumeric:
                    {
                        return "NUMERIC";
                    }

                case MySQLToken.KwOn:
                    {
                        return "ON";
                    }

                case MySQLToken.KwOptimize:
                    {
                        return "OPTIMIZE";
                    }

                case MySQLToken.KwOption:
                    {
                        return "OPTION";
                    }

                case MySQLToken.KwOptionally:
                    {
                        return "OPTIONALLY";
                    }

                case MySQLToken.KwOr:
                    {
                        return "OR";
                    }

                case MySQLToken.KwOrder:
                    {
                        return "ORDER";
                    }

                case MySQLToken.KwOut:
                    {
                        return "OUT";
                    }

                case MySQLToken.KwOuter:
                    {
                        return "OUTER";
                    }

                case MySQLToken.KwOutfile:
                    {
                        return "OUTFILE";
                    }

                case MySQLToken.KwPrecision:
                    {
                        return "PRECISION";
                    }

                case MySQLToken.KwPrimary:
                    {
                        return "PRIMARY";
                    }

                case MySQLToken.KwProcedure:
                    {
                        return "PROCEDURE";
                    }

                case MySQLToken.KwPurge:
                    {
                        return "PURGE";
                    }

                case MySQLToken.KwRange:
                    {
                        return "RANGE";
                    }

                case MySQLToken.KwRead:
                    {
                        return "READ";
                    }

                case MySQLToken.KwReads:
                    {
                        return "READS";
                    }

                case MySQLToken.KwReadWrite:
                    {
                        return "READ_WRITE";
                    }

                case MySQLToken.KwReal:
                    {
                        return "REAL";
                    }

                case MySQLToken.KwReferences:
                    {
                        return "REFERENCES";
                    }

                case MySQLToken.KwRegexp:
                    {
                        return "REGEXP";
                    }

                case MySQLToken.KwRelease:
                    {
                        return "RELEASE";
                    }

                case MySQLToken.KwRename:
                    {
                        return "RENAME";
                    }

                case MySQLToken.KwRepeat:
                    {
                        return "REPEAT";
                    }

                case MySQLToken.KwReplace:
                    {
                        return "REPLACE";
                    }

                case MySQLToken.KwRequire:
                    {
                        return "REQUIRE";
                    }

                case MySQLToken.KwResignal:
                    {
                        return "RESIGNAL";
                    }

                case MySQLToken.KwRestrict:
                    {
                        return "RESTRICT";
                    }

                case MySQLToken.KwReturn:
                    {
                        return "RETURN";
                    }

                case MySQLToken.KwRevoke:
                    {
                        return "REVOKE";
                    }

                case MySQLToken.KwRight:
                    {
                        return "RIGHT";
                    }

                case MySQLToken.KwRlike:
                    {
                        return "RLIKE";
                    }

                case MySQLToken.KwSchema:
                    {
                        return "SCHEMA";
                    }

                case MySQLToken.KwSchemas:
                    {
                        return "SCHEMAS";
                    }

                case MySQLToken.KwSecondMicrosecond:
                    {
                        return "SECOND_MICROSECOND";
                    }

                case MySQLToken.KwSelect:
                    {
                        return "SELECT";
                    }

                case MySQLToken.KwSensitive:
                    {
                        return "SENSITIVE";
                    }

                case MySQLToken.KwSeparator:
                    {
                        return "SEPARATOR";
                    }

                case MySQLToken.KwSet:
                    {
                        return "SET";
                    }

                case MySQLToken.KwShow:
                    {
                        return "SHOW";
                    }

                case MySQLToken.KwSignal:
                    {
                        return "SIGNAL";
                    }

                case MySQLToken.KwSlow:
                    {
                        return "SLOW";
                    }

                case MySQLToken.KwSmallint:
                    {
                        return "SMALLINT";
                    }

                case MySQLToken.KwSpatial:
                    {
                        return "SPATIAL";
                    }

                case MySQLToken.KwSpecific:
                    {
                        return "SPECIFIC";
                    }

                case MySQLToken.KwSql:
                    {
                        return "SQL";
                    }

                case MySQLToken.KwSqlexception:
                    {
                        return "SQLEXCEPTION";
                    }

                case MySQLToken.KwSqlstate:
                    {
                        return "SQLSTATE";
                    }

                case MySQLToken.KwSqlwarning:
                    {
                        return "SQLWARNING";
                    }

                case MySQLToken.KwSqlBigResult:
                    {
                        return "SQL_BIG_RESULT";
                    }

                case MySQLToken.KwSqlCalcFoundRows:
                    {
                        return "SQL_CALC_FOUND_ROWS";
                    }

                case MySQLToken.KwSqlSmallResult:
                    {
                        return "SQL_SMALL_RESULT";
                    }

                case MySQLToken.KwSsl:
                    {
                        return "SSL";
                    }

                case MySQLToken.KwStarting:
                    {
                        return "STARTING";
                    }

                case MySQLToken.KwStraightJoin:
                    {
                        return "STRAIGHT_JOIN";
                    }

                case MySQLToken.KwTable:
                    {
                        return "TABLE";
                    }

                case MySQLToken.KwTerminated:
                    {
                        return "TERMINATED";
                    }

                case MySQLToken.KwThen:
                    {
                        return "THEN";
                    }

                case MySQLToken.KwTinyblob:
                    {
                        return "TINYBLOB";
                    }

                case MySQLToken.KwTinyint:
                    {
                        return "TINYINT";
                    }

                case MySQLToken.KwTinytext:
                    {
                        return "TINYTEXT";
                    }

                case MySQLToken.KwTo:
                    {
                        return "TO";
                    }

                case MySQLToken.KwTrailing:
                    {
                        return "TRAILING";
                    }

                case MySQLToken.KwTrigger:
                    {
                        return "TRIGGER";
                    }

                case MySQLToken.KwUndo:
                    {
                        return "UNDO";
                    }

                case MySQLToken.KwUnion:
                    {
                        return "UNION";
                    }

                case MySQLToken.KwUnique:
                    {
                        return "UNIQUE";
                    }

                case MySQLToken.KwUnlock:
                    {
                        return "UNLOCK";
                    }

                case MySQLToken.KwUnsigned:
                    {
                        return "UNSIGNED";
                    }

                case MySQLToken.KwUpdate:
                    {
                        return "UPDATE";
                    }

                case MySQLToken.KwUsage:
                    {
                        return "USAGE";
                    }

                case MySQLToken.KwUse:
                    {
                        return "USE";
                    }

                case MySQLToken.KwUsing:
                    {
                        return "USING";
                    }

                case MySQLToken.KwUtcDate:
                    {
                        return "UTC_DATE";
                    }

                case MySQLToken.KwUtcTime:
                    {
                        return "UTC_TIME";
                    }

                case MySQLToken.KwUtcTimestamp:
                    {
                        return "UTC_TIMESTAMP";
                    }

                case MySQLToken.KwValues:
                    {
                        return "VALUES";
                    }

                case MySQLToken.KwVarbinary:
                    {
                        return "VARBINARY";
                    }

                case MySQLToken.KwVarchar:
                    {
                        return "VARCHAR";
                    }

                case MySQLToken.KwVarcharacter:
                    {
                        return "VARCHARACTER";
                    }

                case MySQLToken.KwVarying:
                    {
                        return "VARYING";
                    }

                case MySQLToken.KwWhen:
                    {
                        return "WHEN";
                    }

                case MySQLToken.KwWhere:
                    {
                        return "WHERE";
                    }

                case MySQLToken.KwWhile:
                    {
                        return "WHILE";
                    }

                case MySQLToken.KwWith:
                    {
                        return "WITH";
                    }

                case MySQLToken.KwWrite:
                    {
                        return "WRITE";
                    }

                case MySQLToken.KwXor:
                    {
                        return "XOR";
                    }

                case MySQLToken.KwYearMonth:
                    {
                        return "YEAR_MONTH";
                    }

                case MySQLToken.KwZerofill:
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