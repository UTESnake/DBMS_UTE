using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace DoAn.Shared
{
    // Keep the message loop responsive while SQL runs. Defer closing until rollback/cleanup completes.
    public sealed class FormOperation : IDisposable
    {
        private static readonly ConditionalWeakTable<Form, FormOperation> Active = new();
        private readonly Form form;
        private readonly List<(Control Control, bool Enabled)> controls;
        private bool closeRequested;
        private bool disposed;

        private FormOperation(Form owner)
        {
            form = owner;
            controls = Children(owner).Where(c => c is Button || c is TextBox || c is ComboBox || c is DataGridView)
                .Select(c => (c, c.Enabled)).ToList();
            foreach (var item in controls) item.Control.Enabled = false;
            owner.FormClosing += Closing;
        }

        public static FormOperation? TryStart(Form form)
        {
            if (form.IsDisposed || Active.TryGetValue(form, out _)) return null;
            var operation = new FormOperation(form);
            Active.Add(form, operation);
            return operation;
        }

        private static IEnumerable<Control> Children(Control owner)
        {
            foreach (Control child in owner.Controls)
            {
                yield return child;
                foreach (var nested in Children(child)) yield return nested;
            }
        }

        private void Closing(object? sender, FormClosingEventArgs e)
        {
            closeRequested = true;
            e.Cancel = true;
        }

        public void Dispose()
        {
            if (disposed) return;
            disposed = true;
            form.FormClosing -= Closing;
            Active.Remove(form);
            if (form.IsDisposed) return;
            foreach (var item in controls)
                if (!item.Control.IsDisposed) item.Control.Enabled = item.Enabled;
            if (closeRequested) form.BeginInvoke(new Action(form.Close));
        }

        public static string ErrorMessage(Exception exception)
        {
            if (exception is SqlException sql)
            {
                if (sql.Number == -2) return "ERROR – Truy vấn quá thời gian chờ. Bạn có thể thử lại.\n" + sql.Message;
                if (sql.Number == 208 || sql.Number == 2812 || sql.Number == 4121 || sql.Number == 4060)
                    return "ERROR – Thiếu CSDL hoặc đối tượng SQL. Hãy cài đặt/cập nhật script riêng; không tự reset dữ liệu.\n" + sql.Message;
            }
            return "ERROR – Không thể hoàn tất thao tác.\n" + exception.Message;
        }
    }
}
