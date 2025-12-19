using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattagliaNavale
{
    public class Ship
    {
        public int x { get; }
        public int y { get; }
        public int size { get; }

        private int remainingSize;

        public bool vertical { get; }

        public bool sunk { get; set; }


        public Ship (int x, int y, int size, bool vertical)
        {
            this.x = x;
            this.y = y;
            this.size = size;
            this.remainingSize = size;
            this.sunk = false;
            this.vertical = vertical;
        }

        public bool removeTile ()
        {
            /*if (this.sunk)
                return false;*/
            remainingSize--;
            if (remainingSize == 0)
            {
                sunk = true;
                return true;
            }
            return false;
        }

    }
}
