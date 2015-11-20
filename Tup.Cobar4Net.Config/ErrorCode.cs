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

namespace Tup.Cobar4Net.Config
{
    /// <author>xianmao.hexm</author>
    public abstract class ErrorCode
    {
        public const int ErrOpenSocket = 3001;

        public const int ErrConnectSocket = 3002;

        public const int ErrFinishConnect = 3003;

        public const int ErrRegister = 3004;

        public const int ErrRead = 3005;

        public const int ErrPutWriteQueue = 3006;

        public const int ErrWriteByEvent = 3007;

        public const int ErrWriteByQueue = 3008;

        public const int ErrHandleData = 3009;

        public const int ErHashchk = 1000;

        public const int ErNisamchk = 1001;

        public const int ErNo = 1002;

        public const int ErYes = 1003;

        public const int ErCantCreateFile = 1004;

        public const int ErCantCreateTable = 1005;

        public const int ErCantCreateDb = 1006;

        public const int ErDbCreateExists = 1007;

        public const int ErDbDropExists = 1008;

        public const int ErDbDropDelete = 1009;

        public const int ErDbDropRmdir = 1010;

        public const int ErCantDeleteFile = 1011;

        public const int ErCantFindSystemRec = 1012;

        public const int ErCantGetStat = 1013;

        public const int ErCantGetWd = 1014;

        public const int ErCantLock = 1015;

        public const int ErCantOpenFile = 1016;

        public const int ErFileNotFound = 1017;

        public const int ErCantReadDir = 1018;

        public const int ErCantSetWd = 1019;

        public const int ErCheckread = 1020;

        public const int ErDiskFull = 1021;

        public const int ErDupKey = 1022;

        public const int ErErrorOnClose = 1023;

        public const int ErErrorOnRead = 1024;

        public const int ErErrorOnRename = 1025;

        public const int ErErrorOnWrite = 1026;

        public const int ErFileUsed = 1027;

        public const int ErFilsortAbort = 1028;

        public const int ErFormNotFound = 1029;

        public const int ErGetErrno = 1030;

        public const int ErIllegalHa = 1031;

        public const int ErKeyNotFound = 1032;

        public const int ErNotFormFile = 1033;

        public const int ErNotKeyfile = 1034;

        public const int ErOldKeyfile = 1035;

        public const int ErOpenAsReadonly = 1036;

        public const int ErOutofmemory = 1037;

        public const int ErOutOfSortmemory = 1038;

        public const int ErUnexpectedEof = 1039;

        public const int ErConCountError = 1040;

        public const int ErOutOfResources = 1041;

        public const int ErBadHostError = 1042;

        public const int ErHandshakeError = 1043;

        public const int ErDbaccessDeniedError = 1044;

        public const int ErAccessDeniedError = 1045;

        public const int ErNoDbError = 1046;

        public const int ErUnknownComError = 1047;

        public const int ErBadNullError = 1048;

        public const int ErBadDbError = 1049;

        public const int ErTableExistsError = 1050;

        public const int ErBadTableError = 1051;

        public const int ErNonUniqError = 1052;

        public const int ErServerShutdown = 1053;

        public const int ErBadFieldError = 1054;

        public const int ErWrongFieldWithGroup = 1055;

        public const int ErWrongGroupField = 1056;

        public const int ErWrongSumSelect = 1057;

        public const int ErWrongValueCount = 1058;

        public const int ErTooLongIdent = 1059;

        public const int ErDupFieldname = 1060;

        public const int ErDupKeyname = 1061;

        public const int ErDupEntry = 1062;

        public const int ErWrongFieldSpec = 1063;

        public const int ErParseError = 1064;

        public const int ErEmptyQuery = 1065;

        public const int ErNonuniqTable = 1066;

        public const int ErInvalidDefault = 1067;

        public const int ErMultiplePriKey = 1068;

        public const int ErTooManyKeys = 1069;

        public const int ErTooManyKeyParts = 1070;

        public const int ErTooLongKey = 1071;

        public const int ErKeyColumnDoesNotExits = 1072;

        public const int ErBlobUsedAsKey = 1073;

        public const int ErTooBigFieldlength = 1074;

        public const int ErWrongAutoKey = 1075;

        public const int ErReady = 1076;

        public const int ErNormalShutdown = 1077;

        public const int ErGotSignal = 1078;

        public const int ErShutdownComplete = 1079;

        public const int ErForcingClose = 1080;

        public const int ErIpsockError = 1081;

        public const int ErNoSuchIndex = 1082;

        public const int ErWrongFieldTerminators = 1083;

        public const int ErBlobsAndNoTerminated = 1084;

        public const int ErTextfileNotReadable = 1085;

        public const int ErFileExistsError = 1086;

        public const int ErLoadInfo = 1087;

        public const int ErAlterInfo = 1088;

        public const int ErWrongSubKey = 1089;

        public const int ErCantRemoveAllFields = 1090;

        public const int ErCantDropFieldOrKey = 1091;

        public const int ErInsertInfo = 1092;

        public const int ErUpdateTableUsed = 1093;

        public const int ErNoSuchThread = 1094;

        public const int ErKillDeniedError = 1095;

        public const int ErNoTablesUsed = 1096;

        public const int ErTooBigSet = 1097;

        public const int ErNoUniqueLogfile = 1098;

        public const int ErTableNotLockedForWrite = 1099;

        public const int ErTableNotLocked = 1100;

        public const int ErBlobCantHaveDefault = 1101;

        public const int ErWrongDbName = 1102;

        public const int ErWrongTableName = 1103;

        public const int ErTooBigSelect = 1104;

        public const int ErUnknownError = 1105;

        public const int ErUnknownProcedure = 1106;

        public const int ErWrongParamcountToProcedure = 1107;

        public const int ErWrongParametersToProcedure = 1108;

        public const int ErUnknownTable = 1109;

        public const int ErFieldSpecifiedTwice = 1110;

        public const int ErInvalidGroupFuncUse = 1111;

        public const int ErUnsupportedExtension = 1112;

        public const int ErTableMustHaveColumns = 1113;

        public const int ErRecordFileFull = 1114;

        public const int ErUnknownCharacterSet = 1115;

        public const int ErTooManyTables = 1116;

        public const int ErTooManyFields = 1117;

        public const int ErTooBigRowsize = 1118;

        public const int ErStackOverrun = 1119;

        public const int ErWrongOuterJoin = 1120;

        public const int ErNullColumnInIndex = 1121;

        public const int ErCantFindUdf = 1122;

        public const int ErCantInitializeUdf = 1123;

        public const int ErUdfNoPaths = 1124;

        public const int ErUdfExists = 1125;

        public const int ErCantOpenLibrary = 1126;

        public const int ErCantFindDlEntry = 1127;

        public const int ErFunctionNotDefined = 1128;

        public const int ErHostIsBlocked = 1129;

        public const int ErHostNotPrivileged = 1130;

        public const int ErPasswordAnonymousUser = 1131;

        public const int ErPasswordNotAllowed = 1132;

        public const int ErPasswordNoMatch = 1133;

        public const int ErUpdateInfo = 1134;

        public const int ErCantCreateThread = 1135;

        public const int ErWrongValueCountOnRow = 1136;

        public const int ErCantReopenTable = 1137;

        public const int ErInvalidUseOfNull = 1138;

        public const int ErRegexpError = 1139;

        public const int ErMixOfGroupFuncAndFields = 1140;

        public const int ErNonexistingGrant = 1141;

        public const int ErTableaccessDeniedError = 1142;

        public const int ErColumnaccessDeniedError = 1143;

        public const int ErIllegalGrantForTable = 1144;

        public const int ErGrantWrongHostOrUser = 1145;

        public const int ErNoSuchTable = 1146;

        public const int ErNonexistingTableGrant = 1147;

        public const int ErNotAllowedCommand = 1148;

        public const int ErSyntaxError = 1149;

        public const int ErDelayedCantChangeLock = 1150;

        public const int ErTooManyDelayedThreads = 1151;

        public const int ErAbortingConnection = 1152;

        public const int ErNetPacketTooLarge = 1153;

        public const int ErNetReadErrorFromPipe = 1154;

        public const int ErNetFcntlError = 1155;

        public const int ErNetPacketsOutOfOrder = 1156;

        public const int ErNetUncompressError = 1157;

        public const int ErNetReadError = 1158;

        public const int ErNetReadInterrupted = 1159;

        public const int ErNetErrorOnWrite = 1160;

        public const int ErNetWriteInterrupted = 1161;

        public const int ErTooLongString = 1162;

        public const int ErTableCantHandleBlob = 1163;

        public const int ErTableCantHandleAutoIncrement = 1164;

        public const int ErDelayedInsertTableLocked = 1165;

        public const int ErWrongColumnName = 1166;

        public const int ErWrongKeyColumn = 1167;

        public const int ErWrongMrgTable = 1168;

        public const int ErDupUnique = 1169;

        public const int ErBlobKeyWithoutLength = 1170;

        public const int ErPrimaryCantHaveNull = 1171;

        public const int ErTooManyRows = 1172;

        public const int ErRequiresPrimaryKey = 1173;

        public const int ErNoRaidCompiled = 1174;

        public const int ErUpdateWithoutKeyInSafeMode = 1175;

        public const int ErKeyDoesNotExits = 1176;

        public const int ErCheckNoSuchTable = 1177;

        public const int ErCheckNotImplemented = 1178;

        public const int ErCantDoThisDuringAnTransaction = 1179;

        public const int ErErrorDuringCommit = 1180;

        public const int ErErrorDuringRollback = 1181;

        public const int ErErrorDuringFlushLogs = 1182;

        public const int ErErrorDuringCheckpoint = 1183;

        public const int ErNewAbortingConnection = 1184;

        public const int ErDumpNotImplemented = 1185;

        public const int ErFlushMasterBinlogClosed = 1186;

        public const int ErIndexRebuild = 1187;

        public const int ErMaster = 1188;

        public const int ErMasterNetRead = 1189;

        public const int ErMasterNetWrite = 1190;

        public const int ErFtMatchingKeyNotFound = 1191;

        public const int ErLockOrActiveTransaction = 1192;

        public const int ErUnknownSystemVariable = 1193;

        public const int ErCrashedOnUsage = 1194;

        public const int ErCrashedOnRepair = 1195;

        public const int ErWarningNotCompleteRollback = 1196;

        public const int ErTransCacheFull = 1197;

        public const int ErSlaveMustStop = 1198;

        public const int ErSlaveNotRunning = 1199;

        public const int ErBadSlave = 1200;

        public const int ErMasterInfo = 1201;

        public const int ErSlaveThread = 1202;

        public const int ErTooManyUserConnections = 1203;

        public const int ErSetConstantsOnly = 1204;

        public const int ErLockWaitTimeout = 1205;

        public const int ErLockTableFull = 1206;

        public const int ErReadOnlyTransaction = 1207;

        public const int ErDropDbWithReadLock = 1208;

        public const int ErCreateDbWithReadLock = 1209;

        public const int ErWrongArguments = 1210;

        public const int ErNoPermissionToCreateUser = 1211;

        public const int ErUnionTablesInDifferentDir = 1212;

        public const int ErLockDeadlock = 1213;

        public const int ErTableCantHandleFt = 1214;

        public const int ErCannotAddForeign = 1215;

        public const int ErNoReferencedRow = 1216;

        public const int ErRowIsReferenced = 1217;

        public const int ErConnectToMaster = 1218;

        public const int ErQueryOnMaster = 1219;

        public const int ErErrorWhenExecutingCommand = 1220;

        public const int ErWrongUsage = 1221;

        public const int ErWrongNumberOfColumnsInSelect = 1222;

        public const int ErCantUpdateWithReadlock = 1223;

        public const int ErMixingNotAllowed = 1224;

        public const int ErDupArgument = 1225;

        public const int ErUserLimitReached = 1226;

        public const int ErSpecificAccessDeniedError = 1227;

        public const int ErLocalVariable = 1228;

        public const int ErGlobalVariable = 1229;

        public const int ErNoDefault = 1230;

        public const int ErWrongValueForVar = 1231;

        public const int ErWrongTypeForVar = 1232;

        public const int ErVarCantBeRead = 1233;

        public const int ErCantUseOptionHere = 1234;

        public const int ErNotSupportedYet = 1235;

        public const int ErMasterFatalErrorReadingBinlog = 1236;

        public const int ErSlaveIgnoredTable = 1237;

        public const int ErIncorrectGlobalLocalVar = 1238;

        public const int ErWrongFkDef = 1239;

        public const int ErKeyRefDoNotMatchTableRef = 1240;

        public const int ErOperandColumns = 1241;

        public const int ErSubqueryNo1Row = 1242;

        public const int ErUnknownStmtHandler = 1243;

        public const int ErCorruptHelpDb = 1244;

        public const int ErCyclicReference = 1245;

        public const int ErAutoConvert = 1246;

        public const int ErIllegalReference = 1247;

        public const int ErDerivedMustHaveAlias = 1248;

        public const int ErSelectReduced = 1249;

        public const int ErTablenameNotAllowedHere = 1250;

        public const int ErNotSupportedAuthMode = 1251;

        public const int ErSpatialCantHaveNull = 1252;

        public const int ErCollationCharsetMismatch = 1253;

        public const int ErSlaveWasRunning = 1254;

        public const int ErSlaveWasNotRunning = 1255;

        public const int ErTooBigForUncompress = 1256;

        public const int ErZlibZMemError = 1257;

        public const int ErZlibZBufError = 1258;

        public const int ErZlibZDataError = 1259;

        public const int ErCutValueGroupConcat = 1260;

        public const int ErWarnTooFewRecords = 1261;

        public const int ErWarnTooManyRecords = 1262;

        public const int ErWarnNullToNotnull = 1263;

        public const int ErWarnDataOutOfRange = 1264;

        public const int WarnDataTruncated = 1265;

        public const int ErWarnUsingOtherHandler = 1266;

        public const int ErCantAggregate2collations = 1267;

        public const int ErDropUser = 1268;

        public const int ErRevokeGrants = 1269;

        public const int ErCantAggregate3collations = 1270;

        public const int ErCantAggregateNcollations = 1271;

        public const int ErVariableIsNotStruct = 1272;

        public const int ErUnknownCollation = 1273;

        public const int ErSlaveIgnoredSslParams = 1274;

        public const int ErServerIsInSecureAuthMode = 1275;

        public const int ErWarnFieldResolved = 1276;

        public const int ErBadSlaveUntilCond = 1277;

        public const int ErMissingSkipSlave = 1278;

        public const int ErUntilCondIgnored = 1279;

        public const int ErWrongNameForIndex = 1280;

        public const int ErWrongNameForCatalog = 1281;

        public const int ErWarnQcResize = 1282;

        public const int ErBadFtColumn = 1283;

        public const int ErUnknownKeyCache = 1284;

        public const int ErWarnHostnameWontWork = 1285;

        public const int ErUnknownStorageEngine = 1286;

        public const int ErWarnDeprecatedSyntax = 1287;

        public const int ErNonUpdatableTable = 1288;

        public const int ErFeatureDisabled = 1289;

        public const int ErOptionPreventsStatement = 1290;

        public const int ErDuplicatedValueInType = 1291;

        public const int ErTruncatedWrongValue = 1292;

        public const int ErTooMuchAutoTimestampCols = 1293;

        public const int ErInvalidOnUpdate = 1294;

        public const int ErUnsupportedPs = 1295;

        public const int ErGetErrmsg = 1296;

        public const int ErGetTemporaryErrmsg = 1297;

        public const int ErUnknownTimeZone = 1298;

        public const int ErWarnInvalidTimestamp = 1299;

        public const int ErInvalidCharacterString = 1300;

        public const int ErWarnAllowedPacketOverflowed = 1301;

        public const int ErConflictingDeclarations = 1302;

        public const int ErSpNoRecursiveCreate = 1303;

        public const int ErSpAlreadyExists = 1304;

        public const int ErSpDoesNotExist = 1305;

        public const int ErSpDropFailed = 1306;

        public const int ErSpStoreFailed = 1307;

        public const int ErSpLilabelMismatch = 1308;

        public const int ErSpLabelRedefine = 1309;

        public const int ErSpLabelMismatch = 1310;

        public const int ErSpUninitVar = 1311;

        public const int ErSpBadselect = 1312;

        public const int ErSpBadreturn = 1313;

        public const int ErSpBadstatement = 1314;

        public const int ErUpdateLogDeprecatedIgnored = 1315;

        public const int ErUpdateLogDeprecatedTranslated = 1316;

        public const int ErQueryInterrupted = 1317;

        public const int ErSpWrongNoOfArgs = 1318;

        public const int ErSpCondMismatch = 1319;

        public const int ErSpNoreturn = 1320;

        public const int ErSpNoreturnend = 1321;

        public const int ErSpBadCursorQuery = 1322;

        public const int ErSpBadCursorSelect = 1323;

        public const int ErSpCursorMismatch = 1324;

        public const int ErSpCursorAlreadyOpen = 1325;

        public const int ErSpCursorNotOpen = 1326;

        public const int ErSpUndeclaredVar = 1327;

        public const int ErSpWrongNoOfFetchArgs = 1328;

        public const int ErSpFetchNoData = 1329;

        public const int ErSpDupParam = 1330;

        public const int ErSpDupVar = 1331;

        public const int ErSpDupCond = 1332;

        public const int ErSpDupCurs = 1333;

        public const int ErSpCantAlter = 1334;

        public const int ErSpSubselectNyi = 1335;

        public const int ErStmtNotAllowedInSfOrTrg = 1336;

        public const int ErSpVarcondAfterCurshndlr = 1337;

        public const int ErSpCursorAfterHandler = 1338;

        public const int ErSpCaseNotFound = 1339;

        public const int ErFparserTooBigFile = 1340;

        public const int ErFparserBadHeader = 1341;

        public const int ErFparserEofInComment = 1342;

        public const int ErFparserErrorInParameter = 1343;

        public const int ErFparserEofInUnknownParameter = 1344;

        public const int ErViewNoExplain = 1345;

        public const int ErFrmUnknownType = 1346;

        public const int ErWrongObject = 1347;

        public const int ErNonupdateableColumn = 1348;

        public const int ErViewSelectDerived = 1349;

        public const int ErViewSelectClause = 1350;

        public const int ErViewSelectVariable = 1351;

        public const int ErViewSelectTmptable = 1352;

        public const int ErViewWrongList = 1353;

        public const int ErWarnViewMerge = 1354;

        public const int ErWarnViewWithoutKey = 1355;

        public const int ErViewInvalid = 1356;

        public const int ErSpNoDropSp = 1357;

        public const int ErSpGotoInHndlr = 1358;

        public const int ErTrgAlreadyExists = 1359;

        public const int ErTrgDoesNotExist = 1360;

        public const int ErTrgOnViewOrTempTable = 1361;

        public const int ErTrgCantChangeRow = 1362;

        public const int ErTrgNoSuchRowInTrg = 1363;

        public const int ErNoDefaultForField = 1364;

        public const int ErDivisionByZero = 1365;

        public const int ErTruncatedWrongValueForField = 1366;

        public const int ErIllegalValueForType = 1367;

        public const int ErViewNonupdCheck = 1368;

        public const int ErViewCheckFailed = 1369;

        public const int ErProcaccessDeniedError = 1370;

        public const int ErRelayLogFail = 1371;

        public const int ErPasswdLength = 1372;

        public const int ErUnknownTargetBinlog = 1373;

        public const int ErIoErrLogIndexRead = 1374;

        public const int ErBinlogPurgeProhibited = 1375;

        public const int ErFseekFail = 1376;

        public const int ErBinlogPurgeFatalErr = 1377;

        public const int ErLogInUse = 1378;

        public const int ErLogPurgeUnknownErr = 1379;

        public const int ErRelayLogInit = 1380;

        public const int ErNoBinaryLogging = 1381;

        public const int ErReservedSyntax = 1382;

        public const int ErWsasFailed = 1383;

        public const int ErDiffGroupsProc = 1384;

        public const int ErNoGroupForProc = 1385;

        public const int ErOrderWithProc = 1386;

        public const int ErLoggingProhibitChangingOf = 1387;

        public const int ErNoFileMapping = 1388;

        public const int ErWrongMagic = 1389;

        public const int ErPsManyParam = 1390;

        public const int ErKeyPart0 = 1391;

        public const int ErViewChecksum = 1392;

        public const int ErViewMultiupdate = 1393;

        public const int ErViewNoInsertFieldList = 1394;

        public const int ErViewDeleteMergeView = 1395;

        public const int ErCannotUser = 1396;

        public const int ErXaerNota = 1397;

        public const int ErXaerInval = 1398;

        public const int ErXaerRmfail = 1399;

        public const int ErXaerOutside = 1400;

        public const int ErXaerRmerr = 1401;

        public const int ErXaRbrollback = 1402;

        public const int ErNonexistingProcGrant = 1403;

        public const int ErProcAutoGrantFail = 1404;

        public const int ErProcAutoRevokeFail = 1405;

        public const int ErDataTooLong = 1406;

        public const int ErSpBadSqlstate = 1407;

        public const int ErStartup = 1408;

        public const int ErLoadFromFixedSizeRowsToVar = 1409;

        public const int ErCantCreateUserWithGrant = 1410;

        public const int ErWrongValueForType = 1411;

        public const int ErTableDefChanged = 1412;

        public const int ErSpDupHandler = 1413;

        public const int ErSpNotVarArg = 1414;

        public const int ErSpNoRetset = 1415;

        public const int ErCantCreateGeometryObject = 1416;

        public const int ErFailedRoutineBreakBinlog = 1417;

        public const int ErBinlogUnsafeRoutine = 1418;

        public const int ErBinlogCreateRoutineNeedSuper = 1419;

        public const int ErExecStmtWithOpenCursor = 1420;

        public const int ErStmtHasNoOpenCursor = 1421;

        public const int ErCommitNotAllowedInSfOrTrg = 1422;

        public const int ErNoDefaultForViewField = 1423;

        public const int ErSpNoRecursion = 1424;

        public const int ErTooBigScale = 1425;

        public const int ErTooBigPrecision = 1426;

        public const int ErMBiggerThanD = 1427;

        public const int ErWrongLockOfSystemTable = 1428;

        public const int ErConnectToForeignDataSource = 1429;

        public const int ErQueryOnForeignDataSource = 1430;

        public const int ErForeignDataSourceDoesntExist = 1431;

        public const int ErForeignDataStringInvalidCantCreate = 1432;

        public const int ErForeignDataStringInvalid = 1433;

        public const int ErCantCreateFederatedTable = 1434;

        public const int ErTrgInWrongSchema = 1435;

        public const int ErStackOverrunNeedMore = 1436;

        public const int ErTooLongBody = 1437;

        public const int ErWarnCantDropDefaultKeycache = 1438;

        public const int ErTooBigDisplaywidth = 1439;

        public const int ErXaerDupid = 1440;

        public const int ErDatetimeFunctionOverflow = 1441;

        public const int ErCantUpdateUsedTableInSfOrTrg = 1442;

        public const int ErViewPreventUpdate = 1443;

        public const int ErPsNoRecursion = 1444;

        public const int ErSpCantSetAutocommit = 1445;

        public const int ErNoViewUser = 1446;

        public const int ErViewFrmNoUser = 1447;

        public const int ErViewOtherUser = 1448;

        public const int ErNoSuchUser = 1449;

        public const int ErForbidSchemaChange = 1450;

        public const int ErRowIsReferenced2 = 1451;

        public const int ErNoReferencedRow2 = 1452;

        public const int ErSpBadVarShadow = 1453;

        public const int ErPartitionRequiresValuesError = 1454;

        public const int ErPartitionWrongValuesError = 1455;

        public const int ErPartitionMaxvalueError = 1456;

        public const int ErPartitionSubpartitionError = 1457;

        public const int ErPartitionWrongNoPartError = 1458;

        public const int ErPartitionWrongNoSubpartError = 1459;

        public const int ErConstExprInPartitionFuncError = 1460;

        public const int ErNoConstExprInRangeOrListError = 1461;

        public const int ErFieldNotFoundPartError = 1462;

        public const int ErListOfFieldsOnlyInHashError = 1463;

        public const int ErInconsistentPartitionInfoError = 1464;

        public const int ErPartitionFuncNotAllowedError = 1465;

        public const int ErPartitionsMustBeDefinedError = 1466;

        public const int ErRangeNotIncreasingError = 1467;

        public const int ErInconsistentTypeOfFunctionsError = 1468;

        public const int ErMultipleDefConstInListPartError = 1469;

        public const int ErPartitionEntryError = 1470;

        public const int ErMixHandlerError = 1471;

        public const int ErPartitionNotDefinedError = 1472;

        public const int ErTooManyPartitionsError = 1473;

        public const int ErSubpartitionError = 1474;

        public const int ErCantCreateHandlerFile = 1475;

        public const int ErBlobFieldInPartFuncError = 1476;

        public const int ErCharSetInPartFieldError = 1477;

        public const int ErUniqueKeyNeedAllFieldsInPf = 1478;

        public const int ErNoPartsError = 1479;

        public const int ErPartitionMgmtOnNonpartitioned = 1480;

        public const int ErDropPartitionNonExistent = 1481;

        public const int ErDropLastPartition = 1482;

        public const int ErCoalesceOnlyOnHashPartition = 1483;

        public const int ErOnlyOnRangeListPartition = 1484;

        public const int ErAddPartitionSubpartError = 1485;

        public const int ErAddPartitionNoNewPartition = 1486;

        public const int ErCoalescePartitionNoPartition = 1487;

        public const int ErReorgPartitionNotExist = 1488;

        public const int ErSameNamePartition = 1489;

        public const int ErConsecutiveReorgPartitions = 1490;

        public const int ErReorgOutsideRange = 1491;

        public const int ErDropPartitionFailure = 1492;

        public const int ErDropPartitionWhenFkDefined = 1493;

        public const int ErPluginIsNotLoaded = 1494;
        // cobar error code
        // mysql error code
    }

    public static class ErrorCodeConstants
    {
    }
}