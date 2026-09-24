using System.Data;
using System.Reflection;
using Microsoft.Data.SqlClient;
using DoAn.Shared;
using ThongTinThuVien;
using Bai_06_Trigger_ThuVien;

internal static class Program
{
    private static int passed;
    private static string Cs(string server, string database) =>
        $"Data Source={server};Initial Catalog={database};Integrated Security=True;Encrypt=False;TrustServerCertificate=True";

    [STAThread]
    private static void Main(string[] args)
    {
        if (args.Length != 5 || args.Skip(1).Any(x => !x.StartsWith("Audit_", StringComparison.Ordinal)))
            throw new ArgumentException("Pass server and four temporary Audit_ databases (DeAn, ThuVien, Gara, Truong).");
        string deAn=Cs(args[0],args[1]), library=Cs(args[0],args[2]), garage=Cs(args[0],args[3]), school=Cs(args[0],args[4]);

        Check(Compare(typeof(Bài_1.Form1),"SoSanhKetQua","Phương trình có nghiệm: x = 2","Phương trình có nghiệm: x = 2","B1-TC03"),"B1 comparator accepts correct root");
        Check(!Compare(typeof(Bài_1.Form1),"SoSanhKetQua","Phương trình có nghiệm: x = 3","Phương trình có nghiệm: x = 2","B1-TC03"),"B1 comparator rejects wrong root");
        Check(Compare(typeof(Bài_2.Form1),"SoSanhKetQuaB2","Phương trình có 2 nghiệm: x1 = 2 và x2 = 1","Phương trình có 2 nghiệm: x1 = 2 và x2 = 1","B2-TC04"),"B2 comparator accepts both correct roots");
        Check(!Compare(typeof(Bài_2.Form1),"SoSanhKetQuaB2","Phương trình có 2 nghiệm: x1 = 3 và x2 = 1","Phương trình có 2 nghiệm: x1 = 2 và x2 = 1","B2-TC04"),"B2 comparator rejects wrong root");
        Check(!Compare(typeof(TinhTuoi.Form1),"SoSanhKetQuaB4","Ngày sinh không đúng định dạng!","Ngày sinh không tồn tại!","31/02/2005"),"B4 comparator distinguishes invalid date");
        Check(!Compare(typeof(TinhTuoi.Form1),"SoSanhKetQuaB4","999 tuổi","Tuổi hợp lệ","15/03/2006"),"B4 comparator rejects wrong age");

        foreach (string? input in new string?[] { null, "", "UNKNOWN" })
        {
            try { TestcaseBai5Helper.ChayProcedure(library,"dbo.sp_ThongtinDocGia","B5A",input); throw new Exception("Expected rejection"); }
            catch (SqlException ex) { Check(SqlFailureClassifier.IsLibraryLookup(ex), "B5 business rejection: " + (input ?? "NULL")
                + " — " + string.Join("; ", ex.Errors.Cast<SqlError>().Select(x=>$"{x.Number}/{x.Procedure}: {x.Message}"))); }
        }
        try { Table(library,"RAISERROR(N'Unexpected failure',16,1)"); throw new Exception("Expected rejection"); }
        catch (SqlException ex) { Check(!SqlFailureClassifier.IsLibraryLookup(ex), "Unrelated RAISERROR is not a lookup result"); }
        try { TestcaseBai5Helper.ChayProcedure("invalid connection","dbo.sp_ThongtinDocGia","B5A",new string('A',21)); throw new Exception("Expected validation"); }
        catch (ArgumentException ex) { Check(ex.Message.Contains("20"),"B5 rejects long codes before connecting"); }
        Check(TestcaseBai5Helper.ChayProcedure(library,"dbo.sp_ThongtinDocGia","B5A","DG001").Rows.Count==1,"B5 valid reader");
        try { TestcaseBai5Helper.ChayProcedure(library,"dbo.sp_ThongTinDauSach","B5B","UNKNOWN"); throw new Exception("Expected rejection"); }
        catch(SqlException ex) { Check(SqlFailureClassifier.IsLibraryLookup(ex),"B3/B5 missing ISBN is a lookup result"); }
        try { Table(library,"RAISERROR(N'Không tìm thấy độc giả có mã này.',16,1)"); throw new Exception("Expected rejection"); }
        catch(SqlException ex) { Check(!SqlFailureClassifier.IsLibraryLookup(ex),"Same message from wrong procedure is not accepted"); }

        string before=Snapshot(library);
        DateTime day=new(2026,9,1);
        foreach (var item in new[] {
            ("MISSING","CS001","DG001",day.AddDays(14),"FK_Muon_Cuonsach"),
            ("ISBN001","CS001","MISSING",day.AddDays(14),"FK_Muon_DocGia"),
            ("ISBN001","CS004","DG001",day.AddDays(14),"PK_Muon"),
            ("ISBN001","CS004","DG002",day.AddDays(14),"UQ_Muon_CuonDangMuon"),
            ("ISBN001","CS001","DG001",day.AddDays(-1),"CK_Muon_ThoiHan") })
        {
            string result=TestCaseBai6Helper.KiemTraInsMuon(library,item.Item1,item.Item2,item.Item3,day,item.Item4,expectedConstraint:item.Item5);
            Check(result.Contains("PASS"),"B6 expected " + item.Item5);
        }
        string title=(string)Table(library,"SELECT TOP 1 ma_tuasach FROM dbo.Tuasach ORDER BY ma_tuasach").Rows[0][0];
        Check(TestCaseBai6Helper.KiemTraInfThongBao(library,"INSERT",title,"Test","Author","",expectedConstraint:"PK_Tuasach").Contains("PASS"),"B6 duplicate title");
        Check(TestCaseBai6Helper.KiemTraInsMuon(library,"ISBN001","CS001","DG001",day,day.AddDays(14),expectedConstraint:"CK_Muon_ThoiHan").Contains("FAIL"),"B6 accepted input must fail negative test");
        try { TestCaseBai6Helper.KiemTraInsMuon(library,"MISSING","CS001","DG001",day,day.AddDays(14),expectedConstraint:"CK_Muon_ThoiHan"); throw new Exception("Expected mismatch"); }
        catch(SqlException ex) { Check(!SqlFailureClassifier.IsConstraint(ex,"CK_Muon_ThoiHan"),"B6 wrong constraint cannot PASS"); }
        Check(before==Snapshot(library),"B6 all test data rolled back");

        foreach(string input in new[] { "", "abc", "NaN", "Infinity", "100000000", "1.001" })
            ExpectInputFailure("7.4",input,"","invalid connection");
        foreach(var item in new[] { ("-1",0m),("0",0m),("29.99",0m),("30",500m),("60",500m),("60,01",1000m),("100",1200m),("150",1600m) })
            Check(Convert.ToDecimal(Query7("7.4",item.Item1,"",deAn).Rows[0][0])==item.Item2,"B7 hours " + item.Item1);
        ExpectInputFailure("7.1","UNKNOWN","",deAn);
        ExpectInputFailure("7.2","UNKNOWN","DA01",deAn);
        ExpectInputFailure("7.2","NV01","UNKNOWN",deAn);
        Check(Query7("7.1","PB01","",deAn).Rows.Count==1,"B7 valid department");

        using var garageForm=new Bai_09_QuanLyGara.Form1();
        string garageSql=Queries(garageForm)["9.RB"];
        Check(Table(garage,garageSql).Rows.Cast<DataRow>().All(x => (string)x["KetQua"]=="Đạt"),"B9 expected trigger and valid control");
        Table(garage,"INSERT dbo.B9_THO VALUES('T99',N'Collision fixture',1,'T01'); SELECT 1;");
        Check((string)Table(garage,garageSql).Rows[1]["KetQua"]=="Không đạt","B9 duplicate key cannot PASS");
        Table(garage,"DELETE dbo.B9_THO WHERE MaTho='T99'; DISABLE TRIGGER dbo.tg_B9_KiemTraNhomTruong ON dbo.B9_THO; SELECT 1;");
        Check((string)Table(garage,garageSql).Rows[1]["KetQua"]=="Không đạt","B9 disabled trigger cannot PASS");
        Table(garage,"ENABLE TRIGGER dbo.tg_B9_KiemTraNhomTruong ON dbo.B9_THO; SELECT 1;");
        using (var probe = new MissingFunctionForm())
        using (var connection = new SqlConnection(garage))
        {
            connection.Open();
            try
            {
                var check = (Task)typeof(ExerciseQueryFormBase)
                    .GetMethod("VerifyQueryObjectsAsync",BindingFlags.Instance|BindingFlags.NonPublic)!
                    .Invoke(probe,[connection])!;
                check.GetAwaiter().GetResult();
                throw new Exception("Expected missing named function");
            }
            catch (InvalidOperationException ex)
            {
                Check(ex.Message.Contains("fn_B9_MissingForAudit"),"Form detects missing required function by name");
            }
        }

        using var schoolForm=new Bai_10_TruongPhoThong.Form1();
        foreach(var query in Queries(schoolForm).Where(x=>x.Key.StartsWith("10.1")))
        {
            Check((string)Table(school,query.Value).Rows[0]["KetQua"]=="Đạt","B10 expected " + query.Key);
            string unexpected=query.Value.Replace("'GV01'","'BAD'").Replace("'MH02'","'BAD'").Replace("'MH01'","'BAD'");
            Check((string)Table(school,unexpected).Rows[0]["KetQua"]=="Không đạt","B10 wrong FK cannot PASS " + query.Key);
        }
        Table(school,"DISABLE TRIGGER dbo.tg_B10_KiemTraBuoiThi ON dbo.B10_BUOITHI; SELECT 1;");
        Check((string)Table(school,Queries(schoolForm)["10.1.b"]).Rows[0]["KetQua"]=="Không đạt","B10 disabled trigger cannot PASS");
        Table(school,"ENABLE TRIGGER dbo.tg_B10_KiemTraBuoiThi ON dbo.B10_BUOITHI; SELECT 1;");
        const string originalDurationMessage = "Môn 30 tiết phải thi 120 phút;môn từ 45 tiết phải thi 150 phút.";
        const string spacedDurationMessage = "Môn 30 tiết phải thi 120 phút; môn từ 45 tiết phải thi 150 phút.";
        string triggerDefinition = (string)Table(school,
            "SELECT OBJECT_DEFINITION(OBJECT_ID(N'dbo.tg_B10_KiemTraBuoiThi'))").Rows[0][0];
        Check(triggerDefinition.Contains(originalDurationMessage),"B10 audit trigger starts with documented message");
        using (var connection = new SqlConnection(school))
        {
            connection.Open();
            string alteredTrigger = System.Text.RegularExpressions.Regex.Replace(
                triggerDefinition.Replace(originalDurationMessage,spacedDurationMessage),
                @"^\s*CREATE(?:\s+OR\s+ALTER)?\s+TRIGGER", "ALTER TRIGGER",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            using var command = new SqlCommand(alteredTrigger,connection);
            command.ExecuteNonQuery();
        }
        foreach (string code in new[] { "10.1.b", "10.1.c" })
        {
            DataRow result = Table(school,Queries(schoolForm)[code]).Rows[0];
            Check((string)result["KetQua"]=="Đạt" && ((string)result["ChiTiet"]).Contains(spacedDurationMessage),
                "B10 accepts equivalent trigger wording " + code);
        }
        using (var form = new FrmTestcaseBai5(library))
        {
            foreach (var item in new[] {
                ("ChayB5A","B5A-OK-01"),("ChayB5A","B5A-OK-02"),
                ("ChayB5A","B5A-NV-03"),("ChayB5A","B5A-SQL-01"),
                ("ChayB5A","B5A-SQL-02"),("ChayB5B","B5B-OK-01"),
                ("ChayB5B","B5B-OK-02"),("ChayB5B","B5B-SQL-02") })
            {
                var (ok, actual) = RunB5(form,item.Item1,item.Item2);
                Check(ok,$"B5 runner {item.Item2}: {actual}");
            }
        }
        Console.WriteLine($"BEHAVIOR CHECKS: {passed} PASS");
    }

    private static Dictionary<string,string> Queries(Form form)
    {
        var items=(System.Collections.IEnumerable)form.GetType().GetProperty("Queries",BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(form)!;
        return items.Cast<object>().ToDictionary(x=>(string)x.GetType().GetProperty("Code")!.GetValue(x)!,x=>(string)x.GetType().GetProperty("Sql")!.GetValue(x)!);
    }
    private static bool Compare(Type formType,string method,string actual,string expected,string input) =>
        (bool)formType.GetMethod(method,BindingFlags.Static|BindingFlags.NonPublic)!.Invoke(null,[actual,expected,input])!;
    private static (bool Pass,string Actual) RunB5(FrmTestcaseBai5 form,string method,string id) =>
        ((bool,string))typeof(FrmTestcaseBai5).GetMethod(method,BindingFlags.Instance|BindingFlags.NonPublic)!
            .Invoke(form,[id,"","",""])!;
    private static DataTable Query7(string code,string a,string b,string connection)
    {
        Type type=typeof(Bai_07_Function_DeAn.Form1);
        var functions=(Array)type.GetField("Fns",BindingFlags.Static|BindingFlags.NonPublic)!.GetValue(null)!;
        object fn=functions.Cast<object>().Single(x=>(string)x.GetType().GetProperty("Code")!.GetValue(x)! == code);
        var task=(Task<DataTable>)type.GetMethod("Query",BindingFlags.Static|BindingFlags.NonPublic)!.Invoke(null,[fn,a,b,connection])!;
        return task.GetAwaiter().GetResult();
    }
    private static void ExpectInputFailure(string code,string a,string b,string connection)
    {
        try { Query7(code,a,b,connection); throw new Exception("Expected validation"); }
        catch(ArgumentException ex) { Check(ex.Message.Contains(code=="7.4" ? "Tổng số giờ" : "không tồn tại"),$"B7 validation {code}: {a}/{b}"); }
    }
    private static DataTable Table(string connection,string sql)
    {
        using var c=new SqlConnection(connection); c.Open();
        using var q=new SqlCommand(sql,c);
        using var r=q.ExecuteReader(); var data=new DataTable(); data.Load(r);return data;
    }
    private static string Snapshot(string connection) => string.Join("|",new[]{"Muon","Cuonsach","Dausach","Tuasach"}.Select(t=>
        Table(connection,$"SELECT CHECKSUM_AGG(BINARY_CHECKSUM(*)) FROM dbo.{t}").Rows[0][0].ToString()));
    private static void Check(bool condition,string name)
    {
        if(!condition)throw new Exception("FAIL: "+name);
        passed++; Console.WriteLine("PASS: "+name);
    }
}

internal sealed class MissingFunctionForm : ExerciseQueryFormBase
{
    protected override string DatabaseName => "QL_Gara";
    protected override string WindowTitle => "Audit object lookup";
    protected override string HeaderTitle => "Audit object lookup";
    protected override System.Drawing.Color HeaderColor => System.Drawing.Color.Navy;
    protected override string ObjectNamePattern => "%B9_%";
    protected override int RequiredObjectCount => 0;
    protected override string[] ScriptResourceSuffixes => [];
    protected override QueryItem[] Queries =>
        [new("audit", "Missing function", "", "SELECT * FROM dbo.fn_B9_MissingForAudit()")];
}
