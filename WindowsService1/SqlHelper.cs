using System;
using System.Data.SqlClient;
using System.Data;
using WindowsService1;
using System.Configuration;

public class SqlHelper
{
    static string conStr = ConfigurationManager.ConnectionStrings["conStr"].ToString();
     
    #region 执行查询语句，返回第一行第一列的值
    /// <summary>
    /// 执行查询语句，返回第一行第一列的值
    /// </summary>
    /// <param name="sql">要执行的sql语句</param>
    /// <param name="type">要执行的sql语句类型</param>
    /// <param name="sps">sql语句中的参数</param>
    /// <returns></returns>
    public static object GetOneDate(string sql, CommandType type, params SqlParameter[] sps)
    {
        SqlConnection con = new SqlConnection(conStr);
        con.Open();
        try
        {
            SqlCommand comm = new SqlCommand(sql, con);
            comm.CommandType = type;  //指定要执行的sql语句类型
            if (sps != null)
            {
                comm.Parameters.AddRange(sps);
            }
            object ob = comm.ExecuteScalar();
            return ob;
        }
        catch (Exception ex)
        {
            WriteLog.write("数据库操作失败，错误信息为" + ex.Message);
            return null;
        }
        finally
        {
            con.Close();
        }
    }
    #endregion

    #region 执行查询语句返回SqlDateReader的结果集
    /// <summary>
    /// 执行查询语句返回SqlDateReader的结果集
    /// </summary>
    /// <param name="sql">要执行的sql语句</param>
    /// <param name="type">要执行的sql语句类型</param>
    /// <param name="sps">sql语句中的参数</param>
    /// <returns></returns>
    public static SqlDataReader GetReader(string sql, CommandType type, params SqlParameter[] sps)
    {
        SqlConnection conn = new SqlConnection(conStr);
        try
        {
            SqlCommand comm = new SqlCommand(sql, conn);
            conn.Open();
            comm.CommandType = type;
            if (sps != null)
            {
                comm.Parameters.AddRange(sps);
            }
            SqlDataReader sdr = comm.ExecuteReader(CommandBehavior.CloseConnection);
            return sdr;
        }
        catch (Exception ex)
        {
            WriteLog.write("数据库操作失败，错误信息为" + ex.Message);
            return null;
        }
    }
    #endregion

    #region 执行增加删除修改或DDl语句  返回受影响的行数
    /// <summary>
    /// 执行增加删除修改或DDl语句  返回受影响的行数
    /// </summary>
    /// <param name="sql">要执行的sql语句</param>
    /// <param name="type">sql语句的类型</param>
    /// <param name="sps">sql语句的参数</param>
    /// <returns></returns>
    public static int? ExecuteSql(string sql, CommandType type, params SqlParameter[] sps)
    {
        SqlConnection conn = new SqlConnection(conStr);
        SqlCommand comm = new SqlCommand(sql, conn);
        try
        {
            conn.Open();
            comm.CommandType = type;
            if (sps != null)
            {
                comm.Parameters.AddRange(sps);
            }
            int? ob = comm.ExecuteNonQuery();
            return ob;
        }
        catch (Exception ex)
        {
            WriteLog.write("数据库操作失败，错误信息为" + ex.Message);
            return null;
        }
        finally
        {
            conn.Close();
        }
    }
    #endregion

    #region 使用dateset的方法
    /// <summary>
    /// 使用dateset的方法,返回dataset
    /// </summary>
    /// <param name="sql">要执行的sql语句</param>
    /// <param name="type">sql语句的类型</param>
    /// <param name="sps">sql语句的参数</param>
    /// <returns></returns>
    public static DataSet GetDS(string sql, CommandType type, params SqlParameter[] sps)
    {
        SqlConnection conn = new SqlConnection(conStr);
        try
        {
            SqlCommand comm = new SqlCommand(sql, conn);  //创建命令对象
            conn.Open();   //打开数据连接
            comm.CommandType = type;  //
            if (sps != null)
            {
                comm.Parameters.AddRange(sps);
            }
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            DataSet ds = new DataSet();
            sda.Fill(ds);    //将数据填充到DataSet中
            return ds;
        }
        catch (Exception ex)
        {
            WriteLog.write("数据库操作失败，错误信息为" + ex.Message);
            return null;
        }
        finally
        {
            conn.Close();
        }
    }
    #endregion

    private static SqlCommand BuildIntCommand(SqlConnection connection, string storedProcName, IDataParameter[] parameters)
    {
        SqlCommand command = BuildQueryCommand(connection, storedProcName, parameters);
        command.Parameters.Add(new SqlParameter("ReturnValue", SqlDbType.Int, 4, ParameterDirection.ReturnValue, false, 0, 0, string.Empty, DataRowVersion.Default, null));
        return command;
    }

    private static SqlCommand BuildQueryCommand(SqlConnection connection, string storedProcName, IDataParameter[] parameters)
    {
        SqlCommand command = new SqlCommand(storedProcName, connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        foreach (SqlParameter parameter in parameters)
        {
            if (parameter != null)
            {
                if (((parameter.Direction == ParameterDirection.InputOutput) || (parameter.Direction == ParameterDirection.Input)) && (parameter.Value == null))
                {
                    parameter.Value = DBNull.Value;
                }
                command.Parameters.Add(parameter);
            }
        }
        return command;
    }

    public static int RunProcedure(string storedProcName, IDataParameter[] parameters, out int rowsAffected)
    {
        using (SqlConnection connection = new SqlConnection(conStr))
        {
            connection.Open();
            SqlCommand command = BuildIntCommand(connection, storedProcName, parameters);
            rowsAffected = command.ExecuteNonQuery();
            return (int)command.Parameters["ReturnValue"].Value;
        }
    }


}
