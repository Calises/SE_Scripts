public class ItemStack
{
    public ItemStack(string _y)
    {
        Amount = MyFixedPoint.Zero;
        Name = _y;
    }
    
    public MyFixedPoint Amount { get; set; }
    public string Name { get; }
}

Dictionary<string, ItemStack> monitoringOres = new Dictionary<string, ItemStack>();
Dictionary<string, ItemStack> monitoringIngots = new Dictionary<string, ItemStack>();
Dictionary<string, ItemStack> monitoringComp = new Dictionary<string, ItemStack>();

static string lcdVolume = "LCDvolume";
static string lcdOre = "LCDore";
static string lcdIngots = "LCDingots";
static string lcdComponents = "LCDcomp";
static string lcdError = "LCDerror";

static int counter = 0;

public Program()
{
    monitoringOres.Add("Ice", new ItemStack("Ice"));
    monitoringOres.Add("Stone", new ItemStack("Stone"));
    monitoringOres.Add("Iron", new ItemStack("Iron"));
    monitoringOres.Add("Silicon", new ItemStack("Silicon"));
    monitoringOres.Add("Nickel", new ItemStack("Nickel"));
    monitoringOres.Add("Scrap", new ItemStack("Scrap Metal"));
    monitoringOres.Add("Gold", new ItemStack("Gold"));
    monitoringOres.Add("Uranium", new ItemStack("Uranium"));
    monitoringOres.Add("Cobalt", new ItemStack("Cobalt"));
    monitoringOres.Add("Magnesium", new ItemStack("Magnesium"));
    monitoringOres.Add("Platinum", new ItemStack("Platinum"));
    monitoringOres.Add("Silver", new ItemStack("Silver"));

    monitoringIngots.Add("Iron", new ItemStack("Iron"));
    monitoringIngots.Add("Silicon", new ItemStack("Silicon"));
    monitoringIngots.Add("Silver", new ItemStack("Silver"));
    monitoringIngots.Add("Nickel", new ItemStack("Nickel"));
    monitoringIngots.Add("Cobalt", new ItemStack("Cobalt"));
    monitoringIngots.Add("Magnesium", new ItemStack("Magnesium"));
    monitoringIngots.Add("Stone", new ItemStack("Gravel"));
    monitoringIngots.Add("Uranium", new ItemStack("Uranium"));
    monitoringIngots.Add("Platinum", new ItemStack("Platinum"));
    monitoringIngots.Add("Gold", new ItemStack("Gold"));

    monitoringComp.Add("Girder", new ItemStack("Girder"));
    monitoringComp.Add("SmallTube", new ItemStack("Small Tube"));
    monitoringComp.Add("SteelPlate", new ItemStack("Steel Plate"));
    monitoringComp.Add("InteriorPlate", new ItemStack("Interior Plate"));
    monitoringComp.Add("MetalGrid", new ItemStack("Metal Grid"));
    monitoringComp.Add("Computer", new ItemStack("Computer"));
    monitoringComp.Add("Reactor", new ItemStack("Reactor"));
    monitoringComp.Add("Motor", new ItemStack("Motor"));
    monitoringComp.Add("LargeTube", new ItemStack("Large Tube"));
    monitoringComp.Add("GravityGenerator", new ItemStack("Gravity Gen."));
    monitoringComp.Add("BulletproofGlass", new ItemStack("B-proof Glass"));
    monitoringComp.Add("Display", new ItemStack("Display"));
    monitoringComp.Add("Medical", new ItemStack("Medical"));
    monitoringComp.Add("Construction", new ItemStack("Constr. Comp."));
    monitoringComp.Add("Detector", new ItemStack("Detector Comp."));
    monitoringComp.Add("Explosives", new ItemStack("Explosives"));
    monitoringComp.Add("RadioCommunication", new ItemStack("Comm. Comp."));
    monitoringComp.Add("Thrust", new ItemStack("Thruster Comp."));
    monitoringComp.Add("SolarCell", new ItemStack("Solar Cell"));
    monitoringComp.Add("PowerCell", new ItemStack("Power Cell"));
    
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
}

public void Save()
{
}

public void Main(string argument, UpdateType updateSource)
{
    ClearError();
    int updateFrom = (int)updateSource;
    Echo("Update from: " + updateFrom.ToString() + ", called: " + counter);
    // test przybli??onego czasu trwania
    //int begin = DateTime.Now.Millisecond;

//fajny, nieu??ywany
    // List<IMyInventory> tempList = new List<IMyInventory>();
    // GetInventoriesFromType<IMyCargoContainer>(tempList);
//fajny, nieu??ywany

    // clear previous counters
    ClearAmounts(monitoringOres);
    ClearAmounts(monitoringIngots);
    ClearAmounts(monitoringComp);
        
    List<MyInventoryItem> items = new List<MyInventoryItem>();
    MyFixedPoint currentVolume = 0;
    MyFixedPoint maxVolume = 0;

    List<IMyTerminalBlock> allBlocks = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocks(allBlocks);
    for(int i=0; i<allBlocks.Count(); i++)
    {
        if(!allBlocks[i].HasInventory)
            continue;
        
        for (int invIdx = 0; invIdx < allBlocks[i].InventoryCount; invIdx++)
        {
            IMyInventory inv = allBlocks[i].GetInventory(invIdx);

            items.Clear();
            inv.GetItems(items);
            foreach(MyInventoryItem item in items)
            {
                switch (item.Type.TypeId)
                {                    
                case "MyObjectBuilder_Ore":
                    tryAddAmount(monitoringOres, item);
                    break;
                case "MyObjectBuilder_Ingot":
                    tryAddAmount(monitoringIngots, item);
                    break;
                case "MyObjectBuilder_Component":
                    tryAddAmount(monitoringComp, item);
                    break;
                }
            }
            
            currentVolume += inv.CurrentVolume;
            maxVolume += inv.MaxVolume;
        }
    }
    
    currentVolume = MyFixedPoint.MultiplySafe(currentVolume, 1000);
    maxVolume = MyFixedPoint.MultiplySafe(maxVolume, 1000);    
    double percentageVolume = Math.Round((double)currentVolume.RawValue / (double)maxVolume.RawValue * 100, 2);
    
////// Printing...
    IMyTextSurface lcd = (IMyTextSurface)GridTerminalSystem.GetBlockWithName(lcdVolume);
    if(lcd != null)
    {
        lcd.WriteText( "Cargo volume:\n"
            +string.Format("{0:0.00}", (double)currentVolume)+" / "
            +string.Format("{0:0.00}", (double)maxVolume)+" L\n"
            +percentageVolume.ToString()+" %", false);
    }
    
    
    lcd = (IMyTextSurface)GridTerminalSystem.GetBlockWithName(lcdOre);
    if(lcd != null)
    {
        lcd.WriteText( "ORES [kg]:\n", false);
        foreach(ItemStack item in monitoringOres.Values)
        {
            lcd.WriteText(string.Format("{0,9:#0.00;;--}", (double)item.Amount)+" "+item.Name+"\n", true);
        }
    }
    
    lcd = (IMyTextSurface)GridTerminalSystem.GetBlockWithName(lcdIngots);
    if(lcd != null)
    {
        lcd.WriteText( "INGOTS [kg]:\n", false);            
        foreach(ItemStack item in monitoringIngots.Values)
        {
            lcd.WriteText(string.Format("{0,9:#0.00;;--}", (double)item.Amount)+" "+item.Name+"\n", true);
        }
    }
    
    lcd = (IMyTextSurface)GridTerminalSystem.GetBlockWithName(lcdComponents);
    if(lcd != null)
    {
        lcd.WriteText( "COMPONENTS:\n", false);            
        foreach(ItemStack item in monitoringComp.Values)
        {
            lcd.WriteText(string.Format("{0,5:#0;;--}", (int)item.Amount)+" "+item.Name+"\n", true);
        }
    }

    //test przybli??onego czasu trwania
    // int end = DateTime.Now.Millisecond;
    // Echo(string.Format("Time: {0} {1} {2}", begin, end, end-begin));
}

static ItemStack itemStack;
private void ClearAmounts(Dictionary<string, ItemStack> list)
{
    foreach(string itemName in list.Keys.ToList())
    {
        itemStack = list[itemName];
        itemStack.Amount = MyFixedPoint.Zero;
        list[itemName] = itemStack;
    }
}

private void tryAddAmount(Dictionary<string, ItemStack> list, MyInventoryItem item)
{
    if(list.TryGetValue(item.Type.SubtypeId, out itemStack))
    {
        itemStack.Amount = MyFixedPoint.AddSafe(itemStack.Amount, item.Amount);
        list[item.Type.SubtypeId] = itemStack;
    }
    else
    {
        PrintTypeDetails(item.Type);
    }
}

private void GetInventoriesFromType<T>(List<IMyInventory> list) where T : class, IMyTerminalBlock
{
    List<T> objects = new List<T>();
    GridTerminalSystem.GetBlocksOfType<T>(objects);
    Echo("Found objects: " + objects.Count().ToString());

    for(int i=0; i<objects.Count(); i++)
    {
        Echo(objects[i].CustomName);
        list.Add(objects[i].GetInventory());
    }
}

private void PrintTypeDetails(MyItemType item)
{
    PrintError(item.TypeId+" "+item.SubtypeId+" "+item.GetHashCode());
}

private void ClearError()
{
    IMyTextSurface lcd = (IMyTextSurface)GridTerminalSystem.GetBlockWithName(lcdError);
    if(lcd != null)
        lcd.WriteText("ERRORS:\n", false);
}

private void PrintError(string msg)
{
    IMyTextSurface lcd = (IMyTextSurface)GridTerminalSystem.GetBlockWithName(lcdError);
    if(lcd != null)
        lcd.WriteText(msg+"\n", true);
}
