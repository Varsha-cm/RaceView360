using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Service.Model
{
    public abstract class BaseRS
    {
    }

    public abstract class BaseRS<TModel> : BaseRS
    {
        public virtual TModel Result { get; set; }
    }
}
