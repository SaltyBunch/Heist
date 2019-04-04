namespace UI
{
    public class TextHelper
    {
        // All text shown to the player should be stored here


        public static string PickupWeapon => "Press <#0000FF>x <#FFFFFF>to pickup weapon"; //todo placeholder for now
        public static string PickupTrap => "Press <#0000FF>x <#FFFFFF>to pickup trap"; //todo placeholder for now
        public static string OpenDoor => "Press <#0000FF>x <#FFFFFF>to open door"; //todo placeholder for now
        public static string CloseDoor => "Press <#0000FF>x <#FFFFFF>to close door"; //todo placeholder for now
        public static string OpenVault => "Press <#0000FF>x <#FFFFFF>to open vault"; //todo placeholder for now
        public static string OpenMiniVault => "Press <#0000FF>x <#FFFFFF>to open safe"; //todo placeholder for now
        public static string TakeGold => "Press <#0000FF>x <#FFFFFF>to take gold";
        public static string DisableHazard => "Press <#0000FF>x <#FFFFFF>to disable hazard";
        public static string BlueKeyPickUp => "<#395BFF>You Picked up a Blue key";
        public static string RedKeyPickUp => "<#FF1100>You Picked up a Red key";
        public static string YellowKeyPickUp => "<#FFCC21>You Picked up a Yellow key";
        public static string StairUnlocked => "Stairs have been unlocked"; //shown for everyone pls
        public static string RequireRedKey => "You require the <#FF1100>Red <#FFFFFF>Key";
        public static string RequireYellowKey => "You require the <#FFCC21>Yellow <#FFFFFF>Key";
        public static string RequireBlueKey => "You require the <#395BFF>Blue <#FFFFFF>Key";

        public static string RequireBothKeys =>
            "You require the <#FF1100>Red <#FFFFFF>and <#FFCC21>Yellow <#FFFFFF>Key";

        public static string LockDownNotif => "<#FF1100>Lock Down Will Initiate in 2 Minutes"; //Shown for everyone pls
    }
}