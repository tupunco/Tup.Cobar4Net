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

using Tup.Cobar.Parser.Ast.Expression.Primary.Function;
using Tup.Cobar.Parser.Ast.Expression.Primary.Function.Arithmetic;
using Tup.Cobar.Parser.Ast.Expression.Primary.Function.Bit;
using Tup.Cobar.Parser.Ast.Expression.Primary.Function.Comparison;
using Tup.Cobar.Parser.Ast.Expression.Primary.Function.Datetime;
using Tup.Cobar.Parser.Ast.Expression.Primary.Function.Encryption;
using Tup.Cobar.Parser.Ast.Expression.Primary.Function.Flowctrl;
using Tup.Cobar.Parser.Ast.Expression.Primary.Function.Groupby;
using Tup.Cobar.Parser.Ast.Expression.Primary.Function.Info;
using Tup.Cobar.Parser.Ast.Expression.Primary.Function.Misc;
using Tup.Cobar.Parser.Ast.Expression.Primary.Function.String;
using Tup.Cobar.Parser.Ast.Expression.Primary.Function.Xml;
using Expr = Tup.Cobar.Parser.Ast.Expression.Expression;

namespace Tup.Cobar.Parser.Recognizer.Mysql
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class MySQLFunctionManager
    {
        public enum FunctionParsingStrategy
        {
            None = 0,
            Default,
            Ordinary,
            Cast,
            Position,
            Substring,
            Trim,
            Avg,
            Count,
            GroupConcat,
            Max,
            Min,
            Sum,
            Row,
            Char,
            Convert,
            Extract,
            Timestampadd,
            Timestampdiff,
            GetFormat
        }

        public static readonly MySQLFunctionManager InstanceMysqlDefault = new MySQLFunctionManager(false);

        private readonly bool allowFuncDefChange;

        /// <summary>non-reserved word named special syntax function</summary>
        private readonly Dictionary<string, FunctionParsingStrategy> parsingStrateg = new Dictionary<string, FunctionParsingStrategy>();

        /// <summary>non-reserved word named ordinary syntax function</summary>
        private IDictionary<string, FunctionExpression> functionPrototype = new Dictionary<string, FunctionExpression>();

        public MySQLFunctionManager(bool allowFuncDefChange)
        {
            this.allowFuncDefChange = allowFuncDefChange;
            parsingStrateg["CAST"] = FunctionParsingStrategy.Cast;
            parsingStrateg["POSITION"] = FunctionParsingStrategy.Position;
            parsingStrateg["SUBSTR"] = FunctionParsingStrategy.Substring;
            parsingStrateg["SUBSTRING"] = FunctionParsingStrategy.Substring;
            parsingStrateg["TRIM"] = FunctionParsingStrategy.Trim;
            parsingStrateg["AVG"] = FunctionParsingStrategy.Avg;
            parsingStrateg["COUNT"] = FunctionParsingStrategy.Count;
            parsingStrateg["GROUP_CONCAT"] = FunctionParsingStrategy.GroupConcat;
            parsingStrateg["MAX"] = FunctionParsingStrategy.Max;
            parsingStrateg["MIN"] = FunctionParsingStrategy.Min;
            parsingStrateg["SUM"] = FunctionParsingStrategy.Sum;
            parsingStrateg["ROW"] = FunctionParsingStrategy.Row;
            parsingStrateg["CHAR"] = FunctionParsingStrategy.Char;
            parsingStrateg["CONVERT"] = FunctionParsingStrategy.Convert;
            parsingStrateg["EXTRACT"] = FunctionParsingStrategy.Extract;
            parsingStrateg["TIMESTAMPADD"] = FunctionParsingStrategy.Timestampadd;
            parsingStrateg["TIMESTAMPDIFF"] = FunctionParsingStrategy.Timestampdiff;
            parsingStrateg["GET_FORMAT"] = FunctionParsingStrategy.GetFormat;
            functionPrototype["ABS"] = new Abs(null);
            functionPrototype["ACOS"] = new Acos(null);
            functionPrototype["ADDDATE"] = new Adddate(null);
            functionPrototype["ADDTIME"] = new Addtime(null);
            functionPrototype["AES_DECRYPT"] = new AesDecrypt(null);
            functionPrototype["AES_ENCRYPT"] = new AesEncrypt(null);
            functionPrototype["ANALYSE"] = new Analyse(null);
            functionPrototype["ASCII"] = new Ascii(null);
            functionPrototype["ASIN"] = new Asin(null);
            functionPrototype["ATAN2"] = new Atan2(null);
            functionPrototype["ATAN"] = new Atan(null);
            functionPrototype["BENCHMARK"] = new Benchmark(null);
            functionPrototype["BIN"] = new Bin(null);
            functionPrototype["BIT_AND"] = new BitAnd(null);
            functionPrototype["BIT_COUNT"] = new BitCount(null);
            functionPrototype["BIT_LENGTH"] = new BitLength(null);
            functionPrototype["BIT_OR"] = new BitOr(null);
            functionPrototype["BIT_XOR"] = new BitXor(null);
            functionPrototype["CEIL"] = new Ceiling(null);
            functionPrototype["CEILING"] = new Ceiling(null);
            functionPrototype["CHAR_LENGTH"] = new CharLength(null);
            functionPrototype["CHARACTER_LENGTH"] = new CharLength(null);
            functionPrototype["CHARSET"] = new Charset(null);
            functionPrototype["COALESCE"] = new Coalesce(null);
            functionPrototype["COERCIBILITY"] = new Coercibility(null);
            functionPrototype["COLLATION"] = new Collation(null);
            functionPrototype["COMPRESS"] = new Compress(null);
            functionPrototype["CONCAT_WS"] = new ConcatWs(null);
            functionPrototype["CONCAT"] = new Concat(null);
            functionPrototype["CONNECTION_ID"] = new ConnectionId(null);
            functionPrototype["CONV"] = new Conv(null);
            functionPrototype["CONVERT_TZ"] = new ConvertTz(null);
            functionPrototype["COS"] = new Cos(null);
            functionPrototype["COT"] = new Cot(null);
            functionPrototype["CRC32"] = new Crc32(null);
            functionPrototype["CURDATE"] = new Curdate();
            functionPrototype["CURRENT_DATE"] = new Curdate();
            functionPrototype["CURRENT_TIME"] = new Curtime();
            functionPrototype["CURTIME"] = new Curtime();
            functionPrototype["CURRENT_TIMESTAMP"] = new Now();
            functionPrototype["CURRENT_USER"] = new CurrentUser();
            functionPrototype["CURTIME"] = new Curtime();
            functionPrototype["DATABASE"] = new Database(null);
            functionPrototype["DATE_ADD"] = new DateAdd(null);
            functionPrototype["DATE_FORMAT"] = new DateFormat(null);
            functionPrototype["DATE_SUB"] = new DateSub(null);
            functionPrototype["DATE"] = new Date(null);
            functionPrototype["DATEDIFF"] = new Datediff(null);
            functionPrototype["DAY"] = new Dayofmonth(null);
            functionPrototype["DAYOFMONTH"] = new Dayofmonth(null);
            functionPrototype["DAYNAME"] = new Dayname(null);
            functionPrototype["DAYOFWEEK"] = new Dayofweek(null);
            functionPrototype["DAYOFYEAR"] = new Dayofyear(null);
            functionPrototype["DECODE"] = new Decode(null);
            functionPrototype["DEFAULT"] = new Default(null);
            functionPrototype["DEGREES"] = new Degrees(null);
            functionPrototype["DES_DECRYPT"] = new DesDecrypt(null);
            functionPrototype["DES_ENCRYPT"] = new DesEncrypt(null);
            functionPrototype["ELT"] = new Elt(null);
            functionPrototype["ENCODE"] = new Encode(null);
            functionPrototype["ENCRYPT"] = new Encrypt(null);
            functionPrototype["EXP"] = new Exp(null);
            functionPrototype["EXPORT_SET"] = new ExportSet(null);
            // functionPrototype.put("EXTRACT", new Extract(null));
            functionPrototype["EXTRACTVALUE"] = new Extractvalue(null);
            functionPrototype["FIELD"] = new Field(null);
            functionPrototype["FIND_IN_SET"] = new FindInSet(null);
            functionPrototype["FLOOR"] = new Floor(null);
            functionPrototype["FORMAT"] = new Format(null);
            functionPrototype["FOUND_ROWS"] = new FoundRows(null);
            functionPrototype["FROM_DAYS"] = new FromDays(null);
            functionPrototype["FROM_UNIXTIME"] = new FromUnixtime(null);
            // functionPrototype.put("GET_FORMAT", new GetFormat(null));
            functionPrototype["GET_LOCK"] = new GetLock(null);
            functionPrototype["GREATEST"] = new Greatest(null);
            functionPrototype["HEX"] = new Hex(null);
            functionPrototype["HOUR"] = new Hour(null);
            functionPrototype["IF"] = new IF(null);
            functionPrototype["IFNULL"] = new Ifnull(null);
            functionPrototype["INET_ATON"] = new InetAton(null);
            functionPrototype["INET_NTOA"] = new InetNtoa(null);
            functionPrototype["INSERT"] = new Insert(null);
            functionPrototype["INSTR"] = new Instr(null);
            functionPrototype["INTERVAL"] = new Interval(null);
            functionPrototype["IS_FREE_LOCK"] = new IsFreeLock(null);
            functionPrototype["IS_USED_LOCK"] = new IsUsedLock(null);
            functionPrototype["ISNULL"] = new Isnull(null);
            functionPrototype["LAST_DAY"] = new LastDay(null);
            functionPrototype["LAST_INSERT_ID"] = new LastInsertId(null);
            functionPrototype["LCASE"] = new Lower(null);
            functionPrototype["LEAST"] = new Least(null);
            functionPrototype["LEFT"] = new Left(null);
            functionPrototype["LENGTH"] = new Length(null);
            functionPrototype["LN"] = new Log(null);
            // Ln(X) equals Log(X)
            functionPrototype["LOAD_FILE"] = new LoadFile(null);
            functionPrototype["LOCALTIME"] = new Now();
            functionPrototype["LOCALTIMESTAMP"] = new Now();
            functionPrototype["LOCATE"] = new Locate(null);
            functionPrototype["LOG10"] = new Log10(null);
            functionPrototype["LOG2"] = new Log2(null);
            functionPrototype["LOG"] = new Log(null);
            functionPrototype["LOWER"] = new Lower(null);
            functionPrototype["LPAD"] = new Lpad(null);
            functionPrototype["LTRIM"] = new Ltrim(null);
            functionPrototype["MAKE_SET"] = new MakeSet(null);
            functionPrototype["MAKEDATE"] = new Makedate(null);
            functionPrototype["MAKETIME"] = new Maketime(null);
            functionPrototype["MASTER_POS_WAIT"] = new MasterPosWait(null);
            functionPrototype["MD5"] = new Md5(null);
            functionPrototype["MICROSECOND"] = new Microsecond(null);
            functionPrototype["MID"] = new Substring(null);
            functionPrototype["MINUTE"] = new Minute(null);
            functionPrototype["MONTH"] = new Month(null);
            functionPrototype["MONTHNAME"] = new Monthname(null);
            functionPrototype["NAME_CONST"] = new NameConst(null);
            functionPrototype["NOW"] = new Now();
            functionPrototype["NULLIF"] = new Nullif(null);
            functionPrototype["OCT"] = new Oct(null);
            functionPrototype["OCTET_LENGTH"] = new Length(null);
            functionPrototype["OLD_PASSWORD"] = new OldPassword(null);
            functionPrototype["ORD"] = new Ord(null);
            functionPrototype["PASSWORD"] = new Password(null);
            functionPrototype["PERIOD_ADD"] = new PeriodAdd(null);
            functionPrototype["PERIOD_DIFF"] = new PeriodDiff(null);
            functionPrototype["PI"] = new PI(null);
            functionPrototype["POW"] = new Pow(null);
            functionPrototype["POWER"] = new Pow(null);
            functionPrototype["QUARTER"] = new Quarter(null);
            functionPrototype["QUOTE"] = new Quote(null);
            functionPrototype["RADIANS"] = new Radians(null);
            functionPrototype["RAND"] = new Rand(null);
            functionPrototype["RELEASE_LOCK"] = new ReleaseLock(null);
            functionPrototype["REPEAT"] = new Repeat(null);
            functionPrototype["REPLACE"] = new Replace(null);
            functionPrototype["REVERSE"] = new Reverse(null);
            functionPrototype["RIGHT"] = new Right(null);
            functionPrototype["ROUND"] = new Round(null);
            functionPrototype["ROW_COUNT"] = new RowCount(null);
            functionPrototype["RPAD"] = new Rpad(null);
            functionPrototype["RTRIM"] = new Rtrim(null);
            functionPrototype["SCHEMA"] = new Database(null);
            functionPrototype["SEC_TO_TIME"] = new SecToTime(null);
            functionPrototype["SECOND"] = new Second(null);
            functionPrototype["SESSION_USER"] = new User(null);
            functionPrototype["SHA1"] = new Sha1(null);
            functionPrototype["SHA"] = new Sha1(null);
            functionPrototype["SHA2"] = new Sha2(null);
            functionPrototype["SIGN"] = new Sign(null);
            functionPrototype["SIN"] = new Sin(null);
            functionPrototype["SLEEP"] = new Sleep(null);
            functionPrototype["SOUNDEX"] = new Soundex(null);
            functionPrototype["SPACE"] = new Space(null);
            functionPrototype["SQRT"] = new Sqrt(null);
            functionPrototype["STD"] = new Std(null);
            functionPrototype["STDDEV_POP"] = new StddevPop(null);
            functionPrototype["STDDEV_SAMP"] = new StddevSamp(null);
            functionPrototype["STDDEV"] = new Stddev(null);
            functionPrototype["STR_TO_DATE"] = new StrToDate(null);
            functionPrototype["STRCMP"] = new Strcmp(null);
            functionPrototype["SUBDATE"] = new Subdate(null);
            functionPrototype["SUBSTRING_INDEX"] = new SubstringIndex(null);
            functionPrototype["SUBTIME"] = new Subtime(null);
            functionPrototype["SYSDATE"] = new Sysdate(null);
            functionPrototype["SYSTEM_USER"] = new User(null);
            functionPrototype["TAN"] = new Tan(null);
            functionPrototype["TIME_FORMAT"] = new TimeFormat(null);
            functionPrototype["TIME_TO_SEC"] = new TimeToSec(null);
            functionPrototype["TIME"] = new Time(null);
            functionPrototype["TIMEDIFF"] = new Timediff(null);
            functionPrototype["TIMESTAMP"] = new Timestamp(null);
            // functionPrototype.put("TIMESTAMPADD", new Timestampadd(null));
            // functionPrototype.put("TIMESTAMPDIFF", new Timestampdiff(null));
            functionPrototype["TO_DAYS"] = new ToDays(null);
            functionPrototype["TO_SECONDS"] = new ToSeconds(null);
            functionPrototype["TRUNCATE"] = new Truncate(null);
            functionPrototype["UCASE"] = new Upper(null);
            functionPrototype["UNCOMPRESS"] = new Uncompress(null);
            functionPrototype["UNCOMPRESSED_LENGTH"] = new UncompressedLength(null);
            functionPrototype["UNHEX"] = new Unhex(null);
            functionPrototype["UNIX_TIMESTAMP"] = new UnixTimestamp(null);
            functionPrototype["UPDATEXML"] = new Updatexml(null);
            functionPrototype["UPPER"] = new Upper(null);
            functionPrototype["USER"] = new User(null);
            functionPrototype["UTC_DATE"] = new UtcDate(null);
            functionPrototype["UTC_TIME"] = new UtcTime(null);
            functionPrototype["UTC_TIMESTAMP"] = new UtcTimestamp(null);
            functionPrototype["UUID_SHORT"] = new UuidShort(null);
            functionPrototype["UUID"] = new Uuid(null);
            functionPrototype["VALUES"] = new Values(null);
            functionPrototype["VAR_POP"] = new VarPop(null);
            functionPrototype["VAR_SAMP"] = new VarSamp(null);
            functionPrototype["VARIANCE"] = new Variance(null);
            functionPrototype["VERSION"] = new Ast.Expression.Primary.Function.Info.Version(null);
            functionPrototype["WEEK"] = new Week(null);
            functionPrototype["WEEKDAY"] = new Weekday(null);
            functionPrototype["WEEKOFYEAR"] = new Weekofyear(null);
            functionPrototype["YEAR"] = new Year(null);
            functionPrototype["YEARWEEK"] = new Yearweek(null);
        }

        /// <param name="extFuncPrototypeMap">
        /// funcName -&gt; extFunctionPrototype. funcName
        /// MUST NOT be the same as predefined function of MySQL 5.5
        /// </param>
        /// <exception cref="System.ArgumentException"/>
        public virtual void AddExtendFunction(IDictionary<string, FunctionExpression> extFuncPrototypeMap)
        {
            if (extFuncPrototypeMap == null || extFuncPrototypeMap.IsEmpty())
            {
                return;
            }
            if (!allowFuncDefChange)
            {
                throw new NotSupportedException("function define is not allowed to be changed");
            }
            lock (this)
            {
                var toPut = new Dictionary<string, FunctionExpression>();
                // check extFuncPrototypeMap
                foreach (var en in extFuncPrototypeMap)
                {
                    string funcName = en.Key;
                    if (funcName == null)
                    {
                        continue;
                    }
                    string funcNameUp = funcName.ToUpper();
                    if (functionPrototype.ContainsKey(funcNameUp))
                    {
                        throw new ArgumentException("ext-function '" + funcName + "' is MySQL's predefined function!");
                    }
                    FunctionExpression func = en.Value;
                    if (func == null)
                    {
                        throw new ArgumentException("ext-function '" + funcName + "' is null!");
                    }
                    toPut[funcNameUp] = func;
                }
                functionPrototype.AddRange(toPut);
            }
        }

        /// <returns>null if</returns>
        public virtual FunctionExpression CreateFunctionExpression(string funcNameUpcase, IList<Expr> arguments)
        {
            var prototype = functionPrototype.GetValue(funcNameUpcase);
            if (prototype == null)
            {
                return null;
            }
            FunctionExpression func = prototype.ConstructFunction(arguments);
            func.Init();
            return func;
        }

        public virtual FunctionParsingStrategy GetParsingStrategy(string funcNameUpcase)
        {
            var s = parsingStrateg.GetValue(funcNameUpcase);
            if (s == FunctionParsingStrategy.None)
            {
                if (functionPrototype.ContainsKey(funcNameUpcase))
                {
                    return FunctionParsingStrategy.Ordinary;
                }
                return FunctionParsingStrategy.Default;
            }
            return s;
        }
    }
}