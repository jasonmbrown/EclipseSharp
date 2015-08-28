using Extensions;
using System;

namespace Client.Database {
    public class TempPlayer {
        public Boolean[]    IsMoving    = new Boolean[(Int32)Enumerations.Direction.Direction_Count];
        public Byte         Frame       = 0;
    }
}
