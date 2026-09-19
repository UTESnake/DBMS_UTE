USE [QL_DeAn]
GO
/****** Object:  StoredProcedure [dbo].[sp_GiaiPTB1]    Script Date: 07/09/2026 4:36:55 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

DROP PROCEDURE IF EXISTS [dbo].[sp_GiaiPTB1];
GO

CREATE PROCEDURE [dbo].[sp_GiaiPTB1]
    @a FLOAT,
    @b FLOAT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        CASE
            -- Kiểm tra NULL
            WHEN @a IS NULL OR @b IS NULL THEN N'Hệ số a và b không được để trống'

            -- a = 0, b = 0
            WHEN @a = 0 AND @b = 0 THEN N'Phương trình có vô số nghiệm'

            -- a = 0, b khác 0
            WHEN @a = 0 AND @b <> 0 THEN N'Phương trình vô nghiệm'
                
            -- a khác 0, b = 0
            WHEN @b = 0 AND @a <> 0 THEN N'Phương trình có nghiệm: x = 0'

            -- a khác 0, b khác 0
            ELSE
                N'Phương trình có nghiệm: x = '
                + CAST(-@b / @a AS NVARCHAR(50))
        END AS KetQua;
END;
