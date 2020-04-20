%errorA = ForceRequiredAddOn("Vehicle_Tank");
%errorB = ForceRequiredAddOn("Weapon_Rocket_Launcher");
%errorC = ForceRequiredAddOn("Projectile_GravityRocket");

if(%errorA == $Error::AddOn_Disabled)
   TankVehicle.uiName = "";
if(%errorB == $Error::AddOn_Disabled)
   rocketLauncherItem.uiName = "";
if(%errorC == $Error::AddOn_Disabled)
   gravityRocketProjectile.uiName = "";

if(%errorA == $Error::AddOn_NotFound)
   error("ERROR: Vehicle_LRCannon - required add-on Vehicle_Tank not found");
else if(%errorB == $Error::AddOn_NotFound)
   error("ERROR: Vehicle_LRCannon - required add-on Weapon_Rocket_Launcher not found");
else if(%errorC == $Error::AddOn_NotFound)
   error("ERROR: Vehicle_LRCannon - required add-on Projectile_GravityRocket not found");
else
   exec("./Vehicle_LRCannon.cs"); 
