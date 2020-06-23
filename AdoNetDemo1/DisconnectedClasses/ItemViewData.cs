using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace AdoNetDemo1.DisconnectedClasses
{
    public class ItemViewData
    {
        public string ResultText { get; set; }
        public DataView GetBooksSortedByAuthorDescending()
        {
            DataView result = null;
            var data = new Module6DataAccess();
            var dt = data.GetBooksAsDataTable();
            if(dt != null)
            {
                result = dt.DefaultView;
                result.Sort = "Author DESC";
                
            }
            return result;
        }
        public DataView GetTransactionViewsLessThan(decimal amount)
        {
            DataView result = null;
            var data = new Module6DataAccess();
            var dt = data.GetTransactionsAsDataTable();
            if(dt != null)
            {
                result = (from trn in dt.AsEnumerable()
                          where trn.Field<decimal>("Amount") < amount
                          orderby trn.Field<decimal>("Amount") descending
                          select trn).AsDataView();
            }
            return result;
        }
        public DataTable GetTransactionsLessThan(decimal amount)
        {
            DataView result = null;
            var data = new Module6DataAccess();
            var dt = data.GetTransactionsAsDataTable();
            if (dt != null)
            {
                result = (from trn in dt.AsEnumerable()
                          where trn.Field<decimal>("Amount") < amount
                          orderby trn.Field<decimal>("Amount") descending
                          select trn).AsDataView();
            }
            return result.ToTable();
        }
    }
}
