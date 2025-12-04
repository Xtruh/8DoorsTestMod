using BepInEx;
using BepInEx.Logging;
using UnityEngine;

namespace _8DoorsTestMod;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    private readonly string[] weaponList = { "Scythe", "Sword", "Bow", "Bat", "Umbrella", "Lamp", "Fan", "Ducroak" };

    private readonly string[] abilityList = { "Inputs", "Movement", "HighLanding", "Jump", "DownJump",
        "Attack", "Jump Attack", "Activate Skill", "Rolling", "Dash", "Wall Hold", "Double Jump",
        "Use Item", "Change Player", "Swim", "Carry Object", "Super Jump", "Wall Super Dash", "Break Wall", "AidOn"};

    private readonly string[] locationList = { "Ch1_1", "Ch1_2", "Ch1_3", "Ch1_4", "Ch1_5", "Ch1_6", "Ch1_7", "Ch1_8", "Ch1_9",
        "Ch1_10", "Ch1_11", "Ch1_12", "IERO", "RunnerRoom1", "RunnerRoom2", "RunnerRoom3", "IEROBack", "Ch2_1", "Ch2_2", "Ch2_3",
        "Ch2_4", "Ch2_5", "Ch2_6", "Ch2_7", "Ch2_8", "Ch2_9", "Ch2_10", "Ch2_12", "Ch2_13", "Ch3_1", "Ch3_2", "Ch3_3", "Ch3_4",
        "Ch3_5", "Ch3_6", "Ch3_7", "Ch3_8", "Ch3_9", "Ch3_10", "Ch3_11", "Ch3_12", "Ch3_13", "Ch3_14", "Ch3_15", "Ch3_16", "Ch3_17",
        "Ch3_18", "Ch3_19", "Ch3_20", "Ch4_1", "Ch4_2", "Ch4_3", "Ch4_4", "Ch4_5", "Ch4_6", "Ch4_7", "Ch4_8", "Ch4_9", "Ch4_10",
        "Ch4_11", "Ch4_12", "Ch4_13", "Ch4_14", "Ch4_15", "Ch4_16", "Ch4_17", "Ch4_18", "Ch4_19", "Ch4_20", "Ch5_1", "Ch5_2",
        "Ch5_3", "Ch5_4", "Ch5_5", "Ch5_6", "Ch5_7", "Ch5_8", "Ch5_9", "Ch5_10", "Ch5_11", "Ch5_12", "Ch5_13", "Ch5_14", "Ch5_15", 
        "Ch5_16", "Ch5_17", "Ch5_18", "Ch6_1", "Ch6_2", "Ch6_3", "Ch6_4", "Ch6_5", "Ch6_6", "Ch6_7", "Ch6_8", "Ch6_9", "Ch6_10",
        "Ch6_11", "Ch6_12", "Ch6_13", "Ch6_14", "Ch6_15", "Ch6_16", "Ch6_17", "Ch6_18", "Ch6_19", "Ch6_20", "Ch7_1", "Ch7_2",
        "Ch7_3", "Ch7_4", "Ch7_5", "Ch7_6", "Ch7_7", "Ch7_8", "Ch7_9", "Ch7_10", "Ch7_11", "Ch7_12", "Ch7_13", "Ch7_14", "Ch7_15",
        "Ch7_16", "Ch7_17", "Ch8_1", "Ch8_2", "Ch8_3", "Ch8_4", "Ch8_5", "Ch8_6", "Ch8_7", "Ch8_8", "Ch8_9", "Ch8_10", "Ch8_11", 
        "Ch8_12", "Ch8_13", "Ch8_14", "Ch8_15", "Ch8_16", "Ch8_17", "Ch9_1", "OxyRoomEscape", "OxyRoom", "Ch9_2"};

    private int selectedWeapon = 0;
    private int selectedAbility = 0;
    private int selectedLocation = 0;

    private Vector2 scrollPos = Vector2.zero;

    internal static new ManualLogSource Logger;
    private bool showMenu = false;

    private Rect WindowRect = new Rect(20, 20, 350, 450);

    private void Awake()
    {
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            showMenu = !showMenu;
        }
    }

    private void OnGUI()
    {
        if (!showMenu) return;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        WindowRect = GUI.Window(123456, WindowRect, DrawWindow, "8Doors Toolkit");
    }

    private void DrawWindow(int id)
    {
        scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Width(330), GUILayout.Height(400));

        // --------------------------
        // Weapons
        // --------------------------
        GUILayout.Label("Weapons", GUI.skin.box);

        selectedWeapon = GUILayout.SelectionGrid(
            selectedWeapon,
            weaponList,
            1,
            GUILayout.Width(280)
        );

        if (GUILayout.Button("Add Weapon", GUILayout.Width(280)))
        {
            string weapon = weaponList[selectedWeapon];
            Logger.LogInfo($"[MOD] Adding weapon: {weapon}");
            GiveWeapon(weapon);
        }

        if (GUILayout.Button("Reset Weapons", GUILayout.Width(250)))
        {
            Logger.LogInfo("[MOD] Resetting all weapons...");
            ResetWeapons();
        }

        GUILayout.Space(20);

        // --------------------------
        // Abilities
        // --------------------------
        GUILayout.Label("Abilities", GUI.skin.box);

        selectedAbility = GUILayout.SelectionGrid(
            selectedAbility,
            abilityList,
            1,
            GUILayout.Width(280)
        );

        if (GUILayout.Button("Add Ability", GUILayout.Width(280)))
        {
            string ability = abilityList[selectedAbility];
            Logger.LogInfo($"[MOD] Adding ability: {ability}");
            GrantAbility(ability);
        }

        if (GUILayout.Button("Reset Ability", GUILayout.Width(250)))
        {
            Logger.LogInfo("[MOD] Resetting all Abilities...");
            ResetAbilities();
        }

        GUILayout.Space(20);

        // --------------------------
        // Teleports
        // --------------------------
        GUILayout.Label("Teleport", GUI.skin.box);

        selectedLocation = GUILayout.SelectionGrid(
            selectedLocation,
            locationList,
            1,
            GUILayout.Width(280)
        );

        if (GUILayout.Button("Teleport", GUILayout.Width(280)))
        {
            string location = locationList[selectedLocation];
            Logger.LogInfo($"[MOD] Teleporting to: {location}");
            TeleportTo(location);
        }

        GUILayout.EndScrollView();

        GUI.DragWindow();
    }

    private int GetWeaponId(string weaponName)
    {
        return weaponName switch
        {
            "Scythe" => 0,
            "Sword" => 1,
            "Bow" => 2,
            "Bat" => 3,
            "Umbrella" => 4,
            "Lamp" => 5,
            "Fan" => 6,
            "Ducroak" => 7,
        };
    }

    private int GetAbilityId(string abilityName)
    {
        return abilityName switch
        {
            "Inputs" => 0,
            "Movement" => 1,
            "HighLanding" => 2,
            "Jump" => 3,
            "DownJump" => 4,
            "Attack" => 5,
            "JumpAttack" => 6,
            "Activate Skill" => 7,
            "Rolling" => 8,
            "Dash" => 9,
            "Wall Hold" => 10,
            "Double Jump" => 11,
            "Use Item" => 12,
            "Change Player" => 13,
            "Swim" => 14,
            "Carry Object" => 15,
            "Super Jump" => 16,
            "Wall Super Dash" => 17,
            "Break Wall" => 18,
            "AidOn" => 19,
        };
    }

    private void GiveWeapon(string weaponName)
    {
        int weaponId = GetWeaponId(weaponName);

        if (weaponId < 0)
        {
            Logger.LogError($"[MOD] Unknown weapon: {weaponName}");
            return;
        }

        var player = Singleton<PlayerBase>.Instance;
        if (player == null)
        {
            Logger.LogError("[MOD] PlayerBase.Instance is NULL! Player not loaded?");
            return;
        }

        var status = player._Status;
        if (status == null)
        {
            Logger.LogError("[MOD] player._Status is NULL!");
            return;
        }

        status.SetWeaponEnable(weaponId, true);

        status.CheckWeapon();

        Logger.LogInfo($"[MOD] Successfully gave weapon '{weaponName}' (ID {weaponId})");
    }

    private void GrantAbility(string abilityName)
    {
        int abilityId = GetAbilityId(abilityName);

        if (abilityId < 0)
        {
            Logger.LogError($"[MOD] Unknown ability: {abilityName}");
            return;
        }

        var player = Singleton<PlayerBase>.Instance;
        if (player == null)
        {
            Logger.LogError("[MOD] PlayerBase.Instance is NULL! Player not loaded?");
            return;
        }

        var status = player._Status;
        if (status == null)
        {
            Logger.LogError("[MOD] player._Status is NULL!");
            return;
        }

        status.SetAbility(abilityId, true);

        Logger.LogInfo($"[MOD] Successfully gave weapon '{abilityName}' (ID {abilityId})");
    }

    private void TeleportTo(string location)
    {
        var mover = Singleton<TeleportMover>.Instance;
        if (mover == null)
        {
            Logger.LogError("[MOD] TeleportMover.Instance is NULL! Teleport system not loaded?");
            return;
        }

        try
        {
            mover.MoveTeleport(0, location);

            Logger.LogInfo($"[MOD] Successfully teleported to {location}");
        }
        catch (System.Exception ex)
        {
            Logger.LogError($"[MOD] Teleport failed: {ex}");
        }
    }

    private void ResetWeapons()
    {
        var ps = Singleton<PlayerBase>.Instance;
        if (ps == null)
        {
            Logger.LogError("[MOD] PlayerStatusData is NULL!");
            return;
        }

        var status = ps._Status;
        if (status == null)
        {
            Logger.LogError("[MOD] player._Status is NULL!");
            return;
        }

        status.ResetWeapon();
        Logger.LogInfo("Weapons Reset");

    }

    private void ResetAbilities()
    {
        var ps = Singleton<PlayerBase>.Instance;
        if (ps == null)
        {
            Logger.LogError("[MOD] PlayerStatusData is NULL!");
            return;
        }

        var status = ps._Status;
        if (status == null)
        {
            Logger.LogError("[MOD] player._Status is NULL!");
            return;
        }

        status.ResetAbility();
        Logger.LogInfo("Abilities Reset");
    }
}
