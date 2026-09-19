using System.Drawing;

namespace Giao_Dien_Chinh;

public sealed class MainForm : Form
{
    private readonly Label _statusLabel;
    private static readonly (string No, string Title, string Type, Color Color, Func<Form> Open)[] Exercises =
    {
        ("01", "Giải phương trình bậc nhất", "STORED PROCEDURE  •  QL_ĐỀ ÁN", Color.FromArgb(37,99,235), () => new global::Bài_1.Form1()),
        ("02", "Giải phương trình bậc hai", "STORED PROCEDURE  •  QL_ĐỀ ÁN", Color.FromArgb(14,165,233), () => new global::Bài_2.Form1()),
        ("03", "Thông tin đầu sách", "STORED PROCEDURE  •  QL_THƯ VIỆN", Color.FromArgb(16,185,129), () => new global::SoLuongSachChuaMuon.Form1()),
        ("04", "Tính tuổi nhân viên", "STORED PROCEDURE  •  QL_ĐỀ ÁN", Color.FromArgb(245,158,11), () => new global::TinhTuoi.Form1()),
        ("05", "Thông tin thư viện", "STORED PROCEDURE  •  TESTCASE", Color.FromArgb(139,92,246), () => new global::ThongTinThuVien.Form1()),
        ("06", "Trigger thư viện", "TRIGGER  •  TESTCASE", Color.FromArgb(236,72,153), () => new global::Bai_06_Trigger_ThuVien.Form1()),
        ("07", "Function đề án", "FUNCTION  •  QL_ĐỀ ÁN", Color.FromArgb(99,102,241), () => new global::Bai_07_Function_DeAn.Form1()),
        ("08", "Thống kê đề án", "FUNCTION  •  QL_ĐỀ ÁN", Color.FromArgb(8,145,178), () => new global::Bai_08_Function_DeAn.Form1()),
        ("09", "Quản lý sửa chữa gara", "FUNCTION  •  RÀNG BUỘC", Color.FromArgb(217,119,6), () => new global::Bai_09_QuanLyGara.Form1()),
        ("10", "Lịch thi trường phổ thông", "FUNCTION  •  TRIGGER", Color.FromArgb(190,24,93), () => new global::Bai_10_TruongPhoThong.Form1())
    };

    public MainForm()
    {
        Text = "Đồ án cuối kỳ Cơ sở dữ liệu";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(1000, 780);
        Size = new Size(1160, 900);
        BackColor = Color.FromArgb(245, 247, 251);
        Font = new Font("Segoe UI", 10F);
        Controls.Add(BuildGrid());

        _statusLabel = new Label
        {
            Text = "●  Sẵn sàng — chọn một bài tập để bắt đầu", Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft, ForeColor = Color.FromArgb(71,85,105),
            Font = new Font("Segoe UI", 9.5F), Padding = new Padding(32,0,0,0)
        };
        var footer = new Panel { Dock = DockStyle.Bottom, Height = 50, BackColor = Color.White };
        footer.Controls.Add(_statusLabel);
        Controls.Add(footer);
        Controls.Add(BuildHeader());
    }

    private static Control BuildHeader()
    {
        var p = new Panel { Dock = DockStyle.Top, Height = 142, BackColor = Color.FromArgb(15,23,42) };
        p.Controls.Add(new Label { Text = "DATABASE MANAGEMENT", AutoSize = true, Location = new Point(37,22), Font = new Font("Segoe UI Semibold",9F,FontStyle.Bold), ForeColor = Color.FromArgb(96,165,250) });
        p.Controls.Add(new Label { Text = "ĐỒ ÁN CUỐI KỲ CƠ SỞ DỮ LIỆU", AutoSize = true, Location = new Point(32,44), Font = new Font("Segoe UI Semibold",24F,FontStyle.Bold), ForeColor = Color.White });
        p.Controls.Add(new Label { Text = "Chọn bài tập bên dưới để mở chương trình và kiểm thử dữ liệu", AutoSize = true, Location = new Point(37,101), Font = new Font("Segoe UI",10.5F), ForeColor = Color.FromArgb(203,213,225) });
        return p;
    }

    private Control BuildGrid()
    {
        var grid = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 5, Padding = new Padding(26,22,26,22), BackColor = BackColor };
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        for (int i = 0; i < 5; i++) grid.RowStyles.Add(new RowStyle(SizeType.Percent, 20));
        for (int i = 0; i < Exercises.Length; i++)
        {
            var ex = Exercises[i];
            var card = CreateCard(ex);
            grid.Controls.Add(card, i % 2, i / 2);
        }
        return grid;
    }

    private Control CreateCard((string No, string Title, string Type, Color Color, Func<Form> Open) ex)
    {
        var card = new Panel { Dock = DockStyle.Fill, Margin = new Padding(9), BackColor = Color.White, Cursor = Cursors.Hand, Tag = ex, Padding = new Padding(1) };
        var accent = new Panel { Dock = DockStyle.Left, Width = 6, BackColor = ex.Color };
        var badge = new Label { Text = ex.No, Size = new Size(54,54), Location = new Point(25,21), TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Segoe UI Semibold",14F,FontStyle.Bold), ForeColor = Color.White, BackColor = ex.Color };
        var title = new Label { Text = ex.Title, AutoSize = true, Location = new Point(98,20), Font = new Font("Segoe UI Semibold",12F,FontStyle.Bold), ForeColor = Color.FromArgb(15,23,42), BackColor = Color.Transparent };
        var type = new Label { Text = ex.Type, AutoSize = true, Location = new Point(99,55), Font = new Font("Segoe UI Semibold",8.2F), ForeColor = Color.FromArgb(100,116,139), BackColor = Color.Transparent };
        var arrow = new Label { Text = "›", AutoSize = true, Anchor = AnchorStyles.Top | AnchorStyles.Right, Location = new Point(card.Width - 40,28), Font = new Font("Segoe UI",21F), ForeColor = ex.Color, BackColor = Color.Transparent };
        card.Controls.AddRange(new Control[] { accent, badge, title, type, arrow });
        card.Resize += (_,_) => arrow.Location = new Point(card.Width - 42, 27);
        EventHandler click = (_,_) => OpenExercise(card);
        card.Click += click;
        foreach (Control c in card.Controls) { c.Cursor = Cursors.Hand; c.Click += click; }
        EventHandler enter = (_,_) => card.BackColor = Color.FromArgb(239,246,255);
        EventHandler leave = (_,_) => card.BackColor = Color.White;
        card.MouseEnter += enter; card.MouseLeave += leave;
        foreach (Control c in card.Controls) { c.MouseEnter += enter; c.MouseLeave += leave; }
        card.Paint += (_,e) => { using var pen = new Pen(Color.FromArgb(226,232,240)); e.Graphics.DrawRectangle(pen,0,0,card.Width-1,card.Height-1); };
        return card;
    }

    private void OpenExercise(Control card)
    {
        if (card.Tag is not ValueTuple<string,string,string,Color,Func<Form>> ex) return;
        try
        {
            _statusLabel.Text = $"●  Đang mở Bài {ex.Item1}: {ex.Item2}...";
            using var form = ex.Item5();
            form.StartPosition = FormStartPosition.CenterParent;
            form.ShowDialog(this);
            _statusLabel.Text = $"●  Đã đóng Bài {ex.Item1} — sẵn sàng chọn bài khác";
        }
        catch (Exception error)
        {
            _statusLabel.Text = $"●  Không thể mở Bài {ex.Item1}";
            MessageBox.Show($"Không thể mở Bài {ex.Item1}.\n\nChi tiết: {error.Message}", "Lỗi mở bài tập", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
