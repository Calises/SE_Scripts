public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
}

public void Save()
{
}

static string lcdDamage = "LCDdamage";

public void Main(string argument, UpdateType updateSource)
{
    int updateFrom = (int)updateSource;
    Echo("Update from: " + updateFrom.ToString());
    // test przybliżonego czasu trwania
    //int begin = DateTime.Now.Millisecond;
    
    IMyTextSurface lcd = (IMyTextSurface)GridTerminalSystem.GetBlockWithName(lcdDamage);
    if(lcd != null)
    {
        lcd.WriteText( "Damaged:\n", false);
        
        List<IMyTerminalBlock> allBlocks = new List<IMyTerminalBlock>();
        List<IMyTerminalBlock> damagedBlocks = new List<IMyTerminalBlock>();
        GridTerminalSystem.GetBlocks(allBlocks);
        for(int i=0; i<allBlocks.Count(); i++)
        {
            if(!allBlocks[i].IsFunctional)
            {
                lcd.WriteText(allBlocks[i].DisplayNameText+"\n", true);
            }
        }        
    }

    //test przybliżonego czasu trwania
    // int end = DateTime.Now.Millisecond;
    // Echo(string.Format("Time: {0} {1} {2}", begin, end, end-begin));
}
