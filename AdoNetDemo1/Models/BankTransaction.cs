using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoNetDemo1
{
    public class BankTransaction
    {
        public string Type { get; set; }
        public SqlMoney Amount { get; set; }
    }
}
