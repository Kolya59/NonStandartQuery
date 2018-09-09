﻿// ReSharper disable StaticMemberInitializerReferesToMemberBelow
// ReSharper disable StyleCop.SA1204
namespace NonStandartQuery
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Windows.Forms;

    using NonStandartQuery.Classes;

    /// <inheritdoc />
    /// <summary>
    /// The form main.
    /// </summary>
    public partial class FormMain : Form
    {
        #region Описание контейнеров данных

        private readonly List<Condition> collectionOfConditions = new List<Condition>();

        private readonly List<OrderedField> collectionOfSelectedFieldsToSort = new List<OrderedField>();

        private readonly Dictionary<Tuple<string, string>, string> collectionOfAdjacentTables =
            new Dictionary<Tuple<string, string>, string>();

        private readonly Dictionary<Tuple<string, string>, string> collectionOfNonAdjacentTables =
            new Dictionary<Tuple<string, string>, string>();

        private readonly List<Field> mainCollectionOfAllFields = new List<Field>();

        private List<Field> collectionOfAllFields = new List<Field>();

        private List<Field> collectionOfSelectedFields = new List<Field>();

        private List<Field> collectionOfAllFieldsToSort = new List<Field>();

        #endregion

        #region Описание данных для БД

        private string currentSqlQuerySelectPart = string.Empty;

        private string currentSqlQueryFromPart = string.Empty;

        private string currentSqlQueryWherePart = string.Empty;

        private string currentSqlQueryOrderPart = string.Empty;

        private string connectionString = string.Empty;

        private SqlCommand сurrentSqlCommand = new SqlCommand();

        private SqlConnection sqlConnection = new SqlConnection();

        public FormMain()
        {
            IntializeConnection();
            CheckConnection();
            if (!FormSetConnection.CheckDataBase(sqlConnection))
            {
                MessageBox.Show(@"Ошибка подключения, проверьте подключение, либо смените базу данных");
                new FormSetConnection(
                    new SqlConnection(
                        new SqlConnectionStringBuilder
                            {
                                DataSource = Properties.Settings.Default.DataSource,
                                InitialCatalog = Properties.Settings.Default.InitialCatalog,
                                IntegratedSecurity = true
                            }.ConnectionString)).ShowDialog();
            }

            InitializeComponent();
            IntializeContent();
        }

        #endregion

        #region Инициализация контейнеров данных на форме

        private void IntializeContent()
        {
            IntializeCollections();
            IntializeContainersOnForm();
        }

        private void IntializeContainersOnForm()
        {
            IntializeLvAllFields();
            IntializeLvSelectedFields();
            IntializeDgvConditions();
            IntializeLvAllFieldsToSort();
            IntializeLvSelectedFieldsToSort();
            IntializeDgvResult();
        }

        private void IntializeLvAllFields()
        {
            lvAllFields.Alignment = ListViewAlignment.Default;
            lvAllFields.FullRowSelect = true;
            lvAllFields.GridLines = true;
            lvAllFields.MultiSelect = true;
            var indicator = string.Empty;
            foreach (var field in collectionOfAllFields)
            {
                var group = new ListViewGroup
                                {
                                    Header = field.CategoryName,
                                    Name = field.CategoryName,
                                    HeaderAlignment = HorizontalAlignment.Left
                                };
                if (field.CategoryName != indicator)
                {
                    indicator = field.CategoryName;
                    lvAllFields.Groups.Add(group);
                    lvSelectedFields.Groups.Add(group);
                    lvAllFieldsToSort.Groups.Add(group);
                    lvSelectedFieldsToSort.Groups.Add(group);
                }

                var item = new ListViewItem { Group = lvAllFields.Groups[field.CategoryName], Name = field.Name, Tag = field, Text = field.DisplayedName };
                lvAllFields.Items.Add(item);
            }
        }

        private void IntializeLvSelectedFields()
        {
            lvSelectedFields.Alignment = ListViewAlignment.Default;
            lvSelectedFields.FullRowSelect = true;
            lvSelectedFields.GridLines = true;
            lvSelectedFields.MultiSelect = true;
        }

        private void IntializeDgvConditions()
        {
            dgvConditions.RowValidating += DgvConditionsRowValidating;
            dgvConditions.UserDeletingRow += DgvConditionsUserDeletingRows;
            dgvConditions.CellValueChanged += DgvConditionsCellValueChanged;
            dgvConditions.RowEnter += DgvConditionsRowEnter;
            dgvConditions.DataError += DgvConditionsDataError;
            //// TODO: Не отображаются условия
            dgvConditions.Columns.Add(
                new DataGridViewComboBoxColumn
                    {
                        DataPropertyName = "DisplayedName",
                        DataSource = mainCollectionOfAllFields,
                        Name = @"Field",
                        HeaderText = @"Поле"
                    });
            dgvConditions.Columns.Add(
                new DataGridViewComboBoxColumn
                    {
                        DataPropertyName = "Criterion", HeaderText = @"Критерий", Name = @"Criterion"
                    });
            dgvConditions.Columns.Add(
                new DataGridViewTextBoxColumn
                    {
                        DataPropertyName = "Expression",
                        HeaderText = @"Выражение",
                        Name = @"Expression",
                        ReadOnly = true,
                        Visible = true
                    });
            dgvConditions.Columns.Add(
                new DataGridViewComboBoxColumn
                    {
                        DataSource = new[] { "И", "ИЛИ" }, HeaderText = @"Связка", Name = "Bunch"
                    });
            dgvConditions.Dock = DockStyle.Fill;
        }

        private void IntializeLvAllFieldsToSort()
        {
            lvAllFieldsToSort.Alignment = ListViewAlignment.Default;
            lvAllFieldsToSort.FullRowSelect = true;
            lvAllFieldsToSort.GridLines = true;
            lvAllFieldsToSort.MultiSelect = true;
        }

        private void IntializeLvSelectedFieldsToSort()
        {
            lvSelectedFieldsToSort.Alignment = ListViewAlignment.Default;
            lvSelectedFieldsToSort.FullRowSelect = true;
            lvSelectedFieldsToSort.GridLines = true;
            lvSelectedFieldsToSort.MultiSelect = true;
        }

        private void IntializeDgvResult()
        {
            dgvResult.AllowUserToAddRows = false;
            dgvResult.AllowUserToDeleteRows = false;
            dgvResult.AllowUserToOrderColumns = false;
        }

        #endregion

        #region Методы связанные с контейнерами данных

        private void IntializeCollections()
        {
            sqlConnection.Open();

            // Считывание информации из таблицы отношений
            var sqlCommand = new SqlCommand
                                {
                                    Connection = sqlConnection,
                                    CommandText = @"SELECT * FROM [_Reltable] WHERE [Relations] <> '' OR [Via] <> ''"
                                };

            var reader = sqlCommand.ExecuteReader();
            while (reader.Read())
            {
                var table1 = (string)reader["Table1"];
                var table2 = (string)reader["Table2"];
                var via = reader["Via"] as string;

                // Проверка наличия непосредственного соединения
                if (reader["Relations"] is string relation)
                {
                    collectionOfAdjacentTables.Add(Tuple.Create(table1, table2), relation);
                    collectionOfAdjacentTables.Add(Tuple.Create(table2, table1), relation);
                }
                else
                {
                    collectionOfNonAdjacentTables.Add(Tuple.Create(table1, table2), via);
                    collectionOfNonAdjacentTables.Add(Tuple.Create(table2, table1), via);
                }
            }

            reader.Close();

            // Получение данных о столбцах в БД
            sqlCommand.CommandText = @"SELECT
	                                     [INFORMATION_SCHEMA].[COLUMNS].[TABLE_NAME], 
	                                     [INFORMATION_SCHEMA].[COLUMNS].[COLUMN_NAME], 
	                                     [INFORMATION_SCHEMA].[COLUMNS].[DATA_TYPE],
	                                     [_Fields].[Transl_fn],
                                         [_Fields].[Category_name]
                                    FROM 
	                                     [INFORMATION_SCHEMA].[COLUMNS] 
	                                     JOIN [_Fields] ON 
                                                    [_Fields].[Table_name] = [INFORMATION_SCHEMA].[COLUMNS].[TABLE_NAME] AND 
                                                    [_Fields].[Field_name] = [INFORMATION_SCHEMA].[COLUMNS].[COLUMN_NAME]
                                     WHERE 
	                                     [INFORMATION_SCHEMA].[COLUMNS].[TABLE_NAME] <> 'sysdiagrams' AND
	                                     [INFORMATION_SCHEMA].[COLUMNS].[TABLE_NAME] <> '_Reltable' AND
	                                     [INFORMATION_SCHEMA].[COLUMNS].[TABLE_NAME] <> '_Fields'";

            reader = sqlCommand.ExecuteReader();
            while (reader.Read())
            {
                mainCollectionOfAllFields.Add(
                    new Field(
                        (string)reader["COLUMN_NAME"],
                        (string)reader["Transl_fn"],
                        (string)reader["DATA_TYPE"],
                        (string)reader["TABLE_NAME"],
                        (string)reader["Category_name"]));
            }

            collectionOfAllFields = mainCollectionOfAllFields;
            UpdateCollectionsAllowedToSort();

            reader.Close();
            sqlConnection.Close();
        }

        private void UpdateAll()
        {
            lvAllFields.Items.Clear();
            lvSelectedFields.Items.Clear();
            lvAllFieldsToSort.Items.Clear();
            lvSelectedFieldsToSort.Items.Clear();
            dgvConditions.Rows.Clear();
            dgvConditions.Columns.Clear();
            dgvResult.Rows.Clear();
            dgvResult.Columns.Clear();
            mainCollectionOfAllFields.Clear();
            collectionOfAllFields.Clear();
            collectionOfConditions.Clear();
            collectionOfAllFieldsToSort.Clear();
            collectionOfAdjacentTables.Clear();
            collectionOfNonAdjacentTables.Clear();
            collectionOfSelectedFields.Clear();
            collectionOfSelectedFieldsToSort.Clear();
            IntializeContent();
            currentSqlQuerySelectPart = string.Empty;
            currentSqlQueryFromPart = string.Empty;
            currentSqlQueryWherePart = string.Empty;
            currentSqlQueryOrderPart = string.Empty;
        }

        private void UpdateCollectionsAllowedToSort()
        {
            lvAllFieldsToSort.Items.Clear();
            collectionOfAllFieldsToSort =
                new List<Field>(collectionOfSelectedFields.Except(collectionOfSelectedFieldsToSort));
            foreach (var field in collectionOfAllFieldsToSort)
            {
                var item = new ListViewItem { Name = field.Name, Tag = field, Text = field.DisplayedName };           
                lvAllFieldsToSort.Groups[field.CategoryName].Items.Add(item);
                lvAllFieldsToSort.Items.Add(item);
            }
        }

        private static DataGridViewCell ChooseRightCellType(Field value)
        {
            var type = new SqlType(value.Type).GetCSharpType();

            if (type == typeof(bool))
            {
                return new DataGridViewCheckBoxCell();
            }

            return type == typeof(DateTime) ? new CalendarCell() : new DataGridViewTextBoxCell();
        }

        #endregion

        #region Проверка/изменение подключения

        private void CheckConnection()
        {
            try
            {
                sqlConnection.Open();
                sqlConnection.Close();
            }
            catch (SqlException)
            {
                MessageBox.Show(@"Не удалось подключиться, проверьте подключение");
                new FormSetConnection(
                    new SqlConnection(
                        new SqlConnectionStringBuilder
                            {
                                DataSource = Properties.Settings.Default.DataSource,
                                InitialCatalog = Properties.Settings.Default.InitialCatalog,
                                IntegratedSecurity = true
                            }.ConnectionString)).ShowDialog();
            }
        }

        private void ChangeConnection()
        {
            new FormSetConnection(
                new SqlConnection(
                    new SqlConnectionStringBuilder
                        {
                            DataSource = Properties.Settings.Default.DataSource,
                            InitialCatalog = Properties.Settings.Default.InitialCatalog,
                            IntegratedSecurity = true
                        }.ConnectionString)).ShowDialog();
            IntializeConnection();
            CheckConnection();
            UpdateAll();
        }

        private void IntializeConnection()
        {
            connectionString = new SqlConnectionStringBuilder
                                   {
                                        DataSource = Properties.Settings.Default.DataSource,
                                        InitialCatalog = Properties.Settings.Default.InitialCatalog,
                                        IntegratedSecurity = Properties.Settings.Default.IntegratedSecurity == "true"
                                    }.ToString();
            сurrentSqlCommand = new SqlCommand
                                    {
                                        Connection = sqlConnection,
                                        CommandText = CurrentSqlQuery()
                                    };
            sqlConnection = new SqlConnection(connectionString);
        }

        #endregion

        #region Обработка событий dgvConditions
        //// TODO: Протестировать добавление условий
        private void DgvConditionsRowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (dgvConditions.IsCurrentRowDirty)
            {
                return;
            }

            // Считывание данных из dgv
            var row = dgvConditions.CurrentRow;
            var field = mainCollectionOfAllFields.FirstOrDefault(
                t => t.DisplayedName == (string)row.Cells["Field"].Value);
            var criterion = (string)row.Cells["Criterion"].Value;
            var expression = (string)row.Cells["Expression"].Value;
            var bunch = (string)row.Cells["Bunch"].Value;

            var errorFlag = false;

            if (field == null)
            {
                errorFlag = true;
                row.Cells["Field"].ErrorText = "Выберите поле";
            }

            if (criterion == null)
            {
                errorFlag = true;
                row.Cells["Criterion"].ErrorText = "Выберите критерий";
            }

            if (!errorFlag && new SqlType(field.Type).GetCSharpType() != typeof(string))
            {
                errorFlag = true;
                row.Cells["Criterion"].ErrorText = "Введите значение выражения";
            }

            if (bunch == null)
            {
                errorFlag = true;
                row.Cells["Bunch"].ErrorText = "Выберите связку";
            }

            if (errorFlag)
            {
                return;
            }

            // Выбор корректной связки
            bunch = bunch == "И" ? "AND" : "OR";

            // Добавление нового условия в коллекцию
            var condition = new Condition(field, new Criterion(criterion), expression, bunch);
            if (!collectionOfConditions.Contains(condition))
            {
                collectionOfConditions.Add(condition);
            }

            // Обновление текста запроса
            UpdateSqlQueryFromWherePart();

            // Удаление ошибок
            foreach (var cell in row.Cells)
            {
                ((DataGridViewCell)cell).ErrorText = string.Empty;
            }
        }

        private void DgvConditionsUserDeletingRows(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (dgvConditions.IsCurrentRowDirty)
            {
                return;
            }

            var row = dgvConditions.CurrentRow;
            var field = (string)row.Cells["Field"].Value;
            var criterion = (string)row.Cells["Criterion"].Value;
            var expression = (string)row.Cells["Expression"].Value;
            var bunch = (string)row.Cells["Bunch"].Value;

            // Удаление условия из коллекции
            var condition = new Condition(
                mainCollectionOfAllFields.Find(t => t.DisplayedName == field),
                new Criterion(criterion),
                expression,
                bunch);
            collectionOfConditions.Remove(condition);
        }

        private void DgvConditionsCellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            var row = dgvConditions.Rows[e.RowIndex];
            if (!row.IsNewRow)
            {
                dgvConditions[e.ColumnIndex, e.RowIndex].ErrorText = "Значение изменено и не сохранено";
            }

            if (!row.Cells["Field"].Selected || row.Selected || row.Cells["Field"].Value == null)
            {
                return;
            }

            // TODO: Сделать ячейки дат видимыми
            row.Cells["Expression"].ReadOnly = false;
            var field = mainCollectionOfAllFields.Find(t => t.DisplayedName == (string)row.Cells["Field"].Value);
            row.Cells["Expression"] = ChooseRightCellType(field);
            dgvConditions.Columns["Expression"].Visible = true;
            ((DataGridViewComboBoxCell)row.Cells["Criterion"]).DataSource = Criterion.GetAllowedOperations(field);
        }

        private void DgvConditionsRowEnter(object sender, DataGridViewCellEventArgs e)
        {
            // TODO: Добавить проверку остальных строк
            if (dgvConditions.Rows[e.RowIndex].Cells["Field"].Value == null)
            {
                dgvConditions.Rows[e.RowIndex].Cells["Expression"].ErrorText =
                    "Для ввода значения выражения сначала нужно выбрать изменяемое поле";
            }
        }

        private void DgvConditionsDataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            //ignore
        }

        #endregion

        #region Генерация запроса и изменение строки соединения

        private void BtChangeConnectionClick(object sender, EventArgs e) => ChangeConnection();

        private void BtShowSqlQueryClick(object sender, EventArgs e) => MessageBox.Show(CurrentSqlQuery());

        private void BtExecuteClick(object sender, EventArgs e)
        {
            sqlConnection.Open();
            UpdateSqlQuery();
            dgvResult.AllowUserToResizeColumns = true;
            dgvResult.Columns.Clear();
            dgvResult.Rows.Clear();
            
            try
            {
                сurrentSqlCommand.Connection = sqlConnection;
                сurrentSqlCommand.CommandText = CurrentSqlQuery();
                var reader = сurrentSqlCommand.ExecuteReader();
                FillingResults(reader);
                MessageBox.Show(@"Запрос выполнен");
                sqlConnection.Close();
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show(@"Ни одно поле не выбрано для демонстрации");
                sqlConnection.Close();
            }
        }

        private void FillingResults(IDataReader reader)
        {
            foreach (var field in collectionOfSelectedFields)
            {
                dgvResult.Columns.Add(field.GetFullName(), field.DisplayedName);
            }

            while (reader.Read())
            {
                var row = new DataGridViewRow();
                for (var i = 0; i < collectionOfSelectedFields.Count; i++)
                {
                    row.Cells.Add(new DataGridViewTextBoxCell { Value = reader[i] });
                }

                dgvResult.Rows.Add(row);
            }

            dgvResult.AutoResizeColumns();
        }

        private void BtCancelClick(object sender, EventArgs e) => UpdateAll();

        private string CurrentSqlQuery() =>
            $"{currentSqlQuerySelectPart}{currentSqlQueryFromPart}{currentSqlQueryWherePart}{currentSqlQueryOrderPart}";

        private void UpdateSqlQuery()
        {
            UpdateSqlQuerySelectPart();
            UpdateSqlQueryFromWherePart();
            UpdateSqlQueryOrderPart();
        }

        private void UpdateSqlQuerySelectPart()
        {
            if (!collectionOfSelectedFields.Any())
            {
                currentSqlQuerySelectPart = string.Empty;
                return;
            }

            // Добавление полей, выбранных для вывода, в запрос (блок SELECT)
            var fieldNames = collectionOfSelectedFields.Select(t => t.GetFullName()).ToList();
            currentSqlQuerySelectPart = "SELECT DISTINCT " + string.Join(", ", fieldNames);
        }

        private void UpdateSqlQueryFromWherePart()
        {
            // Список таблиц, поля которых выбраны для вывода
            var tableNames = collectionOfSelectedFields.Select(t => t.TableName).Distinct().ToList();

            if (!tableNames.Any())
            {
                currentSqlQuerySelectPart = string.Empty;
                currentSqlQueryFromPart = string.Empty;
                currentSqlQueryWherePart = string.Empty;
                currentSqlQueryOrderPart = string.Empty;
                return;
            }

            // Поиск таблиц, поля которых используются в условиях, но значения которых не предназначены для вывода
            foreach (var condition in collectionOfConditions)
            {
                var table = condition.Field.TableName;
                if (!tableNames.Contains(table))
                {
                    tableNames.Add(table);
                }
            }

            GetJoinConditions(tableNames, out var joinList, out var tablesList);

            // Добавление таблиц, содержащие поля, выбранные для вывода, в запрос (блок FROM)
            currentSqlQueryFromPart = "\nFROM " + string.Join(", ", tablesList);

            if (joinList != null)
            {
                // Добавление связей между таблицами (блок WHERE)
                currentSqlQueryWherePart = "\nWHERE " + string.Join(" AND ", joinList);
            }

            // Добавление условий в запрос
            for (var i = 0; i < collectionOfConditions.Count; i++)
            {
                var condition = collectionOfConditions[i];
                var expression = condition.Expression;
                string expressionString;
                switch (expression)
                {
                    case DateTime _:
                        expressionString = "{" + expression + "}";
                        break;
                    case bool _:
                        expressionString = "\'" + expression + "\'";
                        break;
                    default:
                        expressionString = "\'" + expression + "\'";
                        break;
                }

                var sqlTranslating = i != Top
                                         ? condition.Field.GetFullName() + " " + condition.Criterion.GetValue() + " "
                                           + expressionString + " " + condition.Bunch + "\n"
                                         : condition.Field.GetFullName() + " " + condition.Criterion.GetValue() + " "
                                           + expressionString + "\n";
                currentSqlQueryWherePart += sqlTranslating;
            }
        }

        private void UpdateSqlQueryOrderPart()
        {
            try
            {
                if (!collectionOfSelectedFieldsToSort.Any())
                {
                    currentSqlQueryOrderPart = string.Empty;
                    return;
                }
            }
            catch (Exception)
            {
                currentSqlQueryOrderPart = string.Empty;
                return;
            }

            currentSqlQueryOrderPart = "\nORDER BY ";
            collectionOfSelectedFieldsToSort.Sort(new OrderFieldComparer());
            foreach (var orderedField in collectionOfSelectedFieldsToSort)
            {
                currentSqlQueryOrderPart += orderedField.GetFullName() + " " + orderedField.Order + ", ";
            }

            currentSqlQueryOrderPart = currentSqlQueryOrderPart.Remove(currentSqlQueryOrderPart.Length - 2, 2);
        }

        #endregion

        #region Выбор полей

        // Обмен всеми полями
        private void TransferringFieldsBetweenListView(
            ref ListView sourceListView,
            ref List<Field> sourceFields,
            ref ListView targetListView,
            ref List<Field> targetFields)
        {
            foreach (ListViewItem item in sourceListView.Items)
            {
                sourceFields.Remove((Field)item.Tag);
                targetFields.Add((Field)item.Tag);
                sourceListView.Items.Remove(item);
                targetListView.Groups[((Field)item.Tag).CategoryName].Items.Add(item);
                targetListView.Items.Add(item);
            }

            UpdateSqlQuery();
        }

        // Обмен выбранными полями
        private void TransferringSelectedFieldsBetweenListView(
            ref ListView sourceListView,
            ref List<Field> sourceFields,
            ref ListView targetListView,
            ref List<Field> targetFields)
        {
            foreach (ListViewItem item in sourceListView.SelectedItems)
            {
                sourceFields.Remove((Field)item.Tag);
                targetFields.Add((Field)item.Tag);
                sourceListView.Items.Remove(item);                
                targetListView.Groups[((Field)item.Tag).CategoryName].Items.Add(item);
                targetListView.Items.Add(item);
            }

                UpdateSqlQuery();
        }

        private void BtAddClick(object sender, EventArgs e)
        {
            TransferringSelectedFieldsBetweenListView(
                ref lvAllFields,
                ref collectionOfAllFields,
                ref lvSelectedFields,
                ref collectionOfSelectedFields);

            UpdateCollectionsAllowedToSort();
        }

        private void BtDeleteClick(object sender, EventArgs e)
        {
            TransferringSelectedFieldsBetweenListView(
                ref lvSelectedFields,
                ref collectionOfSelectedFields,
                ref lvAllFields,
                ref collectionOfAllFields);

            UpdateCollectionsAllowedToSort();
        }

        private void BtAddAllClick(object sender, EventArgs e)
        {
            TransferringFieldsBetweenListView(
                ref lvAllFields,
                ref collectionOfAllFields,
                ref lvSelectedFields,
                ref collectionOfSelectedFields);

            UpdateCollectionsAllowedToSort();
        }

        private void BtDeleteAllClick(object sender, EventArgs e)
        {
            TransferringFieldsBetweenListView(
                ref lvSelectedFields,
                ref collectionOfSelectedFields,
                ref lvAllFields,
                ref collectionOfAllFields);

            UpdateCollectionsAllowedToSort();
        }

        #endregion

        #region Выбор порядка

        private void BtAddToSortClick(object sender, EventArgs e)
        {
            var selectedItems = lvAllFieldsToSort.SelectedItems;
            foreach (ListViewItem item in selectedItems)
            {
                var field = collectionOfAllFieldsToSort.Find(t => t == (Field)item.Tag);
                collectionOfAllFieldsToSort.Remove(field);
                lvAllFieldsToSort.Items.Remove(item);
                var newItem =
                    new ListViewItem(new[] { item.Text, "По возрастанию" }) { Tag = new OrderedField(field, "ASC", lvSelectedFieldsToSort.Items.Count) };
                lvSelectedFieldsToSort.Items.Add(newItem);
                lvSelectedFieldsToSort.Groups[field.CategoryName].Items.Add(newItem);
                collectionOfSelectedFieldsToSort.Add(
                    new OrderedField(field, "ASC", newItem.Index));
            }

            UpdateCollectionsAllowedToSort();
            UpdateSqlQueryOrderPart();
        }

        private void BtDeleteFromSortClick(object sender, EventArgs e)
        {
            var selectedItems = lvSelectedFieldsToSort.SelectedItems;
            foreach (ListViewItem item in selectedItems)
            {
                var field = collectionOfSelectedFieldsToSort.Find(t => t == (OrderedField)item.Tag);
                collectionOfSelectedFieldsToSort.Remove(field);
                collectionOfAllFieldsToSort.Add(field);
                lvSelectedFieldsToSort.Items.Remove(item);
                var newItem = new ListViewItem(new[] { item.Text }) { Tag = field.ToField() };
                lvAllFieldsToSort.Items.Add(newItem);
            }

            UpdateCollectionsAllowedToSort();
            UpdateSqlQueryOrderPart();
        }

        private void BtAddAllToSortClick(object sender, EventArgs e)
        {
            var selectedItems = lvAllFieldsToSort.Items;
            foreach (ListViewItem item in selectedItems)
            {
                var field = collectionOfAllFieldsToSort.Find(t => t == (Field)item.Tag);
                collectionOfAllFieldsToSort.Remove(field);
                lvAllFieldsToSort.Items.Remove(item);
                var newItem =
                    new ListViewItem(new[] { item.Text, "По возрастанию" }) { Tag = new OrderedField(field, "ASC", lvSelectedFieldsToSort.Items.Count) };
                lvSelectedFieldsToSort.Items.Add(newItem);
                lvSelectedFieldsToSort.Groups[field.CategoryName].Items.Add(newItem);
                collectionOfSelectedFieldsToSort.Add(
                    new OrderedField(field, "ASC", newItem.Index));
            }

            UpdateCollectionsAllowedToSort();
            UpdateSqlQueryOrderPart();
        }

        private void BtDeleteAllFromSortClick(object sender, EventArgs e)
        {
            var selectedItems = lvSelectedFieldsToSort.Items;
            foreach (ListViewItem item in selectedItems)
            {
                var field = collectionOfSelectedFieldsToSort.Find(t => t == (OrderedField)item.Tag);
                collectionOfSelectedFieldsToSort.Remove(field);
                collectionOfAllFieldsToSort.Add(field);
                lvSelectedFieldsToSort.Items.Remove(item);
                var newItem = new ListViewItem(new[] { item.Text }) { Tag = field.ToField() };
                lvAllFieldsToSort.Items.Add(newItem);
            }

            UpdateCollectionsAllowedToSort();
            UpdateSqlQueryOrderPart();
        }

        private void BtAscClick(object sender, EventArgs e)
        {
            var selectedItems = lvSelectedFieldsToSort.SelectedItems;
            if (selectedItems.Count == 0)
            {
                return;
            }

            foreach (ListViewItem item in selectedItems)
            {
                var orderedField = collectionOfSelectedFieldsToSort.Find(t => t == (OrderedField)item.Tag);
                collectionOfSelectedFieldsToSort.Remove(orderedField);
                orderedField.Order = "ASC";
                collectionOfSelectedFieldsToSort.Add(orderedField);
                item.SubItems[1].Text = @"По возрастанию";
            }

            UpdateSqlQueryOrderPart();
        }

        private void BtDescClick(object sender, EventArgs e)
        {
            var selectedItems = lvSelectedFieldsToSort.SelectedItems;
            if (selectedItems.Count == 0)
            {
                return;
            }

            foreach (ListViewItem item in selectedItems)
            {
                var orderedField = collectionOfSelectedFieldsToSort.Find(t => t == (OrderedField)item.Tag);
                collectionOfSelectedFieldsToSort.Remove(orderedField);
                orderedField.Order = "DESC";
                collectionOfSelectedFieldsToSort.Add(orderedField);
                item.SubItems[1].Text = @"По убыванию";
            }

            UpdateSqlQueryOrderPart();
        }

        private void ReOrder(bool order)
        {
            var items = lvSelectedFieldsToSort.Items;
            var selectedItems = lvSelectedFieldsToSort.SelectedItems;
            if (selectedItems.Count == 0 || items.Count == selectedItems.Count)
            {
                return;
            }

            var newOrder = new Dictionary<int, ListViewItem>();

            if (order)
            {
                for (var i = 0; i < selectedItems.Count; i++)
                {
                    var item = selectedItems[i];
                    var index = item.Index;
                    if (index != 0 && !newOrder.ContainsKey(index - 1))
                    {
                        index--;
                    }

                    newOrder.Add(index, item);
                }
            }
            else
            {
                for (var i = selectedItems.Count - 1; i >= 0; i--)
                {
                    var item = selectedItems[i];
                    var index = item.Index;
                    if (index != items.Count - 1 && !newOrder.ContainsKey(index + 1))
                    {
                        index++;
                    }

                    newOrder.Add(index, item);
                }
            }

            foreach (ListViewItem item in items)
            {
                if (newOrder.ContainsValue(item))
                {
                    continue;
                }

                var index = FindRightIndex(item.Index, newOrder, order);
                newOrder.Add(index, item);
            }

            lvSelectedFieldsToSort.Items.Clear();
            collectionOfSelectedFieldsToSort.Clear();

            foreach (var item in newOrder.OrderBy(t => t.Key))
            {
                lvSelectedFieldsToSort.Items.Insert(item.Key, item.Value);
                var newOrderedField = (OrderedField)item.Value.Tag;
                newOrderedField.OrderIndex = item.Key;
                collectionOfSelectedFieldsToSort.Add(newOrderedField);
            }

            UpdateSqlQueryOrderPart();
        }

        private void BtUpClick(object sender, EventArgs e)
        {
            ReOrder(true);
        }

        private void BtDownClick(object sender, EventArgs e)
        {
            ReOrder(false);
        }

        // ReSharper disable once MemberCanBeMadeStatic.Local
        private int FindRightIndex(int oldIndex, IReadOnlyDictionary<int, ListViewItem> source, bool order) =>
            // ReSharper disable once TailRecursiveCall
            source.ContainsKey(oldIndex) ? FindRightIndex(order ? ++oldIndex : --oldIndex, source, order) : oldIndex;

        #endregion

        #region Генерация связей между таблицами

        private void GetJoinConditions(
            IReadOnlyList<string> tablesForJoining,
            out List<string> joinList,
            out List<string> tablesList)
        {
            switch (tablesForJoining.Count)
            {
                case 0:
                    {
                        tablesList = null;
                        joinList = null;
                        return;
                    }

                case 1:
                    {
                        tablesList = tablesForJoining.ToList();
                        joinList = null;
                        return;
                    }

                default:
                    {
                        var firstTable = tablesForJoining.First();

                        joinList = new List<string>();
                        tablesList = new List<string> { firstTable };

                        for (var i = 1; i < tablesForJoining.Count; i++)
                        {
                            var table = tablesForJoining[i];
                            if (tablesList.Contains(table))
                            {
                                continue;
                            }

                            tablesList.Add(table);
                            var tuple = Tuple.Create(firstTable, table);

                            if (collectionOfAdjacentTables.ContainsKey(tuple))
                            {
                                var relation = collectionOfAdjacentTables[tuple];
                                if (!joinList.Contains(relation))
                                {
                                    joinList.Add(relation);
                                }
                            }
                            else
                            {
                                // Проверка наличия косвенной связи между таблицами
                                if (!collectionOfNonAdjacentTables.ContainsKey(tuple))
                                {
                                    throw new Exception("Таблицы не связаны!");
                                }

                                // Поиск промежуточной таблицы
                                var intermedianteTable = collectionOfNonAdjacentTables[tuple];

                                // Построение через промежуточную таблицу
                                var path =
                                    collectionOfAdjacentTables.ContainsKey(
                                        Tuple.Create(tuple.Item1, intermedianteTable))
                                        ? CreatePath(Tuple.Create(tuple.Item1, intermedianteTable), tuple.Item2)
                                            .Split('\"')
                                        : CreatePath(Tuple.Create(tuple.Item2, intermedianteTable), tuple.Item1)
                                            .Split('\"');

                                // TODO: Ошибка при добавлении факультетов
                                // Добавление построенного пути в коллекцию отношений
                                for (var j = 0; j < path.Length - 1; j++)
                                {
                                    var t1 = path[j];
                                    var t2 = path[j + 1];
                                    var relation = collectionOfAdjacentTables[Tuple.Create(t1, t2)];
                                    if (!tablesList.Contains(t2))
                                    {
                                        tablesList.Add(t2);
                                    }

                                    if (!joinList.Contains(relation))
                                    {
                                        joinList.Add(relation);
                                    }
                                }
                            }
                        }
                    }

                    break;
            }
        }

        private string CreatePath(Tuple<string, string> tables, string endTable)
        {
            var currentTables = Tuple.Create(tables.Item2, endTable);
            if (!collectionOfNonAdjacentTables.ContainsKey(currentTables))
            {
                return tables.Item1 + "\"" + tables.Item2 + "\"" + endTable;
            }

            return tables.Item1 + "\"" + CreatePath(
                       Tuple.Create(tables.Item2, collectionOfNonAdjacentTables[currentTables]),
                       endTable);
        }

        #endregion
    }
}