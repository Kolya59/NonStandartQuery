namespace NonStandartQuery
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    /// <inheritdoc />
    public partial class DateTimeCell : DataGridViewTextBoxCell
    {
        private DateTimePicker dtp = new DateTimePicker();
        private Rectangle rectangle;

        public DateTimeCell()
        {
            InitializeComponent();
            components.Add(dtp);
        }

        private void OnCellClick(object sender, DataGridViewCellEventArgs e)
        {
            rectangle = ((DataGridView)sender).GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);
            dtp.Size = new Size(rectangle.Width, rectangle.Height);
            dtp.Location = new Point(rectangle.X, rectangle.Y);
            dtp.Visible = true;
        }

        private void OnTextChange(object sender, EventArgs e)
        {
            ((DataGridView)sender).CurrentCell.Value = dtp.Text.ToString();
        }

        private void OnColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            dtp.Visible = false;
        }

        private void OnScroll(object sender, ScrollEventArgs e)
        {
            dtp.Visible = false;
        }
    }
}
