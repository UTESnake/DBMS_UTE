param(
    [string]$Server = '.\SQLEXPRESS02',
    [switch]$BehaviorChecks
)

$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$suffix = [Guid]::NewGuid().ToString('N').Substring(0, 8)
$databases = @{
    DeAn = "Audit_DeAn_$suffix"
    ThuVien = "Audit_ThuVien_$suffix"
    Gara = "Audit_Gara_$suffix"
    Truong = "Audit_Truong_$suffix"
}
$results = [System.Collections.Generic.List[object]]::new()

function New-Connection([string]$database) {
    return [System.Data.SqlClient.SqlConnection]::new(
        "Data Source=$Server;Initial Catalog=$database;Integrated Security=True;Encrypt=False;TrustServerCertificate=True"
    )
}

function Invoke-NonQuery([string]$database, [string]$sql) {
    $connection = New-Connection $database
    try {
        $connection.Open()
        $command = $connection.CreateCommand()
        $command.CommandTimeout = 120
        $command.CommandText = $sql
        [void]$command.ExecuteNonQuery()
    }
    finally {
        $connection.Dispose()
    }
}

function Invoke-Scalar([string]$database, [string]$sql) {
    $connection = New-Connection $database
    try {
        $connection.Open()
        $command = $connection.CreateCommand()
        $command.CommandTimeout = 120
        $command.CommandText = $sql
        return $command.ExecuteScalar()
    }
    finally {
        $connection.Dispose()
    }
}

function Invoke-Table([string]$database, [string]$sql) {
    $connection = New-Connection $database
    try {
        $connection.Open()
        $command = $connection.CreateCommand()
        $command.CommandTimeout = 120
        $command.CommandText = $sql
        $reader = $command.ExecuteReader()
        $table = [System.Data.DataTable]::new()
        $table.Load($reader)
        return ,$table
    }
    finally {
        $connection.Dispose()
    }
}

function Invoke-ScriptFile([string]$database, [string]$path, [string]$originalDatabase) {
    $sql = Get-Content -LiteralPath $path -Raw -Encoding UTF8
    $sql = $sql.Replace("[$originalDatabase]", "[$database]")
    $sql = $sql.Replace("USE $originalDatabase", "USE [$database]")
    $sql = $sql.Replace("N'$originalDatabase'", "N'$database'")
    $sql = $sql.Replace("CREATE DATABASE $originalDatabase", "CREATE DATABASE [$database]")

    $batches = [Text.RegularExpressions.Regex]::Split(
        $sql,
        '(?im)^\s*GO\s*(?:--.*)?$'
    )

    $connection = New-Connection $database
    try {
        $connection.Open()
        $batchNumber = 0
        foreach ($batch in $batches) {
            if ([string]::IsNullOrWhiteSpace($batch)) { continue }
            $batchNumber++
            $command = $connection.CreateCommand()
            $command.CommandTimeout = 120
            $command.CommandText = $batch
            try {
                [void]$command.ExecuteNonQuery()
            }
            catch {
                $details = @($_.Exception.InnerException.Errors | ForEach-Object {
                    "SQL $($_.Number), dòng $($_.LineNumber): $($_.Message)"
                }) -join ' | '
                throw "Lỗi file $path, batch $batchNumber`: $details"
            }
        }
    }
    finally {
        $connection.Dispose()
    }
}

function Add-Result([string]$case, [bool]$passed, [string]$detail) {
    $results.Add([pscustomobject]@{
        Testcase = $case
        TrangThai = if ($passed) { 'PASS' } else { 'FAIL' }
        ChiTiet = $detail
    })
}

function Test-Equal([string]$case, $expected, $actual) {
    $passed = [object]::Equals($expected, $actual)
    Add-Result $case $passed "Kỳ vọng: $expected; thực tế: $actual"
}

function Test-Decimal([string]$case, [decimal]$expected, $actual, [decimal]$tolerance = 0.01) {
    $number = [Convert]::ToDecimal($actual)
    $passed = [Math]::Abs($number - $expected) -le $tolerance
    Add-Result $case $passed "Kỳ vọng: $expected; thực tế: $number"
}

function Test-True([string]$case, [bool]$condition, [string]$detail) {
    Add-Result $case $condition $detail
}

try {
    foreach ($database in $databases.Values) {
        Invoke-NonQuery 'master' "CREATE DATABASE [$database];"
    }

    $deAnScripts = @(
        @('DATABASE\Bai_01\sp_GiaiPTB1.sql', 'QL_DeAn'),
        @('DATABASE\Bai_02\fn_GiaiPTB2.sql', 'QL_DeAn'),
        @('DATABASE\Bai_04\fn_TinhTuoi.sql', 'QL_DeAn'),
        @('DATABASE\Nhom_1_CSDL_ToanHoc\02_Testcase.sql', 'QL_DeAn'),
        @('DATABASE\Nhom_3_CSDL_DeAn\01_TaoBang_NhapDuLieu.sql', 'QL_DeAn'),
        @('DATABASE\Bai_07\Functions_DeAn.sql', 'QL_DeAn'),
        @('DATABASE\Bai_08\02_Functions.sql', 'QL_DeAn'),
        @('DATABASE\Nhom_3_CSDL_DeAn\02_Testcase.sql', 'QL_DeAn')
    )
    foreach ($item in $deAnScripts) {
        Invoke-ScriptFile $databases.DeAn (Join-Path $root $item[0]) $item[1]
    }

    $libraryScripts = @(
        @('DATABASE\Nhom_2_CSDL_ThuVien\01_TaoBang_NhapDuLieu.sql', 'QL_ThuVien'),
        @('DATABASE\Bai_03\sp_ThongTinDausach.sql', 'QL_ThuVien'),
        @('DATABASE\Bai_05\StoredProcedures_ThuVien.sql', 'QL_ThuVien'),
        @('DATABASE\Bai_06\Triggers_ThuVien.sql', 'QL_ThuVien'),
        @('DATABASE\Nhom_2_CSDL_ThuVien\02_Testcase.sql', 'QL_ThuVien')
    )
    foreach ($item in $libraryScripts) {
        Invoke-ScriptFile $databases.ThuVien (Join-Path $root $item[0]) $item[1]
    }

    $garageScripts = @(
        @('DATABASE\Nhom_4_CSDL_Gara\01_TaoBang_NhapDuLieu.sql', 'QL_Gara'),
        @('DATABASE\Bai_09\02_Functions.sql', 'QL_Gara'),
        @('DATABASE\Nhom_4_CSDL_Gara\03_Testcase.sql', 'QL_Gara')
    )
    foreach ($item in $garageScripts) {
        Invoke-ScriptFile $databases.Gara (Join-Path $root $item[0]) $item[1]
    }

    $schoolScripts = @(
        @('DATABASE\Nhom_5_CSDL_TruongPhoThong\01_TaoBang_NhapDuLieu.sql', 'QL_TruongPhoThong'),
        @('DATABASE\Bai_10\02_Functions.sql', 'QL_TruongPhoThong'),
        @('DATABASE\Nhom_5_CSDL_TruongPhoThong\03_Testcase.sql', 'QL_TruongPhoThong')
    )
    foreach ($item in $schoolScripts) {
        Invoke-ScriptFile $databases.Truong (Join-Path $root $item[0]) $item[1]
    }

    $linear = Invoke-Scalar $databases.DeAn "DECLARE @r table(KetQua nvarchar(255)); INSERT @r EXEC dbo.sp_GiaiPTB1 2,-4; SELECT KetQua FROM @r;"
    Test-Equal 'B1-nghiem-duy-nhat' 'Phương trình có nghiệm: x = 2' ([string]$linear)
    Test-Equal 'B1-vo-nghiem' 'Phương trình vô nghiệm' ([string](Invoke-Scalar $databases.DeAn "DECLARE @r table(KetQua nvarchar(255)); INSERT @r EXEC dbo.sp_GiaiPTB1 0,5; SELECT KetQua FROM @r;"))
    Test-Equal 'B1-vo-so-nghiem' 'Phương trình có vô số nghiệm' ([string](Invoke-Scalar $databases.DeAn "DECLARE @r table(KetQua nvarchar(255)); INSERT @r EXEC dbo.sp_GiaiPTB1 0,0; SELECT KetQua FROM @r;"))

    Test-True 'B2-hai-nghiem' ([string](Invoke-Scalar $databases.DeAn 'SELECT dbo.fn_GiaiPTB2(1,-3,2);')).Contains('2 nghiệm') 'Delta dương trả hai nghiệm.'
    Test-True 'B2-delta-am-sat-0' ([string](Invoke-Scalar $databases.DeAn 'SELECT dbo.fn_GiaiPTB2(1,2,1.00000001);')).Contains('vô nghiệm') 'Delta âm rất nhỏ vẫn vô nghiệm.'
    Test-True 'B2-suy-bien-vo-nghiem' ([string](Invoke-Scalar $databases.DeAn 'SELECT dbo.fn_GiaiPTB2(0,0,5);')).Contains('vô nghiệm') 'a=b=0,c khác 0 được xử lý.'
    $irrational = [string](Invoke-Scalar $databases.DeAn 'SELECT dbo.fn_GiaiPTB2(1,0,-2);')
    Test-True 'B2-nghiem-vo-ti-du-do-chinh-xac' $irrational.Contains('1.41421356') $irrational

    Test-Equal 'B4-sinh-nhat-hom-nay' 20 ([int](Invoke-Scalar $databases.DeAn 'SELECT dbo.fn_TinhTuoi(DATEADD(YEAR,-20,CAST(GETDATE() AS date)));'))
    Test-Equal 'B4-chua-den-sinh-nhat' 19 ([int](Invoke-Scalar $databases.DeAn 'SELECT dbo.fn_TinhTuoi(DATEADD(DAY,1,DATEADD(YEAR,-20,CAST(GETDATE() AS date))));'))
    Test-Equal 'B4-da-qua-sinh-nhat' 20 ([int](Invoke-Scalar $databases.DeAn 'SELECT dbo.fn_TinhTuoi(DATEADD(DAY,-1,DATEADD(YEAR,-20,CAST(GETDATE() AS date))));'))
    Test-True 'B4-ngay-tuong-lai' ((Invoke-Scalar $databases.DeAn 'SELECT dbo.fn_TinhTuoi(DATEADD(DAY,1,CAST(GETDATE() AS date)));') -is [DBNull]) 'Ngày sinh tương lai trả SQL NULL.'
    Test-True 'B4-ngay-nhuan' ([int](Invoke-Scalar $databases.DeAn "SELECT dbo.fn_TinhTuoi(CAST('2000-02-29' AS date));") -ge 0) 'Ngày nhuận hợp lệ trả tuổi không âm.'
    Test-True 'B4-null' ((Invoke-Scalar $databases.DeAn 'SELECT dbo.fn_TinhTuoi(NULL);') -is [DBNull]) 'NULL trả SQL NULL.'

    $book = Invoke-Table $databases.ThuVien "EXEC dbo.sp_ThongTinDauSach 'ISBN001';"
    Test-Equal 'B3-dem-cuon-co-san' 3 ([int]$book.Rows[0]['SoLuongChuaMuon'])
    Test-Equal 'B5a-doc-gia-nguoi-lon' 'Người lớn' ([string](Invoke-Table $databases.ThuVien "EXEC dbo.sp_ThongtinDocGia 'DG001';").Rows[0]['LoaiDocGia'])
    Test-Equal 'B5a-doc-gia-tre-em' 'Trẻ em' ([string](Invoke-Table $databases.ThuVien "EXEC dbo.sp_ThongtinDocGia 'TE001';").Rows[0]['LoaiDocGia'])
    $partialIsbnRejected = $false
    try { [void](Invoke-Table $databases.ThuVien "EXEC dbo.sp_ThongTinDauSach 'ISBN00';") }
    catch { $partialIsbnRejected = $_.Exception.Message.Contains('Không tìm thấy đầu sách') }
    Test-True 'B3-ma-gan-dung-khong-duoc-chon-ngam' $partialIsbnRejected 'ISBN00 không được tự chọn ISBN001.'
    $partialReaderRejected = $false
    try { [void](Invoke-Table $databases.ThuVien "EXEC dbo.sp_ThongtinDocGia 'DG00';") }
    catch { $partialReaderRejected = $_.Exception.Message.Contains('Không tìm thấy độc giả') }
    Test-True 'B5a-ma-gan-dung-khong-duoc-chon-ngam' $partialReaderRejected 'DG00 không được tự chọn DG001.'
    Test-Equal 'B5c-nguoi-lon-dang-muon' 5 ([int](Invoke-Table $databases.ThuVien 'EXEC dbo.sp_ThongtinNguoilonDangmuon;').Rows.Count)
    Test-Equal 'B5e-cap-nguoi-lon-tre-em' 3 ([int](Invoke-Table $databases.ThuVien 'EXEC dbo.sp_DocGiaCoTreEmMuon;').Rows.Count)

    $libraryConnection = New-Connection $databases.ThuVien
    try {
        $libraryConnection.Open()
        $transaction = $libraryConnection.BeginTransaction()
        try {
            $setup = $libraryConnection.CreateCommand()
            $setup.Transaction = $transaction
            $setup.CommandText = "INSERT dbo.Dausach(isbn,ma_tuasach,ngonngu,bia,trangthai) VALUES('ISBN099','TS01',N'Tiếng Việt',N'Bìa mềm',N'Đang phục vụ');"
            [void]$setup.ExecuteNonQuery()
            $emptyBookQuery = $libraryConnection.CreateCommand()
            $emptyBookQuery.Transaction = $transaction
            $emptyBookQuery.CommandText = "EXEC dbo.sp_ThongTinDauSach 'ISBN099';"
            $emptyBook = [System.Data.DataTable]::new()
            $emptyBook.Load($emptyBookQuery.ExecuteReader())
            Test-Equal 'B3-dau-sach-chua-co-cuon' 0 ([int]$emptyBook.Rows[0]['SoLuongChuaMuon'])

            $update = $libraryConnection.CreateCommand()
            $update.Transaction = $transaction
            $update.CommandText = "UPDATE dbo.Muon SET ngay_hethan=CAST(GETDATE() AS date) WHERE isbn='ISBN001' AND ma_cuonsach='CS004';"
            [void]$update.ExecuteNonQuery()
            $query = $libraryConnection.CreateCommand()
            $query.Transaction = $transaction
            $query.CommandText = 'EXEC dbo.sp_ThongtinNguoilonQuahan;'
            $todayTable = [System.Data.DataTable]::new()
            $todayTable.Load($query.ExecuteReader())
            $todayCount = @($todayTable.Select("ISBN='ISBN001' AND MaCuonSach='CS004'")).Count
            Test-Equal 'B5d-den-han-hom-nay' 0 $todayCount

            $update.CommandText = "UPDATE dbo.Muon SET ngay_hethan=DATEADD(DAY,-1,CAST(GETDATE() AS date)) WHERE isbn='ISBN001' AND ma_cuonsach='CS004';"
            [void]$update.ExecuteNonQuery()
            $yesterdayTable = [System.Data.DataTable]::new()
            $yesterdayTable.Load($query.ExecuteReader())
            $yesterdayCount = @($yesterdayTable.Select("ISBN='ISBN001' AND MaCuonSach='CS004' AND SoNgayQuaHan=1")).Count
            Test-Equal 'B5d-tre-mot-ngay' 1 $yesterdayCount

            $duplicate = $libraryConnection.CreateCommand()
            $duplicate.Transaction = $transaction
            $duplicate.CommandText = "INSERT dbo.Muon(isbn,ma_cuonsach,ma_DocGia,ngay_muon,ngay_hethan) VALUES('ISBN001','CS004','DG002',CAST(GETDATE() AS date),DATEADD(DAY,14,CAST(GETDATE() AS date)));"
            $blocked = $false
            try { [void]$duplicate.ExecuteNonQuery() } catch [System.Data.SqlClient.SqlException] { $blocked = $true }
            Test-True 'B6-khong-muon-trung-cuon' $blocked 'UNIQUE (isbn,ma_cuonsach) chặn lượt mượn thứ hai.'

            $borrow = $libraryConnection.CreateCommand()
            $borrow.Transaction = $transaction
            $borrow.CommandText = "INSERT dbo.Muon(isbn,ma_cuonsach,ma_DocGia,ngay_muon,ngay_hethan) VALUES('ISBN006','CS001','DG008',CAST(GETDATE() AS date),DATEADD(DAY,14,CAST(GETDATE() AS date)));"
            [void]$borrow.ExecuteNonQuery()
            $state = $libraryConnection.CreateCommand()
            $state.Transaction = $transaction
            $state.CommandText = "SELECT tinhtrang FROM dbo.Cuonsach WHERE isbn='ISBN006' AND ma_cuonsach='CS001';"
            Test-Equal 'B6.2-muon-doi-tinh-trang-cuon' 'Đang mượn' ([string]$state.ExecuteScalar())
            $state.CommandText = "SELECT trangthai FROM dbo.Dausach WHERE isbn='ISBN006';"
            Test-Equal 'B6.3-het-cuon-doi-trang-thai-dau-sach' 'Ngừng phục vụ' ([string]$state.ExecuteScalar())

            $returnBook = $libraryConnection.CreateCommand()
            $returnBook.Transaction = $transaction
            $returnBook.CommandText = "DELETE dbo.Muon WHERE isbn='ISBN006' AND ma_cuonsach='CS001' AND ma_DocGia='DG008';"
            [void]$returnBook.ExecuteNonQuery()
            $state.CommandText = "SELECT tinhtrang FROM dbo.Cuonsach WHERE isbn='ISBN006' AND ma_cuonsach='CS001';"
            Test-Equal 'B6.1-tra-doi-tinh-trang-cuon' 'Có sẵn' ([string]$state.ExecuteScalar())
            $state.CommandText = "SELECT trangthai FROM dbo.Dausach WHERE isbn='ISBN006';"
            Test-Equal 'B6.3-tra-sach-doi-trang-thai-dau-sach' 'Đang phục vụ' ([string]$state.ExecuteScalar())

            $state.CommandText = "UPDATE dbo.Cuonsach SET tinhtrang=N'Đang mượn' WHERE isbn='ISBN006' AND ma_cuonsach='CS001';"
            [void]$state.ExecuteNonQuery()
            $inconsistentBorrow = $libraryConnection.CreateCommand()
            $inconsistentBorrow.Transaction = $transaction
            $inconsistentBorrow.CommandText = "INSERT dbo.Muon(isbn,ma_cuonsach,ma_DocGia,ngay_muon,ngay_hethan) VALUES('ISBN006','CS001','DG008',CAST(GETDATE() AS date),DATEADD(DAY,14,CAST(GETDATE() AS date)));"
            $blockedInconsistentBorrow = $false
            try { [void]$inconsistentBorrow.ExecuteNonQuery() }
            catch [System.Data.SqlClient.SqlException] { $blockedInconsistentBorrow = $_.Exception.Number -eq 50001 }
            Test-True 'B6-ADD-02-tinh-trang-khong-khop-bi-chan' $blockedInconsistentBorrow 'Cuốn đang mượn không thể tạo phiếu mới khi chưa có phiếu cũ.'
        }
        finally {
            if ($transaction.Connection) { $transaction.Rollback() }
            $transaction.Dispose()
        }
    }
    finally {
        $libraryConnection.Dispose()
    }

    $messageConnection = New-Connection $databases.ThuVien
    $messages = [System.Collections.Generic.List[string]]::new()
    $messageHandler = [System.Data.SqlClient.SqlInfoMessageEventHandler]{
        param($sender, $eventArgs)
        $messages.Add($eventArgs.Message)
    }
    try {
        $messageConnection.add_InfoMessage($messageHandler)
        $messageConnection.Open()
        $messageTransaction = $messageConnection.BeginTransaction()
        try {
            $messageCommand = $messageConnection.CreateCommand()
            $messageCommand.Transaction = $messageTransaction
            $messageCommand.CommandText = "INSERT dbo.Tuasach VALUES('TS99',N'Tựa thử',N'Tác giả thử',N'Tóm tắt'); UPDATE dbo.Tuasach SET tuasach=N'Tựa đã sửa',tacgia=N'Tác giả đã sửa' WHERE ma_tuasach='TS99';"
            [void]$messageCommand.ExecuteNonQuery()
            $allMessages = $messages -join ' | '
            Test-True 'B6.4-thong-bao-them' $allMessages.Contains('Đã thêm mới tựa sách') $allMessages
            Test-True 'B6.4-thong-bao-sua-tua' $allMessages.Contains('Đã sửa tựa sách') $allMessages
            Test-True 'B6.4-thong-bao-sua-tac-gia' $allMessages.Contains('Đã sửa tên tác giả') $allMessages
        }
        finally {
            if ($messageTransaction.Connection) { $messageTransaction.Rollback() }
            $messageTransaction.Dispose()
        }
    }
    finally {
        $messageConnection.remove_InfoMessage($messageHandler)
        $messageConnection.Dispose()
    }

    Test-Equal 'DeAn-sau-bang-goc' 6 ([int](Invoke-Scalar $databases.DeAn "SELECT COUNT(*) FROM sys.tables WHERE name IN('PHONGBAN','NHANVIEN','DEAN','PHANCONG','THANNHAN','DIADIEM_PHG');"))
    Test-Equal 'DeAn-khong-con-bang-luong-cu' 0 ([int](Invoke-Scalar $databases.DeAn "SELECT COUNT(*) FROM sys.tables WHERE name='LUONG';"))
    Test-Equal 'DeAn-khong-con-view-chuan-hoa' 0 ([int](Invoke-Scalar $databases.DeAn "SELECT COUNT(*) FROM sys.views WHERE name IN('B7_PhongBan','B7_NhanVien','B7_DeAn','B7_PhanCong','B7_ThanNhan');"))
    Test-Equal 'DeAn-muoi-chin-nhan-vien' 19 ([int](Invoke-Scalar $databases.DeAn 'SELECT COUNT(*) FROM dbo.NHANVIEN;'))
    Test-Equal 'DeAn-muoi-chin-dong-bang-luong' 19 ([int](Invoke-Scalar $databases.DeAn 'SELECT COUNT(*) FROM dbo.BANGLUONG;'))
    Test-Equal 'DeAn-bang-luong-du-muoi-mot-cot' 11 ([int](Invoke-Scalar $databases.DeAn "SELECT COUNT(*) FROM sys.columns WHERE object_id=OBJECT_ID(N'dbo.BANGLUONG');"))
    Test-Decimal 'DeAn-bang-luong-NV01-thu-nhap' 33000 (Invoke-Scalar $databases.DeAn "SELECT TongThuNhap FROM dbo.BANGLUONG WHERE MaNV='NV01';")
    Test-Decimal 'DeAn-bang-luong-NV01-trung-binh-phong' 35666.67 (Invoke-Scalar $databases.DeAn "SELECT LuongTBPhong FROM dbo.BANGLUONG WHERE MaNV='NV01';")
    Test-Equal 'DeAn-nhan-vien-co-du-muoi-cot' 10 ([int](Invoke-Scalar $databases.DeAn "SELECT COUNT(*) FROM sys.columns WHERE object_id=OBJECT_ID(N'dbo.NHANVIEN');"))
    Test-Equal 'DeAn-chin-khoa-ngoai' 9 ([int](Invoke-Scalar $databases.DeAn "SELECT COUNT(*) FROM sys.foreign_keys WHERE name IN('FK_NHANVIEN_NQL','FK_NHANVIEN_PHG','FK_PHONGBAN_TRPHG','FK_DEAN_PHONGBAN','FK_DIADIEM_PHG_PHONGBAN','FK_THANNHAN_NHANVIEN','FK_PHANCONG_NHANVIEN','FK_PHANCONG_DEAN','FK_BANGLUONG_NHANVIEN');"))
    Test-Equal 'DeAn-moi-nhan-vien-mot-dong-bang-luong' 0 ([int](Invoke-Scalar $databases.DeAn 'SELECT COUNT(*) FROM dbo.NHANVIEN n LEFT JOIN dbo.BANGLUONG b ON b.MaNV=n.MaNV WHERE b.MaNV IS NULL OR ISNULL(b.LuongCoBan,-1)<>ISNULL(n.Luong,-1);'))
    Test-Equal 'DeAn-cap-nhat-nhan-vien-lam-moi-bang-luong' 1 ([int](Invoke-Scalar $databases.DeAn "BEGIN TRY BEGIN TRAN; UPDATE dbo.NHANVIEN SET Luong=33000 WHERE MaNV='NV01'; EXEC dbo.sp_TinhVaCapNhatBangLuong @XuatKetQua=0; DECLARE @ok int=CASE WHEN (SELECT LuongCoBan FROM dbo.BANGLUONG WHERE MaNV='NV01')=33000 AND dbo.fn_B7_LuongTrungBinhPhong('01')=36000 THEN 1 ELSE 0 END; IF @@TRANCOUNT>0 ROLLBACK; SELECT @ok; END TRY BEGIN CATCH IF @@TRANCOUNT>0 ROLLBACK; SELECT 0; END CATCH;"))
    Test-Equal 'DeAn-them-nhan-vien-lam-moi-bang-luong' 1 ([int](Invoke-Scalar $databases.DeAn "BEGIN TRY BEGIN TRAN; INSERT dbo.NHANVIEN(MaNV,TenNV,Luong,Phg) VALUES('NV98',N'Thử',30000,'01'); EXEC dbo.sp_TinhVaCapNhatBangLuong @XuatKetQua=0; DECLARE @ok int=CASE WHEN (SELECT LuongCoBan FROM dbo.BANGLUONG WHERE MaNV='NV98')=30000 THEN 1 ELSE 0 END; IF @@TRANCOUNT>0 ROLLBACK; SELECT @ok; END TRY BEGIN CATCH IF @@TRANCOUNT>0 ROLLBACK; SELECT 0; END CATCH;"))
    Test-Equal 'DeAn-luong-null-giu-nguyen' 1 ([int](Invoke-Scalar $databases.DeAn "SELECT CASE WHEN (SELECT LuongCoBan FROM dbo.BANGLUONG WHERE MaNV='NV17') IS NULL AND (SELECT TongThuNhap FROM dbo.BANGLUONG WHERE MaNV='NV17')=0 THEN 1 ELSE 0 END;"))
    Test-Equal 'DeAn-function-doc-bang-luong' 1 ([int](Invoke-Scalar $databases.DeAn "BEGIN TRY BEGIN TRAN; UPDATE dbo.BANGLUONG SET LuongCoBan=33000 WHERE MaNV='NV01'; UPDATE dbo.BANGLUONG SET LuongCoBan=25001 WHERE MaNV='NV19'; DECLARE @ok int=CASE WHEN dbo.fn_B7_LuongTrungBinhPhong('01')=36000 AND dbo.fn_B7_TongLuongNhanVienDeAn('NV01','01')=16229.51 AND (SELECT SoNhanVienLuongTren25000 FROM dbo.fn_B8_PhongNhieuNhanVienLuongCao() WHERE MaPhg='03')=1 THEN 1 ELSE 0 END; IF @@TRANCOUNT>0 ROLLBACK; SELECT @ok; END TRY BEGIN CATCH IF @@TRANCOUNT>0 ROLLBACK; SELECT 0; END CATCH;"))
    Test-Decimal 'B7.1-luong-trung-binh' 35666.67 (Invoke-Scalar $databases.DeAn "SELECT dbo.fn_B7_LuongTrungBinhPhong('01');")
    Test-Decimal 'B7.1-phong-co-luong-0-null-va-25000' 16333.33 (Invoke-Scalar $databases.DeAn "SELECT dbo.fn_B7_LuongTrungBinhPhong('03');")
    Test-Decimal 'B7.1-phong-dung-30000' 30000 (Invoke-Scalar $databases.DeAn "SELECT dbo.fn_B7_LuongTrungBinhPhong('04');")
    Test-Decimal 'B7.1-phong-rong' 0 (Invoke-Scalar $databases.DeAn "SELECT dbo.fn_B7_LuongTrungBinhPhong('00');")
    Test-Decimal 'B7.1-phong-khong-ton-tai' 0 (Invoke-Scalar $databases.DeAn "SELECT dbo.fn_B7_LuongTrungBinhPhong('99');")
    Test-Decimal 'B7.2-phan-bo-theo-ty-trong' 15737.70 (Invoke-Scalar $databases.DeAn "SELECT dbo.fn_B7_TongLuongNhanVienDeAn('NV01','01');")
    Test-Decimal 'B7.2-khong-tham-gia' 0 (Invoke-Scalar $databases.DeAn "SELECT dbo.fn_B7_TongLuongNhanVienDeAn('NV01','03');")
    Test-Decimal 'B7.2-tong-gio-0' 0 (Invoke-Scalar $databases.DeAn "SELECT dbo.fn_B7_TongLuongNhanVienDeAn('NV12','06');")
    Test-Decimal 'B7.2-luong-null' 0 (Invoke-Scalar $databases.DeAn "SELECT dbo.fn_B7_TongLuongNhanVienDeAn('NV17','06');")
    Test-Decimal 'B7.3-tong-trung-binh-phong' 177500 (Invoke-Scalar $databases.DeAn 'SELECT dbo.fn_B7_TongLuongTrungBinhCacPhong();')
    Test-Equal 'B7.4-cac-khoang-bien' 0 ([int](Invoke-Scalar $databases.DeAn "SELECT COUNT(*) FROM (VALUES (CAST(NULL AS decimal(10,2)),0),(-1,0),(29.99,0),(30,500),(60,500),(60.01,1000),(99.99,1000),(100,1200),(149.99,1200),(150,1600)) AS x(Gio,Thuong) WHERE dbo.fn_B7_TienThuong(x.Gio)<>x.Thuong;"))
    Test-Equal 'B7.5-bay-phong-ke-ca-phong-0-de-an' 7 ([int](Invoke-Scalar $databases.DeAn 'SELECT COUNT(*) FROM dbo.fn_B7_SoDeAnTheoPhong();'))
    Test-Equal 'B7.5-phong-rong-0-de-an' 0 ([int](Invoke-Scalar $databases.DeAn "SELECT SoDeAn FROM dbo.fn_B7_SoDeAnTheoPhong() WHERE MaPhg='00';"))
    Test-Equal 'B7.5-phong-hai-de-an' 2 ([int](Invoke-Scalar $databases.DeAn "SELECT SoDeAn FROM dbo.fn_B7_SoDeAnTheoPhong() WHERE MaPhg='01';"))
    Test-Equal 'B7.6-hai-cach-giong-nhau' 0 ([int](Invoke-Scalar $databases.DeAn 'SELECT (SELECT COUNT(*) FROM (SELECT * FROM dbo.fn_B7_ThongTinNhanVien_Inline() EXCEPT SELECT * FROM dbo.fn_B7_ThongTinNhanVien_Multi()) a) + (SELECT COUNT(*) FROM (SELECT * FROM dbo.fn_B7_ThongTinNhanVien_Multi() EXCEPT SELECT * FROM dbo.fn_B7_ThongTinNhanVien_Inline()) b);'))
    Test-Equal 'B7.6-muoi-chin-nhan-vien' 19 ([int](Invoke-Scalar $databases.DeAn 'SELECT COUNT(*) FROM dbo.fn_B7_ThongTinNhanVien_Inline();'))
    Test-Equal 'B7.6-nguoi-than-0-1-2' 3 ([int](Invoke-Scalar $databases.DeAn "SELECT COUNT(*) FROM dbo.fn_B7_ThongTinNhanVien_Inline() WHERE (MaNV='NV01' AND NguoiThan LIKE N'%Nguyễn Minh%' AND NguoiThan LIKE N'%Hoàng Lan%') OR (MaNV='NV03' AND NguoiThan LIKE N'%Lê Hà%') OR (MaNV='NV06' AND NguoiThan IS NULL);"))
    Test-Equal 'B7.6-ho-ten-co-ten-lot' 'Ninh Thị Yến' ([string](Invoke-Scalar $databases.DeAn "SELECT HoTen FROM dbo.fn_B7_ThongTinNhanVien_Inline() WHERE MaNV='NV19';"))
    Test-Decimal 'B7.6-luong-tb-phong' 35666.67 (Invoke-Scalar $databases.DeAn "SELECT TongLuongTB FROM dbo.fn_B7_ThongTinNhanVien_Inline() WHERE MaNV='NV01';")
    Test-Decimal 'B7.6-nhan-vien-khong-phong' 0 (Invoke-Scalar $databases.DeAn "SELECT TongLuongTB FROM dbo.fn_B7_ThongTinNhanVien_Inline() WHERE MaNV='NV06';")

    Test-Equal 'B8.1-du-an-hon-2-nv' 3 ([int](Invoke-Scalar $databases.DeAn 'SELECT COUNT(*) FROM dbo.fn_B8_DeAnNhieuNhanVien();'))
    Test-Equal 'B8.1-du-an-dung-3-nv' 3 ([int](Invoke-Scalar $databases.DeAn "SELECT SoLuongNhanVien FROM dbo.fn_B8_DeAnNhieuNhanVien() WHERE MaDA='05';"))
    Test-Equal 'B8.1-loai-du-an-2-nv' 0 ([int](Invoke-Scalar $databases.DeAn "SELECT COUNT(*) FROM dbo.fn_B8_DeAnNhieuNhanVien() WHERE MaDA IN('02','06');"))
    Test-Equal 'B8.1-loai-du-an-khong-phan-cong' 0 ([int](Invoke-Scalar $databases.DeAn "SELECT COUNT(*) FROM dbo.fn_B8_DeAnNhieuNhanVien() WHERE MaDA='04';"))
    Test-Equal 'B8.2-phong-hon-2-nv' 5 ([int](Invoke-Scalar $databases.DeAn 'SELECT COUNT(*) FROM dbo.fn_B8_PhongNhieuNhanVienLuongCao();'))
    Test-Equal 'B8.2-phong-3-nv-khong-ai-tren-25000' 0 ([int](Invoke-Scalar $databases.DeAn "SELECT SoNhanVienLuongTren25000 FROM dbo.fn_B8_PhongNhieuNhanVienLuongCao() WHERE MaPhg='03';"))
    Test-Equal 'B8.2-phong-05-ba-nv-tren-25000' 3 ([int](Invoke-Scalar $databases.DeAn "SELECT SoNhanVienLuongTren25000 FROM dbo.fn_B8_PhongNhieuNhanVienLuongCao() WHERE MaPhg='05';"))
    Test-Equal 'B8.2-loai-phong-2-nv' 0 ([int](Invoke-Scalar $databases.DeAn "SELECT COUNT(*) FROM dbo.fn_B8_PhongNhieuNhanVienLuongCao() WHERE MaPhg='04';"))
    Test-Equal 'B8.3-phong-avg-tren-30000' 3 ([int](Invoke-Scalar $databases.DeAn 'SELECT COUNT(*) FROM dbo.fn_B8_PhongLuongTBLon();'))
    Test-Equal 'B8.3-loai-avg-dung-30000' 0 ([int](Invoke-Scalar $databases.DeAn "SELECT COUNT(*) FROM dbo.fn_B8_PhongLuongTBLon() WHERE MaPhg='04';"))
    Test-Equal 'B8.4-phong-tren-30000-khong-co-nam' 0 ([int](Invoke-Scalar $databases.DeAn "SELECT SoLuongNhanVienNam FROM dbo.fn_B8_PhongLuongTBLon_Nam() WHERE MaPhg='06';"))
    Test-Equal 'B8.4-dem-nam' 2 ([int](Invoke-Scalar $databases.DeAn "SELECT SoLuongNhanVienNam FROM dbo.fn_B8_PhongLuongTBLon_Nam() WHERE MaPhg='01';"))
    Test-Equal 'B8.4-phong-02-mot-nam' 1 ([int](Invoke-Scalar $databases.DeAn "SELECT SoLuongNhanVienNam FROM dbo.fn_B8_PhongLuongTBLon_Nam() WHERE MaPhg='02';"))
    Test-Equal 'B8.5-moi-de-an' 6 ([int](Invoke-Scalar $databases.DeAn 'SELECT COUNT(*) FROM dbo.fn_B8_DeAnCoNhanVienPhong5();'))
    Test-Equal 'B8.5-ke-ca-de-an-khong-phan-cong' 0 ([int](Invoke-Scalar $databases.DeAn "SELECT SoNhanVienPhong5 FROM dbo.fn_B8_DeAnCoNhanVienPhong5() WHERE MaDA='04';"))
    Test-Equal 'B8.5-de-an-co-phan-cong-khong-co-nv-phong5' 0 ([int](Invoke-Scalar $databases.DeAn "SELECT SoNhanVienPhong5 FROM dbo.fn_B8_DeAnCoNhanVienPhong5() WHERE MaDA='06';"))
    Test-Equal 'B8.5-de-an-co-ba-nv-phong5' 3 ([int](Invoke-Scalar $databases.DeAn "SELECT SoNhanVienPhong5 FROM dbo.fn_B8_DeAnCoNhanVienPhong5() WHERE MaDA='05';"))
    Test-Equal 'B8.5-de-an-co-mot-nv-phong5' 1 ([int](Invoke-Scalar $databases.DeAn "SELECT SoNhanVienPhong5 FROM dbo.fn_B8_DeAnCoNhanVienPhong5() WHERE MaDA='01';"))

    Test-Equal 'B9.1-tho-khong-tham-gia' 3 ([int](Invoke-Scalar $databases.Gara 'SELECT COUNT(*) FROM dbo.fn_B9_ThoKhongThamGiaHopDong();'))
    Test-Decimal 'B9.2-con-no' 2500000 (Invoke-Scalar $databases.Gara "SELECT ConNo FROM dbo.fn_B9_HopDongDaThanhLyChuaDuTien() WHERE SoHD='HD04';")
    Test-Equal 'B9.3-truoc-31-12-2002' 2 ([int](Invoke-Scalar $databases.Gara 'SELECT COUNT(*) FROM dbo.fn_B9_HopDongCanHoanTatTruoc();'))
    Test-Equal 'B9.4-nhieu-viec-nhat' 'T02' ([string](Invoke-Scalar $databases.Gara 'SELECT MaTho FROM dbo.fn_B9_ThoNhieuCongViecNhat();'))
    Test-Decimal 'B9.5-tri-gia-cao-nhat' 5800000 (Invoke-Scalar $databases.Gara "SELECT TongTriGia FROM dbo.fn_B9_ThoTongTriGiaCaoNhat() WHERE MaTho='T02';")
    Test-Equal 'B9-rang-buoc-cung-nhom' 1 ([int](Invoke-Scalar $databases.Gara "BEGIN TRY BEGIN TRAN; INSERT dbo.B9_THO VALUES('T99',N'Thợ thử',2,'T01'); IF @@TRANCOUNT>0 ROLLBACK; SELECT 0; END TRY BEGIN CATCH IF @@TRANCOUNT>0 ROLLBACK; SELECT 1; END CATCH;"))
    Test-Equal 'B9-moi-nhom-mot-truong' 1 ([int](Invoke-Scalar $databases.Gara "BEGIN TRY BEGIN TRAN; UPDATE dbo.B9_THO SET NhomTruong='T02' WHERE MaTho='T02'; IF @@TRANCOUNT>0 ROLLBACK; SELECT 0; END TRY BEGIN CATCH DECLARE @ok int=CASE WHEN ERROR_PROCEDURE() IN(N'tg_B9_KiemTraDoiNhomTruong',N'dbo.tg_B9_KiemTraDoiNhomTruong') THEN 1 ELSE 0 END; IF @@TRANCOUNT>0 ROLLBACK; SELECT @ok; END CATCH;"))
    Test-Equal 'B9-chen-truong-thu-hai-bi-chan' 1 ([int](Invoke-Scalar $databases.Gara "BEGIN TRY BEGIN TRAN; INSERT dbo.B9_THO VALUES('T08',N'Trưởng thứ hai',1,'T08'); IF @@TRANCOUNT>0 ROLLBACK; SELECT 0; END TRY BEGIN CATCH DECLARE @ok int=CASE WHEN ERROR_PROCEDURE() IN(N'tg_B9_KiemTraNhomTruong',N'dbo.tg_B9_KiemTraNhomTruong') THEN 1 ELSE 0 END; IF @@TRANCOUNT>0 ROLLBACK; SELECT @ok; END CATCH;"))
    Test-Equal 'B9-doi-truong-ca-nhom-hop-le' 1 ([int](Invoke-Scalar $databases.Gara "BEGIN TRY BEGIN TRAN; UPDATE dbo.B9_THO SET NhomTruong='T02' WHERE Nhom=1; DECLARE @ok int=CASE WHEN (SELECT COUNT(*) FROM dbo.B9_THO WHERE Nhom=1 AND NhomTruong='T02')=3 THEN 1 ELSE 0 END; IF @@TRANCOUNT>0 ROLLBACK; SELECT @ok; END TRY BEGIN CATCH IF @@TRANCOUNT>0 ROLLBACK; SELECT 0; END CATCH;"))
    Test-Equal 'B9-tri-gia-bang-tong-cong-viec' 0 ([int](Invoke-Scalar $databases.Gara 'SELECT COUNT(*) FROM dbo.B9_HOPDONG h OUTER APPLY(SELECT SUM(TriGiaCV) AS Tong FROM dbo.B9_CHITIET_HD ct WHERE ct.SoHD=h.SoHD) ct WHERE h.TriGiaHD<>COALESCE(ct.Tong,0);'))
    Test-Decimal 'B9-hd02-tong-chi-tiet' 3300000 (Invoke-Scalar $databases.Gara "SELECT TriGiaHD FROM dbo.B9_HOPDONG WHERE SoHD='HD02';")
    Test-Equal 'B9-chan-sua-tri-gia-sai' 1 ([int](Invoke-Scalar $databases.Gara "BEGIN TRY BEGIN TRAN; UPDATE dbo.B9_HOPDONG SET TriGiaHD=TriGiaHD+1 WHERE SoHD='HD01'; IF @@TRANCOUNT>0 ROLLBACK; SELECT 0; END TRY BEGIN CATCH DECLARE @ok int=CASE WHEN ERROR_PROCEDURE() IN(N'tg_B9_KiemTraTriGiaHopDong',N'dbo.tg_B9_KiemTraTriGiaHopDong') THEN 1 ELSE 0 END; IF @@TRANCOUNT>0 ROLLBACK; SELECT @ok; END CATCH;"))
    Test-Equal 'B9-chan-them-hop-dong-sai-tong' 1 ([int](Invoke-Scalar $databases.Gara "BEGIN TRY BEGIN TRAN; INSERT dbo.B9_HOPDONG VALUES('HD98','2003-02-01','KH01','51Z-000.01',1,'2003-02-02',NULL); IF @@TRANCOUNT>0 ROLLBACK; SELECT 0; END TRY BEGIN CATCH DECLARE @ok int=CASE WHEN ERROR_PROCEDURE() IN(N'tg_B9_KiemTraTriGiaHopDong',N'dbo.tg_B9_KiemTraTriGiaHopDong') THEN 1 ELSE 0 END; IF @@TRANCOUNT>0 ROLLBACK; SELECT @ok; END CATCH;"))
    Test-Equal 'B9-sua-chi-tiet-cap-nhat-tong' 1 ([int](Invoke-Scalar $databases.Gara "BEGIN TRY BEGIN TRAN; UPDATE dbo.B9_CHITIET_HD SET TriGiaCV=TriGiaCV+100000 WHERE SoHD='HD02' AND MaCV='CV01'; DECLARE @ok int=CASE WHEN (SELECT TriGiaHD FROM dbo.B9_HOPDONG WHERE SoHD='HD02')=3400000 THEN 1 ELSE 0 END; IF @@TRANCOUNT>0 ROLLBACK; SELECT @ok; END TRY BEGIN CATCH IF @@TRANCOUNT>0 ROLLBACK; SELECT 0; END CATCH;"))
    Test-Equal 'B9-xoa-chi-tiet-cap-nhat-tong' 1 ([int](Invoke-Scalar $databases.Gara "BEGIN TRY BEGIN TRAN; DELETE dbo.B9_CHITIET_HD WHERE SoHD='HD02' AND MaCV='CV01'; DECLARE @ok int=CASE WHEN (SELECT TriGiaHD FROM dbo.B9_HOPDONG WHERE SoHD='HD02')=2500000 THEN 1 ELSE 0 END; IF @@TRANCOUNT>0 ROLLBACK; SELECT @ok; END TRY BEGIN CATCH IF @@TRANCOUNT>0 ROLLBACK; SELECT 0; END CATCH;"))
    Test-Equal 'B9-du-sau-khoa-chinh' 6 ([int](Invoke-Scalar $databases.Gara "SELECT COUNT(*) FROM sys.key_constraints WHERE type='PK' AND name IN('PK_B9_THO','PK_B9_CONGVIEC','PK_B9_KHACHHANG','PK_B9_HOPDONG','PK_B9_CHITIET_HD','PK_B9_PHIEUTHU');"))
    Test-Equal 'B9-du-sau-khoa-ngoai' 6 ([int](Invoke-Scalar $databases.Gara "SELECT COUNT(*) FROM sys.foreign_keys WHERE name IN('FK_B9_THO_NhomTruong','FK_B9_HOPDONG_KHACHHANG','FK_B9_CHITIET_HOPDONG','FK_B9_CHITIET_CONGVIEC','FK_B9_CHITIET_THO','FK_B9_PHIEUTHU_HOPDONG');"))
    Test-Equal 'B9-du-sau-check-tien' 4 ([int](Invoke-Scalar $databases.Gara "SELECT COUNT(*) FROM sys.check_constraints WHERE name IN('CK_B9_HOPDONG_TriGiaHD','CK_B9_CHITIET_TriGiaCV','CK_B9_CHITIET_KhoanTho','CK_B9_PHIEUTHU_SoTienThu');"))
    Test-Equal 'B9-du-sau-check-chuoi' 6 ([int](Invoke-Scalar $databases.Gara "SELECT COUNT(*) FROM sys.check_constraints WHERE name IN('CK_B9_THO_Chuoi','CK_B9_CONGVIEC_Chuoi','CK_B9_KHACHHANG_Chuoi','CK_B9_HOPDONG_Chuoi','CK_B9_CHITIET_Chuoi','CK_B9_PHIEUTHU_Chuoi');"))
    Test-Equal 'B9-khong-ep-unique-dien-thoai-va-noi-dung' 0 ([int](Invoke-Scalar $databases.Gara "SELECT COUNT(*) FROM sys.key_constraints WHERE name IN('UQ_B9_KHACHHANG_DienThoai','UQ_B9_CONGVIEC_NoiDungCV');"))
    Test-Equal 'B9-unique-xe-ngay' 1 ([int](Invoke-Scalar $databases.Gara "BEGIN TRY BEGIN TRAN; INSERT dbo.B9_HOPDONG VALUES('HD99','2002-12-01','KH01','51A-111.11',0,'2002-12-01',NULL); IF @@TRANCOUNT>0 ROLLBACK; SELECT 0; END TRY BEGIN CATCH DECLARE @ok int=CASE WHEN ERROR_NUMBER() IN(2601,2627) AND ERROR_MESSAGE() LIKE N'%UQ_B9_HOPDONG_Ngay_SoXe%' THEN 1 ELSE 0 END; IF @@TRANCOUNT>0 ROLLBACK; SELECT @ok; END CATCH;"))
    Test-Equal 'B9-phieu-thu-dung-chu' 1 ([int](Invoke-Scalar $databases.Gara "BEGIN TRY BEGIN TRAN; INSERT dbo.B9_PHIEUTHU VALUES('PT99','2002-12-10','HD01','KH02',N'Thử',1); IF @@TRANCOUNT>0 ROLLBACK; SELECT 0; END TRY BEGIN CATCH DECLARE @ok int=CASE WHEN ERROR_NUMBER()=547 AND ERROR_MESSAGE() LIKE N'%FK_B9_PHIEUTHU_HOPDONG%' THEN 1 ELSE 0 END; IF @@TRANCOUNT>0 ROLLBACK; SELECT @ok; END CATCH;"))
    Test-Equal 'B9-khoa-chinh-tho' 1 ([int](Invoke-Scalar $databases.Gara "BEGIN TRY BEGIN TRAN; INSERT dbo.B9_THO VALUES('T01',N'Trùng mã',1,'T01'); IF @@TRANCOUNT>0 ROLLBACK; SELECT 0; END TRY BEGIN CATCH DECLARE @ok int=CASE WHEN ERROR_NUMBER() IN(2601,2627) AND ERROR_MESSAGE() LIKE N'%PK_B9_THO%' THEN 1 ELSE 0 END; IF @@TRANCOUNT>0 ROLLBACK; SELECT @ok; END CATCH;"))
    Test-Equal 'B9-khoa-ngoai-tho-chi-tiet' 1 ([int](Invoke-Scalar $databases.Gara "BEGIN TRY BEGIN TRAN; INSERT dbo.B9_CHITIET_HD VALUES('HD02','CV05',100,'TNOEXIST',0); IF @@TRANCOUNT>0 ROLLBACK; SELECT 0; END TRY BEGIN CATCH DECLARE @ok int=CASE WHEN ERROR_NUMBER()=547 AND ERROR_MESSAGE() LIKE N'%FK_B9_CHITIET_THO%' THEN 1 ELSE 0 END; IF @@TRANCOUNT>0 ROLLBACK; SELECT @ok; END CATCH;"))
    Test-Equal 'B9-check-tri-gia-hop-dong' 1 ([int](Invoke-Scalar $databases.Gara "BEGIN TRY BEGIN TRAN; UPDATE dbo.B9_HOPDONG SET TriGiaHD=-1 WHERE SoHD='HD01'; IF @@TRANCOUNT>0 ROLLBACK; SELECT 0; END TRY BEGIN CATCH DECLARE @ok int=CASE WHEN ERROR_NUMBER()=547 AND ERROR_MESSAGE() LIKE N'%CK_B9_HOPDONG_TriGiaHD%' THEN 1 ELSE 0 END; IF @@TRANCOUNT>0 ROLLBACK; SELECT @ok; END CATCH;"))
    Test-Equal 'B9-check-khoan-tho' 1 ([int](Invoke-Scalar $databases.Gara "BEGIN TRY BEGIN TRAN; UPDATE dbo.B9_CHITIET_HD SET KhoanTho=-1 WHERE SoHD='HD02' AND MaCV='CV01'; IF @@TRANCOUNT>0 ROLLBACK; SELECT 0; END TRY BEGIN CATCH DECLARE @ok int=CASE WHEN ERROR_NUMBER()=547 AND ERROR_MESSAGE() LIKE N'%CK_B9_CHITIET_KhoanTho%' THEN 1 ELSE 0 END; IF @@TRANCOUNT>0 ROLLBACK; SELECT @ok; END CATCH;"))
    Test-Equal 'B9-check-tien-thu' 1 ([int](Invoke-Scalar $databases.Gara "BEGIN TRY BEGIN TRAN; INSERT dbo.B9_PHIEUTHU VALUES('PT98','2002-12-10','HD01','KH01',N'Thử',-1); IF @@TRANCOUNT>0 ROLLBACK; SELECT 0; END TRY BEGIN CATCH DECLARE @ok int=CASE WHEN ERROR_NUMBER()=547 AND ERROR_MESSAGE() LIKE N'%CK_B9_PHIEUTHU_SoTienThu%' THEN 1 ELSE 0 END; IF @@TRANCOUNT>0 ROLLBACK; SELECT @ok; END CATCH;"))
    Test-Equal 'B9-check-so-tien-thu-phai-duong' 1 ([int](Invoke-Scalar $databases.Gara "BEGIN TRY BEGIN TRAN; INSERT dbo.B9_PHIEUTHU VALUES('PT98','2002-12-10','HD01','KH01',N'Thử',0); IF @@TRANCOUNT>0 ROLLBACK; SELECT 0; END TRY BEGIN CATCH DECLARE @ok int=CASE WHEN ERROR_NUMBER()=547 AND ERROR_MESSAGE() LIKE N'%CK_B9_PHIEUTHU_SoTienThu%' THEN 1 ELSE 0 END; IF @@TRANCOUNT>0 ROLLBACK; SELECT @ok; END CATCH;"))
    Test-Equal 'B9-chan-phieu-thu-truoc-ngay-ky' 1 ([int](Invoke-Scalar $databases.Gara "BEGIN TRY BEGIN TRAN; INSERT dbo.B9_PHIEUTHU VALUES('PT98','2002-11-30','HD01','KH01',N'Thử',1); IF @@TRANCOUNT>0 ROLLBACK; SELECT 0; END TRY BEGIN CATCH DECLARE @ok int=CASE WHEN ERROR_NUMBER()=50000 AND ERROR_PROCEDURE() IN(N'tg_B9_KiemTraNgayPhieuThu',N'dbo.tg_B9_KiemTraNgayPhieuThu') AND ERROR_MESSAGE()=N'Ngày lập phiếu thu không được trước ngày ký hợp đồng.' THEN 1 ELSE 0 END; IF @@TRANCOUNT>0 ROLLBACK; SELECT @ok; END CATCH;"))
    Test-Equal 'B9-chan-sua-phieu-thu-truoc-ngay-ky' 1 ([int](Invoke-Scalar $databases.Gara "BEGIN TRY BEGIN TRAN; UPDATE dbo.B9_PHIEUTHU SET NgayLapPT='2002-11-30' WHERE SoPT='PT01'; IF @@TRANCOUNT>0 ROLLBACK; SELECT 0; END TRY BEGIN CATCH DECLARE @ok int=CASE WHEN ERROR_NUMBER()=50000 AND ERROR_PROCEDURE() IN(N'tg_B9_KiemTraNgayPhieuThu',N'dbo.tg_B9_KiemTraNgayPhieuThu') AND ERROR_MESSAGE()=N'Ngày lập phiếu thu không được trước ngày ký hợp đồng.' THEN 1 ELSE 0 END; IF @@TRANCOUNT>0 ROLLBACK; SELECT @ok; END CATCH;"))
    Test-Equal 'B9-chan-doi-ngay-ky-sau-ngay-thu' 1 ([int](Invoke-Scalar $databases.Gara "BEGIN TRY BEGIN TRAN; UPDATE dbo.B9_HOPDONG SET NgayHD='2002-12-06' WHERE SoHD='HD01'; IF @@TRANCOUNT>0 ROLLBACK; SELECT 0; END TRY BEGIN CATCH DECLARE @ok int=CASE WHEN ERROR_NUMBER()=50000 AND ERROR_PROCEDURE() IN(N'tg_B9_KiemTraNgayHopDongPhieuThu',N'dbo.tg_B9_KiemTraNgayHopDongPhieuThu') AND ERROR_MESSAGE()=N'Ngày lập phiếu thu không được trước ngày ký hợp đồng.' THEN 1 ELSE 0 END; IF @@TRANCOUNT>0 ROLLBACK; SELECT @ok; END CATCH;"))
    Test-Equal 'B9-cho-phep-thu-dung-ngay-ky' 1 ([int](Invoke-Scalar $databases.Gara "BEGIN TRY BEGIN TRAN; INSERT dbo.B9_PHIEUTHU VALUES('PT98','2002-12-10','HD02','KH02',N'Thử',1); DECLARE @ok int=CASE WHEN EXISTS(SELECT 1 FROM dbo.B9_PHIEUTHU WHERE SoPT='PT98' AND NgayLapPT='2002-12-10') THEN 1 ELSE 0 END; IF @@TRANCOUNT>0 ROLLBACK; SELECT @ok; END TRY BEGIN CATCH IF @@TRANCOUNT>0 ROLLBACK; SELECT 0; END CATCH;"))
    Test-Equal 'B9-tong-thu-khong-vuot-tri-gia' 0 ([int](Invoke-Scalar $databases.Gara 'SELECT COUNT(*) FROM dbo.B9_HOPDONG h OUTER APPLY(SELECT SUM(SoTienThu) AS Tong FROM dbo.B9_PHIEUTHU p WHERE p.SoHD=h.SoHD) p WHERE COALESCE(p.Tong,0)>h.TriGiaHD;'))
    Test-Equal 'B9-chan-them-phieu-thu-vuot' 1 ([int](Invoke-Scalar $databases.Gara "BEGIN TRY BEGIN TRAN; INSERT dbo.B9_PHIEUTHU VALUES('PT98','2002-12-24','HD04','KH01',N'Thử',2500001); IF @@TRANCOUNT>0 ROLLBACK; SELECT 0; END TRY BEGIN CATCH DECLARE @ok int=CASE WHEN ERROR_NUMBER()=50000 AND ERROR_PROCEDURE() IN(N'tg_B9_KiemTraTongPhieuThu',N'dbo.tg_B9_KiemTraTongPhieuThu') AND ERROR_MESSAGE()=N'Tổng tiền đã thu không được vượt trị giá hợp đồng.' THEN 1 ELSE 0 END; IF @@TRANCOUNT>0 ROLLBACK; SELECT @ok; END CATCH;"))
    Test-Equal 'B9-chan-them-nhieu-phieu-cung-vuot' 1 ([int](Invoke-Scalar $databases.Gara "BEGIN TRY BEGIN TRAN; INSERT dbo.B9_PHIEUTHU VALUES('PT98','2002-12-24','HD04','KH01',N'Thử',1500000),('PT99','2002-12-24','HD04','KH01',N'Thử',1500000); IF @@TRANCOUNT>0 ROLLBACK; SELECT 0; END TRY BEGIN CATCH DECLARE @ok int=CASE WHEN ERROR_NUMBER()=50000 AND ERROR_PROCEDURE() IN(N'tg_B9_KiemTraTongPhieuThu',N'dbo.tg_B9_KiemTraTongPhieuThu') THEN 1 ELSE 0 END; IF @@TRANCOUNT>0 ROLLBACK; SELECT @ok; END CATCH;"))
    Test-Equal 'B9-chan-sua-phieu-thu-vuot' 1 ([int](Invoke-Scalar $databases.Gara "BEGIN TRY BEGIN TRAN; UPDATE dbo.B9_PHIEUTHU SET SoTienThu=4500001 WHERE SoPT='PT03'; IF @@TRANCOUNT>0 ROLLBACK; SELECT 0; END TRY BEGIN CATCH DECLARE @ok int=CASE WHEN ERROR_NUMBER()=50000 AND ERROR_PROCEDURE() IN(N'tg_B9_KiemTraTongPhieuThu',N'dbo.tg_B9_KiemTraTongPhieuThu') THEN 1 ELSE 0 END; IF @@TRANCOUNT>0 ROLLBACK; SELECT @ok; END CATCH;"))
    Test-Equal 'B9-chan-chuyen-phieu-sang-hd-da-du' 1 ([int](Invoke-Scalar $databases.Gara "BEGIN TRY BEGIN TRAN; UPDATE dbo.B9_PHIEUTHU SET SoHD='HD01',MaKH='KH01' WHERE SoPT='PT04'; IF @@TRANCOUNT>0 ROLLBACK; SELECT 0; END TRY BEGIN CATCH DECLARE @ok int=CASE WHEN ERROR_NUMBER()=50000 AND ERROR_PROCEDURE() IN(N'tg_B9_KiemTraTongPhieuThu',N'dbo.tg_B9_KiemTraTongPhieuThu') THEN 1 ELSE 0 END; IF @@TRANCOUNT>0 ROLLBACK; SELECT @ok; END CATCH;"))
    Test-Equal 'B9-chan-giam-tri-gia-duoi-tien-da-thu' 1 ([int](Invoke-Scalar $databases.Gara "BEGIN TRY BEGIN TRAN; UPDATE dbo.B9_CHITIET_HD SET TriGiaCV=TriGiaCV-1 WHERE SoHD='HD01' AND MaCV='CV01'; IF @@TRANCOUNT>0 ROLLBACK; SELECT 0; END TRY BEGIN CATCH DECLARE @ok int=CASE WHEN ERROR_NUMBER()=50000 AND ERROR_PROCEDURE() IN(N'tg_B9_KiemTraTriGiaHopDong',N'dbo.tg_B9_KiemTraTriGiaHopDong') AND ERROR_MESSAGE()=N'Tổng tiền đã thu không được vượt trị giá hợp đồng.' THEN 1 ELSE 0 END; IF @@TRANCOUNT>0 ROLLBACK; SELECT @ok; END CATCH;"))
    Test-Equal 'B9-cho-phep-thu-dung-tri-gia' 1 ([int](Invoke-Scalar $databases.Gara "BEGIN TRY BEGIN TRAN; INSERT dbo.B9_PHIEUTHU VALUES('PT98','2002-12-24','HD04','KH01',N'Thử',2500000); DECLARE @ok int=CASE WHEN (SELECT SUM(SoTienThu) FROM dbo.B9_PHIEUTHU WHERE SoHD='HD04')=4500000 THEN 1 ELSE 0 END; IF @@TRANCOUNT>0 ROLLBACK; SELECT @ok; END TRY BEGIN CATCH IF @@TRANCOUNT>0 ROLLBACK; SELECT 0; END CATCH;"))
    Test-Equal 'B9-chan-ten-tho-rong' 1 ([int](Invoke-Scalar $databases.Gara "BEGIN TRY BEGIN TRAN; UPDATE dbo.B9_THO SET TenTho=N'   ' WHERE MaTho='T02'; IF @@TRANCOUNT>0 ROLLBACK; SELECT 0; END TRY BEGIN CATCH DECLARE @ok int=CASE WHEN ERROR_NUMBER()=547 AND ERROR_MESSAGE() LIKE N'%CK_B9_THO_Chuoi%' THEN 1 ELSE 0 END; IF @@TRANCOUNT>0 ROLLBACK; SELECT @ok; END CATCH;"))
    Test-Equal 'B9-chan-dien-thoai-co-dau-cong-o-giua' 1 ([int](Invoke-Scalar $databases.Gara "BEGIN TRY BEGIN TRAN; UPDATE dbo.B9_KHACHHANG SET DienThoai='090+2000001' WHERE MaKH='KH01'; IF @@TRANCOUNT>0 ROLLBACK; SELECT 0; END TRY BEGIN CATCH DECLARE @ok int=CASE WHEN ERROR_NUMBER()=547 AND ERROR_MESSAGE() LIKE N'%CK_B9_KH_DienThoai%' THEN 1 ELSE 0 END; IF @@TRANCOUNT>0 ROLLBACK; SELECT @ok; END CATCH;"))
    Test-Equal 'B9-cho-phep-dien-thoai-quoc-te' 1 ([int](Invoke-Scalar $databases.Gara "BEGIN TRY BEGIN TRAN; UPDATE dbo.B9_KHACHHANG SET DienThoai='+84902000001' WHERE MaKH='KH01'; DECLARE @ok int=CASE WHEN (SELECT DienThoai FROM dbo.B9_KHACHHANG WHERE MaKH='KH01')='+84902000001' THEN 1 ELSE 0 END; IF @@TRANCOUNT>0 ROLLBACK; SELECT @ok; END TRY BEGIN CATCH IF @@TRANCOUNT>0 ROLLBACK; SELECT 0; END CATCH;"))
    Test-Equal 'B9-cho-phep-trung-so-dien-thoai' 1 ([int](Invoke-Scalar $databases.Gara "BEGIN TRY BEGIN TRAN; UPDATE dbo.B9_KHACHHANG SET DienThoai='0902000001' WHERE MaKH='KH02'; DECLARE @ok int=CASE WHEN (SELECT COUNT(*) FROM dbo.B9_KHACHHANG WHERE DienThoai='0902000001')=2 THEN 1 ELSE 0 END; IF @@TRANCOUNT>0 ROLLBACK; SELECT @ok; END TRY BEGIN CATCH IF @@TRANCOUNT>0 ROLLBACK; SELECT 0; END CATCH;"))
    Test-Equal 'B9-cho-phep-trung-noi-dung-cong-viec' 1 ([int](Invoke-Scalar $databases.Gara "BEGIN TRY BEGIN TRAN; INSERT dbo.B9_CONGVIEC VALUES('CV98',N'Thay nhớt'); DECLARE @ok int=CASE WHEN EXISTS(SELECT 1 FROM dbo.B9_CONGVIEC WHERE MaCV='CV98') THEN 1 ELSE 0 END; IF @@TRANCOUNT>0 ROLLBACK; SELECT @ok; END TRY BEGIN CATCH IF @@TRANCOUNT>0 ROLLBACK; SELECT 0; END CATCH;"))
    Test-Equal 'B9-cho-phep-khoan-tho-hon-tri-gia-cv' 1 ([int](Invoke-Scalar $databases.Gara "BEGIN TRY BEGIN TRAN; UPDATE dbo.B9_CHITIET_HD SET KhoanTho=TriGiaCV+1 WHERE SoHD='HD02' AND MaCV='CV01'; DECLARE @ok int=CASE WHEN (SELECT KhoanTho FROM dbo.B9_CHITIET_HD WHERE SoHD='HD02' AND MaCV='CV01')=800001 THEN 1 ELSE 0 END; IF @@TRANCOUNT>0 ROLLBACK; SELECT @ok; END TRY BEGIN CATCH IF @@TRANCOUNT>0 ROLLBACK; SELECT 0; END CATCH;"))
    Test-Equal 'B9-update-nhieu-dong-hop-le' 1 ([int](Invoke-Scalar $databases.Gara "BEGIN TRY BEGIN TRAN; UPDATE dbo.B9_THO SET Nhom=4 WHERE MaTho IN('T01','T02','T03'); DECLARE @ok int=CASE WHEN (SELECT COUNT(*) FROM dbo.B9_THO WHERE MaTho IN('T01','T02','T03') AND Nhom=4)=3 THEN 1 ELSE 0 END; IF @@TRANCOUNT>0 ROLLBACK; SELECT @ok; END TRY BEGIN CATCH IF @@TRANCOUNT>0 ROLLBACK; SELECT 0; END CATCH;"))

    Test-Equal 'B10.2a-mon-tu-45-tiet' 3 ([int](Invoke-Scalar $databases.Truong 'SELECT COUNT(*) FROM dbo.fn_B10_GiaoVienMonTu45Tiet();'))
    Test-Equal 'B10.2b-gac-hk1' 7 ([int](Invoke-Scalar $databases.Truong 'SELECT COUNT(*) FROM dbo.fn_B10_GiaoVienGacThiHocKy();'))
    Test-Equal 'B10.2b-khong-lay-hk2' 0 ([int](Invoke-Scalar $databases.Truong "SELECT COUNT(*) FROM dbo.fn_B10_GiaoVienGacThiHocKy() WHERE MaGV IN('GV05','GV10');"))
    Test-Equal 'B10.2c-khong-gac-hk1' 3 ([int](Invoke-Scalar $databases.Truong 'SELECT COUNT(*) FROM dbo.fn_B10_GiaoVienKhongGacThiHocKy();'))
    Test-Equal 'B10.2c-co-gv-chi-gac-hk2' 2 ([int](Invoke-Scalar $databases.Truong "SELECT COUNT(*) FROM dbo.fn_B10_GiaoVienKhongGacThiHocKy() WHERE MaGV IN('GV05','GV10');"))
    Test-Equal 'B10.2d-lich-thi-van-hoc' 3 ([int](Invoke-Scalar $databases.Truong 'SELECT COUNT(*) FROM dbo.fn_B10_LichThiMon();'))
    Test-Equal 'B10.2d-chi-mon-van-hoc' 0 ([int](Invoke-Scalar $databases.Truong "SELECT COUNT(*) FROM dbo.fn_B10_LichThiMon() WHERE TenMH<>N'VĂN HỌC';"))
    Test-Equal 'B10.2e-gv-chu-nhiem-van' 5 ([int](Invoke-Scalar $databases.Truong 'SELECT COUNT(*) FROM dbo.fn_B10_BuoiGacThiCuaGiaoVienChuNhiemMon();'))
    Test-Equal 'B10.2e-chi-gv-chu-nhiem-van' 0 ([int](Invoke-Scalar $databases.Truong "SELECT COUNT(*) FROM dbo.fn_B10_BuoiGacThiCuaGiaoVienChuNhiemMon() WHERE MaGV<>'GV01';"))
    Test-Equal 'B10.2e-co-buoi-hk3' 1 ([int](Invoke-Scalar $databases.Truong "SELECT COUNT(*) FROM dbo.fn_B10_BuoiGacThiCuaGiaoVienChuNhiemMon() WHERE MaGV='GV01' AND HKY=3 AND Phg='P301';"))
    Test-Equal 'B10.1a-gv-khong-chu-nhiem-gac-nhieu-buoi' 2 ([int](Invoke-Scalar $databases.Truong "SELECT COUNT(*) FROM dbo.B10_PC_COI_THI WHERE MaGV='GV05' AND HKY=2;"))
    Test-Equal 'B10.1a-khong-gac-mon-chu-nhiem' 1 ([int](Invoke-Scalar $databases.Truong "BEGIN TRY BEGIN TRAN; INSERT dbo.B10_PC_COI_THI VALUES('GV01',1,'2026-05-10','07:30','P101'); IF @@TRANCOUNT>0 ROLLBACK; SELECT 0; END TRY BEGIN CATCH IF @@TRANCOUNT>0 ROLLBACK; SELECT 1; END CATCH;"))
    Test-Equal 'B10.1b-30-tiet-120-phut' 1 ([int](Invoke-Scalar $databases.Truong "BEGIN TRY BEGIN TRAN; INSERT dbo.B10_BUOITHI VALUES(3,'2099-01-01','07:30','Z01','MH02',119); IF @@TRANCOUNT>0 ROLLBACK; SELECT 0; END TRY BEGIN CATCH IF @@TRANCOUNT>0 ROLLBACK; SELECT 1; END CATCH;"))
    Test-Equal 'B10.1c-tu-45-tiet-150-phut' 1 ([int](Invoke-Scalar $databases.Truong "BEGIN TRY BEGIN TRAN; INSERT dbo.B10_BUOITHI VALUES(3,'2099-01-01','07:30','Z02','MH01',149); IF @@TRANCOUNT>0 ROLLBACK; SELECT 0; END TRY BEGIN CATCH IF @@TRANCOUNT>0 ROLLBACK; SELECT 1; END CATCH;"))
    Test-Equal 'B10-update-thoi-luong' 1 ([int](Invoke-Scalar $databases.Truong "BEGIN TRY BEGIN TRAN; UPDATE dbo.B10_BUOITHI SET TGThi=119 WHERE HKY=1 AND Ngay='2026-05-10' AND Gio='13:30' AND Phg='P102'; IF @@TRANCOUNT>0 ROLLBACK; SELECT 0; END TRY BEGIN CATCH IF @@TRANCOUNT>0 ROLLBACK; SELECT 1; END CATCH;"))
    Test-Equal 'B10-update-mon-chu-nhiem' 1 ([int](Invoke-Scalar $databases.Truong "BEGIN TRY BEGIN TRAN; UPDATE dbo.B10_GV SET MaMH='MH01' WHERE MaGV='GV02'; IF @@TRANCOUNT>0 ROLLBACK; SELECT 0; END TRY BEGIN CATCH IF @@TRANCOUNT>0 ROLLBACK; SELECT 1; END CATCH;"))
    Test-Equal 'B10-update-so-tiet' 1 ([int](Invoke-Scalar $databases.Truong "BEGIN TRY BEGIN TRAN; UPDATE dbo.B10_MHOC SET SoTiet=45 WHERE MaMH='MH04'; IF @@TRANCOUNT>0 ROLLBACK; SELECT 0; END TRY BEGIN CATCH IF @@TRANCOUNT>0 ROLLBACK; SELECT 1; END CATCH;"))
    Test-Equal 'B10-update-phan-cong-hop-le' 1 ([int](Invoke-Scalar $databases.Truong "BEGIN TRY BEGIN TRAN; UPDATE dbo.B10_PC_COI_THI SET MaGV='GV05' WHERE MaGV='GV04' AND HKY=1 AND Ngay='2026-05-10' AND Gio='07:30' AND Phg='P101'; DECLARE @ok int=CASE WHEN EXISTS(SELECT 1 FROM dbo.B10_PC_COI_THI WHERE MaGV='GV05' AND HKY=1 AND Ngay='2026-05-10' AND Gio='07:30' AND Phg='P101') THEN 1 ELSE 0 END; IF @@TRANCOUNT>0 ROLLBACK; SELECT @ok; END TRY BEGIN CATCH IF @@TRANCOUNT>0 ROLLBACK; SELECT 0; END CATCH;"))

    if ($BehaviorChecks) {
        $localPackages = Join-Path $env:USERPROFILE '.nuget\packages'
        & dotnet restore (Join-Path $root 'tools\BehaviorChecks\BehaviorChecks.csproj') --source $localPackages --packages $localPackages --ignore-failed-sources -v quiet
        if ($LASTEXITCODE -ne 0) { throw 'Cannot restore behavior checks from local NuGet cache.' }
        & dotnet build (Join-Path $root 'tools\BehaviorChecks\BehaviorChecks.csproj') --no-restore -v quiet -clp:ErrorsOnly
        if ($LASTEXITCODE -ne 0) { throw 'Cannot build behavior checks.' }
        $runner = Join-Path $root 'tools\BehaviorChecks\bin\Debug\net10.0-windows\BehaviorChecks.dll'
        & dotnet $runner $Server $databases.DeAn $databases.ThuVien $databases.Gara $databases.Truong
        if ($LASTEXITCODE -ne 0) { throw 'Behavior checks failed.' }
    }

    $results | Format-Table -AutoSize
    $failed = @($results | Where-Object TrangThai -eq 'FAIL').Count
    Write-Output "TOTAL=$($results.Count) PASS=$($results.Count-$failed) FAIL=$failed"
    if ($failed -gt 0) { throw "Có $failed testcase thất bại." }
}
finally {
    foreach ($database in $databases.Values) {
        try {
            Invoke-NonQuery 'master' "IF DB_ID(N'$database') IS NOT NULL BEGIN ALTER DATABASE [$database] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [$database]; END;"
        }
        catch {
            Write-Warning "Không xóa được database tạm $database`: $($_.Exception.Message)"
        }
    }
}
