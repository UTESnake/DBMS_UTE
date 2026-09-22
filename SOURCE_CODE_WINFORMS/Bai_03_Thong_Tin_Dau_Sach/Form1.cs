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

        public Form1()
        {
            InitializeComponent();
            DinhDangBangDauSach();
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

            if (cboDauSach.SelectedIndex < 0 || cboDauSach.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn một đầu sách cần kiểm tra!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboDauSach.Focus();
                return;
            }

            TraCuuDauSach(cboDauSach.SelectedValue.ToString() ?? "");
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

                txtISBN.Text = reader["ISBN"]?.ToString() ?? "";
                txtMaTuaSach.Text = reader["MaTuaSach"]?.ToString() ?? "";
                txtTuaSach.Text = reader["TuaSach"]?.ToString() ?? "";
                txtTacGia.Text = reader["TacGia"]?.ToString() ?? "";
                txtNgonNgu.Text = reader["NgonNgu"]?.ToString() ?? "";
                txtBia.Text = reader["Bia"]?.ToString() ?? "";
                txtTrangThai.Text = reader["TrangThai"]?.ToString() ?? "";
                txtTomTat.Text = reader["TomTat"]?.ToString() ?? "";
                txtSoLuongChuaMuon.Text = reader["SoLuongChuaMuon"]?.ToString() ?? "0";
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
}
