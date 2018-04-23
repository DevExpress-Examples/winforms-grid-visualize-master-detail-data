using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using DevExpress.XtraGrid.Views.Card;
using DevExpress.XtraEditors.Repository;

namespace Grid_Runtime_MasterDetail_Mode {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e) {
            //Define a connection to the database
            OleDbConnection connection = new OleDbConnection(
              "Provider=Microsoft.Jet.OLEDB.4.0;Data Source = ..\\..\\Data\\nwind.mdb");
            //Create data adapters for retrieving data from the tables
            OleDbDataAdapter AdapterCategories = new OleDbDataAdapter(
              "SELECT CategoryID, CategoryName, Picture FROM Categories", connection);
            OleDbDataAdapter AdapterProducts = new OleDbDataAdapter(
              "SELECT CategoryID, ProductID, ProductName, UnitPrice FROM Products", connection);

            DataSet dataSet11 = new DataSet();
            //Create DataTable objects for representing database's tables
            AdapterCategories.Fill(dataSet11, "Categories");
            AdapterProducts.Fill(dataSet11, "Products");

            //Set up a master-detail relationship between the DataTables
            DataColumn keyColumn = dataSet11.Tables["Categories"].Columns["CategoryID"];
            DataColumn foreignKeyColumn = dataSet11.Tables["Products"].Columns["CategoryID"];
            dataSet11.Relations.Add("CategoriesProducts", keyColumn, foreignKeyColumn);

            //Bind the grid control to the data source
            gridControl1.DataSource = dataSet11.Tables["Categories"];
            gridControl1.ForceInitialize();

            //Assign a CardView to the relationship
            CardView cardView1 = new CardView(gridControl1);
            gridControl1.LevelTree.Nodes.Add("CategoriesProducts", cardView1);
            //Specify text to be displayed within detail tabs.
            cardView1.ViewCaption = "Category Products";

            //Hide the CategoryID column for the master View
            gridView1.Columns["CategoryID"].VisibleIndex = -1;
            
            //Present data in the Picture column as Images
            RepositoryItemPictureEdit riPictureEdit = gridControl1.RepositoryItems.Add("PictureEdit") as RepositoryItemPictureEdit;
            gridView1.Columns["Picture"].ColumnEdit = riPictureEdit;
            //Stretch images within cells.
            riPictureEdit.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Stretch;
            gridView1.Columns["Picture"].OptionsColumn.FixedWidth = true;
            //Change Picture column's width
            gridView1.Columns["Picture"].Width = 180;

            //Change row height in the master View
            gridView1.RowHeight = 50;

            //Create columns for the detail pattern View
            cardView1.PopulateColumns(dataSet11.Tables["Products"]);
            //Hide the CategoryID column for the detail View
            cardView1.Columns["CategoryID"].VisibleIndex = -1;
            //Format UnitPrice column values as currency
            cardView1.Columns["UnitPrice"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            cardView1.Columns["UnitPrice"].DisplayFormat.FormatString = "c2";
        }
    }
}
