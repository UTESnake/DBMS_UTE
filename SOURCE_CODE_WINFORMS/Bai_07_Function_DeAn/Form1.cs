using System.Data;
using System.Drawing;
using System.Globalization;
using Microsoft.Data.SqlClient;

namespace Bai_07_Function_DeAn;

public partial class Form1 : Form
{
    const string Cs=@"Data Source=.\SQLEXPRESS02;Initial Catalog=QL_DeAn;Integrated Security=True;Encrypt=False;TrustServerCertificate=True";
    readonly ComboBox cbo=new(); readonly TextBox p1=new(),p2=new(); readonly Label lp1=new(),lp2=new(),status=new();
    readonly DataGridView result=Grid(); readonly Button run=Button("▶  Thực hiện",Color.FromArgb(16,185,129));
    readonly ComboBox sourceTable=new(){DropDownStyle=ComboBoxStyle.DropDownList,Width=220};
    readonly DataGridView sourceData=Grid();
    readonly Label sourceStatus=new(){AutoSize=true,Margin=new Padding(20,12,0,0),Font=new Font("Segoe UI Semibold",10,FontStyle.Bold)};
    readonly Button loadSource=Button("▣  Load CSDL",Color.FromArgb(37,99,235));
    readonly GroupBox sourceGroup=new(){Dock=DockStyle.Fill,Font=new Font("Segoe UI Semibold",10,FontStyle.Bold),Padding=new Padding(12)};
    bool connected;
    static readonly Fn[] Fns={
        new("7.1","Lương trung bình một phòng","Mã phòng","","SELECT dbo.fn_B7_LuongTrungBinhPhong(@p1) LuongTrungBinh"),
        new("7.2","Tổng lương nhân viên theo đề án","Mã nhân viên","Mã đề án","SELECT dbo.fn_B7_TongLuongNhanVienDeAn(@p1,@p2) TongLuong"),
        new("7.3","Tổng lương trung bình các phòng","","","SELECT dbo.fn_B7_TongLuongTrungBinhCacPhong() TongLuongTrungBinh"),
        new("7.4","Tiền thưởng theo tổng giờ","Tổng số giờ","","SELECT dbo.fn_B7_TienThuong(@p1) TienThuong"),
        new("7.5","Số đề án theo mỗi phòng","","","SELECT * FROM dbo.fn_B7_SoDeAnTheoPhong() ORDER BY MaPB"),
        new("7.6a","Thông tin nhân viên - Inline TVF","","","SELECT * FROM dbo.fn_B7_ThongTinNhanVien_Inline() ORDER BY MaNV"),
        new("7.6b","Thông tin nhân viên - Multistatement TVF","","","SELECT * FROM dbo.fn_B7_ThongTinNhanVien_Multi() ORDER BY MaNV")};

    public Form1(){InitializeComponent();Build();}
    void Build(){
        Text="Bài 7 - Function CSDL Đề án";StartPosition=FormStartPosition.CenterParent;MinimumSize=new Size(1050,820);Size=new Size(1200,950);BackColor=Color.FromArgb(248,250,252);Font=new Font("Segoe UI",10);
        var head=new Panel{Dock=DockStyle.Top,Height=94,BackColor=Color.FromArgb(79,70,229)};head.Controls.Add(new Label{Text="FUNCTION CƠ SỞ DỮ LIỆU ĐỀ ÁN",Dock=DockStyle.Fill,TextAlign=ContentAlignment.MiddleCenter,Font=new Font("Segoe UI Semibold",20,FontStyle.Bold),ForeColor=Color.White});
        var con=new Panel{Dock=DockStyle.Top,Height=58,BackColor=Color.White,Padding=new Padding(28,8,28,8)};var bc=Button("⌁  Kết nối CSDL",Color.FromArgb(37,99,235));bc.Dock=DockStyle.Left;bc.Width=185;bc.Click+=Connect;status.Text="● Chưa kết nối QL_DeAn";status.Dock=DockStyle.Fill;status.Padding=new Padding(18,0,0,0);status.TextAlign=ContentAlignment.MiddleLeft;status.ForeColor=Color.Firebrick;con.Controls.Add(status);con.Controls.Add(bc);
        var body=new TableLayoutPanel{Dock=DockStyle.Fill,Padding=new Padding(28,18,28,18),RowCount=4,ColumnCount=1};body.RowStyles.Add(new RowStyle(SizeType.Absolute,130));body.RowStyles.Add(new RowStyle(SizeType.Percent,50));body.RowStyles.Add(new RowStyle(SizeType.Absolute,58));body.RowStyles.Add(new RowStyle(SizeType.Percent,50));
        var input=new GroupBox{Text="Chọn function và nhập tham số",Dock=DockStyle.Fill,Font=new Font("Segoe UI Semibold",10,FontStyle.Bold)};cbo.DropDownStyle=ComboBoxStyle.DropDownList;cbo.Location=new Point(25,43);cbo.Size=new Size(350,30);cbo.Items.AddRange(Fns.Select(x=>$"{x.Code} - {x.Title}").ToArray());lp1.Location=new Point(400,25);lp1.AutoSize=true;p1.Location=new Point(400,51);p1.Size=new Size(180,30);lp2.Location=new Point(605,25);lp2.AutoSize=true;p2.Location=new Point(605,51);p2.Size=new Size(180,30);run.Location=new Point(815,40);run.Size=new Size(190,43);run.Click+=Execute;
        p1.KeyDown+=(s,e)=>{if(e.KeyCode==Keys.Enter){e.SuppressKeyPress=true;run.PerformClick();}};
        p2.KeyDown+=(s,e)=>{if(e.KeyCode==Keys.Enter){e.SuppressKeyPress=true;run.PerformClick();}};
        sourceData.CellClick+=(_,e)=>{
            if(e.RowIndex<0)return;
            var row=sourceData.Rows[e.RowIndex];
            if(sourceData.Columns.Contains("MaPB") && p1.Visible) p1.Text=row.Cells["MaPB"].Value?.ToString()??"";
            else if(sourceData.Columns.Contains("MaNV") && p1.Visible) p1.Text=row.Cells["MaNV"].Value?.ToString()??"";
            if(sourceData.Columns.Contains("MaDA") && p2.Visible) p2.Text=row.Cells["MaDA"].Value?.ToString()??"";
        };
        input.Controls.AddRange(new Control[]{cbo,lp1,p1,lp2,p2,run});body.Controls.Add(input,0,0);
        var rb=new GroupBox{Text="Kết quả trả về",Dock=DockStyle.Fill,Font=new Font("Segoe UI Semibold",10,FontStyle.Bold),Padding=new Padding(12)};result.Dock=DockStyle.Fill;result.AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.DisplayedCells;result.AutoSizeRowsMode=DataGridViewAutoSizeRowsMode.None;result.DefaultCellStyle.WrapMode=DataGridViewTriState.False;rb.Controls.Add(result);body.Controls.Add(rb,0,1);
        var sourceBar=new FlowLayoutPanel{Dock=DockStyle.Fill,Padding=new Padding(0,7,0,0)};loadSource.Width=180;loadSource.Click+=LoadSourceData;sourceTable.Margin=new Padding(8,6,0,0);sourceTable.SelectedIndexChanged+=(_,_)=>{sourceData.DataSource=null;sourceStatus.Text="Chưa load dữ liệu bảng "+sourceTable.Text;sourceStatus.ForeColor=Color.FromArgb(75,85,99);};sourceStatus.Text="Chưa load dữ liệu CSDL";sourceBar.Controls.AddRange(new Control[]{loadSource,sourceTable,sourceStatus});body.Controls.Add(sourceBar,0,2);
        sourceGroup.Text="Dữ liệu CSDL liên quan";sourceData.Dock=DockStyle.Fill;sourceGroup.Controls.Add(sourceData);body.Controls.Add(sourceGroup,0,3);Controls.Add(body);Controls.Add(con);Controls.Add(head);
        cbo.SelectedIndexChanged+=(_,_)=>Params();cbo.SelectedIndex=0;Enable(false);
    }
    async void Connect(object? s,EventArgs e)
    {
        using var operation=DoAn.Shared.FormOperation.TryStart(this);
        if(operation is null)return;
        try
        {
            await using var c=new SqlConnection(Cs);await c.OpenAsync();
            int count=await FunctionCount(c);
            if(count<7)throw new InvalidOperationException($"Thiếu function Bài 7 ({count}/7). Hãy cài đặt/cập nhật script riêng; dữ liệu chưa bị thay đổi.");
            connected=true;status.Text="● Đã kết nối QL_DeAn";status.ForeColor=Color.SeaGreen;
        }
        catch(Exception x){connected=false;status.Text="● Không thể kết nối CSDL Bài 7";status.ForeColor=Color.Firebrick;MessageBox.Show(DoAn.Shared.FormOperation.ErrorMessage(x),"ERROR",MessageBoxButtons.OK,MessageBoxIcon.Error);}
        finally{operation.Dispose();Enable(connected);}
    }
    static async Task<int> FunctionCount(SqlConnection c){await using var q=new SqlCommand("SELECT COUNT(*) FROM sys.objects WHERE type IN ('FN','IF','TF') AND name LIKE 'fn_B7_%'",c);return Convert.ToInt32(await q.ExecuteScalarAsync());}
    void Enable(bool x){cbo.Enabled=run.Enabled=loadSource.Enabled=sourceTable.Enabled=x;}
    void Params(){if(cbo.SelectedIndex<0)return;var f=Fns[cbo.SelectedIndex];lp1.Text=f.P1;p1.Visible=lp1.Visible=f.P1.Length>0;lp2.Text=f.P2;p2.Visible=lp2.Visible=f.P2.Length>0;result.DataSource=null;UpdateSourceTables(f.Code);}
    async void Execute(object? s,EventArgs e)
    {
        if(!connected || cbo.SelectedIndex < 0)return;
        using var operation=DoAn.Shared.FormOperation.TryStart(this);
        if(operation is null)return;
        int selected=cbo.SelectedIndex;
        result.DataSource=null;
        run.Enabled=false;
        try
        {
            var data=await Query(Fns[selected],p1.Text.Trim(),p2.Text.Trim());
            if(cbo.SelectedIndex==selected)result.DataSource=data;
        }
        catch(ArgumentException x)
        {
            MessageBox.Show(x.Message,"Dữ liệu không hợp lệ",MessageBoxButtons.OK,MessageBoxIcon.Warning);
        }
        catch(Exception x){SqlError(x);}
        finally{run.Enabled=connected;}
    }

    static async Task<DataTable> Query(Fn f,string a,string b,string connectionString=Cs)
    {
        a=a.Trim();b=b.Trim();
        decimal hours=0;
        if(f.Code=="7.4")
        {
            // Accept either decimal separator, with no thousands separators or silent rounding.
            if(!decimal.TryParse(a.Replace(',', '.'), NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign,
                CultureInfo.InvariantCulture,out hours) || hours< -99999999.99m || hours>99999999.99m || decimal.Round(hours,2)!=hours)
                throw new ArgumentException("Tổng số giờ phải là số từ -99999999,99 đến 99999999,99, tối đa 2 chữ số thập phân.");
        }
        else if(f.P1.Length>0 && (string.IsNullOrWhiteSpace(a) || a.Length>10))
            throw new ArgumentException(f.P1+" phải có từ 1 đến 10 ký tự.");
        if(f.P2.Length>0 && (string.IsNullOrWhiteSpace(b) || b.Length>10))
            throw new ArgumentException(f.P2+" phải có từ 1 đến 10 ký tự.");

        await using var c=new SqlConnection(connectionString);
        await c.OpenAsync();
        if(f.Code=="7.1")
            await RequireCode(c,"SELECT COUNT(*) FROM dbo.B7_PhongBan WHERE MaPB=@code",a,"Mã phòng không tồn tại.");
        if(f.Code=="7.2")
        {
            await RequireCode(c,"SELECT COUNT(*) FROM dbo.B7_NhanVien WHERE MaNV=@code",a,"Mã nhân viên không tồn tại.");
            await RequireCode(c,"SELECT COUNT(*) FROM dbo.B7_DeAn WHERE MaDA=@code",b,"Mã đề án không tồn tại.");
        }
        await using var q=new SqlCommand(f.Sql,c);
        if(f.Code=="7.4")
        {
            var parameter=q.Parameters.Add("@p1",SqlDbType.Decimal);
            parameter.Precision=10; parameter.Scale=2; parameter.Value=hours;
        }
        else q.Parameters.Add("@p1",SqlDbType.VarChar,10).Value=a;
        q.Parameters.Add("@p2",SqlDbType.VarChar,10).Value=b;
        await using var r=await q.ExecuteReaderAsync();
        var d=new DataTable();d.Load(r);
        await r.DisposeAsync();
        string note="";
        if(f.Code=="7.1")
        {
            await using var info=new SqlCommand("SELECT COUNT(*) FROM dbo.B7_NhanVien WHERE MaPB=@code",c);
            info.Parameters.Add("@code",SqlDbType.VarChar,10).Value=a;
            if(Convert.ToInt32(await info.ExecuteScalarAsync())==0)note="Phòng tồn tại nhưng chưa có nhân viên.";
        }
        if(f.Code=="7.2")
        {
            await using var info=new SqlCommand("SELECT COUNT(*) FROM dbo.B7_PhanCong WHERE MaNV=@nv AND MaDA=@da",c);
            info.Parameters.Add("@nv",SqlDbType.VarChar,10).Value=a;
            info.Parameters.Add("@da",SqlDbType.VarChar,10).Value=b;
            if(Convert.ToInt32(await info.ExecuteScalarAsync())==0)note="Nhân viên không tham gia đề án này.";
        }
        if(note.Length>0){d.Columns.Add("ThongBao",typeof(string));foreach(DataRow row in d.Rows)row["ThongBao"]=note;}
        return d;
    }

    static async Task RequireCode(SqlConnection connection,string sql,string code,string message)
    {
        await using var command=new SqlCommand(sql,connection);
        command.Parameters.Add("@code",SqlDbType.VarChar,10).Value=code;
        if(Convert.ToInt32(await command.ExecuteScalarAsync())==0)throw new ArgumentException(message);
    }
    void UpdateSourceTables(string code)
    {
        string[] tables=code switch
        {
            "7.1" or "7.3" => ["B7_NhanVien","B7_PhongBan"],
            "7.2" => ["B7_NhanVien","B7_PhanCong","B7_DeAn"],
            "7.4" => ["Tổng giờ theo nhân viên","B7_PhanCong"],
            "7.5" => ["B7_PhongBan","B7_DeAn"],
            "7.6a" or "7.6b" => ["B7_NhanVien","B7_ThanNhan","B7_PhongBan"],
            _ => []
        };
        sourceTable.Items.Clear();
        sourceTable.Items.AddRange(tables);
        if(sourceTable.Items.Count>0)sourceTable.SelectedIndex=0;
        sourceData.DataSource=null;
        sourceGroup.Text=$"Dữ liệu CSDL liên quan {code}";
    }
    static string SourceSql(string table)=>table switch
    {
        "B7_NhanVien" => "SELECT * FROM dbo.B7_NhanVien ORDER BY MaPB,MaNV",
        "B7_PhongBan" => "SELECT * FROM dbo.B7_PhongBan ORDER BY MaPB",
        "B7_PhanCong" => "SELECT * FROM dbo.B7_PhanCong ORDER BY MaNV,MaDA",
        "B7_DeAn" => "SELECT * FROM dbo.B7_DeAn ORDER BY MaDA",
        "B7_ThanNhan" => "SELECT * FROM dbo.B7_ThanNhan ORDER BY MaNV,HoTen",
        "Tổng giờ theo nhân viên" => "SELECT MaNV,SUM(SoGio) AS Time_Total FROM dbo.B7_PhanCong GROUP BY MaNV ORDER BY MaNV",
        _ => throw new InvalidOperationException("Bảng dữ liệu không hợp lệ.")
    };
    async void LoadSourceData(object? s,EventArgs e)
    {
        if(!connected||sourceTable.SelectedItem is not string table)return;
        using var operation=DoAn.Shared.FormOperation.TryStart(this);
        if(operation is null)return;
        loadSource.Enabled=sourceTable.Enabled=cbo.Enabled=false;
        sourceStatus.Text="Đang load dữ liệu CSDL...";
        try
        {
            await using var connection=new SqlConnection(Cs);
            await connection.OpenAsync();
            await using var command=new SqlCommand(SourceSql(table),connection);
            await using var reader=await command.ExecuteReaderAsync();
            var data=new DataTable();
            data.Load(reader);
            sourceData.DataSource=data;
            sourceGroup.Text=$"Dữ liệu CSDL liên quan {Fns[cbo.SelectedIndex].Code} - {table}";
            sourceStatus.Text=$"Đã load {data.Rows.Count} dòng từ {table}";
            sourceStatus.ForeColor=Color.SeaGreen;
        }
        catch(Exception ex)
        {
            sourceData.DataSource=null;
            sourceStatus.Text="Load dữ liệu CSDL thất bại";
            sourceStatus.ForeColor=Color.Firebrick;
            MessageBox.Show(ex.Message,"Lỗi load CSDL",MessageBoxButtons.OK,MessageBoxIcon.Error);
        }
        finally{loadSource.Enabled=sourceTable.Enabled=cbo.Enabled=connected;}
    }
    static void SqlError(Exception x)=>MessageBox.Show("Hãy chạy 2 file trong DATABASE\\Nhom_3_CSDL_DeAn theo thứ tự 01 → 02.\n\n"+x.Message,"Lỗi SQL",MessageBoxButtons.OK,MessageBoxIcon.Error);
    static Button Button(string t,Color c)=>new(){Text=t,BackColor=c,ForeColor=Color.White,FlatStyle=FlatStyle.Flat,Font=new Font("Segoe UI Semibold",10,FontStyle.Bold),Cursor=Cursors.Hand,Height=42};
    static DataGridView Grid()=>new(){ReadOnly=true,AllowUserToAddRows=false,AllowUserToDeleteRows=false,RowHeadersVisible=false,SelectionMode=DataGridViewSelectionMode.FullRowSelect,AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill,AutoSizeRowsMode=DataGridViewAutoSizeRowsMode.AllCells,BackgroundColor=Color.White};
    sealed record Fn(string Code,string Title,string P1,string P2,string Sql);
}
