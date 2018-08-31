// ReSharper disable StaticMemberInitializerReferesToMemberBelow
// ReSharper disable StyleCop.SA1204
namespace NonStandartQuery
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Windows.Forms;

    using NonStandartQuery.Classes;

    using Workstation;

    /// <inheritdoc />
    /// <summary>
    /// The form main.
    /// </summary>
    public partial class FormMain : Form
    {
        #region Описание данных для БД

        private static readonly string ConnectionString =
            new SqlConnectionStringBuilder
                {
                    DataSource = Properties.Settings.Default.DataSource,
                    InitialCatalog = Properties.Settings.Default.InitialCatalog,
                    IntegratedSecurity = Properties.Settings.Default.IntegratedSecurity == "true"
                }.ToString();

        private static readonly SqlCommand СurrentSqlCommand =
            new SqlCommand { Connection = SqlConnection, CommandText = CurrentSqlQuery() };

        private static readonly SqlConnection SqlConnection = new SqlConnection(ConnectionString);

        private static string currentSqlQueryFieldsPart;

        private static string currentSqlQueryConditionsPart;

        private static string currentSqlQueryOrderPart;

        #endregion

        #region Описание контейнеров данных

        private readonly Dictionary<Tuple<string, string>, string> collectionOfAdjacentTables =
            new Dictionary<Tuple<string, string>, string>();

        private readonly Dictionary<Tuple<string, string>, string> collectionOfNonAdjacentTables =
            new Dictionary<Tuple<string, string>, string>();

        private readonly List<Field> mainCollectionOfAllFields = new List<Field>();
        private List<Field> collectionOfAllFields = new List<Field>();
        private List<Field> collectionOfSelectedFields = new List<Field>();
        private List<Condition> collectionOfConditions = new List<Condition>();
        private List<Field> collectionOfAllFieldsToSort = new List<Field>();
        private List<Field> collectionOfSelectedFieldsToSort = new List<Field>();
        private List<DataGridViewRow> collectionOfResultRows = new List<DataGridViewRow>();

        #endregion

        public FormMain()
        {
            CheckConnection();
            CheckDataBase();
            InitializeComponent();
            IntializeContent();
        }

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
            lvAllFields.View = View.List;
            foreach (var field in collectionOfAllFields)
            {
                if (lvAllFields.FindItemWithText("/////" + field.CategoryName + "/////") == null)
                {
                    lvAllFields.Items.Add("/////" + field.CategoryName + "/////");
                }

                lvAllFields.Items.Add(field.ToString());
            }    
        }

        private void IntializeLvSelectedFields()
        {
            lvSelectedFields.Alignment = ListViewAlignment.Default;
            lvSelectedFields.FullRowSelect = true;
            lvSelectedFields.GridLines = true;
            lvSelectedFields.MultiSelect = true;
            lvSelectedFields.View = View.List;
        }

        private void IntializeDgvConditions()
        {
            dgvConditions.RowValidating += DgvConditionsRowValidating;
            dgvConditions.UserDeletingRow += DgvConditionsUserDeletingRows;
            dgvConditions.CellValueChanged += DgvConditionsCellValueChanged;
            dgvConditions.RowEnter += DgvConditionsRowEnter;
            dgvConditions.DataError += DgvConditionsDataError;
            dgvConditions.Columns.Add(new DataGridViewComboBoxColumn
            {
                DataPropertyName = "DisplayedName",
                DataSource = mainCollectionOfAllFields,
                Name = @"Field",
                HeaderText = @"Поле"
            });

            dgvConditions.Columns.Add(new DataGridViewComboBoxColumn
            {
                DataPropertyName = "Criterion",
                HeaderText = @"Критерий",
                Name = @"Criterion"
            });
            dgvConditions.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Expression",
                HeaderText = @"Выражение",
                Name = @"Expression",
                ReadOnly = true
            });
            dgvConditions.Columns.Add(new DataGridViewComboBoxColumn
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
            lvAllFieldsToSort.View = View.List;
        }

        private void IntializeLvSelectedFieldsToSort()
        {
            lvSelectedFieldsToSort.Alignment = ListViewAlignment.Default;
            lvSelectedFieldsToSort.FullRowSelect = true;
            lvSelectedFieldsToSort.GridLines = true;
            lvSelectedFieldsToSort.MultiSelect = true;
            lvSelectedFieldsToSort.View = View.List;
        }

        private void IntializeDgvResult()
        {
            // TODO: Добавить корректные заголовки и типы ячеек
            dgvResult.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "DisplayedName",
                HeaderText = string.Empty
            });
            dgvResult.AllowUserToAddRows = false;
            dgvResult.AllowUserToDeleteRows = false;
            dgvResult.AllowUserToOrderColumns = false;
            dgvResult.AllowUserToResizeColumns = false;
            dgvResult.AllowUserToResizeRows = false;
            dgvResult.DataSource = collectionOfResultRows;
        }

        #endregion

        #region Методы связанные с контейнерами данных

        private void UpdateAll()
        {
            // TODO: Добавить работающее обновление
            lvAllFields.Items.Clear();
            lvSelectedFields.Clear();
            lvAllFieldsToSort.Clear();
            lvSelectedFieldsToSort.Clear();
            mainCollectionOfAllFields.Clear();
            collectionOfAllFields.Clear();
            collectionOfConditions.Clear();
            collectionOfAllFieldsToSort.Clear();
            collectionOfAdjacentTables.Clear();
            collectionOfNonAdjacentTables.Clear();
            collectionOfSelectedFields.Clear();
            collectionOfSelectedFieldsToSort.Clear();
            collectionOfResultRows.Clear();
            IntializeCollections();
            UpdateSqlQuerySelectPart();
            UpdateSqlQueryFromWherePart();
            UpdateSqlQueryOrderPart();
        }

        private void UpdateCollectionsAllowedToSort()
        {
            collectionOfAllFieldsToSort = collectionOfSelectedFields;
        }

        private void IntializeCollections()
        {
            SqlConnection.Open();

            // Считывание информации из таблицы отношений
            var sqlCommand = new SqlCommand
            {
                Connection = SqlConnection,
                CommandText = @"SELECT * FROM [_Reltable] WHERE [Relations] <> NULL OR [Via] <> NULL"
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
            SqlConnection.Close();
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
                SqlConnection.Open();
                SqlConnection.Close();
            }
            catch (SqlException)
            {
                var formSetConnection = new FormSetConnection();
                formSetConnection.ShowDialog();
            }
        }

        private void CheckDataBase()
        {
            SqlConnection.Open();

            var sqlCommand = new SqlCommand
            {
                Connection = SqlConnection,
                CommandText = @"SELECT COUNT (DISTINCT 
	                                INFORMATION_SCHEMA.COLUMNS.TABLE_NAME)
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
                SqlConnection.Close();
                throw new NotImplementedException();
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
                SqlConnection.Close();
                throw new NotImplementedException();
            }

            SqlConnection.Close();
        }

        private void ChangeConnection()
        {
            CheckConnection();
            CheckDataBase();
            UpdateAll();
            IntializeCollections();
        }

        #endregion

        #region Обработка событий dgvConditions

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

            #region Проверка ячеек на пустоту

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

            if (new SqlType(field.Type).GetCSharpType() != typeof(string))
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

            #endregion

            // Выбор корректной связки
            bunch = bunch == "И" ? "AND" : "OR";

            // Добавление нового условия в коллекцию
            var condition = new Condition(
                field,
                new Criterion(criterion),
                expression,
                bunch);
            collectionOfConditions.Add(condition);

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

            // TODO: добавить поиск по критерию
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

            #region Генерация ячеек "Критерий" и "Выражение", в зависимости от типа выбранного поля 

            if (!row.Cells["Field"].Selected || row.Selected || row.Cells["Field"].Value == null)
            {
                return;
            }

            // TODO: Сделать ячейки дат видимыми
            row.Cells["Expression"].ReadOnly = false;
            var field = mainCollectionOfAllFields.Find(t =>
                t.DisplayedName == (string)row.Cells["Field"].Value);
            row.Cells["Expression"] =
                ChooseRightCellType(field);
            ((DataGridViewComboBoxCell)row.Cells["Criterion"]).DataSource = Criterion.GetAllowedOperations(field);

            #endregion
        }

        private void DgvConditionsRowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvConditions.Rows[e.RowIndex].Cells["Field"].Value == null)
            {
                dgvConditions.Rows[e.RowIndex].Cells["Expression"].ErrorText =
                    "Для ввода значения выражения сначала нужно выбрать изменяемое поле";
            }
        }

        private void DgvConditionsDataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Генерация запроса и изменение строки соединения
        // TODO: Добавить смену подключения
        private void BtChangeConnectionClick(object sender, EventArgs e) => ChangeConnection();

        private void BtShowSqlQueryClick(object sender, EventArgs e) => MessageBox.Show(CurrentSqlQuery());

        private void BtExecuteClick(object sender, EventArgs e)
        {
            SqlConnection.Open();
            try
            {
                СurrentSqlCommand.ExecuteReader();
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show(@"Ни одно поле не выбрано для демонстрации");
                SqlConnection.Close();
            }
            
            //// TODO: Добавить заполнение результирующего dgv

            SqlConnection.Close();
        }

        private void BtCancelClick(object sender, EventArgs e) => UpdateAll();

        private static string CurrentSqlQuery() =>
            $"{currentSqlQueryFieldsPart}{currentSqlQueryConditionsPart}{currentSqlQueryOrderPart}";

        private void UpdateSqlQuerySelectPart()
        {
            if (!collectionOfSelectedFields.Any())
            {
                throw new NotImplementedException();
            }

            // Добавление полей, выбранных для вывода, в запрос (блок SELECT)
            var fieldNames = collectionOfSelectedFields.Select(t => t.GetFullName()).ToList();
            currentSqlQueryFieldsPart = "SELECT DISTINCT " + string.Join(", ", fieldNames);
        }

        private void UpdateSqlQueryFromWherePart()
        {
            // Список таблиц, поля которых выбраны для вывода
            var tableNames = collectionOfSelectedFields.Select(t => t.TableName).Distinct().ToList();

            if (!tableNames.Any())
            {
                throw new NotImplementedException();
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
            currentSqlQueryFieldsPart = "\nFROM " + string.Join(", ", tablesList);

            // Добавление связей между таблицами (блок WHERE)
            currentSqlQueryFieldsPart += "\nWHERE " + string.Join(" AND ", joinList);

            // Условия
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
                    ? condition.Field.GetFullName() + " " + condition.Criterion.GetValue() + " " + expressionString
                    + " " + condition.Bunch + "\n"
                    : condition.Field.GetFullName() + " " + condition.Criterion.GetValue() + " " + expressionString
                    + "\n";
                currentSqlQueryConditionsPart += sqlTranslating;
            }
        }

        private void UpdateSqlQueryOrderPart()
        {
            currentSqlQueryOrderPart = collectionOfSelectedFieldsToSort.Any() ? "ORDER BY " : string.Empty;
            foreach (var field in collectionOfSelectedFieldsToSort)
            {
            }
        }

        #endregion

        #region Выбор полей

        private void BtAddClick(object sender, EventArgs e)
        {
            // TODO: Добавить связь lvItems и fields
            var selectedItems = lvAllFields.SelectedItems;
            foreach (Field item in selectedItems)
            {
                collectionOfAllFields.Remove(item);
                collectionOfSelectedFields.Add(item);
                lvAllFields.Sort();
                lvSelectedFields.Sort();
            }

            UpdateCollectionsAllowedToSort();
            UpdateSqlQuerySelectPart();
        }

        private void BtDeleteClick(object sender, EventArgs e)
        {
            var selectedItems = lvSelectedFields.SelectedItems;
            foreach (ListViewItem item in selectedItems)
            {
                lvSelectedFields.Items.Remove(item);
                lvAllFields.Items.Add(item);
                lvSelectedFields.Sort();
                lvAllFields.Sort();
            }

            UpdateCollectionsAllowedToSort();
            UpdateSqlQuerySelectPart();
        }

        private void BtAddAllClick(object sender, EventArgs e)
        {
            var selectedItems = lvAllFields.Items;
            foreach (ListViewItem item in selectedItems)
            {
                lvAllFields.Items.Remove(item);
                lvSelectedFields.Items.Add(item);
                lvAllFields.Sort();
                lvSelectedFields.Sort();
            }

            UpdateCollectionsAllowedToSort();
            UpdateSqlQuerySelectPart();
        }

        private void BtDeleteAllClick(object sender, EventArgs e)
        {
            var selectedItems = lvSelectedFields.Items;
            foreach (ListViewItem item in selectedItems)
            {
                lvSelectedFields.Items.Remove(item);
                lvAllFields.Items.Add(item);
                lvSelectedFields.Sort();
                lvAllFields.Sort();
            }

            UpdateCollectionsAllowedToSort();
            UpdateSqlQuerySelectPart();
        }

        #endregion

        #region Выбор порядка
        // TODO: Добавить связь для выбора направления сортировки
        private void BtAddToSortClick(object sender, EventArgs e)
        {
            var selectedItems = lvAllFieldsToSort.SelectedItems;
            foreach (Field item in selectedItems)
            {
                collectionOfAllFieldsToSort.Remove(item);
                collectionOfSelectedFieldsToSort.Add(item);
            }

            UpdateSqlQueryOrderPart();
        }

        private void BtDeleteFromSortClick(object sender, EventArgs e)
        {
            var selectedItems = lvAllFieldsToSort.SelectedItems;
            foreach (Field item in selectedItems)
            {
                collectionOfSelectedFieldsToSort.Remove(item);
                collectionOfAllFieldsToSort.Add(item);
            }

            UpdateSqlQueryOrderPart();
        }

        private void BtAddAllToSortClick(object sender, EventArgs e)
        {
            var selectedItems = lvAllFieldsToSort.Items;
            foreach (Field item in selectedItems)
            {
                collectionOfAllFieldsToSort.Remove(item);
                collectionOfSelectedFieldsToSort.Add(item);
            }

            UpdateSqlQueryOrderPart();
        }

        private void BtDeleteAllFromSortClick(object sender, EventArgs e)
        {
            var selectedItems = lvSelectedFieldsToSort.Items;
            foreach (Field item in selectedItems)
            {
                collectionOfSelectedFieldsToSort.Remove(item);
                collectionOfAllFieldsToSort.Add(item);
            }

            UpdateSqlQueryOrderPart();
        }

        private void CbOrderSelectedIndexChanged(object sender, EventArgs e)
        {

        }

        #endregion

        #region Генерация связей между таблицами

        private void GetJoinConditions(IReadOnlyList<string> tablesForJoining, out List<string> joinList, out List<string> tablesList)
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
                                var path = collectionOfAdjacentTables.ContainsKey(Tuple.Create(tuple.Item1, intermedianteTable))
                                    ? CreatePath(Tuple.Create(tuple.Item1, intermedianteTable), tuple.Item2).Split()
                                    : CreatePath(Tuple.Create(tuple.Item2, intermedianteTable), tuple.Item1).Split();

                                // Добавление построенного пути в коллекцию отношений
                                for (var j = 0; j < path.Length - 1; j++)
                                {
                                    var t1 = path[j];
                                    var t2 = path[j + 1];
                                    var relation = collectionOfAdjacentTables[Tuple.Create(t1, t2)];
                                    tablesList.Add(t2);

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

            return tables.Item1 + "\'" + CreatePath(
                       Tuple.Create(tables.Item2, collectionOfNonAdjacentTables[currentTables]),
                       endTable);
        }

        #endregion

        //#region Доделать

        //private void CreateQuery()
        //{
        //    //// Сортировка

        //    var allConditionsParam = new List<string>();
        //    var allConditions = new List<string>();
        //    var query = sCommand.CommandText;

        //    if (conditionsParam.Any())
        //    {
        //        allConditionsParam.Add("(");
        //        allConditions.Add("(");

        //        allConditionsParam.AddRange(conditionsParam);
        //        allConditions.AddRange(conditions);

        //        allConditionsParam.Add(")");
        //        allConditions.Add(")");
        //    }


        //    if (allConditionsParam.Any())
        //    {
        //        sCommand.CommandText += " WHERE " + string.Join(" ", allConditionsParam);
        //        query += " WHERE " + string.Join(" ", allConditions);
        //    }

        //    // Если в листе для сортировки есть что нибудь, тогда добаляем ORDER BY и поля сортировки
        //    if (!orderList.Any()) return Tuple.Create(sCommand, query);
        //    sCommand.CommandText += " ORDER BY " + string.Join(", ", orderList);
        //    query += " ORDER BY " + string.Join(", ", orderList);

        //    // Возвращаем готовую комманду
        //    return Tuple.Create(sCommand, query);
        //}

        //#region Элементы со вкладки "Условие"
        ////
        //// Обработчик изменения выделенного элемента в комбобоксе
        ////
        //private void CbxNameFieldSelectedIndexChanged(object sender, EventArgs e)
        //{
        //    // Очищаем комбобокс критериев (нужно, к примеру, если после числового выбрали текстовое поле)
        //    cbxCriterion.Items.Clear();

        //    // Преобразуем имя поля к типу столбца
        //    var curColumn = (Column)cbxNameField.SelectedItem;
        //    var curType = curColumn.Type;
        //    // В зависимости от типа, заполняем комбобокс критериев
        //    switch (curType)
        //    {
        //        case "date":
        //        case "datetime":
        //            dtpExpr.Visible = true;
        //            cbxExpression.Visible = false;
        //            cbxCriterion.Items.AddRange(new object[] { "<", ">", "=", "<>" });
        //            break;
        //        case "int":
        //        case "real":
        //        case "float":
        //            dtpExpr.Visible = false;
        //            cbxExpression.Visible = true;
        //            cbxCriterion.Items.AddRange(new object[] { "<", ">", "=", "<>" });
        //            break;
        //        case "nvarchar":
        //        case "varchar":
        //        case "nchar":
        //        case "char":
        //            dtpExpr.Visible = false;
        //            cbxExpression.Visible = true;
        //            cbxCriterion.Items.AddRange(new object[] { "=", "<>", "LIKE" });
        //            break;
        //        default: MessageBox.Show(@"Тип данного столбца еще не обработан"); break;
        //    }

        //    using (var sConn = new SqlConnection(_sConnStr))
        //    {
        //        sConn.Open();
        //        var qName = curColumn.FullColumnName;
        //        var table = curColumn.NameTable;
        //        var column = curColumn.Name;

        //        // Берем из БД данные которые уже есть в этом столбце
        //        var sCommand = new SqlCommand
        //        {
        //            Connection = sConn,
        //            CommandText = @"SELECT DISTINCT" + qName + " " + "FROM " + table
        //        };

        //        var reader = sCommand.ExecuteReader();
        //        var dataInColumns = new List<string>();

        //        while (reader.Read())
        //        {
        //            var vItem = reader[column].ToString();
        //            dataInColumns.Add(vItem);
        //        }

        //        // Очищаем комбобокс выражений и добавляем туда новые выражения
        //        cbxExpression.Items.Clear();
        //        cbxExpression.Items.AddRange(dataInColumns?.ToArray());
        //    }
        //}

        ////
        //// Добавление условия 
        ////
        //private void BtnAddConditionClick(object sender, EventArgs e)
        //{
        //    if (lvConditions.Items.Count > 0 && lvConditions.Items[lvConditions.Items.Count - 1].SubItems[3].Text == string.Empty)
        //    {
        //        MessageBox.Show(@"Нет связки между условиями!", @"Неудача");
        //        return;
        //    }

        //    if (cbxNameField.Text == string.Empty)
        //    {
        //        MessageBox.Show(@"Выберите имя поля!", @"Неудача");
        //        return;
        //    }

        //    if (cbxCriterion.Text == string.Empty)
        //    {
        //        MessageBox.Show(@"Выберите критерий!", @"Неудача");
        //        return;
        //    }

        //    if (cbxExpression.Text == string.Empty)
        //    {
        //        if (dtpExpr.Visible != true)
        //        {
        //            MessageBox.Show(@"Заполните поле ""выражение""!", @"Неудача");
        //            return;
        //        }

        //        cbxExpression.Text = dtpExpr.Value.ToShortDateString();
        //    }
        //    Column column = null;
        //    foreach (var item in _fields)
        //        if (item.DisplayName == cbxNameField.Text)
        //            column = item;
        //    if (column.Type == "int" && !int.TryParse(cbxExpression.Text, out var r))
        //    {
        //        MessageBox.Show(@"Поле ""выражение"" имеет неверный тип!", @"Неудача");
        //        return;
        //    }

        //    if ((column.Type == "real" || column.Type == "float") && !int.TryParse(cbxExpression.Text, out var rr))
        //    {
        //        MessageBox.Show(@"Поле ""выражение"" имеет неверный!", @"Неудача");
        //        return;
        //    }

        //    if (dtpExpr.Visible) cbxExpression.Text = (DateTime.Parse(dtpExpr.Text)).ToShortDateString();
        //    lvConditions.Items.Add(new ListViewItem(new[]
        //    {
        //        cbxNameField.Text, cbxCriterion.Text, cbxExpression.Text, cbxPredicate.Text
        //    }));
        //}

        ////
        //// Удаление выделенных условий
        ////
        //private void BtnDelConditionClick(object sender, EventArgs e)
        //{
        //    foreach (ListViewItem selectedItem in lvConditions.SelectedItems)
        //        lvConditions.Items.Remove(selectedItem);
        //}
        //#endregion

        //#region Элементы со вкладки "Поля"

        ////
        //// Добавление выделенных полей в выбранные 
        ////
        //private void BtnAddFieldClick(object sender, EventArgs e)
        //{
        //    foreach (var item in lbxAllFields.SelectedItems)
        //    {
        //        if (lbxSelectFields.Items.IndexOf(item) != -1 || item.ToString()[0] == '-') continue;
        //        lbxSelectFields.Items.Add(item);
        //        lbxForSort.Items.Add(item);
        //    }
        //}

        ////
        //// Удаление выделенных полей из выбранных
        ////
        //private void BtnDeleteFieldClick(object sender, EventArgs e)
        //{
        //    var delElems = lbxSelectFields.SelectedItems;

        //    foreach (var t in delElems)
        //    {
        //        lbxSort.Items.Remove(t);
        //        lbxForSort.Items.Remove(t);
        //        lbxSelectFields.Items.Remove(t);
        //    }
        //}

        ////
        //// Добавление всех полей в выбранные 
        ////
        //private void BtnAllRightClick(object sender, EventArgs e)
        //{
        //    lbxSelectFields.Items.Clear();
        //    foreach (var item in lbxAllFields.Items)
        //    {
        //        if (lbxSelectFields.Items.IndexOf(item) != -1 || item.ToString()[0] == '-') continue;
        //        lbxSelectFields.Items.Add(item);
        //        lbxForSort.Items.Add(item);
        //    }
        //}

        ////
        //// Удаление всех полей из выбранных
        ////
        //private void BtnAllLeftClick(object sender, EventArgs e)
        //{
        //    lbxSelectFields.Items.Clear();
        //    lbxSort.Items.Clear();
        //    lbxForSort.Items.Clear();
        //}
        //#endregion

        //#region Элементы со вкладки "Порядок"

        ////
        //// Добавление выделенных элементов в лист сортировки 
        ////
        //private void BtnAddInSortClick(object sender, EventArgs e)
        //{
        //    foreach (var item in lbxForSort.SelectedItems)
        //    {
        //        if (lbxSort.Items.IndexOf(item) != -1) continue;
        //        lbxSort.Items.Add(item);
        //        _sortList.Add(item.ToString(), " ASC ");
        //    }
        //}

        ////
        //// Удаление выделенных эементов с листа сортировки 
        ////
        //private void BtnDelFromSortClick(object sender, EventArgs e)
        //{
        //    var delElems = lbxSelectFields.SelectedItems;

        //    foreach (var t in delElems)
        //    {
        //        lbxSort.Items.Remove(t);
        //        _sortList.Remove(t.ToString());
        //    }
        //}

        ////
        //// Добавление всех элементов на лист сортировки 
        ////
        //private void BtnAddAllInSortClick(object sender, EventArgs e)
        //{
        //    _sortList.Clear();
        //    lbxSort.Items.Clear();
        //    foreach (var item in lbxForSort.Items)
        //    {
        //        //if (lbxSort.Items.IndexOf(item) != -1) continue;
        //        lbxSort.Items.Add(item);
        //        _sortList.Add(item.ToString(), " ASС ");
        //    }
        //}

        ////
        //// Удалеине всех элементов с листа сортировки
        ////
        //private void BtnAllDelFromSortClick(object sender, EventArgs e)
        //{
        //    lbxSort.Items.Clear();
        //    _sortList.Clear();
        //}

        ////
        //// Обработчик выделениея строки на листе сортировки
        ////
        //private void LbxSortSelectedIndexChanged(object sender, EventArgs e)
        //{
        //    if (lbxSort.SelectedItem == null) return;
        //    // Изменяем положиение буттона 
        //    if (_sortList[lbxSort.SelectedItem.ToString()] == " DESC ") rbmDesc.Checked = true;
        //    else rbmASC.Checked = true;
        //}

        ////
        //// Обработчик изменения буттона возрастания 
        ////
        //private void RbmAscCheckedChanged(object sender, EventArgs e)
        //{
        //    if (!rbmASC.Checked) return;
        //    foreach (var item in lbxSort.SelectedItems)
        //        _sortList[item.ToString()] = " ASC ";
        //}

        ////
        //// Обработчик изменения буттона убывания 
        ////
        //private void RbmDescCheckedChanged(object sender, EventArgs e)
        //{
        //    if (!rbmDesc.Checked) return;
        //    foreach (var item in lbxSort.SelectedItems)
        //        _sortList[item.ToString()] = " DESC ";
        //}
        //#endregion

        //#endregion
    }
}
