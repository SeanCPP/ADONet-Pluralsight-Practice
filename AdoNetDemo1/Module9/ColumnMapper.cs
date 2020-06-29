using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AdoNetDemo1.Module9
{
    public class ColumnMapper
    {
        public string ColumnName { get; set; }
        public PropertyInfo ColumnProperty { get; set; }
    }
}
