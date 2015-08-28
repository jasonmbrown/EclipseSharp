using Extensions;
using System;


namespace Server.Database {
    public class TempPlayer {
        #region Declarations
        public Boolean InGame           { get; set; }
        public Int32 CurrentCharacter   { get; set; }
        public Boolean[] IsMoving       = new Boolean[(Int32)Enumerations.Direction.Direction_Count];
        #endregion

        #region Constructors
        public TempPlayer() {
            this.InGame = false;
            this.CurrentCharacter = -1;
        }
        #endregion
    }
}
