using System.Collections.Generic;
using System.Data;
using Sybase.Data.AseClient;

namespace WinmedWebServiceNET
{
    public class SybaseData
    {
        public AseConnection sysConn;
        public AseCommand sybCommand;

        public SybaseData()
        {
            sysConn = new AseConnection(); //"Data Source=150.130.102.56; Port=4032; Database=audiorespuesta_prd; Uid=conswmed; Pwd=vitamed1; ConnectionIdleTimeout=400; Charset=iso_1;";
            sysConn.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["VMSybase"].ConnectionString; 
            if (sysConn.State != ConnectionState.Open)
                sysConn.Open();
            sybCommand = sysConn.CreateCommand();
        }
        public void closeConnection()
        {
            try
            {
                //if (sysConn.State == ConnectionState.Open)
                sysConn.Close();
                sysConn.Dispose();
            }
            catch (System.Exception ex)
            {
                ex.ToString();
            }
        }
        public AseParameter NewParm(string name, object value)
        {
            AseParameter sybParam = sybCommand.CreateParameter();
            sybParam.ParameterName = name;
            sybParam.Value = value;

            return sybParam;
        }
        public AseParameter NewParmOut(string name, DbType dbType)
        {
            AseParameter sybParam = sybCommand.CreateParameter();
            sybParam.Direction = ParameterDirection.Output;
            sybParam.ParameterName = name;
            sybParam.DbType = dbType;

            return sybParam;
        }

        public DataTable sybaseGetTable(string StoredProcedure, List<AseParameter> parametros)
        {
            DataTable retval = new DataTable();

            sybCommand.CommandType = CommandType.StoredProcedure;
            sybCommand.CommandText = StoredProcedure;

            foreach (AseParameter param in parametros)
                sybCommand.Parameters.Add(param);

            /*AseDataReader reader = sybCommand.ExecuteReader();
            while (reader.Read())
            {
                mNombre = reader.GetString(0);
                //string id = reader.GetString(1);
                //decimal price = reader.GetDecimal(2);
            }
            reader.Close();*/

            AseDataAdapter da = new AseDataAdapter(sybCommand);
            da.Fill(retval);

            return retval;
        }

        public void sybaseExecuteQuery(string StoredProcedure, List<AseParameter> parametros)
        {
            sybCommand.CommandType = CommandType.StoredProcedure;
            sybCommand.CommandText = StoredProcedure;

            foreach (AseParameter param in parametros)
                sybCommand.Parameters.Add(param);

            sybCommand.ExecuteNonQuery();
        }
    }
}