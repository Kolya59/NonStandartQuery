namespace NonStandartQuery
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Windows.Forms;

    /// <inheritdoc />
    public partial class FormSetConnection : Form
    {
        public FormSetConnection(SqlConnection stableConnection)
        {
            InitializeComponent();
            CurrentSqlConnectionStringBuilder = new SqlConnectionStringBuilder();
            btOk.Click += BtOkClick;
            btCancel.Click += BtCancelClick;
            cbDataBase.SelectedIndexChanged += CbDataBaseSelectedIndexChanged;
            LastStableConnection = stableConnection;
            tbServer.Text = stableConnection.DataSource;
            IntializeCurrentServer(LastStableConnection);
            cbDataBase.SelectedItem = cbDataBase.Items[0];
        }

        private SqlConnection LastStableConnection { get; }

        private SqlConnectionStringBuilder CurrentSqlConnectionStringBuilder { get; set; }

        public static bool CheckDataBase(SqlConnection sqlConnection)
        {
            try
            {
                if (sqlConnection.State != ConnectionState.Open)
                {
                    sqlConnection.Open();
                }

                var sqlCommand = new SqlCommand
                {
                    Connection = sqlConnection,
                    CommandText = @"SELECT 
                                        COUNT (DISTINCT INFORMATION_SCHEMA.COLUMNS.TABLE_NAME)
                                    FROM 
	                                    INFORMATION_SCHEMA.COLUMNS
                                    WHERE 
	                                    TABLE_NAME = '_Fields' AND 
	                                    (COLUMN_NAME = 'id' AND DATA_TYPE = 'INT' OR 
	                                        COLUMN_NAME = 'Field_name' AND DATA_TYPE = 'NVARCHAR' OR 
	                                        COLUMN_NAME = 'Table_name' AND DATA_TYPE = 'NVARCHAR' OR 
	                                        COLUMN_NAME = 'Field_type' AND DATA_TYPE = 'NVARCHAR' OR 
	                                        COLUMN_NAME = 'Transl_fn' AND DATA_TYPE = 'NVARCHAR' OR 
	                                        COLUMN_NAME = 'Category_name' AND DATA_TYPE = 'NVARCHAR')"
                };
                var reader = sqlCommand.ExecuteScalar();
                if ((int)reader != 1)
                {
                    sqlConnection.Close();
                    return false;
                }

                sqlCommand.CommandText = @"SELECT
                                            COUNT (DISTINCT INFORMATION_SCHEMA.COLUMNS.TABLE_NAME)
                                         FROM
                                            INFORMATION_SCHEMA.COLUMNS
                                         WHERE
                                            TABLE_NAME = '_Reltable' AND
                                            (COLUMN_NAME = 'id' AND DATA_TYPE = 'INT' OR
                                             COLUMN_NAME = 'Table1' AND DATA_TYPE = 'NVARCHAR' OR
                                             COLUMN_NAME = 'Table2' AND DATA_TYPE = 'NVARCHAR' OR
                                             COLUMN_NAME = 'Relations' AND DATA_TYPE = 'NVARCHAR' OR
                                             COLUMN_NAME = 'Via' AND DATA_TYPE = 'NVARCHAR')";
                sqlCommand.ExecuteScalar();
                if ((int)reader != 1)
                {
                    sqlConnection.Close();
                    return false;
                }

                sqlConnection.Close();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private static bool IsStandardDataBase(string dbName) =>
            dbName == "tempdb" || dbName == "master" || dbName == "model" || dbName == "msdb";

        private void IntializeCurrentServer(DbConnection connection)
        {
            try
            {
                connection.Open();
            }
            catch
            {
                MessageBox.Show(@"Ошибка подключения к БД, невозможно открыть соединение");
                return;
            }

            cbDataBase.Items.Clear();

            foreach (DataRow dataBaseRow in connection.GetSchema("Databases").Rows)
            {
                var dataBase = (string)dataBaseRow[0];
                SqlConnection newConnection;
                try
                {
                    newConnection = new SqlConnection(new SqlConnectionStringBuilder
                                                              {
                                                                  DataSource = CurrentSqlConnectionStringBuilder.DataSource,
                                                                  InitialCatalog = dataBase,
                                                                  IntegratedSecurity = true
                                                              }.ConnectionString);
                }
                catch (Exception)
                {
                    newConnection = new SqlConnection(new SqlConnectionStringBuilder
                                                              {
                                                                  DataSource = LastStableConnection.DataSource,
                                                                  InitialCatalog = dataBase,
                                                                  IntegratedSecurity = true
                                                              }.ConnectionString);
                }

                if (!IsStandardDataBase(dataBase) && CheckDataBase(newConnection))
                {
                    cbDataBase.Items.Add(dataBase);
                }
            }

            cbDataBase.SelectedIndex = 0;
            connection.Close();
        }

        private void BtOkClick(object sender, EventArgs e)
        {
            if (cbDataBase.SelectedItem.Equals(LastStableConnection))
            {
                DialogResult = DialogResult.Cancel;
                return;
            }

            Properties.Settings.Default.DataSource = tbServer.Text;
            Properties.Settings.Default.InitialCatalog = cbDataBase.Text;
            Properties.Settings.Default.Save();
            DialogResult = DialogResult.OK;
        }

        private void CbDataBaseSelectedIndexChanged(object sender, EventArgs e)
        {
            btOk.Enabled = true;
            CurrentSqlConnectionStringBuilder.InitialCatalog = cbDataBase.SelectedItem.ToString();
        }

        private void BtCancelClick(object sender, EventArgs e) => DialogResult = DialogResult.Cancel;

        private void BtChangeServerClick(object sender, EventArgs e)
        {
            var form = new FormSetServer { Visible = false };
            form.ShowDialog();
            if (form.DialogResult != DialogResult.OK)
            {
                return;
            }

            tbServer.Text = form.CurrentServer;
            CurrentSqlConnectionStringBuilder = new SqlConnectionStringBuilder
                                                    {
                                                        DataSource = form.CurrentServer,
                                                        InitialCatalog = "master",
                                                        IntegratedSecurity = true
                                                    };
            var connection = new SqlConnection(CurrentSqlConnectionStringBuilder.ConnectionString);
            
            IntializeCurrentServer(connection);
        }
    }
}