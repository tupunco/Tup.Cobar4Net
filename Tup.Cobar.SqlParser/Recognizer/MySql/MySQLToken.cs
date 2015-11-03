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

namespace Tup.Cobar.SqlParser.Recognizer.MySql
{
    /**
     * @author <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
     */
    public enum MySQLToken
    {
        None = 0,

        EOF,
        PLACE_HOLDER,
        IDENTIFIER,
        SYS_VAR,
        USR_VAR,

        /** number composed purely of digit */
        LITERAL_NUM_PURE_DIGIT,
        /** number composed of digit mixed with <code>.</code> or <code>e</code> */
        LITERAL_NUM_MIX_DIGIT,
        LITERAL_HEX,
        LITERAL_BIT,
        LITERAL_CHARS,
        LITERAL_NCHARS,
        LITERAL_NULL,
        LITERAL_BOOL_TRUE,
        LITERAL_BOOL_FALSE,

        /** ? */
        QUESTION_MARK,

        /** ( */
        PUNC_LEFT_PAREN,
        /** ) */
        PUNC_RIGHT_PAREN,
        /** { */
        PUNC_LEFT_BRACE,
        /** } */
        PUNC_RIGHT_BRACE,
        /** [ */
        PUNC_LEFT_BRACKET,
        /** ] */
        PUNC_RIGHT_BRACKET,
        /** ; */
        PUNC_SEMICOLON,
        /** , */
        PUNC_COMMA,
        /** , */
        PUNC_DOT,
        /** : */
        PUNC_COLON,
        /** <code>*</code><code>/</code> */
        PUNC_C_STYLE_COMMENT_END,

        // /** &#64; */
        // OP_AT,
        /** = */
        OP_EQUALS,
        /** > */
        OP_GREATER_THAN,
        /** < */
        OP_LESS_THAN,
        /** ! */
        OP_EXCLAMATION,
        /** ~ */
        OP_TILDE,
        /** + */
        OP_PLUS,
        /** - */
        OP_MINUS,
        /** * */
        OP_ASTERISK,
        /** / */
        OP_SLASH,
        /** & */
        OP_AMPERSAND,
        /** | */
        OP_VERTICAL_BAR,
        /** ^ */
        OP_CARET,
        /** % */
        OP_PERCENT,
        /** := */
        OP_ASSIGN,
        /** <= */
        OP_LESS_OR_EQUALS,
        /** <> */
        OP_LESS_OR_GREATER,
        /** >= */
        OP_GREATER_OR_EQUALS,
        /** != */
        OP_NOT_EQUALS,
        /** && */
        OP_LOGICAL_AND,
        /** || */
        OP_LOGICAL_OR,
        /** << */
        OP_LEFT_SHIFT,
        /** >> */
        OP_RIGHT_SHIFT,
        /** <=> */
        OP_NULL_SAFE_EQUALS,

        KW_ACCESSIBLE,
        KW_ADD,
        KW_ALL,
        KW_ALTER,
        KW_ANALYZE,
        KW_AND,
        KW_AS,
        KW_ASC,
        KW_ASENSITIVE,
        KW_BEFORE,
        KW_BETWEEN,
        KW_BIGINT,
        KW_BINARY,
        KW_BLOB,
        KW_BOTH,
        KW_BY,
        KW_CALL,
        KW_CASCADE,
        KW_CASE,
        KW_CHANGE,
        KW_CHAR,
        KW_CHARACTER,
        KW_CHECK,
        KW_COLLATE,
        KW_COLUMN,
        KW_CONDITION,
        KW_CONSTRAINT,
        KW_CONTINUE,
        KW_CONVERT,
        KW_CREATE,
        KW_CROSS,
        KW_CURRENT_DATE,
        KW_CURRENT_TIME,
        KW_CURRENT_TIMESTAMP,
        KW_CURRENT_USER,
        KW_CURSOR,
        KW_DATABASE,
        KW_DATABASES,
        KW_DAY_HOUR,
        KW_DAY_MICROSECOND,
        KW_DAY_MINUTE,
        KW_DAY_SECOND,
        KW_DEC,
        KW_DECIMAL,
        KW_DECLARE,
        KW_DEFAULT,
        KW_DELAYED,
        KW_DELETE,
        KW_DESC,
        KW_DESCRIBE,
        KW_DETERMINISTIC,
        KW_DISTINCT,
        KW_DISTINCTROW,
        KW_DIV,
        KW_DOUBLE,
        KW_DROP,
        KW_DUAL,
        KW_EACH,
        KW_ELSE,
        KW_ELSEIF,
        KW_ENCLOSED,
        KW_ESCAPED,
        KW_EXISTS,
        KW_EXIT,
        KW_EXPLAIN,
        KW_FETCH,
        KW_FLOAT,
        KW_FLOAT4,
        KW_FLOAT8,
        KW_FOR,
        KW_FORCE,
        KW_FOREIGN,
        KW_FROM,
        KW_FULLTEXT,
        KW_GENERAL,
        KW_GRANT,
        KW_GROUP,
        KW_HAVING,
        KW_HIGH_PRIORITY,
        KW_HOUR_MICROSECOND,
        KW_HOUR_MINUTE,
        KW_HOUR_SECOND,
        KW_IF,
        KW_IGNORE,
        KW_IGNORE_SERVER_IDS,
        KW_IN,
        KW_INDEX,
        KW_INFILE,
        KW_INNER,
        KW_INOUT,
        KW_INSENSITIVE,
        KW_INSERT,
        KW_INT,
        KW_INT1,
        KW_INT2,
        KW_INT3,
        KW_INT4,
        KW_INT8,
        KW_INTEGER,
        KW_INTERVAL,
        KW_INTO,
        KW_IS,
        KW_ITERATE,
        KW_JOIN,
        KW_KEY,
        KW_KEYS,
        KW_KILL,
        KW_LEADING,
        KW_LEAVE,
        KW_LEFT,
        KW_LIKE,
        KW_LIMIT,
        KW_LINEAR,
        KW_LINES,
        KW_LOAD,
        KW_LOCALTIME,
        KW_LOCALTIMESTAMP,
        KW_LOCK,
        KW_LONG,
        KW_LONGBLOB,
        KW_LONGTEXT,
        KW_LOOP,
        KW_LOW_PRIORITY,
        KW_MASTER_HEARTBEAT_PERIOD,
        KW_MASTER_SSL_VERIFY_SERVER_CERT,
        KW_MATCH,
        KW_MAXVALUE,
        KW_MEDIUMBLOB,
        KW_MEDIUMINT,
        KW_MEDIUMTEXT,
        KW_MIDDLEINT,
        KW_MINUTE_MICROSECOND,
        KW_MINUTE_SECOND,
        KW_MOD,
        KW_MODIFIES,
        KW_NATURAL,
        KW_NOT,
        KW_NO_WRITE_TO_BINLOG,
        KW_NUMERIC,
        KW_ON,
        KW_OPTIMIZE,
        KW_OPTION,
        KW_OPTIONALLY,
        KW_OR,
        KW_ORDER,
        KW_OUT,
        KW_OUTER,
        KW_OUTFILE,
        KW_PRECISION,
        KW_PRIMARY,
        KW_PROCEDURE,
        KW_PURGE,
        KW_RANGE,
        KW_READ,
        KW_READS,
        KW_READ_WRITE,
        KW_REAL,
        KW_REFERENCES,
        KW_REGEXP,
        KW_RELEASE,
        KW_RENAME,
        KW_REPEAT,
        KW_REPLACE,
        KW_REQUIRE,
        KW_RESIGNAL,
        KW_RESTRICT,
        KW_RETURN,
        KW_REVOKE,
        KW_RIGHT,
        KW_RLIKE,
        KW_SCHEMA,
        KW_SCHEMAS,
        KW_SECOND_MICROSECOND,
        KW_SELECT,
        KW_SENSITIVE,
        KW_SEPARATOR,
        KW_SET,
        KW_SHOW,
        KW_SIGNAL,
        KW_SLOW,
        KW_SMALLINT,
        KW_SPATIAL,
        KW_SPECIFIC,
        KW_SQL,
        KW_SQLEXCEPTION,
        KW_SQLSTATE,
        KW_SQLWARNING,
        KW_SQL_BIG_RESULT,
        KW_SQL_CALC_FOUND_ROWS,
        KW_SQL_SMALL_RESULT,
        KW_SSL,
        KW_STARTING,
        KW_STRAIGHT_JOIN,
        KW_TABLE,
        KW_TERMINATED,
        KW_THEN,
        KW_TINYBLOB,
        KW_TINYINT,
        KW_TINYTEXT,
        KW_TO,
        KW_TRAILING,
        KW_TRIGGER,
        KW_UNDO,
        KW_UNION,
        KW_UNIQUE,
        KW_UNLOCK,
        KW_UNSIGNED,
        KW_UPDATE,
        KW_USAGE,
        KW_USE,
        KW_USING,
        KW_UTC_DATE,
        KW_UTC_TIME,
        KW_UTC_TIMESTAMP,
        KW_VALUES,
        KW_VARBINARY,
        KW_VARCHAR,
        KW_VARCHARACTER,
        KW_VARYING,
        KW_WHEN,
        KW_WHERE,
        KW_WHILE,
        KW_WITH,
        KW_WRITE,
        KW_XOR,
        KW_YEAR_MONTH,
        KW_ZEROFILL
    }

    /// <summary>
    /// MySQLToken Utils
    /// </summary>
    internal static class MySQLTokenUtils
    {
        /// <summary>
        /// keyWordToString
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static string keyWordToString(MySQLToken token)
        {
            switch (token)
            {
                case MySQLToken.KW_ACCESSIBLE:
                    return "ACCESSIBLE";
                case MySQLToken.KW_ADD:
                    return "ADD";
                case MySQLToken.KW_ALL:
                    return "ALL";
                case MySQLToken.KW_ALTER:
                    return "ALTER";
                case MySQLToken.KW_ANALYZE:
                    return "ANALYZE";
                case MySQLToken.KW_AND:
                    return "AND";
                case MySQLToken.KW_AS:
                    return "AS";
                case MySQLToken.KW_ASC:
                    return "ASC";
                case MySQLToken.KW_ASENSITIVE:
                    return "ASENSITIVE";
                case MySQLToken.KW_BEFORE:
                    return "BEFORE";
                case MySQLToken.KW_BETWEEN:
                    return "BETWEEN";
                case MySQLToken.KW_BIGINT:
                    return "BIGINT";
                case MySQLToken.KW_BINARY:
                    return "BINARY";
                case MySQLToken.KW_BLOB:
                    return "BLOB";
                case MySQLToken.KW_BOTH:
                    return "BOTH";
                case MySQLToken.KW_BY:
                    return "BY";
                case MySQLToken.KW_CALL:
                    return "CALL";
                case MySQLToken.KW_CASCADE:
                    return "CASCADE";
                case MySQLToken.KW_CASE:
                    return "CASE";
                case MySQLToken.KW_CHANGE:
                    return "CHANGE";
                case MySQLToken.KW_CHAR:
                    return "CHAR";
                case MySQLToken.KW_CHARACTER:
                    return "CHARACTER";
                case MySQLToken.KW_CHECK:
                    return "CHECK";
                case MySQLToken.KW_COLLATE:
                    return "COLLATE";
                case MySQLToken.KW_COLUMN:
                    return "COLUMN";
                case MySQLToken.KW_CONDITION:
                    return "CONDITION";
                case MySQLToken.KW_CONSTRAINT:
                    return "CONSTRAINT";
                case MySQLToken.KW_CONTINUE:
                    return "CONTINUE";
                case MySQLToken.KW_CONVERT:
                    return "CONVERT";
                case MySQLToken.KW_CREATE:
                    return "CREATE";
                case MySQLToken.KW_CROSS:
                    return "CROSS";
                case MySQLToken.KW_CURRENT_DATE:
                    return "CURRENT_DATE";
                case MySQLToken.KW_CURRENT_TIME:
                    return "CURRENT_TIME";
                case MySQLToken.KW_CURRENT_TIMESTAMP:
                    return "CURRENT_TIMESTAMP";
                case MySQLToken.KW_CURRENT_USER:
                    return "CURRENT_USER";
                case MySQLToken.KW_CURSOR:
                    return "CURSOR";
                case MySQLToken.KW_DATABASE:
                    return "DATABASE";
                case MySQLToken.KW_DATABASES:
                    return "DATABASES";
                case MySQLToken.KW_DAY_HOUR:
                    return "DAY_HOUR";
                case MySQLToken.KW_DAY_MICROSECOND:
                    return "DAY_MICROSECOND";
                case MySQLToken.KW_DAY_MINUTE:
                    return "DAY_MINUTE";
                case MySQLToken.KW_DAY_SECOND:
                    return "DAY_SECOND";
                case MySQLToken.KW_DEC:
                    return "DEC";
                case MySQLToken.KW_DECIMAL:
                    return "DECIMAL";
                case MySQLToken.KW_DECLARE:
                    return "DECLARE";
                case MySQLToken.KW_DEFAULT:
                    return "DEFAULT";
                case MySQLToken.KW_DELAYED:
                    return "DELAYED";
                case MySQLToken.KW_DELETE:
                    return "DELETE";
                case MySQLToken.KW_DESC:
                    return "DESC";
                case MySQLToken.KW_DESCRIBE:
                    return "DESCRIBE";
                case MySQLToken.KW_DETERMINISTIC:
                    return "DETERMINISTIC";
                case MySQLToken.KW_DISTINCT:
                    return "DISTINCT";
                case MySQLToken.KW_DISTINCTROW:
                    return "DISTINCTROW";
                case MySQLToken.KW_DIV:
                    return "DIV";
                case MySQLToken.KW_DOUBLE:
                    return "DOUBLE";
                case MySQLToken.KW_DROP:
                    return "DROP";
                case MySQLToken.KW_DUAL:
                    return "DUAL";
                case MySQLToken.KW_EACH:
                    return "EACH";
                case MySQLToken.KW_ELSE:
                    return "ELSE";
                case MySQLToken.KW_ELSEIF:
                    return "ELSEIF";
                case MySQLToken.KW_ENCLOSED:
                    return "ENCLOSED";
                case MySQLToken.KW_ESCAPED:
                    return "ESCAPED";
                case MySQLToken.KW_EXISTS:
                    return "EXISTS";
                case MySQLToken.KW_EXIT:
                    return "EXIT";
                case MySQLToken.KW_EXPLAIN:
                    return "EXPLAIN";
                case MySQLToken.KW_FETCH:
                    return "FETCH";
                case MySQLToken.KW_FLOAT:
                    return "FLOAT";
                case MySQLToken.KW_FLOAT4:
                    return "FLOAT4";
                case MySQLToken.KW_FLOAT8:
                    return "FLOAT8";
                case MySQLToken.KW_FOR:
                    return "FOR";
                case MySQLToken.KW_FORCE:
                    return "FORCE";
                case MySQLToken.KW_FOREIGN:
                    return "FOREIGN";
                case MySQLToken.KW_FROM:
                    return "FROM";
                case MySQLToken.KW_FULLTEXT:
                    return "FULLTEXT";
                case MySQLToken.KW_GENERAL:
                    return "GENERAL";
                case MySQLToken.KW_GRANT:
                    return "GRANT";
                case MySQLToken.KW_GROUP:
                    return "GROUP";
                case MySQLToken.KW_HAVING:
                    return "HAVING";
                case MySQLToken.KW_HIGH_PRIORITY:
                    return "HIGH_PRIORITY";
                case MySQLToken.KW_HOUR_MICROSECOND:
                    return "HOUR_MICROSECOND";
                case MySQLToken.KW_HOUR_MINUTE:
                    return "HOUR_MINUTE";
                case MySQLToken.KW_HOUR_SECOND:
                    return "HOUR_SECOND";
                case MySQLToken.KW_IF:
                    return "IF";
                case MySQLToken.KW_IGNORE:
                    return "IGNORE";
                case MySQLToken.KW_IGNORE_SERVER_IDS:
                    return "IGNORE_SERVER_IDS";
                case MySQLToken.KW_IN:
                    return "IN";
                case MySQLToken.KW_INDEX:
                    return "INDEX";
                case MySQLToken.KW_INFILE:
                    return "INFILE";
                case MySQLToken.KW_INNER:
                    return "INNER";
                case MySQLToken.KW_INOUT:
                    return "INOUT";
                case MySQLToken.KW_INSENSITIVE:
                    return "INSENSITIVE";
                case MySQLToken.KW_INSERT:
                    return "INSERT";
                case MySQLToken.KW_INT:
                    return "INT";
                case MySQLToken.KW_INT1:
                    return "INT1";
                case MySQLToken.KW_INT2:
                    return "INT2";
                case MySQLToken.KW_INT3:
                    return "INT3";
                case MySQLToken.KW_INT4:
                    return "INT4";
                case MySQLToken.KW_INT8:
                    return "INT8";
                case MySQLToken.KW_INTEGER:
                    return "INTEGER";
                case MySQLToken.KW_INTERVAL:
                    return "INTERVAL";
                case MySQLToken.KW_INTO:
                    return "INTO";
                case MySQLToken.KW_IS:
                    return "IS";
                case MySQLToken.KW_ITERATE:
                    return "ITERATE";
                case MySQLToken.KW_JOIN:
                    return "JOIN";
                case MySQLToken.KW_KEY:
                    return "KEY";
                case MySQLToken.KW_KEYS:
                    return "KEYS";
                case MySQLToken.KW_KILL:
                    return "KILL";
                case MySQLToken.KW_LEADING:
                    return "LEADING";
                case MySQLToken.KW_LEAVE:
                    return "LEAVE";
                case MySQLToken.KW_LEFT:
                    return "LEFT";
                case MySQLToken.KW_LIKE:
                    return "LIKE";
                case MySQLToken.KW_LIMIT:
                    return "LIMIT";
                case MySQLToken.KW_LINEAR:
                    return "LINEAR";
                case MySQLToken.KW_LINES:
                    return "LINES";
                case MySQLToken.KW_LOAD:
                    return "LOAD";
                case MySQLToken.KW_LOCALTIME:
                    return "LOCALTIME";
                case MySQLToken.KW_LOCALTIMESTAMP:
                    return "LOCALTIMESTAMP";
                case MySQLToken.KW_LOCK:
                    return "LOCK";
                case MySQLToken.KW_LONG:
                    return "LONG";
                case MySQLToken.KW_LONGBLOB:
                    return "LONGBLOB";
                case MySQLToken.KW_LONGTEXT:
                    return "LONGTEXT";
                case MySQLToken.KW_LOOP:
                    return "LOOP";
                case MySQLToken.KW_LOW_PRIORITY:
                    return "LOW_PRIORITY";
                case MySQLToken.KW_MASTER_HEARTBEAT_PERIOD:
                    return "MASTER_HEARTBEAT_PERIOD";
                case MySQLToken.KW_MASTER_SSL_VERIFY_SERVER_CERT:
                    return "MASTER_SSL_VERIFY_SERVER_CERT";
                case MySQLToken.KW_MATCH:
                    return "MATCH";
                case MySQLToken.KW_MAXVALUE:
                    return "MAXVALUE";
                case MySQLToken.KW_MEDIUMBLOB:
                    return "MEDIUMBLOB";
                case MySQLToken.KW_MEDIUMINT:
                    return "MEDIUMINT";
                case MySQLToken.KW_MEDIUMTEXT:
                    return "MEDIUMTEXT";
                case MySQLToken.KW_MIDDLEINT:
                    return "MIDDLEINT";
                case MySQLToken.KW_MINUTE_MICROSECOND:
                    return "MINUTE_MICROSECOND";
                case MySQLToken.KW_MINUTE_SECOND:
                    return "MINUTE_SECOND";
                case MySQLToken.KW_MOD:
                    return "MOD";
                case MySQLToken.KW_MODIFIES:
                    return "MODIFIES";
                case MySQLToken.KW_NATURAL:
                    return "NATURAL";
                case MySQLToken.KW_NOT:
                    return "NOT";
                case MySQLToken.KW_NO_WRITE_TO_BINLOG:
                    return "NO_WRITE_TO_BINLOG";
                case MySQLToken.KW_NUMERIC:
                    return "NUMERIC";
                case MySQLToken.KW_ON:
                    return "ON";
                case MySQLToken.KW_OPTIMIZE:
                    return "OPTIMIZE";
                case MySQLToken.KW_OPTION:
                    return "OPTION";
                case MySQLToken.KW_OPTIONALLY:
                    return "OPTIONALLY";
                case MySQLToken.KW_OR:
                    return "OR";
                case MySQLToken.KW_ORDER:
                    return "ORDER";
                case MySQLToken.KW_OUT:
                    return "OUT";
                case MySQLToken.KW_OUTER:
                    return "OUTER";
                case MySQLToken.KW_OUTFILE:
                    return "OUTFILE";
                case MySQLToken.KW_PRECISION:
                    return "PRECISION";
                case MySQLToken.KW_PRIMARY:
                    return "PRIMARY";
                case MySQLToken.KW_PROCEDURE:
                    return "PROCEDURE";
                case MySQLToken.KW_PURGE:
                    return "PURGE";
                case MySQLToken.KW_RANGE:
                    return "RANGE";
                case MySQLToken.KW_READ:
                    return "READ";
                case MySQLToken.KW_READS:
                    return "READS";
                case MySQLToken.KW_READ_WRITE:
                    return "READ_WRITE";
                case MySQLToken.KW_REAL:
                    return "REAL";
                case MySQLToken.KW_REFERENCES:
                    return "REFERENCES";
                case MySQLToken.KW_REGEXP:
                    return "REGEXP";
                case MySQLToken.KW_RELEASE:
                    return "RELEASE";
                case MySQLToken.KW_RENAME:
                    return "RENAME";
                case MySQLToken.KW_REPEAT:
                    return "REPEAT";
                case MySQLToken.KW_REPLACE:
                    return "REPLACE";
                case MySQLToken.KW_REQUIRE:
                    return "REQUIRE";
                case MySQLToken.KW_RESIGNAL:
                    return "RESIGNAL";
                case MySQLToken.KW_RESTRICT:
                    return "RESTRICT";
                case MySQLToken.KW_RETURN:
                    return "RETURN";
                case MySQLToken.KW_REVOKE:
                    return "REVOKE";
                case MySQLToken.KW_RIGHT:
                    return "RIGHT";
                case MySQLToken.KW_RLIKE:
                    return "RLIKE";
                case MySQLToken.KW_SCHEMA:
                    return "SCHEMA";
                case MySQLToken.KW_SCHEMAS:
                    return "SCHEMAS";
                case MySQLToken.KW_SECOND_MICROSECOND:
                    return "SECOND_MICROSECOND";
                case MySQLToken.KW_SELECT:
                    return "SELECT";
                case MySQLToken.KW_SENSITIVE:
                    return "SENSITIVE";
                case MySQLToken.KW_SEPARATOR:
                    return "SEPARATOR";
                case MySQLToken.KW_SET:
                    return "SET";
                case MySQLToken.KW_SHOW:
                    return "SHOW";
                case MySQLToken.KW_SIGNAL:
                    return "SIGNAL";
                case MySQLToken.KW_SLOW:
                    return "SLOW";
                case MySQLToken.KW_SMALLINT:
                    return "SMALLINT";
                case MySQLToken.KW_SPATIAL:
                    return "SPATIAL";
                case MySQLToken.KW_SPECIFIC:
                    return "SPECIFIC";
                case MySQLToken.KW_SQL:
                    return "SQL";
                case MySQLToken.KW_SQLEXCEPTION:
                    return "SQLEXCEPTION";
                case MySQLToken.KW_SQLSTATE:
                    return "SQLSTATE";
                case MySQLToken.KW_SQLWARNING:
                    return "SQLWARNING";
                case MySQLToken.KW_SQL_BIG_RESULT:
                    return "SQL_BIG_RESULT";
                case MySQLToken.KW_SQL_CALC_FOUND_ROWS:
                    return "SQL_CALC_FOUND_ROWS";
                case MySQLToken.KW_SQL_SMALL_RESULT:
                    return "SQL_SMALL_RESULT";
                case MySQLToken.KW_SSL:
                    return "SSL";
                case MySQLToken.KW_STARTING:
                    return "STARTING";
                case MySQLToken.KW_STRAIGHT_JOIN:
                    return "STRAIGHT_JOIN";
                case MySQLToken.KW_TABLE:
                    return "TABLE";
                case MySQLToken.KW_TERMINATED:
                    return "TERMINATED";
                case MySQLToken.KW_THEN:
                    return "THEN";
                case MySQLToken.KW_TINYBLOB:
                    return "TINYBLOB";
                case MySQLToken.KW_TINYINT:
                    return "TINYINT";
                case MySQLToken.KW_TINYTEXT:
                    return "TINYTEXT";
                case MySQLToken.KW_TO:
                    return "TO";
                case MySQLToken.KW_TRAILING:
                    return "TRAILING";
                case MySQLToken.KW_TRIGGER:
                    return "TRIGGER";
                case MySQLToken.KW_UNDO:
                    return "UNDO";
                case MySQLToken.KW_UNION:
                    return "UNION";
                case MySQLToken.KW_UNIQUE:
                    return "UNIQUE";
                case MySQLToken.KW_UNLOCK:
                    return "UNLOCK";
                case MySQLToken.KW_UNSIGNED:
                    return "UNSIGNED";
                case MySQLToken.KW_UPDATE:
                    return "UPDATE";
                case MySQLToken.KW_USAGE:
                    return "USAGE";
                case MySQLToken.KW_USE:
                    return "USE";
                case MySQLToken.KW_USING:
                    return "USING";
                case MySQLToken.KW_UTC_DATE:
                    return "UTC_DATE";
                case MySQLToken.KW_UTC_TIME:
                    return "UTC_TIME";
                case MySQLToken.KW_UTC_TIMESTAMP:
                    return "UTC_TIMESTAMP";
                case MySQLToken.KW_VALUES:
                    return "VALUES";
                case MySQLToken.KW_VARBINARY:
                    return "VARBINARY";
                case MySQLToken.KW_VARCHAR:
                    return "VARCHAR";
                case MySQLToken.KW_VARCHARACTER:
                    return "VARCHARACTER";
                case MySQLToken.KW_VARYING:
                    return "VARYING";
                case MySQLToken.KW_WHEN:
                    return "WHEN";
                case MySQLToken.KW_WHERE:
                    return "WHERE";
                case MySQLToken.KW_WHILE:
                    return "WHILE";
                case MySQLToken.KW_WITH:
                    return "WITH";
                case MySQLToken.KW_WRITE:
                    return "WRITE";
                case MySQLToken.KW_XOR:
                    return "XOR";
                case MySQLToken.KW_YEAR_MONTH:
                    return "YEAR_MONTH";
                case MySQLToken.KW_ZEROFILL:
                    return "ZEROFILL";
                default:
                    throw new ArgumentException("token is not keyword: " + token);
            }
        }
    }
}