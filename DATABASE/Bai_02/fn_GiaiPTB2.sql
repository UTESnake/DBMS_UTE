USE [QL_DeAn]
GO
/****** Object:  UserDefinedFunction [dbo].[fn_GiaiPTB2]    Script Date: 07/09/2026 5:16:24 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER FUNCTION [dbo].[fn_GiaiPTB2]
(
    @a FLOAT,
    @b FLOAT,
    @c FLOAT
)
RETURNS NVARCHAR(255)
AS
BEGIN
    -- Kiểm tra NULL
    IF @a IS NULL OR @b IS NULL OR @c IS NULL
        RETURN N'Hệ số a, b và c không được để trống';

    -- Trường hợp a = 0
    -- Phương trình trở thành bx + c = 0
    IF @a = 0
    BEGIN
        IF @b = 0
        BEGIN
            IF @c = 0
                RETURN N'Phương trình có vô số nghiệm';
            ELSE
                RETURN N'Phương trình vô nghiệm';
        END

        RETURN N'Phương trình có 1 nghiệm: x = '
             + FORMAT(-@c / @b, 'G17', 'en-US');
    END

    -- Trường hợp a khác 0
    DECLARE @Delta FLOAT;
    SET @Delta = @b * @b - 4 * @a * @c;

    -- Delta < 0: phương trình không có nghiệm thực.
    -- Không dùng epsilon vì một delta âm dù rất nhỏ vẫn không bằng 0.
    IF @Delta < 0
        RETURN N'Phương trình vô nghiệm';

    -- Delta bằng 0
    IF @Delta = 0
    BEGIN
        DECLARE @x FLOAT;

        SET @x = -@b / (2 * @a);

        RETURN N'Phương trình có nghiệm kép: x1 = x2 = '
             + FORMAT(@x, 'G17', 'en-US');
    END

    -- Delta > 0
    DECLARE @x1 FLOAT;
    DECLARE @x2 FLOAT;

    SET @x1 =
        (-@b + SQRT(@Delta)) / (2 * @a);

    SET @x2 =
        (-@b - SQRT(@Delta)) / (2 * @a);

    RETURN N'Phương trình có 2 nghiệm: x1 = '
         + FORMAT(@x1, 'G17', 'en-US')
         + N' và x2 = '
         + FORMAT(@x2, 'G17', 'en-US');
END;
