-- Ch?y sau khi ?? c?i d? li?u, procedure, function, trigger v? danh m?c testcase c?a nh?m.
-- C?c SELECT hi?n th? k?t qu? ?? ??i chi?u; d? li?u th? trong transaction ???c ROLLBACK.

USE QL_DeAn;
GO
EXEC dbo.sp_GiaiPTB1 @a=+5, @b=+3.2;
EXEC dbo.sp_GiaiPTB1 @a=2, @b=-0.0;
EXEC dbo.sp_GiaiPTB1 @a=1.7976931348623157E+308, @b=0;
SELECT dbo.fn_GiaiPTB2(1,1e160,1) AS NghiemKhongTran,
       dbo.fn_GiaiPTB2(1,1e16,1) AS NghiemKhongTrietTieu;
SELECT dbo.fn_TinhTuoi(CAST(GETDATE() AS date)) AS TuoiSoSinh,
       dbo.fn_TinhTuoi('19000101') AS TuoiVuot120;
GO
