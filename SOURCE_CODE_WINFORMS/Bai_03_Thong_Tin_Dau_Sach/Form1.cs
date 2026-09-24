using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace SoLuongSachChuaMuon
{
    public partial class Form1 : Form
    {
        private readonly string strCon =
            @"Data Source=.\SQLEXPRESS02;
              Initial Catalog=QL_ThuVien;
              Integrated Security=True;
              Encrypt=False;TrustServerCertificate=True";

        private bool daKetNoi;
        private readonly Button btnChayTestcase = new Button();

        public Form1()
        {
            InitializeComponent();
            DinhDangBangDauSach();
            cboDauSach.DropDownStyle = ComboBoxStyle.DropDown;
            cboDauSach.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cboDauSach.AutoCompleteSource = AutoCompleteSource.ListItems;
            cboDauSach.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    btnKiemTra.PerformClick();
                }
            };
            cboDauSach.TextChanged += (_, _) => XoaThongTin();

            btnChayTestcase.Text = "🧪 Chạy 14 Testcase";
            btnChayTestcase.Location = new Point(415, 32);
            btnChayTestcase.Size = new Size(185, 40);
            btnChayTestcase.BackColor = Color.FromArgb(16, 185, 129);
            btnChayTestcase.ForeColor = Color.White;
            btnChayTestcase.FlatStyle = FlatStyle.Flat;
            btnChayTestcase.FlatAppearance.BorderSize = 0;
            btnChayTestcase.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnChayTestcase.Cursor = Cursors.Hand;
            btnChayTestcase.Click += (_, _) =>
            {
                using var frm = new FrmTestcaseBai3(strCon, daKetNoi);
                frm.ShowDialog(this);
            };
            grpChonDauSach.Controls.Add(btnChayTestcase);
            lblTrangThaiDuLieu.Location = new Point(615, 42);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            daKetNoi = false;
            cboDauSach.DataSource = null;
            dgvDauSach.DataSource = null;
            XoaThongTin();
            btnKetNoi.Text = "🔗 Kết nối CSDL";
            lblTrangThaiDuLieu.Text = "Chưa kết nối CSDL";
            btnTaiLai.Enabled = false;
        }

        private void DinhDangBangDauSach()
        {
            dgvDauSach.AllowUserToAddRows = false;
            dgvDauSach.AllowUserToDeleteRows = false;
            dgvDauSach.AllowUserToResizeRows = false;
            dgvDauSach.ReadOnly = true;
            dgvDauSach.RowHeadersVisible = false;
            dgvDauSach.MultiSelect = false;
            dgvDauSach.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvDauSach.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvDauSach.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvDauSach.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgvDauSach.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 9.5F, FontStyle.Bold);
        }

        private void btnKetNoi_Click(object sender, EventArgs e)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(strCon);
                conn.Open();
                NapDanhSachDauSach(conn);
                daKetNoi = true;
                btnKetNoi.Text = "✅ Đã kết nối";
                btnTaiLai.Enabled = true;
                MessageBox.Show("Kết nối cơ sở dữ liệu thành công!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                daKetNoi = false;
                btnKetNoi.Text = "🔗 Kết nối CSDL";
                btnTaiLai.Enabled = false;
                cboDauSach.DataSource = null;
                dgvDauSach.DataSource = null;
                XoaThongTin();
                lblTrangThaiDuLieu.Text = "Không tải được dữ liệu CSDL";
                MessageBox.Show("Kết nối hoặc tải dữ liệu CSDL thất bại!\n\n" + ex.Message,
                    "Lỗi kết nối", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnTaiLai_Click(object sender, EventArgs e)
        {
            if (!daKetNoi)
                return;

            try
            {
                using SqlConnection conn = new SqlConnection(strCon);
                conn.Open();
                NapDanhSachDauSach(conn);
            }
            catch (Exception ex)
            {
                lblTrangThaiDuLieu.Text = "Tải lại dữ liệu thất bại";
                MessageBox.Show("Không tải lại được danh sách đầu sách!\n\n" + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void NapDanhSachDauSach(SqlConnection conn)
        {
            const string sql = @"
                SELECT ds.isbn AS ISBN,
                       ds.ma_tuasach AS MaTuaSach,
                       ts.tuasach AS TuaSach,
                       ts.tacgia AS TacGia,
                       ds.ngonngu AS NgonNgu,
                       ds.bia AS Bia,
                       ds.trangthai AS TrangThai,
                       SUM(CASE WHEN cs.tinhtrang = N'Có sẵn' THEN 1 ELSE 0 END)
                           AS SoLuongChuaMuon
                FROM dbo.Dausach AS ds
                INNER JOIN dbo.Tuasach AS ts ON ds.ma_tuasach = ts.ma_tuasach
                LEFT JOIN dbo.Cuonsach AS cs ON ds.isbn = cs.isbn
                GROUP BY ds.isbn, ds.ma_tuasach, ts.tuasach, ts.tacgia,
                         ds.ngonngu, ds.bia, ds.trangthai
                ORDER BY ds.isbn;";

            using SqlDataAdapter adapter = new SqlDataAdapter(sql, conn);
            DataTable danhSach = new DataTable();
            adapter.Fill(danhSach);

            DataTable luaChon = new DataTable();
            luaChon.Columns.Add("ISBN", typeof(string));
            luaChon.Columns.Add("HienThi", typeof(string));
            foreach (DataRow row in danhSach.Rows)
                luaChon.Rows.Add(row["ISBN"], $"{row["ISBN"]} - {row["TuaSach"]}");

            cboDauSach.DataSource = null;
            dgvDauSach.DataSource = danhSach;
            DinhDangCotDauSach();
            cboDauSach.DisplayMember = "HienThi";
            cboDauSach.ValueMember = "ISBN";
            cboDauSach.DataSource = luaChon;
            cboDauSach.SelectedIndex = -1;
            dgvDauSach.ClearSelection();
            XoaThongTin();
            lblTrangThaiDuLieu.Text = $"Đã tải {danhSach.Rows.Count} đầu sách từ CSDL";
        }

        private void DinhDangCotDauSach()
        {
            void DinhDang(string ten, string nhan, float doRong)
            {
                DataGridViewColumn cot = dgvDauSach.Columns[ten]!;
                cot.HeaderText = nhan;
                cot.FillWeight = doRong;
            }

            DinhDang("ISBN", "ISBN", 90);
            DinhDang("MaTuaSach", "Mã tựa sách", 85);
            DinhDang("TuaSach", "Tựa sách", 200);
            DinhDang("TacGia", "Tác giả", 130);
            DinhDang("NgonNgu", "Ngôn ngữ", 90);
            DinhDang("TrangThai", "Trạng thái", 105);
            DinhDang("SoLuongChuaMuon", "Số chưa mượn", 100);
            dgvDauSach.Columns["Bia"]!.Visible = false;
            dgvDauSach.Columns["SoLuongChuaMuon"]!.DefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleCenter;
        }

        private void btnKiemTra_Click(object sender, EventArgs e)
        {
            if (!daKetNoi)
            {
                MessageBox.Show("Vui lòng kết nối CSDL trước.", "Chưa kết nối",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string isbn = cboDauSach.SelectedIndex >= 0
                ? cboDauSach.SelectedValue?.ToString() ?? ""
                : cboDauSach.Text.Trim();
            if (isbn.Contains(" - "))
            {
                isbn = isbn.Substring(0, isbn.IndexOf(" - ")).Trim();
            }
            XoaThongTin();
            if (string.IsNullOrWhiteSpace(isbn) || isbn.Length > 20)
            {
                MessageBox.Show("Vui lòng nhập hoặc chọn ISBN từ 1 đến 20 ký tự.", "Dữ liệu không hợp lệ",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboDauSach.Focus();
                return;
            }
            TraCuuDauSach(isbn);
        }

        private void TraCuuDauSach(string isbn)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(strCon);
                using SqlCommand cmd = new SqlCommand("dbo.sp_ThongTinDauSach", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@ISBN", SqlDbType.VarChar, 20).Value = isbn;
                conn.Open();

                using SqlDataReader reader = cmd.ExecuteReader();
                if (!reader.Read())
                {
                    XoaThongTin();
                    MessageBox.Show("Không tìm thấy thông tin đầu sách!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                string foundIsbn = reader["ISBN"]?.ToString() ?? "";
                txtISBN.Text = foundIsbn;
                txtMaTuaSach.Text = reader["MaTuaSach"]?.ToString() ?? "";
                txtTuaSach.Text = reader["TuaSach"]?.ToString() ?? "";
                txtTacGia.Text = reader["TacGia"]?.ToString() ?? "";
                txtNgonNgu.Text = reader["NgonNgu"]?.ToString() ?? "";
                txtBia.Text = reader["Bia"]?.ToString() ?? "";
                txtTrangThai.Text = reader["TrangThai"]?.ToString() ?? "";
                txtTomTat.Text = reader["TomTat"]?.ToString() ?? "";
                txtSoLuongChuaMuon.Text = reader["SoLuongChuaMuon"]?.ToString() ?? "0";

                if (cboDauSach.SelectedValue?.ToString() != foundIsbn)
                {
                    cboDauSach.SelectedValue = foundIsbn;
                }

                foreach (DataGridViewRow row in dgvDauSach.Rows)
                {
                    if (string.Equals(row.Cells["ISBN"].Value?.ToString(), foundIsbn, StringComparison.OrdinalIgnoreCase))
                    {
                        row.Selected = true;
                        dgvDauSach.CurrentCell = row.Cells[0];
                        break;
                    }
                }
            }
            catch (SqlException ex) when (DoAn.Shared.SqlFailureClassifier.IsLibraryLookup(ex))
            {
                XoaThongTin();
                MessageBox.Show(ex.Message, "Thông báo tra cứu", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                XoaThongTin();
                MessageBox.Show("Không tra cứu được đầu sách!\n\n" + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvDauSach_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || !daKetNoi)
                return;

            string isbn = dgvDauSach.Rows[e.RowIndex].Cells["ISBN"].Value?.ToString() ?? "";
            if (string.IsNullOrWhiteSpace(isbn))
                return;

            cboDauSach.SelectedValue = isbn;
            TraCuuDauSach(isbn);
        }

        private void cboDauSach_SelectedIndexChanged(object sender, EventArgs e)
        {
            XoaThongTin();
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            if (cboDauSach.DataSource != null)
                cboDauSach.SelectedIndex = -1;
            cboDauSach.Text = "";
            dgvDauSach.ClearSelection();
            XoaThongTin();
            cboDauSach.Focus();
        }

        private void XoaThongTin()
        {
            txtISBN.Clear();
            txtMaTuaSach.Clear();
            txtTuaSach.Clear();
            txtTacGia.Clear();
            txtNgonNgu.Clear();
            txtBia.Clear();
            txtTrangThai.Clear();
            txtTomTat.Clear();
            txtSoLuongChuaMuon.Clear();
        }

        private void pnlHeader_Paint(object sender, PaintEventArgs e)
        {
        }
    }

    public sealed class FrmTestcaseBai3 : Form
    {
        private readonly string strCon;
        private readonly bool daKetNoi;
        private readonly DataTable dtTestcase = new DataTable();
        private readonly DataGridView dgvTestcase = new DataGridView();
        private readonly Label lblThongKe = new Label();
        private readonly Button btnChayLai = new Button();
        private readonly Button btnDong = new Button();

        public FrmTestcaseBai3(string connectionString, bool isConnected)
        {
            strCon = connectionString;
            daKetNoi = isConnected;

            Text = "Kiểm thử tự động Bài 3 - Thông tin đầu sách (14 testcases)";
            Size = new Size(1150, 700);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.WhiteSmoke;
            Font = new Font("Segoe UI", 9.5F);

            BuildUI();
            NapDanhSachTestcase();
            ChayTatCaTestcase();
        }

        private void BuildUI()
        {
            Panel pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.FromArgb(16, 185, 129),
                Padding = new Padding(25, 15, 25, 15)
            };
            Label lblTitle = new Label
            {
                Text = "BỘ TESTCASE TỰ ĐỘNG BÀI 3 — THÔNG TIN ĐẦU SÁCH",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(25, 15)
            };
            Label lblSubtitle = new Label
            {
                Text = "14 testcases bổ sung bắt buộc: tra cứu hợp lệ, rỗng, khoảng trắng, quá độ dài, ký tự lạ, SQL injection, NULL, khóa nút",
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = Color.FromArgb(220, 252, 231),
                AutoSize = true,
                Location = new Point(27, 48)
            };
            pnlHeader.Controls.AddRange([lblTitle, lblSubtitle]);
            Controls.Add(pnlHeader);

            Panel pnlFooter = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                BackColor = Color.White,
                Padding = new Padding(20, 10, 20, 10)
            };
            lblThongKe.Dock = DockStyle.Left;
            lblThongKe.AutoSize = true;
            lblThongKe.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblThongKe.TextAlign = ContentAlignment.MiddleLeft;
            lblThongKe.Location = new Point(20, 18);

            btnDong.Text = "Đóng";
            btnDong.Size = new Size(110, 38);
            btnDong.Dock = DockStyle.Right;
            btnDong.BackColor = Color.FromArgb(241, 245, 249);
            btnDong.FlatStyle = FlatStyle.Flat;
            btnDong.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            btnDong.Cursor = Cursors.Hand;
            btnDong.Click += (_, _) => Close();

            btnChayLai.Text = "▶ Chạy lại testcase";
            btnChayLai.Size = new Size(160, 38);
            btnChayLai.Dock = DockStyle.Right;
            btnChayLai.BackColor = Color.FromArgb(16, 185, 129);
            btnChayLai.ForeColor = Color.White;
            btnChayLai.FlatStyle = FlatStyle.Flat;
            btnChayLai.FlatAppearance.BorderSize = 0;
            btnChayLai.Cursor = Cursors.Hand;
            btnChayLai.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnChayLai.Click += (_, _) => ChayTatCaTestcase();

            pnlFooter.Controls.AddRange([lblThongKe, btnDong, btnChayLai]);
            Controls.Add(pnlFooter);

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

            dgvTestcase.Columns["MaCase"].HeaderText = "Mã testcase";
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

        private void NapDanhSachTestcase()
        {
            dtTestcase.Rows.Clear();
            dtTestcase.Rows.Add("B3-ADD-01", "ISBN hợp lệ có cuốn có sẵn", "ISBN001", "Trả đúng đầu sách; SoLuongChuaMuon = 3", "", "CHƯA CHẠY");
            dtTestcase.Rows.Add("B3-ADD-02", "ISBN hợp lệ nhưng không còn cuốn có sẵn", "ISBN002", "Trả đúng đầu sách; SoLuongChuaMuon = 0", "", "CHƯA CHẠY");
            dtTestcase.Rows.Add("B3-ADD-03", "ISBN hợp lệ có trạng thái hỗn hợp", "ISBN004", "Trả đúng đầu sách; SoLuongChuaMuon = 2", "", "CHƯA CHẠY");
            dtTestcase.Rows.Add("B3-ADD-04", "Đầu sách chưa có cuốn vật lý", "ISBN010", "Vẫn trả 1 dòng; SoLuongChuaMuon = 0", "", "CHƯA CHẠY");
            dtTestcase.Rows.Add("B3-ADD-05", "ISBN không tồn tại", "ISBN_KHONG_TON_TAI", "Thông báo nghiệp vụ 'Không tìm thấy'", "", "CHƯA CHẠY");
            dtTestcase.Rows.Add("B3-ADD-06", "ISBN rỗng", "(rỗng)", "Form chặn trước SQL", "", "CHƯA CHẠY");
            dtTestcase.Rows.Add("B3-ADD-07", "ISBN chỉ có khoảng trắng", "     ", "Trim thành rỗng và chặn trước SQL", "", "CHƯA CHẠY");
            dtTestcase.Rows.Add("B3-ADD-08", "Khoảng trắng đầu/cuối", "  ISBN001  ", "Trim và trả kết quả như ISBN001 (SoLuongChuaMuon = 3)", "", "CHƯA CHẠY");
            dtTestcase.Rows.Add("B3-ADD-09", "Chuỗi quá 20 ký tự", "ISBN_ABCDEFGHIJKLMNOPQRSTUVWXYZ", "Form báo quá độ dài trước SQL", "", "CHƯA CHẠY");
            dtTestcase.Rows.Add("B3-ADD-10", "Ký tự đặc biệt", "@#$%^&*", "Không tìm thấy; không lỗi hệ thống", "", "CHƯA CHẠY");
            dtTestcase.Rows.Add("B3-ADD-11", "Chuỗi SQL injection", "'; DROP TABLE Dausach;--", "Parameter hóa an toàn; không tìm thấy", "", "CHƯA CHẠY");
            dtTestcase.Rows.Add("B3-ADD-12", "Giá trị NULL gọi trực tiếp SQL", "NULL", "Procedure từ chối; thông báo nghiệp vụ", "", "CHƯA CHẠY");
            dtTestcase.Rows.Add("B3-ADD-13", "Nhấn Tra cứu liên tục", "CLICK_MULTI", "Khóa nút khi thực thi; chống chạy lặp", "", "CHƯA CHẠY");
            dtTestcase.Rows.Add("B3-ADD-14", "Chưa kết nối CSDL", "DISCONNECTED", "Form chặn và thông báo cần kết nối trước", "", "CHƯA CHẠY");
        }

        private void ChayTatCaTestcase()
        {
            using var operation = DoAn.Shared.FormOperation.TryStart(this);
            if (operation is null) return;
            btnChayLai.Enabled = false;
            int pass = 0, fail = 0, error = 0, unverified = 0;

            foreach (DataRow row in dtTestcase.Rows)
            {
                string id = row["MaCase"].ToString()!;
                row["TrangThai"] = "ĐANG CHẠY";
                dgvTestcase.Refresh();
                Application.DoEvents();

                try
                {
                    (bool isPass, string actual) = ChayMotCase(id);
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
                    row["KetQua"] = ex.Message;
                    row["TrangThai"] = "ERROR";
                    error++;
                }

                dgvTestcase.Refresh();
                Application.DoEvents();
            }

            btnChayLai.Enabled = true;
            lblThongKe.Text = $"Tổng: {dtTestcase.Rows.Count} | PASS: {pass} | FAIL: {fail} | ERROR: {error} | CHƯA KIỂM CHỨNG: {unverified}";
            lblThongKe.ForeColor = fail == 0 && error == 0 ? Color.SeaGreen : Color.Firebrick;
        }

        private (bool isPass, string actual) ChayMotCase(string id)
        {
            if (id is "B3-ADD-06" or "B3-ADD-07" or "B3-ADD-09" or "B3-ADD-13" or "B3-ADD-14")
                return (false, "CHƯA KIỂM CHỨNG: cần thử thao tác trên Form; điều kiện mô phỏng không chứng minh kết quả.");
            switch (id)
            {
                case "B3-ADD-01":
                {
                    DataTable dt = QuerySql("ISBN001");
                    if (dt.Rows.Count == 1 && Convert.ToInt32(dt.Rows[0]["SoLuongChuaMuon"]) == 3)
                        return (true, $"Tìm thấy đầu sách TS01; Số chưa mượn: {dt.Rows[0]["SoLuongChuaMuon"]}");
                    return (false, $"Số dòng: {dt.Rows.Count}; Số chưa mượn: {(dt.Rows.Count > 0 ? dt.Rows[0]["SoLuongChuaMuon"] : "N/A")}");
                }
                case "B3-ADD-02":
                {
                    DataTable dt = QuerySql("ISBN002");
                    if (dt.Rows.Count == 1 && Convert.ToInt32(dt.Rows[0]["SoLuongChuaMuon"]) == 0)
                        return (true, $"Tìm thấy đầu sách TS02; Số chưa mượn: {dt.Rows[0]["SoLuongChuaMuon"]}");
                    return (false, $"Số dòng: {dt.Rows.Count}; Số chưa mượn: {(dt.Rows.Count > 0 ? dt.Rows[0]["SoLuongChuaMuon"] : "N/A")}");
                }
                case "B3-ADD-03":
                {
                    DataTable dt = QuerySql("ISBN004");
                    if (dt.Rows.Count == 1 && Convert.ToInt32(dt.Rows[0]["SoLuongChuaMuon"]) == 2)
                        return (true, $"Tìm thấy đầu sách TS04; Số chưa mượn: {dt.Rows[0]["SoLuongChuaMuon"]}");
                    return (false, $"Số dòng: {dt.Rows.Count}; Số chưa mượn: {(dt.Rows.Count > 0 ? dt.Rows[0]["SoLuongChuaMuon"] : "N/A")}");
                }
                case "B3-ADD-04":
                {
                    DataTable dt = QuerySql("ISBN010");
                    if (dt.Rows.Count == 1 && Convert.ToInt32(dt.Rows[0]["SoLuongChuaMuon"]) == 0)
                        return (true, $"Tìm thấy đầu sách TS10 (0 cuốn vật lý); Số chưa mượn: 0");
                    return (false, $"Số dòng: {dt.Rows.Count}; Số chưa mượn: {(dt.Rows.Count > 0 ? dt.Rows[0]["SoLuongChuaMuon"] : "N/A")}");
                }
                case "B3-ADD-05":
                {
                    try
                    {
                        QuerySql("ISBN_KHONG_TON_TAI");
                        return (false, "Lẽ ra phải chặn mã không tồn tại");
                    }
                    catch (SqlException ex) when (DoAn.Shared.SqlFailureClassifier.IsLibraryLookup(ex))
                    {
                        return (true, $"PASS – CSDL đã chặn đúng: {ex.Message}");
                    }
                }
                case "B3-ADD-06":
                {
                    string input = "";
                    if (string.IsNullOrWhiteSpace(input) || input.Length > 20)
                        return (true, "PASS – Form đã chặn đúng: Vui lòng nhập hoặc chọn ISBN từ 1 đến 20 ký tự.");
                    return (false, "Không chặn chuỗi rỗng");
                }
                case "B3-ADD-07":
                {
                    string input = "     ";
                    if (string.IsNullOrWhiteSpace(input.Trim()))
                        return (true, "PASS – Form đã chặn đúng: Trim thành rỗng và chặn trước SQL.");
                    return (false, "Không chặn khoảng trắng");
                }
                case "B3-ADD-08":
                {
                    string input = "  ISBN001  ".Trim();
                    DataTable dt = QuerySql(input);
                    if (dt.Rows.Count == 1 && Convert.ToInt32(dt.Rows[0]["SoLuongChuaMuon"]) == 3)
                        return (true, $"Trim thành công và trả đúng TS01; Số chưa mượn: 3");
                    return (false, "Trim hoặc kết quả thất bại");
                }
                case "B3-ADD-09":
                {
                    string input = "ISBN_ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                    if (input.Length > 20)
                        return (true, "PASS – Form đã chặn đúng: ISBN không được dài quá 20 ký tự.");
                    return (false, "Không chặn độ dài");
                }
                case "B3-ADD-10":
                {
                    try
                    {
                        QuerySql("@#$%^&*");
                        return (false, "Lẽ ra phải báo không tìm thấy");
                    }
                    catch (SqlException ex) when (DoAn.Shared.SqlFailureClassifier.IsLibraryLookup(ex))
                    {
                        return (true, $"PASS – CSDL đã chặn đúng: {ex.Message}");
                    }
                }
                case "B3-ADD-11":
                {
                    try
                    {
                        QuerySql("'; DROP TABLE Dausach;--");
                        return (false, "Lẽ ra phải báo không tìm thấy");
                    }
                    catch (SqlException ex) when (DoAn.Shared.SqlFailureClassifier.IsLibraryLookup(ex))
                    {
                        return (true, $"PASS – Parameter an toàn; CSDL chặn: {ex.Message}");
                    }
                }
                case "B3-ADD-12":
                {
                    try
                    {
                        using SqlConnection conn = new SqlConnection(strCon);
                        using SqlCommand cmd = new SqlCommand("dbo.sp_ThongTinDauSach", conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ISBN", SqlDbType.VarChar, 20).Value = DBNull.Value;
                        conn.Open();
                        using var reader = cmd.ExecuteReader();
                        return (false, "Lẽ ra procedure phải từ chối NULL");
                    }
                    catch (SqlException ex) when (DoAn.Shared.SqlFailureClassifier.IsLibraryLookup(ex))
                    {
                        return (true, $"PASS – Procedure từ chối đúng: {ex.Message}");
                    }
                }
                case "B3-ADD-13":
                {
                    return (true, "PASS – Đã khóa nút và kiểm soát ActiveOperation chống click lặp/treo UI.");
                }
                case "B3-ADD-14":
                {
                    bool testConnected = false;
                    if (!testConnected)
                        return (true, "PASS – Form đã chặn đúng: Vui lòng kết nối CSDL trước.");
                    return (false, "Không chặn khi chưa kết nối");
                }
                default:
                    return (false, "Testcase không xác định");
            }
        }

        private DataTable QuerySql(string isbn)
        {
            using SqlConnection conn = new SqlConnection(strCon);
            using SqlCommand cmd = new SqlCommand("dbo.sp_ThongTinDauSach", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@ISBN", SqlDbType.VarChar, 20).Value = isbn;
            conn.Open();
            using SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }

        private void DgvTestcase_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex < 0 || dgvTestcase.Columns[e.ColumnIndex].DataPropertyName != "TrangThai")
                return;

            string status = e.Value?.ToString() ?? "";
            switch (status)
            {
                case "PASS":
                    e.CellStyle.ForeColor = Color.DarkGreen;
                    e.CellStyle.BackColor = Color.FromArgb(220, 252, 231);
                    e.CellStyle.Font = new Font(dgvTestcase.Font, FontStyle.Bold);
                    break;
                case "FAIL":
                    e.CellStyle.ForeColor = Color.DarkRed;
                    e.CellStyle.BackColor = Color.FromArgb(254, 226, 226);
                    e.CellStyle.Font = new Font(dgvTestcase.Font, FontStyle.Bold);
                    break;
                case "ERROR":
                    e.CellStyle.ForeColor = Color.DarkOrange;
                    e.CellStyle.BackColor = Color.FromArgb(254, 243, 199);
                    e.CellStyle.Font = new Font(dgvTestcase.Font, FontStyle.Bold);
                    break;
                case "ĐANG CHẠY":
                    e.CellStyle.ForeColor = Color.Blue;
                    e.CellStyle.Font = new Font(dgvTestcase.Font, FontStyle.Bold);
                    break;
            }
        }
    }
}
