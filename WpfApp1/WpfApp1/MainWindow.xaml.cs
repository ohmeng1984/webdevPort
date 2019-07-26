using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Windows;
using System.Windows.Input;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.AccessDB.ItemsSource = GetCar();
            this.DataContext = this;
        }


        public class Car
        {
            public int ID { get; set; }
            public string Make { get; set; }
            public string Model { get; set; }
            public int CarYear { get; set; }
        }

        public static string connString = ConfigurationManager.ConnectionStrings["dbx"].ToString();

        public List<Car> GetCar()
        {
            //create data table for the list
            DataTable dt = new DataTable();


            //connect to access(OleDbConnection) or SQL(SqlConnection) 
            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                conn.Open();
                //command to access(OleDbCommand) or SQL(SQLCommand)
                var cmd = new OleDbCommand("Select * from tCar", conn);
                OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                //fill datatable with the list
                da.Fill(dt);
                conn.Close();
            }

            //instantiate Car class that contains Make, Model, CarYear
            var item = new List<Car>();

            
            Type t = typeof(Car);
            var property = t.GetProperties();

            foreach (DataRow rows in dt.Rows)
            {
                var newObject = Activator.CreateInstance<Car>();

                foreach(var p in property)
                {
                    if (dt.Columns.Contains(p.Name) == false) continue;

                    p.SetValue(newObject, rows[p.Name]);

                };

                item.Add(newObject);
            }


            //make sure to return List<Car>
            return item;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            //get values of textboxes
            string makeVal = txtMake.Text;
            string modelVal = txtModel.Text;
            int YearVal = Convert.ToInt32(txtYear.Text);

  

            //connect to access(OleDbConnection) or SQL(SqlConnection) 
            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                conn.Open();

                //INSERT COMMAND FOR ADDING
                var cmd = new OleDbCommand($"INSERT INTO tCar (Make, Model, CarYear) VALUES ('{makeVal}', '{modelVal}', {YearVal})", conn); 

                //Since you're inserting only 1 row just use ExecuteScalar
                cmd.ExecuteScalar();                
                conn.Close();
            }

            //Recall the getCar within the itemsource to update the data within the UI
            this.AccessDB.ItemsSource = GetCar();
        }


        public static void GetCar2(string SQL)
        {

        }
        private void AccessDB_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedItem = this.AccessDB.SelectedItem as Car;
            txtMake.Text = selectedItem.Make;
            txtModel.Text = selectedItem.Model;
            txtYear.Text = selectedItem.CarYear.ToString();
            txtId.Text = selectedItem.ID.ToString();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            int id = Convert.ToInt32(txtId.Text);
            string makeVal = txtMake.Text;

            string modelVal = txtModel.Text;
            int YearVal = Convert.ToInt32(txtYear.Text);

            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                conn.Open();
                var cmd = new OleDbCommand($"UPDATE tCar set Make = '{makeVal}', Model = '{modelVal}', CarYear = {YearVal} where ID = {id} ", conn);
                cmd.ExecuteScalar();
                conn.Close();
            }

            this.AccessDB.ItemsSource = GetCar();
        }



        public int ID { get; set; }
        private void AccessDB_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var selectedItem = this.AccessDB.SelectedItem as Car;
            ID = selectedItem.ID;
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                conn.Open();
                var cmd = new OleDbCommand($"Delete from tCar where ID = {ID}", conn);
                cmd.ExecuteScalar();
                conn.Close();
            }
            this.AccessDB.ItemsSource = GetCar();
        }

        private void AccessDB_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }
    }
}
