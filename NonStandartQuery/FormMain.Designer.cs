namespace NonStandartQuery
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    partial class FormMain
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.tcNonStandartQuery = new System.Windows.Forms.TabControl();
            this.tpFields = new System.Windows.Forms.TabPage();
            this.btDeleteAll = new System.Windows.Forms.Button();
            this.btAddAll = new System.Windows.Forms.Button();
            this.btDelete = new System.Windows.Forms.Button();
            this.btAdd = new System.Windows.Forms.Button();
            this.lvSelectedFields = new System.Windows.Forms.ListView();
            this.columnSelectedFields = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lvAllFields = new System.Windows.Forms.ListView();
            this.columnAllFields = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tpConditions = new System.Windows.Forms.TabPage();
            this.dgvConditions = new System.Windows.Forms.DataGridView();
            this.tpOrder = new System.Windows.Forms.TabPage();
            this.btUp = new System.Windows.Forms.Button();
            this.btDown = new System.Windows.Forms.Button();
            this.btDesc = new System.Windows.Forms.Button();
            this.btAsc = new System.Windows.Forms.Button();
            this.btDeleteAllFromSort = new System.Windows.Forms.Button();
            this.btAddAllToSort = new System.Windows.Forms.Button();
            this.btDeleteFromSort = new System.Windows.Forms.Button();
            this.btAddToSort = new System.Windows.Forms.Button();
            this.lvSelectedFieldsToSort = new System.Windows.Forms.ListView();
            this.columnField = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnOrder = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lvAllFieldsToSort = new System.Windows.Forms.ListView();
            this.columnAllFieldsToSort = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tpResult = new System.Windows.Forms.TabPage();
            this.dgvResult = new System.Windows.Forms.DataGridView();
            this.btExecute = new System.Windows.Forms.Button();
            this.btShowSQLQuery = new System.Windows.Forms.Button();
            this.btCancel = new System.Windows.Forms.Button();
            this.btChangeConnection = new System.Windows.Forms.Button();
            this.tcNonStandartQuery.SuspendLayout();
            this.tpFields.SuspendLayout();
            this.tpConditions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvConditions)).BeginInit();
            this.tpOrder.SuspendLayout();
            this.tpResult.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvResult)).BeginInit();
            this.SuspendLayout();
            // 
            // tcNonStandartQuery
            // 
            this.tcNonStandartQuery.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tcNonStandartQuery.Controls.Add(this.tpFields);
            this.tcNonStandartQuery.Controls.Add(this.tpConditions);
            this.tcNonStandartQuery.Controls.Add(this.tpOrder);
            this.tcNonStandartQuery.Controls.Add(this.tpResult);
            this.tcNonStandartQuery.Location = new System.Drawing.Point(0, 0);
            this.tcNonStandartQuery.Name = "tcNonStandartQuery";
            this.tcNonStandartQuery.SelectedIndex = 0;
            this.tcNonStandartQuery.Size = new System.Drawing.Size(803, 409);
            this.tcNonStandartQuery.TabIndex = 0;
            // 
            // tpFields
            // 
            this.tpFields.Controls.Add(this.btDeleteAll);
            this.tpFields.Controls.Add(this.btAddAll);
            this.tpFields.Controls.Add(this.btDelete);
            this.tpFields.Controls.Add(this.btAdd);
            this.tpFields.Controls.Add(this.lvSelectedFields);
            this.tpFields.Controls.Add(this.lvAllFields);
            this.tpFields.Location = new System.Drawing.Point(4, 22);
            this.tpFields.Name = "tpFields";
            this.tpFields.Padding = new System.Windows.Forms.Padding(3);
            this.tpFields.Size = new System.Drawing.Size(795, 383);
            this.tpFields.TabIndex = 0;
            this.tpFields.Text = "Поля";
            this.tpFields.UseVisualStyleBackColor = true;
            // 
            // btDeleteAll
            // 
            this.btDeleteAll.Location = new System.Drawing.Point(378, 240);
            this.btDeleteAll.Name = "btDeleteAll";
            this.btDeleteAll.Size = new System.Drawing.Size(40, 40);
            this.btDeleteAll.TabIndex = 3;
            this.btDeleteAll.Text = "<<";
            this.btDeleteAll.UseVisualStyleBackColor = true;
            this.btDeleteAll.Click += new System.EventHandler(this.BtDeleteAllClick);
            // 
            // btAddAll
            // 
            this.btAddAll.Location = new System.Drawing.Point(378, 194);
            this.btAddAll.Name = "btAddAll";
            this.btAddAll.Size = new System.Drawing.Size(40, 40);
            this.btAddAll.TabIndex = 2;
            this.btAddAll.Text = ">>";
            this.btAddAll.UseVisualStyleBackColor = true;
            this.btAddAll.Click += new System.EventHandler(this.BtAddAllClick);
            // 
            // btDelete
            // 
            this.btDelete.Location = new System.Drawing.Point(378, 148);
            this.btDelete.Name = "btDelete";
            this.btDelete.Size = new System.Drawing.Size(40, 40);
            this.btDelete.TabIndex = 1;
            this.btDelete.Text = "<";
            this.btDelete.UseVisualStyleBackColor = true;
            this.btDelete.Click += new System.EventHandler(this.BtDeleteClick);
            // 
            // btAdd
            // 
            this.btAdd.Location = new System.Drawing.Point(378, 102);
            this.btAdd.Name = "btAdd";
            this.btAdd.Size = new System.Drawing.Size(40, 40);
            this.btAdd.TabIndex = 0;
            this.btAdd.Text = ">";
            this.btAdd.UseVisualStyleBackColor = true;
            this.btAdd.Click += new System.EventHandler(this.BtAddClick);
            // 
            // lvSelectedFields
            // 
            this.lvSelectedFields.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnSelectedFields});
            this.lvSelectedFields.Location = new System.Drawing.Point(424, 20);
            this.lvSelectedFields.Name = "lvSelectedFields";
            this.lvSelectedFields.Size = new System.Drawing.Size(365, 345);
            this.lvSelectedFields.TabIndex = 11;
            this.lvSelectedFields.UseCompatibleStateImageBehavior = false;
            this.lvSelectedFields.View = System.Windows.Forms.View.Details;
            // 
            // columnSelectedFields
            // 
            this.columnSelectedFields.Text = "Выбранные поля";
            this.columnSelectedFields.Width = 365;
            // 
            // lvAllFields
            // 
            this.lvAllFields.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnAllFields});
            this.lvAllFields.FullRowSelect = true;
            this.lvAllFields.Location = new System.Drawing.Point(3, 20);
            this.lvAllFields.Name = "lvAllFields";
            this.lvAllFields.Size = new System.Drawing.Size(365, 345);
            this.lvAllFields.TabIndex = 10;
            this.lvAllFields.UseCompatibleStateImageBehavior = false;
            this.lvAllFields.View = System.Windows.Forms.View.Details;
            // 
            // columnAllFields
            // 
            this.columnAllFields.Text = "Все поля";
            this.columnAllFields.Width = 360;
            // 
            // tpConditions
            // 
            this.tpConditions.Controls.Add(this.dgvConditions);
            this.tpConditions.Location = new System.Drawing.Point(4, 22);
            this.tpConditions.Name = "tpConditions";
            this.tpConditions.Padding = new System.Windows.Forms.Padding(3);
            this.tpConditions.Size = new System.Drawing.Size(795, 383);
            this.tpConditions.TabIndex = 1;
            this.tpConditions.Text = "Условия";
            this.tpConditions.UseVisualStyleBackColor = true;
            // 
            // dgvConditions
            // 
            this.dgvConditions.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvConditions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvConditions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvConditions.Location = new System.Drawing.Point(3, 3);
            this.dgvConditions.Name = "dgvConditions";
            this.dgvConditions.Size = new System.Drawing.Size(789, 377);
            this.dgvConditions.TabIndex = 13;
            // 
            // tpOrder
            // 
            this.tpOrder.Controls.Add(this.btUp);
            this.tpOrder.Controls.Add(this.btDown);
            this.tpOrder.Controls.Add(this.btDesc);
            this.tpOrder.Controls.Add(this.btAsc);
            this.tpOrder.Controls.Add(this.btDeleteAllFromSort);
            this.tpOrder.Controls.Add(this.btAddAllToSort);
            this.tpOrder.Controls.Add(this.btDeleteFromSort);
            this.tpOrder.Controls.Add(this.btAddToSort);
            this.tpOrder.Controls.Add(this.lvSelectedFieldsToSort);
            this.tpOrder.Controls.Add(this.lvAllFieldsToSort);
            this.tpOrder.Location = new System.Drawing.Point(4, 22);
            this.tpOrder.Name = "tpOrder";
            this.tpOrder.Size = new System.Drawing.Size(795, 383);
            this.tpOrder.TabIndex = 2;
            this.tpOrder.Text = "Порядок";
            this.tpOrder.UseVisualStyleBackColor = true;
            // 
            // btUp
            // 
            this.btUp.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btUp.Location = new System.Drawing.Point(379, 290);
            this.btUp.Name = "btUp";
            this.btUp.Size = new System.Drawing.Size(40, 40);
            this.btUp.TabIndex = 25;
            this.btUp.Text = "↑";
            this.btUp.UseVisualStyleBackColor = true;
            this.btUp.Click += new System.EventHandler(this.BtUpClick);
            // 
            // btDown
            // 
            this.btDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btDown.Location = new System.Drawing.Point(379, 334);
            this.btDown.Name = "btDown";
            this.btDown.Size = new System.Drawing.Size(40, 40);
            this.btDown.TabIndex = 24;
            this.btDown.Text = "↓";
            this.btDown.UseVisualStyleBackColor = true;
            this.btDown.Click += new System.EventHandler(this.BtDownClick);
            // 
            // btDesc
            // 
            this.btDesc.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btDesc.Location = new System.Drawing.Point(379, 244);
            this.btDesc.Name = "btDesc";
            this.btDesc.Size = new System.Drawing.Size(40, 40);
            this.btDesc.TabIndex = 23;
            this.btDesc.Text = "-";
            this.btDesc.UseVisualStyleBackColor = true;
            this.btDesc.Click += new System.EventHandler(this.BtDescClick);
            // 
            // btAsc
            // 
            this.btAsc.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btAsc.Location = new System.Drawing.Point(379, 198);
            this.btAsc.Name = "btAsc";
            this.btAsc.Size = new System.Drawing.Size(40, 40);
            this.btAsc.TabIndex = 22;
            this.btAsc.Text = "+";
            this.btAsc.UseVisualStyleBackColor = true;
            this.btAsc.Click += new System.EventHandler(this.BtAscClick);
            // 
            // btDeleteAllFromSort
            // 
            this.btDeleteAllFromSort.Location = new System.Drawing.Point(379, 152);
            this.btDeleteAllFromSort.Name = "btDeleteAllFromSort";
            this.btDeleteAllFromSort.Size = new System.Drawing.Size(40, 40);
            this.btDeleteAllFromSort.TabIndex = 11;
            this.btDeleteAllFromSort.Text = "<<";
            this.btDeleteAllFromSort.UseVisualStyleBackColor = true;
            this.btDeleteAllFromSort.Click += new System.EventHandler(this.BtDeleteAllFromSortClick);
            // 
            // btAddAllToSort
            // 
            this.btAddAllToSort.Location = new System.Drawing.Point(379, 106);
            this.btAddAllToSort.Name = "btAddAllToSort";
            this.btAddAllToSort.Size = new System.Drawing.Size(40, 40);
            this.btAddAllToSort.TabIndex = 10;
            this.btAddAllToSort.Text = ">>";
            this.btAddAllToSort.UseVisualStyleBackColor = true;
            this.btAddAllToSort.Click += new System.EventHandler(this.BtAddAllToSortClick);
            // 
            // btDeleteFromSort
            // 
            this.btDeleteFromSort.Location = new System.Drawing.Point(379, 60);
            this.btDeleteFromSort.Name = "btDeleteFromSort";
            this.btDeleteFromSort.Size = new System.Drawing.Size(40, 40);
            this.btDeleteFromSort.TabIndex = 9;
            this.btDeleteFromSort.Text = "<";
            this.btDeleteFromSort.UseVisualStyleBackColor = true;
            this.btDeleteFromSort.Click += new System.EventHandler(this.BtDeleteFromSortClick);
            // 
            // btAddToSort
            // 
            this.btAddToSort.Location = new System.Drawing.Point(379, 14);
            this.btAddToSort.Name = "btAddToSort";
            this.btAddToSort.Size = new System.Drawing.Size(40, 40);
            this.btAddToSort.TabIndex = 8;
            this.btAddToSort.Text = ">";
            this.btAddToSort.UseVisualStyleBackColor = true;
            this.btAddToSort.Click += new System.EventHandler(this.BtAddToSortClick);
            // 
            // lvSelectedFieldsToSort
            // 
            this.lvSelectedFieldsToSort.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnField,
            this.columnOrder});
            this.lvSelectedFieldsToSort.Location = new System.Drawing.Point(425, 20);
            this.lvSelectedFieldsToSort.Name = "lvSelectedFieldsToSort";
            this.lvSelectedFieldsToSort.ShowGroups = false;
            this.lvSelectedFieldsToSort.Size = new System.Drawing.Size(363, 345);
            this.lvSelectedFieldsToSort.TabIndex = 21;
            this.lvSelectedFieldsToSort.UseCompatibleStateImageBehavior = false;
            this.lvSelectedFieldsToSort.View = System.Windows.Forms.View.Details;
            // 
            // columnField
            // 
            this.columnField.Text = "Выбранные поля";
            this.columnField.Width = 250;
            // 
            // columnOrder
            // 
            this.columnOrder.Text = "Направление";
            this.columnOrder.Width = 110;
            // 
            // lvAllFieldsToSort
            // 
            this.lvAllFieldsToSort.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnAllFieldsToSort});
            this.lvAllFieldsToSort.FullRowSelect = true;
            this.lvAllFieldsToSort.Location = new System.Drawing.Point(10, 20);
            this.lvAllFieldsToSort.Name = "lvAllFieldsToSort";
            this.lvAllFieldsToSort.Size = new System.Drawing.Size(363, 345);
            this.lvAllFieldsToSort.TabIndex = 20;
            this.lvAllFieldsToSort.UseCompatibleStateImageBehavior = false;
            this.lvAllFieldsToSort.View = System.Windows.Forms.View.Details;
            // 
            // columnAllFieldsToSort
            // 
            this.columnAllFieldsToSort.Text = "Все поля";
            this.columnAllFieldsToSort.Width = 358;
            // 
            // tpResult
            // 
            this.tpResult.Controls.Add(this.dgvResult);
            this.tpResult.Location = new System.Drawing.Point(4, 22);
            this.tpResult.Name = "tpResult";
            this.tpResult.Size = new System.Drawing.Size(795, 383);
            this.tpResult.TabIndex = 3;
            this.tpResult.Text = "Результат";
            this.tpResult.UseVisualStyleBackColor = true;
            // 
            // dgvResult
            // 
            this.dgvResult.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvResult.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvResult.Location = new System.Drawing.Point(0, 0);
            this.dgvResult.Name = "dgvResult";
            this.dgvResult.Size = new System.Drawing.Size(795, 383);
            this.dgvResult.TabIndex = 0;
            // 
            // btExecute
            // 
            this.btExecute.Location = new System.Drawing.Point(573, 415);
            this.btExecute.Name = "btExecute";
            this.btExecute.Size = new System.Drawing.Size(110, 23);
            this.btExecute.TabIndex = 1;
            this.btExecute.Text = "Выполнить запрос";
            this.btExecute.UseVisualStyleBackColor = true;
            this.btExecute.Click += new System.EventHandler(this.BtExecuteClick);
            // 
            // btShowSQLQuery
            // 
            this.btShowSQLQuery.Location = new System.Drawing.Point(389, 415);
            this.btShowSQLQuery.Name = "btShowSQLQuery";
            this.btShowSQLQuery.Size = new System.Drawing.Size(146, 23);
            this.btShowSQLQuery.TabIndex = 2;
            this.btShowSQLQuery.Text = "Показать текст запроса";
            this.btShowSQLQuery.UseVisualStyleBackColor = true;
            this.btShowSQLQuery.Click += new System.EventHandler(this.BtShowSqlQueryClick);
            // 
            // btCancel
            // 
            this.btCancel.Location = new System.Drawing.Point(713, 415);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(75, 23);
            this.btCancel.TabIndex = 3;
            this.btCancel.Text = "Отмена";
            this.btCancel.UseVisualStyleBackColor = true;
            this.btCancel.Click += new System.EventHandler(this.BtCancelClick);
            // 
            // btChangeConnection
            // 
            this.btChangeConnection.Location = new System.Drawing.Point(12, 415);
            this.btChangeConnection.Name = "btChangeConnection";
            this.btChangeConnection.Size = new System.Drawing.Size(132, 23);
            this.btChangeConnection.TabIndex = 4;
            this.btChangeConnection.Text = "Изменить соединение";
            this.btChangeConnection.UseVisualStyleBackColor = true;
            this.btChangeConnection.Click += new System.EventHandler(this.BtChangeConnectionClick);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btChangeConnection);
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.btShowSQLQuery);
            this.Controls.Add(this.btExecute);
            this.Controls.Add(this.tcNonStandartQuery);
            this.Name = "FormMain";
            this.Text = "NonStandartQuery";
            this.tcNonStandartQuery.ResumeLayout(false);
            this.tpFields.ResumeLayout(false);
            this.tpConditions.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvConditions)).EndInit();
            this.tpOrder.ResumeLayout(false);
            this.tpResult.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvResult)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private TabControl tcNonStandartQuery;
        private TabPage tpConditions;
        private TabPage tpResult;
        private Button btExecute;
        private Button btShowSQLQuery;
        private Button btCancel;
        private Button btChangeConnection;
        private DataGridView dgvResult;
        private DataGridView dgvConditions;
        private TabPage tpFields;
        private Button btDeleteAll;
        private Button btAddAll;
        private Button btDelete;
        private Button btAdd;
        private ListView lvSelectedFields;
        private ColumnHeader columnSelectedFields;
        private ListView lvAllFields;
        private ColumnHeader columnAllFields;
        private TabPage tpOrder;
        private Button btAsc;
        private Button btDeleteAllFromSort;
        private Button btAddAllToSort;
        private Button btDeleteFromSort;
        private Button btAddToSort;
        private ListView lvSelectedFieldsToSort;
        private ColumnHeader columnField;
        private ColumnHeader columnOrder;
        private ListView lvAllFieldsToSort;
        private ColumnHeader columnAllFieldsToSort;
        private Button btUp;
        private Button btDown;
        private Button btDesc;
    }
}

