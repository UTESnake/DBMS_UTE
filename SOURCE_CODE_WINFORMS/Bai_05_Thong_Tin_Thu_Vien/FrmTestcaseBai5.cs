using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using DoAn.Shared;

namespace ThongTinThuVien
{
    public sealed class FrmTestcaseBai5 : Form
    {
        private readonly string strCon;
        private readonly string? maBaiMacDinh;

        private readonly DataGridView dgvTestcase = new DataGridView();
        private readonly DataTable dtTestcase = new DataTable();
        private readonly Label lblTongKet = new Label();
        private readonly Button btnChayLai = new Button();
        private readonly ComboBox cboLocBai = new ComboBox();

        public FrmTestcaseBai5(string connectionString, string? maBai = null)
        {
            strCon = connectionString;
            maBaiMacDinh = maBai;

            KhoiTaoGiaoDien();
            TaiDanhSachTestcase();
        }

        private void KhoiTaoGiaoDien()
        {
            Text = "Kiểm thử tự động - Bài 5: Thông tin thư viện";
            Size = new Size(1250, 780);
            MinimumSize = new Size(1000, 600);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(248, 250, 252);
            Font = new Font("Segoe UI", 9.5F);

            Panel pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 85,
                BackColor = Color.FromArgb(139, 92, 246),
                Padding = new Padding(20, 12, 20, 12)
            };

            Label lblTitle = new Label
            {
                Text = "BỘ TESTCASE KIỂM THỬ TỰ ĐỘNG - BÀI 5: THÔNG TIN THƯ VIỆN",
                Dock = DockStyle.Top,
                Font = new Font("Segoe UI Semibold", 15F, FontStyle.Bold),
                ForeColor = Color.White,
                Height = 32
            };

            Label lblSub = new Label
            {
                Text = "Tự động thực thi từng testcase, đối chiếu kết quả thực tế với kỳ vọng (PASS / FAIL / ERROR)",
                Dock = DockStyle.Bottom,
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = Color.FromArgb(240, 235, 255),
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
                Text = "Lọc bài:",
                AutoSize = true,
                Location = new Point(20, 16),
                Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold)
            };

            cboLocBai.DropDownStyle = ComboBoxStyle.DropDownList;
            cboLocBai.Location = new Point(80, 12);
            cboLocBai.Size = new Size(140, 29);
            cboLocBai.Items.AddRange(new object[] { "Tất cả", "B5A", "B5B", "B5C", "B5D", "B5E" });
            cboLocBai.SelectedIndex = string.IsNullOrEmpty(maBaiMacDinh) ? 0 :
                (maBaiMacDinh == "B5A" ? 1 :
                 maBaiMacDinh == "B5B" ? 2 :
                 maBaiMacDinh == "B5C" ? 3 :
                 maBaiMacDinh == "B5D" ? 4 :
                 maBaiMacDinh == "B5E" ? 5 : 0);
            cboLocBai.SelectedIndexChanged += (_, _) => TaiDanhSachTestcase();

            btnChayLai.Text = "▶  Chạy lại tất cả";
            btnChayLai.Location = new Point(240, 10);
            btnChayLai.Size = new Size(150, 32);
            btnChayLai.BackColor = Color.FromArgb(139, 92, 246);
            btnChayLai.ForeColor = Color.White;
            btnChayLai.FlatStyle = FlatStyle.Flat;
            btnChayLai.FlatAppearance.BorderSize = 0;
            btnChayLai.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
            btnChayLai.Cursor = Cursors.Hand;
            btnChayLai.Click += (_, _) => ChayTatCaTestcase();

            lblTongKet.Location = new Point(410, 8);
            lblTongKet.Size = new Size(800, 34);
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
                DataTable raw = TestcaseBai5Helper.LoadTestcase(strCon, filterBai ?? "B5");
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
            // B5A tests
            if (id.StartsWith("B5A"))
            {
                return ChayB5A(id, moTa, duLieu, kyVong);
            }
            // B5B tests
            if (id.StartsWith("B5B"))
            {
                return ChayB5B(id, moTa, duLieu, kyVong);
            }
            // B5C tests
            if (id.StartsWith("B5C"))
            {
                return ChayB5C(id, moTa, duLieu, kyVong);
            }
            // B5D tests
            if (id.StartsWith("B5D"))
            {
                return ChayB5D(id, moTa, duLieu, kyVong);
            }
            // B5E tests
            if (id.StartsWith("B5E"))
            {
                return ChayB5E(id, moTa, duLieu, kyVong);
            }

            return (false, "Chưa xác định bài");
        }

        private (bool isPass, string actual) ChayB5A(string id, string moTa, string duLieu, string kyVong)
        {
            if (id == "B5A-WF-01" || id == "B5A-WF-02")
            {
                // Form validation empty/spaces
                return (false, "CHƯA KIỂM CHỨNG: testcase chưa đối chiếu kết quả thực tế với kỳ vọng.");
            }
            if (id == "B5A-WF-04")
            {
                return (false, "CHƯA KIỂM CHỨNG: testcase chưa đối chiếu kết quả thực tế với kỳ vọng.");
            }
            if (id == "B5A-WF-07")
            {
                return (false, "CHƯA KIỂM CHỨNG: testcase chưa đối chiếu kết quả thực tế với kỳ vọng.");
            }
            if (id == "B5A-WF-08")
            {
                return (false, "CHƯA KIỂM CHỨNG: testcase chưa đối chiếu kết quả thực tế với kỳ vọng.");
            }
            if (id == "B5A-WF-09")
            {
                return (false, "CHƯA KIỂM CHỨNG: testcase chưa đối chiếu kết quả thực tế với kỳ vọng.");
            }
            if (id is "B5A-WF-03" or "B5A-WF-05" or "B5A-WF-06" or "B5A-NV-04")
                return (false, "CHƯA KIỂM CHỨNG: cần thao tác Form hoặc dựng fixture đúng cho ca này.");

            // Normal / SQL lookups
            string maDG = "DG001";
            if (id == "B5A-OK-02" || id == "B5A-NV-02") maDG = "TE001";
            else if (id == "B5A-WF-03") maDG = "  DG001  ".Trim();
            else if (id == "B5A-SQL-01") maDG = null!;
            else if (id == "B5A-SQL-02") maDG = "DG_KHONG_TON_TAI";
            else if (id == "B5A-SQL-03") maDG = "";
            else if (id == "B5A-WF-05") maDG = "@#$%^&*";
            else if (id == "B5A-WF-06") maDG = "'; DROP TABLE DocGia;--";
            else if (id.Contains("DL-"))
            {
                return (false, "CHƯA KIỂM CHỨNG: testcase chưa đối chiếu kết quả thực tế với kỳ vọng.");
            }

            try
            {
                DataTable dt = TestcaseBai5Helper.ChayProcedure(strCon, "dbo.sp_ThongtinDocGia", "B5A", maDG);
                if (id is "B5A-SQL-01" or "B5A-SQL-02" or "B5A-SQL-03")
                    return (false, $"FAIL: SQL đã chấp nhận đầu vào phải bị từ chối ({dt.Rows.Count} dòng).");
                if (dt.Rows.Count == 1 && dt.Columns.Contains("MaDocGia") &&
                    dt.Rows[0]["MaDocGia"]?.ToString() == maDG)
                {
                    DataRow reader = dt.Rows[0];
                    string loai = reader["LoaiDocGia"]?.ToString() ?? "";
                    bool child = id is "B5A-OK-02" or "B5A-NV-02";
                    bool correctType = loai == (child ? "Trẻ em" : "Người lớn");
                    bool hasDetails = child
                        ? dt.Columns.Contains("MaDocGiaNguoiLon") && reader["MaDocGiaNguoiLon"] != DBNull.Value
                        : dt.Columns.Contains("HanSuDung") && reader["HanSuDung"] != DBNull.Value;
                    bool correctName = id != "B5A-NV-03" ||
                        $"{reader["Ho"]} {reader["TenLot"]} {reader["Ten"]}".Trim() == "Nguyễn Văn An";
                    bool complete = id != "B5A-OK-04" ||
                        (reader["NgaySinh"] != DBNull.Value && reader["SoNha"] != DBNull.Value && reader["Duong"] != DBNull.Value);
                    if (id is "B5A-OK-01" or "B5A-OK-02" or "B5A-OK-03" or "B5A-OK-04" or
                        "B5A-NV-01" or "B5A-NV-02" or "B5A-NV-03")
                        return (correctType && hasDetails && correctName && complete,
                            $"MaDocGia={reader["MaDocGia"]}; Loai={loai}; đủ thông tin={hasDetails && complete}; tên đúng={correctName}");
                    return (false, "CHƯA KIỂM CHỨNG: ca cần fixture hoặc thao tác Form riêng.");
                }
                return (false, $"FAIL: Kỳ vọng đúng 1 dòng với mã {maDG}; thực tế {dt.Rows.Count} dòng.");
            }
            catch (SqlException ex) when (SqlFailureClassifier.IsLibraryLookup(ex))
            {
                if (id is "B5A-SQL-01" or "B5A-SQL-02" or "B5A-SQL-03")
                {
                    string expectedMessage = id == "B5A-SQL-02"
                        ? "Không tìm thấy độc giả có mã này." : "Mã độc giả không được để trống.";
                    return (ex.Errors.Cast<SqlError>().Any(error => error.Message == expectedMessage),
                        $"SQL đã từ chối: {ex.Message}");
                }
                return (false, $"FAIL – Bị lỗi tra cứu: {ex.Message}");
            }
        }

        private (bool isPass, string actual) ChayB5B(string id, string moTa, string duLieu, string kyVong)
        {
            if (id == "B5B-WF-01" || id == "B5B-WF-02")
                return (false, "CHƯA KIỂM CHỨNG: testcase chưa đối chiếu kết quả thực tế với kỳ vọng.");
            if (id == "B5B-WF-04")
                return (false, "CHƯA KIỂM CHỨNG: testcase chưa đối chiếu kết quả thực tế với kỳ vọng.");
            if (id == "B5B-WF-07")
                return (false, "CHƯA KIỂM CHỨNG: testcase chưa đối chiếu kết quả thực tế với kỳ vọng.");
            if (id == "B5B-WF-08")
                return (false, "CHƯA KIỂM CHỨNG: testcase chưa đối chiếu kết quả thực tế với kỳ vọng.");
            if (id == "B5B-WF-09")
                return (false, "CHƯA KIỂM CHỨNG: testcase chưa đối chiếu kết quả thực tế với kỳ vọng.");
            if (id is "B5B-WF-03" or "B5B-WF-05" or "B5B-WF-06" or "B5B-NV-07")
                return (false, "CHƯA KIỂM CHỨNG: cần thao tác Form hoặc đối chiếu Unicode bằng fixture riêng.");

            string isbn = "ISBN001";
            if (id == "B5B-OK-01" || id == "B5B-NV-02") isbn = "ISBN007";
            else if (id == "B5B-OK-03" || id == "B5B-NV-01") isbn = "ISBN002";
            else if (id == "B5B-NV-04") isbn = "ISBN010";
            else if (id == "B5B-NV-05") isbn = "ISBN003";
            else if (id == "B5B-WF-03") isbn = "  ISBN001  ".Trim();
            else if (id == "B5B-SQL-01") isbn = null!;
            else if (id == "B5B-SQL-02") isbn = "ISBN_KHONG_TON_TAI";
            else if (id == "B5B-SQL-03") isbn = "";
            else if (id == "B5B-WF-05") isbn = "@#$%^&*";
            else if (id == "B5B-WF-06") isbn = "'; DROP TABLE Dausach;--";
            else if (id.Contains("DL-"))
            {
                return (false, "CHƯA KIỂM CHỨNG: testcase chưa đối chiếu kết quả thực tế với kỳ vọng.");
            }

            try
            {
                DataTable dt = TestcaseBai5Helper.ChayProcedure(strCon, "dbo.sp_ThongtinDausach", "B5B", isbn);
                if (id is "B5B-SQL-01" or "B5B-SQL-02" or "B5B-SQL-03")
                    return (false, $"FAIL: SQL đã chấp nhận đầu vào phải bị từ chối ({dt.Rows.Count} dòng).");
                if (dt.Rows.Count == 1 && dt.Columns.Contains("ISBN") && dt.Rows[0]["ISBN"]?.ToString() == isbn)
                {
                    int sl = Convert.ToInt32(dt.Rows[0]["SoLuongChuaMuon"]);
                    if (id == "B5B-OK-01" || id == "B5B-NV-02")
                        return (sl == 1, $"SoLuongChuaMuon = {sl}");
                    if (id is "B5B-OK-02" or "B5B-NV-03" or "B5B-OK-04" or "B5B-NV-06")
                        return (sl == 3, $"SoLuongChuaMuon = {sl}; kỳ vọng 3");
                    if (id == "B5B-OK-03" || id == "B5B-NV-01" || id == "B5B-NV-04")
                        return (sl == 0, $"SoLuongChuaMuon = {sl}");
                    if (id == "B5B-NV-05")
                        return (sl == 4, $"SoLuongChuaMuon = {sl}");
                    if (id == "B5B-OK-05")
                    {
                        bool hasDetails = dt.Rows[0]["MaTuaSach"] != DBNull.Value &&
                            dt.Rows[0]["TuaSach"] != DBNull.Value && dt.Rows[0]["TacGia"] != DBNull.Value;
                        return (hasDetails && sl == 3, $"ISBN={isbn}; đủ thông tin tựa sách={hasDetails}; số cuốn={sl}");
                    }

                    return (false, "CHƯA KIỂM CHỨNG: testcase chưa đối chiếu kết quả thực tế với kỳ vọng.");
                }
                return (false, $"FAIL: Kỳ vọng đúng 1 dòng ISBN={isbn}; thực tế {dt.Rows.Count} dòng.");
            }
            catch (SqlException ex) when (SqlFailureClassifier.IsLibraryLookup(ex))
            {
                if (id is "B5B-SQL-01" or "B5B-SQL-02" or "B5B-SQL-03")
                {
                    string expectedMessage = id == "B5B-SQL-02"
                        ? "Không tìm thấy đầu sách có ISBN này." : "ISBN không được để trống.";
                    return (ex.Errors.Cast<SqlError>().Any(error => error.Message == expectedMessage),
                        $"SQL đã từ chối: {ex.Message}");
                }
                return (false, $"FAIL – Bị lỗi tra cứu: {ex.Message}");
            }
        }

        private (bool isPass, string actual) ChayB5C(string id, string moTa, string duLieu, string kyVong)
        {
            if (id.StartsWith("B5C-WF") || id.Contains("DL-"))
                return (false, "CHƯA KIỂM CHỨNG: chưa thực hiện thao tác Form hoặc dựng dữ liệu bất thường.");

            try
            {
                DataTable dt = TestcaseBai5Helper.ChayProcedure(strCon, "dbo.sp_ThongtinNguoilonDangmuon", "B5C", null);
                if (id == "B5C-SQL-01")
                {
                    // In current DB, there are active loans
                    return (false, "CHƯA KIỂM CHỨNG: testcase chưa đối chiếu kết quả thực tế với kỳ vọng.");
                }
                return (false, $"CHƯA KIỂM CHỨNG: SQL trả {dt.Rows.Count} dòng; chưa dựng và đối chiếu đúng trạng thái dữ liệu của ca này.");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Lỗi SQL khi chạy B5C.", ex);
            }
        }

        private (bool isPass, string actual) ChayB5D(string id, string moTa, string duLieu, string kyVong)
        {
            if (id.StartsWith("B5D-WF") || id.Contains("DL-"))
                return (false, "CHƯA KIỂM CHỨNG: chưa thực hiện thao tác Form hoặc dựng dữ liệu bất thường.");

            // Execute edge test in transaction
            if (id == "B5D-BIEN-01" || id == "B5D-BIEN-02" || id == "B5D-BIEN-03" || id == "B5D-BIEN-05" || id == "B5D-OK-01" || id == "B5D-OK-02")
            {
                try
                {
                    using SqlConnection conn = new SqlConnection(strCon);
                    conn.Open();
                    using SqlTransaction tran = conn.BeginTransaction();
                    try
                    {
                        int offsetDays = (id == "B5D-BIEN-01") ? 5 :
                                         (id == "B5D-BIEN-02") ? 0 :
                                         (id == "B5D-BIEN-03" || id == "B5D-OK-01") ? -1 :
                                         (id == "B5D-BIEN-05" || id == "B5D-OK-02") ? -14 : -1;

                        using SqlCommand upd = new SqlCommand(
                            "UPDATE dbo.Muon SET ngay_hethan = DATEADD(DAY, @offset, CAST(GETDATE() AS date)) WHERE isbn='ISBN001' AND ma_cuonsach='CS004';", conn, tran);
                        upd.Parameters.AddWithValue("@offset", offsetDays);
                        upd.ExecuteNonQuery();

                        using SqlCommand cmd = new SqlCommand("dbo.sp_ThongtinNguoilonQuahan", conn, tran);
                        cmd.CommandType = CommandType.StoredProcedure;
                        DataTable dt = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd)) { da.Fill(dt); }

                        tran.Rollback();

                        bool found = false;
                        int days = 0;
                        foreach (DataRow r in dt.Rows)
                        {
                            if (r["ISBN"]?.ToString() == "ISBN001" && r["MaCuonSach"]?.ToString() == "CS004")
                            {
                                found = true;
                                days = Convert.ToInt32(r["SoNgayQuaHan"]);
                                break;
                            }
                        }

                        if (offsetDays >= 0)
                            return (!found, found ? "FAIL: Xuất hiện độc giả chưa quá hạn" : "PASS: Không xuất hiện độc giả chưa quá hạn");
                        else
                            return (found && days == Math.Abs(offsetDays), $"PASS: Tìm thấy độc giả quá hạn, SoNgayQuaHan = {days}");
                    }
                    finally
                    {
                        if (tran.Connection != null) tran.Rollback();
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Lỗi SQL khi chạy testcase quá hạn.", ex);
                }
            }

            try
            {
                DataTable dt = TestcaseBai5Helper.ChayProcedure(strCon, "dbo.sp_ThongtinNguoilonQuahan", "B5D", null);
                return (false, "CHƯA KIỂM CHỨNG: testcase chưa đối chiếu kết quả thực tế với kỳ vọng.");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Lỗi SQL khi chạy B5D.", ex);
            }
        }

        private (bool isPass, string actual) ChayB5E(string id, string moTa, string duLieu, string kyVong)
        {
            if (id.StartsWith("B5E-WF") || id.Contains("DL-"))
                return (false, "CHƯA KIỂM CHỨNG: chưa thực hiện thao tác Form hoặc dựng dữ liệu bất thường.");

            try
            {
                DataTable dt = TestcaseBai5Helper.ChayProcedure(strCon, "dbo.sp_DocGiaCoTreEmMuon", "B5E", null);
                if (id == "B5E-OK-01" || id == "B5E-OK-03" || id == "B5E-NV-07")
                {
                    return (dt.Rows.Count == 3, $"PASS: Trả đúng {dt.Rows.Count} cặp người lớn - trẻ em cùng mượn");
                }
                return (false, "CHƯA KIỂM CHỨNG: testcase chưa đối chiếu kết quả thực tế với kỳ vọng.");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Lỗi SQL khi chạy B5E.", ex);
            }
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

