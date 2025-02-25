using TDengineDriver;
using Test.UtilsTools;
using System;
using Xunit;
using System.Collections.Generic;
using Test.UtilsTools.DataSource;
using Test.UtilsTools.ResultSet;
using Xunit.Abstractions;
using Test.Fixture;
using Test.Case.Attributes;
using System.Threading;

namespace Cases
{
    [TestCaseOrderer("XUnit.Case.Orderers.TestExeOrderer", "Cases.ExeOrder")]
    [Collection("Database collection")]

    public class QueryAsyncCases
    {
        DatabaseFixture database;

        private readonly ITestOutputHelper _output;

        public QueryAsyncCases(DatabaseFixture fixture, ITestOutputHelper output)
        {
            this.database = fixture;
            this._output = output;
        }
        /// <author>xiaolei</author>
        /// <Name>QueryAsyncCases.QueryAsyncCases</Name>
        /// <describe>Test query without condition</describe>
        /// <filename>QueryAsync.cs</filename>
        /// <result>pass or failed </result> 
        [Fact(DisplayName = "QueryAsyncCases.QueryWithoutCondition()"), TestExeOrder(1), Trait("Category", "QueryAWithoutCondition")]
        public void QueryWithoutCondition()
        {
            IntPtr conn = database.conn;
            IntPtr _res = IntPtr.Zero;

            var tableName = "query_a_without_condition";
            var createSql = $"create table if not exists {tableName}(ts timestamp,bl bool,i8 tinyint,i16 smallint,i32 int,i64 bigint,bnr binary(50),nchr nchar(50))tags(t_i32 int,t_bnr binary(50),t_nchr nchar(50))";
            var dropSql = $"drop table if exists {tableName}";

            var colData = new List<Object>{1646150410100,true,1,11,1111,11111111,"value one","值壹",
            1646150410200,true,2,22,2222,22222222,"value two","值贰",
            1646150410300,false,3,33,3333,33333333,"value three","值三",
            };
            var tagData = new List<Object> { 1, "tag_one", "标签壹" };
            String insertSql = UtilsTools.ConstructInsertSql(tableName + "_s01", tableName, colData, tagData, 3);
            List<TDengineMeta> expectResMeta = DataSource.GetMetaFromDDL(createSql);
            List<Object> expectResData = UtilsTools.CombineColAndTagData(colData, tagData, 3);

            var querySql = $"select * from {tableName}";
            UtilsTools.ExecuteUpdate(conn, dropSql, _output);
            UtilsTools.ExecuteUpdate(conn, createSql, _output);
            UtilsTools.ExecuteUpdate(conn, insertSql, _output);

            QueryAsyncCallback fq = new QueryAsyncCallback(QueryCallback);
            TDengine.QueryAsync(conn, querySql, fq, IntPtr.Zero);
            Thread.Sleep(2000);
            void QueryCallback(IntPtr param, IntPtr taosRes, int code)
            {
                if (code == 0 && taosRes != IntPtr.Zero)
                {
                    FetchRowAsyncCallback fetchRowAsyncCallback = new FetchRowAsyncCallback(FetchCallback);
                    TDengine.FetchRowAsync(taosRes, fetchRowAsyncCallback, param);
                }
                else
                {
                    _output.WriteLine($"async query data failed, failed code {code}");
                }

            }

            void FetchCallback(IntPtr param, IntPtr taosRes, int numOfRows)
            {
                if (numOfRows > 0)
                {
                    ResultSet actualResult = new ResultSet(taosRes);
                    List<TDengineMeta> actualMeta = actualResult.GetResultMeta();
                    List<String> actualResData = actualResult.GetResultData();
                    //Assert Meta data
                    _output.WriteLine("Assert Meta data");
                    for (int i = 0; i < actualMeta.Count; i++)
                    {
                        Assert.Equal(expectResMeta[i].name, actualMeta[i].name);
                        Assert.Equal(expectResMeta[i].type, actualMeta[i].type);
                        Assert.Equal(expectResMeta[i].size, actualMeta[i].size);
                    }
                    // Assert retrieve data
                    _output.WriteLine("Assert retrieve data");

                    for (int i = 0; i < actualResData.Count; i++)
                    {
                        //_output.WriteLine("{0},{1},{2}", i, expectResData[i], actualResData[i]);
                        Assert.Equal(expectResData[i].ToString(), actualResData[i]);
                    }

                    TDengine.FetchRowAsync(taosRes, FetchCallback, param);
                }
                else
                {
                    if (numOfRows == 0)
                    {
                        _output.WriteLine("async retrieve complete.");

                    }
                    else
                    {
                        _output.WriteLine($"FetchRowAsync callback error, error code {numOfRows}");
                    }
                    TDengine.FreeResult(taosRes);
                }
            }
            _output.WriteLine("QueryAsyncCases.QueryWithoutCondition() pass");

        }

        /// <author>xiaolei</author>
        /// <Name>QueryAsyncCases.QueryWithCondition</Name>
        /// <describe>Test query with condition</describe>
        /// <filename>QueryAsync.cs</filename>
        /// <result>pass or failed </result> 
        [Fact(DisplayName = "QueryAsyncCases.QueryWithCondition()"), TestExeOrder(2), Trait("Category", "QueryAWithCondition")]
        public void QueryWithCondition()
        {
            IntPtr conn = database.conn;
            IntPtr _res = IntPtr.Zero;

            var tableName = "query_a_with_condition";
            var createSql = $"create table if not exists {tableName}(ts timestamp,bl bool,i8 tinyint,i16 smallint,i32 int,i64 bigint,bnr binary(50),nchr nchar(50))tags(t_i32 int,t_bnr binary(50),t_nchr nchar(50))";
            var dropSql = $"drop table if exists {tableName}";
            var querySql = $"select * from {tableName} where bl=true and t_bnr='tag_one' and i8>1 and i8>1 and t_nchr = '标签壹' ";

            var colData = new List<Object>{1646150410100,true,1,11,1111,11111111,"value one","值壹",
            1646150410200,true,2,22,2222,22222222,"value two","值贰",
            1646150410300,false,3,33,3333,33333333,"value three","值三",
            };
            var colDataActual = colData.GetRange(8, 8);
            var tagData = new List<Object> { 1, "tag_one", "标签壹" };
            String insertSql = UtilsTools.ConstructInsertSql(tableName + "_s01", tableName, colData, tagData, 3);
            List<TDengineMeta> expectResMeta = DataSource.GetMetaFromDDL(createSql);
            List<Object> expectResData = UtilsTools.CombineColAndTagData(colDataActual, tagData, 1);

            UtilsTools.ExecuteUpdate(conn, dropSql, _output);
            UtilsTools.ExecuteUpdate(conn, createSql, _output);
            UtilsTools.ExecuteUpdate(conn, insertSql, _output);

            QueryAsyncCallback fq = new QueryAsyncCallback(QueryCallback);
            TDengine.QueryAsync(conn, querySql, fq, IntPtr.Zero);
            Thread.Sleep(2000);
            void QueryCallback(IntPtr param, IntPtr taosRes, int code)
            {
                _output.WriteLine("code:{0}", querySql);
                if (code == 0 && taosRes != IntPtr.Zero)
                {
                    FetchRowAsyncCallback fetchRowAsyncCallback = new FetchRowAsyncCallback(FetchCallback);
                    TDengine.FetchRowAsync(taosRes, fetchRowAsyncCallback, param);
                }
                else
                {
                    throw new Exception($"async query data failed, failed reason:{TDengine.Error(taosRes)} {code}");
                }

            }

            void FetchCallback(IntPtr param, IntPtr taosRes, int numOfRows)
            {
                if (numOfRows > 0)
                {
                    ResultSet actualResult = new ResultSet(taosRes);
                    List<TDengineMeta> actualMeta = actualResult.GetResultMeta();
                    List<String> actualResData = actualResult.GetResultData();

                    //Assert Meta data
                    _output.WriteLine("Assert Meta data");

                    for (int i = 0; i < actualMeta.Count; i++)
                    {
                        Assert.Equal(expectResMeta[i].name, actualMeta[i].name);
                        Assert.Equal(expectResMeta[i].type, actualMeta[i].type);
                        Assert.Equal(expectResMeta[i].size, actualMeta[i].size);
                    }
                    // Assert retrieve data
                    _output.WriteLine("Assert retrieve data");

                    for (int i = 0; i < actualResData.Count; i++)
                    {
                        //_output.WriteLine("{0},{1}", expectResData[i].ToString(), actualResData[i]);
                        Assert.Equal(expectResData[i].ToString(), actualResData[i]);
                    }
                    TDengine.FetchRowAsync(taosRes, FetchCallback, param);
                }
                else
                {
                    if (numOfRows == 0)
                    {
                        _output.WriteLine("async retrieve complete.");

                    }
                    else
                    {
                        throw new Exception($"FetchRowAsync callback error, failed reason:{TDengine.Error(taosRes)}");
                    }
                    TDengine.FreeResult(taosRes);
                }
            }
            _output.WriteLine("QueryAsyncCases.QueryWithoutCondition() pass");
        }

        /// <author>xiaolei</author>
        /// <Name>QueryAsyncCases.QueryWithJsonCondition</Name>
        /// <describe>Test query with condition</describe>
        /// <filename>QueryAsync.cs</filename>
        /// <result>pass or failed </result> 
        [Fact(DisplayName = "QueryAsyncCases.QueryWithJsonCondition()"), TestExeOrder(3), Trait("Category", "QueryAWithJsonCondition")]
        public void QueryWithJsonCondition()
        {
            IntPtr conn = database.conn;
            IntPtr _res = IntPtr.Zero;

            var tableName = "query_a_json_condition";
            var createSql = $"create table if not exists {tableName}(ts timestamp,bl bool,i8 tinyint,i16 smallint,i32 int,i64 bigint,bnr binary(50),nchr nchar(50))tags(jtag json)";
            var dropSql = $"drop table if exists {tableName}";

            var colData1 = new List<Object>{1646150410100,true,1,11,1111,11111111,"value one","值壹",
            1646150410200,true,2,22,2222,22222222,"value two","值贰",
            1646150410300,false,3,33,3333,33333333,"value three","值三",
            };
            var colData2 = new List<Object>{1646150410400,false,4,44,4444,44444444,"value three","值肆",
            1646150410500,true,5,55,5555,55555555,"value one","值伍",
            1646150410600,true,6,66,6666,66666666,"value two","值陆",
            };
            var tagData1 = new List<Object> { "{\"t_bnr\":\"tag1\",\"t_i32\":1,\"t_nchr\":\"标签壹\"}" };
            var tagData2 = new List<Object> { "{\"t_bnr\":\"tag2\",\"t_i32\":2,\"t_nchar\":\"标签贰\"}" };
            var querySql = $"select * from {tableName} where jtag->'t_bnr'='tag1';";


            String insertSql1 = UtilsTools.ConstructInsertSql(tableName + "_s01", tableName, colData1, tagData1, 3);
            String insertSql2 = UtilsTools.ConstructInsertSql(tableName + "_s02", tableName, colData1, tagData2, 3);
            List<TDengineMeta> expectResMeta = DataSource.GetMetaFromDDL(createSql);
            List<Object> expectResData = UtilsTools.CombineColAndTagData(colData1, tagData1, 3);

            UtilsTools.ExecuteUpdate(conn, dropSql, _output);
            UtilsTools.ExecuteUpdate(conn, createSql, _output);
            UtilsTools.ExecuteUpdate(conn, insertSql1, _output);
            UtilsTools.ExecuteUpdate(conn, insertSql2, _output);
            QueryAsyncCallback fq = new QueryAsyncCallback(QueryCallback);
            TDengine.QueryAsync(conn, querySql, fq, IntPtr.Zero);
            Thread.Sleep(2000);
            void QueryCallback(IntPtr param, IntPtr taosRes, int code)
            {
                if (code == 0 && taosRes != IntPtr.Zero)
                {
                    FetchRowAsyncCallback fetchRowAsyncCallback = new FetchRowAsyncCallback(FetchCallback);
                    TDengine.FetchRowAsync(taosRes, fetchRowAsyncCallback, param);
                }
                else
                {
                    _output.WriteLine($"async query data failed, failed code {code}");
                }

            }

            void FetchCallback(IntPtr param, IntPtr taosRes, int numOfRows)
            {
                if (numOfRows > 0)
                {
                    ResultSet actualResult = new ResultSet(taosRes);
                    List<TDengineMeta> actualMeta = actualResult.GetResultMeta();
                    List<String> actualResData = actualResult.GetResultData();
                    //Assert Meta data
                    _output.WriteLine("Assert Meta data");

                    for (int i = 0; i < actualMeta.Count; i++)
                    {
                        Assert.Equal(expectResMeta[i].name, actualMeta[i].name);
                        Assert.Equal(expectResMeta[i].type, actualMeta[i].type);
                        Assert.Equal(expectResMeta[i].size, actualMeta[i].size);
                    }
                    // Assert retrieve data
                    _output.WriteLine("Assert retrieve data");

                    for (int i = 0; i < actualResData.Count; i++)
                    {
                        // _output.WriteLine("expect:{0},actual:{1}", expectResData[i], actualResData[i]);
                        Assert.Equal(expectResData[i].ToString(), actualResData[i]);
                    }

                    TDengine.FetchRowAsync(taosRes, FetchCallback, param);
                }
                else
                {
                    if (numOfRows == 0)
                    {
                        _output.WriteLine("async retrieve complete.");

                    }
                    else
                    {
                        _output.WriteLine($"FetchRowAsync callback error, error code {numOfRows}");
                    }
                    TDengine.FreeResult(taosRes);
                }
            }
            _output.WriteLine("QueryAsyncCases.QueryWithJsonCondition() pass");
        }
    }
}
