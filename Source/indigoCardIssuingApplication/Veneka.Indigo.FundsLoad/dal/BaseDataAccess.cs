using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.Common.Database;

namespace Veneka.Indigo.FundsLoad.dal
{
    public class BaseDataAccess
    {
        protected readonly DatabaseConnectionObject dataBaseConnection = DatabaseConnectionObject.Instance;

        protected SqlParameter CreateSqlParameter(string parameterName, SqlDbType dataType, int size, object value, ParameterDirection direction = ParameterDirection.Input, byte precision = 0, byte scale = 0)
        {
            SqlParameter paramCode = new SqlParameter(string.Format("@{0}", parameterName), dataType);
            if (size != 0)
            {
                paramCode = new SqlParameter(string.Format("@{0}", parameterName), dataType, size);
            }
            if (precision != 0)
            {
                paramCode = new SqlParameter(string.Format("@{0}", parameterName), dataType, size, ParameterDirection.Input, false, precision, scale, string.Empty, DataRowVersion.Default, value);
            }

            if (direction != ParameterDirection.Input)
            {
                paramCode.Direction = direction;
            }
            paramCode.Value = (value != null ? value : DBNull.Value);
            return paramCode;
        }

        protected int ExecuteNonQuery(SqlCommand sqlCommand)
        {
            sqlCommand.Connection = dataBaseConnection.SQLConnection;
            int result = sqlCommand.ExecuteNonQuery();
            return result;
        }

        protected DataTable ExecuteQuery(SqlCommand sqlCommand)
        {
            DataTable result = new DataTable();
            sqlCommand.Connection = dataBaseConnection.SQLConnection;
            using (SqlDataReader reader = sqlCommand.ExecuteReader())
            {
                result.Load(reader);
            }
            return result;
        }

        protected long UnpackLong(DataRow row, string columnName)
        {
            return (row[columnName] != DBNull.Value ? Convert.ToInt64(row[columnName]) : 0);
        }

        protected int UnpackInt(DataRow row, string columnName)
        {
            return (row[columnName] != DBNull.Value ? Convert.ToInt32(row[columnName]) : 0);
        }

        protected decimal UnpackDecimal(DataRow row, string columnName)
        {
            return (row[columnName] != DBNull.Value ? Convert.ToDecimal(row[columnName]) : 0.00M);
        }

        protected string UnpackString(DataRow row, string columnName)
        {
            if (row[columnName] == DBNull.Value)
            {
                return null;
            }
            return (row[columnName] != DBNull.Value ? Convert.ToString(row[columnName]) : string.Empty);
        }

        protected byte[] UnpackBinary(DataRow row, string columnName)
        {
            if (row[columnName] == DBNull.Value)
            {
                return null;
            }
            return (row[columnName] != DBNull.Value ? (byte[])row[columnName] : null);
        }

        protected bool UnpackBoolean(DataRow row, string columnName, bool defaultValue = false)
        {
            return (row[columnName] != DBNull.Value ? Convert.ToBoolean(row[columnName]) : defaultValue);
        }

        protected DateTime UnpackDateTime(DataRow row, string columnName)
        {
            return (row[columnName] != DBNull.Value ? Convert.ToDateTime(row[columnName]) : new DateTime(1900, 1, 1));
        }

        protected Guid UnpackGuid(DataRow row, string columnName)
        {
            return (row[columnName] != DBNull.Value ? new Guid(Convert.ToString(row[columnName])) : new Guid());
        }
    }
}
