using AdoNetDemo1.DisconnectedClasses;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AdoNetDemo1.Module7
{
    public class Module7DataAccess
    {
        //public string ResultText { get; private set; }

        public DataTable BuildDataTable()
        {
            var dt = new DataTable();
            DataColumn column;

            // one way to add a column
            dt.Columns.Add("ProductId", typeof(int));

            // another way, with more control
            column = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "ProductName",
                Caption = "Product Name",
                ReadOnly = false,
            };
            dt.Columns.Add(column);
            // another nother way, using both methods
            dt.Columns.Add(new DataColumn 
            { 
                DataType = typeof(decimal),
                ColumnName = "Price",
                Caption = "Price",
                ReadOnly = false
            });

            // add some data. this method is best for small number of columns
            dt.Rows.Add(1, "Product 1", 49.99m);

            // this method works better for larger data sets
            var dr = dt.NewRow();
            dr["ProductId"] = 2;
            dr["ProductName"] = "Product 2";
            dr["Price"] = 199.99m;
            dt.Rows.Add(dr);

            dt.AcceptChanges();
            return dt;
        }

        public DataTable CloneDataTable()
        {
            return BuildDataTable().Clone();
        }
        public DataTable CopyDataTable()
        {
            return BuildDataTable().Copy();
        }

        public DataTable GetSubsetOfDataTable()
        {
            DataTable dt;
            var data = new Module6DataAccess();
            dt = data.GetTransactionsAsDataTable();
            var dtNew = dt.Clone();


            // V1, using loop
            //var rows = dt.Select("Amount < 25");
            //foreach(var row in rows)
            //{
            //    // importRow needs to be used because 
            //    // a row instance cannot belong to more than one DataTable
            //    dtNew.ImportRow(row);
            //}

            // V2 using helper method
            dtNew = dt.Select("Amount < 25").CopyToDataTable();
            ////

            dtNew.AcceptChanges();
            return dtNew;
        }

        public string ProcessRowsAndColumns(DataTable dt)
        {
            var sb = new StringBuilder();
            int index = 1;

            if (!dt.AsEnumerable().Any())
            {
                foreach(DataColumn col in dt.Columns)
                {
                    sb.Append($" | {col.ColumnName} ");
                }
                return sb.ToString();
            }
            foreach (DataRow row in dt.Rows)
            {
                sb.AppendLine($"** Row: {index}");
                foreach (DataColumn col in dt.Columns)
                {
                    sb.AppendLine($"-{col.ColumnName}: {row[col.ColumnName]}");
                }
                sb.AppendLine();
                ++index;
            }
            return sb.ToString();
        }
    }
}
