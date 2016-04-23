using KAssets.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KAssets.Services
{
    public class BaseService
    {
        public ApplicationDbContext context;

        public BaseService()
        {
            this.context = new ApplicationDbContext();
        }
    }
}
