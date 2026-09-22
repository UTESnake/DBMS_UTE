using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace Bai_06_Trigger_ThuVien
{
    public static class TestCaseBai6Helper
    {
        // =========================================================
        // LOAD TESTCASE
        // =========================================================
        public static DataTable LoadByBai(
            string strCon,
            string maBai)
        {
            DataTable dt =
                new DataTable();

            using SqlConnection conn =
                new SqlConnection(strCon);

            conn.Open();

            using SqlCommand cmd =
                new SqlCommand(
                    "dbo.sp_LoadTestcaseBai6",
                    conn
                );

            cmd.CommandType =
                CommandType.StoredProcedure;

            cmd.Parameters.Add(
                "@MaBai",
                SqlDbType.VarChar,
                10
            ).Value = maBai;

            using SqlDataAdapter da =
                new SqlDataAdapter(cmd);

            da.Fill(dt);

            return dt;
        }


        // =========================================================
        // 6.1 - tg_delMuon
        //
        // DELETE Muon
        // Trigger cập nhật Cuonsach.tinhtrang
        // ROLLBACK để không thay đổi dữ liệu thật
        // =========================================================
        public static string KiemTraDelMuon(
            string strCon,
            string isbn,
            string maCuon,
            string maDocGia,
            Action<SqlConnection, SqlTransaction> luuBanXemTruoc = null)
        {
            using SqlConnection conn =
                new SqlConnection(strCon);

            conn.Open();

            using SqlTransaction tran =
                conn.BeginTransaction();

            try
            {
                string sql = @"
                    DELETE FROM dbo.Muon
                    WHERE isbn = @isbn
                      AND ma_cuonsach = @maCuon
                      AND ma_DocGia = @maDocGia;
                ";

                using SqlCommand cmd =
                    new SqlCommand(
                        sql,
                        conn,
                        tran
                    );

                cmd.Parameters.AddWithValue(
                    "@isbn",
                    isbn
                );

                cmd.Parameters.AddWithValue(
                    "@maCuon",
                    maCuon
                );

                cmd.Parameters.AddWithValue(
                    "@maDocGia",
                    maDocGia
                );

                int soDong =
                    cmd.ExecuteNonQuery();


                string sqlCheck = @"
                    SELECT tinhtrang
                    FROM dbo.Cuonsach
                    WHERE isbn = @isbn
                      AND ma_cuonsach = @maCuon;
                ";

                using SqlCommand check =
                    new SqlCommand(
                        sqlCheck,
                        conn,
                        tran
                    );

                check.Parameters.AddWithValue(
                    "@isbn",
                    isbn
                );

                check.Parameters.AddWithValue(
                    "@maCuon",
                    maCuon
                );
                object value =
                    check.ExecuteScalar();

                if (soDong > 0)
                    luuBanXemTruoc?.Invoke(conn, tran);

                tran.Rollback();
                if (soDong == 0)
                {
                    return
                        "Không tìm thấy phiếu mượn phù hợp."
                        + Environment.NewLine
                        + "Không có dữ liệu bị thay đổi.";
                }
                return
                    "Trigger tg_delMuon đã được kích hoạt."
                    + Environment.NewLine
                    + "Số phiếu mượn đã xóa trong transaction: "
                    + soDong
                    + Environment.NewLine
                    + "Tình trạng cuốn sách sau Trigger: "
                    + Convert.ToString(value)
                    + Environment.NewLine
                    + Environment.NewLine
                    + "Đã ROLLBACK, dữ liệu thật không thay đổi.";
            }
            catch
            {
                try
                {
                    tran.Rollback();
                }
                catch
                {
                }

                throw;
            }
        }


        // =========================================================
        // 6.2 - tg_insMuon
        // =========================================================
        public static string KiemTraInsMuon(
            string strCon,
            string isbn,
            string maCuon,
            string maDocGia,
            DateTime ngayMuon,
            DateTime ngayHetHan,
            Action<SqlConnection, SqlTransaction> luuBanXemTruoc = null)
        {
            using SqlConnection conn =
                new SqlConnection(strCon);

            conn.Open();

            using SqlTransaction tran =
                conn.BeginTransaction();

            try
            {
                string sql = @"
                    INSERT INTO dbo.Muon
                    (
                        isbn,
                        ma_cuonsach,
                        ma_DocGia,
                        ngay_muon,
                        ngay_hethan
                    )
                    VALUES
                    (
                        @isbn,
                        @maCuon,
                        @maDocGia,
                        @ngayMuon,
                        @ngayHetHan
                    );
                ";

                using SqlCommand cmd =
                    new SqlCommand(
                        sql,
                        conn,
                        tran
                    );

                cmd.Parameters.AddWithValue(
                    "@isbn",
                    isbn
                );

                cmd.Parameters.AddWithValue(
                    "@maCuon",
                    maCuon
                );

                cmd.Parameters.AddWithValue(
                    "@maDocGia",
                    maDocGia
                );

                cmd.Parameters.AddWithValue(
                    "@ngayMuon",
                    ngayMuon
                );

                cmd.Parameters.AddWithValue(
                    "@ngayHetHan",
                    ngayHetHan
                );

                cmd.ExecuteNonQuery();


                string sqlCheck = @"
                    SELECT tinhtrang
                    FROM dbo.Cuonsach
                    WHERE isbn = @isbn
                      AND ma_cuonsach = @maCuon;
                ";

                using SqlCommand check =
                    new SqlCommand(
                        sqlCheck,
                        conn,
                        tran
                    );

                check.Parameters.AddWithValue(
                    "@isbn",
                    isbn
                );

                check.Parameters.AddWithValue(
                    "@maCuon",
                    maCuon
                );

                object value =
                    check.ExecuteScalar();

                luuBanXemTruoc?.Invoke(conn, tran);

                tran.Rollback();

                return
                    "Trigger tg_insMuon đã được kích hoạt."
                    + Environment.NewLine
                    + "Tình trạng cuốn sách sau Trigger: "
                    + Convert.ToString(value)
                    + Environment.NewLine
                    + Environment.NewLine
                    + "Đã ROLLBACK, dữ liệu thật không thay đổi.";
            }
            catch
            {
                try
                {
                    tran.Rollback();
                }
                catch
                {
                }

                throw;
            }
        }


        // =========================================================
        // 6.3 - tg_updCuonSach
        // =========================================================
        public static string KiemTraUpdCuonSach(
            string strCon,
            string isbn,
            string maCuon,
            string tinhTrangMoi,
            Action<SqlConnection, SqlTransaction> luuBanXemTruoc = null)
        {
            using SqlConnection conn =
                new SqlConnection(strCon);

            conn.Open();

            using SqlTransaction tran =
                conn.BeginTransaction();

            try
            {
                string sql = @"
                    UPDATE dbo.Cuonsach
                    SET tinhtrang = @tinhTrang
                    WHERE isbn = @isbn
                      AND ma_cuonsach = @maCuon;
                ";

                using SqlCommand cmd =
                    new SqlCommand(
                        sql,
                        conn,
                        tran
                    );

                cmd.Parameters.AddWithValue(
                    "@tinhTrang",
                    tinhTrangMoi
                );

                cmd.Parameters.AddWithValue(
                    "@isbn",
                    isbn
                );

                cmd.Parameters.AddWithValue(
                    "@maCuon",
                    maCuon
                );

                int soDong =
                    cmd.ExecuteNonQuery();


                string sqlCheck = @"
                    SELECT trangthai
                    FROM dbo.Dausach
                    WHERE isbn = @isbn;
                ";

                using SqlCommand check =
                    new SqlCommand(
                        sqlCheck,
                        conn,
                        tran
                    );

                check.Parameters.AddWithValue(
                    "@isbn",
                    isbn
                );

                object value =
                    check.ExecuteScalar();

                if (soDong > 0)
                    luuBanXemTruoc?.Invoke(conn, tran);

                tran.Rollback();


                if (soDong == 0)
                {
                    return
                        "Không tìm thấy cuốn sách cần cập nhật.";
                }


                return
                    "Trigger tg_updCuonSach đã được kích hoạt."
                    + Environment.NewLine
                    + "Trạng thái đầu sách sau Trigger: "
                    + Convert.ToString(value)
                    + Environment.NewLine
                    + Environment.NewLine
                    + "Đã ROLLBACK, dữ liệu thật không thay đổi.";
            }
            catch
            {
                try
                {
                    tran.Rollback();
                }
                catch
                {
                }

                throw;
            }
        }


        // =========================================================
        // 6.4 - tg_InfThongBao
        // =========================================================
        public static string KiemTraInfThongBao(
            string strCon,
            string thaoTac,
            string maTuaSach,
            string tuaSach,
            string tacGia,
            string tomTat,
            Action<SqlConnection, SqlTransaction> luuBanXemTruoc = null)
        {
            using SqlConnection conn =
                new SqlConnection(strCon);

            conn.Open();

            using SqlTransaction tran =
                conn.BeginTransaction();

            string thongBaoSQL =
                "";

            SqlInfoMessageEventHandler handler =
                (sender, e) =>
                {
                    if (!string.IsNullOrWhiteSpace(
                        e.Message))
                    {
                        thongBaoSQL +=
                            e.Message
                            + Environment.NewLine;
                    }
                };

            conn.InfoMessage += handler;

            try
            {
                string sql;


                switch (thaoTac)
                {
                    case "INSERT":
                        sql = @"
                            INSERT INTO dbo.Tuasach
                            (
                                ma_tuasach,
                                tuasach,
                                tacgia,
                                tomtat
                            )
                            VALUES
                            (
                                @maTuaSach,
                                @tuaSach,
                                @tacGia,
                                @tomTat
                            );
                        ";
                        break;


                    case "UPDATE_TUASACH":
                        sql = @"
                            UPDATE dbo.Tuasach
                            SET tuasach = @tuaSach
                            WHERE ma_tuasach = @maTuaSach;
                        ";
                        break;


                    case "UPDATE_TACGIA":
                        sql = @"
                            UPDATE dbo.Tuasach
                            SET tacgia = @tacGia
                            WHERE ma_tuasach = @maTuaSach;
                        ";
                        break;


                    case "UPDATE_BOTH":
                        sql = @"
                            UPDATE dbo.Tuasach
                            SET
                                tuasach = @tuaSach,
                                tacgia = @tacGia
                            WHERE ma_tuasach = @maTuaSach;
                        ";
                        break;


                    case "UPDATE_TOMTAT":
                        sql = @"
                            UPDATE dbo.Tuasach
                            SET tomtat = @tomTat
                            WHERE ma_tuasach = @maTuaSach;
                        ";
                        break;


                    default:
                        throw new Exception(
                            "Thao tác không hợp lệ."
                        );
                }


                using SqlCommand cmd =
                    new SqlCommand(
                        sql,
                        conn,
                        tran
                    );

                cmd.Parameters.AddWithValue(
                    "@maTuaSach",
                    maTuaSach
                );

                cmd.Parameters.AddWithValue(
                    "@tuaSach",
                    string.IsNullOrWhiteSpace(tuaSach)
                        ? DBNull.Value
                        : tuaSach
                );

                cmd.Parameters.AddWithValue(
                    "@tacGia",
                    string.IsNullOrWhiteSpace(tacGia)
                        ? DBNull.Value
                        : tacGia
                );

                cmd.Parameters.AddWithValue(
                    "@tomTat",
                    string.IsNullOrWhiteSpace(tomTat)
                        ? DBNull.Value
                        : tomTat
                );

                int soDong =
                    cmd.ExecuteNonQuery();

                luuBanXemTruoc?.Invoke(conn, tran);

                tran.Rollback();


                if (string.IsNullOrWhiteSpace(
                    thongBaoSQL))
                {
                    thongBaoSQL =
                        "Trigger không trả về thông báo PRINT.";
                }


                return
                    "Số dòng bị tác động: "
                    + soDong
                    + Environment.NewLine
                    + Environment.NewLine
                    + "Thông báo Trigger:"
                    + Environment.NewLine
                    + thongBaoSQL.Trim()
                    + Environment.NewLine
                    + Environment.NewLine
                    + "Đã ROLLBACK, dữ liệu thật không thay đổi.";
            }
            catch
            {
                try
                {
                    tran.Rollback();
                }
                catch
                {
                }

                throw;
            }
            finally
            {
                conn.InfoMessage -= handler;
            }
        }
    }
}
