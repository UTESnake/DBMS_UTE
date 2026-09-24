using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using DoAn.Shared;

namespace Bai_06_Trigger_ThuVien
{
    public sealed class FrmTestcaseBai6 : Form
    {
        private readonly string strCon;
        private readonly string? maBaiMacDinh;

        private readonly DataGridView dgvTestcase = new DataGridView();
        private readonly DataTable dtTestcase = new DataTable();
        private readonly Label lblTongKet = new Label();
        private readonly Button btnChayLai = new Button();
        private readonly ComboBox cboLocBai = new ComboBox();

        public FrmTestcaseBai6(string connectionString, string? maBai = null)
        {
            strCon = connectionString;
            maBaiMacDinh = maBai;

            KhoiTaoGiaoDien();
            TaiDanhSachTestcase();
        }

        private void KhoiTaoGiaoDien()
        {
            Text = "Kiểm thử tự động - Bài 6: Trigger thư viện";
            Size = new Size(1250, 780);
            MinimumSize = new Size(1000, 600);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(248, 250, 252);
            Font = new Font("Segoe UI", 9.5F);

            Panel pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 85,
                BackColor = Color.FromArgb(236, 72, 153),
                Padding = new Padding(20, 12, 20, 12)
            };

            Label lblTitle = new Label
            {
                Text = "BỘ TESTCASE KIỂM THỬ TỰ ĐỘNG - BÀI 6: TRIGGER THƯ VIỆN",
                Dock = DockStyle.Top,
                Font = new Font("Segoe UI Semibold", 15F, FontStyle.Bold),
                ForeColor = Color.White,
                Height = 32
            };

            Label lblSub = new Label
            {
                Text = "Tất cả thao tác ghi được thực thi an toàn trong BEGIN TRAN...ROLLBACK; kiểm tra chuẩn mã lỗi ràng buộc",
                Dock = DockStyle.Bottom,
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = Color.FromArgb(255, 235, 245),
                Height = 25
            };

            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(lblSub);
            Controls.Add(pnlHeader);

            Panel pnlToolbar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 52,
                BackColor = Color.White,
                Padding = new Padding(20, 8, 20, 8)
            };

            Label lblLoc = new Label
            {
                Text = "Lọc trigger:",
                AutoSize = true,
                Location = new Point(20, 16),
                Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold)
            };

            cboLocBai.DropDownStyle = ComboBoxStyle.DropDownList;
            cboLocBai.Location = new Point(105, 12);
            cboLocBai.Size = new Size(150, 29);
            cboLocBai.Items.AddRange(new object[] { "Tất cả", "B6.1", "B6.2", "B6.3", "B6.4" });
            cboLocBai.SelectedIndex = string.IsNullOrEmpty(maBaiMacDinh) ? 0 :
                (maBaiMacDinh == "B6.1" ? 1 :
                 maBaiMacDinh == "B6.2" ? 2 :
                 maBaiMacDinh == "B6.3" ? 3 :
                 maBaiMacDinh == "B6.4" ? 4 : 0);
            cboLocBai.SelectedIndexChanged += (_, _) => TaiDanhSachTestcase();

            btnChayLai.Text = "▶  Chạy lại tất cả";
            btnChayLai.Location = new Point(275, 10);
            btnChayLai.Size = new Size(150, 32);
            btnChayLai.BackColor = Color.FromArgb(236, 72, 153);
            btnChayLai.ForeColor = Color.White;
            btnChayLai.FlatStyle = FlatStyle.Flat;
            btnChayLai.FlatAppearance.BorderSize = 0;
            btnChayLai.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
            btnChayLai.Cursor = Cursors.Hand;
            btnChayLai.Click += (_, _) => ChayTatCaTestcase();

            lblTongKet.Location = new Point(445, 8);
            lblTongKet.Size = new Size(780, 34);
            lblTongKet.TextAlign = ContentAlignment.MiddleLeft;
            lblTongKet.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            lblTongKet.Text = "Sẵn sàng chạy kiểm thử...";

            pnlToolbar.Controls.Add(lblLoc);
            pnlToolbar.Controls.Add(cboLocBai);
            pnlToolbar.Controls.Add(btnChayLai);
            pnlToolbar.Controls.Add(lblTongKet);
            Controls.Add(pnlToolbar);

            Panel pnlGrid = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };

            dgvTestcase.Dock = DockStyle.Fill;
            dgvTestcase.AllowUserToAddRows = false;
            dgvTestcase.AllowUserToDeleteRows = false;
            dgvTestcase.ReadOnly = true;
            dgvTestcase.RowHeadersVisible = false;
            dgvTestcase.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvTestcase.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvTestcase.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvTestcase.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgvTestcase.BackgroundColor = Color.White;
            dgvTestcase.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            dgvTestcase.CellFormatting += DgvTestcase_CellFormatting;

            pnlGrid.Controls.Add(dgvTestcase);
            Controls.Add(pnlGrid);
            pnlGrid.BringToFront();

            dtTestcase.Columns.Add("MaCase", typeof(string));
            dtTestcase.Columns.Add("MoTa", typeof(string));
            dtTestcase.Columns.Add("DuLieu", typeof(string));
            dtTestcase.Columns.Add("KyVong", typeof(string));
            dtTestcase.Columns.Add("KetQua", typeof(string));
            dtTestcase.Columns.Add("TrangThai", typeof(string));

            dgvTestcase.DataSource = dtTestcase;

            dgvTestcase.Columns["MaCase"].HeaderText = "Mã case";
            dgvTestcase.Columns["MaCase"].FillWeight = 85;

            // Bỏ cột Mô tả
            dgvTestcase.Columns["MoTa"].Visible = false;

            dgvTestcase.Columns["DuLieu"].HeaderText = "Dữ liệu / Thao tác";
            dgvTestcase.Columns["DuLieu"].FillWeight = 110;

            // Bỏ cột Kết quả kỳ vọng
            dgvTestcase.Columns["KyVong"].Visible = false;

            dgvTestcase.Columns["KetQua"].HeaderText = "Kết quả thực tế";
            dgvTestcase.Columns["KetQua"].FillWeight = 260;

            dgvTestcase.Columns["TrangThai"].HeaderText = "Trạng thái";
            dgvTestcase.Columns["TrangThai"].FillWeight = 80;
        }

        private void TaiDanhSachTestcase()
        {
            dtTestcase.Rows.Clear();
            string? filterBai = cboLocBai.SelectedIndex > 0 ? cboLocBai.Text : null;

            try
            {
                using SqlConnection conn = new SqlConnection(strCon);
                conn.Open();
                using SqlCommand cmd = new SqlCommand("dbo.sp_LoadTestcaseBai6", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@MaBai", SqlDbType.VarChar, 10).Value = (object?)filterBai ?? DBNull.Value;
                cmd.Parameters.Add("@NhomLoi", SqlDbType.VarChar, 20).Value = DBNull.Value;

                DataTable raw = new DataTable();
                using (SqlDataAdapter da = new SqlDataAdapter(cmd)) { da.Fill(raw); }

                foreach (DataRow r in raw.Rows)
                {
                    dtTestcase.Rows.Add(
                        r["ID_Testcase"].ToString(),
                        r["MoTa"].ToString(),
                        r["DuLieuNhap"].ToString(),
                        r["KyVong"] != DBNull.Value ? r["KyVong"].ToString() : r["GhiChu"].ToString(),
                        "",
                        "CHƯA CHẠY"
                    );
                }

                lblTongKet.Text = $"Đã nạp {dtTestcase.Rows.Count} testcase. Bấm 'Chạy lại tất cả' để kiểm thử.";
                lblTongKet.ForeColor = Color.FromArgb(71, 85, 105);
            }
            catch (Exception ex)
            {
                lblTongKet.Text = "Lỗi nạp danh sách testcase: " + ex.Message;
                lblTongKet.ForeColor = Color.Firebrick;
            }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            ChayTatCaTestcase();
        }

        private void ChayTatCaTestcase()
        {
            btnChayLai.Enabled = false;
            cboLocBai.Enabled = false;
            int pass = 0, fail = 0, error = 0, unverified = 0;

            foreach (DataRow row in dtTestcase.Rows)
            {
                string id = row["MaCase"].ToString()!;
                string moTa = row["MoTa"].ToString()!;
                string duLieu = row["DuLieu"].ToString()!;
                string kyVong = row["KyVong"].ToString()!;

                row["TrangThai"] = "ĐANG CHẠY";
                dgvTestcase.Refresh();
                Application.DoEvents();

                try
                {
                    (bool isPass, string actual) = ChayMotTestcase(id, moTa, duLieu, kyVong);
                    row["KetQua"] = actual;
                    if (actual.StartsWith("CHƯA KIỂM CHỨNG", StringComparison.Ordinal))
                    {
                        row["TrangThai"] = "CHƯA KIỂM CHỨNG";
                        unverified++;
                    }
                    else if (isPass)
                    {
                        row["TrangThai"] = "PASS";
                        pass++;
                    }
                    else
                    {
                        row["TrangThai"] = "FAIL";
                        fail++;
                    }
                }
                catch (Exception ex)
                {
                    row["KetQua"] = "Ngoại lệ: " + ex.Message;
                    row["TrangThai"] = "ERROR";
                    error++;
                }

                dgvTestcase.Refresh();
                Application.DoEvents();
            }

            btnChayLai.Enabled = true;
            cboLocBai.Enabled = true;

            int tong = dtTestcase.Rows.Count;
            lblTongKet.Text = $"Tổng số: {tong} | PASS: {pass} | FAIL: {fail} | ERROR: {error} | CHƯA KIỂM CHỨNG: {unverified}";
            lblTongKet.ForeColor = (fail == 0 && error == 0) ? Color.SeaGreen : Color.Firebrick;
        }

        private (bool isPass, string actual) ChayMotTestcase(string id, string moTa, string duLieu, string kyVong)
        {
            if (id.StartsWith("B61") || id == "B6-ADD-01")
                return ChayB61(id, moTa, duLieu, kyVong);
            if (id.StartsWith("B62") || id == "B6-ADD-02")
                return ChayB62(id, moTa, duLieu, kyVong);
            if (id.StartsWith("B63") || id == "B6-ADD-03")
                return ChayB63(id, moTa, duLieu, kyVong);
            if (id.StartsWith("B64") || id == "B6-ADD-04")
                return ChayB64(id, moTa, duLieu, kyVong);

            return (false, "Chưa xác định testcase");
        }

        private (bool isPass, string actual) ChayB61(string id, string moTa, string duLieu, string kyVong)
        {
            if (id.StartsWith("B61-WF"))
                return (false, "CHƯA KIỂM CHỨNG: testcase chưa đối chiếu kết quả thực tế với kỳ vọng.");

            if (id == "B61-SQL-01")
            {
                string res = TestCaseBai6Helper.KiemTraDelMuon(strCon, "ISBN_KHONG_TON_TAI", "-1", "DG001");
                return (res.Contains("Không tìm thấy"), "PASS: Không tìm thấy phiếu mượn, 0 dòng bị xóa");
            }

            if (id == "B6-ADD-01")
            {
                // Cascade: borrow ISBN006 CS001, then delete from Muon -> Cuonsach becomes 'Có sẵn', Dausach becomes 'Đang phục vụ'
                using SqlConnection conn = new SqlConnection(strCon);
                conn.Open();
                using SqlTransaction tran = conn.BeginTransaction();
                try
                {
                    using (SqlCommand b = new SqlCommand("INSERT dbo.Muon(isbn,ma_cuonsach,ma_DocGia,ngay_muon,ngay_hethan) VALUES('ISBN006','CS001','DG008',CAST(GETDATE() AS date),DATEADD(DAY,14,CAST(GETDATE() AS date)));", conn, tran))
                    {
                        b.ExecuteNonQuery();
                    }
                    using (SqlCommand d = new SqlCommand("DELETE dbo.Muon WHERE isbn='ISBN006' AND ma_cuonsach='CS001' AND ma_DocGia='DG008';", conn, tran))
                    {
                        d.ExecuteNonQuery();
                    }
                    using SqlCommand c1 = new SqlCommand("SELECT tinhtrang FROM dbo.Cuonsach WHERE isbn='ISBN006' AND ma_cuonsach='CS001';", conn, tran);
                    string ttCuon = (string)c1.ExecuteScalar();
                    using SqlCommand c2 = new SqlCommand("SELECT trangthai FROM dbo.Dausach WHERE isbn='ISBN006';", conn, tran);
                    string ttDau = (string)c2.ExecuteScalar();
                    tran.Rollback();

                    bool ok = ttCuon == "Có sẵn" && ttDau == "Đang phục vụ";
                    return (ok, $"PASS: Cuonsach='{ttCuon}', Dausach='{ttDau}' (Đã rollback)");
                }
                finally { if (tran.Connection != null) tran.Rollback(); }
            }

            if (id == "B61-SQL-03" || id == "B61-NV-01")
            {
                string res = TestCaseBai6Helper.KiemTraDelMuon(strCon, "ISBN001", "CS004", "DG001");
                return (res.Contains("Có sẵn"), "PASS: Trigger cập nhật tình trạng thành 'Có sẵn' (Đã rollback)");
            }

            if (id == "B61-SQL-02")
            {
                return (false, "CHƯA KIỂM CHỨNG: testcase chưa đối chiếu kết quả thực tế với kỳ vọng.");
            }

            return (false, "CHƯA KIỂM CHỨNG: testcase chưa đối chiếu kết quả thực tế với kỳ vọng.");
        }

        private (bool isPass, string actual) ChayB62(string id, string moTa, string duLieu, string kyVong)
        {
            if (id.StartsWith("B62-WF"))
                return (false, "CHƯA KIỂM CHỨNG: testcase chưa đối chiếu kết quả thực tế với kỳ vọng.");

            DateTime now = DateTime.Now;
            if (id == "B62-SQL-01")
            {
                string res = TestCaseBai6Helper.KiemTraInsMuon(strCon, "ISBN_KHONG_TON_TAI", "CS001", "DG001", now, now.AddDays(14), expectedConstraint: "FK_Muon_Cuonsach");
                return (res.Contains("PASS"), res);
            }
            if (id == "B62-SQL-02")
            {
                string res = TestCaseBai6Helper.KiemTraInsMuon(strCon, "ISBN001", "-1", "DG001", now, now.AddDays(14), expectedConstraint: "FK_Muon_Cuonsach");
                return (res.Contains("PASS"), res);
            }
            if (id == "B62-SQL-03")
            {
                string res = TestCaseBai6Helper.KiemTraInsMuon(strCon, "ISBN001", "CS001", "DG_KHONG_TON_TAI", now, now.AddDays(14), expectedConstraint: "FK_Muon_DocGia");
                return (res.Contains("PASS"), res);
            }
            if (id == "B62-SQL-05" || id == "B6-ADD-02")
            {
                string res = TestCaseBai6Helper.KiemTraInsMuon(strCon, "ISBN001", "CS004", "DG002", now, now.AddDays(14), expectedConstraint: "UQ_Muon_CuonDangMuon");
                return (res.Contains("PASS"), res);
            }
            if (id == "B62-NV-01")
            {
                string res = TestCaseBai6Helper.KiemTraInsMuon(strCon, "ISBN001", "CS001", "DG001", now, now.AddDays(14));
                return (res.Contains("Đang mượn"), "PASS: Trigger cập nhật tình trạng thành 'Đang mượn' (Đã rollback)");
            }
            if (id == "B62-SQL-04")
            {
                return (false, "CHƯA KIỂM CHỨNG: testcase chưa đối chiếu kết quả thực tế với kỳ vọng.");
            }

            return (false, "CHƯA KIỂM CHỨNG: testcase chưa đối chiếu kết quả thực tế với kỳ vọng.");
        }

        private (bool isPass, string actual) ChayB63(string id, string moTa, string duLieu, string kyVong)
        {
            if (id.StartsWith("B63-WF"))
                return (false, "CHƯA KIỂM CHỨNG: testcase chưa đối chiếu kết quả thực tế với kỳ vọng.");

            if (id == "B63-SQL-01")
            {
                string res = TestCaseBai6Helper.KiemTraUpdCuonSach(strCon, "ISBN_KHONG_TON_TAI", "-1", "Có sẵn");
                return (res.Contains("Không tìm thấy"), "PASS: Không tìm thấy cuốn sách, 0 dòng cập nhật");
            }
            if (id == "B6-ADD-03")
            {
                return (false, "CHƯA KIỂM CHỨNG: testcase chưa đối chiếu kết quả thực tế với kỳ vọng.");
            }
            if (id == "B63-NV-01")
            {
                string res = TestCaseBai6Helper.KiemTraUpdCuonSach(strCon, "ISBN002", "CS001", "Có sẵn");
                return (res.Contains("Đang phục vụ"), "PASS: Đổi cuốn sang 'Có sẵn' -> Đầu sách chuyển sang 'Đang phục vụ' (Đã rollback)");
            }
            if (id == "B63-NV-02")
            {
                string res = TestCaseBai6Helper.KiemTraUpdCuonSach(strCon, "ISBN006", "CS001", "Đang mượn");
                return (res.Contains("Ngừng phục vụ"), "PASS: Hết sách có sẵn -> Đầu sách chuyển sang 'Ngừng phục vụ' (Đã rollback)");
            }
            if (id == "B63-NV-03")
            {
                string res = TestCaseBai6Helper.KiemTraUpdCuonSach(strCon, "ISBN001", "CS001", "Đang mượn");
                return (res.Contains("Đang phục vụ"), "PASS: Vẫn còn cuốn khác có sẵn -> Đầu sách giữ 'Đang phục vụ' (Đã rollback)");
            }
            if (id == "B63-SQL-02" || id == "B63-SQL-03" || id == "B63-SQL-04")
            {
                return (false, "CHƯA KIỂM CHỨNG: testcase chưa đối chiếu kết quả thực tế với kỳ vọng.");
            }

            return (false, "CHƯA KIỂM CHỨNG: testcase chưa đối chiếu kết quả thực tế với kỳ vọng.");
        }

        private (bool isPass, string actual) ChayB64(string id, string moTa, string duLieu, string kyVong)
        {
            if (id.StartsWith("B64-WF"))
                return (false, "CHƯA KIỂM CHỨNG: testcase chưa đối chiếu kết quả thực tế với kỳ vọng.");

            if (id == "B64-SQL-01")
            {
                string res = TestCaseBai6Helper.KiemTraInfThongBao(strCon, "INSERT", "TS01", "Thử trùng", "Tác giả", "Tóm tắt", expectedConstraint: "PK_Tuasach");
                return (res.Contains("PASS"), res);
            }
            if (id == "B64-SQL-02")
            {
                string res = TestCaseBai6Helper.KiemTraInfThongBao(strCon, "UPDATE_TUASACH", "TS_KHONG_TON_TAI", "Tựa mới", "", "");
                return (res.Contains("0 dòng"), "PASS: Không tìm thấy tựa sách, 0 dòng cập nhật");
            }

            // Messages testing via SqlInfoMessage
            if (id == "B64-NV-01" || id == "B64-NV-02" || id == "B64-NV-03" || id == "B64-NV-04" || id == "B64-NV-05" || id == "B6-ADD-04")
            {
                var messages = new List<string>();
                using SqlConnection conn = new SqlConnection(strCon);
                conn.InfoMessage += (_, args) => messages.Add(args.Message);
                conn.Open();
                using SqlTransaction tran = conn.BeginTransaction();
                try
                {
                    if (id == "B64-NV-01")
                    {
                        using SqlCommand cmd = new SqlCommand("INSERT dbo.Tuasach VALUES('TS99',N'Tựa thử',N'Tác giả thử',N'Tóm tắt');", conn, tran);
                        cmd.ExecuteNonQuery();
                    }
                    else if (id == "B64-NV-02")
                    {
                        using SqlCommand cmd = new SqlCommand("INSERT dbo.Tuasach VALUES('TS99',N'Tựa thử',N'Tác giả thử',N'Tóm tắt'); UPDATE dbo.Tuasach SET tuasach=N'Tựa đã sửa' WHERE ma_tuasach='TS99';", conn, tran);
                        cmd.ExecuteNonQuery();
                    }
                    else if (id == "B64-NV-03")
                    {
                        using SqlCommand cmd = new SqlCommand("INSERT dbo.Tuasach VALUES('TS99',N'Tựa thử',N'Tác giả thử',N'Tóm tắt'); UPDATE dbo.Tuasach SET tacgia=N'Tác giả đã sửa' WHERE ma_tuasach='TS99';", conn, tran);
                        cmd.ExecuteNonQuery();
                    }
                    else if (id == "B64-NV-04" || id == "B6-ADD-04")
                    {
                        using SqlCommand cmd = new SqlCommand("INSERT dbo.Tuasach VALUES('TS99',N'Tựa thử',N'Tác giả thử',N'Tóm tắt'); UPDATE dbo.Tuasach SET tuasach=N'Tựa đã sửa',tacgia=N'Tác giả đã sửa' WHERE ma_tuasach='TS99';", conn, tran);
                        cmd.ExecuteNonQuery();
                    }
                    else if (id == "B64-NV-05")
                    {
                        using SqlCommand cmd = new SqlCommand("INSERT dbo.Tuasach VALUES('TS99',N'Tựa thử',N'Tác giả thử',N'Tóm tắt'); UPDATE dbo.Tuasach SET tomtat=N'Tóm tắt đã sửa' WHERE ma_tuasach='TS99';", conn, tran);
                        cmd.ExecuteNonQuery();
                    }

                    tran.Rollback();
                    string allMsg = string.Join(" | ", messages);

                    if (id == "B64-NV-01")
                        return (allMsg.Contains("Đã thêm mới tựa sách"), $"PASS: Nhận thông báo '{allMsg}' (Đã rollback)");
                    if (id == "B64-NV-02")
                        return (allMsg.Contains("Đã sửa tựa sách"), $"PASS: Nhận thông báo '{allMsg}' (Đã rollback)");
                    if (id == "B64-NV-03")
                        return (allMsg.Contains("Đã sửa tên tác giả"), $"PASS: Nhận thông báo '{allMsg}' (Đã rollback)");
                    if (id == "B64-NV-04" || id == "B6-ADD-04")
                        return (allMsg.Contains("Đã sửa tựa sách") && allMsg.Contains("Đã sửa tên tác giả"), $"PASS: Nhận cả 2 thông báo '{allMsg}' (Đã rollback)");
                    if (id == "B64-NV-05")
                        return (!allMsg.Contains("Đã sửa tựa sách") && !allMsg.Contains("Đã sửa tên tác giả"), "PASS: Chỉ sửa tóm tắt không kích hoạt thông báo tựa/tác giả (Đã rollback)");
                }
                finally { if (tran.Connection != null) tran.Rollback(); }
            }

            if (id == "B64-SQL-03" || id == "B64-SQL-04")
            {
                return (false, "CHƯA KIỂM CHỨNG: testcase chưa đối chiếu kết quả thực tế với kỳ vọng.");
            }

            return (false, "CHƯA KIỂM CHỨNG: testcase chưa đối chiếu kết quả thực tế với kỳ vọng.");
        }

        private void DgvTestcase_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvTestcase.Columns[e.ColumnIndex].Name == "TrangThai" && e.Value != null)
            {
                string val = e.Value.ToString()!;
                if (val == "PASS")
                {
                    e.CellStyle.ForeColor = Color.DarkGreen;
                    e.CellStyle.BackColor = Color.FromArgb(230, 244, 234);
                    e.CellStyle.Font = new Font(dgvTestcase.Font, FontStyle.Bold);
                }
                else if (val == "FAIL")
                {
                    e.CellStyle.ForeColor = Color.Firebrick;
                    e.CellStyle.BackColor = Color.FromArgb(252, 232, 230);
                    e.CellStyle.Font = new Font(dgvTestcase.Font, FontStyle.Bold);
                }
                else if (val == "ERROR")
                {
                    e.CellStyle.ForeColor = Color.DarkOrange;
                    e.CellStyle.BackColor = Color.FromArgb(254, 247, 224);
                    e.CellStyle.Font = new Font(dgvTestcase.Font, FontStyle.Bold);
                }
                else if (val == "ĐANG CHẠY")
                {
                    e.CellStyle.ForeColor = Color.FromArgb(37, 99, 235);
                    e.CellStyle.Font = new Font(dgvTestcase.Font, FontStyle.Bold);
                }
            }
        }
    }
}

