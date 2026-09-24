using System;
using Microsoft.Data.SqlClient;

namespace DoAn.Shared
{
    public static class SqlFailureClassifier
    {
        private static bool FromProcedure(SqlError error, string name) =>
            string.Equals(error.Procedure, name, StringComparison.OrdinalIgnoreCase)
            || string.Equals(error.Procedure, "dbo." + name, StringComparison.OrdinalIgnoreCase);

        // RAISERROR uses 50000 for many unrelated errors: check the source and message too.
        public static bool IsLibraryLookup(SqlException exception)
        {
            bool found = false;
            foreach (SqlError error in exception.Errors)
            {
                if (error.Number == 3621) continue;
                bool reader = FromProcedure(error, "sp_ThongtinDocGia")
                    && (error.Message == "Mã độc giả không được để trống."
                        || error.Message == "Không tìm thấy độc giả có mã này."
                        || error.Message == "Độc giả tồn tại nhưng chưa được phân loại.");
                bool book = FromProcedure(error, "sp_ThongTinDauSach")
                    && (error.Message == "ISBN không được để trống."
                        || error.Message == "Không tìm thấy đầu sách có ISBN này.");
                if (error.Number != 50000 || (!reader && !book)) return false;
                found = true;
            }
            return found;
        }

        public static bool IsConstraint(SqlException exception, string constraint)
        {
            int number;
            switch (constraint)
            {
                case "FK_Muon_Cuonsach":
                case "FK_Muon_DocGia":
                case "CK_Muon_ThoiHan": number = 547; break;
                case "PK_Muon":
                case "UQ_Muon_CuonDangMuon":
                case "PK_Tuasach": number = 2627; break;
                default: return false;
            }
            bool found = false;
            foreach (SqlError error in exception.Errors)
            {
                if (error.Number == 3621 || error.Number == 3609) continue;
                if (error.Number != number ||
                    !(error.Message.Contains("\"" + constraint + "\"") || error.Message.Contains("'" + constraint + "'")))
                    return false;
                found = true;
            }
            return found;
        }

        public static bool IsLibraryConstraint(SqlException exception)
        {
            foreach (string constraint in new[] { "FK_Muon_Cuonsach", "FK_Muon_DocGia", "CK_Muon_ThoiHan", "PK_Muon", "UQ_Muon_CuonDangMuon", "PK_Tuasach" })
                if (IsConstraint(exception, constraint)) return true;
            return false;
        }
    }
}
