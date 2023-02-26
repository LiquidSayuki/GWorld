using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Models
{
    internal class Item
    {
        TCharacterItem dbItem;

        public int ItemId;
        public int ItemCount;

        public Item(TCharacterItem item)
        {
            this.dbItem = item;
            this.ItemId = (short)item.ItemId;
            this.ItemCount = (short)item.ItemCount;
        }

        public void Add(int count)
        {
            this.ItemCount += count;
            dbItem.ItemCount = this.ItemCount;
        }

        public void Remove(int count)
        {
            this.ItemCount -= count;
            dbItem.ItemCount = this.ItemCount;
        }

        public bool Use (int count = 1)
        {
            return false;
        }

        public override string ToString()
        {
            return string.Format("ItemID:{0}, Count:{1}, PlayerTID:{2}", this.ItemId, this.ItemCount, this.dbItem.TCharacterID);
        }
    }
}
