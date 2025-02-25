using System;
using Test.UtilsTools;
using TDengineDriver;
using Test.UtilsTools.DataSource;
using System.Collections.Generic;
using Test.UtilsTools.ResultSet;
using Xunit;
using Test.Fixture;
using Test.Case.Attributes;
using Xunit.Abstractions;

namespace Cases
{
    [TestCaseOrderer("XUnit.Case.Orderers.TestExeOrderer", "Cases.ExeOrder")]
    [Collection("Database collection")]

    public class StableStmtCases
    {

        DatabaseFixture database;
        private readonly ITestOutputHelper _output;


        public StableStmtCases(DatabaseFixture fixture, ITestOutputHelper output)
        {
            this.database = fixture;
            this._output = output;
        }
        /// <author>xiaolei</author>
        /// <Name>StableStmtCases.TestBindSingleLineCN</Name>
        /// <describe>Test stmt insert single line of Chinese character into stable by column after column </describe>
        /// <filename>StmtSTable.cs</filename>
        /// <result>pass or failed </result> 
        [Fact(DisplayName = "StableStmtCases.TestBindSingleLineCN()"), TestExeOrder(2), Trait("Category", "BindParamCN")]
        public void TestBindSingleLineCN()
        {
            string tableName = "stb_stmt_cases_test_bind_single_line_cn";
            String createSql = $"create stable if not exists {tableName} " +
                                " (ts timestamp," +
                                "v1 tinyint," +
                                "v2 smallint," +
                                "v4 int," +
                                "v8 bigint," +
                                "u1 tinyint unsigned," +
                                "u2 smallint unsigned," +
                                "u4 int unsigned," +
                                "u8 bigint unsigned," +
                                "f4 float," +
                                "f8 double," +
                                "bin binary(200)," +
                                "blob nchar(200)," +
                                "b bool," +
                                "nilcol int)" +
                                "tags" +
                                "(bo bool," +
                                "tt tinyint," +
                                "si smallint," +
                                "ii int," +
                                "bi bigint," +
                                "tu tinyint unsigned," +
                                "su smallint unsigned," +
                                "iu int unsigned," +
                                "bu bigint unsigned," +
                                "ff float," +
                                "dd double," +
                                "bb binary(200)," +
                                "nc nchar(200)" +
                                ");";
            String insertSql = $"insert into ? using  {tableName} tags(?,?,?,?,?,?,?,?,?,?,?,?,?) values(?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)";
            String dropSql = $"drop table if exists {tableName} ;";
            List<TDengineMeta> expectResMeta = DataSource.GetMetaFromDDL(createSql);
            List<String> expectResData = DataSource.GetSTableCNRowData();
            TAOS_BIND[] tags = DataSource.GetCNTags();
            TAOS_BIND[] binds = DataSource.GetNTableCNRow();

            IntPtr conn = database.conn;
            UtilsTools.ExecuteUpdate(conn, dropSql, _output);
            UtilsTools.ExecuteUpdate(conn, createSql, _output);

            IntPtr stmt = StmtUtilTools.StmtInit(conn);
            StmtUtilTools.StmtPrepare(stmt, insertSql);
            StmtUtilTools.SetTableNameTags(stmt, tableName + "_t1", tags);
            StmtUtilTools.BindParam(stmt, binds);
            StmtUtilTools.AddBatch(stmt);
            StmtUtilTools.StmtExecute(stmt);
            StmtUtilTools.StmtClose(stmt);

            DataSource.FreeTaosBind(tags);
            DataSource.FreeTaosBind(binds);

            string querySql = "select * from " + tableName;
            IntPtr res = UtilsTools.ExecuteQuery(conn, querySql, _output);
            ResultSet actualResult = new ResultSet(res);

            List<TDengineMeta> actualResMeta = actualResult.GetResultMeta();
            List<string> actualResData = actualResult.GetResultData();

            // Assert retrieve data
            _output.WriteLine("Assert retrieve data");

            for (int i = 0; i < actualResData.Count; i++)
            {
                //_output.WriteLine("expect:{0},actual:{1}", expectResData[i], actualResData[i]);
                Assert.Equal(expectResData[i], actualResData[i]);
            }
            // Assert meta data
            _output.WriteLine("Assert meta data");

            for (int i = 0; i < actualResMeta.Count; i++)
            {
                Assert.Equal(expectResMeta[i].name, actualResMeta[i].name);
                Assert.Equal(expectResMeta[i].type, actualResMeta[i].type);
                Assert.Equal(expectResMeta[i].size, actualResMeta[i].size);
            }
            _output.WriteLine("StableStmtCases.TestBindSingleLineCN() pass");

        }

        /// <author>xiaolei</author>
        /// <Name>StableStmtCases.TestBindColumnCN</Name>
        /// <describe>Test stmt insert single line of Chinese character into stable by column after column </describe>
        /// <filename>StmtSTable.cs</filename>
        /// <result>pass or failed </result>
        [Fact(DisplayName = "StableStmtCases.TestBindColumnCN()"), TestExeOrder(4), Trait("Category", "BindParamColumnCN")]
        public void TestBindColumnCN()
        {
            string tableName = "stb_stmt_cases_test_bindcolumn_cn";
            String createSql = $"create stable  if not exists {tableName} " +
                                "(ts timestamp," +
                                "b bool," +
                                "v1 tinyint," +
                                "v2 smallint," +
                                "v4 int," +
                                "v8 bigint," +
                                "f4 float," +
                                "f8 double," +
                                "u1 tinyint unsigned," +
                                "u2 smallint unsigned," +
                                "u4 int unsigned," +
                                "u8 bigint unsigned," +
                                "bin binary(200)," +
                                "blob nchar(200)" +
                                ")" +
                                "tags" +
                                "(bo bool," +
                                "tt tinyint," +
                                "si smallint," +
                                "ii int," +
                                "bi bigint," +
                                "tu tinyint unsigned," +
                                "su smallint unsigned," +
                                "iu int unsigned," +
                                "bu bigint unsigned," +
                                "ff float," +
                                "dd double," +
                                "bb binary(200)," +
                                "nc nchar(200)" +
                                ");";
            String insertSql = "insert into ? using " + tableName + " tags(?,?,?,?,?,?,?,?,?,?,?,?,?) values(?,?,?,?,?,?,?,?,?,?,?,?,?,?)";
            String dropSql = $"drop table if exists {tableName};";
            TAOS_BIND[] tags = DataSource.GetCNTags();
            TAOS_MULTI_BIND[] mBinds = DataSource.GetMultiBindCNArr();
            List<TDengineMeta> expectResMeta = DataSource.GetMetaFromDDL(createSql);
            List<String> expectResData = DataSource.GetMultiBindStableCNRowData();

            IntPtr conn = database.conn;
            UtilsTools.ExecuteUpdate(conn, dropSql, _output);
            UtilsTools.ExecuteUpdate(conn, createSql, _output);

            IntPtr stmt = StmtUtilTools.StmtInit(conn);
            StmtUtilTools.StmtPrepare(stmt, insertSql);
            StmtUtilTools.SetTableNameTags(stmt, tableName + "_t1", tags);

            StmtUtilTools.BindSingleParamBatch(stmt, mBinds[0], 0);
            StmtUtilTools.BindSingleParamBatch(stmt, mBinds[1], 1);
            StmtUtilTools.BindSingleParamBatch(stmt, mBinds[2], 2);
            StmtUtilTools.BindSingleParamBatch(stmt, mBinds[3], 3);
            StmtUtilTools.BindSingleParamBatch(stmt, mBinds[4], 4);
            StmtUtilTools.BindSingleParamBatch(stmt, mBinds[5], 5);
            StmtUtilTools.BindSingleParamBatch(stmt, mBinds[6], 6);
            StmtUtilTools.BindSingleParamBatch(stmt, mBinds[7], 7);
            StmtUtilTools.BindSingleParamBatch(stmt, mBinds[8], 8);
            StmtUtilTools.BindSingleParamBatch(stmt, mBinds[9], 9);
            StmtUtilTools.BindSingleParamBatch(stmt, mBinds[10], 10);
            StmtUtilTools.BindSingleParamBatch(stmt, mBinds[11], 11);
            StmtUtilTools.BindSingleParamBatch(stmt, mBinds[12], 12);
            StmtUtilTools.BindSingleParamBatch(stmt, mBinds[13], 13);

            StmtUtilTools.AddBatch(stmt);
            StmtUtilTools.StmtExecute(stmt);
            StmtUtilTools.StmtClose(stmt);

            DataSource.FreeTaosBind(tags);
            DataSource.FreeTaosMBind(mBinds);

            string querySql = "select * from " + tableName;
            IntPtr res = UtilsTools.ExecuteQuery(conn, querySql, _output);
            ResultSet actualResult = new ResultSet(res);

            List<TDengineMeta> actualResMeta = actualResult.GetResultMeta();
            List<string> actualResData = actualResult.GetResultData();

            // Assert retrieve data
            _output.WriteLine("Assert retrieve data");

            for (int i = 0; i < actualResData.Count; i++)
            {
                // _output.WriteLine("expect:{0},actual:{1}", expectResData[i], actualResData[i]);
                Assert.Equal(expectResData[i], actualResData[i]);
            }
            // Assert meta data
            _output.WriteLine("Assert meta data");

            for (int i = 0; i < actualResMeta.Count; i++)
            {
                Assert.Equal(expectResMeta[i].name, actualResMeta[i].name);
                Assert.Equal(expectResMeta[i].type, actualResMeta[i].type);
                Assert.Equal(expectResMeta[i].size, actualResMeta[i].size);
            }
            _output.WriteLine("StableStmtCases.TestBindColumnCN() pass");

        }

        /// <author>xiaolei</author>
        /// <Name>StableStmtCases.TestBindMultiLineCN</Name>
        /// <describe>Test stmt insert single line of Chinese character into stable by column after column </describe>
        /// <filename>StmtSTable.cs</filename>
        /// <result>pass or failed </result>
        [Fact(DisplayName = "StableStmtCases.TestBindMultiLineCN()"), TestExeOrder(6), Trait("Category", "BindParamBatchCN")]
        public void TestBindMultiLineCN()
        {
            string tableName = "stb_stmt_cases_test_bind_multi_line_cn";
            String createSql = $"create stable  if not exists {tableName} " +
                                "(ts timestamp," +
                                "b bool," +
                                "v1 tinyint," +
                                "v2 smallint," +
                                "v4 int," +
                                "v8 bigint," +
                                "f4 float," +
                                "f8 double," +
                                "u1 tinyint unsigned," +
                                "u2 smallint unsigned," +
                                "u4 int unsigned," +
                                "u8 bigint unsigned," +
                                "bin binary(200)," +
                                "blob nchar(200)" +
                                ")" +
                                "tags" +
                                "(bo bool," +
                                "tt tinyint," +
                                "si smallint," +
                                "ii int," +
                                "bi bigint," +
                                "tu tinyint unsigned," +
                                "su smallint unsigned," +
                                "iu int unsigned," +
                                "bu bigint unsigned," +
                                "ff float," +
                                "dd double," +
                                "bb binary(200)," +
                                "nc nchar(200)" +
                                ");";
            String insertSql = "insert into ? using " + tableName + " tags(?,?,?,?,?,?,?,?,?,?,?,?,?) values(?,?,?,?,?,?,?,?,?,?,?,?,?,?)";
            String dropSql = $"drop table if exists {tableName};";
            TAOS_BIND[] tags = DataSource.GetCNTags();
            TAOS_MULTI_BIND[] mBinds = DataSource.GetMultiBindCNArr();
            List<TDengineMeta> expectResMeta = DataSource.GetMetaFromDDL(createSql);
            List<String> expectResData = DataSource.GetMultiBindStableCNRowData();

            IntPtr conn = database.conn;
            UtilsTools.ExecuteUpdate(conn, dropSql, _output);
            UtilsTools.ExecuteUpdate(conn, createSql, _output);

            IntPtr stmt = StmtUtilTools.StmtInit(conn);
            StmtUtilTools.StmtPrepare(stmt, insertSql);
            StmtUtilTools.SetTableNameTags(stmt, tableName + "_t1", tags);
            StmtUtilTools.BindParamBatch(stmt, mBinds);
            StmtUtilTools.AddBatch(stmt);
            StmtUtilTools.StmtExecute(stmt);

            StmtUtilTools.StmtClose(stmt);
            DataSource.FreeTaosBind(tags);
            DataSource.FreeTaosMBind(mBinds);

            string querySql = "select * from " + tableName;
            IntPtr res = UtilsTools.ExecuteQuery(conn, querySql, _output);
            ResultSet actualResult = new ResultSet(res);

            List<TDengineMeta> actualResMeta = actualResult.GetResultMeta();
            List<string> actualResData = actualResult.GetResultData();

            // Assert retrieve data
            _output.WriteLine("Assert retrieve data");

            for (int i = 0; i < actualResData.Count; i++)
            {
                // _output.WriteLine("expect:{0},actual:{1}", expectResData[i], actualResData[i]);
                Assert.Equal(expectResData[i], actualResData[i]);
            }
            // Assert meta data
            _output.WriteLine("Assert meta data");

            for (int i = 0; i < actualResMeta.Count; i++)
            {
                Assert.Equal(expectResMeta[i].name, actualResMeta[i].name);
                Assert.Equal(expectResMeta[i].type, actualResMeta[i].type);
                Assert.Equal(expectResMeta[i].size, actualResMeta[i].size);
            }
            _output.WriteLine("StableStmtCases.TestBindMultiLineCN() pass");
        }

        /// <author>xiaolei</author>
        /// <Name>StableStmtCases.TestBindMultiLine</Name>
        /// <describe>Test stmt insert single line into stable by column after column </describe>
        /// <filename>StmtSTable.cs</filename>
        /// <result>pass or failed </result>         
        [Fact(DisplayName = "StableStmtCases.TestBindMultiLine()"), TestExeOrder(5), Trait("Category", "BindParamBatch")]
        public void TestBindMultiLine()
        {
            string tableName = "stb_stmt_cases_test_bind_multi_line";
            string createSql = $"create stable if not exists {tableName} " +
                                "(ts timestamp," +
                                "b bool," +
                                "v1 tinyint," +
                                "v2 smallint," +
                                "v4 int," +
                                "v8 bigint," +
                                "f4 float," +
                                "f8 double," +
                                "u1 tinyint unsigned," +
                                "u2 smallint unsigned," +
                                "u4 int unsigned," +
                                "u8 bigint unsigned," +
                                "bin binary(200)," +
                                "blob nchar(200)" +
                                ")" +
                                "tags" +
                                "(bo bool," +
                                "tt tinyint," +
                                "si smallint," +
                                "ii int," +
                                "bi bigint," +
                                "tu tinyint unsigned," +
                                "su smallint unsigned," +
                                "iu int unsigned," +
                                "bu bigint unsigned," +
                                "ff float," +
                                "dd double," +
                                "bb binary(200)," +
                                "nc nchar(200)" +
                                ");";
            String insertSql = "insert into ? using " + tableName + " tags(?,?,?,?,?,?,?,?,?,?,?,?,?) values(?,?,?,?,?,?,?,?,?,?,?,?,?,?)";
            String dropSql = $"drop table if exists {tableName};";
            TAOS_BIND[] tags = DataSource.GetTags();
            TAOS_MULTI_BIND[] mBinds = DataSource.GetMultiBindArr();
            List<TDengineMeta> expectResMeta = DataSource.GetMetaFromDDL(createSql);
            List<String> expectResData = DataSource.GetMultiBindStableRowData();

            IntPtr conn = database.conn;
            UtilsTools.ExecuteUpdate(conn, dropSql, _output);
            UtilsTools.ExecuteUpdate(conn, createSql, _output);

            IntPtr stmt = StmtUtilTools.StmtInit(conn);
            StmtUtilTools.StmtPrepare(stmt, insertSql);
            StmtUtilTools.SetTableNameTags(stmt, tableName + "_t1", tags);
            StmtUtilTools.BindParamBatch(stmt, mBinds);
            StmtUtilTools.AddBatch(stmt);
            StmtUtilTools.StmtExecute(stmt);
            StmtUtilTools.StmtClose(stmt);

            DataSource.FreeTaosBind(tags);
            DataSource.FreeTaosMBind(mBinds);

            string querySql = "select * from " + tableName;
            IntPtr res = UtilsTools.ExecuteQuery(conn, querySql, _output);
            ResultSet actualResult = new ResultSet(res);

            List<TDengineMeta> actualResMeta = actualResult.GetResultMeta();
            List<string> actualResData = actualResult.GetResultData();

            // Assert retrieve data
            _output.WriteLine("Assert retrieve data");

            for (int i = 0; i < actualResData.Count; i++)
            {
                // _output.WriteLine("expect:{0},actual:{1}", expectResData[i], actualResData[i]);
                Assert.Equal(expectResData[i], actualResData[i]);
            }
            // Assert meta data
            _output.WriteLine("Assert meta data");

            for (int i = 0; i < actualResMeta.Count; i++)
            {
                Assert.Equal(expectResMeta[i].name, actualResMeta[i].name);
                Assert.Equal(expectResMeta[i].type, actualResMeta[i].type);
                Assert.Equal(expectResMeta[i].size, actualResMeta[i].size);
            }
            _output.WriteLine("StableStmtCases.TestBindMultiLine() pass");
        }

        /// <author>xiaolei</author>
        /// <Name>StableStmtCases.TestBindColumn</Name>
        /// <describe>Test stmt insert single line of Chinese character into stable by column after column </describe>
        /// <filename>StmtSTable.cs</filename>
        /// <result>pass or failed </result> 
        [Fact(DisplayName = "StableStmtCases.TestBindColumn()"), TestExeOrder(3), Trait("Category", "BindParamColumn")]
        public void TestBindColumn()
        {
            string tableName = "stb_stmt_cases_test_bindcolumn";
            string createSql = $"create stable  if not exists {tableName} " +
                                "(ts timestamp," +
                                "b bool," +
                                "v1 tinyint," +
                                "v2 smallint," +
                                "v4 int," +
                                "v8 bigint," +
                                "f4 float," +
                                "f8 double," +
                                "u1 tinyint unsigned," +
                                "u2 smallint unsigned," +
                                "u4 int unsigned," +
                                "u8 bigint unsigned," +
                                "bin binary(200)," +
                                "blob nchar(200)" +
                                ")" +
                                "tags" +
                                "(bo bool," +
                                "tt tinyint," +
                                "si smallint," +
                                "ii int," +
                                "bi bigint," +
                                "tu tinyint unsigned," +
                                "su smallint unsigned," +
                                "iu int unsigned," +
                                "bu bigint unsigned," +
                                "ff float," +
                                "dd double," +
                                "bb binary(200)," +
                                "nc nchar(200)" +
                                ");";
            String insertSql = "insert into ? using " + tableName + " tags(?,?,?,?,?,?,?,?,?,?,?,?,?) values(?,?,?,?,?,?,?,?,?,?,?,?,?,?)";
            String dropSql = $"drop table if exists {tableName};";
            TAOS_BIND[] tags = DataSource.GetTags();
            TAOS_MULTI_BIND[] mBinds = DataSource.GetMultiBindArr();
            List<TDengineMeta> expectResMeta = DataSource.GetMetaFromDDL(createSql);
            List<String> expectResData = DataSource.GetMultiBindStableRowData();

            IntPtr conn = database.conn;
            UtilsTools.ExecuteUpdate(conn, dropSql, _output);
            UtilsTools.ExecuteUpdate(conn, createSql, _output);

            IntPtr stmt = StmtUtilTools.StmtInit(conn);
            StmtUtilTools.StmtPrepare(stmt, insertSql);

            StmtUtilTools.SetTableNameTags(stmt, tableName + "_t1", tags);
            StmtUtilTools.BindSingleParamBatch(stmt, mBinds[0], 0);
            StmtUtilTools.BindSingleParamBatch(stmt, mBinds[1], 1);
            StmtUtilTools.BindSingleParamBatch(stmt, mBinds[2], 2);
            StmtUtilTools.BindSingleParamBatch(stmt, mBinds[3], 3);
            StmtUtilTools.BindSingleParamBatch(stmt, mBinds[4], 4);
            StmtUtilTools.BindSingleParamBatch(stmt, mBinds[5], 5);
            StmtUtilTools.BindSingleParamBatch(stmt, mBinds[6], 6);
            StmtUtilTools.BindSingleParamBatch(stmt, mBinds[7], 7);
            StmtUtilTools.BindSingleParamBatch(stmt, mBinds[8], 8);
            StmtUtilTools.BindSingleParamBatch(stmt, mBinds[9], 9);
            StmtUtilTools.BindSingleParamBatch(stmt, mBinds[10], 10);
            StmtUtilTools.BindSingleParamBatch(stmt, mBinds[11], 11);
            StmtUtilTools.BindSingleParamBatch(stmt, mBinds[12], 12);
            StmtUtilTools.BindSingleParamBatch(stmt, mBinds[13], 13);

            StmtUtilTools.AddBatch(stmt);
            StmtUtilTools.StmtExecute(stmt);
            StmtUtilTools.StmtClose(stmt);

            DataSource.FreeTaosBind(tags);
            DataSource.FreeTaosMBind(mBinds);

            string querySql = "select * from " + tableName;
            IntPtr res = UtilsTools.ExecuteQuery(conn, querySql, _output);
            ResultSet actualResult = new ResultSet(res);

            List<TDengineMeta> actualResMeta = actualResult.GetResultMeta();
            List<string> actualResData = actualResult.GetResultData();

            // Assert retrieve data
            _output.WriteLine("Assert retrieve data");

            for (int i = 0; i < actualResData.Count; i++)
            {
                // _output.WriteLine("expect:{0},actual:{1}", expectResData[i], actualResData[i]);
                Assert.Equal(expectResData[i], actualResData[i]);
            }
            // Assert meta data
            _output.WriteLine("Assert meta data");

            for (int i = 0; i < actualResMeta.Count; i++)
            {
                Assert.Equal(expectResMeta[i].name, actualResMeta[i].name);
                Assert.Equal(expectResMeta[i].type, actualResMeta[i].type);
                Assert.Equal(expectResMeta[i].size, actualResMeta[i].size);
            }
            _output.WriteLine("StableStmtCases.TestBindColumn() pass");

        }

    }
}