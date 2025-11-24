using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Configuration;

namespace Do_An.DAL
{
    public class Database
    {
        // Chuỗi kết nối (có thể chuyển sang app.config sau)
        private readonly string strCnn = ConfigurationManager.ConnectionStrings["QL_TrungTamNgoaiNgu"].ConnectionString;

        // Kiểm tra kết nối
        public bool KiemTraKetNoi()
        {
            using (SqlConnection sqlConn = new SqlConnection(strCnn))
            {
                try
                {
                    sqlConn.Open();
                    sqlConn.Close();
                    return true;
                }
                catch (Exception ex)
                {
                    throw new Exception("Không thể kết nối đến CSDL: " + ex.Message, ex);
                }
            }
        }

        // Thực hiện SELECT và trả về DataTable
        public DataTable Execute(string sqlStr, Dictionary<string, object> parameters)
        {
            DataTable dt = new DataTable();
            using (SqlConnection sqlConn = new SqlConnection(strCnn))
            {
                using (SqlCommand cmd = new SqlCommand(sqlStr, sqlConn))
                {
                    try
                    {
                        // Thêm tham số chuẩn, tránh AddWithValue
                        foreach (var param in parameters)
                        {
                            cmd.Parameters.Add(new SqlParameter(param.Key, param.Value ?? DBNull.Value));
                        }

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                    catch (SqlException sqlEx)
                    {
                        throw new Exception("Lỗi khi thực hiện truy vấn SQL: " + sqlEx.Message, sqlEx);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Lỗi không xác định khi truy vấn dữ liệu: " + ex.Message, ex);
                    }
                }
            }

            return dt;
        }
        public DataTable Execute(string sqlStr)
        {
            return Execute(sqlStr, new Dictionary<string, object>());
        }
        public T ExecuteScalar<T>(string sqlStr, Dictionary<string, object> parameters = null)
        {
            object result = ExecuteScalar(sqlStr, parameters);
            if (result == null || result == DBNull.Value)
                return default(T);
            return (T)Convert.ChangeType(result, typeof(T));
        }


        // Thực hiện INSERT/UPDATE/DELETE
        public int ExecuteNonQuery(string sqlStr, Dictionary<string, object> parameters)
        {
            int rowsAffected = 0;
            using (SqlConnection sqlConn = new SqlConnection(strCnn))
            {
                using (SqlCommand cmd = new SqlCommand(sqlStr, sqlConn))
                {
                    try
                    {
                        foreach (var param in parameters)
                        {
                            cmd.Parameters.Add(new SqlParameter(param.Key, param.Value ?? DBNull.Value));
                        }

                        sqlConn.Open();
                        rowsAffected = cmd.ExecuteNonQuery();
                    }
                    catch (SqlException sqlEx)
                    {
                        throw new Exception("Lỗi khi thực hiện truy vấn SQL: " + sqlEx.Message, sqlEx);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Lỗi không xác định khi thao tác dữ liệu: " + ex.Message, ex);
                    }
                    finally
                    {
                        if (sqlConn.State == ConnectionState.Open)
                            sqlConn.Close();
                    }
                }
            }
            return rowsAffected;
        }
        // --------------------- XỬ LÝ HỌC PHÍ  ---------------------
        public object ExecuteScalar(string sqlStr, Dictionary<string, object> parameters = null)
        {
            using (SqlConnection sqlConn = new SqlConnection(strCnn))
            {
                using (SqlCommand cmd = new SqlCommand(sqlStr, sqlConn))
                {
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            cmd.Parameters.Add(new SqlParameter(param.Key, param.Value ?? DBNull.Value));
                        }
                    }
                    sqlConn.Open();
                    return cmd.ExecuteScalar();
                }
            }
        }
        // ==========================
        // HỖ TRỢ GỌI NGẮN CHO BLL
        // ==========================
        public DataTable ExecuteQuery(string sqlStr)
        {
            return Execute(sqlStr);
        }
    }
}
