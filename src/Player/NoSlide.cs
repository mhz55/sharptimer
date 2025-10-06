using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Cvars;

namespace SharpTimer;

public class NoSlide
{
    private SharpTimer _plugin;
    private ConVar? _svfriction;
    private float defaultFriction = 5.2f;
    private bool[] noslide = new bool[64];
    
    public NoSlide(SharpTimer plugin)
    {
        _plugin = plugin;
        
        _svfriction = ConVar.Find("sv_friction");
        defaultFriction = _svfriction!.GetPrimitiveValue<float>();
        _svfriction.Flags &= ~ConVarFlags.FCVAR_REPLICATED;
        
        _plugin.RegisterListener<Listeners.OnClientConnected>(slot => noslide[slot] = false);
        _plugin.RegisterListener<Listeners.OnClientDisconnectPost>(slot => noslide[slot] = false);
        _plugin.AddCommand("css_noslide", "Removes player sliding while bhopping ",Command_NoSlide);
        
    }
    
    public int GetSlot(CCSPlayerController? movementServices)
    {
        uint? index = movementServices?.Index;
        
        return (int) index.Value - 1;
    }

    public void Command_NoSlide(CCSPlayerController? controller, CommandInfo command)
    {
        if (controller == null) return;
        int slot = GetSlot(controller);
        bool noslideOn = noslide[slot];
        
        _plugin.PrintToChat(controller, "You have set noslide to " + noslideOn);
        controller.ReplicateConVar("sv_friction", noslideOn ? "50.0" : defaultFriction.ToString());
    }
}