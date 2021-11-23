Adds 20 new Two Handed Weapons to Valheim. With practical damage numbers to match Valheim.
These Weapons also have their own visual look while on Item Stands.

NOTE!!!
All Weapons from V5.0.0 and under will be lost on updating to 5.0.1 and above, as I renamed all the prefab names, I hope this is the last time I'll never do this, at least on a massive scale such as this. You may want to backup the htd.armory configs just in case you may lose any custom tweaks you made to itemdata or recipedata

As of the re-branding from More Two Handers to Armory as of V5.0.0 old config files should have the "htd.moretwohanders" be renamed to "htd.armory" before launching the game to retain any custom changes you may have done, if not it will just regenerate new configs for Armory, the old ones will still be there to reference but then you'll have to move those values over.


Public Localization File (will Update Embedded files with changes here each update): Google Sheet 
You can also download the file as a .tsv and drop it in the "Valheim\BepInEx\config" path to update it yourself if you do not wish to wait for a the mod to update and then delete your old "htd.armory_localization.tsv" file to regenerate from the embedded data.


Installation:
Move the "ValheimHTDArmory.dll" into your Plugins folder.


Config:
A config file is generated that will allow you to enable/disable the recipes for each one, tweak their damage stats, the Name, and Description (temporary means of localization), and where it is crafted, the min level of the station, and the max level the weapon can be upgraded to.
Can also set a Hotkey to use the weapon's 3rd attack, default is "mouse 3" which would be the 4th button on your mouse if it has such.


Console/Cheats/Configs Item IDS:
Buck and Doe ::: DeerFistsHTD <-(was FistBuckAndDoe)
Bronze Knuckles ::: BronzeFistsHTD <-(was FistBronze)
Iron Knuckles ::: IronFistsHTD <-(was FistIron)

Iron Greatsword ::: IronGreatSwordHTD <-(was SwordIronGreat)
Heavy Iron Great Sword ::: IronHeavyGreatSwordHTD <-(was SwordFlametalGreatIron)
Dragon Slayer ::: DragonSlayerSwordHTD <-(was SwordDragonSlayer)

Great Core Mace ::: CoreGreatMaceHTD <-(was MaceCoreGreat)
Great Core Mace Green ::: CoreGreatMaceGreenHTD <-(was MaceCoreGreatGreen)
Great Core Mace Blue ::: CoreGreatMaceBlueHTD

Silver Greatsword ::: SilverGreatSwordHTD <-(was SwordSilverGreat)
Great Silver Mace ::: SilverGreatMaceHTD <-(was MaceSilverGreat)
Silver Battleaxe ::: SilverBattleaxeHTD <-(was AxeSilverBattle)

Black Metal Greatsword ::: BlackMetalGreatSwordHTD <-(was SwordBlackMetalGreat)
Black Metal Greatsword Foreign ::: BlackMetalGreatSwordAltHTD <-(was SwordIronGreatBlack)
Black Metal Battleaxe ::: BlackMetalBattleaxeHTD <-(was AxeBlackMetalBattle)

Grasp of Undying ::: BoneGreatMaceHTD <-(was MaceGraspUndying)
Body of the Devoted ::: BoneGreatSwordHTD <-(was SwordBoneGreat)

Moder's Sorrow ::: ObsidianGreatSwordHTD <-(was SwordObsidianGreat)
Moder's Sorrow Red ::: ObsidianGreatSwordRedHTD
Flametal Greatsword ::: FlametalGreatSwordHTD <-(was SwordFlametalGreat)

ChangeLog:
V 5.0.0 => V 5.0.1
-Renamed ALL Prefab names, effectivly "deleting" all weapons from saves, sorry.
-Updated to latest Valheim (Hearth and Home)
-Updated ServerSync (by Blaxxun) this is responsible for well syncing config data from server to clients
-Fixed all visual errors with maces and axes (was rotated oddly since H&H dropped)
-Revamped and added visual effects for various weapons
-Added a Red version of Moder's Sorrow
-Added a Blue version of the Great Core Mace (now you can have a mace for each color of standing iron torches)
-Made some Two Sided Shaders, will play a major role later, but currently just used for fist weapons so you can't see through them when viewing them the right way
-Fist Weapons now use the Attach_Override of "Hands" this should make them work with the Monk class from Valheim Legends, currently this means they're not attached to your hip like before when sheathed (for now)
-Tweaked all colliders, don't think they were an issue before, but they are redone.

V 4.2.2 => V 5.0.0
-Renamed Mod and .DLL to "ValheimHTDArmory"
-Added a Black Metal Great Sword variant that looks like old one (wavy flamburge sword)
-Added 3 Fist weapons (2 are the same model, different color Bronze and Iron. 1 is unique starting fist weapon with special properties)
-Added Great Bone Sword, special ability of a constant but weak hp regen, is indestructible
-Grasp of Undying has regeneration and indestructible, until I can give it a proper unique effect
-Added more values to edit in the itemData config.

V 4.2.1 => V 4.2.2:
-Fixed a Newline Break in the Great Core Mace Green Description of the Localization File (line 19), so the embedded file shouldn't break the mod anymore. If your .tsv is still breaking the game, look for any line breaks and remove them and save the file.

V 4.2.0 => V 4.2.1:
-Fixed Bug where if your Language didn't have a translation, there would be no name or description for weapons. Defaults to English if that's the case.
-Updated Embedded Localization file with Russian Translation from the Google Sheet 
- _localization.cfg will be automatically converted to .tsv (no local changes you have made will be lost, this is a safe conversion so you can just download the Google Sheet as a .tsv and overwrite yourself if there are any updates there)

V 4.1.0 => V 4.2.0:
-Added Localization Support (_Localization) found in the config location, does not Sync to clients.

V 4.0.0 => V 4.1.0:
-Configs will sync to users when joining a server (_ItemData, _RecipeData) Thanks to Blaxxun
-Silver Great Sword has slimmed down and texture enhanced
-Iron Great Sword has slimmed down and texture enhanced

V 3.0.2 => V 4.0.0:
-(The Bad) Item Losses:
--Great Frost Mace will be lost
--Great Toxic Mace will be a Green Version of the Great Core Mace
--Black Metal Great Sword will be lost
-(The Good):
--New Weapon and Recipe Configs (recommended to delete all htd.moretwohanders.cfgs before starting for a fresh generation)
--Iron Great Sword added (looks like old Black Metal Sword)
--Dragon Slayer from Berserk added
--Iron Version of the Flametal Sword added
-(The Ugly)
--Black Metal Great Axe texture cleaned up.
--Black Metal Great Sword has a new look.
--Flametal Sword model and texture updated.

V 3.0.1 => V 3.0.2:
-Fixed issue of the weapons being indestructible. They will function like all weapons do now, using durability.

V 3.0 => V 3.0.1:
-Possibly Fixed Visual errors with Vulkan Mode, Those that like or have to run with Vulkan rejoice(maybe)
-Fixed Critcal Issue with Great Maces, Secondary and Third Attacks were missing the setting to hit more than one enemy. Which resulted in either hitting the ground and doing nothing, or just one target (which makes no sense for AoE like attacks)

V 2.0 => V 3.0:
-Added Silver Battleaxe
-Added Black Metal Battleaxe
-Updated All weapon textures
-Parry Multipiers on Two Handers are now 3.5 from 4
-Validated that Silver and Black Metal Sword is -10% mov speed, axes are -15%, and heavier ones are -20%
-Adjusted Damage numbers with some more math
-Implemented Hotkey, and Function for doing a 3rd attack for all 9 weapons.
