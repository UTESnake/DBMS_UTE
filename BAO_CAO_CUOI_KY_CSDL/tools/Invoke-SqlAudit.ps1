param(
    [string]$Server = '.\SQLEXPRESS02'
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
        @('DATABASE\Nhom_3_CSDL_DeAn\02_Functions.sql', 'QL_DeAn'),
        @('DATABASE\Bai_08\02_Functions.sql', 'QL_DeAn'),
        @('DATABASE\Nhom_3_CSDL_DeAn\03_Testcase.sql', 'QL_DeAn')
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

    Test-Decimal 'B7.1-luong-trung-binh' 35666.67 (Invoke-Scalar $databases.DeAn "SELECT dbo.fn_B7_LuongTrungBinhPhong('PB01');")
    Test-Equal 'B7.1-phong-khong-ton-tai' ([decimal]0) ([decimal](Invoke-Scalar $databases.DeAn "SELECT dbo.fn_B7_LuongTrungBinhPhong('XXX');"))
    Test-Decimal 'B7.2-phan-bo-theo-ty-trong' 15737.70 (Invoke-Scalar $databases.DeAn "SELECT dbo.fn_B7_TongLuongNhanVienDeAn('NV01','DA01');")
    Test-Equal 'B7.2-tong-gio-0' ([decimal]0) ([decimal](Invoke-Scalar $databases.DeAn "SELECT dbo.fn_B7_TongLuongNhanVienDeAn('NV05','DA03');"))
    Test-Equal 'B7.4-bien-60' ([decimal]500) ([decimal](Invoke-Scalar $databases.DeAn 'SELECT dbo.fn_B7_TienThuong(60);'))
    Test-Equal 'B7.4-bien-100' ([decimal]1200) ([decimal](Invoke-Scalar $databases.DeAn 'SELECT dbo.fn_B7_TienThuong(100);'))
    Test-Equal 'B7.4-bien-150' ([decimal]1600) ([decimal](Invoke-Scalar $databases.DeAn 'SELECT dbo.fn_B7_TienThuong(150);'))
    Test-Equal 'B7.6-hai-cach-giong-nhau' 0 ([int](Invoke-Scalar $databases.DeAn 'SELECT (SELECT COUNT(*) FROM (SELECT * FROM dbo.fn_B7_ThongTinNhanVien_Inline() EXCEPT SELECT * FROM dbo.fn_B7_ThongTinNhanVien_Multi()) a) + (SELECT COUNT(*) FROM (SELECT * FROM dbo.fn_B7_ThongTinNhanVien_Multi() EXCEPT SELECT * FROM dbo.fn_B7_ThongTinNhanVien_Inline()) b);'))
    Test-Decimal 'B7.6-luong-tb-phong' 35666.67 (Invoke-Scalar $databases.DeAn "SELECT TongLuongTB FROM dbo.fn_B7_ThongTinNhanVien_Inline() WHERE MaNV='NV01';")

    Test-Equal 'B8.1-du-an-hon-2-nv' 3 ([int](Invoke-Scalar $databases.DeAn 'SELECT COUNT(*) FROM dbo.fn_B8_DeAnNhieuNhanVien();'))
    Test-Equal 'B8.2-phong-hon-2-nv' 3 ([int](Invoke-Scalar $databases.DeAn 'SELECT COUNT(*) FROM dbo.fn_B8_PhongNhieuNhanVienLuongCao();'))
    Test-Equal 'B8.4-dem-nam' 2 ([int](Invoke-Scalar $databases.DeAn "SELECT SoLuongNhanVienNam FROM dbo.fn_B8_PhongLuongTBLon_Nam() WHERE MaPB='PB01';"))
    Test-Equal 'B8.5-ke-ca-ket-qua-0' 0 ([int](Invoke-Scalar $databases.DeAn "SELECT SoNhanVienPhong5 FROM dbo.fn_B8_DeAnCoNhanVienPhong5() WHERE MaDA='DA04';"))

    Test-Equal 'B9.1-tho-khong-tham-gia' 3 ([int](Invoke-Scalar $databases.Gara 'SELECT COUNT(*) FROM dbo.fn_B9_ThoKhongThamGiaHopDong();'))
    Test-Decimal 'B9.2-con-no' 2500000 (Invoke-Scalar $databases.Gara "SELECT ConNo FROM dbo.fn_B9_HopDongDaThanhLyChuaDuTien() WHERE SoHD='HD04';")
    Test-Equal 'B9.3-truoc-31-12-2002' 2 ([int](Invoke-Scalar $databases.Gara 'SELECT COUNT(*) FROM dbo.fn_B9_HopDongCanHoanTatTruoc();'))
    Test-Equal 'B9.4-nhieu-viec-nhat' 'T02' ([string](Invoke-Scalar $databases.Gara 'SELECT MaTho FROM dbo.fn_B9_ThoNhieuCongViecNhat();'))
    Test-Decimal 'B9.5-tri-gia-cao-nhat' 5800000 (Invoke-Scalar $databases.Gara "SELECT TongTriGia FROM dbo.fn_B9_ThoTongTriGiaCaoNhat() WHERE MaTho='T02';")
    Test-Equal 'B9-rang-buoc-cung-nhom' 1 ([int](Invoke-Scalar $databases.Gara "BEGIN TRY BEGIN TRAN; INSERT dbo.B9_THO VALUES('T99',N'Thợ thử',2,'T01'); IF @@TRANCOUNT>0 ROLLBACK; SELECT 0; END TRY BEGIN CATCH IF @@TRANCOUNT>0 ROLLBACK; SELECT 1; END CATCH;"))
    Test-Equal 'B9-unique-xe-ngay' 1 ([int](Invoke-Scalar $databases.Gara "BEGIN TRY BEGIN TRAN; INSERT dbo.B9_HOPDONG VALUES('HD99','2002-12-01','KH01','51A-111.11',1,'2002-12-01',NULL); IF @@TRANCOUNT>0 ROLLBACK; SELECT 0; END TRY BEGIN CATCH IF @@TRANCOUNT>0 ROLLBACK; SELECT 1; END CATCH;"))
    Test-Equal 'B9-phieu-thu-dung-chu' 1 ([int](Invoke-Scalar $databases.Gara "BEGIN TRY BEGIN TRAN; INSERT dbo.B9_PHIEUTHU VALUES('PT99','2002-12-10','HD01','KH02',N'Thử',1); IF @@TRANCOUNT>0 ROLLBACK; SELECT 0; END TRY BEGIN CATCH IF @@TRANCOUNT>0 ROLLBACK; SELECT 1; END CATCH;"))
    Test-Equal 'B9-update-nhieu-dong-hop-le' 1 ([int](Invoke-Scalar $databases.Gara "BEGIN TRY BEGIN TRAN; UPDATE dbo.B9_THO SET Nhom=4 WHERE MaTho IN('T01','T02','T03'); DECLARE @ok int=CASE WHEN (SELECT COUNT(*) FROM dbo.B9_THO WHERE MaTho IN('T01','T02','T03') AND Nhom=4)=3 THEN 1 ELSE 0 END; IF @@TRANCOUNT>0 ROLLBACK; SELECT @ok; END TRY BEGIN CATCH IF @@TRANCOUNT>0 ROLLBACK; SELECT 0; END CATCH;"))

    Test-Equal 'B10.2a-mon-tu-45-tiet' 3 ([int](Invoke-Scalar $databases.Truong 'SELECT COUNT(*) FROM dbo.fn_B10_GiaoVienMonTu45Tiet();'))
    Test-Equal 'B10.2b-tham-so-hoc-ky-2' 2 ([int](Invoke-Scalar $databases.Truong 'SELECT COUNT(*) FROM dbo.fn_B10_GiaoVienGacThiHocKy(2);'))
    Test-Equal 'B10.2c-khong-gac-hk1' 3 ([int](Invoke-Scalar $databases.Truong 'SELECT COUNT(*) FROM dbo.fn_B10_GiaoVienKhongGacThiHocKy(1);'))
    Test-Equal 'B10.2d-tham-so-ten-mon' 2 ([int](Invoke-Scalar $databases.Truong "SELECT COUNT(*) FROM dbo.fn_B10_LichThiMon(N'TOÁN');"))
    Test-Equal 'B10.2e-gv-chu-nhiem-van' 4 ([int](Invoke-Scalar $databases.Truong "SELECT COUNT(*) FROM dbo.fn_B10_BuoiGacThiCuaGiaoVienChuNhiemMon(N'VĂN HỌC');"))
    Test-Equal 'B10.1a-khong-gac-mon-chu-nhiem' 1 ([int](Invoke-Scalar $databases.Truong "BEGIN TRY BEGIN TRAN; INSERT dbo.B10_PC_COI_THI VALUES('GV01',1,'2026-05-10','07:30','P101'); IF @@TRANCOUNT>0 ROLLBACK; SELECT 0; END TRY BEGIN CATCH IF @@TRANCOUNT>0 ROLLBACK; SELECT 1; END CATCH;"))
    Test-Equal 'B10.1b-30-tiet-120-phut' 1 ([int](Invoke-Scalar $databases.Truong "BEGIN TRY BEGIN TRAN; INSERT dbo.B10_BUOITHI VALUES(3,'2099-01-01','07:30','Z01','MH02',119); IF @@TRANCOUNT>0 ROLLBACK; SELECT 0; END TRY BEGIN CATCH IF @@TRANCOUNT>0 ROLLBACK; SELECT 1; END CATCH;"))
    Test-Equal 'B10.1c-tu-45-tiet-150-phut' 1 ([int](Invoke-Scalar $databases.Truong "BEGIN TRY BEGIN TRAN; INSERT dbo.B10_BUOITHI VALUES(3,'2099-01-01','07:30','Z02','MH01',149); IF @@TRANCOUNT>0 ROLLBACK; SELECT 0; END TRY BEGIN CATCH IF @@TRANCOUNT>0 ROLLBACK; SELECT 1; END CATCH;"))
    Test-Equal 'B10-update-thoi-luong' 1 ([int](Invoke-Scalar $databases.Truong "BEGIN TRY BEGIN TRAN; UPDATE dbo.B10_BUOITHI SET TGThi=119 WHERE HKY=1 AND Ngay='2026-05-10' AND Gio='13:30' AND Phg='P102'; IF @@TRANCOUNT>0 ROLLBACK; SELECT 0; END TRY BEGIN CATCH IF @@TRANCOUNT>0 ROLLBACK; SELECT 1; END CATCH;"))
    Test-Equal 'B10-update-mon-chu-nhiem' 1 ([int](Invoke-Scalar $databases.Truong "BEGIN TRY BEGIN TRAN; UPDATE dbo.B10_GV SET MaMH='MH01' WHERE MaGV='GV02'; IF @@TRANCOUNT>0 ROLLBACK; SELECT 0; END TRY BEGIN CATCH IF @@TRANCOUNT>0 ROLLBACK; SELECT 1; END CATCH;"))
    Test-Equal 'B10-update-so-tiet' 1 ([int](Invoke-Scalar $databases.Truong "BEGIN TRY BEGIN TRAN; UPDATE dbo.B10_MHOC SET SoTiet=45 WHERE MaMH='MH04'; IF @@TRANCOUNT>0 ROLLBACK; SELECT 0; END TRY BEGIN CATCH IF @@TRANCOUNT>0 ROLLBACK; SELECT 1; END CATCH;"))
    Test-Equal 'B10-update-phan-cong-hop-le' 1 ([int](Invoke-Scalar $databases.Truong "BEGIN TRY BEGIN TRAN; UPDATE dbo.B10_PC_COI_THI SET MaGV='GV05' WHERE MaGV='GV04' AND HKY=1 AND Ngay='2026-05-10' AND Gio='07:30' AND Phg='P101'; DECLARE @ok int=CASE WHEN EXISTS(SELECT 1 FROM dbo.B10_PC_COI_THI WHERE MaGV='GV05' AND HKY=1 AND Ngay='2026-05-10' AND Gio='07:30' AND Phg='P101') THEN 1 ELSE 0 END; IF @@TRANCOUNT>0 ROLLBACK; SELECT @ok; END TRY BEGIN CATCH IF @@TRANCOUNT>0 ROLLBACK; SELECT 0; END CATCH;"))

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
