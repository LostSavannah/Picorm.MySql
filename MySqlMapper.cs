using Picorm.Common;
using Picorm.Common.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Picorm.MySql
{
    public class MySqlMapper : Mapper
    {
        public MySqlMapper(MySqlDbInterface dBInterface) : base(dBInterface)
        {

        }

        public MySqlDbInterface? MySqlDbInterface {
            get => DBInterface as MySqlDbInterface;
        }

        public MySqlMapper(string connectionString):this(new MySqlDbInterface(connectionString))
        {

        }
    }
}
