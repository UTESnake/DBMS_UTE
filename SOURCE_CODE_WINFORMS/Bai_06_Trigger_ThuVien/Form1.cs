using System;
using System.Collections.Generic;
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
        private ComboBox cboTinhTrangMoi;

        private Button btnKiemTra;
        private Button btnLamMoi;

        private GroupBox grpKetQua;
        private TextBox txtKetQua;
        private readonly ComboBox cboKyVong = new ComboBox();

        private Button btnLoadCsdl;
        private ComboBox cboBangDuLieu;
        private Label lblTrangThaiDuLieu;
        private GroupBox grpDuLieu;
        private DataGridView dgvDuLieu;
        private Dictionary<string, DataTable> banXemTruoc;


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

            cboTinhTrangMoi =
                new ComboBox();

            cboTinhTrangMoi.DropDownStyle =
                ComboBoxStyle.DropDownList;

            cboTinhTrangMoi.Font =
                new Font(
                    "Segoe UI",
                    10F
                );

            cboTinhTrangMoi.Size =
                new Size(200, 30);


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

            if (loaiTrigger == "insMuon" || loaiTrigger == "InfThongBao")
            {
                var label = new Label { Text = "Kỳ vọng:", Location = new Point(30, 130), AutoSize = true };
                cboKyVong.DropDownStyle = ComboBoxStyle.DropDownList;
                cboKyVong.Location = new Point(140, 127);
                cboKyVong.Size = new Size(540, 30);
                cboKyVong.Items.Add("Thao tác hợp lệ (không dự kiến lỗi)");
                if (loaiTrigger == "insMuon")
                    cboKyVong.Items.AddRange(new object[] { "FK_Muon_Cuonsach", "FK_Muon_DocGia", "PK_Muon", "UQ_Muon_CuonDangMuon", "CK_Muon_ThoiHan" });
                else cboKyVong.Items.Add("PK_Tuasach");
                cboKyVong.SelectedIndex = 0;
                grpNhapLieu.Controls.Add(label);
                grpNhapLieu.Controls.Add(cboKyVong);
            }



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
                new Size(1110, 180);

            Controls.Add(grpKetQua);


            txtKetQua =
                new TextBox();

            txtKetQua.Multiline =
                true;

            txtKetQua.ReadOnly =
                true;

            txtKetQua.ScrollBars =
                ScrollBars.None;

            txtKetQua.WordWrap =
                true;

            txtKetQua.Font =
                new Font(
                    "Segoe UI",
                    10F
                );

            txtKetQua.Location =
                new Point(15, 30);

            txtKetQua.Size =
                new Size(1080, 130);

            grpKetQua.Controls.Add(txtKetQua);


            // =====================================================
            // LOAD DỮ LIỆU CSDL
            // =====================================================
            btnLoadCsdl =
                new Button();

            btnLoadCsdl.Text =
                "▣  Load CSDL";

            btnLoadCsdl.Size =
                new Size(190, 44);

            btnLoadCsdl.Location =
                new Point(35, 505);

            btnLoadCsdl.BackColor =
                Color.FromArgb(37, 99, 235);

            btnLoadCsdl.ForeColor =
                Color.White;

            btnLoadCsdl.FlatStyle =
                FlatStyle.Flat;

            btnLoadCsdl.FlatAppearance.BorderSize =
                0;

            btnLoadCsdl.Font =
                new Font(
                    "Segoe UI",
                    10F,
                    FontStyle.Bold
                );

            btnLoadCsdl.Click +=
                btnLoadCsdl_Click;

            Controls.Add(btnLoadCsdl);


            // =====================================================
            // CHỌN BẢNG LIÊN QUAN TỚI TRIGGER
            // =====================================================
            cboBangDuLieu = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10F),
                Location = new Point(240, 511),
                Size = new Size(220, 30)
            };

            if (loaiTrigger == "delMuon" || loaiTrigger == "insMuon")
                cboBangDuLieu.Items.AddRange(new object[] { "Muon", "Cuonsach", "DocGia" });
            else if (loaiTrigger == "updCuonSach")
                cboBangDuLieu.Items.AddRange(new object[] { "Cuonsach", "Dausach" });
            else
                cboBangDuLieu.Items.Add("Tuasach");

            cboBangDuLieu.SelectedIndex = 0;
            cboBangDuLieu.SelectedIndexChanged += btnLoadCsdl_Click;
            Controls.Add(cboBangDuLieu);


            // =====================================================
            // TRẠNG THÁI DỮ LIỆU
            // =====================================================
            lblTrangThaiDuLieu =
                new Label();

            lblTrangThaiDuLieu.AutoSize =
                true;

            lblTrangThaiDuLieu.Font =
                new Font(
                    "Segoe UI",
                    10F,
                    FontStyle.Bold
                );

            lblTrangThaiDuLieu.ForeColor =
                Color.FromArgb(75, 85, 99);

            lblTrangThaiDuLieu.Location =
                new Point(490, 517);

            lblTrangThaiDuLieu.Text =
                "Chưa load dữ liệu CSDL";

            Controls.Add(lblTrangThaiDuLieu);


            // =====================================================
            // DỮ LIỆU CSDL
            // =====================================================
            grpDuLieu =
                new GroupBox();

            grpDuLieu.Font =
                new Font(
                    "Segoe UI",
                    10F,
                    FontStyle.Bold
                );

            grpDuLieu.Text =
                "Dữ liệu CSDL liên quan " + maBai;

            grpDuLieu.Location =
                new Point(35, 565);

            grpDuLieu.Size =
                new Size(1110, 215);

            Controls.Add(grpDuLieu);


            dgvDuLieu =
                TaoDataGridView();

            dgvDuLieu.Location =
                new Point(15, 30);

            dgvDuLieu.Size =
                new Size(1080, 165);

            grpDuLieu.Controls.Add(dgvDuLieu);
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

                lbl3.Text =
                    "Tình trạng mới:";

                lbl3.AutoSize = false;

                lbl3.Location =
                    new Point(730, 45);

                lbl3.Size =
                    new Size(130, 30);

                lbl3.TextAlign =
                    ContentAlignment.MiddleLeft;

                cboTinhTrangMoi.Location =
                    new Point(865, 45);

                cboTinhTrangMoi.Items.AddRange(
                    new object[]
                    {
                        "Có sẵn",
                        "Đang mượn"
                    }
                );

                cboTinhTrangMoi.SelectedIndex = -1;

                grpNhapLieu.Controls.Add(lbl3);
                grpNhapLieu.Controls.Add(cboTinhTrangMoi);
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
            HienThiKetQua("");
            banXemTruoc = null;
            dgvDuLieu.DataSource = null;
            lblTrangThaiDuLieu.Text = "Chưa load dữ liệu cho lần kiểm tra này.";
            string expectedConstraint = cboKyVong.SelectedIndex > 0 ? cboKyVong.Text : "";
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
                            txt3.Text.Trim(),
                            LuuBanXemTruoc
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
                            ngayHetHan,
                            LuuBanXemTruoc,
                            expectedConstraint
                        );
                }


                else if (loaiTrigger == "updCuonSach")
                {
                    if (Thieu(txt1, txt2))
                        return;

                    if (cboTinhTrangMoi.SelectedIndex < 0)
                    {
                        MessageBox.Show(
                            "Vui lòng chọn tình trạng mới.",
                            "Thông báo",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );

                        cboTinhTrangMoi.Focus();

                        return;
                    }

                    ketQua =
                        TestCaseBai6Helper.KiemTraUpdCuonSach(
                            strCon,
                            txt1.Text.Trim(),
                            txt2.Text.Trim(),
                            cboTinhTrangMoi.Text,
                            LuuBanXemTruoc
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
                            txt5.Text.Trim(),
                            LuuBanXemTruoc,
                            expectedConstraint
                        );
                }


                HienThiKetQua(ketQua);
            }
            catch (SqlException ex) when (DoAn.Shared.SqlFailureClassifier.IsLibraryConstraint(ex))
            {
                string message = string.IsNullOrEmpty(expectedConstraint)
                    ? "Dữ liệu đã bị ràng buộc chặn. Để kiểm thử âm, chọn ràng buộc ở ô Kỳ vọng rồi chạy lại."
                    : "Không đạt / FAIL: SQL báo ràng buộc khác với kỳ vọng " + expectedConstraint + ".";
                HienThiKetQua(message + Environment.NewLine + ex.Message);
                MessageBox.Show(message, "Kết quả kiểm tra dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                HienThiKetQua(
                    "Không đạt / FAIL — lỗi ngoài kỳ vọng: " + ex.Message
                );

                MessageBox.Show(
                    ex.Message,
                    "Không thể hoàn tất kiểm thử",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }


        // =========================================================
        // HIỂN THỊ KẾT QUẢ TỪ DÒNG ĐẦU TIÊN
        // Chỉ dùng thanh cuộn khi nội dung thực sự vượt quá khung.
        // =========================================================
        private void HienThiKetQua(string noiDung)
        {
            txtKetQua.Text = noiDung ?? "";

            int chieuRongDo =
                Math.Max(1, txtKetQua.ClientSize.Width - 10);

            Size kichThuocNoiDung =
                TextRenderer.MeasureText(
                    txtKetQua.Text,
                    txtKetQua.Font,
                    new Size(chieuRongDo, int.MaxValue),
                    TextFormatFlags.WordBreak |
                    TextFormatFlags.TextBoxControl
                );

            txtKetQua.ScrollBars =
                kichThuocNoiDung.Height >
                txtKetQua.ClientSize.Height - 6
                    ? ScrollBars.Vertical
                    : ScrollBars.None;

            txtKetQua.SelectionStart = 0;
            txtKetQua.SelectionLength = 0;
            txtKetQua.ScrollToCaret();
        }


        // =========================================================
        // LƯU TRẠNG THÁI SAU TRIGGER TRƯỚC KHI TRANSACTION ROLLBACK
        // =========================================================
        private void LuuBanXemTruoc(SqlConnection conn, SqlTransaction tran)
        {
            var duLieuMoi = new Dictionary<string, DataTable>();
            foreach (object item in cboBangDuLieu.Items)
            {
                string tenBang = item.ToString();
                duLieuMoi.Add(tenBang, DocBang(conn, tran, tenBang));
            }

            banXemTruoc = duLieuMoi;
        }


        private DataTable DocBang(SqlConnection conn, SqlTransaction tran, string tenBang)
        {
            string sql = tenBang switch
            {
                "Muon" when loaiTrigger == "delMuon" || loaiTrigger == "insMuon" =>
                    "SELECT * FROM dbo.Muon ORDER BY isbn, ma_cuonsach, ma_DocGia",
                "DocGia" when loaiTrigger == "delMuon" || loaiTrigger == "insMuon" =>
                    "SELECT * FROM dbo.DocGia ORDER BY ma_DocGia",
                "Cuonsach" when loaiTrigger != "InfThongBao" =>
                    "SELECT * FROM dbo.Cuonsach ORDER BY isbn, ma_cuonsach",
                "Dausach" when loaiTrigger == "updCuonSach" =>
                    "SELECT * FROM dbo.Dausach ORDER BY isbn",
                "Tuasach" when loaiTrigger == "InfThongBao" =>
                    "SELECT * FROM dbo.Tuasach ORDER BY ma_tuasach",
                _ => throw new InvalidOperationException("Bảng dữ liệu không thuộc bài này.")
            };

            using SqlCommand cmd = new SqlCommand(sql, conn, tran);
            using SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }


        // =========================================================
        // LOAD DỮ LIỆU GỐC HOẶC BẢN XEM TRƯỚC SAU TRIGGER
        // =========================================================
        private void btnLoadCsdl_Click(object sender, EventArgs e)
        {
            string tenBang = cboBangDuLieu.SelectedItem?.ToString() ?? "";

            try
            {
                DataTable dt;
                bool laBanXemTruoc = banXemTruoc != null && banXemTruoc.ContainsKey(tenBang);
                if (laBanXemTruoc)
                {
                    dt = banXemTruoc[tenBang].Copy();
                }
                else
                {
                    using SqlConnection conn = new SqlConnection(strCon);
                    conn.Open();
                    dt = DocBang(conn, null, tenBang);
                }

                dgvDuLieu.DataSource = dt;
                grpDuLieu.Text = $"Dữ liệu CSDL liên quan {maBai} - {tenBang}";
                lblTrangThaiDuLieu.Text = laBanXemTruoc
                    ? $"Bản xem trước sau Trigger: {dt.Rows.Count} dòng (đã ROLLBACK)"
                    : $"Đã load {dt.Rows.Count} dòng từ bảng {tenBang}";
                lblTrangThaiDuLieu.ForeColor = Color.SeaGreen;
            }
            catch (Exception ex)
            {
                dgvDuLieu.DataSource = null;
                lblTrangThaiDuLieu.Text = "Load dữ liệu CSDL thất bại";
                lblTrangThaiDuLieu.ForeColor = Color.Firebrick;
                MessageBox.Show(
                    "Không thể load bảng " + tenBang + ":\n\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
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

            HienThiKetQua("");
            if (cboKyVong.Items.Count > 0) cboKyVong.SelectedIndex = 0;
            banXemTruoc = null;
            dgvDuLieu.DataSource = null;
            lblTrangThaiDuLieu.Text = "Chưa load dữ liệu CSDL.";

            if (loaiTrigger == "insMuon")
            {
                txt4.Text =
                    DateTime.Now.ToString("yyyy-MM-dd");

                txt5.Text =
                    DateTime.Now
                        .AddDays(14)
                        .ToString("yyyy-MM-dd");
            }

            if (loaiTrigger == "updCuonSach")
            {
                cboTinhTrangMoi.SelectedIndex = -1;
            }
        }
    }
}
