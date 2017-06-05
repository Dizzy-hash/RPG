using UnityEngine;
using System.Collections;

namespace ACT
{
    public class ActBuff : ActTree
    {
        public int         ID            { get; private set; }
        public DBuff       DB            { get; private set; }
        public int         CurOverlayNum { get; private set; }
        public Character   Owner         { get; private set; }

        public ActBuff(int id, Character owner)
        {
            this.ID            = id;
            this.Owner         = owner;
            this.CurOverlayNum = 0;
            this.DB            = ReadCfgBuff.GetDataById(this.ID);
        }
    }
}