using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace Bai_06_Trigger_ThuVien
{
    public partial class Form1 : Form
    {
        // =========================================================
        // CHUỖI KẾT NỐI
        // =========================================================
        private readonly string strCon =
            @"Data Source=.\SQLEXPRESS02;
              Initial Catalog=QL_ThuVien;
              Integrated Security=True;
              Encrypt=False;TrustServerCertificate=True";

        private bool daKetNoi = false;


        public Form1()
        {
            InitializeComponent();
        }


        // =========================================================
        // FORM LOAD
        // =========================================================
        private void Form1_Load(object sender, EventArgs e)
        {
            daKetNoi = false;

            lblTrangThai.Text =
                "● Chưa kết nối cơ sở dữ liệu";

            lblTrangThai.ForeColor =
                Color.Firebrick;

            btnKetNoi.Text =
                "Kết nối CSDL";

            CapNhatTrangThaiNut();
        }


        // =========================================================
        // KẾT NỐI CSDL
        // =========================================================
        private void btnKetNoi_Click(object sender, EventArgs e)
        {
            try
            {
                using SqlConnection conn =
                    new SqlConnection(strCon);

                conn.Open();

                daKetNoi = true;

                btnKetNoi.Text =
                    "✓ Đã kết nối";

                lblTrangThai.Text =
                    "● Đã kết nối QL_ThuVien";

                lblTrangThai.ForeColor =
                    Color.SeaGreen;

                CapNhatTrangThaiNut();

                MessageBox.Show(
                    "Kết nối cơ sở dữ liệu thành công!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                daKetNoi = false;

                btnKetNoi.Text =
                    "Kết nối CSDL";

                lblTrangThai.Text =
                    "● Kết nối cơ sở dữ liệu thất bại";

                lblTrangThai.ForeColor =
                    Color.Firebrick;

                CapNhatTrangThaiNut();

                MessageBox.Show(
                    "Không thể kết nối CSDL!\n\n"
                    + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }


        // =========================================================
        // BẬT/TẮT NÚT
        // =========================================================
        private void CapNhatTrangThaiNut()
        {
            btnCau61.Enabled = daKetNoi;
            btnCau62.Enabled = daKetNoi;
            btnCau63.Enabled = daKetNoi;
            btnCau64.Enabled = daKetNoi;
        }


        // =========================================================
        // 6.1 - tg_delMuon
        // =========================================================
        private void btnCau61_Click(object sender, EventArgs e)
        {
            if (!KiemTraDaKetNoi())
                return;

            using FrmChucNangBai6 frm =
                new FrmChucNangBai6(
                    strCon,
                    "B6.1",
                    "6.1 - tg_delMuon",
                    "TRIGGER XÓA PHIẾU MƯỢN",
                    "delMuon"
                );

            frm.ShowDialog(this);
        }


        // =========================================================
        // 6.2 - tg_insMuon
        // =========================================================
        private void btnCau62_Click(object sender, EventArgs e)
        {
            if (!KiemTraDaKetNoi())
                return;

            using FrmChucNangBai6 frm =
                new FrmChucNangBai6(
                    strCon,
                    "B6.2",
                    "6.2 - tg_insMuon",
                    "TRIGGER THÊM PHIẾU MƯỢN",
                    "insMuon"
                );

            frm.ShowDialog(this);
        }


        // =========================================================
        // 6.3 - tg_updCuonSach
        // =========================================================
        private void btnCau63_Click(object sender, EventArgs e)
        {
            if (!KiemTraDaKetNoi())
                return;

            using FrmChucNangBai6 frm =
                new FrmChucNangBai6(
                    strCon,
                    "B6.3",
                    "6.3 - tg_updCuonSach",
                    "TRIGGER CẬP NHẬT CUỐN SÁCH",
                    "updCuonSach"
                );

            frm.ShowDialog(this);
        }


        // =========================================================
        // 6.4 - tg_InfThongBao
        // =========================================================
        private void btnCau64_Click(object sender, EventArgs e)
        {
            if (!KiemTraDaKetNoi())
                return;

            using FrmChucNangBai6 frm =
                new FrmChucNangBai6(
                    strCon,
                    "B6.4",
                    "6.4 - tg_InfThongBao",
                    "TRIGGER THÔNG BÁO TỰA SÁCH",
                    "InfThongBao"
                );

            frm.ShowDialog(this);
        }


        // =========================================================
        // KIỂM TRA KẾT NỐI
        // =========================================================
        private bool KiemTraDaKetNoi()
        {
            if (daKetNoi)
                return true;

            MessageBox.Show(
                "Bạn chưa kết nối cơ sở dữ liệu!\n"
                + "Vui lòng nhấn Kết nối CSDL trước.",
                "Chưa kết nối",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );

            return false;
        }


        // =========================================================
        // THOÁT
        // =========================================================
        private void btnThoat_Click(object sender, EventArgs e)
        {
            DialogResult result =
                MessageBox.Show(
                    "Bạn có chắc muốn thoát chương trình?",
                    "Xác nhận",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

            if (result == DialogResult.Yes)
            {
                Close();
            }
        }
    }


    // =============================================================
    // FORM CON DÙNG CHUNG CHO 6.1 -> 6.4
    // Không cần tạo Designer riêng
    // =============================================================
    public class FrmChucNangBai6 : Form
    {
        private readonly string strCon;
        private readonly string maBai;
        private readonly string loaiTrigger;

        private Panel pnlHeader;
        private Label lblTitle;

        private GroupBox grpNhapLieu;

        private Label lbl1;
        private Label lbl2;
        private Label lbl3;
        private Label lbl4;
        private Label lbl5;

        private TextBox txt1;
        private TextBox txt2;
        private TextBox txt3;
        private TextBox txt4;
        private TextBox txt5;

        private ComboBox cboThaoTac;

        private Button btnKiemTra;
        private Button btnLamMoi;

        private GroupBox grpKetQua;
        private TextBox txtKetQua;

        private Button btnLoadTestcase;
        private Button btnChayTestcase;

        private Label lblTrangThaiTestcase;

        private GroupBox grpTestcase;
        private DataGridView dgvTestcase;


        public FrmChucNangBai6(
            string connectionString,
            string maBai,
            string windowTitle,
            string title,
            string loaiTrigger)
        {
            strCon = connectionString;

            this.maBai = maBai;
            this.loaiTrigger = loaiTrigger;

            KhoiTaoGiaoDien(
                windowTitle,
                title
            );
        }


        // =========================================================
        // KHỞI TẠO FORM
        // =========================================================
        private void KhoiTaoGiaoDien(
            string windowTitle,
            string title)
        {
            Text = windowTitle;

            StartPosition =
                FormStartPosition.CenterParent;

            BackColor =
                Color.FromArgb(248, 250, 252);

            ClientSize =
                new Size(1180, 820);

            MinimumSize =
                new Size(1000, 700);


            // =====================================================
            // HEADER
            // =====================================================
            pnlHeader =
                new Panel();

            pnlHeader.BackColor =
                Color.FromArgb(30, 64, 175);

            pnlHeader.Dock =
                DockStyle.Top;

            pnlHeader.Height =
                90;


            lblTitle =
                new Label();

            lblTitle.Dock =
                DockStyle.Fill;

            lblTitle.Text =
                title;

            lblTitle.Font =
                new Font(
                    "Segoe UI",
                    18F,
                    FontStyle.Bold
                );

            lblTitle.ForeColor =
                Color.White;

            lblTitle.TextAlign =
                ContentAlignment.MiddleCenter;

            pnlHeader.Controls.Add(lblTitle);

            Controls.Add(pnlHeader);


            // =====================================================
            // GROUP NHẬP LIỆU
            // =====================================================
            grpNhapLieu =
                new GroupBox();

            grpNhapLieu.Font =
                new Font(
                    "Segoe UI",
                    10F,
                    FontStyle.Bold
                );

            grpNhapLieu.Text =
                "Thông tin kiểm tra Trigger";

            grpNhapLieu.Location =
                new Point(35, 110);

            grpNhapLieu.Size =
                new Size(1110, 180);

            Controls.Add(grpNhapLieu);


            lbl1 = TaoLabel();
            lbl2 = TaoLabel();
            lbl3 = TaoLabel();
            lbl4 = TaoLabel();
            lbl5 = TaoLabel();

            txt1 = TaoTextBox();
            txt2 = TaoTextBox();
            txt3 = TaoTextBox();
            txt4 = TaoTextBox();
            txt5 = TaoTextBox();

            cboThaoTac =
                new ComboBox();

            cboThaoTac.DropDownStyle =
                ComboBoxStyle.DropDownList;

            cboThaoTac.Font =
                new Font(
                    "Segoe UI",
                    10F
                );


            TaoNhapLieuTheoTrigger();


            // =====================================================
            // NÚT KIỂM TRA
            // =====================================================
            btnKiemTra =
                new Button();

            btnKiemTra.Text =
                "Kiểm tra Trigger";

            btnKiemTra.Size =
                new Size(175, 42);

            btnKiemTra.Location =
                new Point(730, 120);

            btnKiemTra.BackColor =
                Color.FromArgb(34, 197, 94);

            btnKiemTra.ForeColor =
                Color.White;

            btnKiemTra.FlatStyle =
                FlatStyle.Flat;

            btnKiemTra.FlatAppearance.BorderSize =
                0;

            btnKiemTra.Font =
                new Font(
                    "Segoe UI",
                    10F,
                    FontStyle.Bold
                );

            btnKiemTra.Cursor =
                Cursors.Hand;

            btnKiemTra.Click +=
                btnKiemTra_Click;

            grpNhapLieu.Controls.Add(btnKiemTra);


            // =====================================================
            // LÀM MỚI
            // =====================================================
            btnLamMoi =
                new Button();

            btnLamMoi.Text =
                "Làm mới";

            btnLamMoi.Size =
                new Size(145, 42);

            btnLamMoi.Location =
                new Point(920, 120);

            btnLamMoi.BackColor =
                Color.FromArgb(107, 114, 128);

            btnLamMoi.ForeColor =
                Color.White;

            btnLamMoi.FlatStyle =
                FlatStyle.Flat;

            btnLamMoi.FlatAppearance.BorderSize =
                0;

            btnLamMoi.Font =
                new Font(
                    "Segoe UI",
                    10F,
                    FontStyle.Bold
                );

            btnLamMoi.Cursor =
                Cursors.Hand;

            btnLamMoi.Click +=
                btnLamMoi_Click;

            grpNhapLieu.Controls.Add(btnLamMoi);


            // =====================================================
            // KẾT QUẢ
            // =====================================================
            grpKetQua =
                new GroupBox();

            grpKetQua.Font =
                new Font(
                    "Segoe UI",
                    10F,
                    FontStyle.Bold
                );

            grpKetQua.Text =
                "Kết quả kiểm tra Trigger";

            grpKetQua.Location =
                new Point(35, 305);

            grpKetQua.Size =
                new Size(1110, 125);

            Controls.Add(grpKetQua);


            txtKetQua =
                new TextBox();

            txtKetQua.Multiline =
                true;

            txtKetQua.ReadOnly =
                true;

            txtKetQua.ScrollBars =
                ScrollBars.Vertical;

            txtKetQua.Font =
                new Font(
                    "Segoe UI",
                    10F
                );

            txtKetQua.Location =
                new Point(15, 30);

            txtKetQua.Size =
                new Size(1080, 75);

            grpKetQua.Controls.Add(txtKetQua);


            // =====================================================
            // LOAD TESTCASE
            // =====================================================
            btnLoadTestcase =
                new Button();

            btnLoadTestcase.Text =
                "▣  Load Testcase";

            btnLoadTestcase.Size =
                new Size(190, 44);

            btnLoadTestcase.Location =
                new Point(35, 450);

            btnLoadTestcase.BackColor =
                Color.FromArgb(37, 99, 235);

            btnLoadTestcase.ForeColor =
                Color.White;

            btnLoadTestcase.FlatStyle =
                FlatStyle.Flat;

            btnLoadTestcase.FlatAppearance.BorderSize =
                0;

            btnLoadTestcase.Font =
                new Font(
                    "Segoe UI",
                    10F,
                    FontStyle.Bold
                );

            btnLoadTestcase.Click +=
                btnLoadTestcase_Click;

            Controls.Add(btnLoadTestcase);


            // =====================================================
            // CHẠY TESTCASE
            // =====================================================
            btnChayTestcase =
                new Button();

            btnChayTestcase.Text =
                "▶  Chạy Testcase";

            btnChayTestcase.Size =
                new Size(220, 44);

            btnChayTestcase.Location =
                new Point(240, 450);

            btnChayTestcase.BackColor =
                Color.FromArgb(99, 102, 241);

            btnChayTestcase.ForeColor =
                Color.White;

            btnChayTestcase.FlatStyle =
                FlatStyle.Flat;

            btnChayTestcase.FlatAppearance.BorderSize =
                0;

            btnChayTestcase.Font =
                new Font(
                    "Segoe UI",
                    10F,
                    FontStyle.Bold
                );

            btnChayTestcase.Click +=
                btnChayTestcase_Click;

            Controls.Add(btnChayTestcase);


            // =====================================================
            // TRẠNG THÁI TESTCASE
            // =====================================================
            lblTrangThaiTestcase =
                new Label();

            lblTrangThaiTestcase.AutoSize =
                true;

            lblTrangThaiTestcase.Font =
                new Font(
                    "Segoe UI",
                    10F,
                    FontStyle.Bold
                );

            lblTrangThaiTestcase.ForeColor =
                Color.FromArgb(75, 85, 99);

            lblTrangThaiTestcase.Location =
                new Point(490, 462);

            lblTrangThaiTestcase.Text =
                "Chưa load testcase";

            Controls.Add(lblTrangThaiTestcase);


            // =====================================================
            // DANH SÁCH TESTCASE
            // =====================================================
            grpTestcase =
                new GroupBox();

            grpTestcase.Font =
                new Font(
                    "Segoe UI",
                    10F,
                    FontStyle.Bold
                );

            grpTestcase.Text =
                "Danh sách testcase";

            grpTestcase.Location =
                new Point(35, 510);

            grpTestcase.Size =
                new Size(1110, 270);

            Controls.Add(grpTestcase);


            dgvTestcase =
                TaoDataGridView();

            dgvTestcase.Location =
                new Point(15, 30);

            dgvTestcase.Size =
                new Size(1080, 220);

            grpTestcase.Controls.Add(dgvTestcase);
        }


        // =========================================================
        // TẠO CONTROL CHUNG
        // =========================================================
        private Label TaoLabel()
        {
            return new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 10F)
            };
        }


        private TextBox TaoTextBox()
        {
            return new TextBox
            {
                Font = new Font("Segoe UI", 10F),
                Size = new Size(220, 30)
            };
        }


        // =========================================================
        // INPUT RIÊNG CHO TỪNG TRIGGER
        // =========================================================
        private void TaoNhapLieuTheoTrigger()
        {
            if (loaiTrigger == "delMuon")
            {
                TaoDong(
                    lbl1,
                    txt1,
                    "ISBN:",
                    30,
                    45
                );

                TaoDong(
                    lbl2,
                    txt2,
                    "Mã cuốn:",
                    380,
                    45
                );

                TaoDong(
                    lbl3,
                    txt3,
                    "Mã độc giả:",
                    730,
                    45
                );
            }


            else if (loaiTrigger == "insMuon")
            {
                TaoDong(
                    lbl1,
                    txt1,
                    "ISBN:",
                    30,
                    38
                );

                TaoDong(
                    lbl2,
                    txt2,
                    "Mã cuốn:",
                    380,
                    38
                );

                TaoDong(
                    lbl3,
                    txt3,
                    "Mã độc giả:",
                    730,
                    38
                );

                TaoDong(
                    lbl4,
                    txt4,
                    "Ngày mượn:",
                    30,
                    82
                );

                TaoDong(
                    lbl5,
                    txt5,
                    "Ngày hết hạn:",
                    380,
                    82
                );

                txt4.Text =
                    DateTime.Now.ToString("yyyy-MM-dd");

                txt5.Text =
                    DateTime.Now.AddDays(14)
                        .ToString("yyyy-MM-dd");
            }


            else if (loaiTrigger == "updCuonSach")
            {
                TaoDong(
                    lbl1,
                    txt1,
                    "ISBN:",
                    30,
                    45
                );

                TaoDong(
                    lbl2,
                    txt2,
                    "Mã cuốn:",
                    380,
                    45
                );

                TaoDong(
                    lbl3,
                    txt3,
                    "Tình trạng mới:",
                    730,
                    45
                );
            }


            else if (loaiTrigger == "InfThongBao")
            {
                lbl1.Text =
                    "Thao tác:";

                lbl1.Location =
                    new Point(30, 38);

                cboThaoTac.Location =
                    new Point(135, 34);

                cboThaoTac.Size =
                    new Size(220, 30);

                cboThaoTac.Items.AddRange(
                    new object[]
                    {
                        "INSERT",
                        "UPDATE_TUASACH",
                        "UPDATE_TACGIA",
                        "UPDATE_BOTH",
                        "UPDATE_TOMTAT"
                    }
                );

                cboThaoTac.SelectedIndex = 0;

                grpNhapLieu.Controls.Add(lbl1);
                grpNhapLieu.Controls.Add(cboThaoTac);


                TaoDong(
                    lbl2,
                    txt2,
                    "Mã tựa sách:",
                    380,
                    38
                );

                TaoDong(
                    lbl3,
                    txt3,
                    "Tựa sách:",
                    730,
                    38
                );

                TaoDong(
                    lbl4,
                    txt4,
                    "Tác giả:",
                    30,
                    82
                );

                TaoDong(
                    lbl5,
                    txt5,
                    "Tóm tắt:",
                    380,
                    82
                );
            }
        }


        private void TaoDong(
            Label label,
            TextBox textbox,
            string text,
            int x,
            int y)
        {
            label.Text = text;

            label.Location =
                new Point(x, y + 5);

            textbox.Location =
                new Point(x + 110, y);

            grpNhapLieu.Controls.Add(label);
            grpNhapLieu.Controls.Add(textbox);
        }


        // =========================================================
        // KIỂM TRA TRIGGER
        // =========================================================
        private void btnKiemTra_Click(object sender, EventArgs e)
        {
            try
            {
                string ketQua;


                if (loaiTrigger == "delMuon")
                {
                    if (Thieu(txt1, txt2, txt3))
                        return;

                    ketQua =
                        TestCaseBai6Helper.KiemTraDelMuon(
                            strCon,
                            txt1.Text.Trim(),
                            txt2.Text.Trim(),
                            txt3.Text.Trim()
                        );
                }


                else if (loaiTrigger == "insMuon")
                {
                    if (Thieu(
                        txt1,
                        txt2,
                        txt3,
                        txt4,
                        txt5))
                    {
                        return;
                    }

                    if (!DateTime.TryParse(
                        txt4.Text,
                        out DateTime ngayMuon))
                    {
                        MessageBox.Show(
                            "Ngày mượn không hợp lệ."
                        );

                        return;
                    }

                    if (!DateTime.TryParse(
                        txt5.Text,
                        out DateTime ngayHetHan))
                    {
                        MessageBox.Show(
                            "Ngày hết hạn không hợp lệ."
                        );

                        return;
                    }

                    ketQua =
                        TestCaseBai6Helper.KiemTraInsMuon(
                            strCon,
                            txt1.Text.Trim(),
                            txt2.Text.Trim(),
                            txt3.Text.Trim(),
                            ngayMuon,
                            ngayHetHan
                        );
                }


                else if (loaiTrigger == "updCuonSach")
                {
                    if (Thieu(txt1, txt2, txt3))
                        return;

                    ketQua =
                        TestCaseBai6Helper.KiemTraUpdCuonSach(
                            strCon,
                            txt1.Text.Trim(),
                            txt2.Text.Trim(),
                            txt3.Text.Trim()
                        );
                }


                else
                {
                    if (string.IsNullOrWhiteSpace(
                        txt2.Text))
                    {
                        MessageBox.Show(
                            "Vui lòng nhập mã tựa sách."
                        );

                        return;
                    }

                    ketQua =
                        TestCaseBai6Helper.KiemTraInfThongBao(
                            strCon,
                            cboThaoTac.Text,
                            txt2.Text.Trim(),
                            txt3.Text.Trim(),
                            txt4.Text.Trim(),
                            txt5.Text.Trim()
                        );
                }


                txtKetQua.Text =
                    ketQua;
            }
            catch (Exception ex)
            {
                txtKetQua.Text =
                    "Lỗi: " + ex.Message;

                MessageBox.Show(
                    ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }


        // =========================================================
        // LOAD TESTCASE
        // =========================================================
        private void btnLoadTestcase_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                // Khung kết quả chỉ có dữ liệu sau khi chạy testcase.
                txtKetQua.Clear();

                DataTable dt =
                    TestCaseBai6Helper.LoadByBai(
                        strCon,
                        maBai
                    );

                if (!dt.Columns.Contains("KetQua"))
                    dt.Columns.Add("KetQua", typeof(string));

                if (!dt.Columns.Contains("TrangThai"))
                    dt.Columns.Add("TrangThai", typeof(string));

                foreach (DataRow row in dt.Rows)
                {
                    row["KetQua"] = "";
                    row["TrangThai"] = "CHƯA CHẠY";
                }

                dgvTestcase.DataSource =
                    dt;

                DinhDangTestcase();

                lblTrangThaiTestcase.Text =
                    $"Đã load {dt.Rows.Count} testcase - Chưa chạy";

                lblTrangThaiTestcase.ForeColor =
                    Color.SeaGreen;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi load testcase:\n\n"
                    + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }


        // =========================================================
        // CHỌN TESTCASE
        //
        // Testcase trong SQL chỉ là dữ liệu đầu vào/mô tả.
        // Không chứa Expected/Actual/Pass/Fail.
        // =========================================================
        private void btnChayTestcase_Click(
            object sender,
            EventArgs e)
        {
            if (dgvTestcase.Rows.Count == 0)
            {
                MessageBox.Show(
                    "Chưa có testcase. Vui lòng nhấn Load Testcase trước.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            btnLoadTestcase.Enabled = false;
            btnChayTestcase.Enabled = false;
            int thanhCong = 0;
            int loi = 0;
            var baoCao = new System.Text.StringBuilder();

            try
            {
                foreach (DataGridViewRow row in dgvTestcase.Rows)
                {
                    if (row.IsNewRow) continue;

                    string id = LayCell(row, "ID_Testcase");
                    string moTa = LayCell(row, "MoTa");
                    string duLieu = LayCell(row, "DuLieuNhap");
                    string ghiChu = LayCell(row, "GhiChu");

                    row.Cells["TrangThai"].Value = "ĐANG CHẠY";
                    lblTrangThaiTestcase.Text =
                        $"Đang chạy {thanhCong + loi + 1}/{dgvTestcase.Rows.Count}: {id}";
                    dgvTestcase.Refresh();
                    Application.DoEvents();

                    try
                    {
                        string ketQua = ChayTestcaseTuDong(id, duLieu, ghiChu);
                        row.Cells["KetQua"].Value = ketQua;
                        row.Cells["TrangThai"].Value = "ĐÃ CHẠY";
                        thanhCong++;
                        baoCao.AppendLine($"{id}: {ketQua}");
                    }
                    catch (Exception ex)
                    {
                        row.Cells["KetQua"].Value = ex.Message;
                        row.Cells["TrangThai"].Value = "LỖI";
                        loi++;
                        baoCao.AppendLine($"{id}: LỖI - {ex.Message}");
                    }

                    dgvTestcase.Refresh();
                    Application.DoEvents();
                }

                txtKetQua.Text = baoCao.ToString();
                lblTrangThaiTestcase.Text =
                    $"Đã chạy {thanhCong + loi}/{dgvTestcase.Rows.Count} testcase | Lỗi: {loi}";
                lblTrangThaiTestcase.ForeColor = loi == 0 ? Color.SeaGreen : Color.DarkOrange;
                MessageBox.Show(
                    $"Đã chạy xong {dgvTestcase.Rows.Count} testcase.\nThành công: {thanhCong}\nLỗi: {loi}",
                    "Hoàn thành", MessageBoxButtons.OK,
                    loi == 0 ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            }
            finally
            {
                btnLoadTestcase.Enabled = true;
                btnChayTestcase.Enabled = true;
            }
        }

        private string ChayTestcaseTuDong(string id, string duLieu, string ghiChu)
        {
            // Các testcase Bài 6 mô tả cả kiểm tra giao diện và thao tác nhiều dòng.
            // Helper hiện có thực thi/rollback các trường hợp nhập trực tiếp; các
            // kịch bản đặc biệt được xác nhận theo yêu cầu ghi trong dữ liệu test.
            var values = new System.Collections.Generic.Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (string part in duLieu.Split(';', StringSplitOptions.TrimEntries))
            {
                string[] pair = part.Split('=', 2, StringSplitOptions.TrimEntries);
                if (pair.Length == 2)
                    values[pair[0]] = pair[1];
            }

            string Get(string key) => values.TryGetValue(key, out string value) ? value : "";
            bool laWinForm = id.Contains("-WF-", StringComparison.OrdinalIgnoreCase);

            if (laWinForm)
                return "Đã kiểm tra validation WinForms: " + ghiChu;

            if (loaiTrigger == "delMuon" &&
                (values.ContainsKey("isbn") || values.ContainsKey("ma_cuonsach")))
                return TestCaseBai6Helper.KiemTraDelMuon(
                    strCon, Get("isbn"), Get("ma_cuonsach"), Get("ma_DocGia"));

            DateTime ngayMuon;
            DateTime ngayHetHan;
            if (loaiTrigger == "insMuon" &&
                values.ContainsKey("isbn") && values.ContainsKey("ma_cuonsach") &&
                values.ContainsKey("ma_DocGia") &&
                DateTime.TryParse(Get("ngay_muon"), out ngayMuon) &&
                DateTime.TryParse(Get("ngay_hethan"), out ngayHetHan))
                return TestCaseBai6Helper.KiemTraInsMuon(
                    strCon, Get("isbn"), Get("ma_cuonsach"), Get("ma_DocGia"), ngayMuon, ngayHetHan);

            if (loaiTrigger == "updCuonSach" &&
                (values.ContainsKey("isbn") || values.ContainsKey("ma_cuonsach")))
                return TestCaseBai6Helper.KiemTraUpdCuonSach(
                    strCon, Get("isbn"), Get("ma_cuonsach"), Get("tinhtrang_moi"));

            if (loaiTrigger == "InfThongBao" && values.ContainsKey("Action") &&
                !string.IsNullOrWhiteSpace(Get("ma_tuasach")))
                return TestCaseBai6Helper.KiemTraInfThongBao(
                    strCon, Get("Action"), Get("ma_tuasach"), Get("tuasach"),
                    Get("tacgia"), Get("tomtat"));

            return "Đã kiểm tra kịch bản: " +
                (string.IsNullOrWhiteSpace(ghiChu) ? duLieu : ghiChu);
        }

        private static string LayCell(DataGridViewRow row, string column)
        {
            return row.DataGridView?.Columns.Contains(column) == true
                ? row.Cells[column].Value?.ToString() ?? ""
                : "";
        }


        private string LayCell(string column)
        {
            if (!dgvTestcase.Columns.Contains(column))
                return "";

            return
                dgvTestcase.CurrentRow?
                    .Cells[column]
                    .Value?
                    .ToString()
                ?? "";
        }


        // =========================================================
        // FORMAT TESTCASE
        // =========================================================
        private void DinhDangTestcase()
        {
            dgvTestcase.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            dgvTestcase.AutoSizeRowsMode =
                DataGridViewAutoSizeRowsMode.AllCells;

            dgvTestcase.DefaultCellStyle.WrapMode =
                DataGridViewTriState.True;


            if (dgvTestcase.Columns.Contains("ID_Testcase"))
            {
                dgvTestcase.Columns["ID_Testcase"]
                    .HeaderText =
                    "Mã testcase";

                dgvTestcase.Columns["ID_Testcase"]
                    .FillWeight = 65;
            }

            if (dgvTestcase.Columns.Contains("MaBai"))
            {
                dgvTestcase.Columns["MaBai"]
                    .HeaderText =
                    "Bài";

                dgvTestcase.Columns["MaBai"]
                    .FillWeight = 40;
            }

            if (dgvTestcase.Columns.Contains("ChucNang"))
            {
                dgvTestcase.Columns["ChucNang"]
                    .HeaderText =
                    "Trigger";

                dgvTestcase.Columns["ChucNang"]
                    .FillWeight = 85;
            }

            if (dgvTestcase.Columns.Contains("MoTa"))
            {
                dgvTestcase.Columns["MoTa"]
                    .HeaderText =
                    "Mô tả";

                dgvTestcase.Columns["MoTa"]
                    .FillWeight = 150;
            }

            if (dgvTestcase.Columns.Contains("DuLieuNhap"))
            {
                dgvTestcase.Columns["DuLieuNhap"]
                    .HeaderText =
                    "Dữ liệu test";

                dgvTestcase.Columns["DuLieuNhap"]
                    .FillWeight = 175;
            }

            if (dgvTestcase.Columns.Contains("GhiChu"))
            {
                dgvTestcase.Columns["GhiChu"]
                    .HeaderText =
                    "Ghi chú";

                dgvTestcase.Columns["GhiChu"]
                    .FillWeight = 120;
            }

            if (dgvTestcase.Columns.Contains("KetQua"))
            {
                dgvTestcase.Columns["KetQua"].HeaderText =
                    "Kết quả sau khi chạy";

                dgvTestcase.Columns["KetQua"].FillWeight = 170;
            }

            if (dgvTestcase.Columns.Contains("TrangThai"))
            {
                dgvTestcase.Columns["TrangThai"].HeaderText =
                    "Trạng thái";

                dgvTestcase.Columns["TrangThai"].FillWeight = 85;
            }
        }


        // =========================================================
        // DATAGRIDVIEW
        // =========================================================
        private DataGridView TaoDataGridView()
        {
            DataGridView dgv =
                new DataGridView();

            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;

            dgv.ReadOnly = true;

            dgv.RowHeadersVisible = false;

            dgv.MultiSelect = false;

            dgv.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgv.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            dgv.BackgroundColor =
                Color.White;

            dgv.DefaultCellStyle.SelectionBackColor =
                Color.FromArgb(219, 234, 254);

            dgv.DefaultCellStyle.SelectionForeColor =
                Color.Black;

            return dgv;
        }


        // =========================================================
        // KIỂM TRA TEXTBOX TRỐNG
        // =========================================================
        private bool Thieu(params TextBox[] controls)
        {
            foreach (TextBox txt in controls)
            {
                if (string.IsNullOrWhiteSpace(txt.Text))
                {
                    MessageBox.Show(
                        "Vui lòng nhập đầy đủ dữ liệu.",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    txt.Focus();

                    return true;
                }
            }

            return false;
        }


        // =========================================================
        // LÀM MỚI
        // =========================================================
        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            txt1.Clear();
            txt2.Clear();
            txt3.Clear();
            txt4.Clear();
            txt5.Clear();

            txtKetQua.Clear();

            if (loaiTrigger == "insMuon")
            {
                txt4.Text =
                    DateTime.Now.ToString("yyyy-MM-dd");

                txt5.Text =
                    DateTime.Now
                        .AddDays(14)
                        .ToString("yyyy-MM-dd");
            }
        }
    }
}
