USE [QL_DeAn]
GO
/****** Object:  UserDefinedFunction [dbo].[fn_TinhTuoi]    Script Date: 07/09/2026 10:08:12 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER FUNCTION [dbo].[fn_TinhTuoi]
(
    @NgaySinh DATE
)
RETURNS INT
AS
BEGIN
    DECLARE @NgayHienTai DATE = CAST(GETDATE() AS DATE);
    DECLARE @Tuoi INT;

    -- Dữ liệu không hợp lệ trả về SQL NULL để hàm giữ đúng kiểu số.
    IF @NgaySinh IS NULL OR @NgaySinh > @NgayHienTai
        RETURN NULL;

    -- Chỉ tăng tuổi khi sinh nhật của năm hiện tại đã đến.
    SET @Tuoi = DATEDIFF(YEAR, @NgaySinh, @NgayHienTai);

    IF DATEADD(YEAR, @Tuoi, @NgaySinh) > @NgayHienTai
        SET @Tuoi = @Tuoi - 1;

    RETURN @Tuoi;
END;
GO
