namespace NonStandartQuery
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Windows.Forms;

    using static System.Windows.Forms.DialogResult;

    /// <inheritdoc />
    public partial class FormSetServer : Form
    {
        public FormSetServer()
        {
            InitializeComponent();
        }

        public string CurrentServer { get; private set; }

        private static bool CheckServer(IDbConnection sqlConnection)
        {
            try
            {
                sqlConnection.Open();
                sqlConnection.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void BtOkClick(object sender, EventArgs e)
        {
            if (CheckServer(new SqlConnection
                                {
                                    ConnectionString = new SqlConnectionStringBuilder
                                                           {
                                                               DataSource = tbServerName.Text,
                                                               InitialCatalog = "master",
                                                               IntegratedSecurity = true
                                                           }.ConnectionString
                                }))
            {
                CurrentServer = tbServerName.Text;
                DialogResult = OK;
            }
            else
            {
                MessageBox.Show(@"Не удается подключиться к серверу");
            }
        }

        private void BtCancelClick(object sender, EventArgs e)
        {
            DialogResult = Cancel;
        }
    }
}
