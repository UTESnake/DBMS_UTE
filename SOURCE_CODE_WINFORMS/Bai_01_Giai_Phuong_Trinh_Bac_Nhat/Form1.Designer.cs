namespace Bài_1
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (
                disposing &&
                components != null
            )
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            pnlHeader = new Panel();
            lblTitle = new Label();

            grpInput = new GroupBox();
            lblA = new Label();
            txtA = new TextBox();
            lblB = new Label();
            txtB = new TextBox();

            btnGiai = new Button();
            btnLamMoi = new Button();

            btnLoadTestcase = new Button();
            btnTestcase = new Button();

            lblThongKe = new Label();

            dgvTestcase = new DataGridView();

            grpOutput = new GroupBox();
            txtKetQua = new TextBox();

            pnlHeader.SuspendLayout();
            grpInput.SuspendLayout();

            ((System.ComponentModel.ISupportInitialize)dgvTestcase)
                .BeginInit();

            grpOutput.SuspendLayout();

            SuspendLayout();

            // =================================================
            // pnlHeader
            // =================================================
            pnlHeader.BackColor =
                Color.FromArgb(0, 122, 204);

            pnlHeader.Controls.Add(lblTitle);

            pnlHeader.Dock =
                DockStyle.Top;

            pnlHeader.Location =
                new Point(0, 0);

            pnlHeader.Name =
                "pnlHeader";

            pnlHeader.Size =
                new Size(1050, 85);

            pnlHeader.TabIndex =
                0;

            // =================================================
            // lblTitle
            // =================================================
            lblTitle.AutoSize = true;

            lblTitle.Font =
                new Font(
                    "Segoe UI",
                    15F,
                    FontStyle.Bold
                );

            lblTitle.ForeColor =
                Color.White;

            lblTitle.Location =
                new Point(30, 24);

            lblTitle.Name =
                "lblTitle";

            lblTitle.Text =
                "GIẢI PHƯƠNG TRÌNH BẬC NHẤT (ax + b = 0)";

            // =================================================
            // grpInput
            // =================================================
            grpInput.Controls.Add(lblA);
            grpInput.Controls.Add(txtA);

            grpInput.Controls.Add(lblB);
            grpInput.Controls.Add(txtB);

            grpInput.Font =
                new Font(
                    "Segoe UI Semibold",
                    10.2F,
                    FontStyle.Bold
                );

            grpInput.Location =
                new Point(35, 105);

            grpInput.Name =
                "grpInput";

            grpInput.Size =
                new Size(980, 135);

            grpInput.TabIndex =
                1;

            grpInput.TabStop =
                false;

            grpInput.Text =
                "Nhập tham số";

            // =================================================
            // lblA
            // =================================================
            lblA.AutoSize =
                true;

            lblA.Font =
                new Font(
                    "Segoe UI",
                    10F
                );

            lblA.Location =
                new Point(60, 42);

            lblA.Name =
                "lblA";

            lblA.Text =
                "Hệ số a:";

            // =================================================
            // txtA
            // =================================================
            txtA.Font =
                new Font(
                    "Segoe UI",
                    11F
                );

            txtA.Location =
                new Point(150, 37);

            txtA.Name =
                "txtA";

            txtA.PlaceholderText =
                "Nhập số a...";

            txtA.Size =
                new Size(760, 32);

            // =================================================
            // lblB
            // =================================================
            lblB.AutoSize =
                true;

            lblB.Font =
                new Font(
                    "Segoe UI",
                    10F
                );

            lblB.Location =
                new Point(60, 88);

            lblB.Name =
                "lblB";

            lblB.Text =
                "Hệ số b:";

            // =================================================
            // txtB
            // =================================================
            txtB.Font =
                new Font(
                    "Segoe UI",
                    11F
                );

            txtB.Location =
                new Point(150, 82);

            txtB.Name =
                "txtB";

            txtB.PlaceholderText =
                "Nhập số b...";

            txtB.Size =
                new Size(760, 32);

            // =================================================
            // btnGiai
            // =================================================
            btnGiai.BackColor =
                Color.FromArgb(40, 167, 69);

            btnGiai.Cursor =
                Cursors.Hand;

            btnGiai.FlatAppearance.BorderSize =
                0;

            btnGiai.FlatStyle =
                FlatStyle.Flat;

            btnGiai.Font =
                new Font(
                    "Segoe UI",
                    10.5F,
                    FontStyle.Bold
                );

            btnGiai.ForeColor =
                Color.White;

            btnGiai.Location =
                new Point(175, 260);

            btnGiai.Name =
                "btnGiai";

            btnGiai.Size =
                new Size(220, 45);

            btnGiai.Text =
                "⚡ Giải Phương Trình";

            btnGiai.UseVisualStyleBackColor =
                false;

            btnGiai.Click +=
                btnGiai_Click;

            // =================================================
            // btnLamMoi
            // =================================================
            btnLamMoi.BackColor =
                Color.FromArgb(108, 117, 125);

            btnLamMoi.Cursor =
                Cursors.Hand;

            btnLamMoi.FlatAppearance.BorderSize =
                0;

            btnLamMoi.FlatStyle =
                FlatStyle.Flat;

            btnLamMoi.Font =
                new Font(
                    "Segoe UI",
                    10.5F,
                    FontStyle.Bold
                );

            btnLamMoi.ForeColor =
                Color.White;

            btnLamMoi.Location =
                new Point(410, 260);

            btnLamMoi.Name =
                "btnLamMoi";

            btnLamMoi.Size =
                new Size(160, 45);

            btnLamMoi.Text =
                "🔄 Làm Mới";

            btnLamMoi.UseVisualStyleBackColor =
                false;

            btnLamMoi.Click +=
                btnLamMoi_Click;

            // =================================================
            // btnLoadTestcase
            // =================================================
            btnLoadTestcase.BackColor =
                Color.FromArgb(0, 123, 255);

            btnLoadTestcase.Cursor =
                Cursors.Hand;

            btnLoadTestcase.FlatAppearance.BorderSize =
                0;

            btnLoadTestcase.FlatStyle =
                FlatStyle.Flat;

            btnLoadTestcase.Font =
                new Font(
                    "Segoe UI",
                    10.5F,
                    FontStyle.Bold
                );

            btnLoadTestcase.ForeColor =
                Color.White;

            btnLoadTestcase.Location =
                new Point(585, 260);

            btnLoadTestcase.Name =
                "btnLoadTestcase";

            btnLoadTestcase.Size =
                new Size(190, 45);

            btnLoadTestcase.Text =
                "📋 Load Testcase";

            btnLoadTestcase.UseVisualStyleBackColor =
                false;

            btnLoadTestcase.Click +=
                btnLoadTestcase_Click;

            // =================================================
            // btnTestcase
            // =================================================
            btnTestcase.BackColor =
                Color.FromArgb(255, 153, 0);

            btnTestcase.Cursor =
                Cursors.Hand;

            btnTestcase.FlatAppearance.BorderSize =
                0;

            btnTestcase.FlatStyle =
                FlatStyle.Flat;

            btnTestcase.Font =
                new Font(
                    "Segoe UI",
                    10.5F,
                    FontStyle.Bold
                );

            btnTestcase.ForeColor =
                Color.White;

            btnTestcase.Location =
                new Point(790, 260);

            btnTestcase.Name =
                "btnTestcase";

            btnTestcase.Size =
                new Size(200, 45);

            btnTestcase.Text =
                "🧪 Chạy Testcase";

            btnTestcase.UseVisualStyleBackColor =
                false;

            btnTestcase.Click +=
                btnTestcase_Click;

            // =================================================
            // lblThongKe
            // =================================================
            lblThongKe.AutoSize =
                true;

            lblThongKe.Font =
                new Font(
                    "Segoe UI",
                    10F,
                    FontStyle.Bold
                );

            lblThongKe.ForeColor =
                Color.FromArgb(64, 64, 64);

            lblThongKe.Location =
                new Point(35, 325);

            lblThongKe.Name =
                "lblThongKe";

            lblThongKe.Text =
                "Chưa load testcase";

            // =================================================
            // dgvTestcase
            // =================================================
            dgvTestcase.AllowUserToAddRows =
                false;

            dgvTestcase.AllowUserToDeleteRows =
                false;

            dgvTestcase.AllowUserToResizeRows =
                false;

            dgvTestcase.BackgroundColor =
                Color.White;

            dgvTestcase.BorderStyle =
                BorderStyle.Fixed3D;

            dgvTestcase.ColumnHeadersHeight =
                40;

            dgvTestcase.ColumnHeadersHeightSizeMode =
                DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            dgvTestcase.Location =
                new Point(35, 360);

            dgvTestcase.Name =
                "dgvTestcase";

            dgvTestcase.ReadOnly =
                true;

            dgvTestcase.RowHeadersVisible =
                false;

            dgvTestcase.RowHeadersWidth =
                51;

            dgvTestcase.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgvTestcase.Size =
                new Size(980, 310);

            dgvTestcase.TabIndex =
                7;

            // =================================================
            // grpOutput
            // =================================================
            grpOutput.Controls.Add(txtKetQua);

            grpOutput.Font =
                new Font(
                    "Segoe UI Semibold",
                    10.2F,
                    FontStyle.Bold
                );

            grpOutput.Location =
                new Point(35, 690);

            grpOutput.Name =
                "grpOutput";

            grpOutput.Size =
                new Size(980, 100);

            grpOutput.TabIndex =
                8;

            grpOutput.TabStop =
                false;

            grpOutput.Text =
                "Kết quả thực thi từ SQL Server";

            // =================================================
            // txtKetQua
            // =================================================
            txtKetQua.BackColor =
                Color.FromArgb(248, 249, 250);

            txtKetQua.Font =
                new Font(
                    "Segoe UI",
                    11F,
                    FontStyle.Bold
                );

            txtKetQua.ForeColor =
                Color.FromArgb(192, 0, 0);

            txtKetQua.Location =
                new Point(20, 40);

            txtKetQua.Name =
                "txtKetQua";

            txtKetQua.ReadOnly =
                true;

            txtKetQua.Size =
                new Size(940, 32);

            txtKetQua.TextAlign =
                HorizontalAlignment.Center;

            // =================================================
            // Form1
            // =================================================
            AutoScaleDimensions =
                new SizeF(8F, 20F);

            AutoScaleMode =
                AutoScaleMode.Font;

            BackColor =
                Color.White;

            ClientSize =
                new Size(1050, 820);

            Controls.Add(grpOutput);
            Controls.Add(dgvTestcase);
            Controls.Add(lblThongKe);

            Controls.Add(btnTestcase);
            Controls.Add(btnLoadTestcase);
            Controls.Add(btnLamMoi);
            Controls.Add(btnGiai);

            Controls.Add(grpInput);
            Controls.Add(pnlHeader);

            FormBorderStyle =
                FormBorderStyle.FixedDialog;

            MaximizeBox =
                false;

            Name =
                "Form1";

            StartPosition =
                FormStartPosition.CenterScreen;

            Text =
                "Bài 1 - Giải phương trình bậc nhất";

            Load +=
                Form1_Load;

            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();

            grpInput.ResumeLayout(false);
            grpInput.PerformLayout();

            ((System.ComponentModel.ISupportInitialize)dgvTestcase)
                .EndInit();

            grpOutput.ResumeLayout(false);
            grpOutput.PerformLayout();

            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel pnlHeader;
        private Label lblTitle;

        private GroupBox grpInput;

        private Label lblA;
        private TextBox txtA;

        private Label lblB;
        private TextBox txtB;

        private Button btnGiai;
        private Button btnLamMoi;

        private Button btnLoadTestcase;
        private Button btnTestcase;

        private Label lblThongKe;

        private DataGridView dgvTestcase;

        private GroupBox grpOutput;
        private TextBox txtKetQua;
    }
}