using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parcel.Data
{
    public static class RandomChoices
    {
        public static int Choice(int min, int max) => new Random().Next(min, max);
    }
}
