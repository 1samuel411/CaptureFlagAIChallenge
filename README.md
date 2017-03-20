
<p align="center">
<img src="http://samuelarminana.com/u/38_20_03_2017.png"/>
</p>

# Welcome to the Capture the Flag AI Challenge!

# Documentation
Take a look at our Github Wiki page for documentation

# Judging
Three different tournaments will occur with team sizes set at 1v1, 2v2, and 4v4.

During judging your bots will go through a single-elimination tournament. [More Info](https://www.wikiwand.com/en/Single-elimination_tournament)

# Rules
1. You cannot store instances of objects, this includes but is not limited to, IAgent or Soldier classes and IGrabbable or Flag classes. These classes should not be accessed anywhere out of the callbacks provided.
2. Using or Getting components of the following classes is prohibited, (UIManager, TeamManager, CameraManager, Spawn, Flag, Weapon, Soldier, AnimationController, PoolManager, PoolObject)
3. Custom navigation systems are allowed, however the unity navmesh system is not, use the Grid provided to your advantage. (GridManager)
4. You can only move using the MoveTowards and Move methods from the SoldierWrapper class.
5. You cannot call the DropFlag method on any IAgent that does not belong to you.
6. No bug abusing or exploiting. If you see any bugs please notify me as soon as possible!
7. Other prohibitions are specified in the Wiki.

These rules should be self-explanatory for the most part. If you need to break any of these rules go ahead but they may not be allowed through to compete with others depending on the situtation.

**Note:** The only thing you submit will be your scripts, so editing values of prefabs is fine as they will not be taken into account in the competition.

# Instructions

## Downloading the Project
1. Press 'Clone or download' in the upper right

## Getting Started
1. Open Scenes/level.unity
2. Duplicate the SoldierBase.cs file (ctrl+d) or create a new class that inherits from the SoldierWrapper class
3. Create your bot and use the example as reference and take a look at our Github Wiki page for documentation

## Adding Your bot
1. Expand the Prefabs/Soldier folder
2. Select SoldierA or SoldierB and delete their SoldierExample.cs component
3. Add your custom bot script to the prefab
4. Press play!

## Changing the bot count
1. In the level.unity scene click on the TeamManager object
2. In the TeamManager component, edit the Team Size attribute

## Sending Your Submission
Send your submission directly to me via email: armi.sam99@gmail.com or via skype: arminana.s OR send your submission via hastebin links for all necessary code and post it in the forum post.

**Note:** You should send all files that relate to your entry, so if you have a navigation script, be sure to include that.

<p align="center">
<img src="http://samuelarminana.com/u/30_20_03_2017.png"/>
</p>

# Controls for testing
- WASD - Move Camera
- Q/E - Zoom in/out
- Space - Cycle Spectate
- Right Mouse Btn - Overview
- H - Reset
- G - Restart

# Credits for assets useds
- Denis Sylkin - RTS Camera
- Unity - Unity Particle Pack
- Marcelo Fernanadez Music - Music
- Dogzerx (Jose Diaz) - Cartoon Soldier      (<3 Dogzer)
- Telecaster - Cloth animation based Flag 1.0

# Contact
Email: armi.sam99@gmail.com

Skype: arminana.s

Website: http://samuelarminana.com/
