static string lcdBat = "lcdBat";
private void BatteryMonitor()
{
    float energy = 0;
    float maxEnergy = 0;
    float energyIn = 0;
    float energyOut = 0;

    List<IMyBatteryBlock> allBatts = new List<IMyBatteryBlock>();
    GridTerminalSystem.GetBlocksOfType<IMyBatteryBlock>(allBatts);

    for(int i=0; i<allBatts.Count(); i++)
    {
        energy += allBatts[i].CurrentStoredPower;
        maxEnergy += allBatts[i].MaxStoredPower;
        energyIn += allBatts[i].CurrentInput;
        energyOut += allBatts[i].CurrentOutput;
    }

    double total = energyIn-energyOut;
    double timeS = 0;
    if(total > 0)
    {
        timeS = (maxEnergy-energy) / total;
    }
    else
    {
        timeS = (energy) / -total;
    }
    double percentage = Math.Round(energy / maxEnergy * 100, 0);
    IMyTextSurface lcd = (IMyTextSurface)GridTerminalSystem.GetBlockWithName(lcdBat);
    if(lcd != null)
        lcd.WriteText("Battery: " + string.Format("{0:#0.000;;--}", energy) + " / "
            + string.Format("{0:#0.000;;--}", (double)maxEnergy) + " MWh [" + percentage.ToString() + "%]\n"
            + "IN : " + string.Format("{0,6:#0.000;;--}", energyIn) + " MW | TOTAL: " + string.Format("{0,6:#0.000;;--}", total)+ " MW\n"
            + "OUT: " + string.Format("{0,6:#0.000;;--}", energyOut) + " MW | TIME : " + string.Format("{0,6:#0.0;;--}", timeS) + " h\n"
        , false);
}
