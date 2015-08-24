using System;
using Extensions;

namespace Extensions.Database {
    public class Player {

        #region Declares
        // Account
        public  String  Username        { get; set; }
        public  String  Password        { get; set; }
        public  String  Salt            { get; set; }

        // General
        public Character[] Characters;
        #endregion

        #region Constructors
        public Player() {
            this.Username       = String.Empty;
            this.Password       = String.Empty;
            this.Salt           = String.Empty;
            this.Characters = new Character[3];
        }
        #endregion

        #region Methods
        public void SetPassword(String input) {
            this.Salt = String.Format("{0}HelloWorldThisIsPartOfASalt{1}", DateTime.UtcNow.ToString(), input).ToSha256();
            this.Password = String.Format("{0}{1}", input.ToSha256(), this.Salt).ToSha256();
        }
        public Boolean ComparePassword(String input) {
            return String.Equals(this.Password, String.Format("{0}{1}", input.ToSha256(), this.Salt).ToSha256());
        }
        #endregion
    }

    public class Character {

        #region Declares
        // General Data
        public String           Name            { get; set; }
        public Byte             Gender          { get; set; }
        public Int32            Class           { get; set; }
        public Int32            Sprite          { get; set; }
        public Byte             Level           { get; set; }
        public Int32            Experience      { get; set; }

        // Stats
        public Int32[]          Statistic       = new Int32[(Int32)Enumerations.Stats.Stat_Count];

        // Position
        public Int32            Map             { get; set; }
        public Byte             X               { get; set; }
        public Byte             Y               { get; set; }
        public Byte             Direction       { get; set; }
        #endregion

        #region Constructors 
        public Character() {
            this.Name           = String.Empty;
            this.Gender         = new Byte();
            this.Class          = new Int32();
            this.Sprite         = new Int32();
            this.Level          = new Byte();
            this.Experience     = new Int32();
            this.X              = new Byte();
            this.Y              = new Byte();
            this.Direction      = new Byte();
        }
        #endregion

        #region Methods
       
        #endregion
    }
}
