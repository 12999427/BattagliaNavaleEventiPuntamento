using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattagliaNavaleEventi
{
    public class ColpitoEventArgs : EventArgs
    {
        public bool colpito;
        public Point coord;

        public ColpitoEventArgs(bool colpitoo, Point coordd)
        {
            colpito = colpitoo;
            coord = coordd;
        }
    }
}
