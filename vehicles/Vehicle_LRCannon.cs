//Cannon! Thank you Ephi for being sexy, and saving me the trouble of having to program most of this.
exec("./player.cs");
datablock PlayerData(LRCannonTurret : PlayerStandardArmor)
{
	cameraVerticalOffset = 3;
	shapefile = "./LRCannon.dts";
	canJet = 0;
	mass = 200000;
   //drag = 0.02;
   //density = 0.6;
   drag = 1;
   density = 5;
   runSurfaceAngle = 1;
   jumpSurfaceAngle = 0;
   maxForwardSpeed = 0;
   maxBackwardSpeed = 0;
   maxBackwardCrouchSpeed = 0;
   maxForwardCrouchSpeed = 0;
   maxSideSpeed = 0;
   maxSideCrouchSpeed = 0;
   maxStepHeight = 0;
    maxUnderwaterSideSpeed = 0;

	uiName = "Long Range Cannon";
	showEnergyBar = false;
	
   jumpForce = 0; //8.3 * 90;
   jumpEnergyDrain = 10000;
   minJumpEnergy = 10000;
   jumpDelay = 127;
   minJumpSpeed = 0;
   maxJumpSpeed = 0;
	
	rideable = true;
	canRide = true;
	paintable = true;
	
   boundingBox			= vectorScale("3 3 1", 4);
   crouchBoundingBox	= vectorScale("3 3 1", 4);

   lookUpLimit = 0.6;
   lookDownLimit = 0.2;
	
   numMountPoints = 1;
   mountThread[0] = "root";
   
   upMaxSpeed = 1;
   upResistSpeed = 1;
   upResistFactor = 1;
   maxdamage = 300;
   minlookangle = -0.4;
   maxlookangle = 0.2;

   useCustomPainEffects = true;
   PainHighImage = "";
   PainMidImage  = "";
   PainLowImage  = "";
   painSound     = "";
   deathSound    = "";
};







function CannonSmokeImage::onDone(%this,%obj,%slot)
{
	%obj.unMountImage(%slot);
}


AddDamageType("LRCannonBallDirect",   '<bitmap:add-ons/Vehicle_Pirate_Cannon/ball> %1',       '%2 <bitmap:add-ons/Vehicle_Pirate_Cannon/ball> %1',       1, 1);
AddDamageType("LRCannonBallRadius",   '<bitmap:add-ons/Vehicle_Pirate_Cannon/ballRadius> %1', '%2 <bitmap:add-ons/Vehicle_Pirate_Cannon/ballRadius> %1', 1, 0);




function CannonFuseImage::onDone(%this,%obj,%slot)
{
	%obj.unMountImage(%slot);
}



datablock ExplosionData(LRCannonBaseExplosion)
{
   //explosionShape = "";
   lifeTimeMS = 150;

   soundProfile = vehicleExplosionSound;
   
   emitter[0] = vehicleExplosionEmitter;
   emitter[1] = vehicleExplosionEmitter2;
   //emitter[0] = "";
   //emitter[1] = "";

   debris = CannonDebris;
   debrisNum = 1;
   debrisNumVariance = 0;
   debrisPhiMin = 0;
   debrisPhiMax = 360;
   debrisThetaMin = 0;
   debrisThetaMax = 20;
   debrisVelocity = 18;
   debrisVelocityVariance = 3;

   faceViewer     = true;
   explosionScale = "1 1 1";

   shakeCamera = true;
   camShakeFreq = "7.0 8.0 7.0";
   camShakeAmp = "10.0 10.0 10.0";
   camShakeDuration = 0.75;
   camShakeRadius = 15.0;

   // Dynamic light
   lightStartRadius = 0;
   lightEndRadius = 20;
   lightStartColor = "0.45 0.3 0.1";
   lightEndColor = "0 0 0";

   //impulse
   impulseRadius = 15;
   impulseForce = 1000;
   impulseVertical = 2000;

   //radius damage
   radiusDamage        = 40;
   damageRadius        = 4.0;

   //burn the players?
   playerBurnTime = 1000;

};

datablock ProjectileData(CannonBaseExplosionProjectile)
{
   directDamage        = 0;
   radiusDamage        = 0;
   damageRadius        = 0;
   explosion           = CannonBaseExplosion;

   directDamageType  = $DamageType::jeepExplosion;
   radiusDamageType  = $DamageType::jeepExplosion;

   explodeOnDeath		= 1;

   armingDelay         = 0;
   lifetime            = 10;
};

function CannonTurret::onDisabled(%this,%obj,%state)
{
   %p = new Projectile()
   {
      dataBlock = CannonBaseExplosionProjectile;
      initialPosition = %obj.gettransform();
      initialVelocity = "0 0 1";
      client = %obj.lastDamageClient;
      sourceClient = %obj.lastDamageClient;
   };
   MissionCleanup.add(%p);
   %obj.hidenode("Cannon");

   %obj.schedule(3000,removebody);
   %player = %obj.getMountedObject(0);

   //boot out the driver
   %driver = %obj.getMountedObject(0);
   if(isObject(%driver))
      %driver.getdatablock().dodismount(%driver);

   //remove the burning fuse
   %obj.unMountImage(2); 
   
   %obj.burn(5);
}

package PirateCannonPackage
{
   function armor::onTrigger(%this, %obj, %triggerNum, %val)
   {
      %mount = %obj.getObjectMount();

      //hack so we can shoot if we ARE a cannon
      if(%obj.getDataBlock().getID() == CannonTurret.getID())
         %mount = %obj;

      if(isObject(%mount) && (%obj == %mount || %obj.getControlObject() == %mount))
      {
         if(%mount.getDataBlock().getId() == CannonTurret.getId() && %triggerNum == 0 )
         {
            if(%val == 1)
            {
               %client = %obj.client;
               if(isObject(%client))
                  ServerCmdUnUseTool(%client);

               if(getSimTime() - %obj.lastShotTime < 2500)
               {
                  cancel(%mount.stsched);
                  return;
               }
               %mount.shotpower = 0;
               CannonStrengthLoop(%mount);
               return;
            }
            else
            {
               cancel(%mount.stsched);
               if(%mount.shotpower != 0)
               {
                  CannonFire(%this, %obj, %triggerNum, %val,%mount);
                  %mount.shotpower = 0;
                  return;
               }
            }
         }
      }
      
      Parent::onTrigger(%this,%obj,%triggerNum,%val);
   }

   function armor::onMount(%this,%obj,%col,%slot)
   {
      Parent::onMount(%this,%obj,%col,%slot);
      if(%col.getDataBlock().getId() == CannonTurret.getId())
      {
         %client = %obj.client;
         if(isObject(%client))
            ServerCmdUnUseTool(%client);
      }
   }
};
activatepackage(PirateCannonPackage);

function CannonFire(%this, %obj, %triggerNum, %val,%mount)
{
   %client = %obj.client;
   if(isObject(%client))
      ServerCmdUnUseTool(%client);

   if(getSimTime() - %obj.lastShotTime < 2500)
      return;

   %scaleFactor = getWord(%mount.getScale(), 2);
   %p = new Projectile()
   {
      dataBlock       = LRCannonBallProjectile;
      initialPosition = %mount.getEyeTransform();
      initialVelocity = vectorScale(%mount.getMuzzleVector(0), %mount.shotpower * 5.5 * %scaleFactor);
      sourceObject    = %obj;
      client          = %obj.client;
      sourceSlot      = 0;
      originPoint     = vectorAdd( %mount.getEyeTransform(), vectorScale(%mount.getEyeVector(), 3) );
   };
   MissionCleanup.add(%p);
   %p.setScale(%scaleFactor SPC %scaleFactor SPC %scaleFactor);

   %mount.mountImage(CannonSmokeImage, 1);
   %mount.unMountImage(2); 

   serverPlay3D(TankshotSound, %obj.getPosition());

   %obj.playThread(0, activate);
   %obj.lastShotTime = getSimTime();
}

function CannonStrengthLoop(%obj)
{
	cancel(%obj.stsched);

	%obj.ShotPower += 1;
   
   %client = %obj.getControllingClient();
   if(isObject(%client))
      bottomprintCannonStrength(%client, %obj.shotpower);

	if(%obj.ShotPower >= 10)
		return;

	%obj.stsched = schedule(200, %obj, cannonstrengthloop, %obj);

   %obj.mountImage(CannonFuseImage, 2);
} 

function bottomprintCannonStrength(%client, %amount)
{
   %per = %amount * 2;
   %maxcounters = 20;
   %foo = %maxcounters - %per;
   %char = "|";for(%a =0; %a<%per; %a++){%fchar = %char @ %fchar;}
   %char2 = "|";for(%b =0; %b<%foo; %b++){%ftchar = %char2 @ %ftchar;}
   bottomprint(%client, "<just:center><color:FF0000>Fire! <color:FFFFFF>:<color:FFFF00>" @ %fchar @ "<color:000000>" @%ftchar, 1, 1);
}
