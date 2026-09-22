using System.Data;
using System.Drawing;
using Microsoft.Data.SqlClient;

namespace Bai_07_Function_DeAn;

public partial class Form1 : Form
{
    const string Cs=@"Data Source=.\SQLEXPRESS02;Initial Catalog=QL_DeAn;Integrated Security=True;Encrypt=False;TrustServerCertificate=True";
    readonly ComboBox cbo=new(); readonly TextBox p1=new(),p2=new(); readonly Label lp1=new(),lp2=new(),status=new(),testStatus=new();
    readonly DataGridView result=Grid(), tests=Grid(); readonly Button run=Button("▶  Thực hiện",Color.FromArgb(16,185,129)),load=Button("▣  Load Testcase",Color.FromArgb(37,99,235)),runTests=Button("▶  Chạy Testcase",Color.FromArgb(249,115,22));
    readonly GroupBox testGroup=new(){Dock=DockStyle.Fill,Font=new Font("Segoe UI Semibold",10,FontStyle.Bold),Padding=new Padding(12)};
    readonly ComboBox sourceTable=new(){DropDownStyle=ComboBoxStyle.DropDownList,Width=220};
    readonly DataGridView sourceData=Grid();
    readonly Label sourceStatus=new(){AutoSize=true,Margin=new Padding(20,12,0,0),Font=new Font("Segoe UI Semibold",10,FontStyle.Bold)};
    readonly Button loadSource=Button("▣  Load CSDL",Color.FromArgb(37,99,235));
    readonly GroupBox sourceGroup=new(){Dock=DockStyle.Fill,Font=new Font("Segoe UI Semibold",10,FontStyle.Bold),Padding=new Padding(12)};
    bool connected;
    int loadedFunctionIndex=-1;
    static readonly Fn[] Fns={
        new("7.1","Lương trung bình một phòng","Mã phòng","","SELECT dbo.fn_B7_LuongTrungBinhPhong(@p1) LuongTrungBinh"),
        new("7.2","Tổng lương nhân viên theo đề án","Mã nhân viên","Mã đề án","SELECT dbo.fn_B7_TongLuongNhanVienDeAn(@p1,@p2) TongLuong"),
        new("7.3","Tổng lương trung bình các phòng","","","SELECT dbo.fn_B7_TongLuongTrungBinhCacPhong() TongLuongTrungBinh"),
        new("7.4","Tiền thưởng theo tổng giờ","Tổng số giờ","","SELECT dbo.fn_B7_TienThuong(TRY_CONVERT(decimal(10,2),NULLIF(@p1,''))) TienThuong"),
        new("7.5","Số đề án theo mỗi phòng","","","SELECT * FROM dbo.fn_B7_SoDeAnTheoPhong() ORDER BY MaPB"),
        new("7.6a","Thông tin nhân viên - Inline TVF","","","SELECT * FROM dbo.fn_B7_ThongTinNhanVien_Inline() ORDER BY MaNV"),
        new("7.6b","Thông tin nhân viên - Multistatement TVF","","","SELECT * FROM dbo.fn_B7_ThongTinNhanVien_Multi() ORDER BY MaNV")};

    public Form1(){InitializeComponent();Build();}
    void Build(){
        Text="Bài 7 - Function CSDL Đề án";StartPosition=FormStartPosition.CenterParent;MinimumSize=new Size(1050,820);Size=new Size(1200,950);BackColor=Color.FromArgb(248,250,252);Font=new Font("Segoe UI",10);
        var head=new Panel{Dock=DockStyle.Top,Height=94,BackColor=Color.FromArgb(79,70,229)};head.Controls.Add(new Label{Text="FUNCTION CƠ SỞ DỮ LIỆU ĐỀ ÁN",Dock=DockStyle.Fill,TextAlign=ContentAlignment.MiddleCenter,Font=new Font("Segoe UI Semibold",20,FontStyle.Bold),ForeColor=Color.White});
        var con=new Panel{Dock=DockStyle.Top,Height=58,BackColor=Color.White,Padding=new Padding(28,8,28,8)};var bc=Button("⌁  Kết nối CSDL",Color.FromArgb(37,99,235));bc.Dock=DockStyle.Left;bc.Width=185;bc.Click+=Connect;status.Text="● Chưa kết nối QL_DeAn";status.Dock=DockStyle.Fill;status.Padding=new Padding(18,0,0,0);status.TextAlign=ContentAlignment.MiddleLeft;status.ForeColor=Color.Firebrick;con.Controls.Add(status);con.Controls.Add(bc);
        var body=new TableLayoutPanel{Dock=DockStyle.Fill,Padding=new Padding(28,18,28,18),RowCount=6,ColumnCount=1};body.RowStyles.Add(new RowStyle(SizeType.Absolute,130));body.RowStyles.Add(new RowStyle(SizeType.Percent,25));body.RowStyles.Add(new RowStyle(SizeType.Absolute,58));body.RowStyles.Add(new RowStyle(SizeType.Percent,35));body.RowStyles.Add(new RowStyle(SizeType.Absolute,58));body.RowStyles.Add(new RowStyle(SizeType.Percent,40));
        var input=new GroupBox{Text="Chọn function và nhập tham số",Dock=DockStyle.Fill,Font=new Font("Segoe UI Semibold",10,FontStyle.Bold)};cbo.DropDownStyle=ComboBoxStyle.DropDownList;cbo.Location=new Point(25,43);cbo.Size=new Size(350,30);cbo.Items.AddRange(Fns.Select(x=>$"{x.Code} - {x.Title}").ToArray());lp1.Location=new Point(400,25);lp1.AutoSize=true;p1.Location=new Point(400,51);p1.Size=new Size(180,30);lp2.Location=new Point(605,25);lp2.AutoSize=true;p2.Location=new Point(605,51);p2.Size=new Size(180,30);run.Location=new Point(815,40);run.Size=new Size(190,43);run.Click+=Execute;input.Controls.AddRange(new Control[]{cbo,lp1,p1,lp2,p2,run});body.Controls.Add(input,0,0);
        var rb=new GroupBox{Text="Kết quả trả về",Dock=DockStyle.Fill,Font=new Font("Segoe UI Semibold",10,FontStyle.Bold),Padding=new Padding(12)};result.Dock=DockStyle.Fill;rb.Controls.Add(result);body.Controls.Add(rb,0,1);
        var bar=new FlowLayoutPanel{Dock=DockStyle.Fill,Padding=new Padding(0,7,0,0)};load.Width=180;runTests.Width=190;load.Click+=LoadTestCases;runTests.Click+=RunAll;testStatus.Text="Chưa load testcase";testStatus.AutoSize=true;testStatus.Margin=new Padding(22,12,0,0);testStatus.Font=new Font("Segoe UI Semibold",10,FontStyle.Bold);bar.Controls.AddRange(new Control[]{load,runTests,testStatus});body.Controls.Add(bar,0,2);
        testGroup.Text="Danh sách testcase Bài 7";tests.Dock=DockStyle.Fill;testGroup.Controls.Add(tests);body.Controls.Add(testGroup,0,3);
        var sourceBar=new FlowLayoutPanel{Dock=DockStyle.Fill,Padding=new Padding(0,7,0,0)};loadSource.Width=180;loadSource.Click+=LoadSourceData;sourceTable.Margin=new Padding(8,6,0,0);sourceTable.SelectedIndexChanged+=(_,_)=>{sourceData.DataSource=null;sourceStatus.Text="Chưa load dữ liệu bảng "+sourceTable.Text;sourceStatus.ForeColor=Color.FromArgb(75,85,99);};sourceStatus.Text="Chưa load dữ liệu CSDL";sourceBar.Controls.AddRange(new Control[]{loadSource,sourceTable,sourceStatus});body.Controls.Add(sourceBar,0,4);
        sourceGroup.Text="Dữ liệu CSDL liên quan";sourceData.Dock=DockStyle.Fill;sourceGroup.Controls.Add(sourceData);body.Controls.Add(sourceGroup,0,5);Controls.Add(body);Controls.Add(con);Controls.Add(head);
        cbo.SelectedIndexChanged+=(_,_)=>Params();cbo.SelectedIndex=0;Enable(false);
    }
    async void Connect(object? s,EventArgs e){try{Enable(false);status.Text="● Đang kiểm tra CSDL Bài 7...";status.ForeColor=Color.DarkOrange;await using var c=new SqlConnection(Cs);await c.OpenAsync();int count=await FunctionCount(c);if(count<7){status.Text="● Đang tự động cài đặt dữ liệu và function Bài 7...";Application.DoEvents();await InstallDatabase(c);count=await FunctionCount(c);}if(count<7)throw new InvalidOperationException($"Cài đặt chưa hoàn tất ({count}/7 function).");connected=true;status.Text="● Đã kết nối QL_DeAn • Đủ 7 function";status.ForeColor=Color.SeaGreen;Enable(true);MessageBox.Show("CSDL Bài 7 đã sẵn sàng. Bạn có thể thực hiện function hoặc chạy testcase.","Kết nối thành công",MessageBoxButtons.OK,MessageBoxIcon.Information);}catch(Exception x){connected=false;Enable(false);status.Text="● Không thể khởi tạo CSDL Bài 7";status.ForeColor=Color.Firebrick;MessageBox.Show("Không thể khởi tạo CSDL Bài 7.\n\n"+x.Message,"Lỗi",MessageBoxButtons.OK,MessageBoxIcon.Error);}}
    static async Task<int> FunctionCount(SqlConnection c){await using var q=new SqlCommand("SELECT COUNT(*) FROM sys.objects WHERE type IN ('FN','IF','TF') AND name LIKE 'fn_B7_%'",c);return Convert.ToInt32(await q.ExecuteScalarAsync());}
    static async Task InstallDatabase(SqlConnection c){var asm=typeof(Form1).Assembly;string[] files={"01_TaoBang_NhapDuLieu.sql","02_Functions.sql","03_Testcase.sql"};foreach(string file in files){string? resource=asm.GetManifestResourceNames().FirstOrDefault(n=>n.EndsWith(file,StringComparison.OrdinalIgnoreCase));if(resource==null)throw new InvalidOperationException("Không tìm thấy script nhúng: "+file);using var stream=asm.GetManifestResourceStream(resource)!;using var reader=new StreamReader(stream);string sql=await reader.ReadToEndAsync();string[] batches=System.Text.RegularExpressions.Regex.Split(sql,@"^\s*GO\s*$(?:\r?\n)?",System.Text.RegularExpressions.RegexOptions.Multiline|System.Text.RegularExpressions.RegexOptions.IgnoreCase);foreach(string batch in batches.Where(x=>!string.IsNullOrWhiteSpace(x))){await using var cmd=new SqlCommand(batch,c){CommandTimeout=60};await cmd.ExecuteNonQueryAsync();}}}
    void Enable(bool x){cbo.Enabled=run.Enabled=load.Enabled=runTests.Enabled=loadSource.Enabled=sourceTable.Enabled=x;}
    void Params(){if(cbo.SelectedIndex<0)return;var f=Fns[cbo.SelectedIndex];lp1.Text=f.P1;p1.Visible=lp1.Visible=f.P1.Length>0;lp2.Text=f.P2;p2.Visible=lp2.Visible=f.P2.Length>0;result.DataSource=null;UpdateSourceTables(f.Code);if(loadedFunctionIndex>=0)LoadTestCases(null,EventArgs.Empty);else testGroup.Text=$"Danh sách testcase {f.Code}";}
    async void Execute(object? s,EventArgs e){if(!connected)return;int selected=cbo.SelectedIndex;try{var data=await Query(Fns[selected],p1.Text.Trim(),p2.Text.Trim());if(cbo.SelectedIndex==selected)result.DataSource=data;}catch(Exception x){SqlError(x);}}
    static async Task<DataTable> Query(Fn f,string a,string b){await using var c=new SqlConnection(Cs);await c.OpenAsync();await using var q=new SqlCommand(f.Sql,c);q.Parameters.AddWithValue("@p1",a);q.Parameters.AddWithValue("@p2",b);await using var r=await q.ExecuteReaderAsync();var d=new DataTable();d.Load(r);return d;}
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
    void LoadTestCases(object? s,EventArgs e){if(cbo.SelectedIndex<0)return;result.DataSource=null;int selected=cbo.SelectedIndex;var d=new DataTable();foreach(var c in new[]{"MaCase","YeuCau","DuLieu","KetQua","TrangThai"})d.Columns.Add(c);foreach(var t in Cases().Where(t=>t.Fi==selected))d.Rows.Add(t.Id,t.Req,t.Input,"","CHƯA CHẠY");tests.DataSource=d;loadedFunctionIndex=selected;testGroup.Text=$"Danh sách testcase {Fns[selected].Code}";string[] h={"Mã testcase","Yêu cầu","Dữ liệu test","Kết quả sau khi chạy","Trạng thái"};for(int i=0;i<5;i++)tests.Columns[i].HeaderText=h[i];tests.Columns[0].FillWeight=65;tests.Columns[1].FillWeight=140;tests.Columns[2].FillWeight=90;tests.Columns[3].FillWeight=180;tests.Columns[4].FillWeight=70;testStatus.Text=$"Đã load {d.Rows.Count} testcase {Fns[selected].Code} - Chưa chạy";testStatus.ForeColor=Color.SeaGreen;}
    async void RunAll(object? s,EventArgs e){int selected=cbo.SelectedIndex;var cs=Cases().Where(t=>t.Fi==selected).ToArray();if(loadedFunctionIndex!=selected||tests.DataSource is not DataTable d||d.Rows.Count!=cs.Length){MessageBox.Show("Hãy Load Testcase cho function đang chọn trước.");return;}load.Enabled=runTests.Enabled=cbo.Enabled=run.Enabled=false;int ok=0,fail=0;try{for(int i=0;i<cs.Length;i++){var row=d.Rows[i];row["TrangThai"]="ĐANG CHẠY";tests.Refresh();Application.DoEvents();try{row["KetQua"]=Format(await Query(Fns[selected],cs[i].A,cs[i].B));row["TrangThai"]="ĐÃ CHẠY";ok++;}catch(Exception x){row["KetQua"]=x.Message;row["TrangThai"]="LỖI";fail++;}testStatus.Text=$"{Fns[selected].Code}: Đã chạy {i+1}/{cs.Length} | Lỗi: {fail}";}testStatus.ForeColor=fail==0?Color.SeaGreen:Color.DarkOrange;MessageBox.Show($"Đã chạy xong {cs.Length} testcase {Fns[selected].Code}.\nThành công: {ok}\nLỗi: {fail}");}finally{load.Enabled=runTests.Enabled=cbo.Enabled=run.Enabled=connected;}}
    static string Format(DataTable d)=>d.Rows.Count==0?"Không có dữ liệu":string.Join(" | ",d.Rows.Cast<DataRow>().Take(3).Select(r=>string.Join("; ",r.ItemArray.Select(v=>v==DBNull.Value?"NULL":v?.ToString()))))+(d.Rows.Count>3?$" | ... ({d.Rows.Count} dòng)":"");
    static void SqlError(Exception x)=>MessageBox.Show("Hãy chạy đủ 3 file trong DATABASE\\Nhom_3_CSDL_DeAn theo thứ tự 01 → 02 → 03.\n\n"+x.Message,"Lỗi SQL",MessageBoxButtons.OK,MessageBoxIcon.Error);
    static Button Button(string t,Color c)=>new(){Text=t,BackColor=c,ForeColor=Color.White,FlatStyle=FlatStyle.Flat,Font=new Font("Segoe UI Semibold",10,FontStyle.Bold),Cursor=Cursors.Hand,Height=42};
    static DataGridView Grid()=>new(){ReadOnly=true,AllowUserToAddRows=false,AllowUserToDeleteRows=false,RowHeadersVisible=false,SelectionMode=DataGridViewSelectionMode.FullRowSelect,AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill,AutoSizeRowsMode=DataGridViewAutoSizeRowsMode.AllCells,BackgroundColor=Color.White};
    static Tc[] Cases()=>new Tc[]{
        new("B71-01","Phòng hợp lệ","PB01",0,"PB01",""),new("B71-02","Phòng không tồn tại","XXX",0,"XXX",""),new("B71-03","Phòng không nhân viên","PB00",0,"PB00",""),new("B71-04","Mã phòng rỗng","rỗng",0,"",""),
        new("B72-01","Có tham gia đề án","NV01, DA01",1,"NV01","DA01"),new("B72-02","Không tham gia đề án","NV01, DA03",1,"NV01","DA03"),new("B72-03","Nhân viên không tồn tại","XXX",1,"XXX","DA01"),new("B72-04","Đề án không tồn tại","XXX",1,"NV01","XXX"),
        new("B73-01","Tổng trung bình các phòng","Không tham số",2,"",""),new("B74-01","Dưới 30 giờ","29",3,"29",""),new("B74-02","Biên 30 giờ","30",3,"30",""),new("B74-03","Biên 60 giờ","60",3,"60",""),new("B74-04","Trên 60 giờ","61",3,"61",""),new("B74-05","Biên 99 giờ","99",3,"99",""),new("B74-06","Biên 100 giờ","100",3,"100",""),new("B74-07","Biên 149 giờ","149",3,"149",""),new("B74-08","Biên 150 giờ","150",3,"150",""),new("B74-09","Giờ âm","-1",3,"-1",""),new("B74-10","NULL/rỗng","rỗng",3,"",""),new("B75-01","Mọi phòng, kể cả 0 đề án","Không tham số",4,"",""),new("B76A-01","Inline TVF","Có/không người thân",5,"",""),new("B76B-01","Multistatement TVF","Đối chiếu Inline",6,"","")};
    sealed record Fn(string Code,string Title,string P1,string P2,string Sql);sealed record Tc(string Id,string Req,string Input,int Fi,string A,string B);
}
