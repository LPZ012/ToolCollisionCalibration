using ToolCollisionCalibration.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFLibrary.DataBase.Dappers.SqlsClientDapper;

namespace ToolCollisionCalibration.Devices
{
    public class SqlClient : AbsSqlClientDapper,ISqlClient
    {
        public SqlClient(string sqlConnectionString) : base(sqlConnectionString)
        {
        }

        public bool UpDateParameters(DBParams dBParams,string LineNum)
        {
            string JsonText = JsonConvert.SerializeObject(dBParams);
            string sql = $"Update [dbo].[ParamsConfiguration] set ToolCollisionCalibration=N'{JsonText}' Where LineNum='{LineNum}'";
            int Line = ExecuteNonQuery(sql);
            if (Line > 0) return true;
            else return false;
        }

        public bool CheckKanbanInfo(string Line, string CustModel)
        {
            string sql = $"SELECT Top 1 * FROM [dbo].[KanbanInfo] WHERE Line='{Line}' " +
                $"AND CustModel='{CustModel}' AND using = 1";
            object o = ExecuteScalar(sql);
            if (o == null) return false;
            return true;
        }

        public bool CheckPreviousStation(string SN_CODE, string ProdLineNo, string Product_Model)
        {
            string sql = $"SELECT TOP 1 * FROM [dbo].[BDP_WaterTightTest] WHERE SN_Code='{SN_CODE}' AND ProdLineNo='{ProdLineNo}' AND ProductModel='{Product_Model}' AND TestResult='{true}' AND Rework=0 order by UploadTime desc";
            object r = ExecuteScalar(sql);
            if (r == null) return false;
            return true;
        }

        public DBParams GetParameters(string LineNum)
        {
            string sql = $"Select ToolCollisionCalibration From [dbo].[ParamsConfiguration] Where LineNum='{LineNum}'";
            object obj = ExecuteScalar(sql);
            if (obj == null) return null;
            return JsonConvert.DeserializeObject<DBParams>(Convert.ToString(obj));
        }

        public bool NumberDurationOfFailureChecks(string SN_CODE, int SetCount, int SetTime)
        {
            string coutsql = $"SELECT COUNT(*) FROM [dbo].[BDP1HPBoilingwaterTest] WHERE SN_Code='{SN_CODE}' AND TestResult = '{false}'And Rework=0";
            int Count = (int)ExecuteScalar(coutsql);
            if (Count > 0)
            {
                if (Count >= SetCount) return false;
                string firsttimesql = $"SELECT TOP 1 UploadTime FROM [dbo].[BDP1HPBoilingwaterTest] WHERE SN_Code='{SN_CODE}' AND TestResult = '{false}'And Rework=0 ORDER BY UpLoadTime DESC";
                DateTime FirstUploadTime = (DateTime)ExecuteScalar(firsttimesql);
                string currentsqltime = "SELECT GETDATE()";
                DateTime CurrentsqlTime = (DateTime)ExecuteScalar(currentsqltime);
                if ((CurrentsqlTime - FirstUploadTime).TotalMinutes > SetTime) return false;
                return true;
            }
            else return true;

        }



        public bool UpLoad(DataBaseModel dataBaseModel)
        {
            string sql = @"
            INSERT INTO [dbo].[ToolCrashAssemblyTest] (
            [OrderNum], [WorkStation], [ProductModel], [BarCodeNumber], 
            [SN_Code], [ProdLineNo],  [TestResult], 
            [ErrorReportingStep], [ErrorReportingInformation], [StartingAngle], 
            [EndAngle], [AngleDifference], [InvertAngleCompensation], 
            [StartingTorque], [EndTorque], [TorqueDifference]
            ) VALUES (
            @OrderNum, @WorkStation, @ProductModel, @BarCodeNumber, 
            @SN_Code, @ProdLineNo,  @TestResult, 
            @ErrorReportingStep, @ErrorReportingInformation, @StartingAngle, 
            @EndAngle, @AngleDifference, @InvertAngleCompensation, 
            @StartingTorque, @EndTorque, @TorqueDifference
            )";
            return Execute(sql, dataBaseModel) > 0;
        }
    }
}
