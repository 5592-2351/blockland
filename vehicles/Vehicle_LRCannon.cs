//Cannon! Thank you Ephi for being sexy, and saving me the trouble of having to program most of this.
exec("./player.cs");
datablock PlayerData(CannonTurret : PlayerStandardArmor)
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
   maxForwardSpeed = 5;
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


datablock ParticleData(cannonBallTrailParticle)
{
	dragCoefficient		= 3.0;
	windCoefficient		= 0.0;
	gravityCoefficient	= 0.0;
	inheritedVelFactor	= 0.0;
	constantAcceleration	= 0.0;
	lifetimeMS		= 250;
	lifetimeVarianceMS	= 0;
	spinSpeed		= 10.0;
	spinRandomMin		= -50.0;
	spinRandomMax		= 50.0;
	useInvAlpha		= true;
	animateTexture		= false;
	//framesPerSec		= 1;

	textureName		= "base/data/particles/dot";
	//animTexName		= "~/data/particles/dot";

	// Interpolation variables
	colors[0]	= "0.2 0.2 0.2 0.1";
	colors[1]	= "0.2 0.2 0.2 0.0";
	sizes[0]	= 0.8;
	sizes[1]	= 0.0;
	times[0]	= 0.0;
	times[1]	= 1.0;
};

datablock ParticleEmitterData(cannonBallTrailEmitter)
{
   ejectionPeriodMS = 5;
   periodVarianceMS = 0;

   ejectionVelocity = 0; //0.25;
   velocityVariance = 0; //0.10;

   ejectionOffset = 0;

   thetaMin         = 0.0;
   thetaMax         = 90.0;  

   particles = cannonBallTrailParticle;

   useEmitterColors = true;
   uiName = "Cannon Ball Trail";
};


datablock ParticleData(CannonSmokeParticle)
{
	dragCoefficient      = 3;
	gravityCoefficient   = -1;
	inheritedVelFactor   = 0.6;
	constantAcceleration = 0.0;
	lifetimeMS           = 1200;
	lifetimeVarianceMS   = 55;
	textureName          = "base/data/particles/cloud";
	spinSpeed		= 10.0;
	spinRandomMin		= -500.0;
	spinRandomMax		= 500.0;
	colors[0]     = "0.5 0.5 0.5 0.2";
	colors[1]     = "0.5 0.5 0.5 0.0";
	sizes[0]      = 1.0;
	sizes[1]      = 1.2;

	useInvAlpha = true;
};

datablock ParticleEmitterData(CannonSmokeEmitter)
{
   ejectionPeriodMS = 40;
   periodVarianceMS = 4;
   ejectionVelocity = 3;
   velocityVariance = 2;
   ejectionOffset   = 0;
   thetaMin         = 0;
   thetaMax         = 50;
   phiReferenceVel  = 0;
   phiVariance      = 360;
   overrideAdvance = false;
   particles = "CannonSmokeParticle";

   uiName = "Cannon Smoke";
};

datablock ShapeBaseImageData(CannonSmokeImage : TankSmokeImage)
{
   offset = "0 0 0.5";
   rotation = eulerToMatrix("90 0 0");

   stateTimeoutValue[3]          = 2.0;
	stateEmitter[3]               = CannonSmokeEmitter;
	stateEmitterTime[3]           = 2.0;
};
function CannonSmokeImage::onDone(%this,%obj,%slot)
{
	%obj.unMountImage(%slot);
}

datablock AudioProfile(WhistleLoopSound)
{
   filename    = "./whistle.wav";
   description = AudioDefaultLooping3d;
   preload = true;
};

AddDamageType("LRCannonBallDirect",   '<bitmap:add-ons/Vehicle_Pirate_Cannon/ball> %1',       '%2 <bitmap:add-ons/Vehicle_Pirate_Cannon/ball> %1',       1, 1);
AddDamageType("LRCannonBallRadius",   '<bitmap:add-ons/Vehicle_Pirate_Cannon/ballRadius> %1', '%2 <bitmap:add-ons/Vehicle_Pirate_Cannon/ballRadius> %1', 1, 0);
datablock ProjectileData(LRCannonBallProjectile)
{
   projectileShapeName = "./CannonBall.dts";
   directDamage        = 100;
   directDamageType = $DamageType::CannonBallDirect;
   radiusDamageType = $DamageType::CannonBallRadius;
   impactImpulse	   = 1000;
   verticalImpulse	   = 1000;
   explosion           = TankShellExplosion;
   particleEmitter     = cannonBallTrailEmitter;

   brickExplosionRadius = 5;
   brickExplosionImpact = true;          //destroy a brick if we hit it directly?
   brickExplosionForce  = 50;             
   brickExplosionMaxVolume = 60;          //max volume of bricks that we can destroy
   brickExplosionMaxVolumeFloating = 120;  //max volume of bricks that we can destroy if they aren't connected to the ground (should always be >= brickExplosionMaxVolume)

   sound = WhistleLoopSound;

   muzzleVelocity      = 450;
   velInheritFactor    = 1.0;

   armingDelay         = 0;
   lifetime            = 10000;
   fadeDelay           = 10000;
   bounceElasticity    = 0.5;
   bounceFriction      = 0.20;
   isBallistic         = true;
   gravityMod = 0.5;

   hasLight    = false;
   lightRadius = 5.0;
   lightColor  = "1 0.5 0.0";

   explodeOnDeath = 1;

   uiName = "Cannon Ball"; //naming it this way because it's a cannon ball
};



datablock ParticleData(cannonFuseAParticle)
{
	dragCoefficient      = 3;
	gravityCoefficient   = 0.0;
	inheritedVelFactor   = 0.2;
	constantAcceleration = 0.0;
	lifetimeMS           = 150;
	lifetimeVarianceMS   = 15;
	textureName          = "base/data/particles/cloud";
	spinSpeed		= 10.0;
	spinRandomMin		= -500.0;
	spinRandomMax		= 500.0;
	colors[0]     = "0.9 0.0 0.0 0.9";
	colors[1]     = "0.9 0.7 0.0 0.0";
	sizes[0]      = 0.051;
	sizes[1]      = 0.021;

	useInvAlpha = false;
};
datablock ParticleEmitterData(cannonFuseAEmitter)
{
   ejectionPeriodMS = 3;
   periodVarianceMS = 0;
   ejectionVelocity = 2.0;
   velocityVariance = 1.0;
   ejectionOffset   = 0.0;
   thetaMin         = 0;
   thetaMax         = 20;
   phiReferenceVel  = 0;
   phiVariance      = 360;
   overrideAdvance = false;
   particles = "cannonFuseAParticle";

   uiName = "Cannon Fuse A";
};

datablock ParticleData(cannonFuseBParticle)
{
	dragCoefficient      = 3;
	gravityCoefficient   = -0.5;
	inheritedVelFactor   = 0.2;
	constantAcceleration = 0.0;
	lifetimeMS           = 50;
	lifetimeVarianceMS   = 15;
	textureName          = "base/data/particles/star1";
	spinSpeed		= 10.0;
	spinRandomMin		= -500.0;
	spinRandomMax		= 500.0;
	colors[0]     = "0.9 0.7 0.0 0.9";
	colors[1]     = "0.9 0.2 0.0 0.0";
	sizes[0]      = 0.2;
	sizes[1]      = 0.3;

	useInvAlpha = false;
};
datablock ParticleEmitterData(cannonFuseBEmitter)
{
   ejectionPeriodMS = 3;
   periodVarianceMS = 0;
   ejectionVelocity = 1.0;
   velocityVariance = 1.0;
   ejectionOffset   = 0.0;
   thetaMin         = 0;
   thetaMax         = 20;
   phiReferenceVel  = 0;
   phiVariance      = 360;
   overrideAdvance = false;
   particles = "cannonFuseBParticle";

   uiName = "Cannon Fuse B";
};

datablock ParticleData(cannonFuseCParticle)
{
	dragCoefficient      = 3;
	gravityCoefficient   = -0.5;
	inheritedVelFactor   = 0.2;
	constantAcceleration = 0.0;
	lifetimeMS           = 50;
	lifetimeVarianceMS   = 15;
	textureName          = "base/data/particles/star1";
	spinSpeed		= 100.0;
	spinRandomMin		= -500.0;
	spinRandomMax		= 500.0;
	colors[0]     = "0.9 0.7 0.0 0.9";
	colors[1]     = "0.9 0.2 0.0 0.0";
	sizes[0]      = 0.5;
	sizes[1]      = 0.7;

	useInvAlpha = false;
};
datablock ParticleEmitterData(cannonFuseCEmitter)
{
   ejectionPeriodMS = 3;
   periodVarianceMS = 0;
   ejectionVelocity = 1.0;
   velocityVariance = 1.0;
   ejectionOffset   = 0.0;
   thetaMin         = 0;
   thetaMax         = 20;
   phiReferenceVel  = 0;
   phiVariance      = 360;
   overrideAdvance = false;
   particles = "cannonFuseCParticle";

   uiName = "Cannon Fuse C";
};

datablock ShapeBaseImageData(CannonFuseImage)
{
	shapeFile = "base/data/shapes/empty.dts";
	emap = false;

	mountPoint = 1;
   offset = "0 -0.45 -3.2";
   rotation = eulerToMatrix("180 0 0");

	stateName[0]                  = "Ready";
	stateTransitionOnTimeout[0]   = "FireA";
	stateTimeoutValue[0]          = 0.01;

	stateName[1]                  = "FireA";
	stateTransitionOnTimeout[1]   = "FireB";
	stateWaitForTimeout[1]        = True;
	stateTimeoutValue[1]          = 0.9;
	stateEmitter[1]               = cannonFuseAEmitter;
	stateEmitterTime[1]           = 0.9;

	stateName[2]                  = "FireB";
	stateTransitionOnTimeout[2]   = "FireC";
	stateWaitForTimeout[2]        = True;
	stateTimeoutValue[2]          = 0.9;
	stateEmitter[2]               = cannonFuseBEmitter; 
	stateEmitterTime[2]           = 0.9;

	stateName[3]                  = "FireC";
	stateTransitionOnTimeout[3]   = "Done";
	stateWaitForTimeout[3]        = True;
	stateTimeoutValue[3]          = 20.0;
	stateEmitter[3]               = cannonFuseCEmitter;
	stateEmitterTime[3]           = 20.0;

	stateName[4]                  = "Done";
	stateScript[4]                = "onDone";
};

function CannonFuseImage::onDone(%this,%obj,%slot)
{
	%obj.unMountImage(%slot);
}

datablock DebrisData(CannonDebris)
{
   //emitters = "jeepDebrisTrailEmitter";
   emitters = "";

	shapeFile = "./cannonDebris.dts";
	lifetime = 3.5;
	minSpinSpeed = -300.0;
	maxSpinSpeed = 300.0;
	elasticity = 0.5;
	friction = 0.2;
	numBounces = 1;
	staticOnMaxBounce = true;
	snapOnMaxBounce = false;
	fade = true;

	gravModifier = 2;
};

datablock ExplosionData(CannonBaseExplosion)
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
   radiusDamage        = 30;
   damageRadius        = 8.0;

   //burn the players?
   playerBurnTime = 5000;

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
