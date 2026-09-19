param(
    [string]$Server = '.\SQLEXPRESS02'
)

$ErrorActionPreference = 'Stop'
$projectRoot = Split-Path -Parent $PSScriptRoot
$connectionString =
    "Data Source=$Server;Initial Catalog=QL_DeAn;Integrated Security=True;Encrypt=False;TrustServerCertificate=True"

function Get-SqlBatches([string]$path) {
    $sql = Get-Content -LiteralPath $path -Raw -Encoding UTF8

    return [Text.RegularExpressions.Regex]::Split(
        $sql,
        '(?im)^\s*GO\s*(?:--.*)?$'
    )
}

$scriptPaths = @(
    (Join-Path $projectRoot 'DATABASE\Bai_04\fn_TinhTuoi.sql'),
    (Join-Path $projectRoot 'DATABASE\Nhom_1_CSDL_ToanHoc\02_Testcase.sql')
)

$connection = [System.Data.SqlClient.SqlConnection]::new($connectionString)

try {
    $connection.Open()
    $transaction = $connection.BeginTransaction()

    try {
        foreach ($path in $scriptPaths) {
            foreach ($batch in (Get-SqlBatches $path)) {
                if ([string]::IsNullOrWhiteSpace($batch)) {
                    continue
                }

                $command = $connection.CreateCommand()
                $command.Transaction = $transaction
                $command.CommandTimeout = 120
                $command.CommandText = $batch
                [void]$command.ExecuteNonQuery()
            }
        }

        $transaction.Commit()
    }
    catch {
        $transaction.Rollback()
        throw
    }

    $verify = $connection.CreateCommand()
    $verify.CommandText = @'
SELECT COUNT(*) AS SoTestcaseBai4
FROM dbo.Testcase_Nhom1
WHERE Bai = 'B4';

SELECT MaCase, MoTa, NamSinh, LoaiTest
FROM dbo.Testcase_Nhom1
WHERE Bai = 'B4'
ORDER BY ThuTu;

SELECT
    dbo.fn_TinhTuoi(DATEADD(YEAR, -20, CAST(GETDATE() AS date))) AS TuoiSinhNhatHomNay,
    dbo.fn_TinhTuoi(DATEADD(DAY, 1, DATEADD(YEAR, -20, CAST(GETDATE() AS date)))) AS TuoiChuaDenSinhNhat,
    dbo.fn_TinhTuoi(DATEADD(DAY, 1, CAST(GETDATE() AS date))) AS NgaySinhTuongLai;
'@

    $tables = [System.Data.DataSet]::new()
    $adapter = [System.Data.SqlClient.SqlDataAdapter]::new($verify)
    [void]$adapter.Fill($tables)

    Write-Host "Đã cập nhật database QL_DeAn trên $Server."
    Write-Host "Số testcase Bài 4: $($tables.Tables[0].Rows[0]['SoTestcaseBai4'])"
    $tables.Tables[1] | Format-Table -AutoSize
    $tables.Tables[2] | Format-Table -AutoSize
}
finally {
    $connection.Dispose()
}
