public Program()
{
   // The constructor, called only once every session and
   // always before any other method is called. Use it to
   // initialize your script. 
   //     
   // The constructor is optional and can be removed if not
   // needed.
   // 
   // It's recommended to set RuntimeInfo.UpdateFrequency 
   // here, which will allow your script to run itself without a 
   // timer block.
}

public void Save()
{
   // Called when the program needs to save its state. Use
   // this method to save your state to the Storage field
   // or some other means. 
   // 
   // This method is optional and can be removed if not
   // needed.
}

static int counter = 0;
static float pi = 3.14159265f;
static double radToDeg = 180 / pi;
static double degToRad = pi / 180;

public void Main(string argument, UpdateType updateSource)
{
   int updateFrom = (int)updateSource;
   Echo("Update from: " + updateFrom.ToString() + ", called: " + counter);

   IMyMotorAdvancedStator rotor = (IMyMotorAdvancedStator)GridTerminalSystem.GetBlockWithName("rotorek");

   if(IsManualUpdate(updateSource))
   {
      if(rotor.TargetVelocityRPM > 0)
      {
         rotor.TargetVelocityRPM = 0;
         Runtime.UpdateFrequency = 0;
      }
      else
      {           
         float targetAngle;
         if(float.TryParse(argument, out targetAngle))
         {
            Runtime.UpdateFrequency = UpdateFrequency.Update1;

            float actualAngle = rotor.Angle;
            targetAngle = targetAngle * degToRad;				
            float diff = targetAngle - actualAngle;

            if(diff(i) > -pi && diff(i) < 0)
            {
               rotor.TargetVelocityRad = -1;
               diff = -diff;
            }
            else
            {
               rotor.TargetVelocityRad = 1;
               if(diff < 0)
                  diff += 2*pi;
            }
            counter = Math.Round(diff*60.f);
         }
         else
         {
            rotor.TargetVelocityRPM = 10;
            Runtime.UpdateFrequency = UpdateFrequency.Update10;
            counter = -1;
         }
      }
   }
   else
   {
      IMyTextSurface lcd = (IMyTextSurface)GridTerminalSystem.GetBlockWithName("LCD");
      if(lcd != null)
         lcd.WriteText( Math.Round(rotor.Angle * radToDeg).ToString(), false);

      --counter;
      if(counter <= 0)
      {
         rotor.TargetVelocityRPM = 0;
         Runtime.UpdateFrequency = 0;
      }
   }

   //List<IMyTerminalBlock> rotors = new List<IMyTerminalBlock>();
   //GridTerminalSystem.GetBlocksOfType<IMyMotorAdvancedStator>(rotors);
   //Echo("Found rotors: " + rotors.Count().ToString());
}

private bool IsManualUpdate(UpdateType type)
{
   return (type == UpdateType.Terminal || type == UpdateType.Trigger);
}

