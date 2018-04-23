Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Linq
Imports System.Text
Imports System.Windows.Forms
Imports System.Data.OleDb
Imports DevExpress.XtraGrid.Views.Card
Imports DevExpress.XtraEditors.Repository

Namespace Grid_Runtime_MasterDetail_Mode
	Partial Public Class Form1
		Inherits Form
		Public Sub New()
			InitializeComponent()
		End Sub

		Private Sub Form1_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
			'Define a connection to the database
			Dim connection As New OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source = ..\..\Data\nwind.mdb")
			'Create data adapters for retrieving data from the tables
			Dim AdapterCategories As New OleDbDataAdapter("SELECT CategoryID, CategoryName, Picture FROM Categories", connection)
			Dim AdapterProducts As New OleDbDataAdapter("SELECT CategoryID, ProductID, ProductName, UnitPrice FROM Products", connection)

			Dim dataSet11 As New DataSet()
			'Create DataTable objects for representing database's tables
			AdapterCategories.Fill(dataSet11, "Categories")
			AdapterProducts.Fill(dataSet11, "Products")

			'Set up a master-detail relationship between the DataTables
			Dim keyColumn As DataColumn = dataSet11.Tables("Categories").Columns("CategoryID")
			Dim foreignKeyColumn As DataColumn = dataSet11.Tables("Products").Columns("CategoryID")
			dataSet11.Relations.Add("CategoriesProducts", keyColumn, foreignKeyColumn)

			'Bind the grid control to the data source
			gridControl1.DataSource = dataSet11.Tables("Categories")
			gridControl1.ForceInitialize()

			'Assign a CardView to the relationship
			Dim cardView1 As New CardView(gridControl1)
			gridControl1.LevelTree.Nodes.Add("CategoriesProducts", cardView1)
			'Specify text to be displayed within detail tabs.
			cardView1.ViewCaption = "Category Products"

			'Hide the CategoryID column for the master View
			gridView1.Columns("CategoryID").VisibleIndex = -1

			'Present data in the Picture column as Images
			Dim riPictureEdit As RepositoryItemPictureEdit = TryCast(gridControl1.RepositoryItems.Add("PictureEdit"), RepositoryItemPictureEdit)
			gridView1.Columns("Picture").ColumnEdit = riPictureEdit
			'Stretch images within cells.
			riPictureEdit.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Stretch
			gridView1.Columns("Picture").OptionsColumn.FixedWidth = True
			'Change Picture column's width
			gridView1.Columns("Picture").Width = 180

			'Change row height in the master View
			gridView1.RowHeight = 50

			'Create columns for the detail pattern View
			cardView1.PopulateColumns(dataSet11.Tables("Products"))
			'Hide the CategoryID column for the detail View
			cardView1.Columns("CategoryID").VisibleIndex = -1
			'Format UnitPrice column values as currency
			cardView1.Columns("UnitPrice").DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric
			cardView1.Columns("UnitPrice").DisplayFormat.FormatString = "c2"
		End Sub
	End Class
End Namespace
